 #pragma warning disable 1587
/// <summary>
/// File name:	TaxRateMarkupSummaryForm.cs
/// Purpose:	This page allows the user to see a summary of the Markup tax, edit a line item, or
///            remove a line item.
///            
/// Comments:	Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 
///				2000.  This file shall not be copied or reproduced in any form 
///				without the express written consent of Endress+Hauser.
///				
/// Author(s):	Richard R. Panachida
/// Version:	1.0.0  Current version
///	
/// Modification History:
///	Date:			   By:						Reason:
///	----------		--------------------	----------------------------------
///	yyyy-mm-dd		Developer's name		Reason for the change
///		
/// </summary>
#pragma warning restore 1587
namespace FuelsManager.FinanceWebApp
{
    using System;
    using System.Web.UI.WebControls;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.Constants;
    using FMBusinessObjects.DataObjects;

    using FuelsManager.Accounting;

    public partial class TaxRateMarkupSummaryForm : AccountingWebFormView
   {
      #region Private data members
      private MarkupDOCollection markupCollection;
      #endregion

      #region Page load
      /// <summary>
      /// This is the main entry point for the Markup Summary page.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      protected void Page_Load(object sender, EventArgs e)
      {

         this.markupCollection = FMChannelHelper.MakeCall<IMarkups, MarkupDOCollection>(
																	 x =>
																	 x.GetAll(this.security)
																);

         if (this.Page.IsPostBack == false)
         {
            if (this.security.HasRight(RIGHT.MODIFY_FINANCIAL_DATA) == false)
            {
               this.EnableControls(false);
            }

            this.UpdateView();
         }
      }
      #endregion

      #region Private methods
      /// <summary>
      /// This method updates the Markup Summary grid with new data.
      /// </summary>
      private void UpdateView()
      {
         this.GridSizeDropDown.SetPageSize(this.MarkupDataGrid, this.markupCollection.Count);
         this.MarkupDataGrid.DataSource = this.markupCollection;
         this.MarkupDataGrid.DataBind();
      }

      /// <summary>
      /// This method enables controls based on the input.
      /// </summary>
      /// <param name="enable"></param>
      private void EnableControls(bool enable)
      {
         this.AddTopButton.Enabled = enable;
         this.AddBottomButton.Enabled = enable;
      }

      /// <summary>
      /// This method will create a new Markup object, place it into session and redirect
      /// the adding to the Markup detail page.
      /// </summary>
      private void AddNewMarkup()
      {
          MarkupDO newMarkup = new MarkupDO { IdentityGuid = Guid.Empty, PurchasingEntity = "", MarkupRate = 0.0 };

          if (this.Page.Session[PageSessionKeyConstants.TAX_MARKUP_SUMMARY_OBJECT] != null)
         {
            this.Page.Session.Remove(PageSessionKeyConstants.TAX_GST_SUMMARY_OBJECT);
         }

         this.Page.Session.Add(PageSessionKeyConstants.TAX_MARKUP_SUMMARY_OBJECT, newMarkup);
         this.Redirect("TaxMarkupDetailForm.aspx?Mode=Add");
      }
      #endregion

      #region Handle event methods
      /// <summary>
      /// This method handles the grid's item data binding. It will disable the edit and delete buttons in
      /// the grid if the user does not have premissions.
      /// </summary>
      /// <param name="source"></param>
      /// <param name="eventArgs"></param>
      protected void MarkupDataGridItemDataBound(object source, DataGridItemEventArgs eventArgs)
      {
         try
         {
            LinkButton editButton   = (LinkButton)eventArgs.Item.FindControl("btnEdit");
            LinkButton deleteButton = (LinkButton)eventArgs.Item.FindControl("btnDelete");

            // Disable the edit and delete buttons if the user does not have modify rights
            if ((editButton != null) && (deleteButton != null))
            {
               if (this.security.HasRight(RIGHT.MODIFY_FINANCIAL_DATA) == false)
               {
                  editButton.Enabled   = false;
                  deleteButton.Enabled = false;
               }
            }
         }
         catch (Exception except)
         {
            this.ErrorHandler(except);
         }
      }

      /// <summary>
      /// This method will handle the add button (top button) event.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      protected void AddButtonTopClick(object sender, EventArgs e)
      {
         this.AddNewMarkup();
      }

      /// <summary>
      /// This method will handle the add button (bottom button) event.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      protected void AddButtonBottomClick(object sender, EventArgs e)
      {
         this.AddNewMarkup();
      }

