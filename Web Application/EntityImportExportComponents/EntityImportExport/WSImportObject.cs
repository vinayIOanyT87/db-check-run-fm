namespace EntityImportExport
{
    using System;
    using System.Collections;
    using System.Reflection;

    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.Exceptions;
    using System.Globalization;

    public class WSImportObject : WorksheetBaseImport
    {
        Hashtable memberHashTable;
        public string WorkSheetName = string.Empty;

        public WSImportObject(string wrkshtName)
           : base(wrkshtName)
        {
            WorkSheetName = wrkshtName;
        }

        public override void ParseWorksheet()
        {
        }

        public void CreateMemberHashTable(Type objectType)
        {
            if (objectType != null)
            {
                memberHashTable = new Hashtable();

                MemberInfo[] members = objectType.GetMembers();

                foreach (MemberInfo member in members)
                {
                    if (member.MemberType != MemberTypes.Property && member.MemberType != MemberTypes.Field)
                    {
                        continue;
                    }

                    var attributes = member.GetCustomAttributes(typeof(EntityImportExportAttribute), false) as EntityImportExportAttribute[];

                    if (attributes == null || attributes.Length == 0)
                    {
                        continue;
                    }


                    memberHashTable.Add(attributes[0].MemberName.ToUpper(), member);
                    break;
                }
            }
        }

        public string GetTranslatedValue(object obj, string xmlValue, string attributeName)
        {
            if (obj is CompanyRoleMapClass && attributeName == "ROLEID*")
            {
                var tempValue = xmlValue.ToUpper();
                switch (tempValue)
                {
                     case "OWN":
                        xmlValue = "OWNER";
                        break;
                    case "CONSUMER":
                    case "CON":
                        xmlValue = "SHIP TO";
                        break;
                    case "VENDOR":
                    case "VEN":
                        xmlValue = "CARRIER";
                        break;
                    case "SUP":
                        xmlValue = "SUPPLIER";
                        break;
                }
            }

            return xmlValue;
        }

        public void ImportExcelRow(Hashtable columnNamesAndDataList,
                     object obj,
                     EntityImportExportAttribute[] collectionImportExportAttributes)
        {
            foreach (EntityImportExportAttribute attribute in collectionImportExportAttributes)
            {
                if (memberHashTable == null)
                {
                    CreateMemberHashTable(obj.GetType(), collectionImportExportAttributes);
                }

                var hashTable = this.memberHashTable;

                if (hashTable != null)
                {
                    var member = hashTable[attribute.MemberName.ToUpper()] as MemberInfo;

                    if (member == null)
                    {
                        throw new Exception("Object Member not found - " + attribute.MemberName.ToUpper());
                    }

                    var xmlValue = (string)columnNamesAndDataList[attribute.XMLColumnName.ToUpper()];


                    if (xmlValue != null)
                    {
                        xmlValue = xmlValue.Trim();

                        xmlValue = GetTranslatedValue(obj, xmlValue, attribute.XMLColumnName.ToUpper());

                        if (member.Name.ToUpper() == "SITEGUID" || member.Name.ToUpper() == "SITE")
                        {
                            xmlValue = this.GetSiteGuid(xmlValue).ToString();
                        }

                        this.SetMemberValue(member, obj, xmlValue);
                    }
                    else
                    {
                        // Deleting value in excel spread sheet may remove cell from xml file if it the last in row.
                        // We set String.Empty so we dont get default value.
                        // e.g. company role defaults to "Manager" by seting sting empty we get no role set.
                        this.SetMemberValue(member, obj, String.Empty);
                    }
                }
            }
        }

        public void CreateMemberHashTable(Type objectType, EntityImportExportAttribute[] importExportAttributes)
        {
            if (objectType != null)
            {
                memberHashTable = new Hashtable();

                MemberInfo[] members = objectType.GetMembers();

                foreach (EntityImportExportAttribute attribute in importExportAttributes)
                {
                    foreach (MemberInfo member in members)
                    {
                        if (member.MemberType != MemberTypes.Property && member.MemberType != MemberTypes.Field)
                        {
                            continue;
                        }

                        var attributes = member.GetCustomAttributes(typeof(EntityImportExportAttribute), false) as EntityImportExportAttribute[];

                        if (attributes == null || attributes.Length == 0)
                        {
                            continue;
                        }

                        if (attribute.MemberName.ToUpper() != attributes[0].MemberName.ToUpper())
                        {
                            continue;
                        }

                        memberHashTable.Add(attribute.MemberName.ToUpper(), member);
                        break;
                    }
                }
            }
        }

        public string GetRootData(string rootID)
        {
            string returndata = "";

            // set the data from the rows into the collection
            if (this.recordRows != null && this.recordRows.Count > 0)
            {
                var columnNamesAndDataList = this.recordRows[0] as Hashtable;

                if (columnNamesAndDataList != null)
                {
                    returndata = (string)columnNamesAndDataList[rootID.ToUpper()];
                }
            }

            return returndata;
        }

        public void ImportExcelMemberData(Hashtable columnNamesAndDataList, MemberInfo member, EntityImportExportAttribute[] ImportExportAttributes, object o)
        {
            var xmlValue = (string)columnNamesAndDataList[ImportExportAttributes[0].XMLColumnName.ToUpper()];

            if (xmlValue != null)
            {
                if (member.Name.ToUpper() == "SITEGUID" || member.Name.ToUpper() == "SITE")
                {
                    xmlValue = GetSiteGuid(xmlValue).ToString();
                }

                SetMemberValue(member, o, xmlValue);
            }
        }

        public object GetMemberValue(MemberInfo member, object o)
        {
            object value = null;

            if (member.MemberType == MemberTypes.Property)
            {
                var property = member as PropertyInfo;

                if (property != null)
                {
                    value = property.GetValue(o, null);
                }
            }

            else if (member.MemberType == MemberTypes.Field)
            {
                var field = member as FieldInfo;

                if (field != null)
                {
                    value = field.GetValue(o);
                }
            }

            return value;
        }

        public void SetMemberValue(MemberInfo member, object o, string value)
        {
            try
            {
                if (member.MemberType == MemberTypes.Property)
                {
                    var property = member as PropertyInfo;

                    if (property != null && !property.CanWrite)
                    {
                        return;
                    }

                    if (property != null && property.PropertyType.BaseType == typeof(Enum))
                    {
                        // The error message we get when a enum value is not provided isn't the greatest, it says "Must specify valid information for parsing in the string"
                        // It would be great to use Enum.TryParse() here instead of try...catch but TryParse() doesn't seem to be compatible with a generic enum type - you must specify
                        // the type of enum, attempts to use object or Enum will result in a compiler error telling you it has to be a non-nullable value type.
                        try
                        {
                            property.SetValue(o, Enum.Parse(property.PropertyType, value, true), null);
                        }
                        catch (ArgumentException)
                        {
                            string message = property.Name + " - "
                                             + (string.IsNullOrWhiteSpace(value)
                                                ? "enumeration value cannot be blank"
                                                : "invalid enumeration value: " + value);

                            throw new Exception(message);
                        }
                    }
                    else if (property != null && property.PropertyType == typeof(Guid))
                    {
                        var otype = o.GetType();

                        if ((String.IsNullOrEmpty(value) && o.GetType().ToString() == "FMBusinessObjects.DataObjects.Point"
                             && member.Name == "PointGuid")
                            || (String.IsNullOrEmpty(value) && o.GetType().ToString() == "FMBusinessObjects.DataObjects.PointTemplate"
                                && member.Name == "PointTemplateGuid"))
                        {
                            property.SetValue(o, Guid.Parse("00000000-0000-0000-0000-000000000000"), null);
                        }
                        else if ((String.IsNullOrEmpty(value) && member.Name == "ApplicationStringGuid"))
                        {
                            property.SetValue(o, Guid.NewGuid(), null);
                        }
                        else
                        {
                            property.SetValue(o, new Guid(value), null);
                        }
                    }
                    else if (property != null && property.PropertyType == typeof(Date))
                    {
                        var dateTimeFormatInfo = new DateTimeFormatInfo();

                        if (this.Site != null)
                        {
                            dateTimeFormatInfo = this.Site.GetDateTimeFormatInfo();
                        }

                        var newDate = new Date { Value = DateTimeOffset.Parse(value, dateTimeFormatInfo) };
                        property.SetValue(o, newDate, null);
                    }
                    else if (property != null && property.PropertyType == typeof(DateAndTime))
                    {
                        var dateTimeFormatInfo = new DateTimeFormatInfo();

                        if (this.Site != null)
                        {
                            dateTimeFormatInfo = this.Site.GetDateTimeFormatInfo();
                        }

                        var newDateAndTime = new DateAndTime { Value = DateTimeOffset.Parse(value, dateTimeFormatInfo) };
                        property.SetValue(o, newDateAndTime, null);
                    }
                    else if (property != null && (property.PropertyType == typeof(DateTimeOffset) || Nullable.GetUnderlyingType(property.PropertyType) == typeof(DateTimeOffset)))
                    {
                        var dateTimeFormatInfo = new DateTimeFormatInfo();

                        if (this.Site != null)
                        {
                            dateTimeFormatInfo = this.Site.GetDateTimeFormatInfo();
                        }
                        if (!String.IsNullOrEmpty(value))
                            property.SetValue(o, DateTimeOffset.Parse(value, dateTimeFormatInfo), null);
                    }
                    else if (property != null && property.PropertyType == typeof(Boolean))
                    {
                        if (value == "0") property.SetValue(o, false, null);
                        else if (value == "1") property.SetValue(o, true, null);
                        else property.SetValue(o, Convert.ChangeType(value.ToUpper(), property.PropertyType), null);
                    }
                    else if (property != null && Nullable.GetUnderlyingType(property.PropertyType) != null)
                    {
                        if (Nullable.GetUnderlyingType(property.PropertyType) == typeof(Guid))
                        {
                            var safeValue = (string.IsNullOrEmpty(value)) ? null : (object)new Guid(value as string);
                            property.SetValue(o, safeValue, null);
                        }
                        else
                        {
                            var safeValue = (string.IsNullOrEmpty(value))
                               ? null
                               : Convert.ChangeType(value, Nullable.GetUnderlyingType(property.PropertyType));
                            property.SetValue(o, safeValue, null);
                        }
                    }
                    else if (property != null)
                    {
                        property.SetValue(o, Convert.ChangeType(value, property.PropertyType), null);                   
                    }
                }
                else if (member.MemberType == MemberTypes.Field)
                {
                    var field = member as FieldInfo;

                    if (field != null && field.FieldType.BaseType == typeof(Enum))
                    {
                        field.SetValue(o, Enum.Parse(field.FieldType, value, true));
                    }
                    else if (field != null && field.FieldType == typeof(Guid))
                    {
                        field.SetValue(o, new Guid(value));
                    }
                    else if (field != null && field.FieldType == typeof(Date))
                    {
                        var dateTimeFormatInfo = new DateTimeFormatInfo();

                        if (this.Site != null)
                        {
                            dateTimeFormatInfo = this.Site.GetDateTimeFormatInfo();
                        }

                        var newDate = new Date { Value = DateTimeOffset.Parse(value, dateTimeFormatInfo) };
                        field.SetValue(o, newDate);
                    }
                    else if (field != null && field.FieldType == typeof(DateAndTime))
                    {
                        var dateTimeFormatInfo = new DateTimeFormatInfo();

                        if (this.Site != null)
                        {
                            dateTimeFormatInfo = this.Site.GetDateTimeFormatInfo();
                        }

                        var newDateAndTime = new DateAndTime { Value = DateTimeOffset.Parse(value, dateTimeFormatInfo) };
                        field.SetValue(o, newDateAndTime);
                    }
                    else if (field != null && Nullable.GetUnderlyingType(field.FieldType) != null)
                    {
                        var safeValue = (string.IsNullOrEmpty(value)) ? null : Convert.ChangeType(value, field.FieldType);
                        field.SetValue(o, safeValue);
                    }
                    else if (field != null)
                    {
                        field.SetValue(o, Convert.ChangeType(value, field.FieldType));
                    }
                }
            }
            catch (Exception e)
            {
                while (e.InnerException != null)
                {
                    e = e.InnerException;
                }
                if (e is CompanyRoleMapCollectionException)
                {
                    throw e;
                }
                throw new Exception(e.Message + " Invalid Cell Value: " + value + ". ");
            }
        }


    }
}
