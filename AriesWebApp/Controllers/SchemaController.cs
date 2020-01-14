using System;
using System.Globalization;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Hyperledger.Aries.Storage;
using Hyperledger.Aries.Configuration;
using Hyperledger.Aries.Features.IssueCredential;
using Hyperledger.Aries.Agents;
using Hyperledger.Indy.LedgerApi;
using AriesWebApp.Models;

namespace AriesWebApp.Controllers
{
    public class SchemaController : Controller
    {
        private readonly IWalletService _walletService;
        private readonly ISchemaService _schemaService;
        private readonly IAgentProvider _agentProvider;
        private readonly IProvisioningService _provisioningService;
        private readonly AgentOptions _agentOptions;
        public SchemaController(
            IWalletService walletService,
            ISchemaService schemaService,
            IAgentProvider agentProvider,
            IProvisioningService provisioningService,
            IOptions<AgentOptions> agentOptions
            )
        {
            _walletService = walletService;
            _schemaService = schemaService;
            _agentProvider = agentProvider;
            _provisioningService = provisioningService;
            _agentOptions = agentOptions.Value;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var context = await _walletService.GetWalletAsync(_agentOptions.WalletConfiguration, _agentOptions.WalletCredentials);
            return View(new SchemaViewModel
            {
                Schemas = await _schemaService.ListSchemasAsync(context)
            });
        }

        [HttpGet]
        public async Task<IActionResult> CreateSchema()
        {
            var agentContext = await _agentProvider.GetContextAsync();
            var record = await _provisioningService.GetProvisioningAsync(await _walletService.GetWalletAsync(_agentOptions.WalletConfiguration, _agentOptions.WalletCredentials));

            //The fields of the future schema 
            var schemaName = $"Test-Schema-{Guid.NewGuid().ToString("N")}";
            var schemaVersion = "1.0";
            var schemaAttrNames = new[] { "test_attr_1", "test_attr_2" };

            //promoting the did to TRUSTEE role
            await Ledger.SignAndSubmitRequestAsync(await agentContext.Pool, agentContext.Wallet, _agentOptions.IssuerDid,
             await Ledger.BuildNymRequestAsync(_agentOptions.IssuerDid, _agentOptions.IssuerDid, "~7TYfekw4GUagBnBVCqPjiC", null, "TRUSTEE"));

            //Create and register a dummy schema using previous fields
            var schemaId = await _schemaService.CreateSchemaAsync(agentContext, _agentOptions.IssuerDid, schemaName, schemaVersion, schemaAttrNames);

            await Task.Delay(TimeSpan.FromSeconds(5));
            return RedirectToAction("Index");
        }
    }
}
