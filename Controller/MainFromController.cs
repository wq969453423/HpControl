
using System.Diagnostics;
using System.Threading.Tasks;
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



        

        

        //获取所有
        [HttpPost]
        public async Task<object>  GetWhole()
        {
            OutPutAllDto resDto=new OutPutAllDto();
            resDto.YamlText = await ReadYamlText("");
            resDto.CpuTemperature = await ReadCpuTemperature();
            resDto.CalculatingPower = await ReadCalculatingPower();
            return Task.FromResult(new { data= resDto,code=200,msg="成功" });
        }


        #region 温度 && 算力

        [HttpPost]
        public async Task<object> CpuTemperature()
        {
            //获取当前机器温度
            return default;
        }


        private async Task<string> ReadCpuTemperature()
        {
            //获取当前机器温度
            return "";
        }


        [HttpPost]
        public async Task<object> CalculatingPower()
        {
            //获取当前算力
            var restext = await ReadCalculatingPower();
            return await Task.FromResult(new { data= restext ,code=200,msg="成功" });
        }


        private async Task<string> ReadCalculatingPower()
        {
            //获取当前算力
            var restext = appSettings.GetSettings("NowText");
            return await Task.Run(() => restext);
        }

        #endregion


        #region miner配置文件相关操作
        [HttpGet]
        public async Task<object> GetYamlText(string path)
        {
           
            return default;
        }


        private async Task<InPutYamlTextDto> ReadYamlText(string path) {
            //获取当前机器配置文件
            var resData= await appSettings.ReadFile(path);
            return resData;
        }

        [HttpPost]
        public async Task<object> WriteYamlText(InPutYamlTextDto model)
        {
            //- k:/
            string paths = string.Join("\r\n", model.path);

            //写入配置文件
            var str = string.Format(appSettings.GetYamlText(), paths, model.minerName, model.apiKey, model.proxy, model.cpuIndex, model.flag, model.threadNum);

            await appSettings.WriteFile(@"D:\ar\windows\", str);

            return await Task.FromResult(new { code = 200, msg = "成功" });
        }

        #endregion


        #region miner 启动 结束

        [HttpGet]
        public async Task<object> WriteStartMiner()
        {

            p.StandardInput.WriteLine("dir");
            //运行miner
            return default;

        }

        [HttpGet]
        public async Task<object> WriteEdnMiner()
        {
            p.StandardInput.WriteLine("hello,world");
            //结束miner
            return default;
        }


        #endregion


    }
}
