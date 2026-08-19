using FMBusinessObjects.DataObjects;
using FMWebAPIBusinessLogic.Interfaces.FMBusinessLogic.TransactionPipeline;
using FMWebAPIBusinessLogic.Interfaces.FMProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMWebAPIBusinessLogic.Services.FMBusinessLogic.TransactionPipeline.Inbound
{
    public class ProductGuidResolver : IPipelineCommand
    {
        private readonly IProductsProxy _productsProxy;
        public ProductGuidResolver(IProductsProxy productsProxy)
        {
            _productsProxy = productsProxy;
        }
        public void Execute(TransactionDO trxDO, TransactionAliasClass trxAlias)
        {
            var products = _productsProxy.Enumerate();
            foreach (LineItemDO lineItem in trxDO.LineItems)
            {
                if (!string.IsNullOrEmpty(lineItem.Product))
                {
                    var foundProduct = products.Find(objP => objP.ID == lineItem.Product);
                    lineItem.ProductGuid = foundProduct.MasterRecordGuid;
                }
            }
        }
    }
}
