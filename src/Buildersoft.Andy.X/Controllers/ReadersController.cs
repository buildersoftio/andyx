using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Buildersoft.Andy.X.Data.Model;
using Buildersoft.Andy.X.Data.Model.Enums;
using Buildersoft.Andy.X.Logic;
using Buildersoft.Andy.X.Logic.Interfaces.Readers;
using Buildersoft.Andy.X.Logic.Readers;
using Buildersoft.Andy.X.Utilities.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Buildersoft.Andy.X.Controllers
{
    [Route("api/v1")]
    [ApiController]
    [RequireHttps]
    [Authorize(TenantOnly = true)]
    public class ReadersController : ControllerBase
    {
        private IReaderLogic _readerLogic;
        private readonly StorageMemoryRepository _memoryRepository;
        public ReadersController(StorageMemoryRepository memoryRepository)
        {
            _memoryRepository = memoryRepository;
        }

        [HttpGet("tenants/{tenantName}/products/{productName}/components/{componentName}/books/{bookName}/readers")]
        public ActionResult<List<Reader>> GetAllReaders(string tenantName, string productName, string componentName, string bookName)
        {
            _readerLogic = new ReaderLogic(
                _memoryRepository.GetReaders(tenantName, productName, componentName, bookName));

            return Ok(_readerLogic.GetAllReaders());
        }

        [HttpGet("tenants/{tenantName}/products/{productName}/components/{componentName}/books/{bookName}/readers/{readerName}")]
        public ActionResult<Reader> GetReader(string tenantName, string productName, string componentName, string bookName, string readerName)
        {
            _readerLogic = new ReaderLogic(
                _memoryRepository.GetReaders(tenantName, productName, componentName, bookName));

            Reader reader = _readerLogic.GetReader(readerName);
            if (reader != null)
                return Ok(reader);

            return NotFound("READER_NOT_FOUND");
        }

        [HttpPost("tenants/{tenantName}/products/{productName}/components/{componentName}/books/{bookName}/readers/{readerName}")]
        public ActionResult<Reader> CreateReader(string tenantName, string productName, string componentName, string bookName, string readerName)
        {
            _readerLogic = new ReaderLogic(
                _memoryRepository.GetReaders(tenantName, productName, componentName, bookName));

            if (_readerLogic.GetReader(readerName) != null)
                return BadRequest("READER_EXISTS");

            Reader reader = _readerLogic.CreateReader(readerName, ReaderTypes.Exclusive);
            if (reader != null)
                return Ok(reader);

            return BadRequest("READER_NOT_CREATED");
        }

        [HttpPost("tenants/{tenantName}/products/{productName}/components/{componentName}/books/{bookName}/readers/{readerName}/type/{readerType}")]
        public ActionResult<Reader> CreateReader(string tenantName, string productName, string componentName, string bookName, string readerName, string readerType)
        {
            _readerLogic = new ReaderLogic(
                _memoryRepository.GetReaders(tenantName, productName, componentName, bookName));

            if (_readerLogic.GetReader(readerName) != null)
                return BadRequest("READER_EXISTS");

            ReaderTypes _readerType = (ReaderTypes)Enum.Parse(typeof(ReaderTypes), readerType);
            Reader reader = _readerLogic.CreateReader(readerName, _readerType);
            if (reader != null)
                return Ok(reader);

            return BadRequest("READER_NOT_CREATED");
        }
    }
}