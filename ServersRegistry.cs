using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace GameServerRegistry
{
    public interface IServersRegistry
    {
        IEnumerable<IPEndPoint> GetServers();
        void RegisterServer(IPEndPoint server);
    }
    public class ServersRegistry : IServersRegistry
    {
        public ServersRegistry(ILogger<ServersRegistry> log)
        {
            _checkServers = new Timer(RemoveDeadServers, null, 0, 1000);
            _log = log;
        }

        public IEnumerable<IPEndPoint> GetServers()
        {
            return _servers.Keys;
        }

        public void RegisterServer(IPEndPoint server)
        {
            lock (_servers)
            {
                _servers[server] = DateTime.Now;
            }
        }

        private void RemoveDeadServers(object state)
        {
            lock (_servers)
            {
                var now = DateTime.Now;
                var serversToRemove = new List<IPEndPoint>();
                foreach (var server in _servers)
                {
                    if ((now - server.Value).TotalSeconds >= SERVER_TIMEOUT_SECONDS)
                    {
                        serversToRemove.Add(server.Key);
                    }
                }

                foreach (var server in serversToRemove)
                {
                    _log.LogCritical($"Server {server} timed out. Removing");
                    _servers.Remove(server);
                }
            }
        }

        private Dictionary<IPEndPoint, DateTime> _servers = new Dictionary<IPEndPoint, DateTime>();
        private Timer _checkServers;
        private const int SERVER_TIMEOUT_SECONDS = 10;
        private readonly ILogger<ServersRegistry> _log;
    }
}
