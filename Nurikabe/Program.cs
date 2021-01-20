using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.VisualBasic.CompilerServices;


namespace Nurikabe
{
    class Program
    {
        private static Nurikabe nur = null;
        private static string nurikabePath = "..\\..\\..\\NurikabeFields";
        
        private int _myProperty;
        
        static void Main(string[] args)
        {

            Start();
            Console.WriteLine("Zazenem se enkrat? y/n");
            var in1 = Console.ReadLine();
            if (in1 == "y" || in1 == "Y")
            {
                Main(null);
            }
            
        }
        
        private static void nowNurikabe()
        {
            Console.Clear();
            string[] subdirectoryEntries = Directory.GetFiles(nurikabePath);
            int indx = 0;
            foreach (var f in subdirectoryEntries)
            {
                   Console.WriteLine(indx+" "+f.Split('\\')[f.Split('\\').Length-1]);
                   indx++;
            }
            Console.WriteLine(subdirectoryEntries.Length + " Nazaj");
            var r = Console.ReadLine();
            string backk = subdirectoryEntries.Count().ToString();
            if (r == backk)
            {
                Start();
            }
            else
            {
                int x = 0;
                Int32.TryParse(r, out x);
                narisiMeni(subdirectoryEntries[x]);
            }

        }



        private static void narisiMeni(string i)
        {
            Nurikabe n = new Nurikabe(i);
            string risba = n.NarisiMe();
            Console.WriteLine(risba);
            Console.WriteLine("1 naivni\n2 genetski");
            var r = Console.ReadLine();

            switch (r)
            {
                case "1":
                {
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    n.ResiMe();
                    sw.Stop();

                    risba = n.NarisiMe();
                    Console.WriteLine(risba);
                    Console.WriteLine("Elapsed={0}ms",sw.ElapsedMilliseconds);
                    break;
                }
                case "2":
                {
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    n.ResiMeGen();
                    sw.Stop();
                    Console.WriteLine("Elapsed={0}ms",sw.ElapsedMilliseconds);
                    break;
                }
            }
        }

        private static void Start()
        {
            Console.Clear();
            Console.WriteLine("0 nov nurikabe\n1 exit");
            var r = Console.ReadLine();
            switch (r)
            {
                case "0":
                    nowNurikabe();
                    break;
                case "1":
                    System.Environment.Exit(1);
                    break;
            }        }
    }
}