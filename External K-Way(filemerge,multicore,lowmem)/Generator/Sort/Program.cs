using System;
using Sort;


class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("no file specified");
            return;
        }

        Console.WriteLine("sorting " + args[0]);
        SortMerge srt = new SortMerge(args[0]);
        Console.WriteLine(String.Format("complete in {0:F1}sec {2:F1}mb/s. see result {1}", srt.CompleteTime.TotalSeconds, srt.FileResult, srt.AverageSpeed));

        Console.ReadLine();
    }
}

