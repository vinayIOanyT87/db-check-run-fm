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
using System.Collections.Specialized;
using System.Configuration;

using Accounting;
using FMBusinessObjects.DataObjects;

namespace ADFWebApp
{
    public partial class SingleSelectAssociateTxDialog : SelectAssociatedTxDialog
    {
        public SingleSelectAssociateTxDialog()
            : base()
        {
            // initialise controls that are no longer there (if any)
        }

        protected new void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            BindControls();
        }

        #region Overrides
        protected override void dgTransactions_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemIndex != -1)
            {
                // hides transaction which has already been associated
                HtmlInputHidden hidTransID = (HtmlInputHidden)e.Item.FindControl("hidTransID");
                AssociatedTxDO txDO = base.gridTransactions[e.Item.ItemIndex] as AssociatedTxDO;

                hidTransID.Value = txDO.TransID;

                /*
                if ((PassFilter(txDO) == false) && (txDO.Associated != 1))
                {
                    e.Item.Visible = false;
                }*/
                if (txDO.Associated == 1)
                {
                    e.Item.Visible = false;
                }

                FMControls.FMSelectLinkButton btnSelect = e.Item.FindControl("selectLinkButton") as FMControls.FMSelectLinkButton;
                if (btnSelect != null)
                {
                    btnSelect.CommandArgument = e.Item.ItemIndex.ToString();
                }
            }
        }
        #endregion // Overrides

        #region Event handling
        protected void BindControls()
        {
            this.dgTransactions.ItemDataBound += new DataGridItemEventHandler(this.dgTransactions_ItemDataBound);
            this.dgTransactions.SelectedIndexChanged += new EventHandler(dgTransactions_SelectedIndexChanged);
        }

        protected void dgTransactions_SelectedIndexChanged(object sender, EventArgs e)
        {
            FMControls.FMDataGrid dg = sender as FMControls.FMDataGrid;
            if (dg != null)
            {
                int itemIndex = dg.SelectedIndex;

                AssociatedTxDO txDO = base.gridTransactions[itemIndex] as AssociatedTxDO;

                // set the results to be returned
                txDO.Associated = 1;
                base.lineItem.AssociatedTransactions.Clear();
                base.lineItem.AssociatedTransactions.Add(txDO);
                //base.lineItem.Product = txDO.Product;

                // close the dialog
                this.Response.Write("<script language=\"JavaScript\">window.returnValue = new Array(\"OK_Clicked\");window.close()</script>");
            }
        }
        #endregion // Event handling
    }
}
