using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hyperledger.Aries.Models.Records;

namespace AriesWebApp.Models
{
    public class OfferViewModel
    {
        public SchemaRecord Schema { get; set; }
        public string ConnectionId { get; set; }
    }
}
