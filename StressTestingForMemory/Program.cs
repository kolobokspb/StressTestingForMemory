using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace StressTesting;

public static class Sort
{
    public static class Bubble
    {
        public static void Run(int[] arr, int n)
        {
            for (var i = 0; i < n - 1; i++)
            {
                for (var j = 0; j < n - i - 1; j++)
                {
                    if (arr[j] > arr[j + 1])
                        (arr[j], arr[j + 1]) = (arr[j + 1], arr[j]);
                }
            }
        }
    }

    public static class Qsort
    {
        private static int Partition(int[] arr, int low, int high)
        {
            var pivot = arr[high];
            var i = low - 1;

            for (var j = low; j <= high - 1; j++)
            {
                if (arr[j] >= pivot) 
                    continue;
                i++;
                (arr[i], arr[j]) = (arr[j], arr[i]);
            }

            (arr[i + 1], arr[high]) = (arr[high], arr[i + 1]);
            return i + 1;
        }

        private static void QuickSort(int[] arr, int low, int high)
        {
            if (low >= high)
                return;

            var pi = Partition(arr, low, high);

            QuickSort(arr, low, pi - 1);
            QuickSort(arr, pi + 1, high);
        }

        public static void Run(int[] arr, int n)
        {
            QuickSort(arr, 0, n - 1);
        }
    }

    public static class Shell
    {
        public static void Run(int[] arr, int n)
        {
            for (var gap = n / 2; gap > 0; gap /= 2)
            {
                for (var i = gap; i < n; i++)
                {
                    var temp = arr[i];
                    int j;

                    for (j = i; j >= gap && arr[j - gap] > temp; j -= gap)
                    {
                        arr[j] = arr[j - gap];
                    }
                    arr[j] = temp;
                }
            }
        }
    }

    public static class Selection
    {
        public static void Run(int[] arr, int n)
        {
            for (var i = 0; i < n - 1; i++)
            {
                var minIndex = i;
                for (var j = i + 1; j < n; j++)
                {
                    if (arr[j] < arr[minIndex])
                    {
                        minIndex = j;
                    }
                }

                (arr[i], arr[minIndex]) = (arr[minIndex], arr[i]);
            }
        }
    }
}

public static class Memory
{
    public static class SGen
    {
        public static void Run(CancellationToken cancellationToken)
        {
            while (true)
            {
                var poolArrayGen1 = new byte[64 * 1024][];
                
                for (var i = 0; i < 1024; i++)
                {
                    if(cancellationToken.IsCancellationRequested)
                        return;
                    
                    var arrayGen1 = new byte[64 * 1024];
                    poolArrayGen1[i] = arrayGen1;
                    Thread.Sleep(10);
                }
            }
        }
    }
    
    public static class LGen
    {
        public static void Run(CancellationToken cancellationToken)
        {
            while (true)
            {                    
                if(cancellationToken.IsCancellationRequested)
                    return;
                
                var array = new byte[64*1024];
                
                GetSize(array);
                
                Thread.Sleep(10);
            }
        }
    }
    
    public static class PGen
    {
        public static void Run(CancellationToken cancellationToken)
        {
            while (true)
            {                    
                if(cancellationToken.IsCancellationRequested)
                    return;

                var array = GC.AllocateArray<byte>( 64 * 1024, true);

                GetSize(array);
                
                Thread.Sleep(10);
            }
        }
    }
    
    public static class FGen
    {
        public static void Run(CancellationToken cancellationToken)
        {
            while (true)
            {                    
                if(cancellationToken.IsCancellationRequested)
                    return;

                var s = string.Empty;
                for (var i = 0; i < 1000; i++)
                {
                    s += i;
                }
                
                Thread.Sleep(10);
            }
        }
    }

    private static int GetSize(byte[] array)
    {
        /*
        var sum = 0;
        //var value = 0;
        for (var i = 0; i < array.Length; i++)
        {
            sum += array[i];
        }
        */
        return array.Length;
    }
}

internal static class Program
{
    
    
    private static void Main()
    {
        //Console.WriteLine($"This example show 4 types sorting (bubble, qsort, shell, selection) for {sizeArray} elements");
        Console.WriteLine("Run...");
        
        var cancelTokenSource = new CancellationTokenSource();
        
        var sTask = Task.Run(() =>
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            Memory.SGen.Run(cancelTokenSource.Token);
            stopWatch.Stop();
        });
        
        var lTask = Task.Run(() =>
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            Memory.LGen.Run(cancelTokenSource.Token);
            stopWatch.Stop();
        });
        
        var pTask = Task.Run(() =>
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            Memory.PGen.Run(cancelTokenSource.Token);
            stopWatch.Stop();
        });
        
        var fTask = Task.Run(() =>
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            Memory.FGen.Run(cancelTokenSource.Token);
            stopWatch.Stop();
        });
        
        Console.WriteLine("Press any key to close");
        
        Console.ReadKey();  
        cancelTokenSource.Cancel();
        
        Task.WaitAll(sTask, lTask, pTask, fTask);
    }
}