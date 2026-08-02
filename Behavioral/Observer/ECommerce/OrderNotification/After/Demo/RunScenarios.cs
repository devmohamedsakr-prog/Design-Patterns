using System;

namespace OrderNotification.After.Demo
{
    /// <summary>
    /// RunScenarios: Interactive menu for Observer pattern demos
    /// </summary>
    class RunScenarios
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("════════════════════════════════════════════════════════════════");
                Console.WriteLine("  Observer Pattern - Order Notification System");
                Console.WriteLine("  Scenario Menu");
                Console.WriteLine("════════════════════════════════════════════════════════════════\n");

                Console.WriteLine("Select a scenario to run:\n");
                Console.WriteLine("1. Single Observer (Email Only)");
                Console.WriteLine("2. Multiple Observers (All Channels)");
                Console.WriteLine("3. Complete Order Flow (Process → Ship → Deliver)");
                Console.WriteLine("4. Dynamic Subscription/Unsubscription");
                Console.WriteLine("0. Exit\n");

                Console.Write("Enter scenario number: ");
                string input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        Console.WriteLine("\nLaunching Scenario 1...\n");
                        System.Threading.Thread.Sleep(500);
                        Scenario1_SingleObserver.Main(null);
                        break;

                    case "2":
                        Console.WriteLine("\nLaunching Scenario 2...\n");
                        System.Threading.Thread.Sleep(500);
                        Scenario2_MultipleObservers.Main(null);
                        break;

                    case "3":
                        Console.WriteLine("\nLaunching Scenario 3...\n");
                        System.Threading.Thread.Sleep(500);
                        Scenario3_CompleteOrderFlow.Main(null);
                        break;

                    case "4":
                        Console.WriteLine("\nLaunching Scenario 4...\n");
                        System.Threading.Thread.Sleep(500);
                        Scenario4_DynamicSubscription.Main(null);
                        break;

                    case "0":
                        Console.WriteLine("Exiting...");
                        return;

                    default:
                        Console.WriteLine("Invalid option. Press any key to try again...");
                        Console.ReadKey();
                        break;
                }

                Console.WriteLine("\n\nPress any key to return to menu...");
                Console.ReadKey();
            }
        }
    }
}
