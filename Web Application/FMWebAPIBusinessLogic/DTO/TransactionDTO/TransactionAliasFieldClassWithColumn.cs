using FMBusinessObjects.DataObjects;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMWebAPIBusinessLogic.DTO.TransactionDTO
{

    public class TransactionAliasFieldClassWithColumn
    {
        public void Copy<T>(T baseClass)
        {
            try
            {
                //little bit of magic
                //https://stackoverflow.com/questions/14613919/copying-the-contents-of-a-base-class-from-a-derived-class
                var propsToCopy = baseClass.GetType()
                    .GetProperties()
                    .Where(x => x.CanRead && x.CanWrite);
                foreach (var property in propsToCopy)
                {
                    var thisProperty = typeof(TransactionAliasFieldClassWithColumn)
                        .GetProperties()
                        .FirstOrDefault(x => x.Name == property.Name);
                    if (thisProperty == null)
                    {
                        continue;
                    }
                    try
                    {
                        var value = property.GetValue(baseClass);
                        thisProperty.SetValue(this, value);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
            catch(Exception e)
            {
                throw;
            }
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public TransactionFieldType Type { get; set; }
        public string ID { get; set; }
        public bool ClearOnNew { get; set; }
        public bool DispatchField { get; set; }
        public bool VirtualField { get; set; }
        public bool FieldRequired { get; set; }
        public string DisplayName { get; set; }
        public string UserGroupId { get; set; }
        public int DisplayOrder { get; set; }
        public Guid IdentityGuid { get; set; }
        public string AliasName { get; set; }
        public Guid TransactionAliasGuid { get; set; }
        public string DbName { get; set; }
        public bool IsUserDataField { get; set; }
        public USER_DATA_TYPE UserDataType { get; set; }
        public TransactionAliasFieldExtendedAttributes ColumnDefinition { get; set; }

        /// <summary>
        /// Reversed engineered from the sql code
        /// </summary>
        public string PropertyPath { get; set; }
    }
}
