/******************************************************************************

	FILE NAME:		PermissivesForm.aspx.cs


	PURPOSE:			Implementation of PermissivesForm


	COMMENTS:

		Copyright (C) Varec, Inc. Norcross, GA, USA, 2008

		This file shall not be copied or reproduced in any form without
				the express written consent of Varec.


	AUTHOR(S):	W. Gray


	VERSION:		1.0.0  Current version



	MODIFICATION HISTORY:
		Date:			By:			Reason:
		---------	----------  -------------------------------------------

*******************************************************************************/

namespace FuelsManager.FMWebApp
{
    using System;
    using System.Collections;
    using System.Data;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Web.UI.WebControls;

    using FMBusinessObjects.DataObjects;

    using FMCore;

    using Opc;

    using Convert = System.Convert;

    /// <summary>
	/// Summary description for PermissivesForm.
	/// </summary>
	public partial class PermissivesForm : FMFormBase
	{


		private void UpdateOutputPermissivesView()
		{
			PermissivesClass permissives=this.GetPermissives();
			this.OutputPermissivesDataGrid.DataSource=this.PermissivesView(permissives.Outputs);
			this.Session["OutputPermissivesDataGrid.CurrentPageIndex"]=this.OutputPermissivesDataGrid.CurrentPageIndex;
			this.OutputPermissivesDataGrid.DataBind();
		}

		private void UpdateInputPermissivesView()
		{
			PermissivesClass permissives = this.GetPermissives();
			this.InputPermissivesDataGrid.DataSource=this.PermissivesView(permissives.Inputs);
			this.Session["InputPermissivesDataGrid.CurrentPageIndex"]=this.InputPermissivesDataGrid.CurrentPageIndex;
			this.InputPermissivesDataGrid.DataBind();
		}

		private ICollection PermissivesView(ProcessVariableCollectionClass permissives)
		{
			DataTable			pvDataTable=new DataTable();

		    pvDataTable.Columns.Add("Index",typeof(int));
			pvDataTable.Columns.Add("Host",typeof(string));
			pvDataTable.Columns.Add("OPCServerID",typeof(string));
			pvDataTable.Columns.Add("OPCItemID",typeof(string));
			pvDataTable.Columns.Add("MessageID",typeof(string));

			int item=0;
			foreach(ProcessVariableClass permissive in permissives)
			{
			    var pv = this.Session["ProcessVariable"] as ProcessVariableClass;
			    if (pv != null
				&& pv.ProcessVariableType == permissive.ProcessVariableType
				&& pv.InstanceNumber == permissive.InstanceNumber)
				{
					var editedProcessVariable = pv;
					permissive.Load(editedProcessVariable);
					this.Session.Remove("ProcessVariable");
				}

				var				pvDataRow = pvDataTable.NewRow();

				pvDataRow["Index"] = item;
				URL url=new URL( permissive.URL );
				pvDataRow["Host"] = url.HostName;
				pvDataRow["OPCServerID"] = permissive.ProgID;
				pvDataRow["OPCItemID"] = permissive.OPCItemID;
				pvDataRow["MessageID"] = permissive.MessageID;
				pvDataTable.Rows.Add(pvDataRow);
				item++;
			}

			DataView		pvDataView=new DataView(pvDataTable);
			return pvDataView;
		}


