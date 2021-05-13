using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace HolidayPlanner.Model
{
    public class HolidayPlanner
    {
        private readonly IHttpClientFactory _clientFactory;
        public string msg {get;set;}
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
        /// <returns>boolean</returns>
        public bool CheckIsDateChronologicalOrder(DateTime startDate, DateTime endDate)
        {
            Debug.WriteLine("CheckIsDateChronologicalOrder fired");
            bool isValid;
            Debug.WriteLine(startDate.ToString() + " - " + endDate.ToString());
            if (startDate.Date <= endDate.Date)
            {
                Debug.WriteLine("startDay is lower than end day");
                isValid = true;
            }
            else
            {
                Debug.WriteLine("startDay is higher than end day");
                isValid = false;
            }
            Debug.WriteLine("is in order: " + isValid);
            return isValid;
        }
        /// <summary>
        /// Check if dates count are over given count
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="dates"></param>
        /// <returns>boolean</returns>
        public bool CheckIsDateOverDateCount(DateTime startDate, DateTime endDate, int dates)
        {

            bool isValid;
            TimeSpan timeSpan = endDate - startDate;
            Debug.WriteLine("TotalDays: " + timeSpan.TotalDays);
            Debug.WriteLine("Days:" + timeSpan.Days);
            if (timeSpan.TotalDays >= dates)
            {
                Debug.WriteLine("Dates over 50");
                isValid = true;
            }
            else
            {
                isValid = false;
            }

            return isValid;
        }
        /// <summary>
        /// Check if given date is between to DateTimes
        /// </summary>
        /// <param name="date">DateTime</param>
        /// <param name="timePeriodStart">Between starts</param>
        /// <param name="timePeriodEnd">between ends</param>
        /// <returns>boolean</returns>
        public bool CheckIsDateBetweenTimePeriod(DateTime startDate,DateTime endDate, DateTime timePeriodStart, DateTime timePeriodEnd)
        {
            bool isValid;
            Debug.WriteLine(startDate + " " + endDate);
            timePeriodEnd = timePeriodEnd.AddDays(1);
            timePeriodStart = timePeriodStart.AddDays(-2);
            Debug.WriteLine(timePeriodStart + " " + timePeriodEnd);
            TimeSpan span = endDate- startDate;
            bool isLoading = true;
            DateTime date = startDate;
            Debug.WriteLine("Dates span:"+span.Days);
            int count = 0;
            do
            {
                count++;
                if (timePeriodStart.Date <date.Date && timePeriodEnd.Date>date.Date)
                {
                    Debug.WriteLine("timeperiod is between timeperiod");
                    Debug.WriteLine(timePeriodStart.Date + " <= " + date.Date + " >= " + timePeriodEnd.Date);
                    isValid = true;
                    isLoading = false;
                }
                else
                {
                    Debug.WriteLine(timePeriodStart.Date + " <- " + date.Date + " -> " + timePeriodEnd.Date);
                    Debug.WriteLine("Date is not between timeperiod");
                    isValid = false;
                }
                date = date.AddDays(1);
                if (count >= span.Days)
                {
                    isLoading = false;
                }
                
            } while (isLoading);
           

            return isValid;

        }

        /// <summary>
        /// Get dates between two date times
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns>int count of days</returns>
        public async Task<int> CalculateWorkDatesFromTimePeriod(DateTime startDate, DateTime endDate, string countryCode, string cultureCode)
        {
            List<DateTime> publicHolidaysList = await GetPublicHolidays(startDate.Year, countryCode);
            bool isLoading = true;
            int dayCount = 0;
            int count = 0;
            Debug.WriteLine(JsonConvert.SerializeObject(publicHolidaysList));
            endDate = endDate.AddDays(1);
            bool isWorkDay = true;
            TimeSpan span = endDate - startDate;
            do
            {
                isWorkDay = true;
                count++;
                Debug.WriteLine("Check day: " + startDate.Date + " count:" + count + "/" + span.Days);
                foreach (var item in publicHolidaysList)
                {

                    if (item.Date == startDate.Date)
                    {
                        Debug.WriteLine(startDate.Date + " : " + item);
                        Debug.WriteLine("It is a holiday!");
                        isWorkDay = false;
                    }
                    else
                    {

                    }
                }
                if (startDate.DayOfWeek.ToString() == "Sunday")
                {
                    Debug.WriteLine("Current day is Sunday");
                    isWorkDay = false;
                }
                if (isWorkDay)
                {
                    dayCount++;
                }
                startDate = startDate.AddDays(1);
                if (count >= span.Days)
                {
                    isLoading = false;
                }

            } while (isLoading);
            Debug.WriteLine("Return workdays: " + dayCount);
            return dayCount;
        }
        /// <summary>
        /// Calculate how many holidays
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="countryCode"></param>
        /// <param name="cultureCode"></param>
        /// <returns></returns>
        public async Task<int> GetHolidayCountFromTimeperiod(DateTime startDate, DateTime endDate, string countryCode, string cultureCode)
        {
            int days = 0;
            var cultureInfo = new CultureInfo(cultureCode);
            string pStart = "1 April " + startDate.Year;
            string pEnd = "31 March " + (startDate.Year);
            DateTime periodStart = DateTime.Parse(pStart, cultureInfo);
            DateTime periodEnd = DateTime.Parse(pEnd, cultureInfo);
            if (CheckIsDateChronologicalOrder(startDate, startDate))
            {
                Debug.WriteLine("ChronologicalOrder pass");

                if(!CheckIsDateOverDateCount(startDate, endDate, 50))
                {
                    Debug.WriteLine("CheckIsDateOverDateCount pass");
                    if (!CheckIsDateBetweenTimePeriod(startDate, endDate, periodStart, periodEnd))
                    {
                        Debug.WriteLine("CheckIsDateBetweenTimePeriod pass");
                        days = await CalculateWorkDatesFromTimePeriod(startDate, endDate, countryCode, cultureCode);
                        return days;
                    }
                    else
                    {
                        msg = "The whole time span has to be within the same holiday period that begins on the 1st of April and ends on the 31st of March.";
                    }
                }
                else
                {
                    msg = "The maximum length of the time span is 50 days";
                }
            }
            else
            {
                msg = "The dates for the time span must be in chronological order";
            }
            return days;
        }
        /// <summary>
        /// Get public holidays from REST API
        /// </summary>
        /// <param name="year">year</param>
        /// <param name="countryCode">country code</param>
        /// <returns></returns>
        private async Task<List<DateTime>> GetPublicHolidays(int year, string countryCode)
        {
            string uri = "https://date.nager.at/Api/v2/PublicHolidays/";
            string suffix = year + "/" + countryCode;
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
                if (!string.IsNullOrEmpty(responseContent))
                {
                    List<PublicHolidaysDatamodel> listOfPublicDates = JsonConvert.DeserializeObject<List<PublicHolidaysDatamodel>>(responseContent);
                    foreach (var item in listOfPublicDates)
                    {
                        if (item.global)
                        {
                            DateTime dt = DateTime.Parse(item.date);
                            listOfDates.Add(dt.Date);
                        }

                    }
                }
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
