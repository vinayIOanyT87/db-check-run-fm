// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataDictionaryForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the DataDictionaryForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using System.Web;
	using System.Web.UI;
	using System.Web.UI.WebControls;
	using System.Xml;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Interfaces;

	using FMControls;

	/// <summary>
	///    Code behind for DataDictionaryForm.
	/// </summary>
	public partial class DataDictionaryForm : FMAutoSubmitFormBase, IEntityDiscovery, IMenuDiscovery
	{
		#region Constants and Fields

		protected Image Image1;

		protected int PriorEditItemIndex = -2;

		private const string DICTIONARY_FIND_STRING = "DictionaryFindString";

		private bool DictionaryAssigned;

		private string searchString;

		#endregion

		#region Public Properties

		public string SearchString
		{
			get
			{
				return this.searchString;
			}
			set
			{
				this.searchString = value;
			}
		}

		#endregion

		#region Explicit Interface Properties

		bool IEntityDiscovery.EntityAssignable
		{
			get
			{
				return true;
			}
		}

		Type IEntityDiscovery.EntityEngineType
		{
			get
			{
				return typeof(IDataDictionariesClass);
			}
		}

		ENTITY_TYPE IEntityDiscovery.EntityType
		{
			get
			{
				return ENTITY_TYPE.DATA_DICTIONARY;
			}
		}

		#endregion

		#region Public Methods and Operators

		public static string ConvertFromSpecialCharacter(string Key)
		{
			string HTMLCompatibleKey = Key.Replace("&", "&amp;");
			HTMLCompatibleKey = HTMLCompatibleKey.Replace("\"", "&quot;");
			HTMLCompatibleKey = HTMLCompatibleKey.Replace("<", "&lt;");
			HTMLCompatibleKey = HTMLCompatibleKey.Replace(">", "&gt;");
			return HTMLCompatibleKey;
		}

		/// <summary>
		///    This method will convert the HTML special character string to a
		///    regular character.
		/// </summary>
		/// <param name="inStr"></param>
		/// <returns></returns>
		public static string ConvertToSpecialCharacter(string inStr)
		{
			string outStr = inStr.Replace("&AMP;", "&");
			outStr = outStr.Replace("&QUOT;", "\"");
			outStr = outStr.Replace("&LT;", "<");
			outStr = outStr.Replace("&GT;", ">");

			return outStr;
		}

		public Dictionary<string, string> GetDataDictionaryDataset(char DictionaryFilter)
		{
			var dataDictionaryTable = new Dictionary<string, string>(5000);

			string strAssemblyList =
				FMChannelHelper.MakeCall<IConfigurationSettings, string>(
					x => x.GetKeyValueByKey(base.Security, ConfigurationSettingDOClass.Key_DataDictionaryAssemblies));

			if (string.IsNullOrEmpty(strAssemblyList) == false)
			{
				char[] separator = { ';' };
				string[] dataDictionList = strAssemblyList.Split(separator, StringSplitOptions.RemoveEmptyEntries);

				if (dataDictionList.Length > 0)
				{
					string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

					//foreach(Object AssemblyName in (Array)IDataDictionaryAssemblies)
					for (int nextAssembly = 0; nextAssembly < dataDictionList.Length; nextAssembly++)
					{
						string assemblyName = dataDictionList[nextAssembly];
						Assembly DLL = null;

						if (!AssemblyDictionary.ContainsKey(assemblyName.ToLower()))
						{
							try
							{
								DLL = Assembly.LoadFrom(baseDirectory + "\\bin\\" + assemblyName);
							}
							catch
							{
								try
								{
									DLL = Assembly.Load(assemblyName);
								}
								catch(Exception ex)
								{
									string message = "Assembly Load Error on Data Dictionary Form. " + ex.Message;
									FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(message, FMEventLogEntryType.Warning));
								}
							}

							if (DLL != null)
								AssemblyDictionary.Add(assemblyName.ToLower(), DLL);
						}
						else
						{
							DLL = AssemblyDictionary.Get(assemblyName.ToLower());
						}

						if (DLL == null)
							continue;

						this.GetAssemblyKeys(DictionaryFilter, dataDictionaryTable, DLL);
					}
				}
			}

			dataDictionaryTable = FMChannelHelper.MakeCall<IDataDictionariesClass, Dictionary<string, string>>(
				x => x.TranslateKeyPairTable(this.Security.SiteGuid, dataDictionaryTable));

			return dataDictionaryTable;
		}

		/// <summary>
		///    Gets a list of menu items that should be displayed for the current user.
		/// </summary>
		/// <param name="security">The security object of the current session</param>
		/// <param name="siteGroup">Whether the current logged-in site is a site group</param>
		/// <param name="options">Hardware key options</param>
		/// <returns>
		///    List of menu items to be displayed
		/// </returns>
		public List<FMMenuItem> GetMenuItems(SecurityClass security, bool siteGroup, ushort word1, ushort word2, ushort useNewLicenseKey, uint options)
		{
			if (useNewLicenseKey == 1)
			{

			}
			else
			{
				// Depends Upon Shared Components Config
				if ((options & 0x4000) == 0)
				{
					return null;
				}
			}

			if (!security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS) && !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			{
				return null;
			}

			var menuItems = new List<FMMenuItem>();

			var menuItem = new FMMenuItem
			{
				MenuItemType = FMMenuItemType.ADMIN_SYSTEM_DATA_DICTIONARY,
				RootMenuName = "Administration",
				CategoryName = "System",
				ItemName = "Data Dictionary",
				NavigateUrl = "DataDictionaryForm.aspx",
				ApplyDataDictionary = ApplyDataDictionary.Apply
			};

			menuItems.Add(menuItem);

			return menuItems;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender">sender</param>
		/// <param name="e">event args</param>
		protected void DataDictionaryChanged(object sender, EventArgs e)
		{
			this.dataDictionaryChangedField.Value = "true";
		}

		#endregion

		#region Explicit Interface Methods

		EntityToSiteMapCollectionClass IEntityDiscovery.EnumerateEntityMaps(
			SecurityClass Security, ENTITY_ASSIGNMENT_TYPE Type)
		{
			var EntityToSiteMapCollection = new EntityToSiteMapCollectionClass();

			if (Type == ENTITY_ASSIGNMENT_TYPE.OWNED)
			{
			}
			else
			{
				EntityToSiteMapClass EntityToSiteMap =
					FMChannelHelper.MakeCall<IEntityToSiteMaps, EntityToSiteMapClass>(
						x => x.Get(Security, ((IEntityDiscovery)this).EntityType, Security.LoginSiteGuid));

				if (Type == ENTITY_ASSIGNMENT_TYPE.ASSIGNED)
				{
					if (Security.LoginSiteGuid == EntityToSiteMap.IdentityGuid)
					{
						EntityToSiteMap.ID = "All Key/Value Pairs";
						EntityToSiteMapCollection.Add(EntityToSiteMap);
					}
				}
				else
				{
					if (EntityToSiteMap.IdentityGuid == Guid.Empty)
					{
						EntityToSiteMap = new EntityToSiteMapClass();
						EntityToSiteMap.SiteGuid = Guid.Empty;
						EntityToSiteMap.ID = "All Key/Value Pairs";
						EntityToSiteMap.TypeID = ((IEntityDiscovery)this).EntityType;
						EntityToSiteMap.IdentityGuid = Security.SiteGuid;
						EntityToSiteMapCollection.Add(EntityToSiteMap);
					}
				}
			}
			return EntityToSiteMapCollection;
		}

		Guid IEntityDiscovery.GetIdentityGuid(SecurityClass security, string ID)
		{
			EntityToSiteMapClass EntityToSiteMap = FMChannelHelper.MakeCall<IEntityToSiteMaps, EntityToSiteMapClass>(
																	 x =>
																	 x.Get(security, ((IEntityDiscovery)this).EntityType, security.LoginSiteGuid)
																);

			return (EntityToSiteMap.IdentityGuid == Guid.Empty) ? security.SiteGuid : EntityToSiteMap.IdentityGuid;
		}

		void IEntityDiscovery.SetSiteGuid(SecurityClass security, Guid guid, Guid SiteGuid)
		{
			Guid OriginalSiteGuid = security.SiteGuid;

			string lastKey = string.Empty;

			try
			{
				// Need to purge any DataDictionary Assignments
				var DataDictionary = new DataDictionaryClass();
				EntityToSiteMapCollectionClass EntityToSiteMapCollection = FMChannelHelper.MakeCall<IEntityToSiteMaps, EntityToSiteMapCollectionClass>(
																	 x =>
																	 x.EnumerateByTypeIDAndGuid(security, DataDictionary.EntityType, OriginalSiteGuid)
																);

				foreach (EntityToSiteMapClass EntityToSiteMap in EntityToSiteMapCollection)
				{
					this.PurgeEntityToSiteMaps(security, EntityToSiteMap);
				}

				DataDictionaryCollectionClass DataDictionaryCollection = FMChannelHelper.MakeCall<IDataDictionariesClass, DataDictionaryCollectionClass>(
																	 x =>
																	 x.EnumerateBySite(security)
																);

				foreach (string Key in DataDictionaryCollection.Keys)
				{
					lastKey = Key;
					this.PurgeDictionaryValueByKey(security, Key);
					security.SiteGuid = SiteGuid;
					DataDictionary.Key = Key;
					DataDictionary.Value = DataDictionaryCollection[Key];
					this.AddDictionaryValueByKey(security, DataDictionary);
					security.SiteGuid = OriginalSiteGuid;
				}

				// Reomve any LinkKey strings from session
				for (int ItemIndex = 0; ItemIndex < this.Session.Keys.Count; ItemIndex++)
				{
					if (this.Session.Keys[ItemIndex].StartsWith(FMLinkButton.LINK_KEY))
					{
						this.Session.Remove(this.Session.Keys[ItemIndex]);
						ItemIndex--;
					}
				}
			}
			catch (Exception daExcept)
			{
				string errMsg = daExcept.Message;
				errMsg = errMsg.Replace("exists.", "exists (" + lastKey + ").");
				throw new Exception(errMsg);
			}
			finally
			{
				security.SiteGuid = OriginalSiteGuid;
			}
		}

		private void AddDictionaryValueByKey(SecurityClass security, DataDictionaryClass dataDictionary)
		{
			FMChannelHelper.MakeCall<IDataDictionariesClass>(
																	 x =>
																	 x.Add(security, dataDictionary)
																);
		}

		private void PurgeDictionaryValueByKey(SecurityClass security, string key)
		{
			FMChannelHelper.MakeCall<IDataDictionariesClass>(
																	 x =>
																	 x.Purge(security, key)
																);
		}

		private void PurgeEntityToSiteMaps(SecurityClass security, EntityToSiteMapClass EntityToSiteMap)
		{
			FMChannelHelper.MakeCall<IEntityToSiteMaps>(
													x =>
																	 x.Purge(security, EntityToSiteMap)
												);
		}

		#endregion

		#region Methods

		/// <summary>
		///    This method is called when the find button is pressed. It will retrieve data from the find
		///    text box and set the search string. If there is no data, then the search string is set to null.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">
		///    The <see cref="System.EventArgs" /> instance containing the event data.
		/// </param>
		protected void FindButtonOnClick(object sender, EventArgs e)
		{
			if ((this.FindTextBox == null) || (this.FindTextBox.Text.Length < 1))
			{
				this.searchString = null;
				this.Session.Remove(DICTIONARY_FIND_STRING);
			}
			else
			{
				this.searchString = this.FindTextBox.Text.ToUpper();
				this.FindTextBox.Text = this.searchString;
				this.Session.Add(DICTIONARY_FIND_STRING, this.searchString);
			}

			// Update the page with the new contents.
			this.DataDictionaryDataGrid.CurrentPageIndex = 0;
			this.UpdateView();
		}

		protected override void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			this.InitializeComponent();

			// Don't disable all of the buttons on the form after a user clicks a button.
			// If we disable the buttons, clicking the Export button will disable all of the buttons 
			// and they won't be enabled because when we export we write a file to the response, not the page.
			// If the page isn't in the response, the buttons will remain disabled.
			this.IgnoreInputDisable = true;
			base.OnInit(e);
		}

		protected void PageSizeDropDown_SelectedIndexChanged(object source, EventArgs e)
		{
			this.UpdateView();
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				var DataDictionary = new DataDictionaryClass();

				EntityToSiteMapCollectionClass EntityToSiteMapCollection =
					FMChannelHelper.MakeCall<IEntityToSiteMaps, EntityToSiteMapCollectionClass>(
																	 x =>
																	 x.EnumerateByTypeIDAndSiteGuid(this.Security, DataDictionary.EntityType, this.Security.SiteGuid)
																);

				if (EntityToSiteMapCollection.Count != 0)
				{
					this.DictionaryAssigned = true;
				}

				if (this.Page.IsPostBack)
				{
					// Set this now, because menu control will depend on it
					this.Session["UseDataDictionary"] = this.UseDataDictionaryCheckBox.Checked;
					this.Security.UseDataDictionary = this.UseDataDictionaryCheckBox.Checked;
				}
				else
				{
					this.UseDataDictionaryCheckBox.Checked = (this.Session["UseDataDictionary"] == null
															  || (bool)this.Session["UseDataDictionary"]);

					if (!this.Security.HasRight(RIGHT.TOGGLE_DATA_DICTIONARY))
					{
						this.UseDataDictionaryCheckBox.Enabled = false;
					}

					if (this.DictionaryAssigned || !this.Security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS)
						|| this.Security.SiteGuid != this.Security.LoginSiteGuid)
					{
						this.ImportButton.Enabled = false;
					}

					if (this.Session["DictionaryFilter"] == null)
					{
						this.Session["DictionaryFilter"] = 'A';
					}

					this.UpdateView();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    This method is called when the show all button is pressed. It will set the search string to null
		///    indicating that we do not want to use the filter on finding keys.  In addition, the find
		///    text box is cleared.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">
		///    The <see cref="System.EventArgs" /> instance containing the event data.
		/// </param>
		protected void ShowAllButtonOnClick(object sender, EventArgs e)
		{
			this.Session.Remove(DICTIONARY_FIND_STRING);
			this.searchString = null;
			this.FindTextBox.Text = string.Empty;
			this.DataDictionaryDataGrid.CurrentPageIndex = 0;
			this.UpdateView();
		}

		private void AButton_Command(object sender, CommandEventArgs e)
		{
			this.Session["DictionaryFilter"] = 'A';
			this.DataDictionaryDataGrid.EditItemIndex = -1;
			this.DataDictionaryDataGrid.CurrentPageIndex = 0;
			this.searchString = null;
			this.FindTextBox.Text = string.Empty;
			this.Session.Remove(DICTIONARY_FIND_STRING);
			this.UpdateView();
		}

		private void BButton_Command(object sender, CommandEventArgs e)
		{
			this.Session["DictionaryFilter"] = 'B';
			this.DataDictionaryDataGrid.EditItemIndex = -1;
			this.DataDictionaryDataGrid.CurrentPageIndex = 0;
			this.searchString = null;
			this.FindTextBox.Text = string.Empty;
			this.Session.Remove(DICTIONARY_FIND_STRING);
			this.UpdateView();
		}

		private void CButton_Command(object sender, CommandEventArgs e)
		{
			this.Session["DictionaryFilter"] = 'C';
			this.DataDictionaryDataGrid.EditItemIndex = -1;
			this.DataDictionaryDataGrid.CurrentPageIndex = 0;
			this.searchString = null;
			this.FindTextBox.Text = string.Empty;
			this.Session.Remove(DICTIONARY_FIND_STRING);
			this.UpdateView();
		}

		private void DButton_Command(object sender, CommandEventArgs e)
		{
			this.Session["DictionaryFilter"] = 'D';
			this.DataDictionaryDataGrid.EditItemIndex = -1;
			this.DataDictionaryDataGrid.CurrentPageIndex = 0;
			this.searchString = null;
			this.FindTextBox.Text = string.Empty;
			this.Session.Remove(DICTIONARY_FIND_STRING);
			this.UpdateView();
		}

		/// <summary>
		///    This method handles the cancel edit event. It ignores the change and removes the item
		///    from edit mode.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		private void DataDictionaryDataGrid_CancelCommand(object source, DataGridCommandEventArgs e)
		{
			this.PriorEditItemIndex = this.DataDictionaryDataGrid.EditItemIndex;
			this.DataDictionaryDataGrid.EditItemIndex = -1;

			if ((this.FindTextBox.Text == null) || (this.FindTextBox.Text.Length < 1))
			{
				this.searchString = null;
				this.Session.Remove(DICTIONARY_FIND_STRING);
			}
			else
			{
				this.searchString = this.FindTextBox.Text.ToUpper();
				this.Session.Add(DICTIONARY_FIND_STRING, this.searchString);
			}

			this.EnableControls(true);
			this.UpdateView();
		}

		/// <summary>
		///    This method handles the edit event for a selected item. It places the select item
		///    in edit mode.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		private void DataDictionaryDataGrid_EditCommand(object source, DataGridCommandEventArgs e)
		{
			this.DataDictionaryDataGrid.EditItemIndex = e.Item.ItemIndex;

			if ((this.FindTextBox.Text == null) || (this.FindTextBox.Text.Length < 1))
			{
				this.searchString = null;
				this.Session.Remove(DICTIONARY_FIND_STRING);
			}

			else
			{
				this.searchString = this.FindTextBox.Text.ToUpper();
				this.Session.Add(DICTIONARY_FIND_STRING, this.searchString);
			}

			this.EnableControls(false);
			this.UpdateView();
		}

		private void DataDictionaryDataGrid_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			var EditButton = (LinkButton)e.Item.FindControl("EditButton");

			if (EditButton != null)
			{
				if (this.DictionaryAssigned || !this.Security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
				{
					EditButton.Enabled = false;
				}
			}

			if (this.DataDictionaryDataGrid.EditItemIndex != -1 && e.Item.ItemIndex == this.DataDictionaryDataGrid.EditItemIndex)
			{
				var ValueTextBox = (TextBox)e.Item.FindControl("ValueTextBox");

				if (ValueTextBox != null)
				{
					ValueTextBox.Text = this.Server.HtmlDecode(ValueTextBox.Text);
				}
			}
			else
			{
				var ValueLabel = e.Item.FindControl("ValueLabel") as Label;

				if (ValueLabel != null)
				{
					ValueLabel.Text = this.Server.HtmlEncode(ValueLabel.Text);
				}
			}

			if ((this.DataDictionaryDataGrid != null && this.DataDictionaryDataGrid.EditItemIndex == e.Item.ItemIndex)
				|| this.PriorEditItemIndex == e.Item.ItemIndex)
			{
				// Now set the focus to the edit control
				Control ctrl = null;
				if (this.DataDictionaryDataGrid.EditItemIndex == e.Item.ItemIndex)
				{
					ctrl = e.Item.FindControl("ValueTextBox");
				}
				else
				{
					ctrl = e.Item.FindControl("EditButton");
				}

				if (ctrl != null)
				{
					string script = @"<script language='javascript'> document.getElementById('{0}').focus(); </script>";
					this.Page.ClientScript.RegisterStartupScript(
						this.GetType(), "page_set_focus", string.Format(script, ctrl.ClientID));
				}
			}
		}

		/// <summary>
		///    This method handles the page change event.  It moves the page to the next set of results.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		private void DataDictionaryDataGrid_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			// if we are editing do not allow a page change
			if (this.DataDictionaryDataGrid.EditItemIndex > -1)
			{
				return;
			}
			this.DataDictionaryDataGrid.CurrentPageIndex = e.NewPageIndex;

			if ((this.FindTextBox.Text == null) || (this.FindTextBox.Text.Length < 1))
			{
				this.searchString = null;
				this.Session.Remove(DICTIONARY_FIND_STRING);
			}
			else
			{
				this.searchString = this.FindTextBox.Text.ToUpper();
				this.Session.Add(DICTIONARY_FIND_STRING, this.searchString);
			}

			this.UpdateView();
		}

		/// <summary>
		///    This method handles the update event and saves any changes made to the data dictionary for the selected
		///    row.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		private void DataDictionaryDataGrid_UpdateCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				var DataDictionary = new DataDictionaryClass();

				this.PriorEditItemIndex = this.DataDictionaryDataGrid.EditItemIndex;

				var KeyLabel = (Label)e.Item.FindControl("KeyLabel");
				var ValueTextBox = (TextBox)e.Item.FindControl("ValueTextBox");

				if ((KeyLabel != null) && (ValueTextBox != null))
				{
					string keyLabelStr = HttpUtility.HtmlDecode(KeyLabel.Text);
					DataDictionary.SiteGuid = this.Security.SiteGuid;
					DataDictionary.Key = keyLabelStr;
					DataDictionary.Value = ValueTextBox.Text;

					string CurrentValue = GetDataDictionaryValueByKey(this.Security.SiteGuid, DataDictionary.Key);

					char[] Seperators = { '|' };
					string[] KeyStrings = DataDictionary.Key.Split(Seperators);

					if ((KeyStrings.Length > 1 && CurrentValue == KeyStrings[1]) || CurrentValue == DataDictionary.Key)
					{
						// Don't let the user enter the same exact value as the key
						if (DataDictionary.Value != string.Empty && DataDictionary.Value != DataDictionary.Key)
						{
							DataDictionary.SiteGuid = this.Security.SiteGuid;
							FMChannelHelper.MakeCall<IDataDictionariesClass>(x => x.Add(this.Security, DataDictionary));
						}
					}
					else
					{
						// If they enter the key value to modify an already translated item, we need to remove the key
						if (DataDictionary.Value == string.Empty || DataDictionary.Value == DataDictionary.Key)
						{
							FMChannelHelper.MakeCall<IDataDictionariesClass>(x => x.Purge(this.Security, DataDictionary.Key));
						}
						else
						{
							DataDictionary.SiteGuid = this.Security.SiteGuid;
							FMChannelHelper.MakeCall<IDataDictionariesClass>(x => x.Modify(this.Security, DataDictionary));
						}
					}
					this.dataDictionaryChangedField.Value = "true";

					// Reomve any LinkKey strings from session
					for (int ItemIndex = 0; ItemIndex < this.Session.Keys.Count; ItemIndex++)
					{
						if (this.Session.Keys[ItemIndex].StartsWith(FMLinkButton.LINK_KEY))
						{
							this.Session.Remove(this.Session.Keys[ItemIndex]);
							ItemIndex--;
						}
					}
				}

				this.DataDictionaryDataGrid.EditItemIndex = -1;

				if ((this.FindTextBox.Text == null) || (this.FindTextBox.Text.Length < 1))
				{
					this.searchString = null;
					this.Session.Remove(DICTIONARY_FIND_STRING);
				}
				else
				{
					this.searchString = this.FindTextBox.Text.ToUpper();
					this.Session.Add(DICTIONARY_FIND_STRING, this.searchString);
				}

				this.EnableControls(true);
				this.ucFMMenuBar.Refresh();
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.EnableControls(true);
				this.ErrorHandler(except);
			}
		}

		private void EButton_Command(object sender, CommandEventArgs e)
		{
			this.Session["DictionaryFilter"] = 'E';
			this.DataDictionaryDataGrid.EditItemIndex = -1;
			this.DataDictionaryDataGrid.CurrentPageIndex = 0;
			this.searchString = null;
			this.FindTextBox.Text = string.Empty;
			this.Session.Remove(DICTIONARY_FIND_STRING);
			this.UpdateView();
		}

		/// <summary>
		///    This method will disable/enable controls.
		/// </summary>
		/// <param name="enable">True to enable the controls.</param>
		private void EnableControls(bool enable)
		{
			this.DataDictFormPageSizeDropDown.Enabled = enable;
			this.FindBtn.Enabled = enable;
			this.ShowAllButton.Enabled = enable;
			this.ImportButton.Enabled = enable;
			this.ExportButton.Enabled = enable;
			this.FindTextBox.Enabled = enable;
			this.NonAlphaButton.Enabled = enable;
			this.UseDataDictionaryCheckBox.Enabled = enable;
			this.AButton.Enabled = enable;
			this.BButton.Enabled = enable;
			this.CButton.Enabled = enable;
			this.DButton.Enabled = enable;
			this.EButton.Enabled = enable;
			this.FButton.Enabled = enable;
			this.GButton.Enabled = enable;
			this.HButton.Enabled = enable;
			this.IButton.Enabled = enable;
			this.JButton.Enabled = enable;
			this.KButton.Enabled = enable;
			this.LButton.Enabled = enable;
			this.MButton.Enabled = enable;
			this.NButton.Enabled = enable;
			this.OButton.Enabled = enable;
			this.PButton.Enabled = enable;
			this.QButton.Enabled = enable;
			this.RButton.Enabled = enable;
			this.SButton.Enabled = enable;
			this.TButton.Enabled = enable;
			this.UButton.Enabled = enable;
			this.VButton.Enabled = enable;
			this.WButton.Enabled = enable;
			this.XButton.Enabled = enable;
			this.YButton.Enabled = enable;
			this.ZButton.Enabled = enable;
		}

		/// <summary>
		///    This method will return a data view collection to bind to the grid. It
		///    calls a method to retrieve the data dictionary items using either a Letter
		///    or a search string as a criterion.
		/// </summary>
		/// <returns></returns>
		private ICollection EnumerateDataDictionary()
		{
			// Locate the previous search string from the session. Set the set
			// string if found.
			if (this.Session[DICTIONARY_FIND_STRING] != null)
			{
				this.FindTextBox.Text = this.Session[DICTIONARY_FIND_STRING] as string;
				this.searchString = this.Session[DICTIONARY_FIND_STRING] as string;

				// If we have a persisted search string, set the dictionary filter to the 
				// first character so the key enumeration will get the right set of data.
				if (string.IsNullOrEmpty(this.searchString) == false)
				{
					this.Session["DictionaryFilter"] = this.searchString[0];
				}
			}

			var dictionaryFilter = (char)this.Session["DictionaryFilter"];

			var dataDictionaryDataTable = this.GetDataDictionaryDataset(dictionaryFilter);

			var searchValue = this.searchString ?? string.Empty;
			var sortedList = from s in dataDictionaryDataTable
								  where s.Key.ToUpper().Contains(searchValue) || s.Value.ToUpper().Contains(searchValue)
								  orderby s.Key
								  select s;

			// Bind the new results to the grid.
			return sortedList.ToList();
		}

		/// <summary>
		///    Handles the Command event of the ExportButton control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">
		///    The <see cref="System.Web.UI.WebControls.CommandEventArgs" /> instance containing the event data.
		/// </param>
		private void ExportButton_Command(object sender, CommandEventArgs e)
		{
			try
			{
				var DataDictionary = new DataDictionaryClass();

				var Document = new XmlDocument();
				XmlNode DataDictionaryNode = Document.CreateNode(XmlNodeType.Element, "DataDictionary", null);
				Document.AppendChild(DataDictionaryNode);

				string strAssemblyList =
					FMChannelHelper.MakeCall<IConfigurationSettings, string>(
						x => x.GetKeyValueByKey(base.Security, ConfigurationSettingDOClass.Key_DataDictionaryAssemblies));

				if (string.IsNullOrEmpty(strAssemblyList) == false)
				{
					char[] separator = { ';' };
					string[] dataDictionList = strAssemblyList.Split(separator, StringSplitOptions.RemoveEmptyEntries);

					string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
					if (dataDictionList.Length > 0)
					{
						for (int nextAssembly = 0; nextAssembly < dataDictionList.Length; nextAssembly++)
						{
							string assemblyName = dataDictionList[nextAssembly];
							Assembly DLL = null;
							if (!AssemblyDictionary.ContainsKey(assemblyName.ToLower()))
							{
								try
								{
									DLL = Assembly.LoadFrom(baseDirectory + "\\bin\\" + assemblyName);
								}
								catch
								{
									try
									{
										DLL = Assembly.Load(assemblyName);
									}
									catch (Exception ex)
									{
										string message = "Assembly Load Error on Data Dictionary Export. " + ex.Message;
										FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(message, FMEventLogEntryType.Warning));
									}
								}

								if (DLL != null)
									AssemblyDictionary.Add(assemblyName.ToLower(), DLL);
							}
							else
							{
								DLL = AssemblyDictionary.Get(assemblyName.ToLower());
							}

							if (DLL == null)
								continue;

							try
							{
								Type[] Types = DLL.GetTypes();
								foreach (Type Module in Types)
								{
									if (!Module.IsClass)
									{
										continue;
									}

									Type IDataDictionaryInterface = Module.GetInterface("FMBusinessObjects.DataObjects.IDataDictionary");

									if (IDataDictionaryInterface != null)
									{
										Object Engine = Activator.CreateInstance(Module);
										var IDataDictionary = (IDataDictionary)Engine;

										if (IDataDictionary != null)
										{
											string[] Keys = IDataDictionary.Keys(this.Security);

											//if we didn't find any keys, continue to avoid a null reference exception
											//some classes that implement IDataDictionary only return keys if you have the proper permissions, like TFMDTreeNav
											if (Keys == null)
											{
												continue;
											}

											foreach (string Key in Keys)
											{
												// Keys can be duplicated in different modules so check for a duplicate
												bool Found = false;
												foreach (XmlNode DataDictionaryEntryNode in DataDictionaryNode.ChildNodes)
												{
													if (DataDictionaryEntryNode.Attributes["Key"].Value == Key)
													{
														Found = true;
														break;
													}
												}

												if (!Found)
												{
													try
													{
														DataDictionary.Key = Key;
														string Value = GetDataDictionaryValueByKey(this.Security.LoginSiteGuid, Key);

														char[] Seperator = { '|' };
														string[] KeyStrings = Key.Split(Seperator);

														if (KeyStrings.Length > 1 && Value == KeyStrings[1] || Key == Value)
														{
															DataDictionary.Value = string.Empty;
														}
														else
														{
															DataDictionary.Value = Value;
														}
													}
													catch (Exception except)
													{
														this.ErrorHandler(except);
														continue;
													}

													var DataDictionaryEntryElement =
														(XmlElement)Document.CreateNode(XmlNodeType.Element, "DataDictionaryEntry", null);
													DataDictionary.Store(DataDictionaryEntryElement);
													DataDictionaryNode.AppendChild(DataDictionaryEntryElement);
												}
											}
										}
									}
								}
							}
							catch { } // Try: Type[] Types = DLL.GetTypes()
						}
					}
				}

				this.Response.ClearContent();
				this.Response.ClearHeaders();
				this.Response.ContentType = "application/xml";
				this.Response.AddHeader("Content-Disposition", "attachment; filename=DataDictionary.xml");
				Document.Save(this.Response.OutputStream);
				this.Response.Flush();
				this.Response.SuppressContent = true;
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void FButton_Command(object sender, CommandEventArgs e)
		{
			this.Session["DictionaryFilter"] = 'F';
			this.DataDictionaryDataGrid.EditItemIndex = -1;
			this.DataDictionaryDataGrid.CurrentPageIndex = 0;
			this.searchString = null;
			this.FindTextBox.Text = string.Empty;
			this.Session.Remove(DICTIONARY_FIND_STRING);
			this.UpdateView();
		}

		private void GButton_Command(object sender, CommandEventArgs e)
		{
			this.Session["DictionaryFilter"] = 'G';
			this.DataDictionaryDataGrid.EditItemIndex = -1;
			this.DataDictionaryDataGrid.CurrentPageIndex = 0;
			this.searchString = null;
			this.FindTextBox.Text = string.Empty;
			this.Session.Remove(DICTIONARY_FIND_STRING);
			this.UpdateView();
		}

		/// <summary>
		///    This method retrieves all the dictionary keys from all the assemblies and searches for any value
		///    change from the database. It then builds a table of keys and values for the criterion.  The criterion
		///    can be a letter or the search string.
		/// </summary>
		/// <param name="DictionaryFilter"></param>
		/// <param name="dataDictionaryTable"></param>
		/// <param name="DLL"></param>
		private void GetAssemblyKeys(char DictionaryFilter, Dictionary<string, string> dataDictionaryTable, Assembly DLL)
		{
			try
			{
				Type[] Types = DLL.GetTypes();
				bool useSearch = false;

				// Determine if the search string contains a value, if so, then
				// we want to use the search string as a criterion and not a letter.
				if ((this.searchString != null) && (this.searchString.Length > 0))
				{
					useSearch = true;
				}

				foreach (Type Module in Types)
				{
					if (!Module.IsClass)
					{
						continue;
					}

					Type IDataDictionaryInterface = Module.GetInterface("FMBusinessObjects.DataObjects.IDataDictionary");
					if (IDataDictionaryInterface != null)
					{
						Object Engine = Activator.CreateInstance(Module);
						var DataDictionary = Engine as IDataDictionary;

						if (DataDictionary != null)
						{
							try
							{
								string[] Keys = DataDictionary.Keys(this.Security);

								if (Keys == null)
								{
									continue;
								}

								foreach (string Key in Keys)
								{
									if (Key == string.Empty)
									{
										continue;
									}

									if (useSearch == false)
									{
										char KeyFirstCharacter = Key.ToUpper()[0];

										if ((DictionaryFilter >= 'A') && (DictionaryFilter <= 'Z'))
										{
											if (KeyFirstCharacter != DictionaryFilter)
											{
												continue;
											}
										}
										else
										{
											if ((KeyFirstCharacter >= 'A') && (KeyFirstCharacter <= 'Z'))
											{
												continue;
											}
										}
									}

									string htmlCompatibleKey = ConvertFromSpecialCharacter(Key);

									if (dataDictionaryTable.ContainsKey(htmlCompatibleKey) == false)
									{
										dataDictionaryTable.Add(htmlCompatibleKey, string.Empty);
									}
								}
							}
							catch (Exception except)
							{
								if (except.Message == "Access Denied")
								{
									continue;
								}
								else
								{
									this.ErrorHandler(except);
								}
							}
						}
					}
				}
			}
			catch { }
		}

		private void HButton_Command(object sender, CommandEventArgs e)
		{
			this.Session["DictionaryFilter"] = 'H';
			this.DataDictionaryDataGrid.EditItemIndex = -1;
			this.DataDictionaryDataGrid.CurrentPageIndex = 0;
			this.searchString = null;
			this.FindTextBox.Text = string.Empty;
			this.Session.Remove(DICTIONARY_FIND_STRING);
			this.UpdateView();
		}

		private void IButton_Command(object sender, CommandEventArgs e)
		{
			this.Session["DictionaryFilter"] = 'I';
			this.DataDictionaryDataGrid.EditItemIndex = -1;
			this.DataDictionaryDataGrid.CurrentPageIndex = 0;
			this.searchString = null;
			this.FindTextBox.Text = string.Empty;
			this.Session.Remove(DICTIONARY_FIND_STRING);
			this.UpdateView();
		}

		private void ImportButton_Command(object sender, CommandEventArgs e)
		{
			XmlTextReader reader = null;
			var document = new XmlDocument();
			try
			{
				if (this.Request.Files.AllKeys.Length != 0)
				{
					List<DataDictionaryClass> dictionaryAddList = new List<DataDictionaryClass>();
					List<DataDictionaryClass> dictionaryModList = new List<DataDictionaryClass>();
					List<DataDictionaryClass> dictionaryDelList = new List<DataDictionaryClass>();

					HttpPostedFile File = this.Request.Files[0];

					if (File.FileName != string.Empty && File.ContentLength != 0)
					{
						reader = new XmlTextReader(File.InputStream);
						document.Load(reader);
						var currentDataDictList = FMChannelHelper.MakeCall<IDataDictionariesClass, List<DataDictionaryClass>>(x => x.EnumerateBySite2(this.Security));

						foreach (XmlNode dataDictionaryNode in document)
						{
							if (dataDictionaryNode.Name == "DataDictionary")
							{
								foreach (XmlNode dataDictionaryEntryNode in dataDictionaryNode.ChildNodes)
								{
									var dataDictionary = new DataDictionaryClass();
									try
									{
										dataDictionary.Load(dataDictionaryEntryNode);
									}
									catch (Exception except1)
									{
										this.ErrorHandler(except1);
										continue;
									}

									var currentDictValue = currentDataDictList.Find(x => x.Key == dataDictionary.Key);

									char[] seperators = { '.', '|' };
									string[] keyStrings = dataDictionary.Key.Split(seperators);

									if (currentDictValue == null
										|| (keyStrings.Length > 1 && currentDictValue.Value == keyStrings[1])
										|| currentDictValue.Value == dataDictionary.Key)
									{
										if (dataDictionary.Value != string.Empty && dataDictionary.Value != dataDictionary.Key)
										{
											dataDictionary.SiteGuid = this.Security.SiteGuid;
											dictionaryAddList.Add(dataDictionary);
										}
									}
									else
									{
										if (dataDictionary.Value == string.Empty || dataDictionary.Value == dataDictionary.Key)
										{
											dictionaryDelList.Add(dataDictionary);
										}
										else
										{
											dataDictionary.SiteGuid = this.Security.SiteGuid;
											dictionaryModList.Add(dataDictionary);
										}
									}
								}
							}
						}

						if (dictionaryAddList.Count > 0 || dictionaryModList.Count > 0 || dictionaryDelList.Count > 0)
						{
							FMChannelHelper.MakeCall<IDataDictionariesClass>(x => x.ImportData(this.Security, dictionaryAddList, dictionaryModList, dictionaryDelList));
						}

						this.searchString = null;
						this.ucFMMenuBar.Refresh();
						this.UpdateView();
					}
					else
					{
						throw new Exception("Select a file to import");
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
			finally
			{
				if (reader != null)
				{
					reader.Close();
				}
			}
		}

		private void ModifyDictionaryValueByKey(SecurityClass securityClass, DataDictionaryClass DataDictionary)
		{
			FMChannelHelper.MakeCall<IDataDictionariesClass>(
																	 x =>
																	 x.Modify(securityClass, DataDictionary)
																);
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.NonAlphaButton.Command += this.NonAlphaButton_Command;
			this.DataDictionaryDataGrid.EditCommand += this.DataDictionaryDataGrid_EditCommand;
			this.DataDictionaryDataGrid.PageIndexChanged += this.DataDictionaryDataGrid_PageIndexChanged;
			this.DataDictionaryDataGrid.CancelCommand += this.DataDictionaryDataGrid_CancelCommand;
			this.DataDictionaryDataGrid.UpdateCommand += this.DataDictionaryDataGrid_UpdateCommand;
			this.DataDictionaryDataGrid.ItemDataBound += this.DataDictionaryDataGrid_ItemDataBound;
			this.ExportButton.Command += this.ExportButton_Command;
			this.ImportButton.Command += this.ImportButton_Command;
			this.AButton.Command += this.AButton_Command;
			this.BButton.Command += this.BButton_Command;
			this.CButton.Command += this.CButton_Command;
			this.DButton.Command += this.DButton_Command;
			this.EButton.Command += this.EButton_Command;
			this.FButton.Command += this.FButton_Command;
			this.GButton.Command += this.GButton_Command;
			this.HButton.Command += this.HButton_Command;
			this.IButton.Command += this.IButton_Command;
			this.JButton.Command += this.JButton_Command;
			this.KButton.Command += this.KButton_Command;
			this.LButton.Command += this.LButton_Command;
			this.MButton.Command += this.MButton_Command;
			this.NButton.Command += this.NButton_Command;
			this.OButton.Command += this.OButton_Command;
			this.PButton.Command += this.PButton_Command;
			this.QButton.Command += this.QButton_Command;
			this.RButton.Command += this.RButton_Command;
			this.SButton.Command += this.SButton_Command;
			this.TButton.Command += this.TButton_Command;
			this.UButton.Command += this.UButton_Command;
			this.VButton.Command += this.VButton_Command;
			this.WButton.Command += this.WButton_Command;
			this.XButton.Command += this.XButton_Command;
			this.YButton.Command += this.YButton_Command;
			this.ZButton.Command += this.ZButton_Command;
		}

		private void JButton_Command(object sender, CommandEventArgs e)
		{
			this.Session["DictionaryFilter"] = 'J';
			this.DataDictionaryDataGrid.EditItemIndex = -1;
			this.DataDictionaryDataGrid.CurrentPageIndex = 0;
			this.searchString = null;
			this.FindTextBox.Text = string.Empty;
			this.Session.Remove(DICTIONARY_FIND_STRING);
			this.UpdateView();
		}

		private void KButton_Command(object sender, CommandEventArgs e)
		{
			this.Session["DictionaryFilter"] = 'K';
			this.DataDictionaryDataGrid.EditItemIndex = -1;
			this.DataDictionaryDataGrid.CurrentPageIndex = 0;
			this.searchString = null;
			this.FindTextBox.Text = string.Empty;
			this.Session.Remove(DICTIONARY_FIND_STRING);
			this.UpdateView();
		}

		private void LButton_Command(object sender, CommandEventArgs e)
		{
			this.Session["DictionaryFilter"] = 'L';
			this.DataDictionaryDataGrid.EditItemIndex = -1;
			this.DataDictionaryDataGrid.CurrentPageIndex = 0;
			this.searchString = null;
			this.FindTextBox.Text = string.Empty;
			this.Session.Remove(DICTIONARY_FIND_STRING);
			this.UpdateView();
		}

		private void MButton_Command(object sender, CommandEventArgs e)
		{
			this.Session["DictionaryFilter"] = 'M';
			this.DataDictionaryDataGrid.EditItemIndex = -1;
			this.DataDictionaryDataGrid.CurrentPageIndex = 0;
			this.searchString = null;
			this.FindTextBox.Text = string.Empty;
			this.Session.Remove(DICTIONARY_FIND_STRING);
			this.UpdateView();
		}

		private void NButton_Command(object sender, CommandEventArgs e)
		{
			this.Session["DictionaryFilter"] = 'N';
			this.DataDictionaryDataGrid.EditItemIndex = -1;
			this.DataDictionaryDataGrid.CurrentPageIndex = 0;
			this.searchString = null;
			this.FindTextBox.Text = string.Empty;
			this.Session.Remove(DICTIONARY_FIND_STRING);
			this.UpdateView();
		}

		private void NonAlphaButton_Command(object sender, CommandEventArgs e)
		{
			this.Session["DictionaryFilter"] = ' ';
			this.DataDictionaryDataGrid.EditItemIndex = -1;
			this.DataDictionaryDataGrid.CurrentPageIndex = 0;
			this.searchString = null;
			this.FindTextBox.Text = string.Empty;
			this.Session.Remove(DICTIONARY_FIND_STRING);
			this.UpdateView();
		}

		private void OButton_Command(object sender, CommandEventArgs e)
		{
			this.Session["DictionaryFilter"] = 'O';
			this.DataDictionaryDataGrid.EditItemIndex = -1;
			this.DataDictionaryDataGrid.CurrentPageIndex = 0;
			this.searchString = null;
			this.FindTextBox.Text = string.Empty;
			this.Session.Remove(DICTIONARY_FIND_STRING);
			this.UpdateView();
		}

		private void PButton_Command(object sender, CommandEventArgs e)
		{
			this.Session["DictionaryFilter"] = 'P';
			this.DataDictionaryDataGrid.EditItemIndex = -1;
			this.DataDictionaryDataGrid.CurrentPageIndex = 0;
			this.searchString = null;
			this.FindTextBox.Text = string.Empty;
			this.Session.Remove(DICTIONARY_FIND_STRING);
			this.UpdateView();
		}

		private void QButton_Command(object sender, CommandEventArgs e)
		{
			this.Session["DictionaryFilter"] = 'Q';
			this.DataDictionaryDataGrid.EditItemIndex = -1;
			this.DataDictionaryDataGrid.CurrentPageIndex = 0;
			this.searchString = null;
			this.FindTextBox.Text = string.Empty;
			this.Session.Remove(DICTIONARY_FIND_STRING);
			this.UpdateView();
		}

		private void RButton_Command(object sender, CommandEventArgs e)
		{
			this.Session["DictionaryFilter"] = 'R';
			this.DataDictionaryDataGrid.EditItemIndex = -1;
			this.DataDictionaryDataGrid.CurrentPageIndex = 0;
			this.searchString = null;
			this.FindTextBox.Text = string.Empty;
			this.Session.Remove(DICTIONARY_FIND_STRING);
			this.UpdateView();
		}

		private void SButton_Command(object sender, CommandEventArgs e)
		{
			this.Session["DictionaryFilter"] = 'S';
			this.DataDictionaryDataGrid.EditItemIndex = -1;
			this.DataDictionaryDataGrid.CurrentPageIndex = 0;
			this.searchString = null;
			this.FindTextBox.Text = string.Empty;
			this.Session.Remove(DICTIONARY_FIND_STRING);
			this.UpdateView();
		}

		private void TButton_Command(object sender, CommandEventArgs e)
		{
			this.Session["DictionaryFilter"] = 'T';
			this.DataDictionaryDataGrid.EditItemIndex = -1;
			this.DataDictionaryDataGrid.CurrentPageIndex = 0;
			this.searchString = null;
			this.FindTextBox.Text = string.Empty;
			this.Session.Remove(DICTIONARY_FIND_STRING);
			this.UpdateView();
		}

		private void UButton_Command(object sender, CommandEventArgs e)
		{
			this.Session["DictionaryFilter"] = 'U';
			this.DataDictionaryDataGrid.EditItemIndex = -1;
			this.DataDictionaryDataGrid.CurrentPageIndex = 0;
			this.searchString = null;
			this.FindTextBox.Text = string.Empty;
			this.Session.Remove(DICTIONARY_FIND_STRING);
			this.UpdateView();
		}

		private void UpdateView()
		{
			try
			{
				ICollection Dictionary = this.EnumerateDataDictionary();

				this.DataDictFormPageSizeDropDown.SetPageSize(this.DataDictionaryDataGrid, Dictionary.Count);

				this.DataDictionaryDataGrid.DataSource = Dictionary;
				this.DataDictionaryDataGrid.DataBind();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void VButton_Command(object sender, CommandEventArgs e)
		{
			this.Session["DictionaryFilter"] = 'V';
			this.DataDictionaryDataGrid.EditItemIndex = -1;
			this.DataDictionaryDataGrid.CurrentPageIndex = 0;
			this.searchString = null;
			this.FindTextBox.Text = string.Empty;
			this.Session.Remove(DICTIONARY_FIND_STRING);
			this.UpdateView();
		}

		private void WButton_Command(object sender, CommandEventArgs e)
		{
			this.Session["DictionaryFilter"] = 'W';
			this.DataDictionaryDataGrid.EditItemIndex = -1;
			this.DataDictionaryDataGrid.CurrentPageIndex = 0;
			this.searchString = null;
			this.FindTextBox.Text = string.Empty;
			this.Session.Remove(DICTIONARY_FIND_STRING);
			this.UpdateView();
		}

		private void XButton_Command(object sender, CommandEventArgs e)
		{
			this.Session["DictionaryFilter"] = 'X';
			this.DataDictionaryDataGrid.EditItemIndex = -1;
			this.DataDictionaryDataGrid.CurrentPageIndex = 0;
			this.searchString = null;
			this.FindTextBox.Text = string.Empty;
			this.Session.Remove(DICTIONARY_FIND_STRING);
			this.UpdateView();
		}

		private void YButton_Command(object sender, CommandEventArgs e)
		{
			this.Session["DictionaryFilter"] = 'Y';
			this.DataDictionaryDataGrid.EditItemIndex = -1;
			this.DataDictionaryDataGrid.CurrentPageIndex = 0;
			this.searchString = null;
			this.FindTextBox.Text = string.Empty;
			this.Session.Remove(DICTIONARY_FIND_STRING);
			this.UpdateView();
		}

		private void ZButton_Command(object sender, CommandEventArgs e)
		{
			this.Session["DictionaryFilter"] = 'Z';
			this.DataDictionaryDataGrid.EditItemIndex = -1;
			this.DataDictionaryDataGrid.CurrentPageIndex = 0;
			this.searchString = null;
			this.FindTextBox.Text = string.Empty;
			this.Session.Remove(DICTIONARY_FIND_STRING);
			this.UpdateView();
		}

		#endregion
	}
}