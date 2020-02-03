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
using Hyperledger.Indy.AnonCredsApi;
using AriesWebApp.Models;
using System.Threading;

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
            return View(new ProofsViewModel
            {
                Proofs = await _proofService.ListAsync(agentContext),

            });
        }

        [HttpGet]
        public async Task<IActionResult> Details(string proofRecordId)
        {
            var agentContext = await _agentProvider.GetContextAsync();
            var proofRecord = await _proofService.GetAsync(agentContext, proofRecordId);
            var model = new ProofsDetailViewModel
            {
                ProofPartial = JsonConvert.DeserializeObject<PartialProof>(proofRecord.ProofJson),
            };
            return View(model);
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
        public async Task<IActionResult> SendProof(string proofRecordId)
        {
            var agentContext = await _agentProvider.GetContextAsync();
            var proofRecord = await _proofService.GetAsync(agentContext, proofRecordId);
            var connectionRecord = await _connectionService.GetAsync(agentContext, proofRecord.ConnectionId);
            var request = JsonConvert.DeserializeObject<ProofRequest>(proofRecord.RequestJson);
            var requestedCredentials = new RequestedCredentials();
            foreach (var requestedAttribute in request.RequestedAttributes)
            {
                var credentials = await _proofService.ListCredentialsForProofRequestAsync(agentContext, request, requestedAttribute.Key);

                requestedCredentials.RequestedAttributes.Add(requestedAttribute.Key, 
                    new RequestedAttribute
                    {
                        CredentialId = credentials.First().CredentialInfo.Referent,
                        Revealed = true
                    });
            }

            foreach (var requestedAttribute in request.RequestedPredicates)
            {
                var credentials =
                    await _proofService.ListCredentialsForProofRequestAsync(agentContext, request,
                        requestedAttribute.Key);

                requestedCredentials.RequestedPredicates.Add(requestedAttribute.Key,
                    new RequestedAttribute
                    {
                        CredentialId = credentials.First().CredentialInfo.Referent,
                        Revealed = false
                    });
            }

            var (proofMsg, record) = await _proofService.CreatePresentationAsync(agentContext, proofRecordId, requestedCredentials);
            await _messageService.SendAsync(agentContext.Wallet, proofMsg, connectionRecord);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> SendProofNameRequest(string connectionId)
        {
            var agentContext = await _agentProvider.GetContextAsync();
            var connectionRecord = await _walletRecordService.GetAsync<ConnectionRecord>(agentContext.Wallet, connectionId);
            var proofNameRequest = await CreateProofNameMessage(connectionRecord);
            await _messageService.SendAsync(agentContext.Wallet, proofNameRequest, connectionRecord);

            return RedirectToAction("Index");
        }

        public async Task<RequestPresentationMessage> CreateProofNameMessage(ConnectionRecord connectionRecord)
        {
            var agentContext = await _agentProvider.GetContextAsync();
            var name = connectionRecord.Alias?.Name ?? connectionRecord.Id;
            var proofRequest = new ProofRequest
            {
                Name = "ProofReq",
                Version = "1.0",
                Nonce = await AnonCreds.GenerateNonceAsync(),
                RequestedAttributes = new Dictionary<string, ProofAttributeInfo>
                    {
                        {"firstname-required", new ProofAttributeInfo {Name = "firstname"}}
                    }
            };

            (var msg, _) = await _proofService.CreateRequestAsync(agentContext, proofRequest);

            return msg;
        }

        public async Task<RequestPresentationMessage> CreateOver21ProofMessage(ConnectionRecord connectionRecord)
        {
            var agentContext = await _agentProvider.GetContextAsync();
            var name = connectionRecord.Alias?.Name ?? connectionRecord.Id;
            var proofRequest = new ProofRequest
            {
                Name = "Over21Request",
                Version = "1.0",
                Nonce = await AnonCreds.GenerateNonceAsync(),
                RequestedAttributes = new Dictionary<string, ProofAttributeInfo>
                {
                    { "birthdate", new ProofAttributeInfo { Name = "birthdate" } }
                },
            };

            (var msg, _) = await _proofService.CreateRequestAsync(agentContext, proofRequest);

            return msg;
        }

        public async Task<IActionResult> SendOver21ProofRequest(string connectionId)
        {
            var agentContext = await _agentProvider.GetContextAsync();
            var connectionRecord = await _walletRecordService.GetAsync<ConnectionRecord>(agentContext.Wallet, connectionId);
            var proofNameRequest = await CreateOver21ProofMessage(connectionRecord);
            await _messageService.SendAsync(agentContext.Wallet, proofNameRequest, connectionRecord);

            return RedirectToAction("Index");
        }
    
        [HttpGet]
        public async Task<IActionResult> VerifyProof(string proofRecordId)
        {
            var agentContext = await _agentProvider.GetContextAsync();
            var proofRecord = await _proofService.GetAsync(agentContext, proofRecordId);
            var request = JsonConvert.DeserializeObject<PartialProof>(proofRecord.RequestJson);
            var proof = JsonConvert.DeserializeObject<PartialProof>(proofRecord.ProofJson);
            /*foreach (var item in request.RequestedProof.RevealedAttributes)
            {
                Console.WriteLine(item.Value);
            }*/
            foreach (var item in proof.RequestedProof.RevealedAttributes)
            {
                Console.WriteLine(item.Value);
            }
            
            return RedirectToAction("Index");
        }
    }
}