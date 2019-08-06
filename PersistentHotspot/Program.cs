using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersistentHotspot
{
    class Program
    {
        static async Task Main(string[] args)
        {
            while(true)
            {
                var connectionProfile = Windows.Networking.Connectivity.NetworkInformation.GetInternetConnectionProfile();
                var tetheringManager = Windows.Networking.NetworkOperators.NetworkOperatorTetheringManager.CreateFromConnectionProfile(connectionProfile);
                if (tetheringManager.TetheringOperationalState == Windows.Networking.NetworkOperators.TetheringOperationalState.On)
                {
                    Console.WriteLine("Hotspot is on.");
                }
                else
                {
                    Console.Write("Hotspot is off, turning that shit on.. ");
                    var result = (Windows.Networking.NetworkOperators.NetworkOperatorTetheringOperationResult) await tetheringManager.StartTetheringAsync();
                    switch(result.Status)
                    {
                        case Windows.Networking.NetworkOperators.TetheringOperationStatus.OperationInProgress:
                            Console.WriteLine("Operation In Progress..");
                            break;
                        case Windows.Networking.NetworkOperators.TetheringOperationStatus.Success:
                            Console.WriteLine("Success!");
                            break;

                        case Windows.Networking.NetworkOperators.TetheringOperationStatus.WiFiDeviceOff:
                            Console.WriteLine("Failure, WiFi Device Off!");
                            break;

                        default:
                        case Windows.Networking.NetworkOperators.TetheringOperationStatus.Unknown:
                            Console.WriteLine("Unknown Failure!");
                            break;
                    }
                }
                System.Threading.Thread.Sleep(10000); //wait 10s
            }           
        }
    }
}
