using System;

namespace VacationApproval.After.Demo
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
                Console.WriteLine("  HR Vacation Approval - Scenario Menu");
                Console.WriteLine("  Chain of Responsibility Pattern");
                Console.WriteLine("════════════════════════════════════════════════════════════════\n");

                Console.WriteLine("Select a scenario to run:\n");
                Console.WriteLine("1. Simple Vacation Request (5 days)");
                Console.WriteLine("2. Extended Leave (10 days, needs Director)");
                Console.WriteLine("3. High-Cost Request (needs Executive)");
                Console.WriteLine("4. Rejected Request (exceeds limits)");
                Console.WriteLine("5. Urgent Request (Family emergency)");
                Console.WriteLine("6. Dynamic Chain Building");
                Console.WriteLine("0. Exit\n");

                Console.Write("Enter scenario number: ");
                string input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        Console.WriteLine("\nLaunching Scenario 1...\n");
                        System.Threading.Thread.Sleep(500);
                        Scenario1_SimpleVacation.Main(null);
                        break;

                    case "2":
                        Console.WriteLine("\nLaunching Scenario 2...\n");
                        System.Threading.Thread.Sleep(500);
                        Scenario2_ExtendedLeave.Main(null);
                        break;

                    case "3":
                        Console.WriteLine("\nLaunching Scenario 3...\n");
                        System.Threading.Thread.Sleep(500);
                        Scenario3_HighCost.Main(null);
                        break;

                    case "4":
                        Console.WriteLine("\nLaunching Scenario 4...\n");
                        System.Threading.Thread.Sleep(500);
                        Scenario4_Rejected.Main(null);
                        break;

                    case "5":
                        Console.WriteLine("\nLaunching Scenario 5...\n");
                        System.Threading.Thread.Sleep(500);
                        Scenario5_Urgent.Main(null);
                        break;

                    case "6":
                        Console.WriteLine("\nLaunching Scenario 6...\n");
                        System.Threading.Thread.Sleep(500);
                        Scenario6_DynamicChain.Main(null);
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
