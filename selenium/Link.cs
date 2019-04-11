using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace selenium
{
    [DataContract]
    internal class Link : Post
    {
        [DataMember]
        private List<string> ContentLinks { get; set; }

        public Link(string id, List<string> content) : base(id) => ContentLinks = content;
    }
}
