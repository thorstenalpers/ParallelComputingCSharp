using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelComputing
{
    class Program
    {
        private const string durationTimeFormat = @"hh\:mm\:ss\.ff";
        private const int numberOfComputingLoops = 10000000;
        private const int numberOfComputingTasks = 20;

        static void Main()
        {
            string elapsedTime;
            Console.WriteLine("This example demonstrates the performance advantage of parralel computing");
            Console.WriteLine($"* numberOfComputingLoops={numberOfComputingLoops}");
            Console.WriteLine($"* numberOfComputingTasks={numberOfComputingTasks}");
            Console.WriteLine($"-------------------------------");

            elapsedTime = ComputeSynchronously();
            Console.WriteLine($"ComputeSynchronously - elapsedTime={elapsedTime}, format={durationTimeFormat}");

            elapsedTime = ComputeAsynchronouslySequaential().GetAwaiter().GetResult();
            Console.WriteLine($"ComputeAsynchronouslySequaential - elapsedTime={elapsedTime}, format={durationTimeFormat}");

            elapsedTime = ComputeAsynchrounslyParallel().GetAwaiter().GetResult();
            Console.WriteLine($"ComputeAsynchrounslyParallel - elapsedTime={elapsedTime}, format={durationTimeFormat}");
        }

        static string ComputeSynchronously()
        {
            long total = 0;
            var stopwatch = Stopwatch.StartNew();
            for(int i = 0; i < numberOfComputingTasks; i++)
            {
                total += DoSomethingVeryLong();
            }
            stopwatch.Stop();
            Console.WriteLine($"DummyResult={total}");
            return stopwatch.Elapsed.ToString(durationTimeFormat);
        }

        static async Task<string> ComputeAsynchronouslySequaential()
        {
            long total = 0;
            var results = new List<long>();

            var stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < numberOfComputingTasks; i++)
            {
                total += await Task.Run(() =>
                {
                    return DoSomethingVeryLong();
                });
            }
            stopwatch.Stop();

            Console.WriteLine($"DummyResult={total}");
            return stopwatch.Elapsed.ToString(durationTimeFormat);
        }

        static async Task<string> ComputeAsynchrounslyParallel()
        {
            long total = 0;
            var tasks = new List<Task<long>>();

            var stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < numberOfComputingTasks; i++)
            {
                tasks.Add(Task.Run(() =>
                {
                    return DoSomethingVeryLong();
                }));
            }
            await Task.WhenAll(tasks);
            foreach (var task in tasks)
            {
                total += task.Result;
            }
            Console.WriteLine($"DummyResult={total}");

            stopwatch.Stop();    
            return stopwatch.Elapsed.ToString(durationTimeFormat);
        }

        static long DoSomethingVeryLong()
        {
            long result = 0;
            for(int i = 0; i < numberOfComputingLoops; i++)
            {
                result += DateTime.Now.Ticks;
            }
            return result;
        }
    }
}
