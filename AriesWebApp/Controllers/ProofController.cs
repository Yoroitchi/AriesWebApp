﻿using System;
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


    }
}