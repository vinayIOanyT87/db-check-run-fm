using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using System.Xml;
using System.Reflection;

using FMBusinessObjects.Exceptions;
using FMBusinessObjects.DataObjects;

namespace FMBusinessServices.InternalClasses.EntityImportExport
{
    using global::EntityImportExport;

    public class WSExportObject : WorksheetBaseExport
    {
        EntityImportExportAttribute[] importExportAttributes;
        Hashtable memberHashTable;

        public string WorkSheetName = "";
        public WSExportObject(string wrkshtName)
           : base(wrkshtName)
        {
            WorkSheetName = wrkshtName;
        }

        #region Public override methods
        /// <summary>
        /// This method is the entry of creating the worksheet for the company.
        /// </summary>
        public override void CreateWorksheet(object obj)
        {
        }
        #endregion

        public bool IsEnumerable(object value)
        {
            // strings are enumerable but not interested in them
            if (typeof(string).IsInstanceOfType(value))
                return false;

            if (value as IEnumerable == null)
                return false;

            return true;
        }

        public object GetMemberValue(MemberInfo member, object o)
        {
            object value = null;

            if (member.MemberType == MemberTypes.Property)
            {
                PropertyInfo property = member as PropertyInfo;
                value = property.GetValue(o, null);
            }

            else if (member.MemberType == MemberTypes.Field)
            {
                FieldInfo field = member as FieldInfo;
                value = field.GetValue(o);
            }

            return value;
        }


        public EntityImportExportAttribute[] GetImportExportAttributes(Type objectType)
        {
            ArrayList attributes = new ArrayList();

            MemberInfo[] members = objectType.GetMembers();

            foreach (MemberInfo member in members)
            {
                if (member.MemberType != MemberTypes.Property
                && member.MemberType != MemberTypes.Field)
                    continue;

                EntityImportExportWorksheetAttribute[] worksheetAttributes = member.GetCustomAttributes(typeof(EntityImportExportWorksheetAttribute), false) as EntityImportExportWorksheetAttribute[];
                if (worksheetAttributes != null
                && worksheetAttributes.Length > 0)
                    continue;


                EntityImportExportAttribute[] importExportAttributes = member.GetCustomAttributes(typeof(EntityImportExportAttribute), false) as EntityImportExportAttribute[];
                if (importExportAttributes == null
                || importExportAttributes.Length == 0)
                    continue;

                attributes.Add(importExportAttributes[0]);
            }

            return attributes.ToArray(typeof(EntityImportExportAttribute)) as EntityImportExportAttribute[];
        }

        public void CreateHeaderandWidthData(
           string rootName,
           ref EntityImportExportAttribute rootAttribute,
           EntityImportExportAttribute[] importExportAttributes)
        {
            this.importExportAttributes = importExportAttributes;


            ArrayList ColumnNameArray = new ArrayList();
            ArrayList ColumnWidthArray = new ArrayList();

            if (rootAttribute == null)
            {
                foreach (EntityImportExportAttribute attribute in importExportAttributes)
                {
                    if (attribute.ColumnName == rootName)
                    {
                        rootAttribute = attribute;
                        break;
                    }
                }
            }
            else
            {
                ColumnNameArray.Add(rootAttribute.ColumnName);
                ColumnWidthArray.Add(rootAttribute.ColumnWidth);
            }

            foreach (EntityImportExportAttribute attribute in importExportAttributes)
            {
                ColumnNameArray.Add(attribute.ColumnName);
                ColumnWidthArray.Add(attribute.ColumnWidth);
            }

            if (ColumnNameArray.Count == 0 ||
               ColumnWidthArray.Count == 0 ||
               ColumnNameArray.Count != ColumnWidthArray.Count)
            {
                return;
            }

            CreateColumnWidth(ColumnWidthArray);
            CreateHeaderRow(ColumnNameArray);
        }

        public void CreateColumnWidth(ArrayList ColumnWidthArray)
        {
            for (int count = 0; count < ColumnWidthArray.Count; count++)
            {
                XmlNode columnNode = (XmlNode)base.tableNode.OwnerDocument.CreateNode(XmlNodeType.Element, "Column", null);
                XmlAttribute attribute = columnNode.OwnerDocument.CreateAttribute("ss", "Width", base.ExcelXmlUrl);
                attribute.Value = ColumnWidthArray[count].ToString();
                columnNode.Attributes.Append(attribute);
                base.tableNode.AppendChild(columnNode);
            }
        }

