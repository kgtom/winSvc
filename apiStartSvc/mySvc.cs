using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
using System.Configuration;


namespace apiStartSvc
{
    public partial class mySvc : ServiceBase
    {

        private Timer time = new Timer();
        string processPath = ConfigurationManager.AppSettings["processPath"];
        string reStartTime = ConfigurationManager.AppSettings["reStartTime"];
        string processName = ConfigurationManager.AppSettings["processName"];
        public mySvc()
        {
            InitializeComponent();
        }

        //服务开启
        protected override void OnStart(string[] args)
        {
#if DEBUG
            if (!Debugger.IsAttached)
                Debugger.Launch();
            Debugger.Break();

#endif

            WriteLog("服务启动，时间：" + DateTime.Now.ToString("HH:mm:ss") + "rn");
            time.Elapsed += new ElapsedEventHandler(ApiStartEvent);
            time.Interval = 60 * 1000;//时间间隔为5秒
            time.Start();
        }
        /// <summary>
        /// 服务重启
        /// </summary>
        protected override void OnContinue()
        {
#if DEBUG
            if (!Debugger.IsAttached)
                Debugger.Launch();
            Debugger.Break();
#endif
            WriteLog("服务恢复，时间：" + DateTime.Now.ToString("HH:mm:ss") + "rn");
            base.OnContinue();
        }

        /// <summary>
        /// 计算机关闭
        /// </summary>
        protected override void OnShutdown()
        {
            WriteLog("计算机关闭，时间：" + DateTime.Now.ToString("HH:mm:ss") + "rn");
            base.OnShutdown();
        }
        /// <summary>
        /// 服务停止
        /// </summary>
        protected override void OnStop()
        {
#if DEBUG
            if (!Debugger.IsAttached)
                Debugger.Launch();
            Debugger.Break();

#endif
            try
            {
                foreach (Process pro in Process.GetProcesses())
                {
                    if (pro.ProcessName.Equals(processName))

                    {

                        pro.Kill();
                        WriteLog("服务停止，时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                    }

                }
                //todo 邮件提醒

            }
            catch (Exception ex)
            {
                WriteLog("服务停止,出现异常，时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "ex:" + ex.Message);

            }

        }
        /// <summary>
        /// 启动服务Event
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void ApiStartEvent(object source, System.Timers.ElapsedEventArgs e)
        {
            time.Enabled = false;
            string result = string.Empty;
            try
            {
                Process[] currentProcess = Process.GetProcessesByName(processName);
                if (currentProcess.Length > 0)
                {
                    if (DateTime.Now.ToString("HH:mm").Equals(reStartTime))
                    {
                        currentProcess[0].StartInfo.Verb = "runas";
                        currentProcess[0].Kill();
                        // currentProcess[0].CloseMainWindow();
                        result = "服务定时关闭，时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                        System.Threading.Thread.Sleep(1000 * 30);//等待30s
                        StartProcess();
                        result = "服务定时启动，执行成功，时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    }
                }
                if (currentProcess.Length == 0)
                {
                    StartProcess();
                    //.........
                    result = "服务启动，执行成功，时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                }


            }
            catch (Exception ex)
            {
                result = "执行失败，原因：" + ex.Message;
            }
            finally
            {
                WriteLog(result);
                time.Enabled = true;
            }
        }


        /// <summary>
        /// 启动api
        /// </summary>
        private void StartProcess()
        {
            System.Diagnostics.Process exep = new System.Diagnostics.Process();
            exep.StartInfo.FileName = processPath;
            exep.StartInfo.Arguments = "";
            exep.StartInfo.CreateNoWindow = true;
            exep.StartInfo.UseShellExecute = true;
            //SetWindowText(exep.MainWindowHandle, "MyApi");
            exep.Start();

        }
        /// <summary>
        /// 日志记录
        /// </summary>
        /// <param name="logInfo"></param>
        private void WriteLog(string logInfo)
        {
            try
            {
                if (string.IsNullOrEmpty(logInfo)) { return; }
                string logDirectory = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
                if (!Directory.Exists(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                }
                string filePath = logDirectory + "\\" + DateTime.Now.ToString("yyyy - MM - dd") + ".txt";
                File.AppendAllText(filePath, logInfo);
            }
            catch
            {

            }
        }
    }
}
