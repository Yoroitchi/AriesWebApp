using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using Hyperledger.Aries.Features.PresentProof;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Storage;
using Hyperledger.Aries.Features.DidExchange;
using Hyperledger.Aries.Utils;
using AriesWebApp.Models;

namespace AriesWebApp.Controllers
{
    public class ProofController : Controller
    {
        private readonly IAgentProvider _agentProvider;
        private readonly IProofService _proofService;
        private readonly IWalletRecordService _walletRecordService;
        private readonly IMessageService _messageService;
        private readonly IConnectionService _connectionService;

        public ProofController(
            IAgentProvider agentProvider,
            IProofService proofService,
            IWalletRecordService walletRecordService,
            IMessageService messageService,
            IConnectionService connectionService)
        {
            _agentProvider = agentProvider;
            _proofService = proofService;
            _walletRecordService = walletRecordService;
            _messageService = messageService;
            _connectionService = connectionService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var agentContext = await _agentProvider.GetContextAsync();

            //Some tests
            var proof = await _proofService.GetAsync(agentContext, "781d20af-664d-460f-b61d-d9f20d827ea8");
            Console.WriteLine("Proof request : " + proof.RequestJson);

            return View(new ProofsViewModel
            {
                Proofs = await _proofService.ListAsync(agentContext),
            });
        }


        public async Task<RequestPresentationMessage> CreateProofNameMessage(string connectionId)
        {
            var agentContext = await _agentProvider.GetContextAsync();
            
            var proofRequest = new ProofRequest
            {
                Name = "ProoveYourName" + connectionId,
                Version = "1.1",
                //Adding a new proof attribut => { "", new ProofAttributeInfo { } },
                RequestedAttributes = new Dictionary<string, ProofAttributeInfo>()
                {
                    { "name", new ProofAttributeInfo {Name = "NameAttribute" } },
                },
            };

            (var msg, _) = await _proofService.CreateRequestAsync(agentContext, proofRequest);

            return msg;
        }

        [HttpGet]
        public async Task<IActionResult> SendProofRequestNameView()
        {
            var agentContext = await _agentProvider.GetContextAsync();
            var model = new ConnectionsViewModel
            {
                Connections = await _connectionService.ListAsync(agentContext)
            };
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ProoveName(string proofRecordId)
        {
            var agentContext = await _agentProvider.GetContextAsync();
            var proofRecord = await _proofService.GetAsync(agentContext, proofRecordId);
            var connectionRecord = await _connectionService.GetAsync(agentContext, proofRecord.ConnectionId);
            var request = JsonConvert.DeserializeObject<ProofRequest>(proofRecord.RequestJson);
            var requestedCred = new RequestedCredentials();
            foreach(var requestedAttr in request.RequestedAttributes)
            {
                var cred = await _proofService.ListCredentialsForProofRequestAsync(agentContext, request, requestedAttr.Key);

                requestedCred.RequestedAttributes.Add(requestedAttr.Key, new RequestedAttribute
                {
                    CredentialId = cred.First().CredentialInfo.Referent,
                    Revealed = true
                });
            }

            var (proofMsg, record) = await _proofService.CreatePresentationAsync(agentContext, proofRecordId, requestedCred);
            await _messageService.SendAsync(agentContext.Wallet, proofMsg, connectionRecord);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> SendProofNameRequest(string connectionId)
        {
            var agentContext = await _agentProvider.GetContextAsync();
            var proofNameRequest = await CreateProofNameMessage(connectionId);
            var connectionRecord = await _walletRecordService.GetAsync<ConnectionRecord>(agentContext.Wallet, connectionId);
            await _messageService.SendAsync(agentContext.Wallet, proofNameRequest, connectionRecord);

            return RedirectToAction("Index");
        }
    }
}