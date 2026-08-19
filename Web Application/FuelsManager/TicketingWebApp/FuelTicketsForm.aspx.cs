/******************************************************************************

	FILE NAME:		FuelTicketsGroupsForm.aspx.cs


	PURPOSE:			Implementation of FuelTicketsForm


	COMMENTS:

		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2002

		This file shall not be copied or reproduced in any form without
				the express written consent of Endress+Hauser.


	AUTHOR(S):	W. Gray


	VERSION:		1.0.0  Current version



	MODIFICATION HISTORY:
		Date:			By:			Reason:
		---------	----------  -------------------------------------------

*******************************************************************************/
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Microsoft.Web.UI.WebControls;
using FMCommon;
using FMWebApp;
using Interop.FMUtil;
using WebTicketingBLL;
using WebTicketingDataObjects;
using FMControls;
using ConsolidatedDataObjects;

namespace TicketingWebApp
{
	/// <summary>
	/// Summary description for FuelingTicketsForm.
	/// </summary>
	public partial class FuelingTicketsForm : FMFormBase, ITreeNodeDiscovery, IDataDictionary
	{
		protected SiteClass CurrentSite;

		Microsoft.Web.UI.WebControls.TreeNode
            ITreeNodeDiscovery.GetLeftViewTreeNode(SecurityClass Security,bool SiteGroup,uint Options,uint SpecialKeyCodes)
		{
			if(SiteGroup)
				return null;

			if(!Security.HasRight(RIGHT.VIEW_TICKETING_DATA)
			&& !Security.HasRight(RIGHT.MODIFY_TICKETING_DATA))
				return null;

			// Depends Upon WebTicketing
			if((Options & 0x40000) == 0)
				return null;

            Microsoft.Web.UI.WebControls.TreeNode AviationNode = new Microsoft.Web.UI.WebControls.TreeNode();
			AviationNode.NavigateUrl="AviationForm.aspx";
			AviationNode.Text="IntoPlane";

            Microsoft.Web.UI.WebControls.TreeNode FuelTicketsNode = new Microsoft.Web.UI.WebControls.TreeNode();
			AviationNode.Nodes.Add(FuelTicketsNode);
			FuelTicketsNode.NavigateUrl="..\\TicketingWebApp\\FuelTicketsForm.aspx";
			FuelTicketsNode.Text="Fuel Tickets";
			FuelTicketsNode.ImageUrl="images\\ctxmsc_cls.gif";
			FuelTicketsNode.SelectedImageUrl="images\\ctxmsc_opn.gif";

			return AviationNode;
		}

		string [] IDataDictionary.Keys(SecurityClass Security)
		{
			string [] Keys={	"Fuel Tickets",
									"Date",
									"Type",
									"Manager",
									"Owner",
									"Agent",
									"Airline",
									"Flight Number",
									"Add",
									"Send",
									"IntoPlane"
								};

			return Keys;
		}


		private void UpdateView()
		{
			FuelTicketsDataGrid.DataSource=EnumerateFuelTickets();
			FuelTicketsDataGrid.DataBind();
		}

		private ICollection EnumerateFuelTickets()
		{
			FuelTicketCollectionClass	FuelTicketCollection;
			FuelTicketsClass FuelTickets=new FuelTicketsClass();
			FuelTicketCollection=FuelTickets.Enumerate(Security);

			DataTable			FuelTicketDataTable=new DataTable();
			DataRow				FuelTicketDataRow;
	
			FuelTicketDataTable.Columns.Add("Index",typeof(Int32));
			FuelTicketDataTable.Columns.Add("SequenceNumber",typeof(string));
			FuelTicketDataTable.Columns.Add("Date",typeof(string));
			FuelTicketDataTable.Columns.Add("Type",typeof(string));
			FuelTicketDataTable.Columns.Add("ManagerID",typeof(string));
			FuelTicketDataTable.Columns.Add("OwnerID",typeof(string));
			FuelTicketDataTable.Columns.Add("CarrierID",typeof(string));
			FuelTicketDataTable.Columns.Add("ShipToID",typeof(string));
			FuelTicketDataTable.Columns.Add("FlightNumber",typeof(string));


			foreach(FuelTicketClass FuelTicket in FuelTicketCollection)
			{
				FuelTicketDataRow=FuelTicketDataTable.NewRow();

				FuelTicketDataRow[0]=FuelTicket.Index;
				FuelTicketDataRow[1]=FuelTicket.SequenceNumber.ToString("D4");
				FuelTicketDataRow[2]=FuelTicket.Date.ToString("d",CurrentSite.GetDateTimeFormatInfo());
				FuelTicketDataRow[3]=FuelTicket.Type;
				FuelTicketDataRow[4]=FuelTicket.ManagerID;
				FuelTicketDataRow[5]=FuelTicket.OwnerID;
				FuelTicketDataRow[6]=FuelTicket.VendorID;
				FuelTicketDataRow[7]=FuelTicket.ShipToID;
				FuelTicketDataRow[8]=FuelTicket.FlightNumber;

				FuelTicketDataTable.Rows.Add(FuelTicketDataRow);
			}

			DataView		FuelTicketDataView=new DataView(FuelTicketDataTable);
			return FuelTicketDataView;
		}


