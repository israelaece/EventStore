using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using EventStore.ClientAPI;
using EventStore.Common.Utils;
using EventStore.Core.Messages;
using EventStore.Transport.Http;
using EventStore.Transport.Http.Codecs;
using NUnit.Framework;
using EventStore.Core.Tests.ClientAPI;
using EventStore.Core.Tests.Helpers;

namespace EventStore.Core.Tests.Services.Transport.Http
{
    [TestFixture]
    public class stat_controller_should : SpecificationWithMiniNode
    {
        private PortableServer _portableServer;
        private IPEndPoint _serverEndPoint;

        [TestFixtureSetUp]
        public void SetUp()
        {
            var port = PortsHelper.GetAvailablePort(IPAddress.Loopback);
            _serverEndPoint = new IPEndPoint(IPAddress.Loopback, port);
            _portableServer = new PortableServer(_serverEndPoint);
            _portableServer.SetUp();
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            _portableServer.TearDown();
        }

        protected override void When()
        {
        }

        [Test]
        public void should_return_bytes_sent_and_received_stats_for_each_connection ()
        {
            var url = _HttpEndPoint.ToHttpUrl("/stats/tcp");
            Func<HttpResponse, bool> verifier = response => {
                List<MonitoringMessage.TcpConnectionStats> stats = Codec.Json.From<List<MonitoringMessage.TcpConnectionStats>>(response.Body);
                var hasBytesSent = stats.Any(stat => stat.TotalBytesSent > 0);
                var hasBytesReceived = stats.Any(stat => stat.TotalBytesReceived > 0);
                return stats.Count == 2 && hasBytesSent && hasBytesReceived;
            };
            var settings = ConnectionSettings.Create ();
            using (var conn = EventStoreConnection.Create(settings, _node.TcpEndPoint))
            {
                conn.ConnectAsync().Wait();
                var testEvent = new EventData(Guid.NewGuid(),"TestEvent",true,Encoding.ASCII.GetBytes("{'Test' : 'OneTwoThree'}"),null);
                conn.AppendToStreamAsync("tests",ExpectedVersion.Any,testEvent).Wait();
                var result = _portableServer.StartServiceAndSendRequest(HttpBootstrap.RegisterStat, url, verifier);
                Assert.IsTrue(result.Item1);
            }
        }
    }
}
