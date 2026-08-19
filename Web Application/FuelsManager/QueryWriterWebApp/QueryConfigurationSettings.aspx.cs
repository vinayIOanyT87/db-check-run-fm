// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueryConfigurationSettings.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the QueryConfigurationSettings type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.QueryWriterWebApp
{
	using System;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Interfaces;

	using FuelsManager.FMWebApp;


	/// <summary>
	/// The control code behind class for the QueryConfigurationSettings form.
	/// </summary>
	public partial class QueryConfigurationSettings : FMFormBaseAjax, IEntityDiscovery
	{
		#region Constants and Fields

		/// <summary>
		/// The entity name for assignment and ownership.
		/// </summary>
		private const string EntityName = "Default Fields and Settings";

		/// <summary>
		/// The session key for the default field collection.
		/// </summary>
		private const string QuerywriterDefaultFieldCollection = "QueryWriter.DefaultFieldCollection";

		/// <summary>
		/// The session key for the default header footer object.
		/// </summary>
		private const string QuerywriterDefaultHeaderFooter = "QueryWriter.DefaultHeaderFooter";

		/// <summary>
		/// The default field collection.
		/// </summary>
		private QueryDefaultFieldCollectionClass defaultFieldCollection;

		/// <summary>
		/// The default header footer object.
		/// </summary>
		private QueryDefaultClass defaultHeaderFooter;

		/// <summary>
		/// Flag indicating if query settings are assigned from another site.
		/// </summary>
		private bool querySettingsAssigned;

		#endregion

		#region Explicit Interface Properties

		/// <summary>
		/// Gets a value indicating whether [entity assignable].
		/// </summary>
		/// <value>
		///   <c>true</c> if [entity assignable]; otherwise, <c>false</c>.
		/// </value>
		bool IEntityDiscovery.EntityAssignable
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Gets the type of the entity engine.
		/// </summary>
		/// <value>
		/// The type of the entity engine.
		/// </value>
		Type IEntityDiscovery.EntityEngineType
		{
			get
			{
				return typeof(IQueryDefaults);
			}
		}

		/// <summary>
		/// Gets the type of the entity.
		/// </summary>
		/// <value>
		/// The type of the entity.
		/// </value>
		ENTITY_TYPE IEntityDiscovery.EntityType
		{
			get
			{
				return ENTITY_TYPE.QUERY_DEFAULT_FIELD;
			}
		}

		#endregion

		#region Explicit Interface Methods

		/// <summary>

		/// <summary>
		/// Enumerates the entity maps.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="type">The type.</param>
		/// <returns>An entity to site map collection.</returns>
		EntityToSiteMapCollectionClass IEntityDiscovery.EnumerateEntityMaps(
			SecurityClass security, ENTITY_ASSIGNMENT_TYPE type)
		{
			var entityToSiteMapCollection = new EntityToSiteMapCollectionClass();

			if (type == ENTITY_ASSIGNMENT_TYPE.OWNED)
			{
			}
			else
			{
				var entityToSiteMap =
					FMChannelHelper.MakeCall<IEntityToSiteMaps, EntityToSiteMapClass>(
						x => x.Get(security, ((IEntityDiscovery)this).EntityType, security.LoginSiteGuid));

				if (type == ENTITY_ASSIGNMENT_TYPE.ASSIGNED)
				{
					if (security.LoginSiteGuid == entityToSiteMap.IdentityGuid)
					{
						entityToSiteMap.ID = EntityName;
						entityToSiteMapCollection.Add(entityToSiteMap);
					}
				}
				else
				{
					if (entityToSiteMap.IdentityGuid == Guid.Empty)
					{
						entityToSiteMap = new EntityToSiteMapClass
							{
								SiteGuid = Guid.Empty,
								ID = EntityName,
								TypeID = ((IEntityDiscovery)this).EntityType,
								IdentityGuid = security.SiteGuid
							};

						entityToSiteMapCollection.Add(entityToSiteMap);
					}
				}
			}

			return entityToSiteMapCollection;
		}

		/// <summary>
		/// Gets the identity GUID.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="id">The id.</param>
		/// <returns>The Identity Guid of the object.</returns>
		Guid IEntityDiscovery.GetIdentityGuid(SecurityClass security, string id)
		{
			var entityToSiteMap =
				FMChannelHelper.MakeCall<IEntityToSiteMaps, EntityToSiteMapClass>(
					x => x.Get(security, ((IEntityDiscovery)this).EntityType, security.LoginSiteGuid));

			return (entityToSiteMap.IdentityGuid == Guid.Empty) ? security.SiteGuid : entityToSiteMap.IdentityGuid;
		}

		/// <summary>
		/// Sets the site GUID.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="guid">The GUID.</param>
		/// <param name="siteGuid">The site GUID.</param>
		void IEntityDiscovery.SetSiteGuid(SecurityClass security, Guid guid, Guid siteGuid)
		{
			Guid originalSiteGuid = security.SiteGuid;
			try
			{
				// Need to purge any DataDictionary Assignments
				var field = new QueryDefaultFieldClass();

				FMChannelHelper.MakeCall<IEntityToSiteMaps>(x => this.PurgeEntityMaps(x, security, originalSiteGuid, field));

				FMChannelHelper.MakeCall<IQueryDefaultFields>(x => this.SetQueryFieldGuid(x, security, siteGuid));

				FMChannelHelper.MakeCall<IQueryDefaults>(x => this.SetQueryDefaultsGuid(x, security, siteGuid));
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
			finally
			{
				security.SiteGuid = originalSiteGuid;
			}
		}

		/// <summary>
		/// Sets the query defaults GUID.
		/// </summary>
		/// <param name="queryDefaults">The query defaults.</param>
		/// <param name="security">The security.</param>
		/// <param name="siteGuid">The site GUID.</param>
		protected void SetQueryDefaultsGuid(IQueryDefaults queryDefaults, SecurityClass security, Guid siteGuid)
		{
			var defaultClass = queryDefaults.EnumerateBySite(security);

			defaultClass.SiteGuid = siteGuid;
			queryDefaults.Modify(security, defaultClass);
		}

		/// <summary>
		/// Sets the query field GUID.
		/// </summary>
		/// <param name="queryDefaultFields">The query default fields.</param>
		/// <param name="security">The security.</param>
		/// <param name="siteGuid">The site GUID.</param>
		protected void SetQueryFieldGuid( IQueryDefaultFields queryDefaultFields, SecurityClass security, Guid siteGuid )
		{
			QueryDefaultFieldCollectionClass fieldCollection = queryDefaultFields.EnumerateBySite(security);

			foreach (QueryDefaultFieldClass queryField in fieldCollection)
			{
				queryField.SiteGuid = siteGuid;
				queryDefaultFields.Modify(security, queryField);
			}
		}

		/// <summary>
		/// Purges the entity maps.
		/// </summary>
		/// <param name="entityToSiteMaps">The entity to site maps.</param>
		/// <param name="security">The security.</param>
		/// <param name="originalSiteGuid">The original site GUID.</param>
		/// <param name="field">The field.</param>
		protected void PurgeEntityMaps(IEntityToSiteMaps entityToSiteMaps, SecurityClass security, Guid originalSiteGuid, QueryDefaultFieldClass field)
		{
			EntityToSiteMapCollectionClass entityToSiteMapCollection = entityToSiteMaps.EnumerateByTypeIDAndGuid(
				security, field.EntityType, originalSiteGuid);

			foreach (EntityToSiteMapClass entityToSiteMap in entityToSiteMapCollection)
			{
				entityToSiteMaps.Purge(security, entityToSiteMap);
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Adds to default collection.
		/// </summary>
		/// <param name="availableItem">The available item.</param>
		protected void AddToDefaultCollection(ListItem availableItem)
		{
			if (this.DefaultFieldExists(availableItem.Value))
			{
				throw new ApplicationException("Default query field already exists.");
			}

			// Get a list of QueryWriterFields and figure out which one we want
			QueryWriterTopic topic = FMChannelHelper.MakeCall<IQueryWriterTopics, QueryWriterTopic>(x => x.Get(this.Security, this.QueryTypeDropDown.SelectedValue));
			QueryWriterFieldCollection fields = topic.GetFields(this.Security, true);

			QueryWriterField field = fields.Get(availableItem.Value);

			// Create a QueryDefaultFieldClass
			var defaultField = new QueryDefaultFieldClass(field);

			// Add the QueryDefaultField object to our list of assigned fields
			this.defaultFieldCollection.Add(defaultField);
		}

		/// <summary>
		/// Handles the Click event of the ApplyButton control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void ApplyButtonClick(object sender, EventArgs e)
		{
			try
			{
				this.Save();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Handles the Click event of the AssignButton control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void AssignButtonClick(object sender, EventArgs e)
		{
			try
			{
				ListItem availableItem;
				while ((availableItem = this.AvailableFieldsList.SelectedItem) != null)
				{
					this.AvailableFieldsList.Items.Remove(availableItem);
					availableItem.Selected = false;

					this.SelectedFieldsList.Items.Add(availableItem);

					this.AddToDefaultCollection(availableItem);
				}

				this.SetOrderSequence();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Returns true if the Query Default Field exists in the list of assigned fields
		/// </summary>
		/// <param name="id">
		/// A QueryWriterField.id string value 
		/// </param>
		/// <returns>
		/// The System.Boolean.
		/// </returns>
		protected bool DefaultFieldExists(string id)
		{
			foreach (QueryDefaultFieldClass field in this.defaultFieldCollection)
			{
				if (field.ID == id)
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Loads the query types.
		/// </summary>
		protected void LoadQueryTypes()
		{
			this.QueryTypeDropDown.Items.Clear();

			QueryWriterTopicCollection topics = FMChannelHelper.MakeCall<IQueryWriterTopics, QueryWriterTopicCollection>(x => x.Enumerate(this.Security));

			foreach (QueryWriterTopic topic in topics)
			{
				this.QueryTypeDropDown.Items.Add(new ListItem(topic.DisplayName, topic.ObjectType.ToString()));
			}

			// Get the saved set of default query fields
			this.defaultFieldCollection =
				FMChannelHelper.MakeCall<IQueryDefaultFields, QueryDefaultFieldCollectionClass>(
					defaults => defaults.Enumerate(this.Security));

			this.Session.Add(QuerywriterDefaultFieldCollection, this.defaultFieldCollection);

			if (this.QueryTypeDropDown.Items.Count > 0)
			{
				this.QueryTypeDropDown.SelectedIndex = 0;
				this.QueryTypeDropDownSelectedIndexChanged(null, null);
			}
		}

		/// <summary>
		/// Loads the selected list box.
		/// </summary>
		/// <param name="topic">The topic.</param>
		protected void LoadSelectedListBox(QueryWriterTopic topic)
		{
			QueryWriterFieldCollection fieldCollection = topic.GetFields(this.Security, true);

			foreach (QueryDefaultFieldClass field in this.defaultFieldCollection)
			{
				if (field.Topic == topic.ObjectType.ToString())
				{
					QueryWriterField fieldAttribute = fieldCollection.Get(field.ID);

					if (fieldAttribute != null)
					{
						this.SelectedFieldsList.Items.Add(NewListItem(fieldAttribute));
					}
					else
					{
						string message = "Unknown query field configured: " + field.ID;
						
						FMChannelHelper.MakeCall<IFMEventLog>(
							x => x.WriteEntry(message, FMEventLogEntryType.Error));
					}
				}
			}
		}

		/// <summary>
		/// Raises the <see cref="OnInit"/> event.
		/// </summary>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			this.InitializeComponent();
		}

		/// <summary>
		/// Handles the Load event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				if (this.IsPostBack == false)
				{
					var queryDefault = new QueryDefaultClass();

					EntityToSiteMapCollectionClass entityToSiteMapCollection =
						FMChannelHelper.MakeCall<IEntityToSiteMaps, EntityToSiteMapCollectionClass>(
							x => x.EnumerateByTypeIDAndSiteGuid(this.Security, queryDefault.EntityType, this.Security.SiteGuid));

					if (entityToSiteMapCollection.Count != 0)
					{
						this.querySettingsAssigned = true;
					}

					this.LoadQueryTypes();
					this.UpdateView();
					this.SetButtonDefaults();
				}
				else
				{
					this.defaultFieldCollection = (QueryDefaultFieldCollectionClass)this.Session[QuerywriterDefaultFieldCollection];
					this.defaultHeaderFooter = (QueryDefaultClass)this.Session[QuerywriterDefaultHeaderFooter];
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Queries the type drop down selected index changed.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void QueryTypeDropDownSelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				this.SetOrderSequence();

				// Clear the list boxes
				this.SelectedFieldsList.Items.Clear();
				this.AvailableFieldsList.Items.Clear();

				// Load the already configured items into the selected fields list box
				QueryWriterTopic topic = FMChannelHelper.MakeCall<IQueryWriterTopics, QueryWriterTopic>(x => x.Get(this.Security, this.QueryTypeDropDown.SelectedValue));
				this.LoadSelectedListBox(topic);

				// Load the available fields list box with the fields that are left
				QueryWriterFieldCollection fieldCollection = topic.GetFields(this.Security, true);

				foreach (QueryWriterField field in fieldCollection)
				{
					if (this.DefaultFieldExists(field.ID) == false)
					{
						this.AvailableFieldsList.Items.Add(NewListItem(field));
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Removes the button click.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void RemoveButtonClick(object sender, EventArgs e)
		{
			try
			{
				ListItem selectedItem;
				while ((selectedItem = this.SelectedFieldsList.SelectedItem) != null)
				{
					this.SelectedFieldsList.Items.Remove(selectedItem);
					selectedItem.Selected = false;

					this.AvailableFieldsList.Items.Add(selectedItem);

					this.RemoveFromDefaultCollection(selectedItem);
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Removes item from default collection.
		/// </summary>
		/// <param name="item">The item.</param>
		protected void RemoveFromDefaultCollection(ListItem item)
		{
			for (int index = 0; index < this.defaultFieldCollection.Count; ++index)
			{
				if (this.defaultFieldCollection[index].ID == item.Value)
				{
					this.defaultFieldCollection.RemoveAt(index);
					break;
				}
			}
		}

		/// <summary>
		/// Saves this instance.
		/// </summary>
		protected void Save()
		{
			try
			{
				this.SetOrderSequence();

				// Save the values
				this.defaultHeaderFooter.Header = this.HeaderTextBox.Value;
				this.defaultHeaderFooter.Footer = this.FooterTextBox.Value;
				FMChannelHelper.MakeCall<IQueryDefaults>(x => x.Update(this.Security, this.defaultHeaderFooter));

				// Save default field settings
				FMChannelHelper.MakeCall<IQueryDefaultFields>(x => x.Update(this.Security, this.defaultFieldCollection));
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Sets the button defaults.
		/// </summary>
		protected void SetButtonDefaults()
		{
			bool enabled = (this.querySettingsAssigned == false) && this.Security.HasRight(RIGHT.CONFIGURE_QUERIES);

			this.ApplyButton.Enabled = enabled;
			this.AssignButton.Enabled = enabled;
			this.RemoveButton.Enabled = enabled;
		}

		/// <summary>
		/// Sets the order sequence.
		/// </summary>
		protected void SetOrderSequence()
		{
			for (int index = 0; index < this.SelectedFieldsList.Items.Count; ++index)
			{
				string itemID = this.SelectedFieldsList.Items[index].Value;
				this.SetOrderValue(itemID, index);
			}
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		protected void UpdateView()
		{
			try
			{
				this.defaultHeaderFooter =
					FMChannelHelper.MakeCall<IQueryDefaults, QueryDefaultClass>(x => x.Enumerate(this.Security));

				// Get Header & Footer from Site Object
				this.HeaderTextBox.Value = this.defaultHeaderFooter.Header;
				this.FooterTextBox.Value = this.defaultHeaderFooter.Footer;

				this.Session[QuerywriterDefaultHeaderFooter] = this.defaultHeaderFooter;
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Creates a new list item.
		/// </summary>
		/// <param name="field">The field.</param>
		/// <returns>A new list item.</returns>
		private static ListItem NewListItem(QueryWriterField field)
		{
			var newItem = new ListItem(field.DisplayName, field.ID);
			newItem.Attributes.Add("title", field.DisplayName);
			return newItem;
		}

		/// <summary>
		/// Initializes the component.
		/// </summary>
		private void InitializeComponent()
		{
			this.ApplyButton.Click += this.ApplyButtonClick;
			this.QueryTypeDropDown.SelectedIndexChanged += this.QueryTypeDropDownSelectedIndexChanged;
			this.AssignButton.Click += this.AssignButtonClick;
			this.RemoveButton.Click += this.RemoveButtonClick;
			this.MoveUpButton.Click += this.MoveUpButtonClick;
			this.MoveDownButton.Click += this.MoveDownButtonClick;
		}

		/// <summary>
		/// Move down button click.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void MoveDownButtonClick(object sender, EventArgs e)
		{
			try
			{
				for (int index = this.SelectedFieldsList.Items.Count - 2; index >= 0; --index)
				{
					if (this.SelectedFieldsList.Items[index].Selected)
					{
						this.SelectedFieldsList.Swap(index + 1, index);
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Move up button click.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void MoveUpButtonClick(object sender, EventArgs e)
		{
			try
			{
				for (int index = 1; index < this.SelectedFieldsList.Items.Count; ++index)
				{
					if (this.SelectedFieldsList.Items[index].Selected)
					{
						this.SelectedFieldsList.Swap(index, index - 1);
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Sets the order value.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <param name="sequenceNumber">The sequence number.</param>
		private void SetOrderValue(string id, int sequenceNumber)
		{
			foreach (QueryDefaultFieldClass field in this.defaultFieldCollection)
			{
				if (field.ID == id)
				{
					field.Order = sequenceNumber;
					break;
				}
			}
		}

		#endregion
	}
}