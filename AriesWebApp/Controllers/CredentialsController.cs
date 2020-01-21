using System;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Configuration;
using Hyperledger.Aries.Contracts;
using Hyperledger.Aries.Features.IssueCredential;
using Hyperledger.Aries.Features.DidExchange;
using Hyperledger.Aries.Storage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using System.Collections.Generic;
using AriesWebApp.Models;
using Hyperledger.Indy.PoolApi;
using Newtonsoft.Json;

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
        private readonly IConnectionService _connectionService;
        private readonly IMessageService _messageService;

        public CredentialsController(
            IWalletService walletService,
            ICredentialService credentialService,
            IWalletRecordService walletRecordService,
            IAgentProvider agentContextProvider,
            IOptions<AgentOptions> agentOptions,
            ISchemaService schemaService,
            IPoolService poolService,
            IProvisioningService provisioningService,
            ILedgerService ledgerService,
            IConnectionService connectionService,
            IMessageService messageService
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
            _connectionService = connectionService;
            _messageService = messageService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var agentContext = await _agentContextProvider.GetContextAsync();
  
            return View(new CredentialsViewModel
            {
                Credentials = await _credentialService.ListAsync(agentContext),
            });
        }

        [HttpGet]
        public async Task<IActionResult> ProcessRequest(string id)
        {
            var agentContext = await _agentContextProvider.GetContextAsync();
            var credentialRecord = await _credentialService.GetAsync(agentContext, id);
            var connectionId = credentialRecord.ConnectionId;
            var connectionRecord = await _connectionService.GetAsync(agentContext, connectionId);
            (var cred, _) = await _credentialService.CreateCredentialAsync(agentContext, id);
            await _messageService.SendAsync(agentContext.Wallet, cred, connectionRecord);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> ProcessOffer(string id)
        {
            var agentContext = await _agentContextProvider.GetContextAsync();
            var credentialRecord = await _credentialService.GetAsync(agentContext, id);
            var connectionId = credentialRecord.ConnectionId;
            var connectionRecord = await _connectionService.GetAsync(agentContext, connectionId);
            (var request, _) = await _credentialService.CreateRequestAsync(agentContext, id);
            await _messageService.SendAsync(agentContext.Wallet, request, connectionRecord);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            var agentContext = await _agentContextProvider.GetContextAsync();
            var credentialRecord = await _credentialService.GetAsync(agentContext, id);
            var model = new CredentialViewModel
            {
                Name = credentialRecord.CredentialId,
                CreatedAt = credentialRecord.CreatedAtUtc.Value.ToLocalTime(),
                State = credentialRecord.State,
                CredentialAttributesValues = credentialRecord.CredentialAttributesValues
            };
            return View(model);
        }
        //Issuer issue at first, a credential offer
        [HttpPost]
        public async Task<IActionResult> SendOfferCredential(string connectionId, string credDefId)
        {
            var agentContext = await _agentContextProvider.GetContextAsync();
            var connectionRecord = await _connectionService.GetAsync(agentContext, connectionId);

            //var schemaAttrNames = new[] { "Name", "test_attr_2", "test_attr_3", "test_attr_4" };
            var offerConfig = new OfferConfiguration
            {
                CredentialDefinitionId = credDefId,
                IssuerDid = "Th7MpTaRZVRYnPiabds81Y"

                /*CredentialAttributeValues = new[]
                {
                    new CredentialPreviewAttribute("Name","r1"),
                    new CredentialPreviewAttribute("test_attr_2", "test_attr_2"),
                    new CredentialPreviewAttribute("test_attr_3", "test_attr_3"),
                    new CredentialPreviewAttribute("test_attr_4", "test_attr_4")
                }*/
            };

            
            (var credOfferMsg, var credRecord) = await _credentialService.CreateOfferAsync(agentContext, offerConfig, connectionId);
            await _messageService.SendAsync(agentContext.Wallet, credOfferMsg, connectionRecord);

            return RedirectToAction("Details", "Connections", new { id = connectionId });
        }

    }
}