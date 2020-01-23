using Hyperledger.Aries.Features.IssueCredential;
using System.Collections.Generic;
using System;

namespace AriesWebApp.Models
{
    public class CredentialViewModel
    {
        public string Name { get; set; }

        public DateTime CreatedAt { get; set; }

        public CredentialState State { get; set; }

        public IEnumerable<CredentialPreviewAttribute> CredentialAttributesValues { get; set; }
        
        public string CredentialRecordId { get; set; }
    }
}
