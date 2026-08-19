using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMWebAPIBusinessLogic.Test.Services
{
    using System.Linq.Expressions;

    using FMBusinessObjects.DataObjects;

    using FMWebAPIBusinessLogic.Services.FMBusinessLogic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class AutoDocumentServiceUnitTests
    {
        [TestMethod]
        public void HasAutoDocumentNumberAvaliable_AutomaticBOL_True()
        {
            //arrange
            var toTest = new AutoDocumentNumberService();
            var transactionAlias = new TransactionAliasClass()
            {
                TransTypeID = TransactionTypes.T5_PrimaryDisbursement
            };
            var currentSite = new SiteClass()
            {
                AutomaticBOLNextNumber = "17"
            };
            //act
            var shouldBeTrue = toTest.HasAutoDocumentNumberAvaliable(transactionAlias, currentSite);
            //assert
            Assert.IsTrue(shouldBeTrue);
        }

        [TestMethod]
        public void HasAutoDocumentNumberAvaliable_ManualBOL_True()
        {
            //arrange
            var toTest = new AutoDocumentNumberService();
            var transactionAlias = new TransactionAliasClass()
            {
                TransTypeID = TransactionTypes.T5_PrimaryDisbursement
            };
            var currentSite = new SiteClass()
            {
                AutomaticBOLNextNumber = "0",
                SeparateManualBOLNumbering = true,
                ManualBOLNextNumber = "6883"
            };
            //act
            var shouldBeTrue = toTest.HasAutoDocumentNumberAvaliable(transactionAlias, currentSite);
            //assert
            Assert.IsTrue(shouldBeTrue);
        }

        [TestMethod]
        public void HasAutoDocumentNumberAvaliable_ManualBOL_false()
        {
            //arrange
            var toTest = new AutoDocumentNumberService();
            var transactionAlias = new TransactionAliasClass()
            {
                TransTypeID = TransactionTypes.T5_PrimaryDisbursement
            };
            var currentSite = new SiteClass()
            {
                AutomaticBOLNextNumber = "0",
                SeparateManualBOLNumbering = true,
                ManualBOLNextNumber = "0"
            };
            //act
            var shouldBeFalse = toTest.HasAutoDocumentNumberAvaliable(transactionAlias, currentSite);
            //assert
            Assert.IsFalse(shouldBeFalse);
        }

        [TestMethod]
        public void HasAutoDocumentNumberAvaliable_OrderNumber_True()
        {
            //arrange
            var toTest = new AutoDocumentNumberService();
            var transactionAlias = new TransactionAliasClass()
            {
                TransTypeID = TransactionTypes.T17_Order
            };
            var currentSite = new SiteClass()
            {
                OrderNextNumber = "6883"
            };
            //act
            var shouldBeTrue = toTest.HasAutoDocumentNumberAvaliable(transactionAlias, currentSite);
            //assert
            Assert.IsTrue(shouldBeTrue);
        }

        [TestMethod]
        public void HasAutoDocumentNumberAvaliable_TransactionNumber_True()
        {
            //arrange
            var toTest = new AutoDocumentNumberService();
            var transactionAlias = new TransactionAliasClass()
            {
                TransTypeID = TransactionTypes.T6_SecondaryDisbursement
            };
            var currentSite = new SiteClass()
            {
                TransactionNextNumber = "6883"
            };
            //act
            var shouldBeTrue = toTest.HasAutoDocumentNumberAvaliable(transactionAlias, currentSite);
            //assert
            Assert.IsTrue(shouldBeTrue);
        }

        [TestMethod]
        public void HasAutoDocumentNumberAvaliable_TransactionNumber_false()
        {
            //arrange
            var toTest = new AutoDocumentNumberService();
            var transactionAlias = new TransactionAliasClass()
                                   {
                                       TransTypeID = TransactionTypes.T6_SecondaryDisbursement
                                   };
            var currentSite = new SiteClass()
                              {
                                  TransactionNextNumber = "0000000"
                              };
            //act
            var shouldBeFalse = toTest.HasAutoDocumentNumberAvaliable(transactionAlias, currentSite);
            //assert
            Assert.IsFalse(shouldBeFalse);
        }
    }
}
