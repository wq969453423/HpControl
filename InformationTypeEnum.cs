using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 子端
{
    public enum InformationTypeEnum
    {
        [Description("所有")]
        所有 = 100,

        [Description("配置文件")]
        配置文件 = 200,

        [Description("温度")]
        温度 = 300,

        [Description("算力")]
        算力 = 400,

    }
}
