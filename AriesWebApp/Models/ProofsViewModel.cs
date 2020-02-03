using System.Collections.Generic;
using Hyperledger.Aries.Features.PresentProof;


namespace AriesWebApp.Models
{
    public class ProofsViewModel
    {
        public IEnumerable<ProofRecord> Proofs { get; set; }

    }

    public class ProofsDetailViewModel
    {
        public PartialProof ProofPartial { get; set; }
    }
}
