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
using Hyperledger.Indy.DidApi;
using AriesWebApp.Models;

namespace AriesWebApp.Controllers
{
    public class SchemaController : Controller
    {
        private readonly IWalletService _walletService;
        private readonly ISchemaService _schemaService;
        private readonly IAgentProvider _agentContextProvider;
        private readonly IWalletRecordService _walletRecordService;
        private readonly AgentOptions _agentOptions;
        public SchemaController(
            IWalletService walletService,
            ISchemaService schemaService,
            IAgentProvider agentContextProvider,
            IWalletRecordService walletRecordService,
            IOptions<AgentOptions> agentOptions
            )
        {
            _walletService = walletService;
            _schemaService = schemaService;
            _agentContextProvider = agentContextProvider;
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

            //The fields of the future schema 
            var schemaName = "fictional-passport-" + $"{ Guid.NewGuid().ToString("N")}";

            var schemaVersion = "1.1";
            var schemaAttrNames = new[] { "holderdid", "type", "passportNumber", "issuerCountryCode", "firstname", "familyname", "birthdate", "citizenship", "sex", "placeOfBirth", "issuingDate", "expiryDate" };


            //promoting the did to TRUSTEE role
            
            var verkey = await Did.KeyForLocalDidAsync(agentContext.Wallet, _agentOptions.IssuerDid);

            await Ledger.SignAndSubmitRequestAsync(await agentContext.Pool, agentContext.Wallet, _agentOptions.IssuerDid,
             await Ledger.BuildNymRequestAsync(_agentOptions.IssuerDid, _agentOptions.IssuerDid, verkey, null, "TRUSTEE"));

            //Create and register a dummy schema using previous fields
            await _schemaService.CreateSchemaAsync(agentContext, _agentOptions.IssuerDid, schemaName, schemaVersion, schemaAttrNames);


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
            var agentContext = await _agentContextProvider.GetContextAsync();
            await _schemaService.CreateCredentialDefinitionAsync(agentContext, id, "Tag", false, 0);
            return RedirectToAction("Details", "Schema", new { id });
        }

        [HttpGet]
        public async Task<IActionResult> CreateOfferFromSchema(string connectionId)
        {
            var agentContext = await _agentContextProvider.GetContextAsync();
            return View(new CreateOfferViewModel
            {
                Schemas = await _schemaService.ListSchemasAsync(agentContext.Wallet),
                ConnectionId = connectionId
            });
        }

    }
}
