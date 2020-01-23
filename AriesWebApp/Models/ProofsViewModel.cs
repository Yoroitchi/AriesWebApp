using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hyperledger.Aries.Features.PresentProof;
using Hyperledger.Aries.Features.IssueCredential;

namespace AriesWebApp.Models
{
    public class ProofsViewModel
    {
        public IEnumerable<ProofRecord> Proofs { get; set; }

    }

    public class ProofsDetailViewModel
    {
        public ProofRecord ProofRecord { get; set; }
    }
}
