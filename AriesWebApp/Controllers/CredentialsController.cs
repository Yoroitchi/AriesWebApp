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
using AriesWebApp.Models;
using Hyperledger.Aries.Contracts;
using Hyperledger.Aries.Features.IssueCredential;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Configuration;
using Hyperledger.Aries.Storage;
using Hyperledger.Indy.PoolApi;
using Hyperledger.Indy.DidApi;
using Hyperledger.Indy.LedgerApi;


namespace AriesWebApp.Controllers
{

    public class CredentialsController : Controller
    {
        //Here the local Interfaces of the controller
        private readonly IWalletService _walletService;
        private readonly ICredentialService _credentialService;
        private readonly IWalletRecordService _recordService;
        private readonly IAgentProvider _agentContextProvider;
        private readonly IPoolService _poolService;
        private readonly AgentOptions _agentOptions;
        private readonly IProvisioningService _provisioningService;
        private readonly ISchemaService _schemaService;
        private readonly ILedgerService _ledgerService;

        public CredentialsController(
            IWalletService walletService,
            ICredentialService credentialService,
            IWalletRecordService recordService,
            IAgentProvider agentContextProvider,
            IOptions<AgentOptions> agentOptions,
            ISchemaService schemaService,
            IPoolService poolService,
            IProvisioningService provisioningService,
            ILedgerService ledgerService
            )
        {
            _walletService = walletService;
            _credentialService = credentialService;
            _recordService = recordService;
            _agentContextProvider = agentContextProvider;
            _agentOptions = agentOptions.Value;
            _schemaService = schemaService;
            _poolService = poolService;
            _provisioningService = provisioningService;
            _ledgerService = ledgerService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            
            var context = await _agentContextProvider.GetContextAsync();

            return View(new CredentialsViewModel
            {
                Credentials = await _credentialService.ListAsync(context)
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetCredDef()
        {
            var walletContext = await _walletService.GetWalletAsync(_agentOptions.WalletConfiguration, _agentOptions.WalletCredentials);
            var agentContext = await _agentContextProvider.GetContextAsync();
            var poolContext = await _poolService.GetPoolAsync("AriesTest", 2);
            var ledgerContext = await _ledgerService.LookupNymAsync(poolContext, _agentOptions.IssuerDid);
            var schemaContext = await _schemaService.LookupSchemaAsync(poolContext, 11);
            var schemaContext1 = await _schemaService.LookupSchemaAsync(poolContext, 12);
            var schemaContext2 = await _schemaService.LookupSchemaAsync(poolContext, 13);
            // var credContext = await _schemaService.CreateCredentialDefinitionAsync(agentContext,"Th7MpTaRZVRYnPiabds81Y:2:MOI:2.23", _agentOptions.IssuerDid, "Tag",false,100,new Uri("http://mock/tails"));
            //var credDef = await _schemaService.LookupCredentialDefinitionAsync(poolContext, "Th7MpTaRZVRYnPiabds81Y:2:CL:11:1");
            //var walletCredDef = await _schemaService.GetCredentialDefinitionAsync(walletContext, "Th7MpTaRZVRYnPiabds81Y:3:CL:11:Tag");
            var schemaRecord = await _schemaService.ListSchemasAsync(walletContext);
            //Console.WriteLine("AgentDID "+_agentOptions.AgentDid);
            //Console.WriteLine("AgentKey " + _agentOptions.AgentKey);
            //Console.WriteLine("AgentKeySeed " + _agentOptions.AgentKeySeed);
            //Console.WriteLine("IssuerDID "+_agentOptions.IssuerDid);
            //Console.WriteLine("IssuerKeySEED " + _agentOptions.IssuerKeySeed);
            //Console.WriteLine("Pool context "+poolContext.ToString());
            Console.WriteLine("Nym for Th7MpTaRZVRYnPiabds81Y " + ledgerContext);
            Console.WriteLine("schema def " + schemaContext);
            Console.WriteLine("schema def " + schemaContext1);
            Console.WriteLine("schema def " + schemaContext2);
            //Console.WriteLine(credContext);
            //Console.WriteLine("cred def "+credDef); //Th7MpTaRZVRYnPiabds81Y:3:CL:11:Tag => initialement Th7MpTaRZVRYnPiabds81Y:2:CL:11:1
            //Console.WriteLine("wallet cred def " + walletCredDef);

            foreach (var element in schemaRecord)
            {
                Console.WriteLine("###############################################");
                Console.WriteLine(element.Id);
                schemaContext = await _schemaService.LookupSchemaAsync(poolContext, element.Id);
                Console.WriteLine(schemaContext);
            }
            
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> SendCredDef()
        {
            var agentContext = await _agentContextProvider.GetContextAsync();
            var record = await _provisioningService.GetProvisioningAsync(await _walletService.GetWalletAsync(_agentOptions.WalletConfiguration, _agentOptions.WalletCredentials));
            Console.WriteLine(record.IssuerDid);
            //The fields of the future schema 
            var schemaName = $"Test-Schema-{Guid.NewGuid().ToString("N")}";
            var schemaVersion = "1.0";
            var schemaAttrNames = new[] { "test_attr_1", "test_attr_2" };
            Console.WriteLine(schemaName);

            //promoting the did to TRUSTEE role
            //await Ledger.SignAndSubmitRequestAsync(await agentContext.Pool, agentContext.Wallet, _agentOptions.IssuerDid,
            // await Ledger.BuildNymRequestAsync(_agentOptions.IssuerDid, _agentOptions.IssuerDid, "~7TYfekw4GUagBnBVCqPjiC", null, "TRUSTEE"));

            //Create and register a dummy schema using previous fields
            var schemaId = await _schemaService.CreateSchemaAsync(agentContext, schemaName, schemaVersion, schemaAttrNames);

            await Task.Delay(TimeSpan.FromSeconds(5));
            return RedirectToAction("Index");
        }
    }
}