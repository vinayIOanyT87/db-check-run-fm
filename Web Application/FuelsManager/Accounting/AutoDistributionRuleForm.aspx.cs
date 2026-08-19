///***************************************************************************
/// Module Name:  AutoDistributionRuleForm
/// Author:       Daniel Or
/// Copyright (c) Varec, Inc.  All rights reserved.
///***************************************************************************

namespace FuelsManager.Accounting
{
    using System;
    using System.Collections.Generic;
    using System.Web.UI.WebControls;
    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.Constants;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.UtilityObjects;
    using FMControls;

    using FMCore;
	using FMWebApp;

    using FuelsManager.Accounting;

    public partial class AutoDistributionRuleForm : AccountingWebFormView
	{

		#region constants and fields

		/* constants referenced in the html/menu */
		public const string PageUrl = "AutoDistributionRuleForm.aspx";
		public const string PageTitle = "Automatic Distribution Rule Configuration";
		public const string MenuName = "Rules";

		public const string IDLabelText = "Rule ID";
		public const string DescriptionLabelText = "Description";
		public const string EnabledCheckboxText = "Enabled";
		public const string DefaultEOMCheckboxText = "Default EOM";
		public const string DistributionAliasLabelText = "Distribution Transaction Alias";
		public const string DefaultReasonLabelText = "Default Reason Code";
		public const string DefaultNoteLabelText = "Default Note";
		public const string SelectedManagersLabelText = "Assigned Managers";
		public const string AvailableManagersLabelText = "Unassigned Managers";
		public const string SelectedProductsLabelText = "Assigned Products";
		public const string AvailableProductsLabelText = "Unassigned Products";
		public const string SelectedTransactionsLabelText = "Assigned Throughput Transactions";
		public const string AvailableTransactionsLabelText = "Unassigned Throughput Transactions";
		public const string SelectedOwnersLabelText = "Assigned Owners";
		public const string AvailableOwnersLabelText = "Unassigned Owners";
		

		/* private */
		private const string NoSessionObjectMessage = "Expected session to contain rule object.";
		private const string GroupPrefix = "*";
		private const string NongroupPrefix = "";
		private const string SeparatorString = "------------------------------";

		#endregion constants and fields

		#region Fields
		private AutoDistributionRuleDO currentRule = null;
		// The following is a collection of dictionary of each Map Type, ManagerGroup, Maanger, ProductGroup, etc.
		// The key is the actual MapList variable
		// The inner dictionary uses the Assigned Guid(ManagerGuid, ProductGuid) as the key.
		private Dictionary<AutoDistributionRuleMapDOCollection, AutoDistributionRuleMapDOCollection> originalMaps = null;

		#endregion

		#region Properties
		/// <summary>
		/// Sets and returns Session[AutoDistributionRulesessionKey] as AutoDistributionRuleClass
		/// </summary>
		private AutoDistributionRuleDO MySessionObject
		{
			get
			{
				return this.Session[PageSessionKeyConstants.AutoDistributionRule] as AutoDistributionRuleDO;
			}
			set
			{
				this.Session[PageSessionKeyConstants.AutoDistributionRule] = value;
			}
		}
		#endregion

		#region page events
		protected override void OnInit(EventArgs eventArgList)
		{
			base.OnInit(eventArgList);
			base.Initialize();
			this.InitializeComponents();
		}

		private void InitializeComponents()
		{
			this.assignManagerButton.Click += new EventHandler(this.AssignManagerButton_Click);
			this.unassignManagerButton.Click += new EventHandler(this.UnassignManagerButton_Click);
			this.assignOwnerButton.Click += new EventHandler(this.AssignOwnerButton_Click);
			this.unassignOwnerButton.Click += new EventHandler(this.UnassignOwnerButton_Click);
			this.assignProductButton.Click += new EventHandler(this.AssignProductButton_Click);
			this.unassignProductButton.Click += new EventHandler(this.UnassignProductButton_Click);
			this.assignTransactionButton.Click += new EventHandler(this.AssignTransactionButton_Click);
			this.unassignTransactionButton.Click += new EventHandler(this.UnassignTransactionButton_Click);
		}

