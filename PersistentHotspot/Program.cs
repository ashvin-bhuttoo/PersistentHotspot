using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace PersistentHotspot
{
    static class Program
    {
        private static bool userToggle = false;
        private static NotifyIcon trayIcon = new NotifyIcon();
        private static Stopwatch timeWatch = new Stopwatch();
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
            if (args.Length == 1 && args[0] == "INSTALLER") {
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
                FileSystemAccessRule fsar = new FileSystemAccessRule("Users", FileSystemRights.FullControl, AccessControlType.Allow);
                DirectorySecurity ds = null;

                ds = di.GetAccessControl();
                ds.AddAccessRule(fsar);
                di.SetAccessControl(ds);

                Process.Start(Application.ExecutablePath); 
                return; 
            }

            Thread.Sleep(2000);
            Process[] runningProcesses = Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName);
            if (runningProcesses.Length == 1 || (args.Length == 1 && args[0] == "OVERRIDE_PROCESS_CHECK")) // if its just me or OVERRIDE is set, let me run!
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

                if (Reg.auto_restart_hotspot > 0)
                    timeWatch.Restart();

                //Monitor hotspot thread
                var task = MonitorHotspot();
                HandleException(task);

                Updater.Run("PersistentHotspot");
                Application.Run();
            }           
        }

        private static async Task MonitorHotspot()
        {
            int badConnProfileCtr = 0;
            while (true)
            {
                try
                {
                    var connectionProfile = Windows.Networking.Connectivity.NetworkInformation.GetInternetConnectionProfile();

                    //Fix issue with GetInternetConnectionProfile() returning null (Windows API issue)
                    if (connectionProfile == null)
                    {
                        if(++badConnProfileCtr > 2) 
                            break;

                        throw new Exception("BAD_INETCONN_PROFILE");
                    }

                    var tetheringManager = Windows.Networking.NetworkOperators.NetworkOperatorTetheringManager.CreateFromConnectionProfile(connectionProfile);
                    if (tetheringManager.TetheringOperationalState == Windows.Networking.NetworkOperators.TetheringOperationalState.On)
                    {
                        trayIcon.Icon = green;

                        if (Reg.stay_online)
                        {
                            if(Reg.auto_restart_hotspot > 0)
                            {
                                if(timeWatch.ElapsedMilliseconds/60000 >= Reg.auto_restart_hotspot)
                                {
                                    trayIcon.Text = "Restarting Hotspot...";
                                    trayIcon.Icon = amber;
                                    var result = await tetheringManager.StopTetheringAsync();
                                    timeWatch.Restart();
                                }
                                else
                                {
                                    var time_secs = Reg.auto_restart_hotspot*60 - timeWatch.ElapsedMilliseconds / 1000;
                                    trayIcon.Text = $"Hotspot is on.{(Reg.auto_restart_hotspot > 0 ? $" AutoRestart in {(time_secs > 60 ? $"{time_secs/60} mins {time_secs - ((time_secs/60)*60)} secs" : $"{time_secs} secs")}." : string.Empty)}";
                                }
                            }
                            else
                            {
                                trayIcon.Text = "Hotspot is on.";
                            }
                        }
                        else
                        {
                            trayIcon.Text = "Turning off Hotspot...";
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

                        if (Reg.stay_online)
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
                    trayIcon.Text = e.Message == "BAD_INETCONN_PROFILE" ? trayIcon.Text : $"Unknown Failure! Retrying..";
                    trayIcon.Icon = e.Message == "BAD_INETCONN_PROFILE" ? amber: red;
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

            //Fix issue with GetInternetConnectionProfile() returning null (Windows API issue)
            trayIcon.Visible = false;
            Process.Start(Application.ExecutablePath, "OVERRIDE_PROCESS_CHECK");
            Application.Exit();
            Environment.Exit(Environment.ExitCode);
        }
        
        #region tray icon mode selection
        private static void stayOnlineToggle_Click(object sender, EventArgs e)
        {
            Reg.stay_online = !Reg.stay_online;
            userToggle = true;
        }

        private static void autoRestartToggle_Click(object sender, EventArgs e)
        {
            if (Reg.auto_restart_hotspot == 0)
                new frmAutoRestartHS().ShowDialog();
            else            
                Reg.auto_restart_hotspot = 0;

            if (Reg.auto_restart_hotspot == 0)
                timeWatch.Stop();
            else
                timeWatch.Restart();         

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
            trayIcon.ContextMenuStrip.Items.Add(ToolStripMenuItemWithHandler(Reg.stay_online ? "Turn Off" : "Turn On", stayOnlineToggle_Click));
            trayIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());
            //Implement Feature Request: Cycle on/off status at custom interval #2
            trayIcon.ContextMenuStrip.Items.Add(ToolStripMenuItemWithHandler(Reg.auto_restart_hotspot > 0 ? "Disable Hotspot Auto Restart" : "Enable Hotspot Auto Restart", autoRestartToggle_Click));
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
            catch (Exception)
            {
                trayIcon.Visible = false;
                Application.Exit();
                Environment.Exit(Environment.ExitCode);
            }
        }

        public static class Reg
        {
            static Microsoft.Win32.RegistryKey rootKey;

            static Reg()
            {
                rootKey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(@"Software\PersistentHotspot");
            }

            //Implement Feature Request: Cycle on/off status at custom interval #2
            public static long auto_restart_hotspot
            {
                get
                {
                    var defaultValue = 0;
                    var info = System.Reflection.MethodBase.GetCurrentMethod() as System.Reflection.MethodInfo;
                    var name = info.Name.Substring(4);
                    return (dynamic)Convert.ChangeType(rootKey.GetValue(name, defaultValue.ToString()), info.ReturnType);
                }

                set
                {
                    var name = System.Reflection.MethodBase.GetCurrentMethod().Name.Substring(4);
                    rootKey.SetValue(name, value);
                }
            }

            public static bool stay_online
            {
                get
                {
                    var defaultValue = true;
                    var info = System.Reflection.MethodBase.GetCurrentMethod() as System.Reflection.MethodInfo;
                    var name = info.Name.Substring(4);
                    return (dynamic)Convert.ChangeType(rootKey.GetValue(name, defaultValue.ToString()), info.ReturnType);
                }

                set
                {
                    var name = System.Reflection.MethodBase.GetCurrentMethod().Name.Substring(4);
                    rootKey.SetValue(name, value);
                }
            }
        }
        #endregion support methods
    }
}
