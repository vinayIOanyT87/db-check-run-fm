/******************************************************************************

	FILE NAME:		FuelTicketForm.aspx.cs


	PURPOSE:			Implementation of FuelTicketForm


	COMMENTS:

		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2002

		This file shall not be copied or reproduced in any form without
				the express written consent of Endress+Hauser.


	AUTHOR(S):	W. Gray


	VERSION:		1.0.0  Current version



	MODIFICATION HISTORY:
		Date:			By:			Reason:
		---------	----------  -------------------------------------------
		08/16/2005	W.Gray		7.0.0.30 - Changed to Enumerate Equipment as all Carts & Tankers 

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
using WebTicketingBLL;
using WebTicketingDataObjects;
using FMCommon;
using FMWebApp;
using ConsolidatedBLL;
using ConsolidatedDataObjects;
using FMControls;

namespace TicketingWebApp
{
	/// <summary>
	/// Summary description for FuelTicketForm.
	/// </summary>
	public partial class FuelTicketForm : FMFormBase, IDataDictionary
	{
		protected SiteClass CurrentSite;
		public ListItemCollection EquipmentListItemCollection;
		public ListItemCollection OperatorListItemCollection;
		public ListItemCollection ProductListItemCollection;

		string [] IDataDictionary.Keys(SecurityClass Security)
		{
			string [] Keys={	"Fuel Ticket",
									"Date",
									"Set",
									"Owner",
									"Agent",
									"Airline Code",
									"Flight Number",
									"Tail Number",
									"Aircraft Type",
									"Destination",
									"Gate",
									"Credit Card",
									"Valid Thru",
									"FTZ",
									"Ticket",
									"Pit",
									"Equipment",
									"Operator",
									"Product",
									"Meter Start",
									"Meter End",
									"Total",
									"Start Time",
									"Stop Time",
									"Add",
									"Submit",
									"Close",
									"Gauges",
									"Arrival",
									"Required",
									"Final",
									"Issue",
									"Defuel",
									"Type",
									"Agent Not Selected",
									"Airline Not Selected"
								};

			return Keys;
		}
	
		private void UpdateView()
		{
			try
			{
				EquipmentListItemCollection=EnumerateEquipment();
				OperatorListItemCollection=EnumerateOperators();
				ProductListItemCollection=EnumerateProducts();
				FuelTicketLineItemsDataGrid.DataSource=EnumerateFuelTicketLineItems();
				FuelTicketLineItemsDataGrid.DataBind();
			}
			catch (Exception except)
			{
				ErrorHandler(except);
			}
		}
			
		private ICollection EnumerateFuelTicketLineItems()
		{
			FuelTicketClass FuelTicket=(FuelTicketClass) Session["FuelTicket"];

			DataTable			FuelTicketLineItemDataTable=new DataTable();
			DataRow				FuelTicketLineItemDataRow;
	
			FuelTicketLineItemDataTable.Columns.Add("Index",typeof(Int32));
			FuelTicketLineItemDataTable.Columns.Add("DocumentNumber",typeof(string));
			FuelTicketLineItemDataTable.Columns.Add("PitID",typeof(string));
			FuelTicketLineItemDataTable.Columns.Add("MeterStart",typeof(double));
			FuelTicketLineItemDataTable.Columns.Add("MeterEnd",typeof(double));
			FuelTicketLineItemDataTable.Columns.Add("Total",typeof(double));
			FuelTicketLineItemDataTable.Columns.Add("StartTime",typeof(string));
			FuelTicketLineItemDataTable.Columns.Add("StopTime",typeof(string));

			if(FuelTicket != null)
			{
				foreach(FuelTicketLineItemClass FuelTicketLineItem in FuelTicket.FuelTicketLineItemCollection)
				{
					FuelTicketLineItemDataRow=FuelTicketLineItemDataTable.NewRow();

					FuelTicketLineItemDataRow[0]=FuelTicketLineItem.Index;
					FuelTicketLineItemDataRow[1]=FuelTicketLineItem.DocumentNumber;
					FuelTicketLineItemDataRow[2]=FuelTicketLineItem.PitID;
					FuelTicketLineItemDataRow[3]=FuelTicketLineItem.MeterStart;
					FuelTicketLineItemDataRow[4]=FuelTicketLineItem.MeterEnd;
					FuelTicketLineItemDataRow[5]=FuelTicketLineItem.GrossQuantity;
					FuelTicketLineItemDataRow[6]=FuelTicketLineItem.StartTime.ToShortTimeString();
					FuelTicketLineItemDataRow[7]=FuelTicketLineItem.StopTime.ToShortTimeString();

					FuelTicketLineItemDataTable.Rows.Add(FuelTicketLineItemDataRow);
				}
			}

			DataView		FuelTicketLineItemDataView=new DataView(FuelTicketLineItemDataTable);
			return FuelTicketLineItemDataView;

		}
		
		public ListItemCollection EnumerateEquipment()
		{
			ListItemCollection	ListItems=new ListItemCollection();

			EquipmentsClass Equipments=new EquipmentsClass();
			EquipmentCollectionClass EquipmentCollection=Equipments.Enumerate(Security);
			foreach(EquipmentClass Equipment in EquipmentCollection)
			{
				if(Equipment.LockedOut)
					continue;

				if(Equipment.Type != EQUIPMENT_TYPE.HYDRANT_CART_TYPE
				&& Equipment.Type != EQUIPMENT_TYPE.STATIONARY_CART_TYPE
				&& Equipment.Type != EQUIPMENT_TYPE.TANKER_TYPE)
					continue;

				ListItem Item=new ListItem(Equipment.ID,Equipment.Index.ToString());
				ListItems.Add(Item);
			}
			
			return ListItems;
		}

		public ListItemCollection EnumerateOperators()
		{
			ListItemCollection	ListItems=new ListItemCollection();

			if(CarrierDropDownList.SelectedIndex == -1)
				throw new Exception("Agent not selected");

			PersonnelClass Personnel=new PersonnelClass();
			PersonCollectionClass PersonCollection=Personnel.EnumerateByCompany(Security,System.Convert.ToInt32(CarrierDropDownList.SelectedValue));
			foreach(PersonClass Person in PersonCollection)
			{
				if(Person.LockedOut)
					continue;

				ListItem Item=new ListItem(Person.ID,Person.Index.ToString());
				ListItems.Add(Item);
			}
			
			return ListItems;
		}

		public ListItemCollection EnumerateProducts()
		{
			ListItemCollection	ListItems=new ListItemCollection();

			if(ShipToDropDownList.SelectedIndex == -1)
				throw new Exception("Airline not selected");

			ProductMapsClass ProductMaps=new ProductMapsClass();
			ProductMapCollectionClass ProductMapCollection=ProductMaps.EnumerateByAssignedToIndexAndType(Security,System.Convert.ToInt32(ShipToDropDownList.SelectedValue),PRODUCT_MAP_TYPE.PRODUCT_COMPANY_MAP);
			foreach(ProductMapClass ProductMap in ProductMapCollection)
			{
				if(ProductMap.LockedOut)
					continue;

				ListItem Item=new ListItem(ProductMap.AssignedID,ProductMap.AssignedIndex.ToString());
				ListItems.Add(Item);
			}
			
			if(ListItems.Count == 0)
				throw(new Exception("No Products Available."));

			return ListItems;
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				GetSecurity();

				CurrentSite=Sites.Get(Security,Security.SiteIndex);

				FuelTicketClass FuelTicket;

				if (! Page.IsPostBack) 
				{
					if(!Security.HasRight(RIGHT.MODIFY_TICKETING_DATA))
					{
						SubmitButton.Enabled=false;
						AddButton.Enabled=false;
					}

					if(Session["Index"] != null)
					{
						FuelTicketsClass	FuelTickets=new FuelTicketsClass();
						FuelTicket=FuelTickets.Get(Security,System.Convert.ToInt32((string) Session["Index"]));

						Session["FuelTicketType"]=FuelTicket.Type;
						Session["FuelTicketManagerID"]=FuelTicket.ManagerID;
						Session["FuelTicketOwnerID"]=FuelTicket.OwnerID;
						Session["FuelTicketShipToID"]=FuelTicket.ShipToID;
						Session["FuelTicketVendorID"]=FuelTicket.VendorID;
					}
					else
					{
						FuelTicket=new FuelTicketClass();
						FuelTicket.FuelTicketLineItemCollection.Add(new FuelTicketLineItemClass());

						if(Session["FuelTicketType"] != null)
							FuelTicket.Type=(string) Session["FuelTicketType"];

						if(Session["FuelTicketManagerID"] != null)
							FuelTicket.ManagerID=(string) Session["FuelTicketManagerID"];

						if(Session["FuelTicketOwnerID"] != null)
							FuelTicket.OwnerID=(string) Session["FuelTicketOwnerID"];

						if(Session["FuelTicketShipToID"] != null)
							FuelTicket.ShipToID=(string) Session["FuelTicketShipToID"];

						if(Session["FuelTicketVendorID"] != null)
							FuelTicket.VendorID=(string) Session["FuelTicketVendorID"];
					}

					Session["FuelTicket"]=FuelTicket;

					DateTextbox.Text = FuelTicket.Date.ToString("d",CurrentSite.GetDateTimeFormatInfo());

					// Populate TypeDropDownList
					string [] Types={	"Issue",
											"Defuel"
										};
					int Index=0;
					foreach(string Type in Types)
					{
						ListItem Item=new ListItem(Type,Index.ToString());
						TypeDropDownList.Items.Add(Item);
						if(Type == FuelTicket.Type)
							TypeDropDownList.SelectedIndex=TypeDropDownList.Items.Count-1;
						Index++;
					}

					// Populate ManagerDropDownList
					CompaniesClass	Companies=new CompaniesClass();
					CompanyCollectionClass Managers=Companies.EnumerateByRole(Security,COMPANY_ROLE.MANAGER,true);
					foreach(CompanyClass Manager in Managers)
					{
						if(Manager.LockedOut
						&& FuelTicket.ManagerID != Manager.ID)
							continue;

						ListItem Item=new ListItem(Manager.ID,Manager.Index.ToString());
						ManagerDropDownList.Items.Add(Item);
						if(FuelTicket.ManagerID == Manager.ID)
							ManagerDropDownList.SelectedIndex=ManagerDropDownList.Items.Count-1;
					}

					// Populate OwnerDropDownList
					CompanyCollectionClass Owners=Companies.EnumerateByRole(Security,COMPANY_ROLE.OWNER,true);
					foreach(CompanyClass Owner in Owners)
					{
						if(Owner.LockedOut
						&& FuelTicket.OwnerID != Owner.ID)
							continue;

						ListItem Item=new ListItem(Owner.ID,Owner.Index.ToString());
						OwnerDropDownList.Items.Add(Item);
						if(FuelTicket.OwnerID == Owner.ID)
							OwnerDropDownList.SelectedIndex=OwnerDropDownList.Items.Count-1;
					}

					// Populate ShipToDropDownList
					CompanyCollectionClass Customers=Companies.EnumerateByRole(Security,COMPANY_ROLE.CUSTOMER_SHIPTO,true);
					foreach(CompanyClass ShipTo in Customers)
					{
						if(ShipTo.LockedOut
						&& FuelTicket.ShipToID != ShipTo.ID)
							continue;

						ListItem Item=new ListItem(ShipTo.ID,ShipTo.Index.ToString());
						ShipToDropDownList.Items.Add(Item);
						if(FuelTicket.ShipToID == ShipTo.ID)
							ShipToDropDownList.SelectedIndex=ShipToDropDownList.Items.Count-1;
					}

					ShipToDropDownList_SelectedIndexChanged(null,null);

					FlightNumberTextBox.Text=FuelTicket.FlightNumber;

					// Populate the AircraftDropDownList
					EquipmentsClass Equipments=new EquipmentsClass();
					EquipmentCollectionClass AircraftCollection=Equipments.EnumerateByTypeAndProduct(Security,EQUIPMENT_TYPE.AIRCRAFT_TYPE, 0);
					foreach(EquipmentClass Aircraft in AircraftCollection)
					{
						ListItem Item=new ListItem(Aircraft.ID,Aircraft.Index.ToString());
						AircraftDropDownList.Items.Add(Item);
						if(FuelTicket.TailNumber == Aircraft.ID)
							AircraftDropDownList.SelectedIndex=AircraftDropDownList.Items.Count-1;
					}

					AircraftTypeTextBox.Text=FuelTicket.AircraftType;

					// Populate the DestinationDropDownList
					IATACodesClass IATACodes=new IATACodesClass();
					IATACodeCollectionClass IATACodeCollection=IATACodes.Enumerate(Security);
					foreach(IATACodeClass IATACode in IATACodeCollection)
					{
						ListItem Item=new ListItem(IATACode.ID,IATACode.Index.ToString());
						DestinationDropDownList.Items.Add(Item);
						if(FuelTicket.Destination == IATACode.ID)
							DestinationDropDownList.SelectedIndex=DestinationDropDownList.Items.Count-1;
					}

					// Populate the GateDropDownList
					GatesClass Gates=new GatesClass();
					GateCollectionClass GateCollection=Gates.Enumerate(Security);
					foreach(GateClass Gate in GateCollection)
					{
						ListItem Item=new ListItem(Gate.ID,Gate.Index.ToString());
						GateDropDownList.Items.Add(Item);
						if(FuelTicket.Gate == Gate.ID)
							GateDropDownList.SelectedIndex=GateDropDownList.Items.Count-1;
					}

					ArrivalGaugeTextBox.Text=FuelTicket.ArrivalGaugeQuantity.ToString();;
					RequiredGaugeTextBox.Text=FuelTicket.RequiredGaugeQuantity.ToString();;
					FinalGaugeTextBox.Text=FuelTicket.FinalGaugeQuantity.ToString();;

					FTZCheckBox.Checked=FuelTicket.FTZ;
				}
				
				else
				{

					int Index=FuelTicketLineItemsDataGrid.CurrentPageIndex*FuelTicketLineItemsDataGrid.PageSize;
					FuelTicket=(FuelTicketClass) Session["FuelTicket"];
					foreach(DataGridItem Item in FuelTicketLineItemsDataGrid.Items)
					{
						FuelTicketLineItemClass FuelTicketLineItem=FuelTicket.FuelTicketLineItemCollection.Item(Index);

						TextBox DocumentTextBox=(TextBox) Item.FindControl("DocumentTextBox");
						if(DocumentTextBox != null)
							FuelTicketLineItem.DocumentNumber=DocumentTextBox.Text;		

						TextBox PitIDTextBox=(TextBox) Item.FindControl("PitIDTextBox");
						if(PitIDTextBox != null)
							FuelTicketLineItem.PitID=PitIDTextBox.Text;		

						DropDownList EquipmentDropDownList=(DropDownList) Item.FindControl("EquipmentDropDownList");
						if(EquipmentDropDownList != null
						&& EquipmentDropDownList.SelectedIndex != -1)
						{
							FuelTicketLineItem.EquipmentID=EquipmentDropDownList.SelectedItem.Text;
							FuelTicketLineItem.EquipmentIndex=Convert.ToInt32(EquipmentDropDownList.SelectedValue);
						}

						DropDownList OperatorDropDownList=(DropDownList) Item.FindControl("OperatorDropDownList");
						if(OperatorDropDownList != null
						&& OperatorDropDownList.SelectedIndex != -1)
						{
							FuelTicketLineItem.PersonID=OperatorDropDownList.SelectedItem.Text;
							FuelTicketLineItem.PersonIndex=Convert.ToInt32(OperatorDropDownList.SelectedValue);
						}

						DropDownList ProductDropDownList=(DropDownList) Item.FindControl("ProductDropDownList");
						if(ProductDropDownList != null
						&& ProductDropDownList.SelectedIndex != -1)
						{
							FuelTicketLineItem.ProductID=ProductDropDownList.SelectedItem.Text;
							FuelTicketLineItem.ProductIndex=Convert.ToInt32(ProductDropDownList.SelectedValue);
						}

						TextBox MeterStartTextBox=(TextBox) Item.FindControl("MeterStartTextBox");
						if(MeterStartTextBox != null)
							FuelTicketLineItem.MeterStart=System.Convert.ToDouble(MeterStartTextBox.Text);		

						TextBox MeterEndTextBox=(TextBox) Item.FindControl("MeterEndTextBox");
						if(MeterEndTextBox != null)
							FuelTicketLineItem.MeterEnd=System.Convert.ToDouble(MeterEndTextBox.Text);		

						TextBox TotalTextBox=(TextBox) Item.FindControl("TotalTextBox");
						if(TotalTextBox != null)
							FuelTicketLineItem.GrossQuantity=System.Convert.ToDouble(TotalTextBox.Text);		

						TextBox StartTimeTextBox=(TextBox) Item.FindControl("StartTimeTextBox");
						if(StartTimeTextBox != null)
							FuelTicketLineItem.StartTime=DateTime.Parse(DateTextbox.Text+" "+StartTimeTextBox.Text);		

						TextBox StopTimeTextBox=(TextBox) Item.FindControl("StopTimeTextBox");
						if(StopTimeTextBox != null)
							FuelTicketLineItem.StopTime=DateTime.Parse(DateTextbox.Text+" "+StopTimeTextBox.Text);		

						if(FuelTicketLineItem.StopTime < FuelTicketLineItem.StartTime)
							FuelTicketLineItem.StopTime.AddDays(1);

						Index++;
					}
				}
			}
			catch (Exception except)
			{
				ErrorHandler(except);
				Response.End();
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
			this.DateButton.Command += new System.Web.UI.WebControls.CommandEventHandler(this.DateButton_Command);
			this.FuelTicketLineItemsDataGrid.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.FuelTicketLineItemsDataGrid_PageIndexChanged);
			this.FuelTicketLineItemsDataGrid.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.FuelTicketLineItemsDataGrid_DeleteCommand);
			this.FuelTicketLineItemsDataGrid.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.FuelTicketLineItemsDataGrid_ItemDataBound);
			this.AddButton.Command += new System.Web.UI.WebControls.CommandEventHandler(this.AddButton_Command);
			this.SubmitButton.Command += new System.Web.UI.WebControls.CommandEventHandler(this.SubmitButton_Command);
			this.CloseButton.Command += new System.Web.UI.WebControls.CommandEventHandler(this.CloseButton_Command);

		}
		#endregion

		private void DateButton_Command(object sender, System.Web.UI.WebControls.CommandEventArgs e)
		{
			DateCalendar.Visible=true;
			TypeDropDownList.Visible=false;
			ManagerDropDownList.Visible=false;
			OwnerDropDownList.Visible=false;
			ShipToDropDownList.Visible=false;
			CarrierDropDownList.Visible=false;		
			if(DateTextbox.Text != "")		
				DateCalendar.SelectedDate=DateTime.Parse(DateTextbox.Text,CurrentSite.GetDateTimeFormatInfo());
		}

		private void CloseButton_Command(object sender, System.Web.UI.WebControls.CommandEventArgs e)
		{
			if(ManagerDropDownList.SelectedIndex != -1)
				Session["FuelTicketManagerID"]=ManagerDropDownList.SelectedItem.Text;

			if(OwnerDropDownList.SelectedIndex != -1)
				Session["FuelTicketOwnerID"]=OwnerDropDownList.SelectedItem.Text;

			if(CarrierDropDownList.SelectedIndex != -1)
				Session["FuelTicketVendorID"]=CarrierDropDownList.SelectedItem.Text;

			if(ShipToDropDownList.SelectedIndex != -1)
				Session["FuelTicketShipToID"]=ShipToDropDownList.SelectedItem.Text;

			Response.Redirect("FuelTicketsForm.aspx");
		}

		protected void DateCalendar_SelectionChanged(object sender, System.EventArgs e)
		{
			DateCalendar.Visible=false;
			TypeDropDownList.Visible=true;		
			ManagerDropDownList.Visible=true;
			OwnerDropDownList.Visible=true;
			ShipToDropDownList.Visible=true;
			CarrierDropDownList.Visible=true;		
			DateTextbox.Text=DateCalendar.SelectedDate.ToString("d",CurrentSite.GetDateTimeFormatInfo());
		}

		private void SubmitButton_Command(object sender, System.Web.UI.WebControls.CommandEventArgs e)
		{
			FuelTicketClass FuelTicket=(FuelTicketClass) Session["FuelTicket"];

			FuelTicket.Date=DateTime.Parse(DateTextbox.Text,CurrentSite.GetDateTimeFormatInfo());

			if(TypeDropDownList.SelectedIndex != -1)
			{
				FuelTicket.Type=TypeDropDownList.SelectedItem.Text;
				Session["FuelTicketType"]=FuelTicket.Type;
			}

			if(ManagerDropDownList.SelectedIndex != -1)
			{
				FuelTicket.ManagerID=ManagerDropDownList.SelectedItem.Text;
				FuelTicket.ManagerIndex=Convert.ToInt32(ManagerDropDownList.SelectedValue);
				Session["FuelTicketManagerID"]=FuelTicket.ManagerID;
			}

			if(OwnerDropDownList.SelectedIndex != -1)
			{
				FuelTicket.OwnerID=OwnerDropDownList.SelectedItem.Text;
				FuelTicket.OwnerIndex=Convert.ToInt32(OwnerDropDownList.SelectedValue);
				Session["FuelTicketOwnerID"]=FuelTicket.OwnerID;
			}

			if(CarrierDropDownList.SelectedIndex != -1)
			{
				FuelTicket.VendorID=CarrierDropDownList.SelectedItem.Text;
				FuelTicket.VendorIndex=Convert.ToInt32(CarrierDropDownList.SelectedValue);
				Session["FuelTicketVendorID"]=FuelTicket.VendorID;
			}

			if(ShipToDropDownList.SelectedIndex != -1)
			{
				FuelTicket.ShipToID=ShipToDropDownList.SelectedItem.Text;
				FuelTicket.ShipToIndex=Convert.ToInt32(ShipToDropDownList.SelectedValue);
				Session["FuelTicketShipToID"]=FuelTicket.ShipToID;
			}

			FuelTicket.FlightNumber=FlightNumberTextBox.Text;

			if(AircraftDropDownList.SelectedIndex != -1)
			{
				FuelTicket.TailNumber=AircraftDropDownList.SelectedItem.Text;
				FuelTicket.TailNumberIndex=Convert.ToInt32(AircraftDropDownList.SelectedValue);
			}

			FuelTicket.AircraftType=AircraftTypeTextBox.Text;

			if(DestinationDropDownList.SelectedIndex != -1)
			{
				FuelTicket.Destination=DestinationDropDownList.SelectedItem.Text;
				FuelTicket.DestinationIndex=Convert.ToInt32(DestinationDropDownList.SelectedValue);
			}

			if(GateDropDownList.SelectedIndex != -1)
			{
				FuelTicket.Gate=GateDropDownList.SelectedItem.Text;
				FuelTicket.GateIndex=Convert.ToInt32(GateDropDownList.SelectedValue);
			}

			FuelTicket.ArrivalGaugeQuantity=Convert.ToDouble(ArrivalGaugeTextBox.Text);
			FuelTicket.RequiredGaugeQuantity=Convert.ToDouble(RequiredGaugeTextBox.Text);
			FuelTicket.FinalGaugeQuantity=Convert.ToDouble(FinalGaugeTextBox.Text);

			FuelTicket.FTZ=FTZCheckBox.Checked;

			FuelTicketsClass	FuelTickets=new FuelTicketsClass();

			try
			{
				if(FuelTicket.Index == 0)
					FuelTickets.Add(Security,FuelTicket);
				else
					FuelTickets.Modify(Security,FuelTicket);

				Session.Remove("Index");

			}
			catch (Exception except)
			{
				ErrorHandler(except);
				return;
			}

			if(FuelTicket.Index == 0)
				Response.Redirect("FuelTicketForm.aspx");
			else
				Response.Redirect("FuelTicketsForm.aspx");
		}

		protected void ShipToDropDownList_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			try
			{
				CarrierDropDownList.Items.Clear();

				if(ShipToDropDownList.SelectedIndex != -1)
				{
					FuelTicketClass FuelTicket=(FuelTicketClass) Session["FuelTicket"];

					CompaniesClass	Companies=new CompaniesClass();
					CompanyClass	Company=Companies.Get(Security,System.Convert.ToInt32(ShipToDropDownList.SelectedValue));

					// Populate CarrierDropDownList
					foreach(CompanyMapClass Carrier in Company.AuthorizedCarrierCollection)
					{
						if(Carrier.LockedOut
						&& FuelTicket.VendorID != Carrier.AssignedID)
							continue;

						ListItem Item=new ListItem(Carrier.AssignedID,Carrier.AssignedIndex.ToString());
						CarrierDropDownList.Items.Add(Item);
						if(FuelTicket.VendorID == Carrier.AssignedID)
							CarrierDropDownList.SelectedIndex=CarrierDropDownList.Items.Count-1;
					}
				}

				UpdateView();		
			}
			catch (Exception except)
			{
				ErrorHandler(except);
				return;
			}
			
		}

		protected void CarrierDropDownList_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			try
			{
				UpdateView();
			}
			catch (Exception except)
			{
				ErrorHandler(except);
			}
		}

		private void AddButton_Command(object sender, System.Web.UI.WebControls.CommandEventArgs e)
		{
			FuelTicketClass FuelTicket=(FuelTicketClass) Session["FuelTicket"];
			FuelTicketLineItemClass FuelTicketLineItem=new FuelTicketLineItemClass();
			FuelTicketLineItem.ItemNumber=(byte) FuelTicket.FuelTicketLineItemCollection.Count;
			FuelTicket.FuelTicketLineItemCollection.Add(FuelTicketLineItem);

			if(FuelTicketLineItemsDataGrid.Items.Count < FuelTicketLineItemsDataGrid.PageSize)
				FuelTicketLineItemsDataGrid.EditItemIndex=FuelTicketLineItemsDataGrid.Items.Count;
			else
			{
				FuelTicketLineItemsDataGrid.EditItemIndex=0;
				FuelTicketLineItemsDataGrid.CurrentPageIndex++;
			}

			try
			{
				UpdateView();		
			}
			catch (Exception except)
			{
				ErrorHandler(except);
				FuelTicket.FuelTicketLineItemCollection.Remove(FuelTicket.FuelTicketLineItemCollection.Count-1);
				FuelTicketLineItemsDataGrid.EditItemIndex=-1;
				UpdateView();		
			}
		}


		private void FuelTicketLineItemsDataGrid_DeleteCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			FuelTicketClass FuelTicket=(FuelTicketClass) Session["FuelTicket"];
			int Index=e.Item.ItemIndex+FuelTicketLineItemsDataGrid.CurrentPageIndex*FuelTicketLineItemsDataGrid.PageSize;

			FuelTicket.FuelTicketLineItemCollection.Remove(Index);

			byte ItemNumber=0;
			foreach(FuelTicketLineItemClass LineItem in FuelTicket.FuelTicketLineItemCollection)
			{
				LineItem.ItemNumber=ItemNumber;
				ItemNumber++;
			}

			if(FuelTicketLineItemsDataGrid.Items.Count == 1
			&& FuelTicketLineItemsDataGrid.CurrentPageIndex > 0)
				FuelTicketLineItemsDataGrid.CurrentPageIndex--;

			UpdateView();
		}


		private void FuelTicketLineItemsDataGrid_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			// if we are editing do not allow a page change
			if (FuelTicketLineItemsDataGrid.EditItemIndex > -1)
				return;
			FuelTicketLineItemsDataGrid.CurrentPageIndex = e.NewPageIndex;
			UpdateView();
		}

		private void FuelTicketLineItemsDataGrid_ItemDataBound(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
		{
			if(e.Item.ItemIndex < 0 
			|| e.Item.ItemIndex > FuelTicketLineItemsDataGrid.PageSize)
				return;

			FuelTicketClass FuelTicket=(FuelTicketClass) Session["FuelTicket"];

			int Index=e.Item.ItemIndex+FuelTicketLineItemsDataGrid.CurrentPageIndex*FuelTicketLineItemsDataGrid.PageSize;
			FuelTicketLineItemClass FuelTicketLineItem=FuelTicket.FuelTicketLineItemCollection.Item(Index);

			TextBox DocumentTextBox=(TextBox) e.Item.FindControl("DocumentTextBox");
			if(DocumentTextBox != null)
				DocumentTextBox.TabIndex=Convert.ToInt16(20+Index*10);		

			TextBox PitIDTextBox=(TextBox) e.Item.FindControl("PitIDTextBox");
			if(PitIDTextBox != null)
				PitIDTextBox.TabIndex=Convert.ToInt16(21+Index*10);		


			DropDownList EquipmentDropDownList=(DropDownList) e.Item.FindControl("EquipmentDropDownList");
			if(EquipmentDropDownList != null)
			{
				EquipmentDropDownList.TabIndex=Convert.ToInt16(22+Index*10);
				ListItem Item=EquipmentDropDownList.Items.FindByText(FuelTicketLineItem.EquipmentID);
				if(Item != null)
					Item.Selected=true;
			}

			DropDownList OperatorDropDownList=(DropDownList) e.Item.FindControl("OperatorDropDownList");
			if(OperatorDropDownList != null)
			{
				OperatorDropDownList.TabIndex=Convert.ToInt16(23+Index*10);
				ListItem Item=OperatorDropDownList.Items.FindByText(FuelTicketLineItem.PersonID);
				if(Item != null)
					Item.Selected=true;
			}

			DropDownList ProductDropDownList=(DropDownList) e.Item.FindControl("ProductDropDownList");
			if(ProductDropDownList != null)
			{
				ProductDropDownList.TabIndex=Convert.ToInt16(24+Index*10);
				ListItem Item=ProductDropDownList.Items.FindByText(FuelTicketLineItem.ProductID);
				if(Item != null)
					Item.Selected=true;
			}

			TextBox MeterStartTextBox=(TextBox) e.Item.FindControl("MeterStartTextBox");
			if(MeterStartTextBox != null)
				MeterStartTextBox.TabIndex=Convert.ToInt16(25+Index*10);

			TextBox MeterEndTextBox=(TextBox) e.Item.FindControl("MeterEndTextBox");
			if(MeterEndTextBox != null)
				MeterEndTextBox.TabIndex=Convert.ToInt16(26+Index*10);		

			TextBox TotalTextBox=(TextBox) e.Item.FindControl("TotalTextBox");
			if(TotalTextBox != null)
				TotalTextBox.TabIndex=Convert.ToInt16(27+Index*10);		

			TextBox StartTimeTextBox=(TextBox) e.Item.FindControl("StartTimeTextBox");
			if(StartTimeTextBox != null)
				StartTimeTextBox.TabIndex=Convert.ToInt16(28+Index*10);		

			TextBox StopTimeTextBox=(TextBox) e.Item.FindControl("StopTimeTextBox");
			if(StopTimeTextBox != null)
				StopTimeTextBox.TabIndex=Convert.ToInt16(29+Index*10);		
		}
	}
}
