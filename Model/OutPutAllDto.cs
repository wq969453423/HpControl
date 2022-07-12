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
        //当前算力
        public string CalculatingPower { get; set; }


        //cpu温度
        public decimal CpuTemperature { get; set; }
        public string CpuTemperatureColor
        {
            get
            {
                if (SsdTemperature < 60)
                {
                    return "green";
                }
                else if (SsdTemperature < 80)
                {
                    return "blue";
                }
                else
                {
                    return "red";
                }
            }
        }

        //cpu功耗
        public decimal CpuPower { get; set; }

        //cpu占用
        public decimal CpuLoad { get; set; }


        //cpu频率
        public decimal CpuFrequency { get; set; }


        //内存大小
        public decimal MemorySize { get; set; }

        //内存占用
        public decimal MemoryLoad { get; set; }

        public string MemoryLoadColor
        {
            get
            {
                if (SsdTemperature > 95)
                {
                    return "red";
                }
                else
                {
                    return "green";
                }
               
            }
        }


        //固态温度
        public decimal SsdTemperature { get; set; }

        public string SsdTemperatureColor { 
            get {
                if (SsdTemperature<40)
                {
                    return "green";
                }
                else if(SsdTemperature < 60)
                {
                    return "blue";
                }
                else if (SsdTemperature > 60)
                {
                    return "red";
                }
                return "";
            } 
        }


        //固态占用
        public decimal SsdLoad { get; set; }
        public string SsdLoadColor
        {
            get
            {
                if (SsdTemperature < 40)
                {
                    return "green";
                }
                else if (SsdTemperature < 60)
                {
                    return "blue";
                }
                else if (SsdTemperature > 60)
                {
                    return "red";
                }
                return "";
            }
        }




    }
}
