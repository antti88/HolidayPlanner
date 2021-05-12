using HolidayPlanner.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HolidayPlanner;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using Moq;
using System.Diagnostics;
using System.Threading.Tasks;

namespace HolidayPlannerTest.Model.Tests
{

    [TestClass()]
    public class UnitTest1
    {
       
        [TestMethod()]
        public async void CalculateWorkDatesFromTimePeriod()
        {

            DateTime startDate = DateTime.Now.AddDays(-10);
            DateTime endDate = DateTime.Now;
            var mockFactory = new Mock<IHttpClientFactory>();
            IHttpClientFactory icf = mockFactory.Object;
            HolidayPlanner.Model.HolidayPlanner hp = new HolidayPlanner.Model.HolidayPlanner(icf);
            //Act
            int count = await hp.CalculateWorkDatesFromTimePeriod(startDate, endDate,"FI");
            //Assert
            Assert.AreEqual(count,10, "Time span days are correct");
        }
        
        //[TestMethod()]
        //public async Task CalculateHolidaysTest()
        //{
        //    int count = await TestCalculationAsync.TestCalculation();
        //    Assert.AreEqual(count, 10,"Calculation match");
        //}
        [TestMethod]
        public void CheckIsDateChronologicalOrder()
        {
            DateTime startDate = DateTime.Now.AddDays(-10);
            DateTime endDate = DateTime.Now;
            var mockFactory = new Mock<IHttpClientFactory>();
            IHttpClientFactory icf = mockFactory.Object;
            HolidayPlanner.Model.HolidayPlanner hp = new HolidayPlanner.Model.HolidayPlanner(icf);
            //Act
            bool isInRightOrder = hp.CheckIsDateChronologicalOrder(startDate, endDate);
            //Assert
            Assert.IsTrue(isInRightOrder, "Start day is newer than end day.");
            isInRightOrder = hp.CheckIsDateChronologicalOrder(endDate, startDate);
            Assert.IsFalse(isInRightOrder, "Start day is Older than end day.");
        }

        //public sealed class TestCalculationAsync 
        //{
        //    public static async Task<int> TestCalculation()
        //    {
        //        DateTime startDate = DateTime.Now.AddDays(-10);
        //        DateTime endDate = DateTime.Now;
        //        var mockFactory = new Mock<IHttpClientFactory>();
        //        IHttpClientFactory icf = mockFactory.Object;
        //        HolidayPlanner.Model.HolidayPlanner hp = new HolidayPlanner.Model.HolidayPlanner(icf);
        //        int count = await hp.CalculateHolidays(startDate, endDate);
        //        return count;
        //    }
        //}
    }
}