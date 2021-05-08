using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MinerRestartService
{
    public partial class Service1 : ServiceBase
    {
        public bool _shouldRun = true;
        public DateTime _lastDataTimeExecuted;
        public int _previousProcessId;

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // Loop through and check last restarted time
            while (_shouldRun == true)
            {
                var nextExecuteTime = _lastDataTimeExecuted.AddHours(Variables.RestartEveryHours);

                if (DateTime.Now > nextExecuteTime)
                {
                    StartMiner();
                    Thread.Sleep(500000);
                }
            }
        }

        protected override void OnStop()
        {
            _shouldRun = false;
        }

        private void StartMiner()
        {
            if (_previousProcessId > 0)
            {
                var existingProcess = Process.GetProcessById(_previousProcessId);
                existingProcess.Kill();
            }
            
            ProcessStartInfo processInfo;
            Process process;

            processInfo = new ProcessStartInfo(Variables.MinerExeLocation);
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            // *** Redirect the output ***
            processInfo.RedirectStandardError = true;
            processInfo.RedirectStandardOutput = true;

            process = Process.Start(processInfo);
            _previousProcessId = process.Id;
        }
    }
}
