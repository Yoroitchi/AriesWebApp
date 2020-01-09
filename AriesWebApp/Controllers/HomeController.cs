using AriesWebApp.Models;
using Hyperledger.Aries.Configuration;
using Hyperledger.Aries.Storage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Threading.Tasks;


namespace AriesWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWalletService _walletService;
        private readonly IProvisioningService _provisioningService;
        private readonly AgentOptions _agentOptions;

        public HomeController(
            IWalletService walletService,
            IProvisioningService provisioningService,
            IOptions<AgentOptions> agentOptions
            )
        {
            _walletService = walletService;
            _provisioningService = provisioningService;
            _agentOptions = agentOptions.Value;
        }

        public async Task<IActionResult> Index()
        {
            var wallet = await _walletService.GetWalletAsync(
                configuration: _agentOptions.WalletConfiguration,
                credentials: _agentOptions.WalletCredentials);

            var provisioning = await _provisioningService.GetProvisioningAsync(wallet);
            return View(provisioning);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
