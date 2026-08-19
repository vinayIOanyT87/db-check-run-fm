// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FieldConfiguration.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   The purpose of this module is to read an XML document for field lengths.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TransactionFields
{
	using System;
	using System.Configuration;
	using System.Linq;
	using System.Web.UI;
	using System.Xml.Linq;

	using FMBusinessObjects.LogClient;

	/// <summary>
	/// The field configuration.
	/// </summary>
	public class FieldConfiguration
	{
		#region Private data members
		private bool fileFound;
		private string configFileName;
		private Page page;
		private XDocument txConfigurationXML;
		private Logger logger;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="FieldConfiguration"/> class.
		/// </summary>
		/// <param name="page">
		/// The page.
		/// </param>
		public FieldConfiguration(Page page)
		{
			this.Init(page);
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets a value indicating whether file found. This property returns true if the configuration file was found.
		/// Otherwise, it returns false.
		/// </summary>
		public bool FileFound
		{
			get { return this.fileFound; }
		}
		#endregion

		#region Public methods
		/// <summary>
		/// This method will load the transaction field configuration data.
		/// </summary>
		public void LoadConfigurationData( )
		{
			if ( this.page == null )
			{
				this.fileFound = false;
				this.logger.Debug("FieldConfiguration.LoadConfigurationData(); No Page object.");
			}
			else
			{
				bool fileNameFound = this.ReadFieldConfigFileName( );

				if ( fileNameFound )
				{
					try
					{
						this.txConfigurationXML = XDocument.Load(this.page.Server.MapPath(this.configFileName));

						if ( this.txConfigurationXML != null )
						{
							this.fileFound = true;
						}
					}
					catch ( Exception ex )
					{
						this.logger.Error("FieldConfiguration.LoadConfigurationData(); Error reading transaction field Config file. " +
										  ex.Message);
						this.fileFound = false;
					}
				}
			}
		}

		/// <summary>
		/// This method will return the field length based on the field name and
		/// transaction alias name. If the transaction alias name is empty, then "ALL"
		/// is used. If the field cannot be found or the length cannot be converted
		/// to integer, then a -1 is returned.
		/// </summary>
		/// <param name="inFieldName">
		/// The field name.
		/// </param>
		/// <param name="txAlias">
		/// The transaction alias.
		/// </param>
		/// <returns>
		/// The <see cref="int"/>.
		/// </returns>
		public int GetFieldLength(string inFieldName, string txAlias)
		{
			int length = -1;
			string aliasName = "ALL";

			if ( string.IsNullOrEmpty(inFieldName) )
			{
				return length;
			}

			if ( !string.IsNullOrEmpty(txAlias) )
			{
				aliasName = txAlias;
			}

			// Remove unwanted characters from the name.
			string fieldName = this.FormatFieldName(inFieldName);

			// Check for a cached value first
			var cachedLength = AppDomain.CurrentDomain.GetData(aliasName + "/ml/" + fieldName) as int?;
			if (cachedLength != null)
			{
				length = cachedLength.Value;
			}
			else
			{
				var txFieldLengths = from fieldLength in this.txConfigurationXML.Descendants("FieldLength").DefaultIfEmpty(null)
					where
						fieldLength.Attribute("fieldName") != null && fieldLength.Attribute("fieldName").Value == fieldName
						&& fieldLength.Attribute("TxAlias") != null && fieldLength.Attribute("TxAlias").Value == aliasName
					select new { length = fieldLength.Attribute("length").Value };

				try
				{
					var fieldLength = txFieldLengths.FirstOrDefault();

					if (fieldLength != null)
					{
						length = Convert.ToInt32(fieldLength.length);
					}

					AppDomain.CurrentDomain.SetData(aliasName + "/ml/" + fieldName, length);
				}
				catch (Exception)
				{
					length = -1;
					this.logger.Error("FieldConfiguration.GetFieldLength(); Invalid field length for field " + fieldName);
				}
			}

			return length;
		}

		/// <summary>
		/// This method will return true if the field is explicitly exempted from a glossary link.
		/// </summary>
		/// <param name="inFieldName">
		/// The field name.
		/// </param>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool IsFieldExemptedFromGlossary(string inFieldName)
		{
			string fieldName = this.FormatFieldName(inFieldName);

			var txExemptFields = from exemptField in this.txConfigurationXML.Descendants("GlossaryExemption").DefaultIfEmpty(null)
								 where exemptField.Attribute("fieldName") != null
								 && exemptField.Attribute("fieldName").Value == fieldName
								 select new
								 {
									 fieldExempt = exemptField.Attribute("Exempt").Value
								 };

			bool exemptValue = false;

			try
			{
				var txExemptField = txExemptFields.FirstOrDefault( );
				if ( txExemptField != null )
				{
					exemptValue = Convert.ToBoolean(txExemptField.fieldExempt);
				}
			}
			catch ( Exception )
			{
				this.logger.Debug("FieldConfiguration.IsFieldExemptedFromGlossary(); Error reading exemption entry for: " + fieldName);
			}

			return exemptValue;
		}

		/// <summary>
		/// This method will return whether a field is conditional based on the field name and
		/// transaction alias name. If the transaction alias name or field name cannot be found,
		/// then false is returned. Otherwise, true is returned.
		/// </summary>
		/// <param name="inFieldName">
		/// The field name.
		/// </param>
		/// <param name="txAlias">
		/// The transaction alias.
		/// </param>
		/// <returns>
		/// The <see cref="bool?"/>.
		/// </returns>
		public bool? IsFieldRequiredByExternalInterface(string inFieldName, string txAlias)
		{
			bool? fieldConditional = null;

			if ( string.IsNullOrEmpty(inFieldName) || (string.IsNullOrEmpty(txAlias)) )
			{
				return null;
			}

			// Remove unwanted characters from the name.
			string fieldName = this.FormatFieldName(inFieldName);

			var txConditionalField = from conditionalField in this.txConfigurationXML.Descendants("RequiredField").DefaultIfEmpty(null)
									 where conditionalField.Attribute("fieldName") != null
										   && conditionalField.Attribute("fieldName").Value == fieldName
										   && conditionalField.Attribute("TxAlias") != null
										   && conditionalField.Attribute("TxAlias").Value == txAlias
									 select new
									 {
										 fieldConditional = conditionalField.Attribute("Conditional").Value
									 };

			try
			{
				var condField = txConditionalField.FirstOrDefault( );
				if ( condField != null )
				{
					fieldConditional = Convert.ToBoolean(condField.fieldConditional);
				}
			}
			catch ( Exception )
			{
				fieldConditional = null;
				this.logger.Debug("FieldConfiguration.IsFieldConditionallyRequiredByExternalInterface(); No field named: " + fieldName);
			}

			return fieldConditional;
		}

		/// <summary>
		/// This method will return whether an alias has conditional fields based on the 
		/// transaction alias name. If the transaction alias name,
		/// then false is returned. Otherwise, true is returned.
		/// </summary>
		/// <param name="txAlias">
		/// The tx alias.
		/// </param>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool DoesAliasHaveConditionalFields(string txAlias)
		{
			bool hasConditionalFields = false;

			if ( string.IsNullOrEmpty(txAlias) )
			{
				return false;
			}

			var txConditionalField = from conditionalField in this.txConfigurationXML.Descendants("RequiredField").DefaultIfEmpty(null)
									 where conditionalField.Attribute("TxAlias") != null
										   && conditionalField.Attribute("TxAlias").Value == txAlias
									 select new
									 {
										 fieldConditional = conditionalField.Attribute("Conditional").Value
									 };

			try
			{
				if (txConditionalField.FirstOrDefault() != null)
				{
					string conditional = txConditionalField.FirstOrDefault().fieldConditional;

					if (conditional.ToUpper().Equals("TRUE"))
					{
						hasConditionalFields = true;
					}
				}

			}
			catch ( Exception )
			{
				hasConditionalFields = false;
				this.logger.Debug("FieldConfiguration.DoesAliasHaveConditionalFields(); No conditional field for transaction: " + txAlias);
			}

			return hasConditionalFields;
		}

		#endregion

		#region Private methods
		/// <summary>
		/// This method will remove the Transaction Alias and spaces from the 
		/// field name. It will return the formatted name or return the original
		/// name if the strings cannot be found.
		/// </summary>
		/// <param name="fieldName">
		/// The field name.
		/// </param>
		/// <returns>
		/// The <see cref="string"/>.
		/// </returns>
		private string FormatFieldName(string fieldName)
		{
			string outName				= fieldName;
			int indexLineItemUserData	= fieldName.IndexOf("TALUD");
			int indexUserData			= fieldName.IndexOf("TAUD");

			if ( indexLineItemUserData != -1 )
			{
				outName = fieldName.Substring(indexLineItemUserData);
				outName = outName.Replace(" ", string.Empty);
			}
			else if ( indexUserData != -1 )
			{
				outName = fieldName.Substring(indexUserData);
				outName = outName.Replace(" ", string.Empty);
			}

			return outName;
		}

		/// <summary>
		/// This method initializes the object to its initial state.
		/// </summary>
		/// <param name="inPage">
		/// The page.
		/// </param>
		private void Init(Page inPage)
		{
			this.configFileName		= string.Empty;
			this.page				= inPage;
			this.txConfigurationXML = null;
			this.fileFound			= false;
			this.logger				= new Logger("Accounting");
		}

		/// <summary>
		/// This method reads the application settings for the transaction field configuration
		/// file name.
		/// </summary>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		private bool ReadFieldConfigFileName( )
		{
			bool fileNameFound = false;

			if ( ConfigurationManager.AppSettings["TransactionFieldConfiguration"] != null )
			{
				this.configFileName = ConfigurationManager.AppSettings["TransactionFieldConfiguration"];

				if ( !string.IsNullOrEmpty(this.configFileName) )
				{
					fileNameFound = true;
				}
			}
			else
			{
				this.logger.Debug("FieldConfiguration.ReadFieldConfigFileName(); Tx field configuration name not found");
			}

			return fileNameFound;
		}
		#endregion
	}
}
