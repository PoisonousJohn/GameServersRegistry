using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace GameServerRegistry.Controllers
{
    [Route("api/[controller]")]
    public class ServersController : Controller
    {

        public ServersController(IServersRegistry serversRegistry, ILogger<ServersController> logger)
        {
            _serversRegistry = serversRegistry;
            _log = logger;
        }

        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return _serversRegistry.GetServers().Select(i => i.ToString());
        }

        // POST api/values
        [HttpPost]
        public ActionResult Post([FromBody]int port)
        {
            var ip = Request.HttpContext.Connection.RemoteIpAddress;
            var endpoint = new System.Net.IPEndPoint(ip, port);
            _serversRegistry.RegisterServer(endpoint);
            _log.LogWarning($"Got ping from server {endpoint}");
            return Ok();
        }

        private static readonly Dictionary<System.Net.EndPoint, DateTime> registry
                                                    = new Dictionary<System.Net.EndPoint, DateTime>();
        private readonly IServersRegistry _serversRegistry;
        private readonly ILogger<ServersController> _log;
    }

}
