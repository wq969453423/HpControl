using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using 子端.Help;

namespace 子端
{
    public partial class MainFrom : Form
    {
        public static MainFrom mainfromInstance;
        public static Process p { get; set; }
        AppSettingsHelps appSettings;
        public MainFrom()
        {
            InitializeComponent();
            StartApi();
            appSettings = new AppSettingsHelps();
            p = CMDHelps.ExeCommand();
            p.Start();
            p.BeginOutputReadLine();
            p.BeginErrorReadLine();
            p.ErrorDataReceived += p_ErrorDataReceived;
            p.OutputDataReceived += p_OutputDataReceived;

            //开机自启
            
            kjzq();
            mainfromInstance = this;
        }

        private async void kjzq() {
            var SelfStarting = await appSettings.GetSettings("SelfStarting");
            if (SelfStarting == "false") {
                string execPath = Application.ExecutablePath;
                RegistryKey rk = Registry.LocalMachine;
                RegistryKey rk2 = rk.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");
                rk2.SetValue("MyExec", execPath);
                await appSettings.SetSettings("SelfStarting", "true");
                rk2.Dispose();
                rk.Dispose();
            }
            //string.Format("[注册表操作]添加注册表键值：path = {0}, key = {1}, value = {2} 成功", rk2.Name, "TuniuAutoboot", execPath));

        }

        private async void StartApi() {
            子端.Controller.HttpServer httpServer = new 子端.Controller.HttpServer("127.0.0.1", 8081);
            await httpServer.StartHttpServer();
        }


        public Process GetProcess() {
            return  p;
        }

        public void p_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            
        }
        public delegate void SetTextCallback(string text);
        public void SetText(string text)
        {
            // InvokeRequired需要比较调用线程ID和创建线程ID
            // 如果它们不相同则返回true
            if (this.textBox1.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.textBox1.AppendText(text); 
            }
        }
        public async void p_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            //往配置文件写入
            
            if (e.Data.Contains("H/s"))
            {
                await appSettings.SetSettings("NowText", e.Data);
                
            }
            SetText("\r\n"+e.Data);

        }

        private async void uiButton2_Click(object sender, EventArgs e)
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
            Thread.Sleep(150);
            var path = await appSettings.GetSettings("YamlPath");
            p.StandardInput.WriteLine(path + "hpool-miner-ar-console.exe");

        }

        private void uiButton1_Click(object sender, EventArgs e)
        {
            using (var process = CMDHelps.ExeCommand())
            {
                process.Start();
                process.OutputDataReceived += p_OutputDataReceived;
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.StandardInput.WriteLine("taskkill /im hpool-miner-ar-console.exe /f");
                process.Close();
            };
        }
        
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }


        
    }
}
