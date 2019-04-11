using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support;


namespace selenium
{
    [DataContract]
    public abstract class Post
    {
        [DataMember]
        public String Id { get; set;  }

        protected Post(string id) => Id = id;
    }
}
