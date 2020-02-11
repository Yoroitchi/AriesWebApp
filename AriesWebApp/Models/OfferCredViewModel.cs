using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hyperledger.Aries.Features.DidExchange;
using Hyperledger.Aries.Models.Records;

namespace AriesWebApp.Models
{
    public class OfferCredViewModel
    {
        public SchemaRecord Schema { get; set; }
        public DefinitionRecord CredDef { get; set; }
        public ConnectionRecord Connection { get; set; }
    }
}
