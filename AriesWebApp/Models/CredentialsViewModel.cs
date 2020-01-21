using System.Collections.Generic;
using Hyperledger.Aries.Features.IssueCredential;
using Hyperledger.Aries.Features.BasicMessage;
using Hyperledger.Aries.Features.DidExchange;
using AriesWebApp.Models;


namespace AriesWebApp.Models
{
    public class CredentialsViewModel
    {
        public IEnumerable<CredentialRecord> Credentials { get; set; }

    }
}
