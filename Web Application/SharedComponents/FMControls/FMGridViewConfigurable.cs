///***************************************************************************
/// Module Name:  FMGridViewConfigurable.cs
/// Author:       Ryan Hill
/// Copyright (c) Varec, Inc.  All rights reserved.
///***************************************************************************

using System.Web.UI;

[assembly: TagPrefix("FMControls", "FMControls")]
namespace FMControls
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Linq;
	using System.Web.UI.WebControls;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.ServiceRequests;

	/// <summary>
	/// A grid view that allows you to use a custom field configuration and order specified by the list view functionality.
	/// 
	/// To use this control, you must have a column defined in your grid view with a HeaderText matching the list view field name,
	/// For example "Meter Start".
	/// 
	/// Any column not in the list view configuration will not be displayed unless it is in the FixedColumns list. 
	/// This is so that we can always display fields like "View Details" that we don't want the user moving around or hiding.
	/// </summary>
	public class FMGridViewConfigurable : FMGridView
	{
		/// <summary>
		/// Default Constructor
		/// </summary>
		public FMGridViewConfigurable()
		{
		}

		private LISTVIEW_STANDARD_TYPE listViewStandardType = LISTVIEW_STANDARD_TYPE.TYPE_MAX;

		private LISTVIEW_TYPE listViewType = LISTVIEW_TYPE.TYPE_MAX;

		private SecurityClass security = null;

		private AccountingSite accountingSite = null;

		private List<string> fixedColumns = new List<string>();

		/// <summary>
		/// The list view standard type that should be used when getting the custom field configuration
		/// </summary>
		[Category("Custom Fields"), Description("The list view standard type that should be used when getting the custom field configuration")]
		public LISTVIEW_STANDARD_TYPE ListViewStandardType
		{
			get
			{
				return listViewStandardType;
			}
			set
			{
				this.listViewStandardType = value;
			}
		}

		/// <summary>
		/// The list view type that should be used when getting the custom field configuration
		/// </summary>
		[Category("Custom Fields"), Description("The list view type that should be used when getting the custom field configuration")]
		public LISTVIEW_TYPE ListViewType 
		{
			get
			{
				return listViewType;
			}
			set
			{
				this.listViewType = value;
			}
		}

		/// <summary>
		/// Columns that will always be displayed no matter what is in list views. 
		/// Comma delimited for now - is there a better way to store a collection of strings in an aspx file?
		/// </summary>
		[Category("Custom Fields"), Description("Columns that will always be displayed no matter what is in list views. Comma delimited")]
		public string FixedColumns
		{
			get { return string.Join(",", this.fixedColumns.ToArray()); }
			set
			{
				this.fixedColumns.Clear();

				foreach (string fixedColumn in value.Split(','))
				{
					this.fixedColumns.Add(fixedColumn);
				}
			}
		}

		/// <summary>
		/// When the control is initialized, get security and then add / remove the columns we need
		/// </summary>
		/// <param name="e">event arguments</param>
		protected override void OnInit(EventArgs e)
		{
			if (!DesignMode && this.Page.Session["Security"] != null)
			{
				this.security = this.Page.Session["Security"] as SecurityClass;
			}
			else
			{
				this.security = null;
			}

			this.AddAndRemoveColumns();

			base.OnInit(e);
		}

		/// <summary>
		/// Removes all columns from the grid, and then adds the one 
		/// the user has configured in the order they specified.
		/// </summary>
		private void AddAndRemoveColumns()
		{
			DataControlFieldCollection originalColumns = this.Columns.CloneFields();

			this.Columns.Clear();

			List<string> columnsToAdd = this.fixedColumns.Union(this.GetListViewFields()).ToList();

			foreach (string column in columnsToAdd)
			{
				foreach (DataControlField field in originalColumns)
				{
					if (field.HeaderText == column)
					{
						this.Columns.Add(field);
						break;
					}
				}
			}
		}

		/// <summary>
		/// Get the fields the user configured for the grid view in the order they wish to display them
		/// </summary>
		/// <returns>An array of strings containing the fields the user wants to display</returns>
		private List<string> GetListViewFields()
		{
			if (this.ListViewType != LISTVIEW_TYPE.TYPE_MAX && this.ListViewStandardType != LISTVIEW_STANDARD_TYPE.TYPE_MAX)
			{
				// Get site information.
				this.accountingSite = FMChannelHelper.MakeCall<IAccountingSites, AccountingSite>(
																	 x =>
																	 x.LoadSiteInfo(this.security, this.security.SiteGuid)
																);

				this.accountingSite.GetUserCompanies = false;

				List<string> list = new List<string>();
				ListViewSR sr = new ListViewSR();
				sr.Security = this.security;
				sr.Site = this.accountingSite.CurrentSiteName;
				sr.Type = this.ListViewType;
				sr.TypeGuid = ListViewClass.GetGuidFromStandardType(this.ListViewStandardType);
				sr.ListViewGuid = Guid.Empty;

				ListViewDO listViewDO = FMChannelHelper.MakeCall<IListViewProcessor, ListViewDO>(
																	 x =>
																	 x.Process(sr)
																);


				ListViewColumnDO columnDO = new ListViewColumnDO();

				for (int index = 0; (columnDO = listViewDO[index]) != null; ++index)
				{
					list.Add(columnDO.ColumnName);
				}

				return list;
			}
			else
			{
				// The developer did not set at least one of the List View enumeration types. Warn them
				System.Diagnostics.Debug.Assert(this.ListViewStandardType != LISTVIEW_STANDARD_TYPE.TYPE_MAX, "Custom grid view's List View Standard Type not set");
				System.Diagnostics.Debug.Assert(this.ListViewType != LISTVIEW_TYPE.TYPE_MAX, "Custom grid view's List View Type not set");
				return new List<string>();
			}
		}
	}
}
