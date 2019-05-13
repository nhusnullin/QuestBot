using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreBot.Domain;
using CoreBot.Service;
using Microsoft.AspNetCore.Mvc;

namespace CoreBot.Controllers
{
    [Route("quest")]
    [ApiController]
    public class QuestController : Controller
    {
        private readonly IReportService _reportService;

        public QuestController(IReportService reportService)
        {
            _reportService = reportService;
        }
        [HttpGet]
        [Route("results")]
        public async Task<IActionResult> GetResults()
        {
            var result = await _reportService.GetTeamQuestResults();
            return View("TeamResult", result);
        }
    }
}