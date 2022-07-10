
using System.Diagnostics;
using System.Web.Http;
using 子端.Help;
using 子端.Model;

namespace 子端.Controller
{
    public class MainFromController : ApiController
    {
        AppSettingsHelps appSettings;
        Process p;
        public MainFromController() {
            appSettings = new AppSettingsHelps();
            p = MainFrom.mainfromInstance.GetProcess();
        }
        public static string ReadTextNow;


        private string ReadText()
        {
            //获取当前算力
            var restext = appSettings.GetSettings("NowText");
            return restext;
        }

        private string ReadCpuTemperature()
        {
            //获取当前机器温度
            return default;
        }

        private string ReadYamlText()
        {
            //获取当前机器配置文件
            return default;
        }


        [HttpGet]
        public string WriteStartMiner()
        {
            
            p.StandardInput.WriteLine("dir");
            //运行miner
            return default;
            
        }
        [HttpGet]
        public string WriteEdnMiner()
        {
            p.StandardInput.WriteLine("hello,world");
            //结束miner
            return default;
        }
        [HttpPost]
        public object WriteYamlText(InPutYamlTextDto model)
        {
            //- k:/
            string paths = string.Join("\r\n", model.path);

            //写入配置文件
            var str = string.Format(appSettings.GetYamlText(),paths,model.minerName,model.apiKey,model.proxy,model.cpuIndex,model.flag,model.threadNum);

            appSettings.WriteFile(@"D:\ar\windows\", str);

            return new { code=200,msg="成功" };
        }

    }
}
