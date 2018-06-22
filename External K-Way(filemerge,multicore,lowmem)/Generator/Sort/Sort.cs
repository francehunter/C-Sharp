using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

/// <summary>merge sort the massive file of strings using n-way method</summary>
namespace Sort
{
    public struct FilePair
    {
        public string fnameSrc1;
        public string fnameSrc2;
        public string fnameDst;

        public FilePair(string fnameSrc1, string fnameSrc2, string fnameDst)
        {
            this.fnameSrc1 = fnameSrc1;
            this.fnameSrc2 = fnameSrc2;
            this.fnameDst = fnameDst;
        }
    }


    class SortMerge
    {
        string tmpPath;
        //
        int phase = 0;
        // timing
        DateTime stTime;
        DateTime enTime;
        public SortMerge(string fname)
        {
            tmpPath = "tmp" + Guid.NewGuid();
            Directory.CreateDirectory(tmpPath);
            // begin calculate simplified performance
            stTime = DateTime.Now;

            Console.WriteLine("initial dividing..");
            int countFiles = DivideAndSortInitial(fname);

            string fnameResult = string.Empty;
            while(true)
            {
                // observe exist files
                string[] arr = Directory.GetFiles(tmpPath, "p" + phase +  "_*.txt");
                // detect what sorting complete if files merged in one
                if (arr.Length == 1)
                {
                    fnameResult = arr[0];
                    break;
                }
                // begin new phase
                int len = arr.Length / 2;
                phase++; Console.WriteLine("phase " + phase);
                // prepare data for tasks
                var pairs = new List<FilePair>();
                for (int i = 0; i < len; i++)
                {
                    int idx = i * 2;
                    string fnameSrc1 = arr[idx];
                    string fnameSrc2 = arr[idx + 1];
                    string fnameDst = tmpPath + "\\p" + phase + "_" + i + ".txt";
                    FilePair pair = new FilePair(fnameSrc1, fnameSrc2, fnameDst);
                    pairs.Add(pair);
                }
                // parallel sort pairs
                Parallel.ForEach(pairs, pair => { SortPair(pair); } );
                // odd file was migrate to next phase
                if (arr.Length > len * 2)
                    File.Move(arr[arr.Length-1], tmpPath + "\\p" + phase + "_" + len + ".txt");                    
            }
            // emit result file to source dirrectory
            FileResult = fname + ".sorted";
            if (File.Exists(FileResult))
                FileResult += "." + Guid.NewGuid();
            File.Move(fnameResult, FileResult);
            // clean temporary files
            Directory.Delete(tmpPath, true);
            //begin calculate simplified performance
            enTime = DateTime.Now;
        }

        /// <summary>divide source file to N sorted files and return count</summary>
        int DivideAndSortInitial(string fname)
        {
            int maxCount = 4096 * 8;
            int idx = 0;
            List<string> list = new List<string>();
            //string[] arr = new string[0];
            using (StreamReader sr = new StreamReader(fname))
            {                
                while (true)
                {
                    string line = sr.ReadLine();
                    if (line == null)
                        break;

                    list.Add(line);

                    if (list.Count == maxCount)
                    {
                        //list.Sort();
                        string[] arr = Sedgewick.Sort(list.ToArray());
                        File.WriteAllLines(tmpPath + "\\p0_" + idx + ".txt", arr);
                        //File.WriteAllLines(tmpPath+"\\p0_"+idx+".txt", list);
                        list.Clear();
                        idx++;
                    }
                }

                if (list.Count > 0)
                {
                    //list.Sort();
                    //File.WriteAllLines(tmpPath + "\\p0_" + idx + ".txt", list);
                    string[] arr = Sedgewick.Sort(list.ToArray());
                    File.WriteAllLines(tmpPath + "\\p0_" + idx + ".txt", arr);
                    idx++;
                }
            }
            return idx;
        }

        void SortPair(object state)
        {
            FilePair pair = (FilePair)state;

            using (StreamReader sr1 = new StreamReader(pair.fnameSrc1),
                    sr2 = new StreamReader(pair.fnameSrc2))
            {
                using (StreamWriter sw = new StreamWriter(pair.fnameDst))
                {
                    string line1 = null;
                    string line2 = null;
                    while (true)
                    {
                        if (line1 == null)
                            if (sr1.Peek() >= 0)
                                line1 = sr1.ReadLine();

                        if (line2 == null)
                            if (sr2.Peek() >= 0)
                                line2 = sr2.ReadLine();

                        if ((line1 == null) && (line2 == null))
                            break;

                        if (String.Compare(line1, line2) > 0)
                        {
                            sw.WriteLine(line1);
                            line1 = null;
                        }
                        else
                        {
                            sw.WriteLine(line2);
                            line2 = null;
                        }

                    }
                    sw.Flush();
                }
            }
        }

        public TimeSpan CompleteTime { get { return enTime - stTime; } }

        public string FileResult { get; set; }
        /// <summary>average performance mb/sec</summary>
        public double AverageSpeed
        {
            get
            {
                return 
                    new FileInfo(FileResult).Length / 
                    (1024 * 1024) / 
                    ((enTime - stTime).TotalSeconds + 0.0001f);
            }

        }
    }
}
