using System.Collections.Generic;
using Hyperledger.Aries.Features.IssueCredential;
using Hyperledger.Aries.Models.Records;

namespace AriesWebApp.Models
{
    public class CredentialsViewModel
    {
        public IEnumerable<CredentialRecord> Credentials { get; set; }
    }
}
