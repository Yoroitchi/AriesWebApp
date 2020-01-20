using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Hyperledger.Aries.Features.PresentProof;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Models;
using Hyperledger.Aries.Utils;
using Hyperledger.Aries.Storage;
using AriesWebApp.Models;

namespace AriesWebApp.Controllers
{
    public class ProofController : Controller
    {
        private readonly IAgentProvider _agentProvider;
        private readonly IProofService _proofService;

        public ProofController(
            IAgentProvider agentProvider,
            IProofService proofService)
        {
            _agentProvider = agentProvider;
            _proofService = proofService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var agentContext = await _agentProvider.GetContextAsync();

            return View(new ProofsViewModel
            {
                Proofs = await _proofService.ListAsync(agentContext)
            });
        }

        [HttpGet]
        public async Task<IActionResult> SendProofRequest(string connectionId)
        {


            return RedirectToAction("Index");
        }
    }
}