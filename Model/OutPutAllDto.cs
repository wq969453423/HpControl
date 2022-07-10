using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 子端.Model
{
    public class OutPutAllDto
    {
        //配置文件
        public InPutYamlTextDto YamlText { get; set; }

        //cpu温度
        public List<string> CpuTemperature { get; set; }

        //当前算力
        public string CalculatingPower { get; set; }

    

    }
}
