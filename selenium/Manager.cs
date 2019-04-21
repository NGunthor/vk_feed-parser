using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text.RegularExpressions;
using System.Threading;
using JetBrains.Annotations;

namespace selenium
{
    public class Manager
    {
        public static readonly object TextsLocker = new object();
        public static readonly object LinksLocker = new object();
        public static readonly object ImagesLocker = new object();
        private List<Text> Texts { get; set; }
        private List<Link> Links { get; set; }
        private List<Link> Images { get; set; }
        private List<IWebElement> Wall { get; set; }
        private IWebDriver Driver { get; set; }

        public Manager(IWebDriver driver, string url)
        {
            Driver = driver;
            Driver.Navigate().GoToUrl(url);
            Driver.FindElement(By.Id("index_email")).SendKeys("79152373374");
            Driver.FindElement(By.Id("index_pass")).SendKeys("53aruheh" + Keys.Enter);

            Thread.Sleep(5000);

            MakeWall();
        }

        public void FindTexts()
        {
            Monitor.Enter(TextsLocker);
            Console.WriteLine("inFindTexts");
            Texts = new List<Text>();
            foreach (var feedRow in Wall)
            {
                var id = GetId(feedRow);
                var textField = GetTextFieldElement(feedRow);
                if (textField.GetAttribute("class") != "wall_post_text")
                    Texts.Add(new Text(id, null));
                else if (textField.Text != string.Empty)
                    Texts.Add(new Text(id, textField.GetAttribute("textContent")));
            }
            WriteInFile(Texts, "texts.json");
            Console.WriteLine("outFindTexts");
            Monitor.Exit(TextsLocker);
        }
        public void FindLinks()
        {
            Monitor.Enter(LinksLocker);
            Console.WriteLine("inFindLinks");
            Links = new List<Link>();
            foreach (var feedRow in Wall)
            {
                var id = GetId(feedRow);
                var textField = GetTextFieldElement(feedRow);
                var aElements = textField.FindElements(By.TagName("a"))
                    .Where(element => element.GetAttribute("href") != null).ToList();
                if (textField.GetAttribute("class") != "wall_post_text" || aElements.Count == 0)
                {
                    Links.Add(new Link(id, null));
                    continue;
                }

                var contentLinks = new List<string>();
                foreach (var element in aElements)
                {
                    if (element.GetAttribute("href") != (string.Empty))
                        contentLinks.Add(element.GetAttribute("href"));
                }

                Links.Add(new Link(id, contentLinks));
            }

            WriteInFile(Links, "links.json");
            Console.WriteLine("outFindLinks");
            Monitor.Exit(LinksLocker);
        }
        
        public void FindImages()
        {
            Monitor.Enter(ImagesLocker);
            Console.WriteLine("inFindImages");
            Images = new List<Link>();
            foreach (var feedRow in Wall)
            {
                var id = GetId(feedRow);
                var imageField = GetImageFieldElement(feedRow);
                if (imageField == null || imageField.GetAttribute("class") != "page_post_sized_thumbs  clear_fix")
                    Images.Add(new Link(id, null));
                else
                {
                    var aElements = imageField.FindElements(By.TagName("a"));
                    var imageLinks = aElements.Select(GetImageUrl).ToList();
                    Images.Add(new Link(id, imageLinks));
                }
            }

            WriteInFile(Images, "images.json");
            Console.WriteLine("outFindImages");
            Monitor.Exit(ImagesLocker);
        }

        private string GetId(IWebElement feedRow) =>
            feedRow.FindElement(By.XPath("./div")).GetAttribute("data-post-id");

        private IWebElement GetTextFieldElement(IWebElement feedRow) =>
            feedRow.FindElement(By.ClassName("wall_text")).FindElement(By.XPath("./div/div"));

        [CanBeNull]
        private IWebElement GetImageFieldElement(IWebElement feedRow) => feedRow.FindElement(By.ClassName("wall_text"))
            .FindElements(By.XPath("./div/div[2]")).SingleOrDefault();

        private string GetImageUrl(IWebElement aElement)
        {
            return Regex.Match(aElement.GetAttribute("style"), @"https(.*?)jpg").ToString();
        }

        private static void WriteInFile<T>(List<T> collection, string filename) where T : Post
        {
            using (var file = new FileStream("../../" + filename, FileMode.Create))
            {
                foreach (var text in collection)
                {
                    var jsonFormatter = new DataContractJsonSerializer(text.GetType());
                    jsonFormatter.WriteObject(file, text);
                }
            }
        }

        private void MakeWall()
        {
            IList<IWebElement> wall = Driver.FindElements(By.ClassName("feed_row "));
            Wall = new List<IWebElement>();
            foreach (var feedRow in wall)
            {
                var id = GetId(feedRow);
                if ((id == null) || (feedRow.FindElement(By.XPath("./div")).GetAttribute("data-ad") != null))
                    continue;
                Wall.Add(feedRow);
            }
        }
    }
}