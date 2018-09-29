using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;

namespace nexus_web_uploader_api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class RepositoriosController : ControllerBase
    {
        private IConfiguration config;

        public RepositoriosController(IConfiguration iConfig)
        {
            config = iConfig;
        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            return config.GetSection("Repositorios").Get<string[]>();
        }
    }
}
