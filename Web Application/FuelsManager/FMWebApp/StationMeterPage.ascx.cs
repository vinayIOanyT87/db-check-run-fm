// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StationMeterPage.ascx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the StationMeterPage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.UtilityObjects;

	/// <summary>
	///    Summary description for StationMeterPage.
	/// </summary>
	public partial class StationMeterPage : FMUserControlBase
	{
		#region Methods

		protected override void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				var Station = (StationClass)this.Session["Station"];
				if (Station.Type != STATION_TYPE.METER)
				{
					return;
				}

				if (!this.Page.IsPostBack)
				{
					this.UpdateProcessVariablesView();

					TankCollectionClass tankCollection = FMChannelHelper.MakeCall<ITanks, TankCollectionClass>(
																	 x =>
																	 x.Enumerate(this.Security)
																);


					var newItem = new ListItem("{None}", Guid.Empty.ToString());
					this.AssociatedTanks.Items.Insert(0, newItem);
					if (Station.AssociatedTankGuid.IsEmpty())
					{
						this.AssociatedTanks.SelectedIndex = this.AssociatedTanks.Items.Count - 1;
					}

					foreach (TankClass tank in tankCollection)
					{
						newItem = new ListItem(tank.ID, tank.IdentityGuid.ToString());
						foreach (ListItem existingItem in this.AssociatedTanks.Items)
						{
							if (existingItem.Text.CompareTo(newItem.Text) > 0)
							{
								int index = this.AssociatedTanks.Items.IndexOf(existingItem);
								this.AssociatedTanks.Items.Insert(index, newItem);
								if (Station.AssociatedTankGuid == tank.IdentityGuid)
								{
									this.AssociatedTanks.SelectedIndex = index;
								}
								newItem = null;
								break;
							}
						}

						if (newItem != null)
						{
							this.AssociatedTanks.Items.Add(newItem);
							if (Station.AssociatedTankGuid == tank.IdentityGuid)
							{
								this.AssociatedTanks.SelectedIndex = this.AssociatedTanks.Items.Count - 1;
							}
						}
					}

					this.ArmsServiced.Text = Station.ArmsServiced;
				    this.MeterIDTextBox.Text = Station.Meter.ID;
				    this.NumberOfDigitsTextBox.Text = Station.Meter.NumberOfDigits.ToString();
				    this.RotatesBackwardCheckBox.Checked = Station.Meter.RotatesBackwardsFlag;
				    this.ReceiptMeterCheckBox.Checked = Station.Meter.ReceiptMeterFlag;
					this.MeterFactorTextBox.Text = Station.Meter.MeterFactor?.ToString("F4") ?? "1.0000";
					this.FuelCompressionFactorTextBox.Text = Station.Meter.FuelCompressionFactor?.ToString("F4") ?? "1.0000";
				}
				else
				{
					Station.ArmsServiced = this.ArmsServiced.Text;

					Station.AssociatedTankGuid = new Guid(this.AssociatedTanks.SelectedValue);
					Station.AssociatedTankId = this.AssociatedTanks.SelectedItem.Text;

                    Station.Meter.ID = MeterClass.ValidateMeterID(this.MeterIDTextBox.Text);
                    Station.Meter.NumberOfDigits = MeterClass.ValidateNumberOfDigits(this.NumberOfDigitsTextBox.Text);
                    Station.Meter.RotatesBackwardsFlag = this.RotatesBackwardCheckBox.Checked;
                    Station.Meter.ReceiptMeterFlag = this.ReceiptMeterCheckBox.Checked;
					Station.Meter.MeterFactor = MeterClass.ValidateMeterFactor(this.MeterFactorTextBox.Text);
					Station.Meter.FuelCompressionFactor = MeterClass.ValidateFuelCompressionFactor(this.FuelCompressionFactorTextBox.Text);
                }
			}

			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.ProcessVariablesDataGrid.EditCommand +=
				new System.Web.UI.WebControls.DataGridCommandEventHandler(this.ProcessVariablesDataGrid_EditCommand);
		}

		private void ProcessVariablesDataGrid_EditCommand(object source, DataGridCommandEventArgs e)
		{
			((StationForm)this.Page).UpdateData();
			this.Session["UnitForm"] = "StationForm.aspx";
			var Station = (StationClass)this.Session["Station"];
			this.Session["TabIndex"] = 1;
			this.Session["ProcessVariable"] = Station.ProcessVariableCollection[Convert.ToInt32(e.Item.Cells[1].Text)];
			this.Redirect("OPCConnectionForm.aspx");
		}

		private ICollection ProcessVariablesView()
		{
			var PVDataTable = new DataTable();
			DataRow PVDataRow;

			PVDataTable.Columns.Add("Index", typeof(Int32));
			PVDataTable.Columns.Add("OPCServerID", typeof(string));
			PVDataTable.Columns.Add("OPCItemID", typeof(string));

			var Station = (StationClass)this.Session["Station"];
			int Item = 0;
			foreach (ProcessVariableClass ProcessVariable in Station.ProcessVariableCollection)
			{
				if (ProcessVariable.ProcessVariableType != PROCESS_VARIABLE_TYPE.COMPONENT_METER_FLOW_TOTAL_PV)
				{
					Item++;
					continue;
				}

				if (Session["ProcessVariable"] is ProcessVariableClass
				&& (Session["ProcessVariable"] as ProcessVariableClass).ProcessVariableType == ProcessVariable.ProcessVariableType
				&& (Session["ProcessVariable"] as ProcessVariableClass).InstanceNumber == ProcessVariable.InstanceNumber)
				{
					var editedProcessVariable = Session["ProcessVariable"] as ProcessVariableClass;
					ProcessVariable.Load(editedProcessVariable);
					Session.Remove("ProcessVariable");
				}

				PVDataRow = PVDataTable.NewRow();

				PVDataRow["Index"] = Item;
				PVDataRow["OPCServerID"] = ProcessVariable.ProgID;
				PVDataRow["OPCItemID"] = ProcessVariable.OPCItemID;
				PVDataTable.Rows.Add(PVDataRow);
				Item++;
			}

			var PVDataView = new DataView(PVDataTable);
			return PVDataView;
		}

		private void UpdateProcessVariablesView()
		{
			this.ProcessVariablesDataGrid.DataSource = this.ProcessVariablesView();
			this.ProcessVariablesDataGrid.DataBind();
		}
            
	    public void UpdateData()
	    {
            var Station = (StationClass)this.Session["Station"];

	        Station.Meter.ID = MeterClass.ValidateMeterID(this.MeterIDTextBox.Text);
	        Station.Meter.NumberOfDigits = MeterClass.ValidateNumberOfDigits(this.NumberOfDigitsTextBox.Text);
	        Station.Meter.RotatesBackwardsFlag = this.RotatesBackwardCheckBox.Checked;
	        Station.Meter.ReceiptMeterFlag = this.ReceiptMeterCheckBox.Checked;
			Station.Meter.MeterFactor = MeterClass.ValidateMeterFactor(this.MeterFactorTextBox.Text);
			Station.Meter.FuelCompressionFactor = MeterClass.ValidateFuelCompressionFactor(this.FuelCompressionFactorTextBox.Text);
		}

		#endregion
	}
}