using System.Collections.Generic;
using Hyperledger.Aries.Features.IssueCredential;
using AriesWebApp.Models;

namespace AriesWebApp.Models
{
    public class CredentialsViewModel
    {
        public IEnumerable<CredentialRecord> Credentials { get; set; }
    }
}
