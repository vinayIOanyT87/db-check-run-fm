// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LedgerAggregateColumnsForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the LedgerAggregateColumnsForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.Accounting
{
	using System;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Interfaces;

	using FMControls;

	public partial class LedgerAggregateColumnsForm : AccountingWebFormView, IEntityDiscovery
	{
		#region Constants and Fields

		private const string ENTITY_NAME = "Ledger Aggregate Columns";

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
				return typeof(ILedgerAggregateColumns);
			}
		}

		ENTITY_TYPE IEntityDiscovery.EntityType
		{
			get
			{
				return ENTITY_TYPE.LEDGER_AGGREGATE_COLUMN;
			}
		}

		#endregion

		#region Explicit Interface Methods

		EntityToSiteMapCollectionClass IEntityDiscovery.EnumerateEntityMaps(
			SecurityClass security, ENTITY_ASSIGNMENT_TYPE Type)
		{
			LedgerAggregateColumnCollectionClass columnCollection =
				FMChannelHelper.MakeCall<ILedgerAggregateColumns, LedgerAggregateColumnCollectionClass>(
						x =>
						x.Enumerate(security)
				);

			var EntityToSiteMapCollection = new EntityToSiteMapCollectionClass();

			foreach (LedgerAggregateColumnClass column in columnCollection)
			{
				if (Type == ENTITY_ASSIGNMENT_TYPE.ASSIGNED)
				{
					if (security.SiteGuid == column.SiteGuid)
					{
						continue;
					}

					if (security.LoginSiteGuid != column.SiteGuid)
					{
						continue;
					}
				}
				else
				{
					if (security.SiteGuid != column.SiteGuid)
					{
						continue;
					}
				}

				var EntityToSiteMap = new EntityToSiteMapClass(column);
				EntityToSiteMapCollection.Add(EntityToSiteMap);
			}

			return EntityToSiteMapCollection;
		}

		Guid IEntityDiscovery.GetIdentityGuid(SecurityClass security, string ID)
		{
			return FMChannelHelper.MakeCall<ILedgerAggregateColumns, Guid>(x => x.GetIdentityGuid(security, ID));
		}

		void IEntityDiscovery.SetSiteGuid(SecurityClass security, Guid guid, Guid SiteGuid)
		{
			LedgerAggregateColumnClass column =
				FMChannelHelper.MakeCall<ILedgerAggregateColumns, LedgerAggregateColumnClass>(
					x => x.GetByColumnGuid(security, guid));

			column.SiteGuid = SiteGuid;

			FMChannelHelper.MakeCall<ILedgerAggregateColumns>(x => x.Modify(security, column));
		}

		#endregion

		#region Methods

		protected void AggregateGrid_RowCommand(object sender, CommandEventArgs e)
		{
			try
			{
				if (e.CommandName.Equals("Edit", StringComparison.OrdinalIgnoreCase))
				{
					int rowIndex = Convert.ToInt32(e.CommandArgument);
					GridViewRow row = this.AggregateGrid.Rows[rowIndex];

					var identityGuidLabel = (Label)row.FindControl("IdentityGuidLabel");
					Guid columnGuid = Guid.Parse(identityGuidLabel.Text);

					LedgerAggregateColumnClass column =
						FMChannelHelper.MakeCall<ILedgerAggregateColumns, LedgerAggregateColumnClass>(
								x =>
								x.GetByColumnGuid(this.security, columnGuid)
						);

					this.Session[PageSessionKeyConstants.LEDGER_AGGREGATE_COLUMN_OBJECT] = column;

					this.Redirect("LedgerAggregateColumnForm.aspx");
				}
				else if (e.CommandName.Equals("Delete", StringComparison.OrdinalIgnoreCase))
				{
					int rowIndex = Convert.ToInt32(e.CommandArgument);
					GridViewRow row = this.AggregateGrid.Rows[rowIndex];

					var identityGuidLabel = (Label)row.FindControl("IdentityGuidLabel");
					Guid columnGuid = Guid.Parse(identityGuidLabel.Text);

					FMChannelHelper.MakeCall<ILedgerAggregateColumns>(
																	 x =>
																	 x.Purge(this.security, columnGuid)
																);

					this.UpdateView();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected override void OnInit(EventArgs e)
		{
			base.Initialize();
			this.InitializeControls();
			base.OnInit(e);
		}

		protected void Page_Init(object sender, EventArgs e)
		{
			try
			{
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void Page_Load(object sender, EventArgs e)
		{
		}

		protected void UpdateView()
		{
			LedgerAggregateColumnCollectionClass columnsCollection =
				FMChannelHelper.MakeCall<ILedgerAggregateColumns, LedgerAggregateColumnCollectionClass>(
						x =>
						x.EnumerateByFindText(this.security, this.FindText.Text)
				);

			this.PageSizeDropDown.SetPageSize(this.AggregateGrid, columnsCollection.Count);

			this.AggregateGrid.DataSource = columnsCollection;
			this.AggregateGrid.DataBind();
		}

		private void AddButton_Click(object sender, EventArgs e)
		{
			var column = new LedgerAggregateColumnClass();
			this.Session[PageSessionKeyConstants.LEDGER_AGGREGATE_COLUMN_OBJECT] = column;
			this.Redirect("LedgerAggregateColumnForm.aspx");
		}

		private void AggregateGrid_RowDataBound(object sender, GridViewRowEventArgs e)
		{
			try
			{
				if (e.Row.RowType == DataControlRowType.DataRow)
				{
					var column = (LedgerAggregateColumnClass)e.Row.DataItem;

					var deleteButton = (FMDeleteLinkButton)e.Row.FindControl("DeleteButton");
					if (deleteButton != null)
					{
						deleteButton.CommandArgument = e.Row.RowIndex.ToString();
						deleteButton.Enabled = column.SiteGuid == this.security.SiteGuid;
					}

					var editButton = (FMEditLinkButton)e.Row.FindControl("EditButton");
					if (editButton != null)
					{
						editButton.CommandArgument = e.Row.RowIndex.ToString();
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		//protected void AggregateGrid_RowCommand ( object sender, GridViewCommandEventArgs e )

		private void FindButton_Click(object sender, EventArgs e)
		{
			try
			{
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void InitializeControls()
		{
			this.AddButton.Click += this.AddButton_Click;
			this.AddButton2.Click += this.AddButton_Click;
			this.FindButton.Click += this.FindButton_Click;
			this.AggregateGrid.RowCommand += this.AggregateGrid_RowCommand;
			this.AggregateGrid.RowDataBound += this.AggregateGrid_RowDataBound;
		}

		#endregion
	}
}
