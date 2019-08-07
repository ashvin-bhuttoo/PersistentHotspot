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
    class Program
    {
        
        static async Task Main(string[] args)
        {
            if(args.Length == 1 && args[0] == "INSTALLER") { Process.Start(Application.ExecutablePath); return; }
            Icon red = new Icon(Assembly.GetExecutingAssembly().GetManifestResourceStream($"PersistentHotspot.resources.red.ico"));
            Icon amber = new Icon(Assembly.GetExecutingAssembly().GetManifestResourceStream($"PersistentHotspot.resources.amber.ico"));
            Icon green = new Icon(Assembly.GetExecutingAssembly().GetManifestResourceStream($"PersistentHotspot.resources.green.ico"));

            NotifyIcon trayIcon = new NotifyIcon();
            trayIcon.Text = "PersistentHotspot loading..";
            trayIcon.Icon = amber;
            trayIcon.Visible = true;

            //Application.Run();

            while (true)
            {
                var connectionProfile = Windows.Networking.Connectivity.NetworkInformation.GetInternetConnectionProfile();
                var tetheringManager = Windows.Networking.NetworkOperators.NetworkOperatorTetheringManager.CreateFromConnectionProfile(connectionProfile);
                if (tetheringManager.TetheringOperationalState == Windows.Networking.NetworkOperators.TetheringOperationalState.On)
                {
                    //Console.WriteLine("Hotspot is on.");
                    trayIcon.Text = "Hotspot is on.";
                    trayIcon.Icon = green;
                }
                else
                {
                    //Console.Write("Hotspot is off, turning that shit on.. ");
                    trayIcon.Text = "Turning on Hotspot...";
                    trayIcon.Icon = amber;
                    var result = (Windows.Networking.NetworkOperators.NetworkOperatorTetheringOperationResult) await tetheringManager.StartTetheringAsync();
                    switch(result.Status)
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
                System.Threading.Thread.Sleep(10000); //wait 10s
            }           
        }
    }
}
