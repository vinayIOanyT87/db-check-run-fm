using Microsoft.VisualStudio.TestTools.UnitTesting;
using FMWebAPIBusinessLogic.Interfaces.FMProxy;
using Moq;
using System;

namespace FMWebAPIBusinessLogic.Test.Services.TransactionPipeline.Inbound
{
    using System.Collections.Generic;
    using System.Linq;

    using FMBusinessObjects.DataObjects;

    using FMWebAPIBusinessLogic.Services.FMBusinessLogic.TransactionPipeline.Inbound;

    [TestClass]
    public class ProductGuidResolverUnitTests
    {
        [TestMethod]
        public void SetsGuidCorrectly()
        {
            //arrange
            var productProxyMock = new Mock<IProductsProxy>();
            var productGuid = Guid.NewGuid();
            productProxyMock
                .Setup(x => x.Enumerate(It.IsAny<bool>()))
                .Returns(new ProductCollectionClass()
                    { new ProductClass() { ID = "MyFakeLiquid", IdentityGuid = productGuid, MasterRecordGuid = productGuid} });
            //act
            var toTest = new ProductGuidResolver(productProxyMock.Object);
            var productGuidShouldBeResolved = 
                new TransactionDO(){ LineItems = new List<LineItemDO>()
                 { new LineItemDO() { Product = "MyFakeLiquid" } }};
            toTest.Execute(productGuidShouldBeResolved, new TransactionAliasClass());
            //assert
            Assert.AreEqual(productGuid, productGuidShouldBeResolved.LineItems.First().ProductGuid);
        }
    }
}
