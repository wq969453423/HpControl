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
        public string GetSettings(string key) {
            //根据KEY读取值
            string Str = config.AppSettings.Settings[key].Value;
            return Str;
        }


        public void SetSettings(string key, string SetValue)
        {
            config.AppSettings.Settings[key].Value = SetValue;
            SaveSettings();
        }

        public void AddSettings(string key,string SetValue)
        {
            config.AppSettings.Settings.Add(key, SetValue);
            SaveSettings();
        }

        public void DelSettings(string key)
        {
            config.AppSettings.Settings.Remove(key);
            SaveSettings();
        }

        private void SaveSettings()
        {
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("AppSettings");
        }


        public bool WriteFile(string filePath,string content, string fileName= "config.yaml")
        {

            FileInfo fi = new FileInfo(filePath+ fileName);

            if (fi.Exists) {
                fi.Delete();
            }
            StreamWriter sw = fi.CreateText();
            sw.WriteLine(content);
            sw.Flush();
            sw.Close();
            sw.Dispose();
            return true;
        }

        public InPutYamlTextDto ReadFile(string filePath, string fileName = "config.yaml")
        {

            StreamReader sr = new StreamReader(filePath + fileName);
            string input = sr.ReadToEnd();
            sr.Close();
            sr.Dispose();

            var pathIndex = input.IndexOf("path:");

            var minerNameIndex = input.IndexOf("minerName:");

            var apiKeyIndex = input.IndexOf("apiKey:");

            InPutYamlTextDto resModel = new InPutYamlTextDto();
            resModel.path = input.Substring(pathIndex, minerNameIndex).Split('-').ToList();
            return default;

        }


        public string GetYamlText()
        {
            string YamlText = @"path:
@{0}
minerName:@{1}
apiKey: @{2}
log:
  lv: info
  path: ./log/
  name: miner.log
url:
  proxy: '@{3}'
proxy:
    url: ''
    username: ''
    password: ''
language: cn
line: cn
extraParams:
  cpuIndex: '@{4}'
  disableDirectIo: 'false'
  flag: @{5}
  threadNum: '@{6}'";
            return YamlText;
        }


    }
}
