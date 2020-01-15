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
using Hyperledger.Aries.Models.Records;
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
        private readonly IWalletRecordService _walletRecordService;
        private readonly IAgentProvider _agentContextProvider;
        private readonly IPoolService _poolService;
        private readonly AgentOptions _agentOptions;
        private readonly IProvisioningService _provisioningService;
        private readonly ISchemaService _schemaService;
        private readonly ILedgerService _ledgerService;

        public CredentialsController(
            IWalletService walletService,
            ICredentialService credentialService,
            IWalletRecordService walletRecordService,
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
            _walletRecordService = walletRecordService;
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


            return RedirectToAction("Index", "Schema");
            /*return View(new CredentialsViewModel
            {
                Credentials = 
            });*/
        }

        [HttpGet]
        public async Task<IActionResult> GetCredDef()
        {
            
            return RedirectToAction("Index");
        }

        
    }
}