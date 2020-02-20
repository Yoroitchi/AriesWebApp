using System;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Mvc;
using Hyperledger.Aries.Features.PresentProof;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Storage;
using Hyperledger.Aries.Features.DidExchange;
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

            PartialProof partialProof = new PartialProof();
            

            if (proofRecord.ProofJson != null)
            {
                partialProof = JsonConvert.DeserializeObject<PartialProof>(proofRecord.ProofJson);
            }
            var model = new ProofsDetailViewModel
            {
                ProofRecord = proofRecord,
                ProofRequest = JsonConvert.DeserializeObject<ProofRequest>(proofRecord.RequestJson),
                PartialProof = partialProof,
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

            foreach (var requestedPredicates in request.RequestedPredicates)
            {
                var credentials =
                    await _proofService.ListCredentialsForProofRequestAsync(agentContext, request,
                        requestedPredicates.Key);

                requestedCredentials.RequestedPredicates.Add(requestedPredicates.Key,
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


    }
}