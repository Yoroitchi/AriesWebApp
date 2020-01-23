using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Hyperledger.Aries.Storage;
using Hyperledger.Aries.Configuration;
using Hyperledger.Aries.Features.IssueCredential;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Models.Records;
using Hyperledger.Indy.LedgerApi;
using AriesWebApp.Models;

namespace AriesWebApp.Controllers
{
    public class SchemaController : Controller
    {
        private readonly IWalletService _walletService;
        private readonly ISchemaService _schemaService;
        private readonly IAgentProvider _agentContextProvider;
        private readonly IProvisioningService _provisioningService;
        private readonly IWalletRecordService _walletRecordService;
        private readonly AgentOptions _agentOptions;
        public SchemaController(
            IWalletService walletService,
            ISchemaService schemaService,
            IAgentProvider agentContextProvider,
            IProvisioningService provisioningService,
            IWalletRecordService walletRecordService,
            IOptions<AgentOptions> agentOptions
            )
        {
            _walletService = walletService;
            _schemaService = schemaService;
            _agentContextProvider = agentContextProvider;
            _provisioningService = provisioningService;
            _walletRecordService = walletRecordService;
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
            var agentContext = await _agentContextProvider.GetContextAsync();
            var record = await _provisioningService.GetProvisioningAsync(await _walletService.GetWalletAsync(_agentOptions.WalletConfiguration, _agentOptions.WalletCredentials));

            //The fields of the future schema 
            var schemaName = "fictional-passeport-" + $"{ Guid.NewGuid().ToString("N")}";

            var schemaVersion = "1.1";
            var schemaAttrNames = new[] { "type", "passportNumber", "issuerCountryCode", "firstname", "familyname", "birthdate", "citizenship", "sex", "placeOfBirth", "issuingDate", "expiryDate" };
            //var schemaAttrNames = new [] { "first_name", "last_name" };
            //promoting the did to TRUSTEE role
            await Ledger.SignAndSubmitRequestAsync(await agentContext.Pool, agentContext.Wallet, _agentOptions.IssuerDid,
             await Ledger.BuildNymRequestAsync(_agentOptions.IssuerDid, _agentOptions.IssuerDid, "~7TYfekw4GUagBnBVCqPjiC", null, "TRUSTEE"));

            //Create and register a dummy schema using previous fields
            var schemaId = await _schemaService.CreateSchemaAsync(agentContext, _agentOptions.IssuerDid, schemaName, schemaVersion, schemaAttrNames);

            await Task.Delay(TimeSpan.FromSeconds(5));

            //TODO: Need a CreateSchemaView => Not necessary for the PoC
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            var walletContext = await _walletService.GetWalletAsync(_agentOptions.WalletConfiguration, _agentOptions.WalletCredentials);

            var model = new SchemaDetailViewModel
            {
                Schema = await _walletRecordService.GetAsync<SchemaRecord>(walletContext, id),
                AssociateCredDefinition = await _schemaService.ListCredentialDefinitionsAsync(walletContext)
            };
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> SendCredDefToLedger(string id)
        {
            var walletContext = await _walletService.GetWalletAsync(_agentOptions.WalletConfiguration, _agentOptions.WalletCredentials);
            var agentContext = await _agentContextProvider.GetContextAsync();
            await _schemaService.CreateCredentialDefinitionAsync(agentContext, id, "Tag", false, 100);
            await Task.Delay(TimeSpan.FromSeconds(2));
            return RedirectToAction("Details", "Schema", new { id });
        }
    }
}
