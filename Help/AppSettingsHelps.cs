using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 子端.Model;

namespace 子端.Help
{
    public class AppSettingsHelps
    {
        //获取Configuration对象
        Configuration config = System.Configuration.ConfigurationManager.OpenExeConfiguration("子端.exe");
        
        
        public async Task<string> GetSettings(string key) {
            //根据KEY读取值
            string Str = config.AppSettings.Settings[key].Value;
            return await Task.FromResult(Str);
        }


        public async Task SetSettings(string key, string SetValue)
        {
            config.AppSettings.Settings[key].Value = SetValue;
            await SaveSettings();
        }

        public async Task AddSettings(string key,string SetValue)
        {
            config.AppSettings.Settings.Add(key, SetValue);
            await SaveSettings();
        }

        public async Task DelSettings(string key)
        {
            config.AppSettings.Settings.Remove(key);
            await SaveSettings();
        }

        private async Task SaveSettings()
        {
            await Task.Run(() =>
            {
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("AppSettings");
            });
            
        }


        public async Task<bool> WriteFile(string filePath,string content, string fileName= "config.yaml")
        {
            await Task.Run(() => {

                FileInfo fi = new FileInfo(filePath + fileName);

                if (fi.Exists)
                {
                    fi.Delete();
                }
                StreamWriter sw = fi.CreateText();
                sw.WriteLine(content);
                sw.Flush();
                sw.Close();
                sw.Dispose();
            });
            
            return true;
        }

        public async Task<InPutYamlTextDto>  ReadFile(string filePath, string fileName = "config.yaml")
        {

            StreamReader sr = new StreamReader(filePath + fileName);
            string input = sr.ReadToEnd().Replace("\n","").Replace("\"", "'");
            sr.Close();
            sr.Dispose();

            if (input.IndexOf("#")>-1)
            {
                input = input.Replace("# AR数据文件路径，支持到文件夹的上层目录", "")
                .Replace("# AR data file path, support to the upper directory of the folder", "")
                .Replace("# miner名称，不填使用电脑名", "")
                .Replace("# miner name, use the computer name without filling in", "")
                .Replace("# 在 https://www.hpool.in/center/configuration 找ApiKey", "")
                .Replace("# Find ApiKey at https://www.hpool.in/center/configuration", "")
                .Replace("# 日志配置", "")
                .Replace("# Log configuration", "")
                .Replace("# x-proxy配置", "")
                .Replace("# x-proxy configuration", "")
                .Replace("# 一个局域网内，代理只需要开一台就可以了，如代理所在的机器Ip是192.168.1.88，端口9190", "")
                .Replace("# proxy: 'http://192.168.1.88:9190'", "")
                .Replace("# In a local area network, only one proxy is required. For example, the IP of the machine where the proxy is located is 192.168.1.88 and the port is 9190", "")
                .Replace("# The following configuration is changed to", "")
                .Replace("# proxy: 'http://192.168.1.88:9190'", "")
                .Replace("# socket 或者是http代理配置", "")
                .Replace("# 语言选择", "")
                .Replace("# language selection", "")
                .Replace("# 线路", "")
                .Replace("# 参数配置", "")
                .Replace("# Parameter configuration", "")
                .Replace("# Binding CPU start serial number", "")
                .Replace("# 是否禁用DirectIo", "")
                .Replace("# Whether to disable DirectIo", "")
                .Replace("# randomx 标识 largepages启用需要在电脑系统中开启Hugepage,不配置则使用默认flag", "")
                .Replace("# randomx flag Enabling largepages requires opening Hugepage in the computer system", "")
                .Replace("# largepages,hardaes,fullmem,jit,secure,argon2ssse3,argon2avx2,argon2,If not configured, use the default flag", "")
                .Replace("# randomx flag Enabling largepages requires opening Hugepage in the computer system", "")
                .Replace("# 计算randomx线程，0使用全部线程", "")
                .Replace("# Calculate randomx threads, 0 use all threads", "")
                .Replace("# 下面配置改为", "")
                .Replace("# 绑定CPU开始序号", "")
                .Replace("# E.g http://127.0.0.1:8888 socket5://127.0.0.1:8888", "")
                .Replace("# socket5 or http proxy", "");
            }
            input.Replace(" ", "");


            var pathIndex = input.IndexOf("path:");
            var minerNameIndex = input.IndexOf("minerName:");
            var apiKeyIndex = input.IndexOf("apiKey:");
            var logIndex = input.IndexOf("log:");
            var proxyIndex = input.IndexOf("proxy:");
            var proxy2Index = input.IndexOf("proxy:url:");
            var flagIndex = input.IndexOf("flag:");
            var threadNumIndex = input.IndexOf("threadNum:");

            InPutYamlTextDto resModel = new InPutYamlTextDto();
            resModel.path = input.Substring(pathIndex+5, minerNameIndex- (pathIndex + 5)).Split('-').Where(e=>!string.IsNullOrEmpty(e.Trim())).ToList();
            resModel.minerName = input.Substring(minerNameIndex+10, apiKeyIndex- (minerNameIndex + 10)).Trim();
            resModel.apiKey = input.Substring(apiKeyIndex+7, logIndex-(apiKeyIndex + 7)).Trim();
            resModel.proxy = input.Substring(proxyIndex + 6, proxy2Index - (proxyIndex + 6)).Trim();
            resModel.flag = input.Substring(flagIndex + 5, threadNumIndex - (flagIndex + 5)).Trim();
            resModel.threadNum = input.Substring(threadNumIndex + 10, input.Length - (threadNumIndex + 10)).Trim();
            await SetSettings("YamlPath", filePath);
            return resModel;

        }


        public string GetYamlText()
        {
            string YamlText = @"path:
{0}
minerName:{1}
apiKey: {2}
log:
  lv: info
  path: ./log/
  name: miner.log
url:
  proxy: '{3}'
proxy:
    url: ''
    username: ''
    password: ''
language: cn
line: cn
extraParams:
  cpuIndex: '{4}'
  disableDirectIo: 'false'
  flag: {5}
  threadNum: '{6}'";
            return YamlText;
        }


    }
}
