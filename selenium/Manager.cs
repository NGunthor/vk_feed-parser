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

namespace selenium
{
    class Manager
    {
        List<Text> texts;
        List<Link> links;
        List<Link> images;
        private List<IWebElement> Wall{ get; set; }
        private IWebDriver Driver { get; set; }

        public Manager(IWebDriver driver, string url)
        {
            Driver = driver;
            Driver.Navigate().GoToUrl(url);
            Driver.FindElement(By.Id("index_email")).SendKeys("79152373374");
            Driver.FindElement(By.Id("index_pass")).SendKeys("53aruheh" + Keys.Enter);
        }

        public void FindTexts()
        {
            //TODO ReadMoreButton
            texts = new List<Text>();
            foreach (var feedRow in Wall)
            {
                var id = GetId(feedRow);
                var textField = GetTextFieldElement(feedRow);
                if (textField.GetAttribute("class") != "wall_post_text")
                    texts.Add(new Text(id, null));
                else if(textField.Text != string.Empty)
                    texts.Add(new Text(id, textField.Text));
            }
            WriteInFile(texts, "texts.json");
        }

        public void FindLinks()
        {
            links = new List<Link>();
            foreach (var feedRow in Wall)
            {
                var id = GetId(feedRow);
                var textField = GetTextFieldElement(feedRow);
                var aElements = textField.FindElements(By.TagName("a"));
                if (textField.GetAttribute("class") != "wall_post_text" || aElements.Count == 0)
                {
                    links.Add(new Link(id, null));
                    continue;
                }
                var contentLinks = new List<string>();
                foreach (var element in aElements)
                {
                    if (element.GetAttribute("href") != (string.Empty))
                        contentLinks.Add(element.GetAttribute("href"));
                }
                links.Add(new Link(id, contentLinks));
            }
            WriteInFile(links, "links.json");
        }

        public void FindImages()
        {
            images = new List<Link>();
            foreach (var feedRow in Wall)
            {
                var id = GetId(feedRow);
                var imageField = GetImageFieldElement(feedRow);
                if (imageField.GetAttribute("class") != "page_post_sized_thumbs  clear_fix")
                    images.Add(new Link(id, null));
                else
                {
                    var imageLinks = new List<string>();
                    var aElements = imageField.FindElements(By.TagName("a"));
                    foreach (var element in aElements)
                    {
                        var imageUrl = GetImageUrl(element);
                        imageLinks.Add(imageUrl);
                    }
                    images.Add(new Link(id, imageLinks));
                }
            }
            WriteInFile(images, "images.json");
        }

        private string GetId(IWebElement feedRow) => feedRow.FindElement(By.XPath("./div")).GetAttribute("data-post-id");
        
        private IWebElement GetTextFieldElement(IWebElement feedRow) => feedRow.FindElement(By.ClassName("wall_text")).FindElement(By.XPath("./div/div"));
        
        private IWebElement GetImageFieldElement(IWebElement feedRow) => feedRow.FindElement(By.ClassName("wall_text")).FindElement(By.XPath("./div/div[2]"));

        private string GetImageUrl(IWebElement aElement)
        {
            var styleSting = aElement.GetAttribute("style");
            var url = styleSting.Substring(styleSting.IndexOf("url(\"") + 5);
            return url.Substring(0, url.IndexOf(".jpg") + 4);
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

        public void MakeWall()
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
