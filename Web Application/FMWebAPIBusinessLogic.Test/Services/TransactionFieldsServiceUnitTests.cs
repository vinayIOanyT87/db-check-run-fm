using FMBusinessObjects.DataObjects;
using FMCore.Interfaces;
using FMWebAPIBusinessLogic.Interfaces.FMProxy;
using FMWebAPIBusinessLogic.Services.FMBusinessLogic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMWebAPIBusinessLogic.Test.Services
{
    [TestClass]
    public class TransactionFieldsServiceUnitTests
    {
        [TestMethod]
        public void ShouldAssignColumnInfoToCorrectField()
        {
            //arrange
            var transactionAliasFieldsProxyMock = new Mock<ITransactionAliasFieldsProxy>();
            var transactionAliasesProxyMock = new Mock<ITransactionAliasesProxy>();
            var fmLoggerMock = new Mock<IFMCustomLogger>();
            Guid transactionGuid = Guid.NewGuid();

            transactionAliasesProxyMock.Setup(x => x.Get(transactionGuid, false))
                .Returns(new TransactionAliasClass()
                {
                    TransactionFieldCollection = new TransactionAliasFieldCollectionClass()
                    {
                        new TransactionAliasFieldClass()
                        {
                            ID = "Test"
                        }
                    }
                });
            transactionAliasFieldsProxyMock.Setup(x => x.GetColumnDefinitionsForTransactions())
                .Returns(new List<TransactionAliasFieldExtendedAttributes>()
                {
                    new TransactionAliasFieldExtendedAttributes()
                    {
                        PropertyName = "Test",
                        ColumnName = "Test"
                    }
                });

            var toTest = new TransactionFieldsService(transactionAliasFieldsProxyMock.Object,
                transactionAliasesProxyMock.Object,
                fmLoggerMock.Object);
            //act
            var fields = toTest.GeTransactionFieldDefinitionsForUI(transactionGuid);
            //assert
            Assert.AreEqual(1, fields.Count());
        }
    }
}
