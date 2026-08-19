// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SynchronizationConflict.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the SynchronizationConflict type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMEntityImportWebApp
{
	using System;
	using System.Data;
	using System.Web.UI.WebControls;
	using System.Collections.Generic;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

    using FMCore;

	using FuelsManager.FMWebApp;

	public partial class SynchronizationConflict : SynchronizationSessionFormBase
	{
		private const string SynConflictDO = "SyncConflictDO";

		#region Methods and Operators
		/// <summary>
		/// Populate the fields on the screen with data
		/// </summary>
		protected override void UpdateView()
		{
			try
			{
				var syncConflictDO = this.Page.Session[SynConflictDO] as SyncRecordConflictDO;

				var foreignKeyDictionary = new Dictionary<string, ForeignKeyDO>(); 

				string[] fileName = syncConflictDO.TableName.Split(new char[] { '.' });

				if (fileName.Length == 2)
				{
					foreignKeyDictionary = FMChannelHelper.MakeCall<IDBAccess, Dictionary<string, ForeignKeyDO>>(
													x => x.EnumerateForeignKeys(this.Security, fileName[0], fileName[1]));
				}


				var syncConflictDataTable = new DataTable();

				syncConflictDataTable.Columns.Add("Key", typeof(string));
				syncConflictDataTable.Columns.Add("Value", typeof(string));
				syncConflictDataTable.Columns.Add("ReferenceTable", typeof(string));

				if (syncConflictDO != null)
				{
					foreach (var parameter in syncConflictDO.Parameters)
					{
						if (this.UniqueIdentifiersCheckbox.Checked
						&& !(parameter.Value is System.Guid))
						{
							continue;
						}

						if(parameter.Key.StartsWith("@sync_supported_columns"))
                        {
							continue;
                        }

						DataRow syncConflictDataRow = syncConflictDataTable.NewRow();

						syncConflictDataRow["Key"] = parameter.Key;
						syncConflictDataRow["Value"] = parameter.Value;
						string columnName = parameter.Key.Substring(1);
						if (foreignKeyDictionary.ContainsKey(columnName))
						{ 
							var foreignKey = foreignKeyDictionary[columnName];
							syncConflictDataRow["ReferenceTable"] = foreignKey.ReferenceSchema + "." + foreignKey.ReferenceTableName;
						}
						else
						{
							syncConflictDataRow["ReferenceTable"] = "";
						}

						syncConflictDataTable.Rows.Add(syncConflictDataRow);

						this.SyncConflictDataGrid.DataSource = syncConflictDataTable;
					}
				}
				this.SyncConflictDataGrid.DataBind();
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}

		#endregion Methods and Operators

		#region Page Events and Overrides
		protected override void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			this.InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.ClearButton.Command += this.ClearButtonCommand;

		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				if (!this.Security.HasRight(RIGHT.VIEW_SYNC_CONFLICT_STATUS))
				{
					throw new Exception("Insufficient Rights");
				}

				if (!this.Page.IsPostBack)
				{
					if (this.Session["SyncConflictUniqueIdentifers"] is bool)
					{
						this.UniqueIdentifiersCheckbox.Checked = (bool)this.Session["SyncConflictUniqueIdentifers"];
					}

					if (this.Request.GetQueryOrFormValue("SyncConflictGuid") != null)
					{
						var syncConflictGuid = Guid.Parse(this.Request.GetQueryOrFormValue("SyncConflictGuid"));
						var syncConflictDO = FMChannelHelper.MakeCall<ISyncRecordConflicts, SyncRecordConflictDO>(
																			x => x.Get(this.Security, syncConflictGuid));
						this.Page.Session.Add(SynConflictDO, syncConflictDO);

						if(syncConflictDO.SyncConflictResolutionStatusIndex == SYNCCONFLICTRESOLUTIONSTATUS.CLEARED
							|| syncConflictDO.SyncConflictResolutionStatusIndex == SYNCCONFLICTRESOLUTIONSTATUS.RESOLVED
							|| !this.Security.HasRight(RIGHT.MODIFY_SYNC_CONFLICT_STATUS))
						{
							this.ClearButton.Enabled = false;
						}
					}

					this.UpdateView();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}
		#endregion Page Events and Overrides

		#region Control Events
		private void ClearButtonCommand(object sender, CommandEventArgs e)
		{
			try
			{
				this.GetSecurity();

				var syncConflictDO = this.Page.Session[SynConflictDO] as SyncRecordConflictDO;

				if (syncConflictDO != null)
				{
					syncConflictDO.SyncConflictResolutionStatusIndex = SYNCCONFLICTRESOLUTIONSTATUS.CLEARED;
					syncConflictDO.ResolvedBy = this.Security.UserID;
					syncConflictDO.ResolvedDate = DateTimeOffset.Now;
					FMChannelHelper.MakeCall<ISyncRecordConflicts>(x => x.Modify(this.Security, syncConflictDO));
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void UniqueIdentifiers_CheckBoxChanged(object sender, System.EventArgs e)
		{
			this.Session["SyncConflictUniqueIdentifers"] = this.UniqueIdentifiersCheckbox.Checked;

			this.UpdateView();
		}


		#endregion Control Events

		#region Conflict Grid Methods
		protected void SyncConflictDataGrid_OnPageIndexChanging(object sender, GridViewPageEventArgs e)
		{
			try
			{
				this.SyncConflictDataGrid.PageIndex = e.NewPageIndex;
				this.UpdateView();
			}
			catch (Exception error)
			{
				this.ErrorHandler(error);
			}
		}
		#endregion Conflict Grid Methods
	}
}