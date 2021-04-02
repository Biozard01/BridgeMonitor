using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BridgeMonitor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BridgeMonitor.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            List<TTFermetures> info = GetFromAPI();
            info.Sort((s1, s2) => DateTimeOffset.Compare(s1.ClosingDate, s2.ClosingDate));

            foreach (var ferme in info)
            {
                if (DateTimeOffset.Compare(DateTimeOffset.Now, ferme.ClosingDate) < 0)
                {
                    ViewData["Info"] = ferme;
                    break;
                }
            }

            return View();
        }

        public IActionResult Detail(string fermes)
        {
            string[] infos = fermes.Split(".");
            List<TTFermetures> info = GetFromAPI();
            List<TTFermetures> ancienInfo = new List<TTFermetures>();
            for (int i = 0; i < info.Count; i++)
            {
                if (info[i].ClosingDate.CompareTo(DateTime.Now) < 0)
                {
                    ancienInfo.Add(info[i]);
                    info.RemoveAt(i);
                }
            }
            
            info.Sort((s1, s2) => DateTimeOffset.Compare(s1.ClosingDate, s2.ClosingDate));
            ancienInfo.Sort((s1, s2) => DateTimeOffset.Compare(s1.ClosingDate, s2.ClosingDate));

            ViewData["Info"] = infos[0] == "n" ? info[int.Parse(infos[1])] : ancienInfo[int.Parse(infos[1])];
            return View("Index");
        }

        public IActionResult TTLesFerme()
        {
            ViewData["Infos"] = GetFromAPI();
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public static List<TTFermetures> GetFromAPI()
        {
            using (var client = new HttpClient())
            {
                var response = client.GetAsync("https://api.alexandredubois.com/pont-chaban/api.php");
                var stringResult = response.Result.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<List<TTFermetures>>(stringResult.Result);
                return result;
            }
        }
    }
}