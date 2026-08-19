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
	public class WSImportObject : WorksheetBaseImport
	{
		Hashtable memberHashTable;
		public string WorkSheetName = "";
		public WSImportObject(string wrkshtName)
			: base(wrkshtName)
		{
			WorkSheetName = wrkshtName;
		}

		public override void ParseWorksheet()
		{
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


		public void ImportExcelRow(Hashtable columnNamesAndDataList,
		   object o,
		   EntityImportExportAttribute[] collectionImportExportAttributes)
		{
			foreach (EntityImportExportAttribute attribute in collectionImportExportAttributes)
			{
				MemberInfo member = null;

				if (memberHashTable == null)
					CreateMemberHashTable(o.GetType(), collectionImportExportAttributes);

				member = memberHashTable[attribute.MemberName.ToUpper()] as MemberInfo;
				if (member == null)
					throw new Exception("Object Member not found - " + attribute.MemberName.ToUpper());

				string xmlValue = (string)columnNamesAndDataList[attribute.XMLColumnName.ToUpper()];

				if (xmlValue != null)
				{
					if (member.Name.ToUpper() == "SITEGUID")
					{
						xmlValue = GetSiteGuid(xmlValue).ToString();
					}

					SetMemberValue(member, o, xmlValue);
				}
			}
		}


		public string GetRootData(string rootID)
		{
			string returndata = "";

			// set the data from the rows into the collection
			if (base.recordRows != null
			&& base.recordRows.Count > 0)
			{
				Hashtable columnNamesAndDataList = base.recordRows[0] as Hashtable;
				returndata = (string)columnNamesAndDataList[rootID.ToUpper()];
			}

			return returndata;
		}

		public void ImportExcelMemberData(Hashtable columnNamesAndDataList, MemberInfo member, EntityImportExportAttribute[] ImportExportAttributes, object o)
		{
			string xmlValue = (string)columnNamesAndDataList[ImportExportAttributes[0].XMLColumnName.ToUpper()];

			if (xmlValue != null)
			{
				if (member.Name.ToUpper() == "SITEGUID")
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


		public void SetMemberValue(MemberInfo member, object o, string value)
		{

			if (member.MemberType == MemberTypes.Property)
			{
				PropertyInfo property = member as PropertyInfo;

				if (!property.CanWrite)
				{
					return;
				}

				if (property.PropertyType.BaseType == typeof(Enum))
					property.SetValue(o, Enum.Parse(property.PropertyType, value, true), null);
				else if (property.PropertyType == typeof(Guid))
				{
					property.SetValue(o, new Guid(value.ToString()), null);
				}
				else if (property.PropertyType == typeof(Date))
				{
					Date newDate = new Date();
					newDate.Value = DateTimeOffset.Parse(value);
					property.SetValue(o, newDate, null);
				}
				else if (property.PropertyType == typeof(DateAndTime))
				{
					DateAndTime newDateAndTime = new DateAndTime();
					newDateAndTime.Value = DateTimeOffset.Parse(value);
					property.SetValue(o, newDateAndTime, null);
				}
				else
					property.SetValue(o, Convert.ChangeType(value, property.PropertyType), null);

			}

			else if (member.MemberType == MemberTypes.Field)
			{
				FieldInfo field = member as FieldInfo;
				if (field.FieldType.BaseType == typeof(Enum))
					field.SetValue(o, Enum.Parse(field.FieldType, value, true));
				else if (field.FieldType == typeof(Guid))
				{
					field.SetValue(o, new Guid(value.ToString()));
				}
				else if (field.FieldType == typeof(Date))
				{
					Date newDate = new Date();
					newDate.Value = DateTimeOffset.Parse(value);
					field.SetValue(o, newDate);
				}
				else if (field.FieldType == typeof(DateAndTime))
				{
					DateAndTime newDateAndTime = new DateAndTime();
					newDateAndTime.Value = DateTimeOffset.Parse(value);
					field.SetValue(o, newDateAndTime);
				}
				else
					field.SetValue(o, Convert.ChangeType(value, field.FieldType));
			}

		}

	}
}