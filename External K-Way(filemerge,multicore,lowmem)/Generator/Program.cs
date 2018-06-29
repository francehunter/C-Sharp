using System;
using System.IO;
using System.Text;


class Program
{
    /// <summary>args: 0 - max string size, 1 - strings count</summary>
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            PrintHelp();
            return;
        }

        try
        {
            int maxLen = Int32.Parse(args[0]);
            int count = Int32.Parse(args[1]);

            Console.WriteLine(String.Format("generating file with {1} strings and {0} max lenght..", count, maxLen));
            string fname = Generate(maxLen, count);
            Console.WriteLine("generated " + fname);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }            

        Console.ReadLine();
    }

        
    static string Generate(int maxLen, int count)
    {
        string fname = "L" + maxLen + "-C" + count + ".txt";
        if (File.Exists(fname))
            fname = "L" + maxLen + "-C" + count + Guid.NewGuid() + ".txt";

        Random rnd = new Random();
        using (FileStream fs = File.Create(fname))
        {
            StreamWriter sw = new StreamWriter(fs);
            for (int i=0; i<count; i++)
            {
                int curMaxLen =  rnd.Next(1, maxLen);
                StringBuilder sb = new StringBuilder(curMaxLen + 1);
                for (int k = 0; k < curMaxLen; k++)
                    sb.Append((char)('a' + rnd.Next(0, 26)));
                sb.Append((char)13);
                sw.Write(sb.ToString());
            }
            sw.Flush();
        }                

        return fname;
    }

    static void PrintHelp()
    {
        Console.WriteLine("args:\n 0 - maximum string lenght\n 1 - strings count");
        Console.ReadLine();
    }

}

