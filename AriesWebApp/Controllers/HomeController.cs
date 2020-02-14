using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Hyperledger.Aries.Configuration;
using Hyperledger.Aries.Storage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using AriesWebApp.Models;

namespace AriesWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWalletService _walletService;
        private readonly IProvisioningService _provisioningService;
        private readonly AgentOptions _walletOptions;

        public HomeController(
            IWalletService walletService,
            IProvisioningService provisioningService,
            IOptions<AgentOptions> walletOptions)
        {
            _walletService = walletService;
            _provisioningService = provisioningService;
            _walletOptions = walletOptions.Value;
        }

        public async Task<IActionResult> Index()
        {
            var wallet = await _walletService.GetWalletAsync(
                _walletOptions.WalletConfiguration,
                _walletOptions.WalletCredentials);

            var provisioning = await _provisioningService.GetProvisioningAsync(wallet);
            /*var date2 = DateTime.Today.AddYears(-21).ToString("yyyy-MM-dd");
            var date1 = DateTime.Today.ToString("yyyy-MM-dd");

            var dat1 = DateTime.ParseExact(date1, "yyyy-MM-dd", null);
            var dat2 = DateTime.ParseExact(date2, "yyyy-MM-dd", null);
            if (dat1 >= dat2) Console.WriteLine("true");
            else { Console.WriteLine("false"); }*/

            return View(provisioning);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}