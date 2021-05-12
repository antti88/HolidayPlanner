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
        public void OnPost(DateTime startDate, DateTime endDate)
        {
            Debug.WriteLine("CalculateDates fired: " + startDate +"\n" +endDate);
            Debug.WriteLine("class dates: " + StartDate + " " + EndDate);
            bool isValid = _HolidayPlanner.CheckIsDateOver(startDate, endDate,50);
            var cultureInfo = new CultureInfo("fi-FI");
            Debug.WriteLine("IS VALID: " + isValid);
            string pStart = "1 April " + startDate.Year;
            string pEnd = "31 March " + (startDate.Year+1);
            DateTime periodStart = DateTime.Parse(pStart, cultureInfo);
            DateTime periodEnd = DateTime.Parse(pEnd, cultureInfo);
            bool isInBetween = _HolidayPlanner.CheckIsDateBetweenTimePeriod(startDate, periodStart, periodEnd);
            Debug.WriteLine("Start is between time period: " + isInBetween);
            pStart = "1 April " + endDate.Year;
            pEnd = "31 March " + (endDate.Year+1);
            periodStart = DateTime.Parse(pStart, cultureInfo);
            periodEnd = DateTime.Parse(pEnd, cultureInfo);
            isInBetween = _HolidayPlanner.CheckIsDateBetweenTimePeriod(endDate, periodStart, periodEnd);
            Debug.WriteLine("EndDate is between time period: " + isInBetween);
            bool isDateValid = _HolidayPlanner.CheckIsDateChronologicalOrder(startDate, endDate);
            Debug.WriteLine("Is date valid chronological order: " + isDateValid);

            _HolidayPlanner.CalculateHolidays(startDate);
        }

    }
}
