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
        public bool CheckIsDateOverDateCount(DateTime startDate, DateTime endDate, int dates)
        {

            bool isValid;
            TimeSpan timeSpan = endDate - startDate;
            Debug.WriteLine("TotalDays: " + timeSpan.TotalDays);
            Debug.WriteLine("Days:" + timeSpan.Days);
            if (timeSpan.TotalDays >= dates)
            {
                Debug.WriteLine("Dates over 50");
                isValid = false;
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
        public bool CheckIsDateBetweenTimePeriod(DateTime date, DateTime timePeriodStart, DateTime timePeriodEnd)
        {
            bool isValid;
            timePeriodEnd = timePeriodEnd.AddDays(-1);
            timePeriodStart = timePeriodStart.AddDays(1);
            if (date > timePeriodStart && date < timePeriodEnd)
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
        
        /// <summary>
        /// Get dates between two date times
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns>int count of days</returns>
        public async Task<int> CalculateWorkDatesFromTimePeriod(DateTime startDate, DateTime endDate,string countryCode,string cultureCode)
        {
            List<DateTime> publicHolidaysList = await GetPublicHolidays(startDate.Year,countryCode, cultureCode);
            bool isLoading = true;
            int dayCount = 0;
            int count = 0;
            var cultureInfo = new CultureInfo(cultureCode);
            startDate = DateTime.Parse(startDate.AddDays(-1).ToString(), cultureInfo);
            Debug.WriteLine(JsonConvert.SerializeObject(publicHolidaysList));
            do
            {
                startDate = startDate.AddDays(1);
                count++;
                Debug.WriteLine("Check day: " + startDate.Date);
                if (publicHolidaysList.Contains(startDate.Date) || startDate.DayOfWeek.ToString() == "Sunday") 
                {
                    Debug.WriteLine("Current day is public holiday or it is sunday");
                }
                else
                {

                    dayCount++;
                }


                if(startDate.AddDays(count).Date>endDate.Date)
                {
                    isLoading = false;
                }

            } while (isLoading);
            Debug.WriteLine("Return workdays: " + dayCount);
            return dayCount;
        }
        public async Task<int> CalculateHolidays(DateTime startDate, DateTime endDate, string countryCode, string cultureCode)
        {
            int days = 0;
            var cultureInfo = new CultureInfo(cultureCode);
            string pStart = "1 April " + startDate.Year;
            string pEnd = "31 March " + (startDate.Year + 1);
            DateTime periodStart = DateTime.Parse(pStart, cultureInfo);
            DateTime periodEnd = DateTime.Parse(pEnd, cultureInfo);
            if (CheckIsDateChronologicalOrder(startDate,startDate)||!CheckIsDateOverDateCount(startDate,endDate,50)||CheckIsDateBetweenTimePeriod(startDate,periodStart,periodEnd)||CheckIsDateBetweenTimePeriod(endDate,periodStart,periodEnd))
            {
                List<DateTime> publicHolidaysList = await GetPublicHolidays(startDate.Year,countryCode, cultureCode);
                
                return publicHolidaysList.Count;
            }
            else
            {
                return days;
            }

        }
        private async Task<List<DateTime>> GetPublicHolidays(int year, string countryCode,string cultureCode)
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
                            var cultureInfo = new CultureInfo(cultureCode);
                            //Debug.WriteLine("Add public date: " + item.date);
                            DateTime dt = DateTime.Parse(item.date, cultureInfo);
                            //Debug.WriteLine("Add public date parsed: " + dt);
                            //Debug.WriteLine(dt.DayOfWeek.ToString());
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
