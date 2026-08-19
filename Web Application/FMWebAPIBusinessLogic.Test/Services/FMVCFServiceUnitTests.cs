using FMBusinessObjects.DataObjects;
using FMCore.Interfaces;
using FMWebAPIBusinessLogic.Interfaces.FMBusinessLogic;
using FMWebAPIBusinessLogic.Interfaces.FMProxy;
using FMWebAPIBusinessLogic.Services.FMBusinessLogic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Varec.CommonComponents.EngineeringUnitsLibrary;
using Varec.CommonComponents.VolumeCorrection;

namespace FMWebAPIBusinessLogic.Test.Services
{
    [TestClass]
    public class FMVCFServiceUnitTests
    {
        Mock<IFMCustomLogger> fmCustomLoggerMock;
        Mock<IProductsProxy> productProxyMock;
        Mock<ICurrentRequestContext> currentRequestContextMock;
        private void AviationDefaultsSetup()
        {
            this.fmCustomLoggerMock = new Mock<IFMCustomLogger>();
            this.productProxyMock = new Mock<IProductsProxy>();
            this.currentRequestContextMock = new Mock<ICurrentRequestContext>();

            var userGuid = Guid.NewGuid();
            this.currentRequestContextMock.Setup(x => x.GetCurrentSecurityContext())
                .Returns(new SecurityClass()
                {
                    IdentityGuid = userGuid
                });
            this.currentRequestContextMock.Setup(x => x.GetCurrentSite()).Returns(
                new SiteClass()
                {
                    TemperatureUnits = EngineeringUnit.FmtDegF,
                    PressureUnits = EngineeringUnit.FmpPsi,
                    DensityUnits = EngineeringUnit.FmdDegApi
                });
            this.productProxyMock.Setup(x=> x.Enumerate(false))
                .Returns(new ProductCollectionClass()
                {
                    new ProductClass()
                    {
                        ID = "JA",
                        _VcfModuleSettings = new FMBusinessObjects.DataObjects.VcfModuleSettings()
                                             {
                                                 CorrectionMethodType = ECorrectionTypeMajor.CORR_API_F,
                                                 CorrectionMethodSpecific = ECorrectionTypeMinor.CORR_API6A,
                                             }
                    }
                });
            /*
            this.productProxyMock.Setup(x => x.Enumerate(false))
                .Returns(new ProductCollectionClass()   //bds
                {
                    new ProductClass()
                    {
                        ID="JA",
                        _VcfModuleSettings.CorrectionMethodType = ECorrectionTypeMajor.CORR_API_F
                        //_MajorCorrectionMethod = ECorrectionTypeMajor.CORR_API_F,
                        //_MinorCorrectionMethod = ECorrectionTypeMinor.CORR_API6A,
                        //_StandardTemperature = new SIDouble(EngineeringUnit.FmtDegF, null, 15.5555555555556),
                        //_AlternateTemperature = new SIDouble(EngineeringUnit.FmtDegF, null, 15.5555555555556),
                        //_AlternatePressure = new SIDouble(EngineeringUnit.FmpPsi, null, 0.0)
                    }
                });
                */
        }

        [TestMethod]
        public void VcfTest1()
        {
            //arrange
            this.AviationDefaultsSetup();

            var toTest = new FMVCFService(
                this.fmCustomLoggerMock.Object,
                this.productProxyMock.Object, 
                this.currentRequestContextMock.Object);
            //act
            var calculatedVcf = toTest.GetVCFForProductBasedOnUserForAviation("JA", 20, 20);
            //assert
            var expectedVcf = 1.0156d;
            Assert.AreEqual(expectedVcf, calculatedVcf, .000005);
        }

        [TestMethod]
        public void VcfTest2()
        {
            //arrange
            this.AviationDefaultsSetup();

            var toTest = new FMVCFService(
                this.fmCustomLoggerMock.Object,
                this.productProxyMock.Object, 
                this.currentRequestContextMock.Object);
            //act
            var calculatedVcf = toTest.GetVCFForProductBasedOnUserForAviation("JA", 100, 53.78);
            //assert
            var expectedVcf = 0.9764d;
            Assert.AreEqual(expectedVcf, calculatedVcf, .000005);
        }
    }
}
