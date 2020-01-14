using System.Collections.Generic;
using Hyperledger.Aries.Features.IssueCredential;

namespace AriesWebApp.Models
{
    public class CredentialsViewModel
    {
        public IEnumerable<CredentialRecord> Credentials { get; set; }
    }
}
