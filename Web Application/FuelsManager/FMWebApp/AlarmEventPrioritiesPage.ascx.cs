/******************************************************************************
	FILE NAME:		AlarmEventPrioritiesPage.ascx.cs
	PURPOSE:		Implementation of AlarmEventPrioritiesPage

	COMMENTS:
		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2002
		This file shall not be copied or reproduced in any form without
		the express written consent of Endress+Hauser.

	AUTHOR(S):	W. Gray
	VERSION:	1.0.0  Current version

	MODIFICATION HISTORY:
		Date:		By:					Reason:
		----------	-----------------	-------------------------------------------
		2007-01-22	Richard Panachida	Added an override method to disable/enable controls. 
										Currently, it disables/enables the Add button (CSI 4083).
		2007-02-09	Richard Panachida	Added an override method to disable/enable controls. 
										Currently, it disables/enables the Add button (CSI 4083).
*******************************************************************************/
namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Drawing;
	using System.Web.UI.WebControls;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Interfaces;
	using FMBusinessObjects.UtilityObjects;

	using FMControls;

	public partial class AlarmEventPrioritiesPage : FMUserControlBase, IEntityDiscovery
	{
		ENTITY_TYPE IEntityDiscovery.EntityType
		{
			get
			{
				return ENTITY_TYPE.ALARM_PRIORITY;
			}
		}

		EntityToSiteMapCollectionClass IEntityDiscovery.EnumerateEntityMaps(SecurityClass Security, ENTITY_ASSIGNMENT_TYPE Type)
		{
			var alarmPriorityCollection = FMChannelHelper.MakeCall<IAlarmPriorities, AlarmPriorityCollectionClass>(
				alarmPriorities => alarmPriorities.Enumerate(Security));

			EntityToSiteMapCollectionClass EntityToSiteMapCollection = new EntityToSiteMapCollectionClass();

			foreach (AlarmPriorityClass AlarmPriority in alarmPriorityCollection)
			{
				if (Type == ENTITY_ASSIGNMENT_TYPE.ASSIGNED)
				{
					if (Security.SiteGuid == AlarmPriority.SiteGuid)
						continue;

					if (Security.LoginSiteGuid != AlarmPriority.SiteGuid)
						continue;
				}
				else
				{
					if (Security.SiteGuid != AlarmPriority.SiteGuid)
						continue;
				}

				EntityToSiteMapClass EntityToSiteMap = new EntityToSiteMapClass(AlarmPriority);
				EntityToSiteMapCollection.Add(EntityToSiteMap);
			}
			return EntityToSiteMapCollection;
		}

		Type IEntityDiscovery.EntityEngineType
		{
			get
			{
				return typeof(IAlarmPriorities);
			}
		}

		void IEntityDiscovery.SetSiteGuid(SecurityClass security, Guid guid, Guid SiteGuid)
		{
			FMChannelHelper.MakeCall<IAlarmPriorities>(
				alarmPriorities =>
				{
					AlarmPriorityClass alarmPriority = alarmPriorities.Get(security, guid);
					alarmPriority.SiteGuid = SiteGuid;
					alarmPriorities.Modify(security, alarmPriority);
				});
		}

		Guid IEntityDiscovery.GetIdentityGuid(SecurityClass security, string ID)
		{
			return FMChannelHelper.MakeCall<IAlarmPriorities, Guid>(
				alarmPriorities => alarmPriorities.GetIdentityGuid(security, ID));
		}

		bool IEntityDiscovery.EntityAssignable { get { return true; } }

		protected void UpdateView()
		{
			ICollection Priorities = this.EnumerateAlarmPriorities();

			this.AlarmPriorityPageSizeDropDown.SetPageSize(this.PrioritiesDataGrid, Priorities.Count);

			this.PrioritiesDataGrid.DataSource = Priorities;
			this.PrioritiesDataGrid.DataBind();
		}

		private ICollection EnumerateAlarmPriorities()
		{
			AlarmPriorityCollectionClass AlarmPriorityCollection;
			AlarmPriorityCollection = (AlarmPriorityCollectionClass)this.Session["AlarmPriorityCollection"];

			DataTable AlarmPriorityDataTable = new DataTable();
			DataRow AlarmPriorityDataRow;
			AlarmPriorityClass AlarmPriority;

			AlarmPriorityDataTable.Columns.Add("SiteGuid", typeof(Guid));
			AlarmPriorityDataTable.Columns.Add("Index", typeof(Int32));
			AlarmPriorityDataTable.Columns.Add("ID", typeof(string));
			AlarmPriorityDataTable.Columns.Add("SoundFile", typeof(string));
			AlarmPriorityDataTable.Columns.Add("Priority", typeof(string));

			for (int iItem = 0; iItem < AlarmPriorityCollection.Count; iItem++)
			{
				AlarmPriorityDataRow = AlarmPriorityDataTable.NewRow();

				AlarmPriority = (AlarmPriorityClass)AlarmPriorityCollection[iItem];
				AlarmPriorityDataRow["SiteGuid"] = AlarmPriority.SiteGuid;
				AlarmPriorityDataRow["Index"] = iItem;
				AlarmPriorityDataRow["ID"] = AlarmPriority.ID;
				AlarmPriorityDataRow["SoundFile"] = AlarmPriority.SoundFile;
				AlarmPriorityDataRow["Priority"] = (AlarmPriority.Priority.HasValue) ? AlarmPriority.Priority.ToString() : string.Empty;

				AlarmPriorityDataTable.Rows.Add(AlarmPriorityDataRow);
			}
			DataView AlarmPriorityDataView = new DataView(AlarmPriorityDataTable);
			return AlarmPriorityDataView;
		}

		public ListItemCollection EnumerateBackgroundColors()
		{
			ListItemCollection ListItems = new ListItemCollection();
			int Index = 0;
			foreach (KnownColor FMColor in Enum.GetValues(typeof(KnownColor)))
			{
				string name = Enum.GetName(typeof(KnownColor), FMColor);
				if (name.ToLower().Contains("highlight") ||
					name.ToLower().Contains("info") ||
					name.ToLower().Contains("window") ||
					name.ToLower().Contains("control") ||
					name.ToLower().Contains("appworkspace") ||
					name.ToLower().Contains("transparent") ||
					name.ToLower().Contains("desktop") ||
					name.ToLower().Contains("hottrack") ||
					name.ToLower().Contains("active") ||
					name.ToLower().Contains("menu") ||
					name.ToLower().Contains("scrollbar") ||
					name.ToLower().Contains("text")
					)
					continue;
				Color color = Color.FromKnownColor(FMColor);
				int invertedColor = Color.Black.ToArgb();
				if (color.G < 0x77)
					invertedColor = Color.White.ToArgb();

				ListItem Item = new ListItem("Background color: " + name, (Color.FromKnownColor(FMColor).ToArgb()).ToString("X06").Right(6));
				Item.Attributes.Add("style", "background-color: " + name + "; color: #" + invertedColor.ToString("X06").Right(6));
				ListItems.Add(Item);
				Index++;
			}

			return ListItems;
		}

		public ListItemCollection EnumerateTextColors()
		{
			ListItemCollection ListItems = new ListItemCollection();
			int Index = 0;
			foreach (KnownColor FMColor in Enum.GetValues(typeof(KnownColor)))
			{
				string name = Enum.GetName(typeof(KnownColor), FMColor);
				if (name.ToLower().Contains("highlight") ||
					name.ToLower().Contains("info") ||
					name.ToLower().Contains("window") ||
					name.ToLower().Contains("control") ||
					name.ToLower().Contains("appworkspace") ||
					name.ToLower().Contains("transparent") ||
					name.ToLower().Contains("desktop") ||
					name.ToLower().Contains("hottrack") ||
					name.ToLower().Contains("active") ||
					name.ToLower().Contains("menu") ||
					name.ToLower().Contains("scrollbar") ||
					name.ToLower().Contains("text")
					)
					continue;
				Color color = Color.FromKnownColor(FMColor);
				int invertedColor = Color.Black.ToArgb();
				if (color.G < 0x77)
					invertedColor = Color.White.ToArgb();
				ListItem Item = new ListItem("Text color: " + name, (Color.FromKnownColor(FMColor).ToArgb()).ToString("X06").Right(6));
				Item.Attributes.Add("style", "background-color: " + name + "; color: #" + invertedColor.ToString("X06").Right(6));
				ListItems.Add(Item);
				Index++;
			}

			return ListItems;
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				if (!this.Page.IsPostBack)
				{
					if (!this.Security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
					{
						this.AddButton.Enabled = false;
						this.AddButton2.Enabled = false;
					}

					var alarmPriorityCollection = FMChannelHelper.MakeCall<IAlarmPriorities, AlarmPriorityCollectionClass>(
						alarmPriorities => alarmPriorities.Enumerate(this.Security));

					this.Session["AlarmPriorityCollection"] = alarmPriorityCollection;

					this.UpdateView();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// This method enables and disables controls.
		/// </summary>
		/// <param name="enable"></param>
		protected void EnableControls(bool enable)
		{
			this.AddButton.Enabled = enable;
			this.AddButton2.Enabled = enable;

			// Call the main form to disable buttons and tabs.
			AlarmEventConfigurationForm alarmEventConfigurationForm = (AlarmEventConfigurationForm)this.Page;
			alarmEventConfigurationForm.EnableControls(enable);
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			this.InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.PrioritiesDataGrid.EditCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.PrioritiesDataGrid_EditCommand);
			this.PrioritiesDataGrid.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.PrioritiesDataGrid_PageIndexChanged);
			this.PrioritiesDataGrid.CancelCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.PrioritiesDataGrid_CancelCommand);
			this.PrioritiesDataGrid.UpdateCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.PrioritiesDataGrid_UpdateCommand);
			this.PrioritiesDataGrid.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.PrioritiesDataGrid_DeleteCommand);
			this.PrioritiesDataGrid.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.PrioritiesDataGrid_ItemDataBound);
			this.AddButton.Command += new System.Web.UI.WebControls.CommandEventHandler(this.AddButton_Command);
			this.AddButton2.Command += new System.Web.UI.WebControls.CommandEventHandler(this.AddButton_Command);

		}
		#endregion

		protected void PageSizeDropDown_SelectedIndexChanged(object source, System.EventArgs e)
		{
			this.UpdateView();
		}

		protected void AddButton_Command(object sender, System.Web.UI.WebControls.CommandEventArgs e)
		{
			AlarmPriorityCollectionClass AlarmPriorityCollection;
			AlarmPriorityCollection = (AlarmPriorityCollectionClass)this.Session["AlarmPriorityCollection"];
			AlarmPriorityClass AlarmPriority = new AlarmPriorityClass();
			AlarmPriorityCollection.Add(AlarmPriority);
			this.PrioritiesDataGrid.CurrentPageIndex = (AlarmPriorityCollection.Count - 1) / this.PrioritiesDataGrid.PageSize;
			this.PrioritiesDataGrid.EditItemIndex = (AlarmPriorityCollection.Count - 1) % this.PrioritiesDataGrid.PageSize;

			this.EnableControls(false);
			this.UpdateView();
		}

		protected void PrioritiesDataGrid_CancelCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			Label IndexLabel = (Label)e.Item.FindControl("IndexLabel");

			if (IndexLabel != null)
			{
				AlarmPriorityCollectionClass AlarmPriorityCollection;
				AlarmPriorityCollection = (AlarmPriorityCollectionClass)this.Session["AlarmPriorityCollection"];
				AlarmPriorityClass AlarmPriority;
				AlarmPriority = (AlarmPriorityClass)AlarmPriorityCollection[System.Convert.ToInt32(IndexLabel.Text)];

				if (AlarmPriority.IdentityGuid == Guid.Empty)
				{
					AlarmPriorityCollection.RemoveAt(System.Convert.ToInt32(IndexLabel.Text));

					if ((this.PrioritiesDataGrid.Items.Count == 1) && (this.PrioritiesDataGrid.CurrentPageIndex > 0))
					{
						this.PrioritiesDataGrid.CurrentPageIndex--;
					}
				}

				this.PrioritiesDataGrid.EditItemIndex = -1;
				this.EnableControls(true);
				this.UpdateView();
			}
		}

		protected void PrioritiesDataGrid_DeleteCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			try
			{
				Label IndexLabel = (Label)e.Item.FindControl("IndexLabel");
				if (IndexLabel != null)
				{
					AlarmPriorityCollectionClass AlarmPriorityCollection;
					AlarmPriorityCollection = (AlarmPriorityCollectionClass)this.Session["AlarmPriorityCollection"];

					AlarmPriorityClass AlarmPriority;
					AlarmPriority = (AlarmPriorityClass)AlarmPriorityCollection[System.Convert.ToInt32(IndexLabel.Text)];

					if (this.PrioritiesDataGrid.EditItemIndex == e.Item.ItemIndex)
					{
						this.PrioritiesDataGrid.EditItemIndex = -1;
						this.EnableControls(true);
					}
					else if (this.PrioritiesDataGrid.EditItemIndex > e.Item.ItemIndex)
						this.PrioritiesDataGrid.EditItemIndex--;


					// Non empty guid indicates AlarmPriority has been committed to database
					if (AlarmPriority.IdentityGuid != Guid.Empty)
					{
						FMChannelHelper.MakeCall<IAlarmPriorities>(
							alarmPriorities => alarmPriorities.Purge(this.Security, AlarmPriority.IdentityGuid));
					}

					AlarmPriorityCollection.RemoveAt(System.Convert.ToInt32(IndexLabel.Text));
					if (this.PrioritiesDataGrid.Items.Count == 1
					&& this.PrioritiesDataGrid.CurrentPageIndex > 0)
						this.PrioritiesDataGrid.CurrentPageIndex--;
					this.UpdateView();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void PrioritiesDataGrid_EditCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			this.EnableControls(false);
			this.PrioritiesDataGrid.EditItemIndex = e.Item.ItemIndex;
			this.UpdateView();
		}

		protected void PrioritiesDataGrid_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			// if we are editing do not allow a page change
			if (this.PrioritiesDataGrid.EditItemIndex > -1)
				return;
			this.PrioritiesDataGrid.CurrentPageIndex = e.NewPageIndex;
			this.UpdateView();
		}

		protected void PrioritiesDataGrid_UpdateCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			try
			{
				Label IndexLabel = (Label)e.Item.FindControl("IndexLabel");

				if (IndexLabel != null)
				{
					AlarmPriorityCollectionClass AlarmPriorityCollection;
					AlarmPriorityCollection = (AlarmPriorityCollectionClass)this.Session["AlarmPriorityCollection"];
					AlarmPriorityClass AlarmPriority;
					AlarmPriority = AlarmPriorityCollection[System.Convert.ToInt32(IndexLabel.Text)];

					TextBox IDTextBox = (TextBox)e.Item.FindControl("IDTextBox");
					AlarmPriority.ID = IDTextBox.Text;

					FMDropDownList BackgroundSteadyDropDownList = (FMDropDownList)e.Item.FindControl("BackgroundSteadyDropDownList");
					AlarmPriority.BackgroundSteady = BackgroundSteadyDropDownList.Items[BackgroundSteadyDropDownList.SelectedIndex].Value;

					FMDropDownList TextSteadyDropDownList = (FMDropDownList)e.Item.FindControl("TextSteadyDropDownList");
					AlarmPriority.TextSteady = TextSteadyDropDownList.Items[TextSteadyDropDownList.SelectedIndex].Value;

					FMDropDownList BackgroundAlternateDropDownList = (FMDropDownList)e.Item.FindControl("BackgroundAlternateDropDownList");
					AlarmPriority.BackgroundAlternate = BackgroundAlternateDropDownList.Items[BackgroundAlternateDropDownList.SelectedIndex].Value;

					FMDropDownList TextAlternateDropDownList = (FMDropDownList)e.Item.FindControl("TextAlternateDropDownList");
					AlarmPriority.TextAlternate = TextAlternateDropDownList.Items[TextAlternateDropDownList.SelectedIndex].Value;

					TextBox SoundTextBox = (TextBox)e.Item.FindControl("SoundFileTextBox");
					AlarmPriority.SoundFile = SoundTextBox.Text;

					TextBox PriorityTextBox = (TextBox)e.Item.FindControl("PriorityTextBox");
					AlarmPriority.Priority = (string.IsNullOrEmpty(PriorityTextBox.Text)) ? new byte?() : System.Convert.ToByte(PriorityTextBox.Text);

                    // we need to refresh the screen before calling the save because if there is an exception in the call (like missing ID) the dropdowns will not be correct
                    this.EnableControls(true);
                    this.UpdateView();

                    FMChannelHelper.MakeCall<IAlarmPriorities>(
						alarmPriorities =>
						{
							if (AlarmPriority.IdentityGuid == Guid.Empty)
							{
								AlarmPriority.IdentityGuid = alarmPriorities.Add(this.Security, AlarmPriority);
								AlarmPriority.SiteGuid = this.Security.SiteGuid;
							}
							else
							{
								alarmPriorities.Modify(this.Security, AlarmPriority);
							}
						});

					this.EnableControls(true);
					this.PrioritiesDataGrid.EditItemIndex = -1;
					this.UpdateView();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void PrioritiesDataGrid_ItemDataBound(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
		{
			LinkButton EditButton = (LinkButton)e.Item.FindControl("EditButton");
			LinkButton DeleteButton = (LinkButton)e.Item.FindControl("DeleteButton");
			Label SiteGuidLabel = (Label)e.Item.FindControl("SiteGuidLabel");

			if (EditButton != null
			&& DeleteButton != null
			&& SiteGuidLabel != null)
			{
				if (!this.Security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS)
				|| this.Security.SiteGuid != Guid.Parse(SiteGuidLabel.Text))
				{
					EditButton.Enabled = false;
					EditButton.Text = "<img src=Images/Edit_un.gif border=0 align=absmiddle alt='Edit this item'>";
					DeleteButton.Enabled = false;
					DeleteButton.Text = "<img src=Images/Delete_un.gif border=0 align=absmiddle alt='Delete this item'>";
				}
			}
			Label IndexLabel = (Label)e.Item.FindControl("IndexLabel");
			if (IndexLabel != null)
			{
				AlarmPriorityCollectionClass AlarmPriorityCollection;
				AlarmPriorityCollection = (AlarmPriorityCollectionClass)this.Session["AlarmPriorityCollection"];
				AlarmPriorityClass AlarmPriority;
				AlarmPriority = AlarmPriorityCollection[System.Convert.ToInt32(IndexLabel.Text)];

				FMDropDownList BackgroundSteadyDropDownList = (FMDropDownList)e.Item.FindControl("BackgroundSteadyDropDownList");
				if (BackgroundSteadyDropDownList != null)
				{
					foreach (ListItem Item in this.EnumerateBackgroundColors())
						BackgroundSteadyDropDownList.Items.Add(Item);

					BackgroundSteadyDropDownList.SelectedValue = AlarmPriority.BackgroundSteady.ToString();
					BackgroundSteadyDropDownList.BackColor = new WebColorClass(AlarmPriority.BackgroundSteady).Color;
					BackgroundSteadyDropDownList.ForeColor = BackgroundSteadyDropDownList.BackColor.G < 0x77 ? Color.White : Color.Black;

					ListItem findItem = BackgroundSteadyDropDownList.Items.FindByValue(AlarmPriority.BackgroundSteady);
					if (findItem != null)
					{
						BackgroundSteadyDropDownList.SelectedValue = findItem.Value;
					}
					else
					{
						BackgroundSteadyDropDownList.SelectedIndex = -1;
					}
					BackgroundSteadyDropDownList.SelectedValue = AlarmPriority.BackgroundSteady;
				}

				FMDropDownList TextSteadyDropDownList = (FMDropDownList)e.Item.FindControl("TextSteadyDropDownList");
				if (TextSteadyDropDownList != null)
				{
					foreach (ListItem Item in this.EnumerateTextColors())
						TextSteadyDropDownList.Items.Add(Item);

					TextSteadyDropDownList.SelectedValue = AlarmPriority.TextSteady.ToString();
					TextSteadyDropDownList.BackColor = new WebColorClass(AlarmPriority.TextSteady).Color;
					TextSteadyDropDownList.ForeColor = TextSteadyDropDownList.BackColor.G < 0x77 ? Color.White : Color.Black;

					ListItem findItem = TextSteadyDropDownList.Items.FindByValue(AlarmPriority.TextSteady);
					if (findItem != null)
					{
						TextSteadyDropDownList.SelectedValue = findItem.Value;
					}
					else
					{
						TextSteadyDropDownList.SelectedIndex = -1;
					}
				}

				FMDropDownList BackgroundAlternateDropDownList = (FMDropDownList)e.Item.FindControl("BackgroundAlternateDropDownList");
				if (BackgroundAlternateDropDownList != null)
				{
					foreach (ListItem Item in this.EnumerateBackgroundColors())
						BackgroundAlternateDropDownList.Items.Add(Item);

					BackgroundAlternateDropDownList.SelectedValue = AlarmPriority.BackgroundAlternate.ToString();
					BackgroundAlternateDropDownList.BackColor = new WebColorClass(AlarmPriority.BackgroundAlternate).Color;
					BackgroundAlternateDropDownList.ForeColor = BackgroundAlternateDropDownList.BackColor.G < 0x77 ? Color.White : Color.Black;

                    ListItem findItem = BackgroundAlternateDropDownList.Items.FindByValue(AlarmPriority.BackgroundAlternate);
					if (findItem != null)
					{
						BackgroundAlternateDropDownList.SelectedValue = findItem.Value;
					}
					else
					{
						BackgroundAlternateDropDownList.SelectedIndex = -1;
					}
				}

				FMDropDownList TextAlternateDropDownList = (FMDropDownList)e.Item.FindControl("TextAlternateDropDownList");
				if (TextAlternateDropDownList != null)
				{
					foreach (ListItem Item in this.EnumerateTextColors())
						TextAlternateDropDownList.Items.Add(Item);

					TextAlternateDropDownList.SelectedValue = AlarmPriority.TextAlternate.ToString();
					TextAlternateDropDownList.BackColor = new WebColorClass(AlarmPriority.TextAlternate).Color;
					TextAlternateDropDownList.ForeColor = TextAlternateDropDownList.BackColor.G < 0x77 ? Color.White : Color.Black;

                    ListItem findItem = TextAlternateDropDownList.Items.FindByValue(AlarmPriority.TextAlternate);
					if (findItem != null)
					{
						TextAlternateDropDownList.SelectedValue = findItem.Value;
					}
					else
					{
						TextAlternateDropDownList.SelectedIndex = -1;
					}
				}

				Label SteadyLabel = (Label)e.Item.FindControl("SteadyLabel");
				if (SteadyLabel != null)
				{
					SteadyLabel.BackColor = new WebColorClass(AlarmPriority.BackgroundSteady).Color;
					SteadyLabel.ForeColor = new WebColorClass(AlarmPriority.TextSteady).Color;
				}

				Label SteadyColorLabel = (Label)e.Item.FindControl("SteadyColorLabel");
				if (SteadyColorLabel != null)
				{
                    var steadybackgroundcolor = EnumerateBackgroundColors().FindByValue(AlarmPriority.BackgroundSteady);
                    var steadytextcolor = EnumerateTextColors().FindByValue(AlarmPriority.TextSteady);

                    SteadyColorLabel.Text = (steadybackgroundcolor != null ? steadybackgroundcolor.Text : "Background color: " + SteadyLabel.BackColor.Name) + "<br/>" +
                                     (steadytextcolor != null ? steadytextcolor.Text : "Text color: " + SteadyLabel.ForeColor.Name);
				}

				Label AlternateLabel = (Label)e.Item.FindControl("AlternateLabel");
				if (AlternateLabel != null)
				{
					AlternateLabel.BackColor = new WebColorClass(AlarmPriority.BackgroundAlternate).Color;
					AlternateLabel.ForeColor = new WebColorClass(AlarmPriority.TextAlternate).Color;
				}

				Label AlternateColorLabel = (Label)e.Item.FindControl("AlternateColorLabel");
				if (AlternateColorLabel != null)
				{
                    var alternatebackgroundcolor = EnumerateBackgroundColors().FindByValue(AlarmPriority.BackgroundAlternate);
                    var alternatetextcolor = EnumerateTextColors().FindByValue(AlarmPriority.TextAlternate);

                    AlternateColorLabel.Text = (alternatebackgroundcolor != null ? alternatebackgroundcolor.Text : "Background color: " + AlternateLabel.BackColor.Name) + "<br/>" +
                                     (alternatetextcolor != null ? alternatetextcolor.Text : "Text color: " + AlternateLabel.ForeColor.Name);

				}
			}
		}
	}
}
