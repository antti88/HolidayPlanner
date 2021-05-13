using Microsoft.VisualStudio.TestTools.UnitTesting;
using HolidayPlanner.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using Moq;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Globalization;

namespace HolidayPlannerTest.Model.Tests
{
    [TestClass()]
    public class HolidayPlannerTests
    {
        [TestMethod()]
        public void CheckIsDateOverDateCountTest()
        {
            DateTime startDate = DateTime.Now.AddDays(-51);
            DateTime endDate = DateTime.Now;
            var mockFactory = new Mock<IHttpClientFactory>();
            IHttpClientFactory icf = mockFactory.Object;
            HolidayPlanner.Model.HolidayPlanner hp = new HolidayPlanner.Model.HolidayPlanner(icf);
            //Act
            bool isOver = hp.CheckIsDateOverDateCount(startDate, endDate,50);
            //Assert
            Assert.IsTrue(isOver, "days are over 50.");
            startDate = DateTime.Now.AddDays(-50);
            isOver = hp.CheckIsDateOverDateCount(startDate, endDate, 50);
            Assert.IsFalse(isOver, "days are less than 50.");
        }

        [TestMethod()]
        public void CheckIsDateChronologicalOrderTest()
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

        [TestMethod()]
        public void CheckIsDateBetweenTimePeriodTest()
        {
            DateTime startDate = DateTime.Parse("1 March 2021");
            DateTime endDate = DateTime.Parse("1 April 2021");
            var cultureInfo = new CultureInfo("fi-FI");
            string pStart = "1 April 2021";
            string pEnd = "31 March 2021";
            DateTime periodStart = DateTime.Parse(pStart, cultureInfo);
            DateTime periodEnd = DateTime.Parse(pEnd, cultureInfo);
            var mockFactory = new Mock<IHttpClientFactory>();
            IHttpClientFactory icf = mockFactory.Object;
            HolidayPlanner.Model.HolidayPlanner hp = new HolidayPlanner.Model.HolidayPlanner(icf);

            //Act
            bool isIllegalDate = hp.CheckIsDateBetweenTimePeriod(endDate,startDate, periodStart, periodEnd);
            //Assert
            Assert.IsFalse(isIllegalDate, "Date isn't between time period, date ok!");
            //Act
            isIllegalDate = hp.CheckIsDateBetweenTimePeriod(startDate, endDate, periodStart, periodEnd);
            //Assert
            Assert.IsTrue(isIllegalDate, "Date is between time period, date Failed!");

        }

        [TestMethod()]
        public async void CalculateWorkDatesFromTimePeriodTest()
        {
            //CANT RUN ASYNC TEST
            DateTime startDate = DateTime.Now.AddDays(-10);
            DateTime endDate = DateTime.Now;
            var mockFactory = new Mock<IHttpClientFactory>();
            IHttpClientFactory icf = mockFactory.Object;
            HolidayPlanner.Model.HolidayPlanner hp = new HolidayPlanner.Model.HolidayPlanner(icf);
            //Act
            int count = await hp.CalculateWorkDatesFromTimePeriod(startDate, endDate, "FI", "fi-FI");
            Debug.WriteLine("Workdays from timeperiod: " + count);
            //Assert
            Assert.AreEqual(count, 10, "Time span days are correct");
        }

        [TestMethod()]
        public void GetHolidayCountFromTimeperiodTest()
        {
            //Includes async
        }
    }
}