		protected void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				GetSecurity();

				CurrentSite=Sites.Get(Security,Security.SiteIndex);
				
				if (! Page.IsPostBack) 
				{
					if(Session["Page"] != null)
					{
						FuelTicketsDataGrid.CurrentPageIndex=(int) Session["Page"];
						Session.Remove("Page");
					}

					if(!Security.HasRight(RIGHT.MODIFY_TICKETING_DATA))
					{
						AddButton.Enabled=false;
						SendButton.Enabled=false;
					}

					UpdateView();
				}
			}
			catch (Exception except)
			{
				ErrorHandler(except);
			}
		}


		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.FuelTicketsDataGrid.EditCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.FuelTicketsDataGrid_EditCommand);
			this.FuelTicketsDataGrid.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.FuelTicketsDataGrid_PageIndexChanged);
			this.FuelTicketsDataGrid.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.FuelTicketsDataGrid_DeleteCommand);
			this.FuelTicketsDataGrid.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.FuelTicketsDataGrid_ItemDataBound);
			this.AddButton.Command += new System.Web.UI.WebControls.CommandEventHandler(this.AddButton_Command);
			this.SendButton.Command += new System.Web.UI.WebControls.CommandEventHandler(this.SubmitButton_Command);

		}
		#endregion

		private void FuelTicketsDataGrid_DeleteCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			try
			{
				TableCell indexCell = e.Item.Cells[2];
				
				FuelTicketsClass FuelTickets=new FuelTicketsClass();
				FuelTickets.Purge(Security,System.Convert.ToInt32(indexCell.Text));

				FuelTicketsDataGrid.SelectedIndex=-1;
				if(FuelTicketsDataGrid.Items.Count == 1
				&& FuelTicketsDataGrid.CurrentPageIndex > 0)
					FuelTicketsDataGrid.CurrentPageIndex--;

				UpdateView();
			}
			catch (Exception except)
			{
				ErrorHandler(except);
			}
		
		}

		private void FuelTicketsDataGrid_EditCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			Session.Remove("FuelTicket");
			TableCell indexCell = e.Item.Cells[2];
			Session["Index"]=indexCell.Text;
			Session["Page"]=FuelTicketsDataGrid.CurrentPageIndex;
			Response.Redirect("FuelTicketForm.aspx");
		}

		private void FuelTicketsDataGrid_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			// if we are editing do not allow a page change
			if (FuelTicketsDataGrid.EditItemIndex > -1)
				return;
			FuelTicketsDataGrid.CurrentPageIndex = e.NewPageIndex;
			UpdateView();
		}

		private void AddButton_Command(object sender, System.Web.UI.WebControls.CommandEventArgs e)
		{
			Session.Remove("Index");
			Session["Page"]=FuelTicketsDataGrid.CurrentPageIndex;
			Response.Redirect("FuelTicketForm.aspx");
		}

		private void SubmitButton_Command(object sender, System.Web.UI.WebControls.CommandEventArgs e)
		{
			try
			{
				FuelTicketsClass FuelTickets=new FuelTicketsClass();
				FuelTickets.Send(Security);

				FuelTicketsDataGrid.SelectedIndex=-1;
				FuelTicketsDataGrid.CurrentPageIndex=0;

				UpdateView();
			}
			catch (Exception except)
			{
				ErrorHandler(except);
			}
		}

		private void FuelTicketsDataGrid_ItemDataBound(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
		{
			LinkButton DeleteButton = (LinkButton) e.Item.FindControl("DeleteButton");
			if(DeleteButton != null)
			{
				if(!Security.HasRight(RIGHT.MODIFY_TICKETING_DATA))
				{ 
					DeleteButton.Enabled=false;
					DeleteButton.Text="<img src=..\\FMWebApp\\Images\\Delete_un.gif border=0 align=absmiddle alt='Delete this item'>";
				}
			}
		}
	}
}
