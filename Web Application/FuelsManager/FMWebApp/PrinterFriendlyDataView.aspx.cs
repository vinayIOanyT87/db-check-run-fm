namespace FuelsManager.FMWebApp
{
    using System;

    using FMCore;

    public partial class PrinterFriendlyDataView : FMFormBase
   {
      public const string PRINTER_FRIENDLY_DATA_VIEW = "PrinterFriendlyDataView";
      
      protected void Page_Init ( object sender, EventArgs e )
      {
         try
         {
            this.GetSecurity();

            string TitleText = (string) this.Request.GetQueryOrFormValue("Title");
            if (String.IsNullOrEmpty( TitleText ) == false)
            {
               this.TitleLabel.Text = TitleText;
            }

            object DataSource = this.Session[PRINTER_FRIENDLY_DATA_VIEW];
            this.Session.Remove( PRINTER_FRIENDLY_DATA_VIEW );

            this.ResultsGrid.AutoGenerateColumns = true;
            this.ResultsGrid.DataSource = DataSource;
            this.ResultsGrid.DataBind();

         }
         catch (Exception except)
         {
            this.ErrorHandler( except );
         }

      }

   }

}
