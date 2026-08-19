// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TankSelectForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the TankSelectForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Web;
	using System.Web.UI.HtmlControls;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

    using FMCore;

	public partial class TankSelectForm : FMFormBase
	{
        /// <summary>
        /// Retain the state of the Show Hidden checkbox
        /// </summary>
        private bool SessionTankSelectHideHidden
        {
            get
            {
                if (this.Session["TankSelectHideHidden"] is bool)
                {
                    return (bool)this.Session["TankSelectHideHidden"];
                }
                else
                {
                    return false;
                }
            }

            set
            {
                this.Session.Add("TankSelectHideHidden", value);
            }
        }

		#region Methods

		protected void FindAllBtn_OnClick(object sender, EventArgs e)
		{
			this.Session.Remove("TankSelectForm.SearchString");
			this.FindTextBox.Text = "";
			this.UpdateView();
		}

		protected void FindBtn_OnClick(object sender, EventArgs e)
		{
			if (this.FindTextBox.Text.Length < 1)
			{
				this.Session.Remove("TankSelectForm.SearchString");
			}
			else
			{
				this.Session["TankSelectForm.SearchString"] = this.FindTextBox.Text.ToUpper();
			}

			this.UpdateView();
		}

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

				if (this.Page.IsPostBack == false)
				{
					if (this.Request.GetQueryOrFormValue("Type") != null)
					{
						this.Session["TankSelectForm.Type"] = (VESSEL_TYPE)Enum.Parse(typeof(VESSEL_TYPE), this.Request.GetQueryOrFormValue("Type"));
					}
					else
					{
						this.Session.Remove("TankSelectForm.Type");
					}

					if (this.Request.GetQueryOrFormValue("All") != null)
					{
						this.Session["TankSelectForm.All"] = true;
					}
					else
					{
						this.Session.Remove("TankSelectForm.All");
					}

					if (this.Request.GetQueryOrFormValue("Unassigned") != null)
					{
						this.Session["TankSelectForm.Unassigned"] = true;
					}
					else
					{
						this.Session.Remove("TankSelectForm.UnAssigned");
					}

					if (this.Request.GetQueryOrFormValue("Mode") != null)
					{
						this.Session["TankSelectForm.Mode"] = this.Request.GetQueryOrFormValue("Mode");
					}
					else
					{
						this.Session.Remove("TankSelectForm.Mode");
					}

					// Used to link the product to the tank. Contains the product ID.
					if (this.Request.GetQueryOrFormValue("IDProductLink") != null)
					{
						this.Session["TankSelectForm.IDProductLink"] = this.Request.GetQueryOrFormValue("IDProductLink");
					}
					else
					{
						this.Session.Remove("TankSelectForm.IDProductLink");
					}

					// Used to link the manager to the tank. Contains the manager ID.
					if (this.Request.GetQueryOrFormValue("IDManagerLink") != null)
					{
						this.Session["TankSelectForm.IDManagerLink"] = this.Request.GetQueryOrFormValue("IDManagerLink");
					}
					else
					{
						this.Session.Remove("TankSelectForm.IDManagerLink");
					}

                    if (this.Request.GetQueryOrFormValue("HideHidden") != null)
                    {
                        this.SessionTankSelectHideHidden = Convert.ToBoolean(this.Request.GetQueryOrFormValue("HideHidden"));
                    }
                    else
                    {
                        this.SessionTankSelectHideHidden = false;
                    }

					this.UpdateView();
				}

				if (this.Session["TankSelectForm.Mode"] != null)
				{
					var form1 = (HtmlForm)this.FindControl("Form1");
					var okButton = new HtmlInputButton();

					okButton.Attributes.Add("value", this.GetTranslatedText("OK"));
					okButton.Attributes.Add("id", "OkButton");
					okButton.Attributes.Add("class", "formfieldtitle");
					okButton.Attributes.Add("onclick", "MultipleSelect()");
					okButton.Attributes.Add("style", "width:66px;Z-INDEX: 107; LEFT: 662px; POSITION: absolute; TOP: 8px");
					form1.Controls.Add(okButton);

					var cancelButton = new HtmlInputButton();
					cancelButton.Attributes.Add("value", this.GetTranslatedText("Cancel"));
					cancelButton.Attributes.Add("id", "CancelButton");
					cancelButton.Attributes.Add("class", "formfieldtitle");
					cancelButton.Attributes.Add("onclick", "NoSelect()");
					cancelButton.Attributes.Add("style", "width:66px;Z-INDEX: 107; LEFT: 758px; POSITION: absolute; TOP: 8px");
					form1.Controls.Add(cancelButton);
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    This method will determine if there is a linked product ID and filter the
		///    tank list to the associated tanks of the product.
		/// </summary>
		/// <param name="tankCollection"></param>
		private void FilterByProductID(TankCollectionClass tankCollection)
		{
			bool setEmptyFlag = false;

			if (this.Session["TankSelectForm.IDProductLink"] != null)
			{
				var productID = this.Session["TankSelectForm.IDProductLink"] as string;

				if (!string.IsNullOrEmpty(productID))
				{
					tankCollection.Clear();

					TankCollectionClass tankProdCollection;

					if (this.FindTextBox.Text != "")
					{
						tankProdCollection = FMChannelHelper.MakeCall<ITanks, TankCollectionClass>(
															x =>
                                                            x.EnumerateByProductAndFilter(this.Security, this.GetProductMasterGuid(productID), this.FindTextBox.Text, this.SessionTankSelectHideHidden)
													);
					}
					else
					{
						tankProdCollection = FMChannelHelper.MakeCall<ITanks, TankCollectionClass>(
															x =>
                                                            x.EnumerateByProduct(this.Security, this.GetProductMasterGuid(productID), this.SessionTankSelectHideHidden)
													);
					}

					if ((tankProdCollection == null) || (tankProdCollection.Count <= 0))
					{
						setEmptyFlag = true;
					}
					else
					{
						var managerID = this.Session["TankSelectForm.IDManagerLink"] as string;

						foreach (TankClass tank in tankProdCollection)
						{
							// vthompson 9/23/2008
							// This is a quick fix for ADF and will probably need to be better
							// implemented elsewhere.
							if (managerID == "")
							{
								tankCollection.Add(tank);
								continue;
							}

							if (managerID != null && managerID != tank.ManagerID)
							{
								continue;
							}

							tankCollection.Add(tank);
						}
					}

					if (setEmptyFlag)
					{
						if (this.Session["TankSelectForm.Unassigned"] != null)
						{
						    var tank = new TankClass { ID = HttpUtility.HtmlEncode(this.GetTranslatedText("{Unassigned}")) };
						    tankCollection.Add(tank);
						}
						else
						{
						    var tank = new TankClass { IdentityGuid = Guid.Empty, ID = "", ProductID = "", ManagerID = "" };
						    tankCollection.Add(tank);
						}
					}
				}

				else
				{
					var managerID = this.Session["TankSelectForm.IDManagerLink"] as string;

					for (int item = 0; item < tankCollection.Count; item++)
					{
						if (managerID != null && managerID != tankCollection[item].ManagerID)
						{
							tankCollection.RemoveAt(item);
							item--;
						}
					}
				}
			}
		}

		/// <summary>
		///    This method will retrieve the product Guid in order to find
		///    the associated tanks.
		/// </summary>
		/// <param name="productID"></param>
		/// <returns></returns>
		private Guid GetProductMasterGuid(string productID)
		{
			Guid productGuid = Guid.Empty; //was -1

			if (!string.IsNullOrEmpty(productID))
			{
				productGuid = FMChannelHelper.MakeCall<IProducts, Guid>(
											x =>
											x.GetMasterRecordGuidFromID(this.Security,productID)  //Tank is a client of Product, and reference Products using the MasterRecordGuid.
									);
			}

			return productGuid;
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.TankDataGrid.ItemDataBound +=
				this.TankDataGridItemDataBound;
		}

		/// <summary>
		///    This method create all the links for the tank list and places them
		///    on the client side.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TankDataGridItemDataBound(object sender, DataGridItemEventArgs e)
		{
			if (e.Item.ItemIndex == -1)
			{
				if (this.Session["TankSelectForm.Mode"] != null)
				{
					e.Item.Cells[0].Text = this.GetTranslatedText(this.Session["TankSelectForm.Mode"] as string);
				}
				else
				{
					e.Item.Cells[0].Text = this.GetTranslatedText("Select");
				}
				if (this.TankDataGrid.Columns.Count > 0)
					this.TankDataGrid.Columns[0].HeaderText = e.Item.Cells[0].Text;
			}

			else
			{
				if (this.Session["TankSelectForm.Mode"] != null)
				{
					var select = new HtmlInputCheckBox();
					select.ID = "Select";
					e.Item.Cells[0].Controls.Add(select);
					select.Attributes.Add("Title", HttpUtility.JavaScriptStringEncode(this.TankDataGrid.Columns[0].HeaderText + " " + ID));
					e.Item.Cells[1].Text = e.Item.Cells[1].Text.Replace(" ", "&nbsp;");
				}
				else
				{
					string id = "";

					// Leave hard space zero length string
					if (e.Item.Cells[1].Text != "&nbsp;")
					{
						id = HttpUtility.HtmlDecode(e.Item.Cells[1].Text);
					}

					string toolTip = ((e.Item.Cells[2].Text != "&nbsp;") ? e.Item.Cells[2].Text + ", " : "")
					                 + ((e.Item.Cells[3].Text != "&nbsp;") ? e.Item.Cells[3].Text + ", " : "");

					var select = new HtmlAnchor();
					select.ID = "Select";
					select.HRef = HttpUtility.HtmlEncode("javascript:Select('" + HttpUtility.JavaScriptStringEncode(id) + "','" + HttpUtility.JavaScriptStringEncode(toolTip) + "')");
					Image im = new Image();
					im.ImageUrl = "../FMWebApp/Images/Select.gif";
					im.BorderWidth = 0;
					im.Style.Add("align", "absmiddle");
					select.Controls.Add(im);

					e.Item.Cells[0].Controls.Add(select);
				}
			}
		}

		private void UpdateView()
		{
			this.FindTextBox.Text = this.Session["TankSelectForm.SearchString"] as string;
			var type = VESSEL_TYPE.MAX_VESSEL;
			TankCollectionClass tankCollection;

			if (this.Session["TankSelectForm.Type"] != null)
			{
				type = (VESSEL_TYPE)this.Session["TankSelectForm.Type"];
			}

			if (type == VESSEL_TYPE.MAX_VESSEL)
			{
				tankCollection = FMChannelHelper.MakeCall<ITanks, TankCollectionClass>(
																	 x =>
                                                                     x.Enumerate(this.Security, this.SessionTankSelectHideHidden)
																);

			}
			else
			{
				if (this.FindTextBox.Text != "")
				{
					tankCollection = FMChannelHelper.MakeCall<ITanks, TankCollectionClass>(
																	 x =>
                                                                     x.EnumerateByFilter(this.Security, this.FindTextBox.Text, this.SessionTankSelectHideHidden)
																);
				}
				else
				{
					tankCollection = FMChannelHelper.MakeCall<ITanks, TankCollectionClass>(
																	 x =>
																	 x.Enumerate(this.Security, this.SessionTankSelectHideHidden)
																);

				}
			}

			if (this.Session["TankSelectForm.Unassigned"] != null)
			{
			    var tank = new TankClass { ID = HttpUtility.HtmlEncode(this.GetTranslatedText("{Unassigned}")) };
			    tankCollection.Insert(0, tank);
			}

			// Determine if there is a linked product ID. If so, then
			// filter by the authorized tanks for that product.
			this.FilterByProductID(tankCollection);

			this.TankDataGrid.DataSource = tankCollection;
			this.TankDataGrid.DataBind();
		}

		#endregion
	}
}