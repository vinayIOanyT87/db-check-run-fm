using System;
using System.Collections;
using System.Web;
using System.Web.UI.HtmlControls;

using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.ServiceRequests;
using FMBusinessObjects.UtilityObjects;

using FMCore;

namespace FuelsManager.Accounting
{

    public partial class AccountingTankSelectForm : AccountingWebFormView
	{
		#region Protected Attributes
		#endregion

		private void UpdateView()
		{
			this.FindTextBox.Text = this.Session["TankSelectForm.SearchString"] as string;
			string managerId = this.Session["TankSelectForm.IDManagerLink"] as string;
			string productId = this.Session["TankSelectForm.IDProductLink"] as string;

			VESSEL_TYPE type = VESSEL_TYPE.MAX_VESSEL;
			TankCollectionClass tankCollection;

			if (this.Session["TankSelectForm.Type"] != null)
			{
				type = (VESSEL_TYPE)this.Session["TankSelectForm.Type"];
			}

			tankCollection = new TankCollectionClass();

			if (this.Session["TankSelectForm.Unassigned"] != null)
			{
				TankClass tank = new TankClass();
				var str = GetDataDictionaryValueByKey(this.security.SiteGuid, "{Unassigned}");
				tank.ID = HttpUtility.HtmlEncode(str);
				tankCollection.Insert(0, tank);
			}

			managerId = managerId.Remove(managerId.IndexOf('|'));
			ArrayList usedTankList = this.GetUsedTankList(productId, managerId);

			foreach (Object tankIdObject in usedTankList)
			{
				string tankId = tankIdObject as string;

				if (string.IsNullOrEmpty(tankId) == true)
				{
					continue;
				}

				TankClass tank;
				if (tankId == "{All}")
				{
					tank = new TankClass();
					tank.ID = "{All}";
					tank.ProductID = productId;
					tank.ManagerID = managerId;
				}
				else
				{
					tank = FMChannelHelper.MakeCall<ITanks, TankClass>(x => x.Get(base.security, x.GetIdentityGuid(base.security, tankId)));
				}

				if (tank != null)
				{
					tankCollection.Add(tank);
				}
			}

			this.TankDataGrid.DataSource = tankCollection;
			this.TankDataGrid.DataBind();
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				base.Initialize();

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

					this.UpdateView();
				}

				if (this.Session["TankSelectForm.Mode"] != null)
				{
					HtmlForm Form1 = (HtmlForm)this.FindControl("Form1");
					HtmlInputButton OkButton = new HtmlInputButton();
					var str = GetDataDictionaryValueByKey(this.security.SiteGuid, "OK");

					OkButton.Attributes.Add("value", str);
					OkButton.Attributes.Add("id", "OkButton");
					OkButton.Attributes.Add("class", "formfieldtitle");
					OkButton.Attributes.Add("onclick", "MultipleSelect()");
					OkButton.Attributes.Add("style", "width:66px;Z-INDEX: 107; LEFT: 662px; POSITION: absolute; TOP: 8px");
					Form1.Controls.Add(OkButton);

					str = GetDataDictionaryValueByKey(this.security.SiteGuid, "Cancel");

					HtmlInputButton CancelButton = new HtmlInputButton();
					CancelButton.Attributes.Add("value", str);
					CancelButton.Attributes.Add("id", "CancelButton");
					CancelButton.Attributes.Add("class", "formfieldtitle");
					CancelButton.Attributes.Add("onclick", "NoSelect()");
					CancelButton.Attributes.Add("style", "width:66px;Z-INDEX: 107; LEFT: 758px; POSITION: absolute; TOP: 8px");
					Form1.Controls.Add(CancelButton);
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
			this.TankDataGrid.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.TankDataGrid_ItemDataBound);

		}
		#endregion

		protected void FindBtn_OnClick(object sender, System.EventArgs e)
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

		protected void FindAllBtn_OnClick(object sender, System.EventArgs e)
		{
			this.Session.Remove("TankSelectForm.SearchString");
			this.FindTextBox.Text = "";
			this.UpdateView();
		}

		/// <summary>
		/// This method create all the links for the tank list and places them
		/// on the client side.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TankDataGrid_ItemDataBound(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
		{
			if (e.Item.ItemIndex == -1)
			{
				if (this.Session["TankSelectForm.Mode"] != null)
					e.Item.Cells[0].Text = GetDataDictionaryValueByKey(this.security.SiteGuid, this.Session["TankSelectForm.Mode"] as string);
				else
					e.Item.Cells[0].Text = GetDataDictionaryValueByKey(this.security.SiteGuid, "Select");
			}

			else
			{
				if (this.Session["TankSelectForm.Mode"] != null)
				{
					HtmlInputCheckBox Select = new HtmlInputCheckBox();
					Select.ID = "Select";
					e.Item.Cells[0].Controls.Add(Select);

					e.Item.Cells[1].Text = e.Item.Cells[1].Text.Replace(" ", "&nbsp;");
				}
				else
				{
					string ID = "";

					// Leave hard space zero length string
					if (e.Item.Cells[1].Text != "&nbsp;")
						ID = HttpUtility.HtmlDecode(e.Item.Cells[1].Text);

					string ToolTip = ((e.Item.Cells[2].Text != "&nbsp;") ? e.Item.Cells[2].Text + ", " : "") +
									 ((e.Item.Cells[3].Text != "&nbsp;") ? e.Item.Cells[3].Text + ", " : "");


					HtmlAnchor Select = new HtmlAnchor();
					Select.ID = "Select";
                    Select.HRef = HttpUtility.HtmlEncode("javascript:Select('" + HttpUtility.JavaScriptStringEncode(ID) + "','" + HttpUtility.JavaScriptStringEncode(ToolTip) + "')");
					Select.InnerHtml = "<img src=\"../FMWebApp/Images/Select.gif\" border=\"0\" align=\"absmiddle\" alt='" +
						HttpUtility.HtmlEncode(GetDataDictionaryValueByKey(this.security.SiteGuid, "Select this item")) + "'>";

					e.Item.Cells[0].Controls.Add(Select);
				}
			}
		}

		/// <summary>
		/// This method will retrieve the product Guid in order to find
		/// the associated tanks.
		/// </summary>
		/// <param name="productID"></param>
		/// <returns></returns>
		private Guid GetProductGuid(string productID)
		{
			Guid productGuid = Guid.Empty;

			if ((productID != null) || (productID.Length > 0))
			{
				productGuid = FMChannelHelper.MakeCall<IProducts, Guid>(x => x.GetIdentityGuid(base.security, productID));
			}

			return productGuid;
		}

		/// <summary>
		/// This method will determine if there is a linked product ID and filter the 
		/// tank list to the associated tanks of the product.
		/// </summary>
		/// <param name="tankCollection"></param>
		private void FilterByBeingUsed(TankCollectionClass tankCollection, ArrayList usedTankList)
		{
			foreach (TankClass tank in tankCollection)
			{
				if (usedTankList.Contains(tank.ID) == false)
				{
					tankCollection.Remove(tank);
				}
			}
		}

		private ArrayList GetUsedTankList(string productId, string managerId)
		{
			TankListSR tankListSR = new TankListSR();
			tankListSR.ProductId = productId;
			tankListSR.Security = base.security;
			tankListSR.ManagerId = managerId;

			TankListDO tankListDO = FMChannelHelper.MakeCall<ITankListProcessor, TankListDO>(x => x.Process(tankListSR));

			if (tankListDO == null)
			{
				return null;
			}

			return tankListDO.TankList;
		}
	}
}
