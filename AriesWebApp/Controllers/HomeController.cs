using System.Diagnostics;
using System.Threading.Tasks;
using Hyperledger.Aries.Configuration;
using Hyperledger.Aries.Storage;
using Hyperledger.Aries.Agents;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using AriesWebApp.Models;


namespace AriesWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAgentProvider _agentContextProvider;
        private readonly IProvisioningService _provisioningService;
        private readonly AgentOptions _walletOptions;

        public HomeController(
            IAgentProvider agentContextProvider,
            IProvisioningService provisioningService,
            IOptions<AgentOptions> walletOptions)
        {
            _agentContextProvider = agentContextProvider;
            _provisioningService = provisioningService;
            _walletOptions = walletOptions.Value;
        }

        public async Task<IActionResult> Index()
        {
            var agentContext = await _agentContextProvider.GetContextAsync();

            if (_walletOptions.IssuerDid == null)
            {
                agentContext = await _agentContextProvider.GetContextAsync();
                var provrecord = await _provisioningService.GetProvisioningAsync(agentContext.Wallet);
                _walletOptions.IssuerDid = provrecord.IssuerDid;
            }
            var provisioning = await _provisioningService.GetProvisioningAsync(agentContext.Wallet);
            return View(provisioning);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}