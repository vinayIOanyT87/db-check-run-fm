// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StateSelectForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the StateSelectForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Web;
	using System.Web.UI.HtmlControls;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	using FMControls;
    using FMCore;

	/// <summary>
	///    Summary description for StateSelectForm.
	/// </summary>
	public class StateSelectForm : FMAutoSubmitFormBase
	{
		#region Constants and Fields

		protected FMButton FindBtn;

		protected TextBox FindTextBox;

		protected Image Image1;

		protected string SelectThisItemText = null;

		protected FMButton ShowAllBtn;

		protected FMDataGrid StateDataGrid;

		protected StateSelectContextClass StateSelectContext = null;

		#endregion

		#region Methods

		protected override void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			this.InitializeComponent();
			base.OnInit(e);
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				this.SelectThisItemText = this.GetTranslatedText("Select this item");

				if (this.Page.IsPostBack == false)
				{
					this.StateSelectContext = new StateSelectContextClass();

					if (this.Request.GetQueryOrFormValue("Mode") != null)
					{
						this.StateSelectContext.Mode = this.Request.GetQueryOrFormValue("Mode");
					}

					this.Session["StateSelectContext"] = this.StateSelectContext;
					this.UpdateView();
				}
				else
				{
					this.StateSelectContext = this.Session["StateSelectContext"] as StateSelectContextClass;
				}

				var Form1 = (HtmlForm)this.FindControl("Form1");
				var OkButton = new HtmlInputButton();
				OkButton.Attributes.Add("value", this.GetTranslatedText("OK"));
				OkButton.Attributes.Add("id", "OkButton");
				OkButton.Attributes.Add("class", "formfieldtitle");
				OkButton.Attributes.Add("onclick", "MultipleSelect()");
				OkButton.Attributes.Add("style", "width:66px;Z-INDEX: 107; LEFT: 662px; POSITION: absolute; TOP: 8px");
				Form1.Controls.Add(OkButton);

				var CancelButton = new HtmlInputButton();
				CancelButton.Attributes.Add("value", this.GetTranslatedText("Cancel"));
				CancelButton.Attributes.Add("id", "CancelButton");
				CancelButton.Attributes.Add("class", "formfieldtitle");
				CancelButton.Attributes.Add("onclick", "NoSelect()");
				CancelButton.Attributes.Add("style", "width:66px;Z-INDEX: 107; LEFT: 758px; POSITION: absolute; TOP: 8px");
				Form1.Controls.Add(CancelButton);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void FindAllBtn_OnClick(object sender, EventArgs e)
		{
			this.StateSelectContext.SearchString = null;
			this.FindTextBox.Text = "";
			this.UpdateView();
		}

		private void FindBtn_OnClick(object sender, EventArgs e)
		{
			if (this.FindTextBox.Text.Length < 1)
			{
				this.StateSelectContext.SearchString = null;
			}
			else
			{
				this.StateSelectContext.SearchString = this.FindTextBox.Text.ToUpper();
			}

			this.UpdateView();
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.ShowAllBtn.Click += new System.EventHandler(this.FindAllBtn_OnClick);
			this.FindBtn.Click += new System.EventHandler(this.FindBtn_OnClick);
			this.StateDataGrid.ItemDataBound +=
				new System.Web.UI.WebControls.DataGridItemEventHandler(this.StateDataGrid_ItemDataBound);
		}

		private void StateDataGrid_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			if (e.Item.ItemIndex == -1)
			{
				if (e.Item.ItemType == ListItemType.Header)
				{
					e.Item.Cells[0].Text = this.GetTranslatedText(this.StateSelectContext.Mode);
				}
			}

			else
			{
				var Select = new HtmlInputCheckBox();
				Select.ID = "Select";
				e.Item.Cells[0].Controls.Add(Select);
			}
		}

		private void UpdateView()
		{
			var FootNote = this.Session["FootNote"] as FootNoteClass;
			if (FootNote == null)
			{
				return;
			}

			var StateArray = new ArrayList();

			if (this.StateSelectContext.Mode == "Assign")
			{
				// Test for Assignement of {All}
				if (!((FootNote.FootNoteShipToStateMapCollection.Count == 1)
                    && (FootNote.FootNoteShipToStateMapCollection[0].AssignedToGuid == Guid.Empty) 
                    && (FootNote.FootNoteShipToStateMapCollection[0].AssignedToID.Equals("{All}"))))
				{
					StateArray.Add(HttpUtility.HtmlEncode(this.GetTranslatedText("{All}")));

					string[] States = 
						FMChannelHelper.MakeCall<ICompanies, string[]>(
								x =>
								x.EnumerateColumnForAuthorizedCustomerShipTo(this.Security, Guid.Empty, "State")
						);
					
					foreach (string State in States)
					{
						bool Assigned = false;

						if (State == string.Empty)
						{
							continue;
						}

						foreach (ApplicationStringMapClass AssignedApplicationStringMap in FootNote.FootNoteShipToStateMapCollection)
						{
							if (State == AssignedApplicationStringMap.AssignedToID)
							{
								Assigned = true;
								break;
							}
						}

						if (!Assigned)
						{
							StateArray.Add(State);
						}
					}
				}
			}

			else
			{
				foreach (ApplicationStringMapClass AssignedApplicationStringMap in FootNote.FootNoteShipToStateMapCollection)
				{
					StateArray.Add(HttpUtility.HtmlEncode(AssignedApplicationStringMap.AssignedToID));
				}
			}

			var StateDataTable = new DataTable();
			DataRow StateDataRow;

			StateDataTable.Columns.Add("ID", typeof(string));

			string searchStr = this.FindTextBox.Text;

			foreach (string State in StateArray)
			{
				if (searchStr != string.Empty && -1 == State.ToUpper().IndexOf(searchStr.ToUpper()))
				{
					continue;
				}

				StateDataRow = StateDataTable.NewRow();

				StateDataRow["ID"] = State;

				StateDataTable.Rows.Add(StateDataRow);
			}

			var StateDataView = new DataView(StateDataTable);

			this.StateDataGrid.DataSource = StateDataView;
			this.StateDataGrid.DataBind();
		}

		#endregion

		[Serializable]
		public class StateSelectContextClass
		{
			#region Constants and Fields

			public string Mode = null;

			public string SearchString = null;

			#endregion
		}
	}
}