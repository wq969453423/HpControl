using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 子端.Model
{
    public class InPutInformationDto
    {

        public int machineId { get; set; }
        public int UserId { get; set; }
        public string Ip { get; set; }

        public string Alias { get; set; }
        public string Remarks { get; set; }
        public string CreateTime { get; set; }

        public int BeginCore { get; set; }

        public int EndCore { get; set; }

        public string YamlPath { get; set; }

        public int IpIndex { get; set; }

        public List<int> type { get; set; }
        public InPutYamlTextDto yaml { get; set; }
    }


    


}
