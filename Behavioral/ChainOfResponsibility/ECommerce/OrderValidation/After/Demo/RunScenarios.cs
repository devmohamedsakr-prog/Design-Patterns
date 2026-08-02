using System;

namespace OrderValidation.After.Demo
{
    /// <summary>
    /// RunScenarios: Menu to run individual scenario demos
    /// </summary>
    class RunScenarios
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("════════════════════════════════════════════════════════════════");
                Console.WriteLine("  Order Validation - Scenario Menu");
                Console.WriteLine("  Chain of Responsibility Pattern");
                Console.WriteLine("════════════════════════════════════════════════════════════════\n");

                Console.WriteLine("Select a scenario to run:\n");
                Console.WriteLine("1. Simple Validation Chain (Inventory + Payment)");
                Console.WriteLine("2. Complete Validation Chain (All 4 validators)");
                Console.WriteLine("3. Validation Fails at Fraud Detection");
                Console.WriteLine("4. Different Validator Order (Fraud First)");
                Console.WriteLine("0. Exit\n");

                Console.Write("Enter scenario number: ");
                string input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        Console.WriteLine("\nLaunching Scenario 1...\n");
                        System.Threading.Thread.Sleep(500);
                        Scenario1_SimpleValidation.Main(null);
                        break;

                    case "2":
                        Console.WriteLine("\nLaunching Scenario 2...\n");
                        System.Threading.Thread.Sleep(500);
                        Scenario2_CompleteChain.Main(null);
                        break;

                    case "3":
                        Console.WriteLine("\nLaunching Scenario 3...\n");
                        System.Threading.Thread.Sleep(500);
                        Scenario3_FailsAtFraud.Main(null);
                        break;

                    case "4":
                        Console.WriteLine("\nLaunching Scenario 4...\n");
                        System.Threading.Thread.Sleep(500);
                        Scenario4_DifferentOrder.Main(null);
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
