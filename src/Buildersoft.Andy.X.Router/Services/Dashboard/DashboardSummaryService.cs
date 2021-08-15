using Buildersoft.Andy.X.Data.Model.Router.Dashboard;
using Buildersoft.Andy.X.Data.Model.Router.DataStorages;
using Buildersoft.Andy.X.Data.Model.Router.Readers;
using Buildersoft.Andy.X.Logic.Repositories.Interfaces.Dashboard;
using Buildersoft.Andy.X.Router.Hubs.Dashboard;
using Buildersoft.Andy.X.Router.Hubs.Interfaces.Dashboard;
using Buildersoft.Andy.X.Router.Repositories;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace Buildersoft.Andy.X.Router.Services.Dashboard
{
    public class DashboardSummaryService
    {
        private readonly ILogger<DashboardSummaryService> _logger;
        private readonly IHubContext<DashboardHub, IDashboardHub> _hub;
        private readonly IHubRepository<DashboardUser> _userRepository;
        private readonly IHubRepository<DataStorage> _activeDataStorages;
        private readonly IHubRepository<Reader> _activeReaders;

        private readonly IDashboardSummaryMemoryRepository _dashboardSummaryMemoryRepository;

        private Timer timerSendSummaryReportToDashboard;

        public DashboardSummaryService(
            ILogger<DashboardSummaryService> logger,
            IHubContext<DashboardHub, IDashboardHub> hub,
            IHubRepository<DashboardUser> userRepository,
            IHubRepository<DataStorage> activeDataStorages,
            IHubRepository<Reader> activeReaders,
            IDashboardSummaryMemoryRepository dashboardSummaryMemoryRepository)
        {
            _logger = logger;

            _hub = hub;
            _userRepository = userRepository;
            _activeDataStorages = activeDataStorages;
            _activeReaders = activeReaders;
            _dashboardSummaryMemoryRepository = dashboardSummaryMemoryRepository;

            InitializeTimer();
        }
        private void InitializeTimer()
        {
            timerSendSummaryReportToDashboard = new Timer(new TimeSpan(0, 0, 5).TotalMilliseconds);
            timerSendSummaryReportToDashboard.Elapsed += TimerSendSummaryReportToDashboard_Elapsed;
            timerSendSummaryReportToDashboard.AutoReset = true;

            timerSendSummaryReportToDashboard.Start();
        }

        private void TimerSendSummaryReportToDashboard_Elapsed(object sender, ElapsedEventArgs e)
        {
            timerSendSummaryReportToDashboard.Stop();

            UpdateSummaryReportInRepository();
            SendSummaryReport();
            
            timerSendSummaryReportToDashboard.Start();
        }

        private void UpdateSummaryReportInRepository()
        {
            _dashboardSummaryMemoryRepository.Update(new Data.Model.Dashboard.SummaryReport()
            {
                ActiveDataStorages = _activeDataStorages.GetAll().Count,
                ActiveReaders = _activeReaders.GetAll().Count,
                AvgResponseTimeInMs = new Random().Next(3, 6),
                AvailableStorage = 100,

                ActiveDataStorageDetails = new List<DataStorage>(_activeDataStorages.GetAll().Values.ToList()),
                ActiveReaderDetails = new List<Reader>(_activeReaders.GetAll().Values.ToList()),

                LastUpdate = DateTime.Now,
            });
        }

        private void SendSummaryReport()
        {
            foreach (var user in _userRepository.GetAll())
            {
                _hub.Clients.Client(user.Key).SummaryReportReceived(_dashboardSummaryMemoryRepository.Get());
            }
        }
    }
}