        public void CreateHeaderRow(ArrayList ColumnNameArray)
        {
            XmlNode cellNode;
            XmlNode dataNode;
            XmlAttribute attribute;

            XmlNode headerRow = (XmlNode)base.tableNode.OwnerDocument.CreateNode(XmlNodeType.Element, "Row", null);
            base.tableNode.AppendChild(headerRow);

            // Create for each header column name.
            for (int count = 0; count < ColumnNameArray.Count; count++)
            {
                cellNode = (XmlNode)headerRow.OwnerDocument.CreateNode(XmlNodeType.Element, "Cell", null);
                attribute = cellNode.OwnerDocument.CreateAttribute("ss", "StyleID", base.ExcelXmlUrl);
                attribute.Value = "s21";
                cellNode.Attributes.Append(attribute);

                dataNode = (XmlNode)cellNode.OwnerDocument.CreateNode(XmlNodeType.Element, "Data", null);
                attribute = dataNode.OwnerDocument.CreateAttribute("ss", "Type", base.ExcelXmlUrl);
                attribute.Value = "String";
                dataNode.Attributes.Append(attribute);
                dataNode.InnerText = ColumnNameArray[count].ToString();
                cellNode.AppendChild(dataNode);

                headerRow.AppendChild(cellNode);
            }
        }

        public void CreateMemberHashTable(Type objectType)
        {
            if (objectType != null)
            {
                memberHashTable = new Hashtable();

                MemberInfo[] members = objectType.GetMembers();

                foreach (EntityImportExportAttribute attribute in importExportAttributes)
                {
                    foreach (MemberInfo member in members)
                    {
                        if (member.MemberType != MemberTypes.Property
                        && member.MemberType != MemberTypes.Field)
                            continue;

                        EntityImportExportAttribute[] attributes = member.GetCustomAttributes(typeof(EntityImportExportAttribute), false) as EntityImportExportAttribute[];
                        if (attributes == null
                        || attributes.Length == 0)
                            continue;

                        if (attribute.MemberName.ToUpper() != attributes[0].MemberName.ToUpper())
                            continue;

                        memberHashTable.Add(attribute.MemberName.ToUpper(), member);
                        break;
                    }
                }
            }
        }

        public void CreaterRowData(
           EntityImportExportAttribute rootAttribute,
           ref string rootValue,
           object o)
        {
            XmlNode headerRow;
            XmlNode cellNode;
            XmlNode dataNode;
            XmlAttribute xmlAttribute;

            // Start a new row
            headerRow = (XmlNode)base.tableNode.OwnerDocument.CreateNode(XmlNodeType.Element, "Row", null);
            base.tableNode.AppendChild(headerRow);

            if (rootValue != null)
            {
                cellNode = (XmlNode)headerRow.OwnerDocument.CreateNode(XmlNodeType.Element, "Cell", null);
                dataNode = (XmlNode)cellNode.OwnerDocument.CreateNode(XmlNodeType.Element, "Data", null);
                xmlAttribute = dataNode.OwnerDocument.CreateAttribute("ss", "Type", base.ExcelXmlUrl);
                xmlAttribute.Value = "String";
                dataNode.Attributes.Append(xmlAttribute);
                dataNode.InnerText = rootValue as string;
                cellNode.AppendChild(dataNode);
                headerRow.AppendChild(cellNode);
            }

            foreach (EntityImportExportAttribute attribute in importExportAttributes)
            {
                MemberInfo member = null;

                if (memberHashTable == null)
                    CreateMemberHashTable(o.GetType());

                member = memberHashTable[attribute.MemberName.ToUpper()] as MemberInfo;
                if (member == null)
                    throw new Exception("Object Member not found - " + attribute.MemberName.ToUpper());

                object value = GetMemberValue(member, o);

                if (member.Name.ToUpper() == "SITEGUID")
                    value = base.GetSiteID((Guid)value);

                if (value == null)
                    value = "";
                else
                    value = value.ToString();

                if (value.Equals("{None}"))
                    value = "";

                if (rootValue == null
                && rootAttribute.ColumnName == attribute.ColumnName)
                {
                    rootValue = value as string;
                }

                cellNode = (XmlNode)headerRow.OwnerDocument.CreateNode(XmlNodeType.Element, "Cell", null);
                dataNode = (XmlNode)cellNode.OwnerDocument.CreateNode(XmlNodeType.Element, "Data", null);
                xmlAttribute = dataNode.OwnerDocument.CreateAttribute("ss", "Type", base.ExcelXmlUrl);
                xmlAttribute.Value = "String";
                dataNode.Attributes.Append(xmlAttribute);
                dataNode.InnerText = value as string;
                cellNode.AppendChild(dataNode);
                headerRow.AppendChild(cellNode);
            }
        }
    }
}