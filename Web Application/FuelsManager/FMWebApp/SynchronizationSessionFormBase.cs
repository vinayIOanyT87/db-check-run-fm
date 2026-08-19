using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Web.UI.WebControls;

using FMBusinessObjects.DataObjects;

using FMControls;

namespace FuelsManager.FMWebApp
{
	public abstract class SynchronizationSessionFormBase : FMAutoSubmitFormBase
	{
		#region Constants and Fields

		#endregion Constants and Fields

		#region Properties

		/// <summary>
		/// Gets the application data grid.
		/// </summary>
		protected virtual DataGrid ApplicationDataGrid
		{
			get
			{
				return null;
			}
		}

		#endregion Properties

		#region Page Events and Overrides

		/// <summary>
		/// The on init.
		/// </summary>
		/// <param name="e">
		/// The e.
		/// </param>
		protected override void OnInit(EventArgs e)
		{
			this.InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		/// The synchronization session form base load.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		protected void SynchronizationSessionFormBaseLoad(object sender, EventArgs e)
		{
			if (this.IsPostBack == false)
			{
				this.SetPageFocus();
			}
		}

		/// <summary>
		/// The set page focus.
		/// </summary>
		protected void SetPageFocus()
		{
			const string Script = "<script language=\"jscript\">\n" + "var refreshBtn=document.getElementById(\"RefreshButton\");\n"
								  + "if(refreshBtn != undefined && !refreshBtn.disabled)\n" + "refreshBtn.focus();\n" + "</script>\n";

			this.Page.ClientScript.RegisterStartupScript(this.GetType(), "page_set_focus", Script);
		}

		/// <summary>
		/// The initialize component.
		/// </summary>
		private void InitializeComponent()
		{
			this.Load += this.SynchronizationSessionFormBaseLoad;
		}

		#endregion Page Events and Overrides

		#region Control Events

		/// <summary>
		/// The application data grid_ page index changed.
		/// </summary>
		/// <param name="source">
		/// The source.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		protected void ApplicationDataGridPageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			// if we are editing do not allow a page change
			if (this.ApplicationDataGrid.EditItemIndex > -1)
			{
				return;
			}

			this.ApplicationDataGrid.CurrentPageIndex = e.NewPageIndex;
			this.UpdateView();
		}

		#endregion Control Events

		#region Methods and Operators

		/// <summary>
		/// The enable controls.
		/// </summary>
		/// <param name="enable">
		/// The enable.
		/// </param>
		protected virtual void EnableControls(bool enable)
		{
		}

		/// <summary>
		/// The update view.
		/// </summary>
		protected virtual void UpdateView()
		{
			this.UpdateView(null);
		}

		/// <summary>
		/// The update view.
		/// </summary>
		/// <param name="pageSizeDropDown">
		/// The page size drop down.
		/// </param>
		protected void UpdateView(FMPageSizeDropDown pageSizeDropDown)
		{
			//            var data = this.EnumerateQualifications();

			if (pageSizeDropDown != null)
			{
				//                pageSizeDropDown.SetPageSize(this.ApplicationDataGrid, data.Count);
			}

			//            this.ApplicationDataGrid.DataSource = data;
			this.ApplicationDataGrid.DataBind();
		}

		/// <summary>
		/// The initialize sync conflict type drop down list.
		/// </summary>
		/// <param name="dropDownList">
		/// The p drop down list.
		/// </param>
		/// <param name="selectedItem">
		/// The p selected item.
		/// </param>
		protected void InitializeSyncConflictTypeDropDownList(DropDownList dropDownList, SYNCCONFLICTTYPE? selectedItem)
		{
			for (var index = SYNCCONFLICTTYPE.UNKNOWN; index <= SYNCCONFLICTTYPE.CLIENTSERVER_DUPLICATEID; index++)
			{
				string stringVal;

				try
				{
					stringVal = SyncTypes.GetSyncConflictTypeString(index);
				}
				catch
				{
					continue;
				}

				var listItem = new ListItem(stringVal, ((int)index).ToString(CultureInfo.InvariantCulture));

				foreach (ListItem existingItem in dropDownList.Items)
				{
					if (string.Compare(existingItem.Text, listItem.Text, StringComparison.Ordinal) > 0)
					{
						int insert = dropDownList.Items.IndexOf(existingItem);
						dropDownList.Items.Insert(insert, listItem);
						if (selectedItem == index)
						{
							dropDownList.SelectedIndex = insert;
						}

						listItem = null;
						break;
					}
				}

				if (listItem != null)
				{
					dropDownList.Items.Add(listItem);
					if (selectedItem == index)
					{
						dropDownList.SelectedIndex = dropDownList.Items.Count - 1;
					}
				}
			}
		}

		/// <summary>
		/// The initialize sync session status drop down list.
		/// </summary>
		/// <param name="dropDownList">
		/// The p drop down list.
		/// </param>
		/// <param name="selectedItem">
		/// The p selected item.
		/// </param>
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "Reviewed. Suppression is OK here.")]
		protected void InitializeSyncSessionStatusDropDownList(DropDownList dropDownList, SYNCSESSIONSTATUS? selectedItem)
		{
			for (var index = SYNCSESSIONSTATUS.NEW; index <= SYNCSESSIONSTATUS.USERSTOP; index++)
			{
				string stringVal;

				try
				{
					stringVal = SyncTypes.GetSyncSessionStatusString(index);
				}
				catch
				{
					continue;
				}

				var listItem = new ListItem(stringVal, ((int)index).ToString(CultureInfo.InvariantCulture));

				foreach (ListItem existingItem in dropDownList.Items)
				{
					if (string.Compare(existingItem.Text, listItem.Text, StringComparison.Ordinal) > 0)
					{
						int insert = dropDownList.Items.IndexOf(existingItem);
						dropDownList.Items.Insert(insert, listItem);
						if (selectedItem == index)
						{
							dropDownList.SelectedIndex = insert;
						}

						listItem = null;
						break;
					}
				}

				if (listItem != null)
				{
					dropDownList.Items.Add(listItem);
					if (selectedItem == index)
					{
						dropDownList.SelectedIndex = dropDownList.Items.Count - 1;
					}
				}
			}
		}

		#endregion Methods and Operators
	}
}