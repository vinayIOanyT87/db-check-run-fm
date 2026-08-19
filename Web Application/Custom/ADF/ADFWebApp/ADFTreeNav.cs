using System;
using FMCommon;

using ConsolidatedBLL;
using ConsolidatedDataObjects;
using FM7Accounting;
using FMControls;
using System.Web.UI;
using Microsoft.Web.UI.WebControls;
using InvoiceWebApp;

namespace ADFWebApp
{
    public class ADFTreeNav : Page, ITreeNodeDiscovery
    {
        Microsoft.Web.UI.WebControls.TreeNode ITreeNodeDiscovery.GetLeftViewTreeNode(SecurityClass security, bool SiteGroup, uint Options, uint SpecialKeyCodes)
        {
            TreeNode mainNode = this.AddNode(null, "Invoice Entry", "../InvoiceWebApp/InvoiceSplash.aspx", true);
            
            //base.GetLeftViewTreeNodeEx(security, SiteGroup, Options, SpecialKeyCodes);
            

            if (SiteGroup && (this.HasHardwareKey(Options) == true)
               && ((security.HasRight(RIGHT.VIEW_FINANCIAL_DATA) == true) || (security.HasRight(RIGHT.MODIFY_FINANCIAL_DATA) == true))
               && (HasProfessionalSetting(SpecialKeyCodes) == false))
            {
                const string adfWebAppURL = "../ADFWebApp/";

                if (null != mainNode)
                {
                    if (security.HasRight(FMCommon.RIGHT.MODIFY_PAYMENT_DATA))
                    {
                        // JS20091231 CCP-042
                        AddNode(mainNode, "Bulk Payment", "../Accounting/TransactionDetail.aspx?" +
                            TransactionDetail.ModeKey + "=ADD&TransAlias=Bulk Payment", false);
                    }

                    if (mainNode != null && mainNode.Nodes.Count == 0)
                    {
                        mainNode.Remove();
                        //Remove node if nothing is under it. 
                        mainNode = null;
                    }
                }
            }

            return mainNode;
        }

        protected Microsoft.Web.UI.WebControls.TreeNode AddNode(Microsoft.Web.UI.WebControls.TreeNode parent, string text, string url, bool ApplyDataDictionary)
        {
            FMTreeNode node = new FMTreeNode();

            node.NavigateUrl = url;
            node.Text = text;
            node.ImageUrl = "images\\ctxmsc_cls.gif";
            node.SelectedImageUrl = "images\\ctxmsc_opn.gif";
            node.ApplyDataDictionary = ApplyDataDictionary;

            if (parent != null)
            {
                parent.Nodes.Add(node);
                parent.ImageUrl = null;
                parent.SelectedImageUrl = null;
            }

            return node;

        }

        public bool HasHardwareKey(uint Options)
        {
            bool hasKey = true;

            if ((Options & 0x100000) == 0)
            {
                hasKey = false;
            }

            return hasKey;
        }

        public bool HasProfessionalSetting(uint SpecialKeyCodes)
        {
            bool hasSetting = true;
            if ((SpecialKeyCodes & 0x00000020) == 0)
            {
                hasSetting = false;
            }
            return hasSetting;
        }
    }
}
