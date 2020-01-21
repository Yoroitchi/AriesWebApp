using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hyperledger.Aries.Features.PresentProof;

namespace AriesWebApp.Models
{
    public class ProofsViewModel
    {
        public IEnumerable<ProofRecord> Proofs { get; set; }
    }
}
