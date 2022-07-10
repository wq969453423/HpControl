using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 子端.Model
{
    public class InPutYamlTextDto
    {
       
        public List<string> path { get; set; }

        public string minerName { get; set; }

        public string apiKey { get; set; }

        public string cpuIndex { get; set; }
        public string proxy { get; set; }

        // argon2avx2,largepages,hardaes,fullmem,jit
        public string flag { get; set; }

        public string threadNum { get; set; }
    }
}
