
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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





        /// <summary>
        /// 获取机器当前信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<object>  GetWhole(InPutInformationDto model)
        {
            OutPutAllDto resDto=new OutPutAllDto();
            foreach (var item in model.type) {
                switch (item) {
                    case (int)InformationTypeEnum.所有:
                        resDto.YamlText = await ReadYamlText(model.path);
                        resDto.CpuTemperature = await ReadCpuTemperature();
                        resDto.CalculatingPower = await ReadCalculatingPower();
                        break;
                    case (int)InformationTypeEnum.算力:
                        resDto.CalculatingPower = await ReadCalculatingPower();
                        break;
                    case (int)InformationTypeEnum.温度:
                        resDto.CpuTemperature = await ReadCpuTemperature();
                        break;
                    case (int)InformationTypeEnum.配置文件:
                        resDto.YamlText = await ReadYamlText(model.path);
                        break;
                }
                if (item==(int)InformationTypeEnum.所有)
                {
                    break;
                }   
            }
            return Task.FromResult(new { data= resDto,code=200,msg="成功" });
        }


        #region 其他机器配置

        /// <summary>
        /// 获取其他机器配置
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<object> GetOtherWhole(List<InPutInformationDto> list)
        {
            List<OutPutAllDto> resListData=new List< OutPutAllDto >();
            foreach (var item in list)
            {
                var url = "http://" + item.ip + ":8081/api/MainFrom/GetWhole";
                //新建hppt请求
                var client = new HttpClient();
                var parms = JsonConvert.SerializeObject(item);
                HttpContent content = null;
                if (parms != null)
                {
                    content = new StringContent(parms);
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                }
                var httpResponse = await client.PostAsync(url, content);
                string result = await httpResponse.Content.ReadAsStringAsync();
                var ResultData = JsonConvert.DeserializeObject<OutPutAllDto>(result);
                resListData.Add(ResultData);
            }

            return await Task.FromResult(new { data = resListData, code = 200, msg = "成功" });
        }

        #endregion

        #region 温度 && 算力

        [HttpGet]
        public async Task<object> CpuTemperature()
        {
            //获取当前机器温度
            var list = await ReadCpuTemperature();
            return await Task.FromResult(new { data = list, code = 200, msg = "成功" });
        }


        private async Task<List<string>> ReadCpuTemperature()
        {
            UpdateVisitor Visitor=new UpdateVisitor();
            //获取当前机器温度
            List<string> list= Visitor.GetSystemInfo();

            return await Task.FromResult(list);
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
            var yaml = await ReadYamlText(path);
            return await Task.FromResult(new { data = yaml, code = 200, msg = "成功" });
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
            string paths = string.Join("\r\n", model.path.Select(e=>"-"+e).ToList());

            //写入配置文件
            var str = string.Format(appSettings.GetYamlText(), paths, model.minerName, model.apiKey, model.proxy, model.cpuIndex, model.flag, model.threadNum);

            await appSettings.WriteFile(model.filepath, str);

            return await Task.FromResult(new { code = 200, msg = "成功" });
        }

        #endregion


        #region miner 启动 结束

        [HttpGet]
        public async Task<object> WriteStartMiner(string path)
        {
            //先停止
            await WriteEndMiner();
            p.StandardInput.WriteLine(path+ "hpool-miner-ar-console.exe");
            //运行miner
            return await Task.FromResult(new { code = 200, msg = "成功" }); ;

        }

        [HttpGet]
        public async Task<object> WriteEndMiner()
        {
            using (var process = CMDHelps.ExeCommand())
            {
                process.Start();
                process.OutputDataReceived += p_OutputDataReceived;
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.StandardInput.WriteLine("taskkill /im hpool-miner-ar-console.exe /f");
                process.Close();
            }
            return await Task.FromResult(new { code = 200, msg = "成功" });
        }

        private void p_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            MainFrom.mainfromInstance.SetText("\r\n" + e.Data);
        }


        [HttpGet]
        public async Task<object> WriteStartBat(string path)
        {
            p.StandardInput.WriteLine(path);
            //运行miner
            return await Task.FromResult(new { code = 200, msg = "成功" }); ;

        }

        #endregion


    }
}
