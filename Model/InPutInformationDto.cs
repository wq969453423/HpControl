using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 子端.Model
{
    public class InPutInformationDto
    {

        public List<int> type { get; set; }

        public string path { get; set; }
        public string ip { get; set; }

        public InPutYamlTextDto yaml { get; set; }
    }
}
