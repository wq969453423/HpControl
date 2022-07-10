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
        所有 = 1,

        [Description("读取配置文件")]
        读取配置文件 = 2,

        [Description("温度")]
        温度 = 3,

        [Description("算力")]
        算力 = 4,


        [Description("启动")]
        启动 = 5,
        [Description("停止")]
        停止 = 6,
        [Description("设置配置文件")]
        设置配置文件 = 7,
        [Description("运行bat")]
        运行bat = 8,

    }
}
