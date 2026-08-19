using System;
using FMWebAPIBusinessLogic.Services.FMBusinessLogic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FMBusinessObjects.DataObjects;
using Moq;
using FMWebAPIBusinessLogic.Interfaces.FMProxy;

namespace FMWebAPIBusinessLogic.Test.Services
{
    [TestClass]
    public class MeterActionServiceUnitTests
    {
        [TestMethod]
        public void DetectsMeterRollover_MeterDidRollOver_ReturnsTrue()
        {
            var meterMock = new Mock<IMetersProxy>();
            var transactionAliasProxyMock = new Mock<ITransactionAliasesProxy>();
            var meterGuid = Guid.NewGuid();
            var transactionAliasGuid = Guid.NewGuid();
            meterMock.Setup(x => x.GetIdentityGuid("RandomMeterID")).Returns(meterGuid);
            meterMock.Setup(x => x.Get(meterGuid)).Returns(new MeterClass()
            {
                NumberOfDigits = 6
            });
            transactionAliasProxyMock.Setup(x => x.Get(transactionAliasGuid, false))
                .Returns(new TransactionAliasClass()
                {
                    TransTypeID = TransactionTypes.T1_PrimaryAdjustment
                });
            //arrange
            var toTest = new MeterActionService(meterMock.Object, transactionAliasProxyMock.Object);
            //act
            var wasRollOver = toTest.DidMeterRollover("RandomMeterID", transactionAliasGuid, 999999, 1);
            //assert
            Assert.IsTrue(wasRollOver.MeterOverflowed);
            Assert.AreEqual(2, wasRollOver.Difference);
        }


        [TestMethod]
        public void DetectsMeterRollover_MeterDidNotRollOver_ReturnsFalse()
        {
            var meterMock = new Mock<IMetersProxy>();
            var transactionAliasProxyMock = new Mock<ITransactionAliasesProxy>();
            var meterGuid = Guid.NewGuid();
            var transactionAliasGuid = Guid.NewGuid();
            meterMock.Setup(x => x.GetIdentityGuid("RandomMeterID")).Returns(meterGuid);
            meterMock.Setup(x => x.Get(meterGuid)).Returns(new MeterClass()
                                                           {
                                                               NumberOfDigits = 6
                                                           });
            transactionAliasProxyMock.Setup(x => x.Get(transactionAliasGuid, false))
                .Returns(new TransactionAliasClass()
                         {
                             TransTypeID = TransactionTypes.T1_PrimaryAdjustment
                         });
            //arrange
            var toTest = new MeterActionService(meterMock.Object, transactionAliasProxyMock.Object);
            //act
            var wasRollOver = toTest.DidMeterRollover("RandomMeterID", transactionAliasGuid, 999999, 951555);
            //assert
            Assert.IsFalse(wasRollOver.MeterOverflowed);
            Assert.AreEqual(-48444, wasRollOver.Difference);
        }

        [TestMethod]
        public void DetectsMeterRollover_MeterDidRollOverWithDefuelAndMetreRollsBackwards_ReturnsTrue()
        {
            var meterMock = new Mock<IMetersProxy>();
            var transactionAliasProxyMock = new Mock<ITransactionAliasesProxy>();
            var meterGuid = Guid.NewGuid();
            var transactionAliasGuid = Guid.NewGuid();
            meterMock.Setup(x => x.GetIdentityGuid("RandomMeterID")).Returns(meterGuid);
            meterMock.Setup(x => x.Get(meterGuid)).Returns(new MeterClass()
               {
                   NumberOfDigits = 6,
                   RotatesBackwardsFlag = true
            });
            transactionAliasProxyMock.Setup(x => x.Get(transactionAliasGuid, false))
                .Returns(new TransactionAliasClass()
                         {
                             TransTypeID = TransactionTypes.T4_SecondaryDefuel
                });
            //arrange
            var toTest = new MeterActionService(meterMock.Object, transactionAliasProxyMock.Object);
            //act
            var wasRollOver = toTest.DidMeterRollover("RandomMeterID", transactionAliasGuid, 1, 999999);
            //assert
            Assert.IsTrue(wasRollOver.MeterOverflowed);
            Assert.AreEqual(2, wasRollOver.Difference);
        }


        [TestMethod]
        public void DetectsMeterRollover_MeterDidNotRollOverWithDefuelAndMeterRollsBackwards_ReturnsFalse()
        {
            var meterMock = new Mock<IMetersProxy>();
            var transactionAliasProxyMock = new Mock<ITransactionAliasesProxy>();
            var meterGuid = Guid.NewGuid();
            var transactionAliasGuid = Guid.NewGuid();
            meterMock.Setup(x => x.GetIdentityGuid("RandomMeterID")).Returns(meterGuid);
            meterMock.Setup(x => x.Get(meterGuid)).Returns(new MeterClass()
               {
                   NumberOfDigits = 6,
                   RotatesBackwardsFlag = true
            });
            transactionAliasProxyMock.Setup(x => x.Get(transactionAliasGuid, false))
                .Returns(new TransactionAliasClass()
                         {
                             TransTypeID = TransactionTypes.T4_SecondaryDefuel
                });
            //arrange
            var toTest = new MeterActionService(meterMock.Object, transactionAliasProxyMock.Object);
            //act
            var wasRollOver = toTest.DidMeterRollover("RandomMeterID", transactionAliasGuid, 951555, 999999);
            //assert
            Assert.IsFalse(wasRollOver.MeterOverflowed);
            Assert.AreEqual(-48444, wasRollOver.Difference);
        }
    }
}
