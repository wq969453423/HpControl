using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using 子端.Help;

namespace 子端
{
    public partial class MainFrom : Form
    {
        public static MainFrom mainfromInstance;
        public static Process p1 { get; set; }
        public static Process p2 { get; set; }
        public static Process p3 { get; set; }
        public static Process p4 { get; set; }

        AppSettingsHelps appSettings;
        public MainFrom()
        {
            InitializeComponent();
            StartApi();
            appSettings = new AppSettingsHelps();

            p1Start();
            p2Start();
            p3Start();
            p4Start();

            //开机自启
            kjzq();
            mainfromInstance = this;
        }

        private async void kjzq() {
            var SelfStarting = appSettings.GetSettings("SelfStarting");
            if (SelfStarting == "false") {
                string execPath = Application.ExecutablePath;
                RegistryKey rk = Registry.LocalMachine;
                RegistryKey rk2 = rk.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");
                rk2.SetValue("MyExec", execPath);
                appSettings.SetSettings("SelfStarting", "true");
                rk2.Dispose();
                rk.Dispose();
            }
            //string.Format("[注册表操作]添加注册表键值：path = {0}, key = {1}, value = {2} 成功", rk2.Name, "TuniuAutoboot", execPath));

        }

        private async void StartApi() {
            子端.Controller.HttpServer httpServer = new 子端.Controller.HttpServer("127.0.0.1", 8081);
            await httpServer.StartHttpServer();
        }


        public void p1Start() {
            p1 = CMDHelps.ExeCommand();
            p1.Start();
            p1.BeginOutputReadLine();
            p1.BeginErrorReadLine();
            p1.OutputDataReceived += p_OutputDataReceived1;
        }

        public void p2Start()
        {
            p2 = CMDHelps.ExeCommand();
            p2.Start();
            p2.BeginOutputReadLine();
            p2.BeginErrorReadLine();
            p2.OutputDataReceived += p_OutputDataReceived2;
        }

        public void p3Start()
        {
            p3 = CMDHelps.ExeCommand();
            p3.Start();
            p3.BeginOutputReadLine();
            p3.BeginErrorReadLine();
            p3.OutputDataReceived += p_OutputDataReceived3;
        }

        public void p4Start()
        {
            p4 = CMDHelps.ExeCommand();
            p4.Start();
            p4.BeginOutputReadLine();
            p4.BeginErrorReadLine();
            p4.OutputDataReceived += p_OutputDataReceived4;
        }

        public List<Process> GetProcess() {
            List <Process> processList = new List < Process >();
            processList.Add(p1);
            processList.Add(p2);
            processList.Add(p3);
            processList.Add(p4);
            return processList;
        }


        public delegate void SetTextCallback(string text);
        public void SetText(string text)
        {
            // InvokeRequired需要比较调用线程ID和创建线程ID
            // 如果它们不相同则返回true
            if (this.textBox1.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] {text + "\r\n" });
            }
            else
            {
                this.textBox1.AppendText(text+ "\r\n"); 
            }
        }


        #region cmd触发

        public void setCapacity(string key,string data) 
        {
            decimal CalculatingPower = 0;
            var capacityIndex = data.IndexOf("capacity=\"");
            var HKIndex = data.IndexOf("KH/s\"");
            if (capacityIndex > 0 && HKIndex > 0)
            {
                try
                {
                    CalculatingPower = decimal.Parse(data.Substring(capacityIndex + 10, HKIndex - (capacityIndex + 1)));
                }
                catch (Exception)
                {
                    CalculatingPower = 0;
                }

            }
            appSettings.SetSettings(key, CalculatingPower.ToString());
            SetText("\r\n" + data);
        }

        public void p_OutputDataReceived1(object sender, DataReceivedEventArgs e)
        {
            if (e.Data!=null)
            {
                //往配置文件写入
                if (e.Data.Contains("KH/s"))
                {
                    setCapacity("NowText1", e.Data);
                    SetText("1算力：" + e.Data);
                }
                else
                {
                    SetText(e.Data);
                }

            }

        }
        public void p_OutputDataReceived2(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                if (e.Data.Contains("KH/s"))
                {
                    setCapacity("NowText2", e.Data);
                    SetText("2算力：" + e.Data);
                }
            }
        }
        public void p_OutputDataReceived3(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                if (e.Data.Contains("KH/s"))
                {
                    setCapacity("NowText3", e.Data);
                    SetText("3算力：" + e.Data);
                }
            }
            
        }
        public void p_OutputDataReceived4(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                if (e.Data.Contains("KH/s"))
                {
                    setCapacity("NowText4", e.Data);
                    SetText("4算力：" + e.Data);
                }
            }
        }


        #endregion


        private void textBox1_TextChanged(object sender, EventArgs e)
        {



        }


        
    }
}