		protected PermissivesClass GetPermissives()
		{
			var station = this.Session["Station"] as StationClass;
			if (station == null)
			{
				throw new Exception("No Station in Session");
			}

			var mode = this.Session["PermissivesConfigurationMode"] as string;
			if(string.IsNullOrEmpty(mode))
			{
				throw new Exception("No PermissivesConfigurationMode in Session");
			}

			switch (mode)
			{
			    case "StationPermissives":
			        return station.StationPermissives;
			    case "LoadArmPermissives":
			    {
			        LoadArmClass loadArm = station.LoadArmCollection[(int)this.Session["LoadArmIndex"]];
			        if (loadArm == null)
			            throw new Exception("No Load Arm in Session");

			        return loadArm.LoadArmPermissives;
			    }
			    case "NoAdditivePermissives":
			    {
			        LoadArmClass loadArm = station.LoadArmCollection[(int)this.Session["LoadArmIndex"]];
			        if (loadArm == null)
			            throw new Exception("No Load Arm in Session");

			        return loadArm.NoAdditivePermissives;
			    }
			    case "LoadArmComponent":
			    case "LoadArmAdditive":
			    case "LoadArmRecipe":
			    case "LoadArmExternalComponent":
                case "OffLoadExternalProduct":
			    {
			        var productMapIndex = this.Session["PermissivesConfigurationProductMapIndex"] as string;
			        if (string.IsNullOrEmpty(productMapIndex))
			        {
			            throw new Exception("No PermissivesConfigurationProductMapIndex in Session");
			        }

	
			        LoadArmClass loadArm = station.LoadArmCollection[(int)this.Session["LoadArmIndex"]];
			        if (loadArm == null) throw new Exception("No Load Arm in Session");

			        ProductMapClass productMap = null;

			        switch (mode)
			        {
			            case "LoadArmComponent":
			                if (Convert.ToInt32(productMapIndex, CultureInfo.InvariantCulture) > loadArm.ComponentCollection.Count - 1)
			                {
			                    throw new Exception("Index out of range");
			                }

			                productMap = loadArm.ComponentCollection[Convert.ToInt32(productMapIndex, CultureInfo.InvariantCulture)];
			                break;
			            case "LoadArmAdditive":
			                if (Convert.ToInt32(productMapIndex, CultureInfo.InvariantCulture)
			                    > loadArm.AdditiveInjectorCollection.Count - 1)
			                {
			                    throw new Exception("Index out of range");
			                }

			                productMap = loadArm.AdditiveInjectorCollection[Convert.ToInt32(productMapIndex, CultureInfo.InvariantCulture)];
			                break;
			            case "LoadArmRecipe":
			                if (Convert.ToInt32(productMapIndex) > loadArm.ProductRecipeCollection.Count - 1)
			                {
			                    throw new Exception("Index out of range");
			                }

			                productMap = loadArm.ProductRecipeCollection[Convert.ToInt32(productMapIndex, CultureInfo.InvariantCulture)];
			                break;
			            case "LoadArmExternalComponent":
			                if (Convert.ToInt32(productMapIndex, CultureInfo.InvariantCulture)
			                    > loadArm.ExternalComponentCollection.Count - 1)
			                {
			                    throw new Exception("Index out of range");
			                }

			                productMap = loadArm.ExternalComponentCollection[Convert.ToInt32(productMapIndex, CultureInfo.InvariantCulture)];
			                break;
			            case "OffLoadExternalProduct":
			                if (Convert.ToInt32(productMapIndex, CultureInfo.InvariantCulture) > loadArm.OffloadExternalProductCollection.Count - 1)
			                {
			                    throw new Exception("Index out of range");
			                }

			                productMap = loadArm.OffloadExternalProductCollection[Convert.ToInt32(productMapIndex, CultureInfo.InvariantCulture)];
			                break;
			        }

			        if (productMap != null)
			        {
			            return productMap.Permissives;
			        }
			    }
			        break;
			}

			return null;
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				if (! this.Page.IsPostBack) 
				{
					string mode=this.Request.GetQueryOrFormValue("Mode");
					if(mode != null)
					{
						this.Session["PermissivesConfigurationMode"] = mode;
						if (mode == "StationPermissives")
						{
							this.ConfigurationLabel.Text+=" : "+this.GetTranslatedText("Station");
						}

						else if(mode == "LoadArmPermissives")
						{
							this.ConfigurationLabel.Text+=" : "+this.GetTranslatedText("Arm");
						}

						else if(mode == "NoAdditivePermissives")
						{
							this.ConfigurationLabel.Text+=" : "+this.GetTranslatedText("No Additive");
						}


						else if(mode == "LoadArmComponent"
						|| mode == "LoadArmAdditive"
						|| mode == "LoadArmRecipe"
						|| mode == "LoadArmExternalComponent"
                        || mode == "OffLoadExternalProduct")
						{
							StationClass station = this.Session["Station"] as StationClass;
							if (station == null)
								throw new Exception("No Station in Session");

							LoadArmClass loadArm = station.LoadArmCollection[(int)this.Session["LoadArmIndex"]];
							if (loadArm == null)
								throw new Exception("No Load Arm in Session");
						
							string index=this.Request.GetQueryOrFormValue("Index");
							if(string.IsNullOrEmpty(index))
								throw new Exception("No Index in Request.");

							this.Session["PermissivesConfigurationProductMapIndex"] = index;

							ProductMapClass productMap;
							switch (mode)
							{
							    case "LoadArmComponent":
							        if (Convert.ToInt32(index, CultureInfo.InvariantCulture) > loadArm.ComponentCollection.Count - 1)
							            throw new Exception("Index out of range");

							        productMap = loadArm.ComponentCollection[Convert.ToInt32(index, CultureInfo.InvariantCulture)];

							        this.ConfigurationLabel.Text+=" : "+this.GetTranslatedText("Component")+" "+productMap.AssignedID;
							        break;
							    case "LoadArmAdditive":
							        if (Convert.ToInt32(index, CultureInfo.InvariantCulture) > loadArm.AdditiveInjectorCollection.Count - 1)
							            throw new Exception("Index out of range");

							        productMap = loadArm.AdditiveInjectorCollection[Convert.ToInt32(index, CultureInfo.InvariantCulture)];

							        this.ConfigurationLabel.Text+=" : "+this.GetTranslatedText("Additive")+" "+productMap.AssignedID;
							        break;
							    case "LoadArmRecipe":
							        if(Convert.ToInt32(index) > loadArm.ProductRecipeCollection.Count-1)
							            throw new Exception("Index out of range");

							        productMap = loadArm.ProductRecipeCollection[Convert.ToInt32(index, CultureInfo.InvariantCulture)];

							        this.ConfigurationLabel.Text+=" : "+this.GetTranslatedText("Recipe")+" "+productMap.AssignedID;
							        break;
							    case "LoadArmExternalComponent":
							        if (Convert.ToInt32(index, CultureInfo.InvariantCulture) > loadArm.ExternalComponentCollection.Count - 1)
							            throw new Exception("Index out of range");

							        productMap = loadArm.ExternalComponentCollection[Convert.ToInt32(index, CultureInfo.InvariantCulture)];

							        this.ConfigurationLabel.Text+=" : "+this.GetTranslatedText("External Component")+" "+productMap.AssignedID;
							        break;
							    case "OffLoadExternalProduct":
							        if (Convert.ToInt32(index, CultureInfo.InvariantCulture) > loadArm.OffloadExternalProductCollection.Count - 1)
							        {
							            throw new Exception("Index out of range");
							        }

							        productMap = loadArm.OffloadExternalProductCollection[Convert.ToInt32(index, CultureInfo.InvariantCulture)];

							        this.ConfigurationLabel.Text += " : " + this.GetTranslatedText("Offload Product") + " " + productMap.AssignedID;
							        break;
							}
                        }

						this.Session["PermissivesConfigurationLabel"] = this.ConfigurationLabel.Text;
					}
					else
					{
						if(this.Session["PermissivesConfigurationLabel"] == null)
							throw new Exception("Permissives not in session");

						this.ConfigurationLabel.Text=this.Session["PermissivesConfigurationLabel"] as String;
						this.OutputPermissivesDataGrid.CurrentPageIndex=(int) this.Session["OutputPermissivesDataGrid.CurrentPageIndex"];
						this.InputPermissivesDataGrid.CurrentPageIndex=(int) this.Session["InputPermissivesDataGrid.CurrentPageIndex"];

					}

					this.UpdateOutputPermissivesView();
					this.UpdateInputPermissivesView();
				}
			}

			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.OutputPermissivesDataGrid.EditCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.OutputPermissivesDataGridEditCommand);
			this.OutputPermissivesDataGrid.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.OutputPermissivesDataGridPageIndexChanged);
			this.OutputPermissivesDataGrid.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.OutputPermissivesDataGridDeleteCommand);
			this.AddOutputPermissiveButton.Command += new System.Web.UI.WebControls.CommandEventHandler(this.AddOutputPermissiveButtonCommand);
			this.InputPermissivesDataGrid.EditCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.InputPermissivesDataGridEditCommand);
			this.InputPermissivesDataGrid.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.InputPermissivesDataGridPageIndexChanged);
			this.InputPermissivesDataGrid.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.InputPermissivesDataGridDeleteCommand);
			this.AddInputPermissiveButton.Command += new System.Web.UI.WebControls.CommandEventHandler(this.AddInputPermissiveButtonCommand);

		}
		#endregion


		private void OutputPermissivesDataGridEditCommand(object source, DataGridCommandEventArgs e)
		{
			Label			indexLabel=(Label) e.Item.FindControl("OutputPermissiveIndexLabel");
			if(indexLabel != null)
			{
				this.Session["UnitForm"]="PermissivesForm.aspx";
				PermissivesClass permissives = this.GetPermissives();
				this.Session["ProcessVariable"] = permissives.Outputs[Convert.ToInt32(indexLabel.Text, CultureInfo.InvariantCulture)];
				this.Redirect("OPCConnectionForm.aspx");
			}
		}

		private void OutputPermissivesDataGridDeleteCommand(object source, DataGridCommandEventArgs e)
		{
			Label			indexLabel=(Label) e.Item.FindControl("OutputPermissiveIndexLabel");
			if(indexLabel != null)
			{
				PermissivesClass permissives = this.GetPermissives();
				permissives.Outputs.Remove(Convert.ToInt32(indexLabel.Text, CultureInfo.InvariantCulture));

				int instanceNumber=0;
				foreach(ProcessVariableClass permissive in permissives.Outputs)
					permissive.InstanceNumber=instanceNumber++;

				if(this.OutputPermissivesDataGrid.Items.Count == 1
				&& this.OutputPermissivesDataGrid.CurrentPageIndex > 0)
					this.OutputPermissivesDataGrid.CurrentPageIndex--;

				this.UpdateOutputPermissivesView();
			}
		}

		private void AddOutputPermissiveButtonCommand(object sender, CommandEventArgs e)
		{
			PermissivesClass permissives = this.GetPermissives();

		    ProcessVariableClass pv = new ProcessVariableClass
		                              {
		                                  Input = false,
		                                  DataType = VarEnum.VT_BOOL,
		                                  DataTypeEnabled = false,
		                                  UnitType = permissives.OutputUnitType,
		                                  ProcessVariableType = PROCESS_VARIABLE_TYPE.OUTPUT_PERMISSIVE_PV,
		                                  InstanceNumber = permissives.Outputs.Count,
		                                  Parent = permissives
		                              };
		    permissives.Outputs.Add(pv);
			this.Session["UnitForm"]="PermissivesForm.aspx";
			this.Session["ProcessVariable"]=pv;
			this.Redirect("OPCConnectionForm.aspx");
		}

		private void OutputPermissivesDataGridPageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			// if we are editing do not allow a page change
			if (this.OutputPermissivesDataGrid.EditItemIndex > -1)
				return;
			this.OutputPermissivesDataGrid.CurrentPageIndex = e.NewPageIndex;
			this.UpdateOutputPermissivesView();
		}

		private void InputPermissivesDataGridEditCommand(object source, DataGridCommandEventArgs e)
		{
			Label			indexLabel=(Label) e.Item.FindControl("InputPermissiveIndexLabel");
			if(indexLabel != null)
			{
				this.Session["UnitForm"]="PermissivesForm.aspx";
				PermissivesClass permissives = this.GetPermissives();
				this.Session["ProcessVariable"] = permissives.Inputs[Convert.ToInt32(indexLabel.Text, CultureInfo.InvariantCulture)];
				this.Redirect("OPCConnectionForm.aspx");
			}
		}

		private void InputPermissivesDataGridDeleteCommand(object source, DataGridCommandEventArgs e)
		{
			Label			indexLabel=(Label) e.Item.FindControl("InputPermissiveIndexLabel");
			if(indexLabel != null)
			{
				PermissivesClass permissives = this.GetPermissives();
				permissives.Inputs.Remove(Convert.ToInt32(indexLabel.Text, CultureInfo.InvariantCulture));

				int instanceNumber=0;
				foreach(ProcessVariableClass permissive in permissives.Inputs)
					permissive.InstanceNumber=instanceNumber++;

				if(this.InputPermissivesDataGrid.Items.Count == 1
				&& this.InputPermissivesDataGrid.CurrentPageIndex > 0)
					this.InputPermissivesDataGrid.CurrentPageIndex--;

				this.UpdateInputPermissivesView();
			}
		}

		private void AddInputPermissiveButtonCommand(object sender, CommandEventArgs e)
		{
			PermissivesClass permissives=this.GetPermissives();

		    ProcessVariableClass pv = new ProcessVariableClass
		                              {
		                                  Input = true,
		                                  DataType = VarEnum.VT_BOOL,
		                                  DataTypeEnabled = false,
		                                  UnitType = permissives.InputUnitType,
		                                  ProcessVariableType = PROCESS_VARIABLE_TYPE.INPUT_PERMISSIVE_PV,
		                                  InstanceNumber = permissives.Inputs.Count,
		                                  Parent = permissives
		                              };
		    permissives.Inputs.Add(pv);
			this.Session["UnitForm"]="PermissivesForm.aspx";
			this.Session["ProcessVariable"]=pv;
			this.Redirect("OPCConnectionForm.aspx");
		}

		private void InputPermissivesDataGridPageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			// if we are editing do not allow a page change
			if (this.InputPermissivesDataGrid.EditItemIndex > -1)
				return;
			this.InputPermissivesDataGrid.CurrentPageIndex = e.NewPageIndex;
			this.UpdateInputPermissivesView();
		}
	}
}
