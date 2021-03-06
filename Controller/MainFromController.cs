
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
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






        #region 其他机器操作

        /// <summary>
        /// 获取机器当前信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<object> GetWhole(InPutInformationDto model)
        {
            OutPutAllDto resDto = new OutPutAllDto();
            foreach (var item in model.type)
            {
                switch (item)
                {
                    case (int)InformationTypeEnum.所有:
                        UpdateVisitor Visitor = new UpdateVisitor();
                        //获取当前机器温度
                        resDto = Visitor.GetSystemInfo();
                        resDto.YamlText = await ReadYamlText(model.YamlPath);
                        break;
                    case (int)InformationTypeEnum.算力:
                        resDto.CalculatingPower = await ReadCalculatingPower();
                        break;
                    case (int)InformationTypeEnum.温度:
                        resDto.CpuTemperature = await ReadCpuTemperature();
                        break;
                    case (int)InformationTypeEnum.读取配置文件:
                        resDto.YamlText = await ReadYamlText(model.YamlPath);
                        break;
                    default:
                        break;
                }
                if (item == (int)InformationTypeEnum.所有)
                {
                    break;
                }
            }
            
            return Task.FromResult(new { data = resDto, code = 200, msg = "成功" });
        }


        [HttpPost]
        public async Task<object> SetWhole(InPutInformationDto model)
        {

            foreach (var item in model.type)
            {
                switch (item)
                {
                    case (int)InformationTypeEnum.运行bat:
                        await WriteStartBat(model.YamlPath);
                        break;
                    case (int)InformationTypeEnum.启动:
                        await WriteStartMiner(model.YamlPath);
                        break;
                    case (int)InformationTypeEnum.停止:
                        await WriteEndMiner();
                        break;
                    case (int)InformationTypeEnum.设置配置文件:
                        await WriteYamlText(model.yaml);
                        break;
                    default:
                        break;
                }
            }
            return Task.FromResult(new { code = 200, msg = "成功" });
        }


        /// <summary>
        /// 获取其他机器配置
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<object> GetOtherWhole(List<InPutInformationDto> list)
        {
            List<OutPutAllDto> resListData=new List<OutPutAllDto>();
            foreach (var item in list)
            {
                try
                {
                    var url = "http://" + item.Ip + ":8081/api/MainFrom/GetWhole";
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
                    string result = httpResponse.Content.ReadAsStringAsync().Result;
                    var ResultObj = JsonConvert.DeserializeObject<JObject>(result);
                    var ResultData = ResultObj["Result"]["data"].ToObject<OutPutAllDto>();
                    ResultData.YamlPath = item.YamlPath;
                    ResultData.Ip = item.Ip;
                    ResultData.Alias = item.Alias;
                    ResultData.Remarks = item.Remarks;
                    ResultData.UserId = item.UserId;
                    ResultData.machineId = item.machineId;
                    ResultData.CreateTime = DateTime.Now.ToString("HH:mm:ss");
                    resListData.Add(ResultData);
                }
                catch (Exception e)
                {
                    var ResultData = new OutPutAllDto();
                    ResultData.YamlPath = item.YamlPath;
                    ResultData.Ip = item.Ip;
                    ResultData.Alias = item.Alias;
                    ResultData.Remarks = item.Remarks;
                    ResultData.UserId = item.UserId;
                    ResultData.machineId = item.machineId;
                    ResultData.CreateTime = DateTime.Now.ToString("HH:mm:ss");
                    resListData.Add(ResultData);
                }
               
            }
            
            return await Task.FromResult(new { data = resListData, code = 200, msg = "成功" });
        }

        /// <summary>
        /// 设置其他机器配置
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<object> SetOtherWhole(List<InPutInformationDto> list)
        {
            List<string> resListData = new List<string>();
            foreach (var item in list)
            {
                var url = "http://" + item.Ip + ":8081/api/MainFrom/SetWhole";
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
                string result = httpResponse.Content.ReadAsStringAsync().Result;
                var ResultData = JsonConvert.DeserializeObject<HttpResponseMessage>(result);
                resListData.Add(ResultData.StatusCode.ToString());
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


        private async Task<decimal> ReadCpuTemperature()
        {
            UpdateVisitor Visitor=new UpdateVisitor();
            //获取当前机器温度
            OutPutAllDto list = Visitor.GetSystemInfo();

            return await Task.FromResult(list.CpuTemperature);
        }


        [HttpPost]
        public async Task<object> CalculatingPower()
        {
            //获取当前算力
            var restext = await ReadCalculatingPower();
            return await Task.FromResult(new { data= restext ,code=200,msg="成功" });
        }


        private async Task<decimal> ReadCalculatingPower()
        {
            //获取当前算力
            decimal CalculatingPower = 0;
             
            var restext = (await appSettings.GetSettings("NowText")).Replace(" ","");
            var capacityIndex = restext.IndexOf("capacity=\"");
            var HKIndex = restext.IndexOf("KH/s\"");
            if (capacityIndex>0 && HKIndex>0)
            {
                try
                {
                    CalculatingPower = decimal.Parse(restext.Substring(capacityIndex + 10, HKIndex - (capacityIndex + 1)));
                }
                catch (Exception)
                {
                    CalculatingPower = 0;
                }
               
            }
            return await Task.FromResult(CalculatingPower);
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
            Thread.Sleep(1000);
            p.StandardInput.WriteLine(path+ "hpool-miner-ar-console.exe");
            //运行miner
            return await Task.FromResult(new { code = 200, msg = "成功" }); ;
        }

        [HttpGet]
        public async Task<object> WriteEndMiner()
        {
            return await Task.Run(() => {
                using (var process = CMDHelps.ExeCommand())
                {
                    process.Start();
                    process.OutputDataReceived += p_OutputDataReceived;
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                    process.StandardInput.WriteLine("taskkill /im hpool-miner-ar-console.exe /f");
                    process.Close();
                }
                return new { code = 200, msg = "成功" };
            });
        }

        private void p_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            MainFrom.mainfromInstance.SetText("\r\n" + e.Data);
        }

        /// <summary>
        /// 运行bat
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
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
