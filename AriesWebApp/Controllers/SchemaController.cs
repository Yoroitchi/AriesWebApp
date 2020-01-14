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

namespace AriesWebApp.Controllers
{
    public class SchemaController : Controller
    {

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
    }
}
