using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace HolidayPlanner.Model
{
    public class HolidayPlanner
    {
        private readonly IHttpClientFactory _clientFactory;
        public HolidayPlanner(IHttpClientFactory clientFactory)
        {
            Debug.WriteLine("HP fired");
            _clientFactory = clientFactory;
        }
        /// <summary>
        /// Check if date is over given count of days
        /// </summary>
        /// <param name="StartDate">Start date</param>
        /// <param name="EndDate">End date</param>
        /// <param name="dates">count of days</param>
        /// <returns></returns>
        public bool CheckIsDateChronologicalOrder(DateTime startDate, DateTime endDate)
        {
            bool isValid;
            if (startDate > endDate)
            {
                isValid = false;
            }
            else
            {
                isValid = true;
            }
            return isValid;
        }
        public bool CheckIsDateOver(DateTime startDate, DateTime endDate,int dates)
        {
            
            bool isValid;
            TimeSpan timeSpan = endDate - startDate;
            Debug.WriteLine("TotalDays: "+timeSpan.TotalDays);
            Debug.WriteLine("Days:" + timeSpan.Days);
            if(timeSpan.TotalDays > dates)
            {
                Debug.WriteLine("Dates over 50");
                isValid =false;
            }
            else
            {
                isValid = true;
            }

            return isValid;
        }
        /// <summary>
        /// Check if given date is between to DateTimes
        /// </summary>
        /// <param name="date">DateTime</param>
        /// <param name="timePeriodStart">Between starts</param>
        /// <param name="timePeriodEnd">between ends</param>
        /// <returns></returns>
        public bool CheckIsDateBetweenTimePeriod(DateTime date,DateTime timePeriodStart, DateTime timePeriodEnd)
        {
            bool isValid;
            timePeriodEnd = timePeriodEnd.AddDays(-1);
            timePeriodStart = timePeriodStart.AddDays(1);
            if(date > timePeriodStart&& date<timePeriodEnd)
            {
                Debug.WriteLine("timeperiod is between timeperiod");
                isValid = true;
            }
            else
            {
                isValid = false;
            }

            return isValid;

        }
        public async void CalculateHolidays(DateTime startDate)
        {
            await GetPublicHolidays(startDate.Year, "FI");
        }
        private async Task<List<DateTime>> GetPublicHolidays(int year,string countryCode)
        {
            string uri = "https://date.nager.at/Api/v2/PublicHolidays/";
            string suffix = year+"/"+countryCode;
            var client = _clientFactory.CreateClient("restClient");
            var request = new HttpRequestMessage(HttpMethod.Get, uri + suffix);
            var responseClient = await client.SendAsync(request);
            var responseContent = await responseClient.Content.ReadAsStringAsync();
            List<DateTime> listOfDates = new List<DateTime>();
            listOfDates.Add(DateTime.Now);

            Debug.WriteLine("GetpublicHolidays responsecontent: " + responseContent);
            if (responseClient.IsSuccessStatusCode)
            {
                Debug.WriteLine("GetpublicHolidays Response Success! : " + responseClient.ToString());
                return listOfDates;
            }
            else
            {
                Debug.WriteLine("GetpublicHolidays RESPONSE FAILL! : " + responseClient.ToString());
                return null;
            }

        }
    }
}
