using System;
using System.Diagnostics;
using System.Windows.Forms;
using 子端.Help;

namespace 子端
{
    public partial class MainFrom : Form
    {
        
        public static Process p { get; set; }
        AppSettingsHelps appSettings;
        public MainFrom()
        {
            InitializeComponent();
            StartApi();
            appSettings = new AppSettingsHelps();
            string cmdStr = "";
            p = CMDHelps.ExeCommand(cmdStr);
            p.Start();
            p.StandardInput.WriteLine(cmdStr);
            p.BeginOutputReadLine();
            p.BeginErrorReadLine();
            p.ErrorDataReceived += p_ErrorDataReceived;
            p.OutputDataReceived += p_OutputDataReceived;
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
        private delegate void SetTextCallback(string text);
        private void SetText(string text)
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
        public void p_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            //往配置文件写入
            appSettings.SetSettings("NowText", e.Data);
            SetText(e.Data);
        }

        private void uiButton1_Click(object sender, EventArgs e)
        {

        }

        private void uiButton2_Click(object sender, EventArgs e)
        {

        }
        
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }


        
    }
}
