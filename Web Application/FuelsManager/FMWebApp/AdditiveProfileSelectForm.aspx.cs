using System;
using System.Collections;
using System.Data;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using FuelsManager.FMWebApp;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.BusinessInterfaces;
using FMCore;
using FMControls;

namespace FuelsManager.FMWebApp
{
    /// <summary>
    /// Summary description for AdditiveProfileSelectForm.
    /// </summary>
    public partial class AdditiveProfileSelectForm : FMAutoSubmitFormBase
    {
        protected TextBox FindTextBox;
        protected FMButton ShowAllBtn;
        protected FMButton FindBtn;
        protected AdditiveProfileSelectContextClass AdditiveProfileSelectContext;
        protected FMDataGrid AdditiveProfileDataGrid;
        protected string SelectThisItemText;
               
        private void UpdateView()
        {
            FootNoteClass FootNote = Session["FootNote"] as FootNoteClass;
            if (FootNote == null)
                return;


            ArrayList AdditiveProfileArray = new ArrayList();

            if (AdditiveProfileSelectContext.Mode == "Assign")
            {
                // Test for Assignement of <All>
                // Note that AssignedToID is stored untranslated; it is only translated for display.
                if (FootNote.FootNoteAdditiveProfileMapCollection.Count != 1
                || FootNote.FootNoteAdditiveProfileMapCollection[0].AssignedToID != "{All}")
                {

                    AdditiveProfileArray.Add(HttpUtility.HtmlEncode(this.GetTranslatedText("{All}")));

                    AdditiveProfileCollectionClass additiveProfileCollection =
                    FMChannelHelper.MakeCall<IAdditiveProfiles, AdditiveProfileCollectionClass>(x => x.Enumerate(this.Security));

                    foreach (AdditiveProfileClass additiveProfile in additiveProfileCollection)
                    {
                        bool Assigned = false;

                        if (additiveProfile.ID == string.Empty)
                            continue;

                        foreach (ApplicationStringMapClass assignedApplicationStringMap in FootNote.FootNoteAdditiveProfileMapCollection)
                        {
                            if (additiveProfile.ID == assignedApplicationStringMap.AssignedToID)
                            {
                                Assigned = true;
                                break;
                            }
                        }

                        if (!Assigned)
                            AdditiveProfileArray.Add(additiveProfile);
                    }
                }
            }
            
            else
            {
                foreach (ApplicationStringMapClass AssignedApplicationStringMap in FootNote.FootNoteAdditiveProfileMapCollection)
                {
                    if (AssignedApplicationStringMap.AssignedToID == "{All}")
                    {
                        AdditiveProfileArray.Add(HttpUtility.HtmlEncode(this.GetTranslatedText(AssignedApplicationStringMap.AssignedToID)));
                    }
                    else
                    {
                        AdditiveProfileArray.Add(HttpUtility.HtmlEncode(AssignedApplicationStringMap.AssignedToID));
                    }
                }
            }
            
            DataTable AdditiveProfileDataTable = new DataTable();
            DataRow AdditiveProfileDataRow;

            AdditiveProfileDataTable.Columns.Add("ID", typeof(string));
            AdditiveProfileDataTable.Columns.Add("Description", typeof(string));

            string searchStr = this.FindTextBox.Text;

            foreach (object AdditiveProfile in AdditiveProfileArray)
            {
                if (searchStr != string.Empty && (AdditiveProfile is AdditiveProfileClass)
                && -1 == ((AdditiveProfileClass)AdditiveProfile).ID.ToUpper().IndexOf(searchStr.ToUpper()))
                    continue;

                if (searchStr != string.Empty && !(AdditiveProfile is AdditiveProfileClass)
                && -1 == AdditiveProfile.ToString().ToUpper().IndexOf(searchStr.ToUpper()))
                    continue;

                AdditiveProfileDataRow = AdditiveProfileDataTable.NewRow();
                if (AdditiveProfile is AdditiveProfileClass)
                {
                    AdditiveProfileDataRow[0] = ((AdditiveProfileClass)AdditiveProfile).ID;
                    AdditiveProfileDataRow[1] = ((AdditiveProfileClass)AdditiveProfile).Description;
                }
                else
                    AdditiveProfileDataRow[0] = AdditiveProfile.ToString();
                AdditiveProfileDataTable.Rows.Add(AdditiveProfileDataRow);
            }
            
            DataView AdditiveProfileDataView = new DataView(AdditiveProfileDataTable);
            
            AdditiveProfileDataGrid.DataSource = AdditiveProfileDataView;
            AdditiveProfileDataGrid.DataBind();
        }
        

