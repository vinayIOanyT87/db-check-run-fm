using FMWebAPIBusinessLogic.Services.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using FMCore.Interfaces;
namespace FMWebAPIBusinessLogic.Test.Services
{
    using System;
    using System.Threading;

    [TestClass]
    public class CacheServiceUnitTests
    {
        [TestMethod]
        public void AddGet_AddingValidObject_AbleToRetrieveIt()
        {
            //setup
            var loggerMock = new Mock<IFMCustomLogger>();
            var cacheService = new CacheService(loggerMock.Object);

            //act
            cacheService.Add<string>("first", "FirstToSave");
            cacheService.Add<string>("second", "SecondToSave");
            var secondEntry = cacheService.Get<string>("second");
            var firstEntry = cacheService.Get<string>("first");

            //assert
            Assert.AreEqual("FirstToSave", firstEntry);
            Assert.AreEqual("SecondToSave", secondEntry);
        }

        [TestMethod]
        public void Get_NotAvalidObject_ReturnsNull()
        {
            //setup
            var loggerMock = new Mock<IFMCustomLogger>();
            var cacheService = new CacheService(loggerMock.Object);

            //act
            var nullEntry = cacheService.Get<string>("expectingNull");

            //assert
            Assert.IsNull(nullEntry);
        }

        [TestMethod]
        public void AddGetSingleton_ActsAsSingleton_AbleToRetrieveIt()
        {
            //setup
            var loggerMock = new Mock<IFMCustomLogger>();
            var cacheService1 = new CacheService(loggerMock.Object);
            var cacheService2 = new CacheService(loggerMock.Object);

            //act
            cacheService1.Add<string>("third", "ThirdToSave");
            cacheService1.Add<string>("fourth", "FourthToSave");
            var thirdEntry = cacheService2.Get<string>("third");
            var fourthEntry = cacheService2.Get<string>("fourth");

            //assert
            Assert.AreEqual("ThirdToSave", thirdEntry);
            Assert.AreEqual("FourthToSave", fourthEntry);
        }

        [TestMethod]
        public void EntryTimesOut_EntryIsNull_Timedout()
        {
            //setup
            var loggerMock = new Mock<IFMCustomLogger>();
            var cacheService1 = new CacheService(loggerMock.Object);

            //act
            cacheService1.Add<string>("451RandomKey", "this is a random object", new TimeSpan(0,0,1));
            Thread.Sleep(2000);
            var stashedEntry = cacheService1.Get<string>("451RandomKey");
            //assert
            Assert.IsNull(stashedEntry);
        }

        [TestMethod]
        public void Clear_CacheIsFilled_Cleared()
        {
            //setup
            var loggerMock = new Mock<IFMCustomLogger>();
            var cacheService = new CacheService(loggerMock.Object);

            //act
            cacheService.Add<string>("fifth", "FirstToSave");
            cacheService.Add<string>("six", "SecondToSave");
            var firstEntry = cacheService.Get<string>("fifth");
            var secondEntry = cacheService.Get<string>("six");
            cacheService.Clear();
            var firstEntryCleared = cacheService.Get<string>("fifth");
            var secondEntryCleared = cacheService.Get<string>("six");

            //assert
            Assert.AreEqual("FirstToSave", firstEntry);
            Assert.AreEqual("SecondToSave", secondEntry);
            Assert.IsNull(firstEntryCleared);
            Assert.IsNull(secondEntryCleared);
        }
    }
}
