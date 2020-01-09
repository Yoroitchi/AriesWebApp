using Hyperledger.Aries.Features.BasicMessage;
using Hyperledger.Aries.Features.DidExchange;
using System.Collections.Generic;

namespace AriesWebApp.Models
{
    public class ConnectionsViewModel
    {
        public IEnumerable<ConnectionRecord> Connections { get; set; }

        public IEnumerable<ConnectionRecord> Invitations { get; set; }
    }

    public class ConnectionDetailsViewModel
    {
        public ConnectionRecord Connection { get; set; }

        public IEnumerable<BasicMessageRecord> Messages { get; set; }

        public bool? TrustPingSuccess { get; set; }
    }
}
