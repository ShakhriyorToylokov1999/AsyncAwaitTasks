/*
* Study the code of this application to calculate the sum of integers from 0 to N, and then
* change the application code so that the following requirements are met:
* 1. The calculation must be performed asynchronously.
* 2. N is set by the user from the console. The user has the right to make a new boundary in the calculation process,
* which should lead to the restart of the calculation.
* 3. When restarting the calculation, the application should continue working without any failures.
*/

using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncAwait.Task1.CancellationTokens;

internal class Program
{
    /// <summary>
    /// The Main method should not be changed at all.
    /// </summary>
    /// <param name="args"></param>
    private static void Main(string[] args)
    {
        Console.WriteLine("Mentoring program L2. Async/await.V1. Task 1");
        Console.WriteLine("Calculating the sum of integers from 0 to N.");
        Console.WriteLine("Use 'q' key to exit...");
        Console.WriteLine();

        Console.WriteLine("Enter N: ");

        var input = Console.ReadLine();
        while (input.Trim().ToUpper() != "Q")
        {
            
            if (int.TryParse(input, out var n))
            {
                CalculateSum(n);
            }
            else
            {
                Console.WriteLine($"Invalid integer: '{input}'. Please try again.");
                Console.WriteLine("Enter N: ");
            }

            input = Console.ReadLine();
        }

        Console.WriteLine("The Application has been stopped");
    }

    private static void CalculateSum(int n)
    {
        CancellationTokenSource tokenSource = new CancellationTokenSource();
        var token = tokenSource.Token;
        try
        {
            
            var sum = Calculator.CalculateAsync(n, token);
            Thread t1 = new Thread(() => {
                if (Console.ReadKey(true).KeyChar.ToString().ToUpperInvariant().Equals("N"))
                {
                    tokenSource.Cancel();
                }
            });
            t1.Start();
            Console.WriteLine($"The task for {n} started... Enter N to cancel the request:");

            Console.WriteLine($"Sum for {n} = {sum.Result}.");
            Console.WriteLine();
            Console.WriteLine("Enter N: ");
            
        }
        catch (AggregateException ex)
        {
            foreach (var e in ex.InnerExceptions)
                if (e is TaskCanceledException)
                {

                    Console.WriteLine($"Sum for {n} cancelled...");
                    Console.WriteLine("Enter another value to continue: ");
                }
                else
                {
                    Console.WriteLine("Unexpected error occured. Error message: ");
                    Console.WriteLine("   {0}: {1}", e.GetType().Name, e.Message);
                }
                
        }
        finally
        {
            tokenSource.Dispose();
        }
        
    }
}
