using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Buildersoft.Andy.X.Data.Model;
using Buildersoft.Andy.X.Data.Model.DataStorages;
using Buildersoft.Andy.X.Data.Model.Router.DataStorages;
using Buildersoft.Andy.X.IO.Files;
using Buildersoft.Andy.X.Logic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Buildersoft.Andy.X.Controllers
{
    [Route("datastorages")]
    [ApiController]
    [RequireHttps]
    public class StorageController : ControllerBase
    {
        private readonly StorageMemoryRepository _memoryRepository;

        public StorageController(StorageMemoryRepository memoryRepository)
        {
            _memoryRepository = memoryRepository;
        }

        [HttpGet("")]
        public ActionResult<List<DataStorage>> GetDataStorageServers()
        {

            return Ok(ConfigFile.GetDataStoragesFromConfig());
        }

        [HttpPost("add")]
        public ActionResult<string> AddDataStorageServer([FromBody] DataStorage dataStorage)
        {
            List<DataStorage> dataStorages = ConfigFile.GetDataStoragesFromConfig();
            var dsExists = dataStorages.Where(x => x.DataStorageName == dataStorage.DataStorageName).FirstOrDefault();
            if (dsExists != null)
                return BadRequest("This storage is registered, you have to remove if you want to register with this name");
            dataStorage.DataStoregeId = Guid.NewGuid();

            dataStorages.Add(dataStorage);
            bool result = ConfigFile.SetDataStoragesInConfig(dataStorages);
            if (result)
                return Created("", "Andy X Data Storage Server has been configured.");
            return BadRequest("Data Storage Server has not been configured");
        }

        [HttpGet("ping")]
        public ActionResult<string> PingAndyX()
        {
            return Ok("Ok");
        }

        [HttpGet("currentstate")]
        public ActionResult<ConcurrentDictionary<string, Tenant>> GetCurrentState()
        {
            return _memoryRepository.GetTenants();
        }
    }
}
