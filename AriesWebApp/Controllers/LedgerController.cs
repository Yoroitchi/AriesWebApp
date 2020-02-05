using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Configuration;
using Hyperledger.Indy.LedgerApi;
using Hyperledger.Indy.DidApi;

namespace AriesWebApp.Controllers
{
    public class LedgerController : Controller
    {
        private readonly IAgentProvider _agentContextProvider;
        private readonly AgentOptions _agentOptions;
        public LedgerController(
            IAgentProvider agentContextProvider,
            IOptions<AgentOptions> agentOptions
            )
        {
            _agentContextProvider = agentContextProvider;
            _agentOptions = agentOptions.Value;
        }

        [HttpPost]
        public async Task<IActionResult> PostDIDOnLedger(string holderDID)
        {
            var agentContext = await _agentContextProvider.GetContextAsync();

            //Get the abreviate verkey of the TRUSTEE DID
            var abrVerkey = await Did.AbbreviateVerkeyAsync(_agentOptions.IssuerDid, await Did.KeyForLocalDidAsync(agentContext.Wallet, _agentOptions.IssuerDid));

            //NYM transaction to post the holder issuer key 
            /*
            //Summary:
                IssuerDid is the public DID of the individual (holder, company, robot, alien, etc).
                It is the DID mentioned in every following Verifiable Credentials or Presentations (aka proofs)
                It belong onto the ledger, it is the most clever thing to do with it, even if the individual can create a new DID to fulfill this purpose.
            */
            await Ledger.SignAndSubmitRequestAsync(await agentContext.Pool, agentContext.Wallet, _agentOptions.IssuerDid,
             await Ledger.BuildNymRequestAsync(_agentOptions.IssuerDid, holderDID, abrVerkey, null, null));

            return RedirectToAction("Index", "Home");
        }


        //Not usefull for now, maybe later
        public async void PromoteToTrustee(string didToPromote)
        {
            var agentContext = await _agentContextProvider.GetContextAsync();
            var verkey = await Did.KeyForLocalDidAsync(agentContext.Wallet, didToPromote);

            await Ledger.SignAndSubmitRequestAsync(await agentContext.Pool, agentContext.Wallet, _agentOptions.IssuerDid,
             await Ledger.BuildNymRequestAsync(_agentOptions.IssuerDid, _agentOptions.IssuerDid, verkey, null, "TRUSTEE"));
        }
    }
}