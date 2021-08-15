using Buildersoft.Andy.X.Data.Model.Enums;
using Buildersoft.Andy.X.Data.Model.Router.DataStorages;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Buildersoft.Andy.X.Utilities.Validation
{
    public static class HeaderValidations
    {
        public static bool IsDataStorageHeaderRequestValid(IHeaderDictionary headers)
        {
            if (headers["x-andy-storage-name"].ToString() == ""
                || headers["x-andy-storage-environment"].ToString() == ""
                || headers["x-andy-storage-type"].ToString() == ""
                || headers["x-andy-storage-status"].ToString() == "")

                return false;

            //Checking environment, type and status for data storage
            try
            {
                DataStorageEnvironment env = (DataStorageEnvironment)Enum.Parse(typeof(DataStorageEnvironment), headers["x-andy-storage-environment"].ToString());
                DataStorageType type = (DataStorageType)Enum.Parse(typeof(DataStorageType), headers["x-andy-storage-type"].ToString());
                DataStorageStatus status = (DataStorageStatus)Enum.Parse(typeof(DataStorageStatus), headers["x-andy-storage-status"].ToString());
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public static bool IsReaderHeaderRequestValid(IHeaderDictionary headers)
        {
            if (headers["x-andy-x-tenant"].ToString() == ""
                || headers["x-andy-x-product"].ToString() == ""
                || headers["x-andy-x-component"].ToString() == ""
                || headers["x-andy-x-book"].ToString() == ""
                || headers["x-andy-x-reader"].ToString() == "")
                return false;

            try
            {
                ReaderTypes readerType =  (ReaderTypes)Enum.Parse(typeof(ReaderTypes), headers["x-andy-x-readertype"].ToString());
                ReaderAs readerAs = (ReaderAs)Enum.Parse(typeof(ReaderAs), headers["x-andy-x-readeras"].ToString());
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}
