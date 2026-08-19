// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FMTabContainer.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//	 Customizes the Ajax Control Toolkit TabContainer control. Links to our
//   style sheet and implements a method of disabling and graying out tabs.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FMControls
{
	using System;
	using System.Configuration;
	using System.Globalization;
	using System.Linq;
	using System.Web.UI;
	using System.Web.UI.WebControls;

	using AjaxControlToolkit;

	/// <summary>
	///     Customizes the Ajax Control Toolkit TabContainer control. Links to our
	///     style sheet and implements a method of disabling and graying out tabs.
	/// </summary>
	public class FMTabContainer : TabContainer
    {
        // The default width of each tab header, in pixels
        #region Constants

        protected const int DEFAULT_TAB_WIDTH_PX = 79;

        protected const string TABCONTAINER_CSS_CLASS = "tabbedPages";

        // The amount of space around each tab header, in pixels. This must
        // match the style sheet
        protected const int TAB_MARGIN_PX = 5;

        protected const int TAB_FUDGEFACTOR_PX = 4;

        // An amount of pixels to add to the width of the grayed-out row
        // of tab headers in case one of the tab headers is wider than expected
        // due to long text
        protected const int TAB_ROW_FUDGE_FACTOR_PX = 50;

        #endregion

        // The CSS class to use
        #region Fields

        protected Panel pnlGrayedOutTabsPanel = null;

        protected Table pnlGrayedOutTabsTableOutter = null;

        private string previousClientActiveTabIndex = string.Empty;

        /// <summary>
        /// Keeps track of the tab that was last active
        /// </summary>
        public string PreviousClientActiveTabIndex
        {
            get
            {
                return this.previousClientActiveTabIndex;
            }

            set
            {
                this.previousClientActiveTabIndex = value;
            }
        }

        protected string tabWidth;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FMTabContainer"/> class. 
        ///     Default Constructor
        /// </summary>
        public FMTabContainer()
        {
            this.TabWidth = string.Format("{0}px", DEFAULT_TAB_WIDTH_PX);
            this.HeaderEnabled = true;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether the header row of tabs is enabled such that the user can switch tabs
        /// </summary>
        /// <value>
        ///   <c>true</c> if header should be enabled; otherwise, <c>false</c>.
        /// </value>
        public bool HeaderEnabled { get; set; }

        /// <summary>
        /// Gets or sets the width of each tab header, where the title of the tab
        /// is shown
        /// </summary>
        /// <value>
        /// The width of the tab.
        /// </value>
        /// <exception cref="System.ArgumentException">Tab Width must be expressed in pixels</exception>
        public string TabWidth
        {
            get
            {
                return this.tabWidth;
            }

            set
            {
                if (!value.EndsWith("px"))
                {
                    throw new ArgumentException("Tab Width must be expressed in pixels");
                }

                this.tabWidth = value;
            }
        }

        #endregion

        #region Methods
 
        /// <summary>
        ///     Create elements that mimic the look of the row of tab headers, with
        ///     all but the active tab appearing grayed out
        /// </summary>
        protected void AddGrayedOutTabHeader()
        {
            this.pnlGrayedOutTabsPanel = new Panel
            {
                ID = "pnlGrayedOutTabs",
                CssClass = "tabbedPages"
            };

            this.pnlGrayedOutTabsPanel.Style["position"] = "relative";
            this.pnlGrayedOutTabsPanel.Style["top"] = this.Style["top"]; 
            this.pnlGrayedOutTabsPanel.Style["left"] = this.Style["left"]; 
            this.pnlGrayedOutTabsPanel.Style["padding"] = "0px";
            this.pnlGrayedOutTabsPanel.Style["padding-left"] = "0px";
            this.pnlGrayedOutTabsPanel.Style["padding-right"] = "0px";
            this.pnlGrayedOutTabsPanel.Style["padding-top"] = "0px";
            this.pnlGrayedOutTabsPanel.Style["padding-bottom"] = "0px";
            this.pnlGrayedOutTabsPanel.Style["margin-left"] = "0px";
            this.pnlGrayedOutTabsPanel.Style["margin-right"] = "0px";
            this.pnlGrayedOutTabsPanel.Style["margin-top"] = "0px";
            this.pnlGrayedOutTabsPanel.Style["margin-bottom"] = "0px";

            // Unfortunately, we need to figure out how wide the panel should be, otherwise
            // the fake tab headers could wrap
            this.pnlGrayedOutTabsPanel.Style["width"] = string.Format("{0}px", this.CalculateEstimatedWidth());

            // Make sure this row of controls is on top of the real row of tab headers
            string thisZIndex = this.Style["z-index"];
            if (string.IsNullOrEmpty(thisZIndex))
            {
                thisZIndex = "0";
            }

            this.pnlGrayedOutTabsPanel.Style["z-index"] = (int.Parse(thisZIndex) + 1).ToString(CultureInfo.InvariantCulture);


            this.pnlGrayedOutTabsTableOutter = new Table { CssClass = "tabbedPages ajax__tab_header" };
		  string ariaLabel = this.Attributes["aria-label"];
		  if (string.IsNullOrEmpty(ariaLabel))
		  {
		  	ariaLabel = "Tabs";
		  }
		  this.pnlGrayedOutTabsTableOutter.Attributes.Add("aria-label", ariaLabel);
		  this.pnlGrayedOutTabsTableOutter.Attributes.Add("role", "tablist");
		  this.pnlGrayedOutTabsPanel.Controls.Add(this.pnlGrayedOutTabsTableOutter);
            this.pnlGrayedOutTabsTableOutter.Style["padding"] = "0px";
            this.pnlGrayedOutTabsTableOutter.Style["padding-left"] = "0px";
            this.pnlGrayedOutTabsTableOutter.Style["padding-right"] = "0px";
            this.pnlGrayedOutTabsTableOutter.Style["padding-top"] = "0px";
            this.pnlGrayedOutTabsTableOutter.Style["padding-bottom"] = "0px";
            this.pnlGrayedOutTabsTableOutter.Style["margin-left"] = "0px";
            this.pnlGrayedOutTabsTableOutter.Style["margin-right"] = "0px";
            this.pnlGrayedOutTabsTableOutter.Style["margin-top"] = "0px";
            this.pnlGrayedOutTabsTableOutter.Style["margin-bottom"] = "0px";
            this.pnlGrayedOutTabsTableOutter.Style["display"] = "table";
            this.pnlGrayedOutTabsTableOutter.Style["border-spacing"] = "0px";
            this.pnlGrayedOutTabsTableOutter.Style["border-collapse"] = "collapse";

            int tabWidth = int.Parse(this.TabWidth.Substring(0, this.TabWidth.Length - 2)) + TAB_FUDGEFACTOR_PX;  //This padding is probably because of fighting with CssClasses
            int tabHeight = 26; //Originally came out of the FuelsManager.css
 
            var pnlGrayedOutTabRowOutter = new TableRow ();
            this.pnlGrayedOutTabsTableOutter.Rows.Add(pnlGrayedOutTabRowOutter);
            pnlGrayedOutTabRowOutter.Style["vertical-align"] = "bottom";
            pnlGrayedOutTabRowOutter.Style["padding"] = "0px";
            pnlGrayedOutTabRowOutter.Style["padding-left"] = "0px";
            pnlGrayedOutTabRowOutter.Style["padding-right"] = "0px";
            pnlGrayedOutTabRowOutter.Style["padding-top"] = "0px";
            pnlGrayedOutTabRowOutter.Style["padding-bottom"] = "0px";
            pnlGrayedOutTabRowOutter.Style["margin-left"] = "0px";
            pnlGrayedOutTabRowOutter.Style["margin-right"] = "0px";
            pnlGrayedOutTabRowOutter.Style["margin-top"] = "0px";
            pnlGrayedOutTabRowOutter.Style["margin-bottom"] = "0px";
            pnlGrayedOutTabRowOutter.Style["display"] = "table-row";

            foreach (TabPanel tabPnl in this.Tabs)
            {
                if (tabPnl.Visible && tabPnl.Enabled)
                {
                    var pnlGrayedOutTabCellOutter = new TableCell ();
                    pnlGrayedOutTabRowOutter.Cells.Add(pnlGrayedOutTabCellOutter);
                    pnlGrayedOutTabCellOutter.Style["vertical-align"] = "bottom";
                    pnlGrayedOutTabCellOutter.Style["padding"] = "0px";
                    pnlGrayedOutTabCellOutter.Style["padding-left"] = "0px";
                    pnlGrayedOutTabCellOutter.Style["padding-right"] = "0px";
                    pnlGrayedOutTabCellOutter.Style["padding-top"] = "0px";
                    pnlGrayedOutTabCellOutter.Style["padding-bottom"] = "0px";
                    pnlGrayedOutTabCellOutter.Style["margin-left"] = "0px";
                    pnlGrayedOutTabCellOutter.Style["margin-right"] = "0px";
                    pnlGrayedOutTabCellOutter.Style["margin-top"] = "0px";
                    pnlGrayedOutTabCellOutter.Style["margin-bottom"] = "0px";
                    pnlGrayedOutTabCellOutter.Style["display"] = "table-cell";
                    pnlGrayedOutTabCellOutter.Style["text-align"] = "center";

                    var pnlGrayedOutTabTable = new Table();
                    pnlGrayedOutTabCellOutter.Controls.Add(pnlGrayedOutTabTable);
				pnlGrayedOutTabTable.Attributes.Add("aria-label", tabPnl.HeaderText);
				pnlGrayedOutTabTable.Attributes.Add("role", "presentation");
				pnlGrayedOutTabTable.Style["height"] = string.Format("{0}px", tabHeight);
                    pnlGrayedOutTabTable.Style["width"] = string.Format("{0}px", tabWidth);
                    pnlGrayedOutTabTable.Style["padding"] = "0px";
                    pnlGrayedOutTabTable.Style["padding-left"] = "0px";
                    pnlGrayedOutTabTable.Style["padding-right"] = "0px";
                    pnlGrayedOutTabTable.Style["padding-top"] = "0px";
                    pnlGrayedOutTabTable.Style["padding-bottom"] = "0px";
                    pnlGrayedOutTabTable.Style["margin-left"] = "0px";
                    pnlGrayedOutTabTable.Style["margin-right"] = "0px";
                    pnlGrayedOutTabTable.Style["margin-top"] = "0px";
                    pnlGrayedOutTabTable.Style["margin-bottom"] = "0px";
                    pnlGrayedOutTabTable.Style["display"] = "table";
                    pnlGrayedOutTabTable.Style["border-spacing"] = "0";
                    pnlGrayedOutTabTable.Style["border-collapse"] = "collapse";

                    var pnlGrayedOutTabRowInner = new TableRow ();
                    pnlGrayedOutTabTable.Rows.Add(pnlGrayedOutTabRowInner);
                    pnlGrayedOutTabRowInner.Style["height"] = string.Format("{0}px", tabHeight);
                    pnlGrayedOutTabRowInner.Style["width"] = string.Format("{0}px", tabWidth);
                    pnlGrayedOutTabRowInner.Style["padding"] = "0px";
                    pnlGrayedOutTabRowInner.Style["padding-left"] = "0px";
                    pnlGrayedOutTabRowInner.Style["padding-right"] = "0px";
                    pnlGrayedOutTabRowInner.Style["padding-top"] = "0px";
                    pnlGrayedOutTabRowInner.Style["padding-bottom"] = "0px";
                    pnlGrayedOutTabRowInner.Style["margin-left"] = "0px";
                    pnlGrayedOutTabRowInner.Style["margin-right"] = "0px";
                    pnlGrayedOutTabRowInner.Style["margin-top"] = "0px";
                    pnlGrayedOutTabRowInner.Style["margin-bottom"] = "0px";
                    pnlGrayedOutTabRowInner.Style["display"] = "table-row";

                    var pnlGrayedOutTabCellInner = new TableCell();
                    pnlGrayedOutTabRowInner.Cells.Add(pnlGrayedOutTabCellInner);
                    pnlGrayedOutTabCellInner.Style["height"] = string.Format("{0}px", tabHeight);
                    pnlGrayedOutTabCellInner.Style["width"] = string.Format("{0}px", tabWidth);
                    pnlGrayedOutTabCellInner.Style["padding"] = "0px";
                    pnlGrayedOutTabCellInner.Style["padding-left"] = "0px";
                    pnlGrayedOutTabCellInner.Style["padding-right"] = "0px";
                    pnlGrayedOutTabCellInner.Style["padding-top"] = "0px";
                    pnlGrayedOutTabCellInner.Style["padding-bottom"] = "0px";
                    pnlGrayedOutTabCellInner.Style["margin-left"] = "0px";
                    pnlGrayedOutTabCellInner.Style["margin-right"] = "0px";
                    pnlGrayedOutTabCellInner.Style["margin-top"] = "0px";
                    pnlGrayedOutTabCellInner.Style["margin-bottom"] = "0px";
                    pnlGrayedOutTabCellInner.Style["display"] = "table-cell";

                    pnlGrayedOutTabCellInner.Style["border-right-style"] = "solid";
                    pnlGrayedOutTabCellInner.Style["border-right-width"] = String.Format("{0}px", TAB_MARGIN_PX);
                    pnlGrayedOutTabCellInner.Style["border-right-color"] = "#FFFFFF";
                    pnlGrayedOutTabCellInner.Style["white-space"] = "normal";
                    if (tabPnl == this.ActiveTab)
                    {
                        pnlGrayedOutTabTable.CssClass = "tabbedPages";
                        pnlGrayedOutTabRowInner.CssClass = "tabbedPages ajax__tab_active";
                        pnlGrayedOutTabCellInner.CssClass = "tabbedPages ajax__tab_active ajax__tab_tab";
 
                     }
                    else
                    {
                        pnlGrayedOutTabTable.CssClass = "tabbedPages";
                        pnlGrayedOutTabRowInner.CssClass = "tabbedPages grayedFMTab";
                        pnlGrayedOutTabCellInner.CssClass = "tabbedPages grayedFMTab ajax__tab_tab";
                    }
                    pnlGrayedOutTabCellInner.Text = tabPnl.HeaderText;  
                }
            }
        }

        /// <summary>
        ///     Calculate the width in pixels that the header area will take up. This is only
        ///     an estimate, because the text of a header could cause it to widen, so we add
        ///     a "fudge factor."
        /// </summary>
        /// <returns>Estimated header area width, in pixels</returns>
        protected int CalculateEstimatedWidth()
        {
            // Remove "px"
            int tabWidthPx = int.Parse(this.TabWidth.Substring(0, this.TabWidth.Length - 2));

            int numVisibleTabs = 0;
            foreach (TabPanel tabPnl in this.Tabs)
            {
                if (tabPnl.Visible)
                {
                    numVisibleTabs++;
                }
            }

            return (tabWidthPx + TAB_MARGIN_PX + TAB_FUDGEFACTOR_PX) * numVisibleTabs + TAB_ROW_FUDGE_FACTOR_PX;
        }

        /// <summary>
        /// Override to provide a workaround to a bug in the TabContainer control. In
        ///     the case of some controls that cause a page to post back, the
        ///     ActiveTab is not set properly. A hidden field on the page is used to track
        ///     the correct active tab.
        /// </summary>
        /// <param name="clientState">
        /// State of TabContainer being loaded
        /// </param>
        protected override void LoadClientState(string clientState)
        {
            base.LoadClientState(clientState);

            // Inspect hidden field
            if (this.Page.Request.Form.AllKeys.Contains(this.ID + "_ActiveTabIndex"))
            {
                string activeTabIndexFieldValue = this.Page.Request.Form[this.ID + "_ActiveTabIndex"];
                if (!string.IsNullOrEmpty(activeTabIndexFieldValue))
                {
                    // The tricky thing is that the index stored in our hidden field only counts visible tabs,
                    // while the ActiveTabIndex property counts all tabs
                    int visibleTabIndex = -1;
                    int actualTabIndex = -1;
                    foreach (TabPanel tabPnl in this.Tabs)
                    {
                        actualTabIndex++;

                        if (tabPnl.Visible)
                        {
                            visibleTabIndex++;
                            if (visibleTabIndex == int.Parse(activeTabIndexFieldValue))
                            {
                                this.ActiveTabIndex = actualTabIndex;
                                this.previousClientActiveTabIndex = activeTabIndexFieldValue;
                                break;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Load the saved control state and parse out our properties
        /// </summary>
        /// <param name="savedState">
        /// saved control state
        /// </param>
        protected override void LoadControlState(object savedState)
        {
            if (savedState != null)
            {
                var t = savedState as Triplet;
                if (t != null)
                {
                    base.LoadControlState(t.First);
                    this.TabWidth = (string)t.Second;
                    this.HeaderEnabled = (bool)t.Third;
                }
                else
                {
                    var p = savedState as Pair;
                    if (p != null)
                    {
                        this.TabWidth = (string)t.First;
                        this.HeaderEnabled = (bool)t.Second;
                    }
                    else
                    {
                        base.LoadControlState(savedState);
                    }
                }
            }
        }

        /// <summary>
        /// Override that allows pages to display in the designer without error
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            this.CssClass = TABCONTAINER_CSS_CLASS;

            // We don't want any tab to be active, otherwise, Visual Studio shows every
            // control on the page as causing an error
            if (this.DesignMode)
            {
                this.ActiveTabIndex = -1;
            }
        }

        /// <summary>
        /// Widen control if need be; if header is set to disabled, create fake tab panel
        /// with inactive tabs grayed out; hook up the client side event for tracking
        /// active index
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected override void OnPreRender(EventArgs e)
        {
            // Make wider if need be
            double estWidthDbl = this.CalculateEstimatedWidth();

            if (this.Style["width"] != null && this.Style["width"].EndsWith("px"))
            {
                double curWidthDbl;
                if (double.TryParse(this.Style["width"].Substring(0, this.Style["width"].Length - 2), out curWidthDbl)
                    && curWidthDbl < estWidthDbl)
                {
                    this.Style["width"] = string.Format("{0}px", (int)estWidthDbl);
                }
            }
            else if (this.Width.Type == UnitType.Pixel && this.Width.Value < estWidthDbl)
            {
                this.Width = new Unit(estWidthDbl, UnitType.Pixel);
            }

            base.OnPreRender(e);

            if (!this.HeaderEnabled)
            {
                this.AddGrayedOutTabHeader();
            }
            else
            {
                // Hook up JavaScript event for tracking active tab
                this.OnClientActiveTabChanged = this.ID + "_OnClientActiveTabChanged";
            }
        }

        /// <summary>
        /// Override to add in some dynamic controls and styles that are needed
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter" /> object that receives the server control content.</param>
        protected override void Render(HtmlTextWriter writer)
        {
            if (!this.HeaderEnabled && this.pnlGrayedOutTabsPanel != null)
            {
                this.pnlGrayedOutTabsPanel.RenderControl(writer);
            }

            // The background color for active tab header comes from web.config
            writer.WriteFullBeginTag("style");
            writer.Write(".tabbedPages .ajax__tab_active .ajax__tab_tab {background-color:#");
            writer.Write(Convert.ToInt32(ConfigurationManager.AppSettings["ColorHeaderBlue"]).ToString("X").PadLeft(6, '0'));
            writer.Write("}\r\n");

            // Tab width
            writer.Write(".tabbedPages .ajax__tab_inner {width:" + this.TabWidth + "}\r\n");
            writer.WriteEndTag("style");

            // Create hidden field to track active tab
            writer.WriteBeginTag("input");
            writer.WriteAttribute("type", "hidden");
            writer.WriteAttribute("id", this.ID + "_ActiveTabIndex");
            writer.WriteAttribute("name", this.ID + "_ActiveTabIndex");
            writer.WriteAttribute("value", this.previousClientActiveTabIndex);
            writer.Write(" />");

            // Write JavaScript event handler to track active tab
            writer.WriteBeginTag("script");
            writer.WriteAttribute("type", "text/javascript");
            writer.Write(">\r\n");
            writer.Write("function " + this.ID + "_OnClientActiveTabChanged(sender, e) {\r\n");
            writer.Write(
                "document.getElementById(\"" + this.ID
                + "_ActiveTabIndex\").setAttribute(\"value\", sender.get_activeTabIndex());\r\n}\r\n");
            writer.WriteEndTag("script");
            base.Render(writer);
        }

        /// <summary>
        ///     Add our properties into the control state
        /// </summary>
        /// <returns>object for serialization</returns>
        protected override object SaveControlState()
        {
            object baseState = base.SaveControlState();

            if (baseState != null)
            {
                return new Triplet(baseState, this.TabWidth, this.HeaderEnabled);
            }

            return new Pair(this.TabWidth, this.HeaderEnabled);
        }

        #endregion
    }
}