		protected void Page_Init(object sender, EventArgs eventArgList)
		{
			try
			{
				base.MaintainScrollPositionOnPostBack = true;

				this.GetSecurity();
				this.currentRule = this.MySessionObject;
				if (this.currentRule == null)
				{
					throw new NullReferenceException(NoSessionObjectMessage);
				}
				this.SetControlStates();
				this.LoadOriginalRule();

				this.InitializeControls();

			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}
		protected void Page_Load(object sender, EventArgs eventArgList)
		{
			try
			{
				this.ClearSessionErrors();

				if (this.Page.IsPostBack == false)
				{
					this.UpdateView();
				}
			}

			catch (Exception error)
			{
				this.ErrorHandler(error);
			}
		}

		/// <summary>
		/// This method handles the OK button event. It will save the data and return
		/// to the calling page.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgList"></param>
		protected void OK_Command(object sender, System.EventArgs eventArgList)
		{
			try
			{

				if (this.CommitData())
				{
					this.TransferToOriginatingForm();
				}

			}
			catch (Exception error)
			{
				this.ErrorHandler(error);
			}
		}

		/// <summary>
		/// This method handles the cancel event. It will return back to the calling page.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgList"></param>
		protected void Cancel_Command(object sender, System.EventArgs eventArgList)
		{
			try
			{
				this.TransferToOriginatingForm();
			}
			catch (Exception error)
			{
				this.ErrorHandler(error);
			}
		}

		protected void New_Command(object sender, System.EventArgs eventArgList)
		{
			try
			{
				if (this.CommitData())
				{
					AddARule(this);
				}
			}
			catch (Exception error)
			{
				this.ErrorHandler(error);
			}
		}
		#endregion

		#region public methods
		/// <summary>
		/// This is to add a new rule(called from both the summary and the detail page).
		/// </summary>
		/// <param name="currentPage"></param>
		public static void AddARule(FMFormBase currentPage)
		{
			try
			{
				AutoDistributionRuleDO newRule = new AutoDistributionRuleDO();
				newRule.SiteGuid = currentPage.Security.SiteGuid;
				currentPage.Session[PageSessionKeyConstants.AutoDistributionRule] = newRule;
			}
			catch (Exception error)
			{
				currentPage.ErrorHandler(error);
				return;
			}
			currentPage.Redirect(PageUrl);
		}
		#endregion public methods

		#region private methods

		/// <summary>
		/// 
		/// </summary>
		/// <param name="guidString"></param>
		/// <returns></returns>
		private static Guid FromStringToGuid(string guidString)
		{
			Guid retValue = Guid.Empty;

			Guid.TryParse(guidString, out retValue);
			return retValue;
		}

		/// <summary>
		/// populate transaction alias drop down
		/// </summary>
		private void PopulateDistributionAliasDropDown()
		{
			this.distributionAliasDropDownList.Items.Clear();

			TransactionAliasNameCollectionClass aliasList = FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasNameCollectionClass>(
				x =>
				x.EnumerateNamesOnly(this.Security, false)
			);

			foreach (TransactionAliasNameClass alias in aliasList)
			{

				if (alias.TransTypeID == TransactionTypes.T1_PrimaryAdjustment ||
					alias.TransTypeID == TransactionTypes.T2_SecondaryAdjustment)
				{
					ListItem Item = new ListItem(this.GetTranslatedText(alias.AliasName), alias.MasterRecordGuid.ToString());
					this.distributionAliasDropDownList.Items.Add(Item);
				}

			}
		}

		/// <summary>
		/// populate reason code drop down
		/// </summary>
		private void PopulateReasonCodeDropDown()
		{
			this.reasonCodeDropDownList.Items.Clear();

			AutoDistributionReasonCodeCollectionClass reasonCodeList =
				FMChannelHelper.MakeCall<IAutoDistributionReasonCodes, AutoDistributionReasonCodeCollectionClass>(
				x =>
				x.Enumerate(this.Security)
			);

			foreach (AutoDistributionReasonCodeClass reasonCode in reasonCodeList)
			{
				string itemText = string.Format("{0} - {1}", reasonCode.ID, reasonCode.Description);
				ListItem Item = new ListItem(itemText, reasonCode.IdentityGuid.ToString());
				this.reasonCodeDropDownList.Items.Add(Item);
			}

		}

		private void InitializeControls()
		{
			this.PopulateDistributionAliasDropDown();
			this.PopulateReasonCodeDropDown();
		}

		/// <summary>
		/// Disable all the buttons
		/// </summary>
		private void SetControlStates()
		{
			this.enabledCheckBox.Text = EnabledCheckboxText + ":";
			this.defaultEOMCheckBox.Text = DefaultEOMCheckboxText + ":";
			// disable if this site doesn't own the rule or this user does not have edit right.
			if ((this.currentRule.SiteGuid != this.Security.SiteGuid && this.currentRule.IdentityGuid != Guid.Empty) ||
				 (this.Security.HasRight(RIGHT.MODIFY_AUTO_DISTRIBUTION_CONFIGURATION) == false))
			{
				WebControl[] controlList = new WebControl[]
				{
					this.IDTextBox, this.descriptionTextBox, this.defaultNotesTextBox,
					this.enabledCheckBox, this.defaultEOMCheckBox, 
					this.distributionAliasDropDownList, this.reasonCodeDropDownList,
					this.assignManagerButton, this.unassignManagerButton,
					this.assignOwnerButton, this.unassignOwnerButton,
					this.assignProductButton, this.unassignProductButton,
					this.assignTransactionButton, this.unassignTransactionButton,
					this.OKButton, this.NewButton				
				};
				foreach (WebControl currentControl in controlList)
				{
					currentControl.Enabled = false;
				}
			}
		}

		/// <summary>
		/// Go through allEntities, if entity exists in assignedEntites, add it to selectedList, else add it to availableList
		/// </summary>
		/// <typeparam name="EntityType"></typeparam>
		/// <param name="selectedList"></param>
		/// <param name="availableList"></param>
		/// <param name="allEntities"></param>
		/// <param name="assignedEntites"></param>
		/// <param name="valuePrefix"></param>
		/// <param name="clearList"></param>
		private void PopulateLists<EntityType>(FMListBox selectedList, FMListBox availableList,
					IEnumerable<EntityType> allEntities, AutoDistributionRuleMapDOCollection assignedEntites, string valuePrefix, bool clearList)
			where EntityType : BaseDataObject
		{
            selectedList.ThrowIfNull("selectedList");
            availableList.ThrowIfNull("availableList");
            allEntities.ThrowIfNull("allEntities");
            assignedEntites.ThrowIfNull("assignedEntities");

			if (clearList)
			{
				selectedList.Items.Clear();
				availableList.Items.Clear();
			}

			foreach (EntityType theEntity in allEntities)
			{
				// Determine whether the entity is selected or available
				Guid entityGuid = theEntity.IdentityGuid;
				FMListBox targetList;
				if (assignedEntites.ContainsAssignedGuid(entityGuid))
				{
					targetList = selectedList;
				}
				else
				{
					targetList = availableList;
				}

				// add the item to the list
				string newValue = valuePrefix + entityGuid.ToString();
				ListItem newItem = new ListItem(valuePrefix + this.GetTranslatedText(theEntity.ID), newValue);
				targetList.Items.Add(newItem);
			}
		}

		private bool IsStringAGroupString(string src)
		{
            src.ThrowIfNull("src");

			return src.StartsWith(GroupPrefix);
		}

		private bool IsStringASeparator(string src)
		{
            src.ThrowIfNull("src");

            return string.Compare(src, SeparatorString, StringComparison.OrdinalIgnoreCase) == 0;
		}


		/// <summary>
		/// This will remove and add a separator as needed
		/// </summary>
		/// <param name="availableList"></param>
		private void DetectSeparator(FMListBox availableList)
		{
            availableList.ThrowIfNull("availableList");

			ListItemCollection allItems = availableList.Items;

			int groupCount = 0;
			int nonGroupCount = 0;
			int firstNonGroupIndex = -1;
			int separatorIndex = -1;
			// find out the status by finding out the values of the above variables
			for (int idx = 0; idx < allItems.Count; idx++)
			{
				ListItem currentItem = allItems[idx];
				bool thisItemIsAGroup = this.IsStringAGroupString(currentItem.Text);
				bool thisItemIsASeparator = this.IsStringASeparator(currentItem.Text);
				if (thisItemIsAGroup)
				{
					groupCount++;
				}
				else if (thisItemIsASeparator)
				{
					separatorIndex = idx;
				}
				else
				{
					nonGroupCount++;

					if (firstNonGroupIndex == -1)
					{
						firstNonGroupIndex = idx;
					}

				}
			}

			// 1st case, no item
			if ((groupCount == 0) && (nonGroupCount == 0))
			{
				allItems.Clear();
			}
			// 2nd case, only groups or only non-groups, we don't need a separator
			else if ((groupCount == 0) || (nonGroupCount == 0))
			{
				if (separatorIndex >= 0)
				{
					allItems.RemoveAt(separatorIndex);
				}
			}
			// 3rd else, we have both groups and non-groups
			else
			{
				// we should have a separator, add one if it's not there
				if (separatorIndex < 0)
				{
					ListItem separtor = new ListItem(SeparatorString, Guid.Empty.ToString());
					separtor.Enabled = false;
					allItems.Insert(firstNonGroupIndex, separtor);
				}
			}
		}

		/// <summary>
		/// called when the page is loaded, save the maps, so we can retrieve the existing map record when an item is deselected and selected.
		/// </summary>
		private void LoadOriginalRule()
		{
			if (this.currentRule.IdentityGuid.IsEmpty() == false)
			{
				AutoDistributionRuleDO originalRule = FMChannelHelper.MakeCall<IAutoDistributionRules, AutoDistributionRuleDO>(
					x =>
					x.Get(this.Security, this.currentRule.IdentityGuid)
				);
				this.originalMaps = new Dictionary<AutoDistributionRuleMapDOCollection, AutoDistributionRuleMapDOCollection>();
				this.originalMaps.Add(this.currentRule.ManagerList, originalRule.ManagerList);
				this.originalMaps.Add(this.currentRule.ManagerGroupList, originalRule.ManagerGroupList);
				this.originalMaps.Add(this.currentRule.ProductList, originalRule.ProductList);
				this.originalMaps.Add(this.currentRule.ProductGroupList, originalRule.ProductGroupList);
				this.originalMaps.Add(this.currentRule.OwnerList, originalRule.OwnerList);
				this.originalMaps.Add(this.currentRule.OwnerGroupList, originalRule.OwnerGroupList);
				this.originalMaps.Add(this.currentRule.TransactionAliasList, originalRule.TransactionAliasList);
			}
		}

		/// <summary>
		/// Only the following type of aliases are allowed in the list
		/// </summary>
		/// <param name="trxAlias"></param>
		/// <returns></returns>
		private static bool IsAllowedTransactionAlias(TransactionAliasClass trxAlias)
		{
			bool isAllowed = false;
			switch (trxAlias.TransTypeID)
			{
				case TransactionTypes.T1_PrimaryAdjustment:
				case TransactionTypes.T2_SecondaryAdjustment:
				case TransactionTypes.T3_PrimaryDefuel:
				case TransactionTypes.T4_SecondaryDefuel:
				case TransactionTypes.T5_PrimaryDisbursement:
				case TransactionTypes.T6_SecondaryDisbursement:
				case TransactionTypes.T8_Receipt:
				case TransactionTypes.T10_Unload:
				case TransactionTypes.T15_PrimaryRegrade:
				case TransactionTypes.T16_SecondaryRegrade:
				case TransactionTypes.T24_Aggregate:
				case TransactionTypes.T25_Shipment:
					isAllowed = true;
					break;
			}
			return isAllowed;
		}

		/// <summary>
		/// For each list, this method calls the service, enumeate a list and add it to the listbox.
		/// The listbox may have Group and nongroup list.
		/// </summary>
		private void PopulateLists()
		{
			// get company group list, shared by both managers and cowners
			CompanyGroupCollectionClass allCompanyGroupList = FMChannelHelper.MakeCall<ICompanyGroups, CompanyGroupCollectionClass>(
				x =>
				x.Enumerate(this.Security)
			);

			// setting up managers, the group then company
			CompanyCollectionClass allManagerList = FMChannelHelper.MakeCall<ICompanies, CompanyCollectionClass>(
				x =>
				x.EnumerateByRole(this.Security, COMPANY_ROLE.MANAGER, false, true)
			);
			foreach (CompanyClass c in allManagerList)
			{
				c.IdentityGuid = c.MasterRecordGuid;
			}

			this.PopulateLists(this.selectedManagersList, this.availableManagersList, allCompanyGroupList, this.currentRule.ManagerGroupList, GroupPrefix, true);
			this.PopulateLists(this.selectedManagersList, this.availableManagersList, allManagerList, this.currentRule.ManagerList, NongroupPrefix, false);
			this.DetectSeparator(this.selectedManagersList);
			this.DetectSeparator(this.availableManagersList);

			// setting up products, the group then product
			ProductGroupCollectionClass allProductGroupList = FMChannelHelper.MakeCall<IProductGroups, ProductGroupCollectionClass>(
				x =>
				x.Enumerate(this.Security)
			);

			this.PopulateLists(this.selectedProductsList, this.availableProductsList, allProductGroupList, this.currentRule.ProductGroupList, GroupPrefix, true);
			ProductCollectionClass allProductList = FMChannelHelper.MakeCall<IProducts, ProductCollectionClass>(x => x.Enumerate(this.Security));
			foreach (ProductClass p in allProductList)
			{
				p.IdentityGuid = p.MasterRecordGuid;
			}
			this.PopulateLists(this.selectedProductsList, this.availableProductsList, allProductList, this.currentRule.ProductList, NongroupPrefix, false);
			this.DetectSeparator(this.selectedProductsList);
			this.DetectSeparator(this.availableProductsList);

			// setting up aliases
			TransactionAliasCollectionClass allTransactionAliasList = FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasCollectionClass>(
				x =>
				x.Enumerate(this.Security)
			);
			List<TransactionAliasClass> filteredList = allTransactionAliasList.FindAll(trxAlias => IsAllowedTransactionAlias(trxAlias));
			this.PopulateLists(this.selectedTransactionsList, this.availableTransactionsList, filteredList, this.currentRule.TransactionAliasList, NongroupPrefix, true);
			this.DetectSeparator(this.selectedManagersList);
			this.DetectSeparator(this.availableManagersList);

			// setting up owners, the group then company
			CompanyCollectionClass allOwnerList = FMChannelHelper.MakeCall<ICompanies, CompanyCollectionClass>(
				x =>
				x.EnumerateByRole(this.Security, COMPANY_ROLE.OWNER, false, true)
			);
			foreach (CompanyClass c in allOwnerList)
			{
				c.IdentityGuid = c.MasterRecordGuid;
			}
			this.PopulateLists(this.selectedOwnersList, this.availableOwnersList, allCompanyGroupList, this.currentRule.OwnerGroupList, GroupPrefix, true);
			this.PopulateLists(this.selectedOwnersList, this.availableOwnersList, allOwnerList, this.currentRule.OwnerList, NongroupPrefix, false);
			this.DetectSeparator(this.selectedOwnersList);
			this.DetectSeparator(this.availableOwnersList);

		}

		private void UpdateView()
		{
			// Simple controls			
			if (this.currentRule != null)
			{
				//Set the title label with a key field from the bound object appended
				this.titleLabel.Text = this.GetTitleLabelText(PageTitle, this.currentRule.ID);
				this.IDTextBox.Text = this.currentRule.ID;
				this.descriptionTextBox.Text = this.currentRule.Description;
				this.enabledCheckBox.Checked = this.currentRule.Enabled;
				this.defaultEOMCheckBox.Checked = this.currentRule.DefaultEOM;
				this.defaultNotesTextBox.Text = this.currentRule.DefaultNotes;
				this.distributionAliasDropDownList.SelectedValue = this.currentRule.TransactionAliasGuid.ToString();
				this.reasonCodeDropDownList.SelectedValue = this.currentRule.DefaultReasonCodeGuid.ToString();
			}

			// Complex controls - lists
			this.PopulateLists();
		}

		/// <summary>
		/// This method will determine which form to transaction back to.
		/// </summary>
		private void TransferToOriginatingForm()
		{
			this.Redirect(AutoDistributionRulesForm.PageUrl);
		}

		/// <summary>
		/// This method save the data from the screen to the cached object
		/// </summary>
		public void UpdateData()
		{
			this.currentRule.ID = this.IDTextBox.Text;
			this.currentRule.Description = this.descriptionTextBox.Text;
			this.currentRule.Enabled = this.enabledCheckBox.Checked;
			this.currentRule.DefaultEOM = this.defaultEOMCheckBox.Checked;
			this.currentRule.TransactionAliasGuid = FromStringToGuid(this.distributionAliasDropDownList.SelectedValue);
			this.currentRule.DefaultReasonCodeGuid = FromStringToGuid(this.reasonCodeDropDownList.SelectedValue);
			this.currentRule.DefaultNotes = this.defaultNotesTextBox.Text;

			// map data is saved when entities are selected/unselected.
		}

		/// <summary>
		/// Saving data...
		/// </summary>
		/// <returns></returns>
		private bool CommitData()
		{
			try
			{
				this.UpdateData();

				if (this.SessionHasErrors)
				{
					return false;
				}

				if (this.currentRule.IdentityGuid != Guid.Empty)
				{
					FMChannelHelper.MakeCall<IAutoDistributionRules>(x => x.Modify(this.Security, this.currentRule));
				}
				else
				{
					FMChannelHelper.MakeCall<IAutoDistributionRules, Guid>(x => x.Add(this.Security, this.currentRule));
				}

				this.MySessionObject = this.currentRule;
				return true;
			}
			catch (Exception error)
			{
				this.ErrorHandler(error);
				return false;
			}
		}

		/// <summary>
		/// Compare 2 strings and return compare results. They could be GroupPrefix.  Groups are "less" than Non-Groups
		/// </summary>
		/// <param name="src1"></param>
		/// <param name="src2"></param>
		/// <returns></returns>
		private int CompareSpecialStrings(string src1, string src2)
		{
			bool src1IsAGroup = this.IsStringAGroupString(src1);
			bool src2IsAGroup = this.IsStringAGroupString(src2);

			//  either both of the strings are groups or non-groups, just compare
			if (src1IsAGroup == src2IsAGroup)
			{
				return string.Compare(src1, src2, StringComparison.OrdinalIgnoreCase);
			}

			// one of the strings is a group;
			return src1IsAGroup ? -1 : 1;
		}

		/// <summary>
		/// Insert the given item to the given list at the right position.  It uses CompareSpecialStrings above.
		/// </summary>
		/// <param name="targetList"></param>
		/// <param name="theItem"></param>
		private void InsertItemToList(FMListBox targetList, ListItem theItem)
		{
			string searchText = theItem.Text;
			int insertAtIndex = targetList.Items.Count;
			for (int idx = 0; idx < targetList.Items.Count; idx++)
			{
				ListItem currentItem = targetList.Items[idx];
				string currentItemText = currentItem.Text;
				if (this.CompareSpecialStrings(currentItemText, searchText) >= 0)
				{
					insertAtIndex = idx;
					break;
				}
			}
			targetList.Items.Insert(insertAtIndex, theItem);
		}


		/// <summary>
		/// This assigns or unassigns an entity.  This updates the listbox and the session rule object
		/// </summary>
		/// <param name="srcList"></param>
		/// <param name="destList"></param>
		/// <param name="IsAssignment"></param>
		/// <param name="mapList"></param>
		/// <param name="groupMapList"></param>
		private void AssignEntity(FMListBox srcList, FMListBox destList, bool IsAssignment,
				AutoDistributionRuleMapDOCollection mapList, AutoDistributionRuleMapDOCollection groupMapList)
		{
			try
			{
				ListItem item;
				while ((item = srcList.SelectedItem) != null)
				{
					// this takes care of the listboxes on the screen
					srcList.Items.Remove(item);
					item.Selected = false;
					this.InsertItemToList(destList, item);

					// the following determine whether we should be updating the group or non-group map list
					AutoDistributionRuleMapDOCollection targetMapList = mapList;
					string itemValue = item.Value;
					if (groupMapList != null && this.IsStringAGroupString(itemValue))
					{
						targetMapList = groupMapList;
						itemValue = itemValue.Substring(GroupPrefix.Length);
					}

					Guid srcGuid = FromStringToGuid(itemValue);
					if (IsAssignment)
					{
						AutoDistributionRuleMapDO newMap = null;
						AutoDistributionRuleMapDO tempMap = new AutoDistributionRuleMapDO();
						tempMap.AssignedGuid = srcGuid;
						tempMap.AssigneeGuid = this.currentRule.IdentityGuid;

						// Try to ind the original map
						if ((this.originalMaps == null) ||  // originalMaps==null when it is a new rule
							(this.originalMaps[targetMapList].FindMap(tempMap, out newMap) == false))
						{
							newMap = tempMap;
						}
						targetMapList.Add(newMap);
					}
					else
					{
						targetMapList.RemoveByAssignedGuid(srcGuid);
					}
				}
				this.DetectSeparator(srcList);
				this.DetectSeparator(destList);

			}
			catch (Exception error)
			{
				this.ErrorHandler(error);
			}
		}
		private void AssignManagerButton_Click(object sender, EventArgs eventArgList)
		{
			this.AssignEntity(this.availableManagersList, this.selectedManagersList, true, this.currentRule.ManagerList, this.currentRule.ManagerGroupList);
		}
		private void UnassignManagerButton_Click(object sender, EventArgs eventArgList)
		{
			this.AssignEntity(this.selectedManagersList, this.availableManagersList, false, this.currentRule.ManagerList, this.currentRule.ManagerGroupList);
		}
		private void AssignOwnerButton_Click(object sender, EventArgs eventArgList)
		{
			this.AssignEntity(this.availableOwnersList, this.selectedOwnersList, true, this.currentRule.OwnerList, this.currentRule.OwnerGroupList);
		}
		private void UnassignOwnerButton_Click(object sender, EventArgs eventArgList)
		{
			this.AssignEntity(this.selectedOwnersList, this.availableOwnersList, false, this.currentRule.OwnerList, this.currentRule.OwnerGroupList);
		}
		private void AssignProductButton_Click(object sender, EventArgs eventArgList)
		{
			this.AssignEntity(this.availableProductsList, this.selectedProductsList, true, this.currentRule.ProductList, this.currentRule.ProductGroupList);
		}
		private void UnassignProductButton_Click(object sender, EventArgs eventArgList)
		{
			this.AssignEntity(this.selectedProductsList, this.availableProductsList, false, this.currentRule.ProductList, this.currentRule.ProductGroupList);
		}
		private void AssignTransactionButton_Click(object sender, EventArgs eventArgList)
		{
			this.AssignEntity(this.availableTransactionsList, this.selectedTransactionsList, true, this.currentRule.TransactionAliasList, null);
		}
		private void UnassignTransactionButton_Click(object sender, EventArgs eventArgList)
		{
			this.AssignEntity(this.selectedTransactionsList, this.availableTransactionsList, false, this.currentRule.TransactionAliasList, null);
		}
		#endregion
	}
}