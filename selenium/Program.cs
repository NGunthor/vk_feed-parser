using System;
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

namespace selenium
{
    static class Program
    {
        public static void Main(string[] args)
        {
            var driver = new ChromeDriver();
            var manager = new Manager(driver,"http://vk.com");

            Thread.Sleep(10000);
            Console.WriteLine("Start");

            manager.MakeWall();
//            manager.FindTexts();
//            manager.FindLinks();
            manager.FindImages();
            
            Console.WriteLine("End");
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
    }
}
