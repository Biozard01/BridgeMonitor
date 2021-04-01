﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using BridgeMonitor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BridgeMonitor.Controllers
{
    public class HomeController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult TTLesFerme()
        {
            var info = GetFromAPI();
            return View(info);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private static List<TTFermetures> GetFromAPI()
        {
            using var client = new HttpClient();

            var response = client.GetAsync("https://api.alexandredubois.com/pont-chaban/api.php");
            var stringResult = response.Result.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<List<TTFermetures>>(stringResult.Result);
            return result;
        }
    }
}
