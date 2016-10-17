using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace BruteforceAttacker
{
    public class Header
    {
        public string Cnonce { get; set; }
        public string Nonce { get; set; }
        public string Realm { get; set; }
        public string UserName { get; set; }
        public string Uri { get; set; }
        public string Response { get; set; }
        public string Method { get; set; }
        public string NounceCounter { get; set; }
    }

}