        private AdditiveProfileCollectionClass FilterOnFind(AdditiveProfileCollectionClass additiveProfileCollection)
        {
            if (!string.IsNullOrEmpty(this.FindTextBox.Text))
            {
                var newAdditiveProfileCollection = new AdditiveProfileCollectionClass();
                string searchStr = this.FindTextBox.Text;

                foreach (AdditiveProfileClass additiveProfile in additiveProfileCollection)
                {
                    string additiveProfileID = additiveProfile.ID;
                    int found = additiveProfileID.ToUpper().IndexOf(searchStr.ToUpper(), StringComparison.Ordinal);

                    if (found != -1)
                    {
                        newAdditiveProfileCollection.Add(additiveProfile);
                    }
                }

                return newAdditiveProfileCollection;
            }

            return additiveProfileCollection;
        }
        private void Page_Load(object sender, EventArgs e)
        {
            try
            {
                this.GetSecurity();

                this.SelectThisItemText = this.GetTranslatedText("Select this item");

                if (this.Page.IsPostBack == false)
                {
                    this.AdditiveProfileSelectContext = new AdditiveProfileSelectContextClass();

                    if (this.Request.GetQueryOrFormValue("Mode") != null)
                        this.AdditiveProfileSelectContext.Mode = this.Request.GetQueryOrFormValue("Mode");

                    this.Session["AdditiveProfileSelectContext"] = this.AdditiveProfileSelectContext;
                    this.UpdateView();
                }
                else
                    AdditiveProfileSelectContext = Session["AdditiveProfileSelectContext"] as AdditiveProfileSelectContextClass;

                HtmlForm Form1 = (HtmlForm)FindControl("Form1");
                HtmlInputButton OkButton = new HtmlInputButton();
                OkButton.Attributes.Add("value", this.GetTranslatedText("Ok"));
                OkButton.Attributes.Add("id", "OkButton");
                OkButton.Attributes.Add("class", "formfieldtitle");
                OkButton.Attributes.Add("onclick", "MultipleSelect()");
                OkButton.Attributes.Add("style", "width:66px;Z-INDEX: 107; LEFT: 662px; POSITION: absolute; TOP: 8px");
                Form1.Controls.Add(OkButton);

                HtmlInputButton CancelButton = new HtmlInputButton();
                CancelButton.Attributes.Add("value", this.GetTranslatedText("Cancel"));
                CancelButton.Attributes.Add("id", "CancelButton");
                CancelButton.Attributes.Add("class", "formfieldtitle");
                CancelButton.Attributes.Add("onclick", "NoSelect()");
                CancelButton.Attributes.Add("style", "width:66px;Z-INDEX: 107; LEFT: 758px; POSITION: absolute; TOP: 8px");
                Form1.Controls.Add(CancelButton);
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
            this.ShowAllBtn.Click += new EventHandler(this.FindAllBtn_OnClick);
            this.FindBtn.Click += new EventHandler(this.FindBtn_OnClick);
            this.AdditiveProfileDataGrid.ItemDataBound += new DataGridItemEventHandler(this.AdditiveProfileDataGrid_ItemDataBound);
        }
        #endregion

        protected void FindBtn_OnClick(object sender, EventArgs e)
        {
            if (FindTextBox.Text.Length < 1)
                AdditiveProfileSelectContext.SearchString = null;
            else
                AdditiveProfileSelectContext.SearchString = FindTextBox.Text.ToUpper();

            this.UpdateView();
        }

        protected void FindAllBtn_OnClick(object sender, EventArgs e)
        {
            AdditiveProfileSelectContext.SearchString = null;
            FindTextBox.Text = "";
            this.UpdateView();
        }

        private void AdditiveProfileDataGrid_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemIndex == -1)
            {
                if (e.Item.ItemType == ListItemType.Header)
                    e.Item.Cells[0].Text = GetTranslatedText(AdditiveProfileSelectContext.Mode);
            }

            else
            {
                HtmlInputCheckBox Select = new HtmlInputCheckBox();
                Select.ID = "Select";
                e.Item.Cells[0].Controls.Add(Select);
            }
        }

    
        [Serializable]
        public class AdditiveProfileSelectContextClass
        {
            public string Mode = null;
            public string SearchString = null;
            public bool All = false;
            public bool Unassigned = false;
            public bool Null = false;
        }
    }
}
