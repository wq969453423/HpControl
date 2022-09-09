
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
        AppSettingsHelps appSettings = new AppSettingsHelps();
        

        #region 其他机器操作

        /// <summary>
        /// 获取机器当前信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<object> GetWhole(InPutInformationDto model)
        {
            for (int i = 0; i < 4; i++)
            {
                if (i != model.IpIndex)
                {
                    appSettings.SetSettings("NowText" + (i+1), "0");
                }
            }
            
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
                        resDto.CalculatingPower = await ReadCalculatingPower(model.IpIndex);
                        break;
                    case (int)InformationTypeEnum.算力:
                        resDto.CalculatingPower = await ReadCalculatingPower(model.IpIndex);
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
                        await WriteStartMiner(model);
                        break;
                    case (int)InformationTypeEnum.停止:
                        await WriteEndMiner(model);
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
            var resDto = new List<OutPutDto>();
            List<OutPutAllDto> resListData=new List<OutPutAllDto>();
            foreach (var item in list)
            {
                try
                {
                    MainFrom.mainfromInstance.SetText("获取"+ item.Ip+"数据");
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
                    ResultData.IpIndex = item.IpIndex;
                    ResultData.IpIndex = item.BeginCore;
                    ResultData.IpIndex = item.EndCore;
                    ResultData.CreateTime = DateTime.Now.ToString("HH:mm:ss");
                    resListData.Add(ResultData);
                    MainFrom.mainfromInstance.SetText("成功");
                }
                catch (Exception e)
                {
                    var ResultData = new OutPutAllDto();
                    ResultData.YamlPath = item.YamlPath;
                    ResultData.Ip = item.Ip;
                    ResultData.Alias = item.Alias;
                    ResultData.Remarks = item.Remarks;
                    ResultData.UserId = item.UserId;
                    ResultData.IpIndex = item.IpIndex;
                    ResultData.CreateTime = DateTime.Now.ToString("HH:mm:ss");
                    resListData.Add(ResultData);
                }
               
            }

            
            return await Task.FromResult(new { data = resListData, code = 200, msg = "成功" });
        }

        /// <summary>
        /// 设置其他机器
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
            MainFrom.mainfromInstance.SetText("获取当前机器温度");
            UpdateVisitor Visitor=new UpdateVisitor();
            //获取当前机器温度
            OutPutAllDto list = Visitor.GetSystemInfo();

            return await Task.FromResult(list.CpuTemperature);
        }


        [HttpPost]
        public async Task<object> CalculatingPower(int index)
        {
            //获取当前算力
            var restext = await ReadCalculatingPower(index);
            return await Task.FromResult(new { data= restext ,code=200,msg="成功" });
        }


        private async Task<decimal> ReadCalculatingPower(int index)
        {
            MainFrom.mainfromInstance.SetText("获取当前算力");
            //获取当前算力
            return await Task.FromResult(decimal.Parse(appSettings.GetSettings("NowText"+index)));
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
            try
            {
                //获取当前机器配置文件
                MainFrom.mainfromInstance.SetText("获取当前机器配置文件");
                var resData = await appSettings.ReadFile(path);
                return resData;
            }
            catch (Exception e)
            {
                MainFrom.mainfromInstance.SetText("获取配置文件出错");
                return new InPutYamlTextDto(); ;
            }
            

            
        }

        [HttpPost]
        public async Task<object> WriteYamlText(InPutYamlTextDto model)
        {
            MainFrom.mainfromInstance.SetText("写入配置文件");
            //- k:/
            string paths = string.Join("\r\n", model.path.Select(e=>"-"+e).ToList());

            //写入配置文件
            var str = string.Format(appSettings.GetYamlText(), paths, model.minerName, model.apiKey, model.proxy, model.cpuIndex, model.flag, model.threadNum);

            await appSettings.WriteFile(model.filepath, str);

            return await Task.FromResult(new { code = 200, msg = "成功" });
        }

        #endregion


        #region miner 启动 结束

        /// <summary>
        /// 重新启动
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<object> WriteStartMiner(InPutInformationDto model)
        {
            var IpIndex = model.IpIndex - 1;
            List<Process> p = MainFrom.mainfromInstance.GetProcess();

            p[IpIndex].Close();
            switch (IpIndex) {
                case 0:
                    MainFrom.mainfromInstance.p1Start();
                    break;
                case 1:
                    MainFrom.mainfromInstance.p2Start();
                    break;
                case 2:
                    MainFrom.mainfromInstance.p3Start();
                    break;
                case 3:
                    MainFrom.mainfromInstance.p4Start();
                    break;
            }
            p = MainFrom.mainfromInstance.GetProcess();
            if (model.EndCore != 0 && model.BeginCore != 0)
            {
                p[model.IpIndex].ProcessorAffinity = new IntPtr(2121);
            }
            p[IpIndex].StandardInput.WriteLine(model.YamlPath[0] + ":");
            p[IpIndex].StandardInput.WriteLine("cd " + model.YamlPath);
            p[IpIndex].StandardInput.WriteLine(@"hpool-miner-ar-console.exe");


            //运行miner
            return await Task.FromResult(new { code = 200, msg = "成功" }); ;
        }

        /// <summary>
        /// 全部停止
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<object> WriteEndMinerAll()
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

        /// <summary>
        /// 停止
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<object> WriteEndMiner(InPutInformationDto model)
        {
            var IpIndex = model.IpIndex - 1;
            List<Process> p = MainFrom.mainfromInstance.GetProcess();

            p[IpIndex].Close();
            switch (IpIndex)
            {
                case 0:
                    MainFrom.mainfromInstance.p1Start();
                    break;
                case 1:
                    MainFrom.mainfromInstance.p2Start();
                    break;
                case 2:
                    MainFrom.mainfromInstance.p3Start();
                    break;
                case 3:
                    MainFrom.mainfromInstance.p4Start();
                    break;
            }

            return await Task.FromResult(new { code = 200, msg = "成功" });
        }
        
        private void p_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            MainFrom.mainfromInstance.SetText(e.Data);
        }



        /// <summary>
        /// 运行bat
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<object> WriteStartBat(string path)
        {
            MainFrom.mainfromInstance.SetText("运行bat");
            List<Process> plist = MainFrom.mainfromInstance.GetProcess();
            plist[0].StandardInput.WriteLine(path);
            //运行miner
            return await Task.FromResult(new { code = 200, msg = "成功" }); ;
        }

        #endregion


    }
}
