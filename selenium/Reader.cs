using System;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;

namespace selenium
{
    public static class Reader
    {
        private static bool TextsDone { get; set; }
        private static bool LinksDone { get; set; }
        private static bool ImagesDone { get; set; }
        public static void RunReading()
        {
            Console.WriteLine("StartReading");
            while (!(TextsDone && LinksDone && ImagesDone))
            {
                if (!TextsDone && Monitor.TryEnter(Manager.TextsLocker))
                {
                    Console.WriteLine("StReadTexts");
                    ReadFile("texts.json");
                    TextsDone = true;
                    Monitor.Exit(Manager.TextsLocker);
                    Console.WriteLine("EndReadTexts");
                    
                }
                
                if (!LinksDone && Monitor.TryEnter(Manager.LinksLocker))
                {
                    Console.WriteLine("StReadLinks");
                    ReadFile("links.json");
                    LinksDone = true;
                    Monitor.Exit(Manager.LinksLocker);
                    Console.WriteLine("EndReadLinks");
                }

                if (!ImagesDone && Monitor.TryEnter(Manager.ImagesLocker))
                {
                    Console.WriteLine("StReadImages");
                    ReadFile("images.json");
                    ImagesDone = true;
                    Monitor.Exit(Manager.ImagesLocker);
                    Console.WriteLine("EndReadImages");
                }
            }
            Console.WriteLine("EndReading");
        }

        private static void ReadFile(string filename)
        {
            using (StreamWriter sw = new StreamWriter("../../" + filename, true, System.Text.Encoding.UTF8))
            {
                sw.WriteLine();
                sw.Write("nЗдесь был " + filename);
            }
//            using (StreamReader sr = new StreamReader("../../" + filename))
//            {
//                Console.WriteLine(sr.ReadToEnd());
//            }
        }
    }
}