using System;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Configuration;
using Hyperledger.Aries.Contracts;
using Hyperledger.Aries.Features.IssueCredential;
using Hyperledger.Aries.Storage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using System.Collections.Generic;
using AriesWebApp.Models;


namespace AriesWebApp.Controllers
{

    public class CredentialsController : Controller
    {
        //Here the local Interfaces of the controller
        private readonly IWalletService _walletService;
        private readonly ICredentialService _credentialService;
        private readonly IWalletRecordService _walletRecordService;
        private readonly IAgentProvider _agentContextProvider;
        private readonly IPoolService _poolService;
        private readonly AgentOptions _agentOptions;
        private readonly IProvisioningService _provisioningService;
        private readonly ISchemaService _schemaService;
        private readonly ILedgerService _ledgerService;

        public CredentialsController(
            IWalletService walletService,
            ICredentialService credentialService,
            IWalletRecordService walletRecordService,
            IAgentProvider agentContextProvider,
            IOptions<AgentOptions> agentOptions,
            ISchemaService schemaService,
            IPoolService poolService,
            IProvisioningService provisioningService,
            ILedgerService ledgerService
            )
        {
            _walletService = walletService;
            _credentialService = credentialService;
            _walletRecordService = walletRecordService;
            _agentContextProvider = agentContextProvider;
            _agentOptions = agentOptions.Value;
            _schemaService = schemaService;
            _poolService = poolService;
            _provisioningService = provisioningService;
            _ledgerService = ledgerService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {

            var agentContext = await _agentContextProvider.GetContextAsync();
            //return RedirectToAction("Index", "Schema");
            return View(new CredentialsViewModel
            {
                Credentials = await _credentialService.ListAsync(agentContext)
            });
        }

        [HttpPost]
        public async Task<IActionResult> SendOfferCredential(string connectionId, string credDefId)
        {
            var agentContext = await _agentContextProvider.GetContextAsync();
            var walletContext = await _walletService.GetWalletAsync(_agentOptions.WalletConfiguration, _agentOptions.WalletCredentials);

            //var schemaAttrNames = new[] { "Name", "test_attr_2", "test_attr_3", "test_attr_4" };
            var offerConfig = new OfferConfiguration
            {
                CredentialDefinitionId = credDefId,
                IssuerDid = connectionId,
                CredentialAttributeValues = new [] 
                { 
                    new CredentialPreviewAttribute("Name","r1"), 
                    new CredentialPreviewAttribute("test_attr_2", "test_attr_2"),
                    new CredentialPreviewAttribute("test_attr_3", "test_attr_3"),
                    new CredentialPreviewAttribute("test_attr_4", "test_attr_4")
                }
            };

            (var credOfferMsg, var credRecord) = await _credentialService.CreateOfferAsync(agentContext, offerConfig, connectionId);
            Console.WriteLine("Credential preview : " + credOfferMsg.CredentialPreview);
            Console.WriteLine("Credential ID: " + credRecord.CredentialId);

            await _walletRecordService.AddAsync<CredentialRecord>(walletContext, credRecord);
            return RedirectToAction("Details", "Connections", new { id = connectionId});
        }
        
    }
}