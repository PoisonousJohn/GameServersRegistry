using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace GameServerRegistry.Controllers
{
    public class SimpleEndpoint
    {
        public string ip;
        public int port;
    }
    public class ServerList
    {
        public IEnumerable<string> list;
    }

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
        public ServerList Get()
        {
            return new ServerList { list = _serversRegistry.GetServers().Select(i => i.ToString()) };
        }

        // POST api/values
        [HttpPost]
        public ActionResult Post([FromBody]SimpleEndpoint host)
        {
            var endpoint = new System.Net.IPEndPoint(
                System.Net.IPAddress.Parse(host.ip),
                host.port
            );
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
