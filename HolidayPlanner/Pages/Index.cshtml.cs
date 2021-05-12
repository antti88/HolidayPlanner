using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace HolidayPlanner.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        [BindProperty, DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true),DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        [BindProperty, DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true), DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
        public Model.HolidayPlanner _HolidayPlanner { get; set; }
        public IndexModel(ILogger<IndexModel> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            Debug.WriteLine("IndexModel fired");
            _HolidayPlanner = new Model.HolidayPlanner(httpClientFactory);
            StartDate = DateTime.Now;
            EndDate = DateTime.Now;
        }

        public void OnGet()
        {
            Debug.WriteLine("OnGet fired: " + StartDate + " " + EndDate);
        }
        public void OnGetStartDate()
        {
            Debug.WriteLine("Start date fired");
        }
        public void OnGetEndDate()
        {
            Debug.WriteLine("End Date fired");
        }
        public async Task OnPostAsync(DateTime startDate, DateTime endDate)
        {
            int workDays = await _HolidayPlanner.CalculateWorkDatesFromTimePeriod(startDate, endDate, "FI","fi-FI");
            Debug.WriteLine("WORKDAYS: " + workDays);
        }

    }
}
