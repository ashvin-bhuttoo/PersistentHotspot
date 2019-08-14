using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace PersistentHotspot
{
    static class Program
    {
        private static bool stayOnline = true, userToggle = false;
        private static NotifyIcon trayIcon = new NotifyIcon();
        private static System.ComponentModel.IContainer components;
        private static Icon red = new Icon(Assembly.GetExecutingAssembly().GetManifestResourceStream($"PersistentHotspot.resources.red.ico"));
        private static Icon amber = new Icon(Assembly.GetExecutingAssembly().GetManifestResourceStream($"PersistentHotspot.resources.amber.ico"));
        private static Icon green = new Icon(Assembly.GetExecutingAssembly().GetManifestResourceStream($"PersistentHotspot.resources.green.ico"));

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length == 1 && args[0] == "INSTALLER") { Process.Start(Application.ExecutablePath); return; }

            Process[] runningProcesses = Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName);
            if (runningProcesses.Length == 1) // just me, so run!
            {
                //systray icon config
                components = new System.ComponentModel.Container();
                trayIcon = new NotifyIcon(components)
                {
                    ContextMenuStrip = new ContextMenuStrip(),
                    Icon = amber,
                    Text = "PersistentHotspot loading..",
                    Visible = true
                };
                trayIcon.ContextMenuStrip.Opening += ContextMenuStrip_Opening;
                trayIcon.MouseUp += trayIcon_MouseUp;

                //Monitor hotspot thread
                var task = MonitorHotspot();
                HandleException(task);

                Application.Run();
            }           
        }

        private static async Task MonitorHotspot()
        {
            while (true)
            {
                try
                {
                    var connectionProfile = Windows.Networking.Connectivity.NetworkInformation.GetInternetConnectionProfile();
                    var tetheringManager = Windows.Networking.NetworkOperators.NetworkOperatorTetheringManager.CreateFromConnectionProfile(connectionProfile);
                    var tmp_stayOnline = stayOnline;
                    if (tetheringManager.TetheringOperationalState == Windows.Networking.NetworkOperators.TetheringOperationalState.On)
                    {
                        trayIcon.Icon = green;

                        if (tmp_stayOnline)
                        {
                            trayIcon.Text = "Hotspot is on.";
                        }
                        else
                        {
                            trayIcon.Text = "Turning off Hotspot...";
                            //weird issue with StopTetheringAsync causing GetInternetConnectionProfile to return NULL, need to investigate..
                            var result = (Windows.Networking.NetworkOperators.NetworkOperatorTetheringOperationResult)await tetheringManager.StopTetheringAsync();  
                            switch (result.Status)
                            {
                                case Windows.Networking.NetworkOperators.TetheringOperationStatus.OperationInProgress:
                                    //Console.WriteLine("Operation In Progress..");
                                    trayIcon.Text = "Turning off Hotspot...";
                                    trayIcon.Icon = green;
                                    break;
                                case Windows.Networking.NetworkOperators.TetheringOperationStatus.Success:
                                    //Console.WriteLine("Success!");
                                    trayIcon.Text = "Hotspot is off.";
                                    trayIcon.Icon = amber;
                                    break;

                                case Windows.Networking.NetworkOperators.TetheringOperationStatus.WiFiDeviceOff:
                                    //Console.WriteLine("Failure, WiFi Device Off!");
                                    trayIcon.Text = "Failure, WiFi Device Off!";
                                    trayIcon.Icon = red;
                                    break;

                                default:
                                case Windows.Networking.NetworkOperators.TetheringOperationStatus.Unknown:
                                    //Console.WriteLine("Unknown Failure!");
                                    trayIcon.Text = "Unknown Failure!";
                                    trayIcon.Icon = red;
                                    break;
                            }
                        }
                    }
                    else
                    {
                        trayIcon.Icon = amber;

                        if (tmp_stayOnline)
                        {
                            trayIcon.Text = "Turning on Hotspot...";
                            var result = (Windows.Networking.NetworkOperators.NetworkOperatorTetheringOperationResult)await tetheringManager.StartTetheringAsync();
                            switch (result.Status)
                            {
                                case Windows.Networking.NetworkOperators.TetheringOperationStatus.OperationInProgress:
                                    //Console.WriteLine("Operation In Progress..");
                                    trayIcon.Text = "Turning on Hotspot...";
                                    trayIcon.Icon = amber;
                                    break;
                                case Windows.Networking.NetworkOperators.TetheringOperationStatus.Success:
                                    //Console.WriteLine("Success!");
                                    trayIcon.Text = "Hotspot is on.";
                                    trayIcon.Icon = green;
                                    break;

                                case Windows.Networking.NetworkOperators.TetheringOperationStatus.WiFiDeviceOff:
                                    //Console.WriteLine("Failure, WiFi Device Off!");
                                    trayIcon.Text = "Failure, WiFi Device Off!";
                                    trayIcon.Icon = red;
                                    break;

                                default:
                                case Windows.Networking.NetworkOperators.TetheringOperationStatus.Unknown:
                                    //Console.WriteLine("Unknown Failure!");
                                    trayIcon.Text = "Unknown Failure!";
                                    trayIcon.Icon = red;
                                    break;
                            }
                        }
                        else
                        {
                            trayIcon.Text = "Hotspot is off.";
                        }
                    }
                }
                catch (Exception e)
                {
                    trayIcon.Text = $"Unknown Failure!";
                    //MessageBox.Show(e.Message);
                    trayIcon.Icon = red;
                };

                //wait 10s
                for (int k = 0; k < 100 && !userToggle; k++)
                {
                    System.Threading.Thread.Sleep(100);
                    if(!userToggle)
                        Application.DoEvents();
                }

                userToggle = false;
            }
        }
        
        #region tray icon mode selection
        private static void switchMode_Click(object sender, EventArgs e)
        {
            stayOnline = !stayOnline;
            userToggle = true;
        }

        private static void exit_Click(object sender, EventArgs e)
        {
            trayIcon.Visible = false;
            Application.Exit();
            Environment.Exit(Environment.ExitCode);
        }

        private static void trayIcon_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
            {
                MethodInfo mi = typeof(NotifyIcon).GetMethod("ShowContextMenu", BindingFlags.Instance | BindingFlags.NonPublic);
                mi.Invoke(trayIcon, null);
            }
        }

        private static void ContextMenuStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = false;
            trayIcon.ContextMenuStrip.Items.Clear();
            trayIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());
            trayIcon.ContextMenuStrip.Items.Add(ToolStripMenuItemWithHandler(stayOnline ? "Turn Off" : "Turn On", switchMode_Click));
            trayIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());
            trayIcon.ContextMenuStrip.Items.Add(ToolStripMenuItemWithHandler("Exit", exit_Click));
            trayIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());
        }
        #endregion

        #region support methods
        private static ToolStripMenuItem ToolStripMenuItemWithHandler(string displayText, int enabledCount, int disabledCount, EventHandler eventHandler)
        {
            var item = new ToolStripMenuItem(displayText);
            if (eventHandler != null) { item.Click += eventHandler; }

            item.ToolTipText = (enabledCount > 0 && disabledCount > 0) ? $"{enabledCount} enabled, {disabledCount} disabled"
                : (enabledCount > 0) ? string.Format("{0} enabled", enabledCount)
                    : (disabledCount > 0) ? string.Format("{0} disabled", disabledCount)
                        : "";
            return item;
        }

        public static ToolStripMenuItem ToolStripMenuItemWithHandler(string displayText, EventHandler eventHandler)
        {
            return ToolStripMenuItemWithHandler(displayText, 0, 0, eventHandler);
        }

        private static async void HandleException(Task task)
        {
            try
            {
                await Task.Yield();
                await task;
            }
            catch (Exception e)
            {
                //Console.WriteLine(e);
                //Application.ExitThread();
                trayIcon.Visible = false;
                Application.Exit();
                Environment.Exit(Environment.ExitCode);
            }
        }
        # endregion support methods
    }
}
