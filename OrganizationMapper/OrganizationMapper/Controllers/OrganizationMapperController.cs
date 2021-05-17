using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OrganizationMapper.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrganizationMapper.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrganizationMapperController : ControllerBase
    {
        public static IConfiguration _configuration;
        private readonly ILogger<OrganizationMapperController> _logger;

        public OrganizationMapperController(ILogger<OrganizationMapperController> logger, IConfiguration config)
        {
            _logger = logger;
            _configuration = config;
        }

        [HttpGet]
        public string Get()
        {
            try
            {
                string dbConn = _configuration.GetSection("ConnectionStrings").GetSection("StorageConnection").Value;

                var entitiesList = new OrganizationLogic().GetOrganizationData(dbConn);
                
                var json = new OrganizationLogic().GetOrganizationDataAsJson(entitiesList);

                return json;
            }
            catch (Exception e)
            {
                return $"Something went wrong: {e.Message}";
            }
        }

        [HttpPost]
        public string Post(IFormFile file)
        {
            try
            {
                string dbConn = _configuration.GetSection("ConnectionStrings").GetSection("StorageConnection").Value;
                new OrganizationLogic().UploadFileToBlobStorage(dbConn, file);

                return "File uploaded";
            }
            catch (Exception e)
            {
                return $"Something went wrong! Exception: {e.Message}";
            }
         
        }
    }
}
