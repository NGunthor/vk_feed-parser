using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support;
using System.Threading;
using OpenQA.Selenium.Support.UI;
using Newtonsoft.Json;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Json;

namespace selenium
{
    static class Program
    {
        public static void Main(string[] args)
        {
            var driver = new ChromeDriver();
            var manager = new Manager(driver,"http://vk.com");

            Thread.Sleep(5000);
            Console.WriteLine("Start");

            var threads = new ThreadManager(manager);
            
            while (threads.Threads[3].IsAlive)
                continue;
            Console.WriteLine("End");
            var t = "../../texts.json".DeserializeTo<List<Text>>();
            Console.ReadKey();
        }
    }

    public static class Helper
    {
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> act)
        {
            foreach (var t in source)
            {
                act(t);
            }
        }
        
        public static string ToString(this IEnumerable source, string separator = ", ") => string.Join(separator, source);

        public static T DeserializeTo<T>(this string path)
        {
            using (var fs = new FileStream(path, FileMode.Open))
            {
                return  (T) new DataContractJsonSerializer(typeof(T)).ReadObject(fs);
            }  
        }
    }
}
