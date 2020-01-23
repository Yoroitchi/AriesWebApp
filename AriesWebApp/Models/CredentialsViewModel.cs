using System.Collections.Generic;
using Hyperledger.Aries.Features.IssueCredential;
using Hyperledger.Aries.Features.PresentProof;



namespace AriesWebApp.Models
{
    public class CredentialsViewModel
    {
        public IEnumerable<CredentialRecord> Credentials { get; set; }

    }
}
