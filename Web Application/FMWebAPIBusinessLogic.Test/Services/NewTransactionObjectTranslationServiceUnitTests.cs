using System;
using System.Collections.Generic;
using System.Linq;
using FMBusinessObjects.DataObjects;
using FMWebAPIBusinessLogic.DTO.TransactionDTO;
using FMWebAPIBusinessLogic.Interfaces.FMBusinessLogic;
using FMWebAPIBusinessLogic.Interfaces.FMProxy;
using FMWebAPIBusinessLogic.Services.FMBusinessLogic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace FMWebAPIBusinessLogic.Test.Services
{
    using FMCore.Interfaces;

    [TestClass]
    public class NewTransactionObjectTranslationServiceUnitTests
    {
        private Mock<ITransactionFieldsService> transactionFieldsServiceMock = null;
        private Mock<ICurrentRequestContext> currentUserSecurityMock = null;
        private Mock<ITransactionAliasesProxy> transactionAliasesProxyMock = null;
        private Mock<IFMCustomLogger> loggerMock = null;
        private TransactionAliasClass transactionAliasClass = null;

        //returns the Transaction guid to test againts
        private Guid Setup()
        {
            this.transactionFieldsServiceMock = new Mock<ITransactionFieldsService>();
            this.currentUserSecurityMock = new Mock<ICurrentRequestContext>();
            this.transactionAliasesProxyMock = new Mock<ITransactionAliasesProxy>();
            this.loggerMock = new Mock<IFMCustomLogger>();

            this.currentUserSecurityMock
                .Setup(x => x.GetCurrentSecurityContext())
                .Returns(new SecurityClass()
                {
                    SiteID = "MyLittleAirport"
                });
            this.currentUserSecurityMock
                .Setup(x => x.GetCurrentSite())
                .Returns(new SiteClass() { });
            var transactionAliasGuid = Guid.NewGuid();
            this.transactionAliasesProxyMock
                .Setup(x => x.GetIdentityGuid("Issue"))
                .Returns(transactionAliasGuid);
            this.transactionAliasClass = new TransactionAliasClass() { IdentityGuid = transactionAliasGuid };
            this.transactionAliasesProxyMock
                .Setup(x => x.Get(transactionAliasGuid, It.IsAny<bool>()))
                .Returns(this.transactionAliasClass);
            return transactionAliasGuid;
        }

        [TestMethod]
        public void ShouldAssignDocumentNumber()
        {
            //arrange
            var transactionAliasGuid = this.Setup();

            this.transactionFieldsServiceMock
                .Setup(x => x.GeTransactionFieldDefinitionsForUI(It.Is<TransactionAliasClass>(t => t.IdentityGuid == transactionAliasGuid)))
                .Returns(new List<TransactionAliasFieldClassWithColumn>() {
                    new TransactionAliasFieldClassWithColumn(){
                        Type = TransactionFieldType.Transaction,
                        ID = "DocumentNumber",
                        PropertyPath = "DocumentNumber"
                    }
                });

            var toBeTested = new TransactionObjectTranslationService(
                this.transactionFieldsServiceMock.Object,
                this.currentUserSecurityMock.Object,
                this.transactionAliasesProxyMock.Object,
                this.loggerMock.Object
                );
            //act
            var toParseTransaction = new Dictionary<string, string>();
            toParseTransaction.Add("DocumentNumber", "123456");
            var transaction = new TransactionDO()
                              {
                                  LineItems = new List<LineItemDO>()
                                              {
                                                  new LineItemDO()
                                              }
                              };
            toBeTested.ApplyDictionaryToTransaction(transaction, toParseTransaction, this.transactionAliasClass);
            //assert
            Assert.AreEqual("123456", transaction.DocumentNumber);
        }



        [TestMethod]
        public void ShouldAssignStatus()
        {
            //arrange
            var transactionAliasGuid = this.Setup();

            this.transactionFieldsServiceMock
                .Setup(x => x.GeTransactionFieldDefinitionsForUI(It.Is<TransactionAliasClass>(t=> t.IdentityGuid == transactionAliasGuid)))
                .Returns(new List<TransactionAliasFieldClassWithColumn>() {
                  new TransactionAliasFieldClassWithColumn(){
                                                                Type = TransactionFieldType.Transaction,
                                                                ID = "LookupTransactionStatusIndex",
                                                                PropertyPath = "Status"
                                                            }
              });

            var toBeTested = new TransactionObjectTranslationService(
                this.transactionFieldsServiceMock.Object,
                this.currentUserSecurityMock.Object,
                this.transactionAliasesProxyMock.Object,
                this.loggerMock.Object
            );
            //act
            var toParseTransaction = new Dictionary<string, string>();
            toParseTransaction.Add("LookupTransactionStatusIndex", "Enterprise");
            var transaction = new TransactionDO()
                              {
                                  LineItems = new List<LineItemDO>()
                                              {
                                                  new LineItemDO()
                                              }
                              };
            toBeTested.ApplyDictionaryToTransaction(transaction, toParseTransaction, this.transactionAliasClass);
            //assert
            Assert.AreEqual(TransactionStatus.Enterprise, transaction.Status);
        }

        [TestMethod]
        public void ShoulCreatePaymentInfotAndThenAssigCrediCardNumber()
        {
            //arrange
            var transactionAliasGuid = this.Setup();

            this.transactionFieldsServiceMock
                .Setup(x => x.GeTransactionFieldDefinitionsForUI(It.Is<TransactionAliasClass>(t => t.IdentityGuid == transactionAliasGuid)))
                .Returns(new List<TransactionAliasFieldClassWithColumn>() {
                    new TransactionAliasFieldClassWithColumn(){
                        Type = TransactionFieldType.Transaction,
                        ID = "CardNumber",
                        PropertyPath = "PaymentInfo.CreditCardNumber"
                    }
                });

            var toBeTested = new TransactionObjectTranslationService(
                this.transactionFieldsServiceMock.Object,
                this.currentUserSecurityMock.Object,
                this.transactionAliasesProxyMock.Object,
                this.loggerMock.Object
                );
            //act
            var toParseTransaction = new Dictionary<string, string>();
            toParseTransaction.Add("CardNumber", "123456");
            var transaction = new TransactionDO();
            toBeTested.ApplyDictionaryToTransaction(transaction, toParseTransaction, this.transactionAliasClass);
            //assert
            Assert.IsNotNull(transaction.PaymentInfo);
            Assert.AreEqual("123456", transaction.PaymentInfo.CreditCardNumber);
        }

        [TestMethod]
        public void ShoulConvertAndAssignVCFToLineItem()
        {
            //arrange
            var transactionAliasGuid = this.Setup();

            this.transactionFieldsServiceMock
                .Setup(x => x.GeTransactionFieldDefinitionsForUI(It.Is<TransactionAliasClass>(t => t.IdentityGuid == transactionAliasGuid)))
                .Returns(new List<TransactionAliasFieldClassWithColumn>() {
                    new TransactionAliasFieldClassWithColumn(){
                        Type = TransactionFieldType.LineItem,
                        ID = "Vcf",
                        PropertyPath = "Vcf"
                    }
                });

            var toBeTested = new TransactionObjectTranslationService(
                this.transactionFieldsServiceMock.Object,
                this.currentUserSecurityMock.Object,
                this.transactionAliasesProxyMock.Object,
                this.loggerMock.Object
                );
            //act
            var toParseTransaction = new Dictionary<string, string>();
            toParseTransaction.Add("Vcf", "1.55");
            var transaction = new TransactionDO()
                              {
                                  LineItems = new List<LineItemDO>()
                                              {
                                                  new LineItemDO()
                                              }
                              };
            toBeTested.ApplyDictionaryToTransaction(transaction, toParseTransaction, this.transactionAliasClass);
            //assert
            Assert.IsNotNull(transaction.LineItems.FirstOrDefault());
            Assert.AreEqual(1.55d, transaction.LineItems.First().VCF);
        }

        [TestMethod]
        public void ShoulConvertAndGrossQuantityToLineItem()
        {
            //arrange
            var transactionAliasGuid = this.Setup();

            this.transactionFieldsServiceMock
                .Setup(x => x.GeTransactionFieldDefinitionsForUI(It.Is<TransactionAliasClass>(t => t.IdentityGuid == transactionAliasGuid)))
                .Returns(new List<TransactionAliasFieldClassWithColumn>() {
                    new TransactionAliasFieldClassWithColumn(){
                        Type = TransactionFieldType.LineItem,
                        ID = "GrossQuantity",
                        PropertyPath = "Quantity.GrossInventoryChange"
                    }
                });

            var toBeTested = new TransactionObjectTranslationService(
                this.transactionFieldsServiceMock.Object,
                this.currentUserSecurityMock.Object,
                this.transactionAliasesProxyMock.Object,
                this.loggerMock.Object
                );
            //act
            var toParseTransaction = new Dictionary<string, string>();
            toParseTransaction.Add("GrossQuantity", "200");
            var transaction = new TransactionDO()
            {
                LineItems = new List<LineItemDO>()
                                              {
                                                  new LineItemDO()
                                              }
            };
            toBeTested.ApplyDictionaryToTransaction(transaction, toParseTransaction, this.transactionAliasClass);
            //assert
            Assert.IsNotNull(transaction.LineItems.FirstOrDefault());
            Assert.AreEqual(200d, transaction.LineItems.First().Quantity.GrossInventoryChange);
        }


        [TestMethod]
        public void ShouldConvertInventoryDate()
        {
            //arrange
            var transactionAliasGuid = this.Setup();

            this.transactionFieldsServiceMock
                .Setup(x => x.GeTransactionFieldDefinitionsForUI(It.Is<TransactionAliasClass>(t => t.IdentityGuid == transactionAliasGuid)))
                .Returns(new List<TransactionAliasFieldClassWithColumn>() {
                      new TransactionAliasFieldClassWithColumn(){
                                                                    Type = TransactionFieldType.Transaction,
                                                                    ID = "InventoryDate",
                                                                    PropertyPath = "InventoryDate"
                                                                }
                  });

            var toBeTested = new TransactionObjectTranslationService(
                this.transactionFieldsServiceMock.Object,
                this.currentUserSecurityMock.Object,
                this.transactionAliasesProxyMock.Object,
                this.loggerMock.Object
            );
            //act
            var toParseTransaction = new Dictionary<string, string>();
            toParseTransaction.Add("InventoryDate", "2019-01-15T00:00:00.000+00:00");
            var transaction = new TransactionDO()
                              {
                                  LineItems = new List<LineItemDO>()
                                              {
                                                  new LineItemDO()
                                              }
                              };
            toBeTested.ApplyDictionaryToTransaction(transaction, toParseTransaction, this.transactionAliasClass);
            //assert
            Assert.AreEqual("01-15-2019", transaction.InventoryDate.ToString("MM-dd-yyyy"));
        }

        [TestMethod]
        public void ShouldAssignFromOwnerID()
        {
            //arrange
            var transactionAliasGuid = this.Setup();

            this.transactionFieldsServiceMock
                .Setup(x => x.GeTransactionFieldDefinitionsForUI(It.Is<TransactionAliasClass>(t => t.IdentityGuid == transactionAliasGuid)))
                .Returns(new List<TransactionAliasFieldClassWithColumn>() {
                  new TransactionAliasFieldClassWithColumn(){
                                                                Type = TransactionFieldType.Transaction,
                                                                ID = "FromOwnerID",
                                                                PropertyPath = "FromOwnerID"
                                                            }
              });

            var toBeTested = new TransactionObjectTranslationService(
                this.transactionFieldsServiceMock.Object,
                this.currentUserSecurityMock.Object,
                this.transactionAliasesProxyMock.Object,
                this.loggerMock.Object
            );
            //act
            var toParseTransaction = new Dictionary<string, string>();
            toParseTransaction.Add("FromOwnerID", "AAL - Delta");
            var transaction = new TransactionDO()
                              {
                              };
            toBeTested.ApplyDictionaryToTransaction(transaction, toParseTransaction, this.transactionAliasClass);
            //assert
            Assert.AreEqual("AAL - Delta", transaction.FromOwnerID);
        }


        [TestMethod]
        public void RetrievingDocumentNumberFromTransaction()
        {
            //arrange
            var transactionAliasGuid = this.Setup();

            this.transactionFieldsServiceMock
                .Setup(x => x.GeTransactionFieldDefinitionsForUI(transactionAliasGuid))
                .Returns(new List<TransactionAliasFieldClassWithColumn>() {
                    new TransactionAliasFieldClassWithColumn(){
                        Type = TransactionFieldType.Transaction,
                        ID = "DocumentNumber",
                        PropertyPath = "DocumentNumber"
                    }
                });

            var toBeTested = new TransactionObjectTranslationService(
                this.transactionFieldsServiceMock.Object,
                this.currentUserSecurityMock.Object,
                this.transactionAliasesProxyMock.Object,
                this.loggerMock.Object
                );
            //act
            var transactionToParse = new TransactionDO()
            {
                DocumentNumber = "2134",
                TransactionAliasGuid = transactionAliasGuid
            };
            var transaction = toBeTested.CreateTransactionFromDataObject(transactionToParse);
            //assert
            Assert.AreEqual("2134", transaction["DocumentNumber"]);
        }


        [TestMethod]
        public void RetrievingLineNumberVCFFromTransaction()
        {
            //arrange
            var transactionAliasGuid = this.Setup();

            this.transactionFieldsServiceMock
                .Setup(x => x.GeTransactionFieldDefinitionsForUI(transactionAliasGuid))
                .Returns(new List<TransactionAliasFieldClassWithColumn>() {
                    new TransactionAliasFieldClassWithColumn(){
                        Type = TransactionFieldType.LineItem,
                        ID = "Vcf",
                        PropertyPath = "Vcf"
                    }
                });

            var toBeTested = new TransactionObjectTranslationService(
                this.transactionFieldsServiceMock.Object,
                this.currentUserSecurityMock.Object,
                this.transactionAliasesProxyMock.Object,
                this.loggerMock.Object
                );
            //act
            var transactionToParse = new TransactionDO()
            {
                LineItems = new List<LineItemDO>()
                {
                    new LineItemDO()
                    {
                        VCF = 1.005
                    }
                },
                TransactionAliasGuid = transactionAliasGuid
            };
            var transaction = toBeTested.CreateTransactionFromDataObject(transactionToParse);
            //assert
            Assert.AreEqual((1.005d).ToString(), transaction["Vcf"]);
        }

        [TestMethod]
        public void RetrievingSourceRegistrationID1FromTransaction()
        {
            //arrange
            var transactionAliasGuid = this.Setup();

            this.transactionFieldsServiceMock
                .Setup(x => x.GeTransactionFieldDefinitionsForUI(transactionAliasGuid))
                .Returns(new List<TransactionAliasFieldClassWithColumn>() {
                    new TransactionAliasFieldClassWithColumn(){
                        Type = TransactionFieldType.Transaction,
                        ID = "SourceRegistrationID1",
                        PropertyPath = "SourceEQ1.RegistrationID"
                    }
                });

            var toBeTested = new TransactionObjectTranslationService(
                this.transactionFieldsServiceMock.Object,
                this.currentUserSecurityMock.Object,
                this.transactionAliasesProxyMock.Object,
                this.loggerMock.Object
                );
            //act
            var transactionToParse = new TransactionDO()
            {
                SourceEQ1 = new EquipmentDO()
                {
                    RegistrationID = "asdfasdfasdf"
                },
                TransactionAliasGuid = transactionAliasGuid
            };
            var transaction = toBeTested.CreateTransactionFromDataObject(transactionToParse);
            //assert
            Assert.AreEqual("asdfasdfasdf", transaction["SourceRegistrationID1"]);
        }

        [TestMethod]
        public void RetrievingRoutingIDFromTransaction()
        {
            //arrange
            var transactionAliasGuid = this.Setup();

            this.transactionFieldsServiceMock
                .Setup(x => x.GeTransactionFieldDefinitionsForUI(transactionAliasGuid))
                .Returns(new List<TransactionAliasFieldClassWithColumn>() {
                    new TransactionAliasFieldClassWithColumn(){
                        Type = TransactionFieldType.Transaction,
                        ID = "RoutingID",
                        PropertyPath = "RouteInfo.RoutingID"
                    }
                });

            var toBeTested = new TransactionObjectTranslationService(
                this.transactionFieldsServiceMock.Object,
                this.currentUserSecurityMock.Object,
                this.transactionAliasesProxyMock.Object,
                this.loggerMock.Object
                );
            //act
            var transactionToParse = new TransactionDO()
            {
                RouteInfo = new RouteInfoDO()
                {
                    RoutingID = "EverythingIsAwsome"
                },
                TransactionAliasGuid = transactionAliasGuid
            };
            var transaction = toBeTested.CreateTransactionFromDataObject(transactionToParse);
            //assert
            Assert.AreEqual("EverythingIsAwsome", transaction["RoutingID"]);
        }
    }
}
