using Buildersoft.Andy.X.Core.Abstractions.Orchestrators;
using Buildersoft.Andy.X.IO.Locations;
using Buildersoft.Andy.X.Model.App.Topics;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Buildersoft.Andy.X.Core.Synchronizers
{
    public class TopicSynchronizerProcess
    {
        private readonly ILogger<TopicSynchronizerProcess> _logger;

        public string Tenant { get; set; }
        public string Product { get; set; }
        public string Component { get; set; }
        public Topic Topic { get; set; }

        public Process SynchronizerProcess { get; private set; }
        public bool IsProcessRunning { get; private set; }


        public TopicSynchronizerProcess(ILogger<TopicSynchronizerProcess> logger)
        {
            Topic = new Topic();
            SynchronizerProcess = new Process();
            IsProcessRunning = false;
            _logger = logger;
        }

        public void StartProcess()
        {
            if (IsProcessRunning != true)
            {
                IsProcessRunning = true;
                var task = Task.Run(() => RunProcess());
            }
        }

        public void StopProcess()
        {
            IsProcessRunning = false;
        }

        private void RunProcess()
        {
            while (IsProcessRunning == true)
            {
                SynchronizerProcess = new Process();
                SynchronizerProcess.StartInfo = new ProcessStartInfo()
                {
                    FileName = "dotnet",
                    UseShellExecute = false,
                    CreateNoWindow = false,
                };

                SynchronizerProcess.StartInfo.ArgumentList.Add(Path.Join(ConfigurationLocations.GetRootDirectory(), "Andy.X.Storage.Synchronizer.dll"));
                SynchronizerProcess.StartInfo.ArgumentList.Add(Tenant);
                SynchronizerProcess.StartInfo.ArgumentList.Add(Product);
                SynchronizerProcess.StartInfo.ArgumentList.Add(Component);
                SynchronizerProcess.StartInfo.ArgumentList.Add(JsonConvert.SerializeObject(Topic));

                SynchronizerProcess.OutputDataReceived += SynchronizerProcess_OutputDataReceived;
                SynchronizerProcess.Start();
                SynchronizerProcess.WaitForExit();

                SynchronizerProcess.OutputDataReceived -= SynchronizerProcess_OutputDataReceived;
                SynchronizerProcess = null;

                var filesCount = new DirectoryInfo(TenantLocations.GetTempMessageToStoreTopicRootDirectory(Tenant, Product, Component, Topic.Name)).GetFiles().Count();
                if (filesCount == 0)
                {
                    IsProcessRunning = false;
                }
            }
        }

        private void SynchronizerProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            _logger.LogInformation(e.Data);
        }
    }
}
