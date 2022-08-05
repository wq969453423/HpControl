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

        /// <summary>
        /// 是否多开
        /// </summary>
        public string IsOpenMore { get; set; }

        public List<string> YamlPath { get; set; }


        public List<int> type { get; set; }
        public InPutYamlTextDto yaml { get; set; }
    }


    public class InPutInformation
    {
        public int Id { get;set; }
        public int machineId { get; set; }
        public int UserId { get; set; }
        public string Ip { get; set; }

        public string Alias { get; set; }
        public string Remarks { get; set; }
        public string CreateTime { get; set; }

        /// <summary>
        /// 是否多开
        /// </summary>
        public string IsOpenMore { get; set; }

        public string YamlPath { get; set; }


        public List<int> type { get; set; }
        public InPutYamlTextDto yaml { get; set; }
    }


}
