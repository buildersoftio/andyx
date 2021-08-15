using Buildersoft.Andy.X.Data.Model.Router.DataStorages;
using System;
using System.Collections.Generic;
using System.Text;

namespace Buildersoft.Andy.X.Data.Model.Dashboard
{
    public class SummaryReport
    {
        public int ActiveDataStorages { get; set; }
        public int ActiveReaders { get; set; }
        public int AvgResponseTimeInMs { get; set; }
        public int AvailableStorage { get; set; }

        public List<DataStorage> ActiveDataStorageDetails { get; set; }
        public List<Router.Readers.Reader> ActiveReaderDetails { get; set; }

        public DateTime LastUpdate { get; set; }

        public SummaryReport()
        {
            ActiveDataStorageDetails = new List<DataStorage>();
            ActiveReaderDetails = new List<Router.Readers.Reader>();
        }
    }
}
