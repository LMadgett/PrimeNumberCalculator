using Microsoft.VisualBasic;
using System;
using System.Diagnostics;

namespace PrimeNumberCalculator
{
    internal class Program 
    { 
        static int maxTasks = 10;  
        static bool testLastDigits = false;
        static List<Task<PrimeTaskParams>> tasks = new List<Task<PrimeTaskParams>>();

        static void Main(string[] args)
        {
            Stopwatch stopwatch = new Stopwatch();

            int maxPwr10 = 7;
            List<long> primes = new List<long>();
            stopwatch.Start();

            int currentBlock = 0;
            int blockSize = 10000;
            int saveSize = 10000;
            long maxBlock = (long)(MathF.Pow(10, maxPwr10)) / blockSize;
            Console.WriteLine("maxBlock=" + maxBlock);
            string docPath = "E:\\Coding\\CSharp\\PrimeNumberCalculator";
            string fileName = "primesUpTo10^" + maxPwr10 + ".txt";
            string primeDataFileName = "primesDataUpTo10^" + maxPwr10 + ".csv";
            StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, fileName));
            StreamWriter primeData = new StreamWriter(Path.Combine(docPath, primeDataFileName));

            while (currentBlock < maxBlock || tasks.Count > 0)
            {
                if(tasks.Count < maxTasks && currentBlock < maxBlock)
                {
                    PrimeTaskParams primeTaskParams = new PrimeTaskParams(currentBlock * blockSize, (currentBlock * blockSize) + blockSize - 1);
                    currentBlock++;

                    Task<PrimeTaskParams> primeTask = new Task<PrimeTaskParams>((Object obj) =>
                                                                            {
                                                                                PrimeTaskParams data = obj as PrimeTaskParams;
                                                                                if (data == null) return null;
                                                                                return ComputePrimes(data);
                                                                            }, primeTaskParams);
                    tasks.Add(primeTask);
                    primeTask.Start();
                }

                // look at each currently running primeTask to see if they've finished
                // note we go BACKWARDS through the list so we can remove them as we need
                // - if we went forwards then the counting would break as we removed tasks
                for (int i = tasks.Count - 1; i >= 0; i--)
                {
                    Task<PrimeTaskParams> primeTask = tasks[i];

                    if(primeTask != null && primeTask.IsCompleted)
                    {
                        PrimeTaskParams result = primeTask.Result;

                        string lastDigits = null;

                        if (testLastDigits)
                        {
                            int num1s = 0;
                            int num3s = 0;
                            int num7s = 0;
                            int num9s = 0;

                            foreach (long prime in result.primes)
                            {
                                long r = prime % 10;
                                if (r == 1) num1s++;
                                else if (r == 3) num3s++;
                                else if (r == 7) num7s++;
                                else if (r == 9) num9s++;
                            }

                            lastDigits = "," + num1s + "," + num3s + "," + num7s + "," + num9s;
                        }

                        primeData.WriteLine(result.minNum + "," + result.maxNum + "," + result.primes.Count + "," + result.time + lastDigits);
                        primes.AddRange(result.primes);
                        tasks.RemoveAt(i);
                    }
                }

                if(primes.Count >= saveSize)
                {
                    foreach (long prime in primes)
                        outputFile.WriteLine(prime);
                    Console.WriteLine(MathF.Round(primes[primes.Count - 1] / MathF.Pow(10, maxPwr10) * 100) + "% done");
                    primes.Clear();
                }
            }

            foreach (long prime in primes)
                outputFile.WriteLine(prime);
            if(primes.Count > 0) Console.WriteLine(primes[primes.Count - 1]);
            primes.Clear();

            stopwatch.Stop();
            TimeSpan timeSpan = stopwatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds,
                timeSpan.Milliseconds / 10);
            Console.WriteLine("RunTime " + elapsedTime);
            outputFile.Close();
            primeData.Close();
        }

        static PrimeTaskParams ComputePrimes(PrimeTaskParams taskParams)
        {
            Stopwatch timer = new Stopwatch();
            long min = taskParams.minNum;
            long max = taskParams.maxNum;

            timer.Start();
            for(long i = min; i <= max; i++) 
            {
                bool isPrime = IsPrime(i);
                if(isPrime)
                {
                    taskParams.primes.Add(i);
                }
            }
            timer.Stop();
            float timeForChunk = timer.ElapsedMilliseconds;
            taskParams.time = timeForChunk;

            return taskParams;
        }

        static bool IsPrime(long n)
        {
            if (n < 2) return false;
            else if (n == 2) return true;
            else if ((n % 2) == 0) return false;
            else
            {
                for (int i = 3; i < MathF.Truncate(MathF.Sqrt(n)) + 1; i += 2)
                {
                    if(n % i == 0) return false;
                }
                return true;
            }
        }
    }
}
