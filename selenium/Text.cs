using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace selenium
{
    [DataContract]
    internal class Text : Post
    {
        [DataMember]
        public String ContentText { get; set; }

        public Text(String id, String text) : base(id) => ContentText = text;
    }
}