      /// <summary>
      /// This method will handle the deletion of an item in the grid.
      /// </summary>
      /// <param name="source"></param>
      /// <param name="e"></param>
      private void MarkupDataGridDeleteCommand(object source, DataGridCommandEventArgs e)
      {
         // Find the Markup to delete
         MarkupDO selectedMarkup = null;
			Guid selectedGuid = Guid.Parse(e.CommandArgument.ToString());

         foreach (MarkupDO markup in this.markupCollection)
         {
            if (markup.IdentityGuid == selectedGuid)
            {
               selectedMarkup = markup;
               break;
            }
         }

         try
         {
				FMChannelHelper.MakeCall<IMarkups>(
																	 x =>
																	 x.Remove(selectedMarkup, this.security)
																);
         }
         catch (Exception except)
         {
            this.ErrorHandler(except);
         }

         // Now remove the selected Markup from the collection
         this.markupCollection.RemoveByIdentityGuid(selectedMarkup);

         this.UpdateView();
      }

      /// <summary>
      /// This method handles the edit event. It will redirect the item being edit to the
      /// Markup detail page.
      /// </summary>
      /// <param name="source"></param>
      /// <param name="e"></param>
      private void MarkupDataGridEditCommand(object source, DataGridCommandEventArgs e)
      {
         Guid markupItemGuid		= Guid.Parse(e.CommandArgument.ToString());
         int selectedIndex       = e.Item.ItemIndex;
         MarkupDO selectedMarkup = null;

         if ((selectedIndex >= 0) && (selectedIndex < this.markupCollection.Count))
         {
            // This is an existing GST so find it
            foreach (MarkupDO markup in this.markupCollection)
            {
               if (markup.IdentityGuid == markupItemGuid)
               {
                  selectedMarkup = markup;
                  break;
               }
            }
         }

         if (selectedMarkup == null)
         {
            string errMsg = "Markup Selected object not found.";
            this.ErrorHandler(new Exception(errMsg));
         }
         else
         {
            if (this.Page.Session[PageSessionKeyConstants.TAX_MARKUP_SUMMARY_OBJECT] != null)
            {
               this.Page.Session.Remove(PageSessionKeyConstants.TAX_MARKUP_SUMMARY_OBJECT);
            }

            this.Page.Session.Add(PageSessionKeyConstants.TAX_MARKUP_SUMMARY_OBJECT, selectedMarkup);
            this.Redirect("TaxMarkupDetailForm.aspx?Mode=edit");
         }
      }

      /// <summary>
      /// This method will handle the page index change. It will update the view to the
      /// new page.
      /// </summary>
      /// <param name="source"></param>
      /// <param name="e"></param>
      private void MarkupDataGridPageIndexChanged(object source, DataGridPageChangedEventArgs e)
      {
         // if we are editing do not allow a page change
         if (this.MarkupDataGrid.EditItemIndex > -1)
         {
            return;
         }

         this.MarkupDataGrid.CurrentPageIndex = e.NewPageIndex;
         this.UpdateView();
      }

      /// <summary>
      /// This method handles the grid size dropdown change. It will update the 
      /// grid size accordingly.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      protected void GridSizeDropdownOnChange(object sender, EventArgs e)
      {
         // if we are editing do not allow a page change
         if (this.MarkupDataGrid.EditItemIndex > -1)
         {
            return;
         }

         this.UpdateView();
      }
      #endregion

      #region Web Form Designer generated code
      /// <summary>
      /// This method will handle the On Init event for the page. It will initialize the base
      /// page OnInit and setup event handlers.
      /// </summary>
      /// <param name="e"></param>
      override protected void OnInit(EventArgs e)
      {
         this.InitializeComponent();
         base.OnInit(e);
      }

      /// <summary>
      /// This method will initialize event handles.
      /// </summary>
      private void InitializeComponent()
      {
         this.MarkupDataGrid.EditCommand      += new DataGridCommandEventHandler(this.MarkupDataGridEditCommand);
         this.MarkupDataGrid.DeleteCommand    += new DataGridCommandEventHandler(this.MarkupDataGridDeleteCommand);
         this.MarkupDataGrid.ItemDataBound    += new DataGridItemEventHandler(this.MarkupDataGridItemDataBound);
         this.MarkupDataGrid.PageIndexChanged += new DataGridPageChangedEventHandler(this.MarkupDataGridPageIndexChanged);
      }
      #endregion
   }
}
