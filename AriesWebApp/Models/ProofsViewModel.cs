using System.Collections.Generic;
using Hyperledger.Aries.Features.PresentProof;
using Newtonsoft.Json.Linq;

namespace AriesWebApp.Models
{
    public class ProofsViewModel
    {
        public IEnumerable<ProofRecord> Proofs { get; set; }

    }

    public class ProofsDetailViewModel
    {
        public ProofRecord ProofRecord { get; set; }
        public ProofRequest ProofRequest { get; set; }
        public PartialProof PartialProof { get; set; }
    }
}
