using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hyperledger.Aries.Models.Records;

namespace AriesWebApp.Models
{
    public class SchemaViewModel
    {
        public IEnumerable<SchemaRecord> Schemas { get; set; }
    }

    public class SchemaDetailViewModel
    {
        public SchemaRecord Schema { get; set; }
    }
}
