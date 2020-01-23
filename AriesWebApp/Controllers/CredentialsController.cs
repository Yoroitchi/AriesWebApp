using System;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Features.IssueCredential;
using Hyperledger.Aries.Features.DidExchange;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using AriesWebApp.Models;
using Newtonsoft.Json;


namespace AriesWebApp.Controllers
{

    public class CredentialsController : Controller
    {
        private readonly ICredentialService _credentialService;
        private readonly IAgentProvider _agentContextProvider;
        private readonly IConnectionService _connectionService;
        private readonly IMessageService _messageService;


        public CredentialsController(
            ICredentialService credentialService,
            IAgentProvider agentContextProvider,
            IConnectionService connectionService,
            IMessageService messageService
            )
        {
            _credentialService = credentialService;
            _agentContextProvider = agentContextProvider;
            _connectionService = connectionService;
            _messageService = messageService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {

            var agentContext = await _agentContextProvider.GetContextAsync();

            return View(new CredentialsViewModel
            {
                Credentials = await _credentialService.ListAsync(agentContext)
            });
        }

        [HttpGet]
        public async Task<IActionResult> ProcessRequest(string id)
        {
            var agentContext = await _agentContextProvider.GetContextAsync();
            var credentialRecord = await _credentialService.GetAsync(agentContext, id);
            var connectionRecord = await _connectionService.GetAsync(agentContext, credentialRecord.ConnectionId);
            (var cred, _) = await _credentialService.CreateCredentialAsync(agentContext: agentContext, credentialId: id);
            await _messageService.SendAsync(agentContext.Wallet, cred, connectionRecord);
            
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> ProcessOffer(string id)
        {
            var agentContext = await _agentContextProvider.GetContextAsync();
            var credentialRecord = await _credentialService.GetAsync(agentContext, id);
            var connectionId = credentialRecord.ConnectionId;
            var connectionRecord = await _connectionService.GetAsync(agentContext, connectionId);
            (var request, _) = await _credentialService.CreateRequestAsync(agentContext, id);
            await _messageService.SendAsync(agentContext.Wallet, request, connectionRecord);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            var agentContext = await _agentContextProvider.GetContextAsync();
            var credentialRecord = await _credentialService.GetAsync(agentContext, id);
            var model = new CredentialViewModel
            {
                Name = credentialRecord.CredentialId,
                CreatedAt = credentialRecord.CreatedAtUtc.Value.ToLocalTime(),
                State = credentialRecord.State,
                CredentialAttributesValues = credentialRecord.CredentialAttributesValues,
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SendOfferCredential(string connectionId, string credDefId)
        {
            var agentContext = await _agentContextProvider.GetContextAsync();
            var connectionRecord = await _connectionService.GetAsync(agentContext, connectionId);

            //{ "type", "passportNumber", "issuerCountryCode", "firstname", "familyname", "birthdate", "citizenship", "sex", "placeOfBirth", "issuingDate", "expiryDate" }
             var offerConfig = new OfferConfiguration
             {
                 CredentialDefinitionId = credDefId,
                 IssuerDid = "Th7MpTaRZVRYnPiabds81Y",
                 CredentialAttributeValues = new [] 
                 { 
                     new CredentialPreviewAttribute("type","passport"), 
                     new CredentialPreviewAttribute("passportNumber", $"{Guid.NewGuid().ToString("N")}"),
                     new CredentialPreviewAttribute("issuerCountryCode", "CH"),
                     new CredentialPreviewAttribute("firstname", "John"),
                     new CredentialPreviewAttribute("familyname","Doe"),
                     new CredentialPreviewAttribute("birthdate","1968-02-12T:15:00:00Z"),
                     new CredentialPreviewAttribute("citizenship","CH"),
                     new CredentialPreviewAttribute("sex","M"),
                     new CredentialPreviewAttribute("placeOfBirth", "Paris"),
                     new CredentialPreviewAttribute("issuingDate", "20-01-2020"),
                     new CredentialPreviewAttribute("expiryDate", "20-01-2030")
                 }
             };
            /*var offerConfig = new OfferConfiguration
            {
                CredentialDefinitionId = credDefId,
                IssuerDid = "Th7MpTaRZVRYnPiabds81Y",
                CredentialAttributeValues = new[] { new CredentialPreviewAttribute("first_name", "Test"), new CredentialPreviewAttribute("last_name", "Holder") }
            };*/

            (var credOfferMsg, _) = await _credentialService.CreateOfferAsync(agentContext, offerConfig, connectionId);
            await _messageService.SendAsync(agentContext.Wallet, credOfferMsg, connectionRecord);

            return RedirectToAction("Details", "Connections", new { id = connectionId });
        }



       /* public async Task<string> GetCredentialJson(CredentialRecord credentialRecord)
        {

            string credString;
            return credString;
        }*/

    }
}
/* {
  "schema_id": "Th7MpTaRZVRYnPiabds81Y:2:fictional-passeport-28c474fdca064110b3cf7f65f9ba5666:1.1",
  "cred_def_id": "Th7MpTaRZVRYnPiabds81Y:3:CL:89:Tag",
  "rev_reg_id": null,
  "values": {
    "issuingDate": {
      "raw": "20-01-2020",
      "encoded": "1234567890"
    },
    "expiryDate": {
      "raw": "20-01-2030",
      "encoded": "1234567890"
    },
    "issuerCountryCode": {
      "raw": "CH",
      "encoded": "1234567890"
    },
    "type": {
      "raw": "passport",
      "encoded": "1234567890"
    },
    "familyname": {
      "raw": "Doe",
      "encoded": "1234567890"
    },
    "sex": {
      "raw": "M",
      "encoded": "1234567890"
    },
    "citizenship": {
      "raw": "CH",
      "encoded": "1234567890"
    },
    "passportNumber": {
      "raw": "de3025714ba245018c586ac8e68154fa",
      "encoded": "1234567890"
    },
    "firstname": {
      "raw": "John",
      "encoded": "1234567890"
    },
    "birthdate": {
      "raw": "1968-02-12T:15:00:00Z",
      "encoded": "1234567890"
    },
    "placeOfBirth": {
      "raw": "Paris",
      "encoded": "1234567890"
    }
  },
  "signature": {
    "p_credential": {
      "m_2": "101057710311002755186807053840429315778579595364484361574572713591835865826914",
      "a": "87856737862368638042704587786273677121805349842980089413308023257687316775017367594238598283051243843258834015084831213857413641470568889269519377810306021926070735958959158746150386840531739873404725417482057692276762375685775153886388160253223993715433279670739068540520085113250797248261036629033171258684731308346109463140729971800887722548867720988622769163630331286964730252017102175108441884751257413040195552907721372594840573213808003213751352866623396423499377699015952683935426585738993842188830876231772148780770320011167586138795070062977449539580372442110479380463219124633096547965122386256992167259538",
      "e": "259344723055062059907025491480697571938277889515152306249728583105665800713306759149981690559193987143012367913206299323899696942213235956742930033285925305171226733846820611389493",
      "v": "6232962504117037788309667689598288101363354932803468765724493356213150436756933147172127433790386753989606682697609186459716140391738999258236877710329168930374239868658858071316390664353247090723897715034617518550617444957410007407327901378444715418137332284304240276853205965218782201651440903849218349437929588571296108976774080601830039557697250748846334342308476634721804899253111544705895686450722863698157046493779672548322301716346965776071427396857791978283824516195872442109625866860305917736001441360137240755356534756011994571350408259301579065316713087618668483575560672834636564226325647539935700729196727883529469174883921693961459289272929148076124608267326458167128674986763185511377595933951719133114675450250122819645841142283006805066034037826541852931322487764692528776002281124274578718592885015349"
    },
    "r_credential": null
  },
  "signature_correctness_proof": {
    "se": "14142913063139694958491476417293784042450660182172724101026262681651080469249455328861959236716457435914424419447223209541758385856014244726859138575112536416495644781335574409865953963352828134782410579517901579529000459499704147227568628228875289957745082999531955392007175090767483965435412672424173943735624500643165616504610976730915058697367192280241181395692138146016857747131325737223972603160924609601459810184523017836460237730826411680413021430446991946419846552542441351375805018107632776702064804202756069613423754539272817951660724548962680191259436856533094547662656424827421292211224276890258933600291",
    "c": "115763571893743710275845890160072707854272427019503843513277754796037086059604"
  },
  "rev_reg": null,
  "witness": null,
  "alg": "HS256"
}*/
