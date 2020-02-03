using System.Collections.Generic;
using Hyperledger.Aries.Models.Records;

namespace AriesWebApp.Models
{
    public class CreateOfferViewModel
    {
        public IEnumerable<SchemaRecord> Schemas { get; set; }
        public string ConnectionId { get; set; }
    }
}
