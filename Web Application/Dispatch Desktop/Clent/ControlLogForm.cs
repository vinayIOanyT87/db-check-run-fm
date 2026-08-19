namespace Dispatch
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Drawing.Printing;
	using System.Globalization;
	using System.Windows.Forms;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.UtilityObjects;

	using FMDispatchBusinessObjects.BusinessInterfaces;
	using FMDispatchBusinessObjects.ChannelFactories;

	public enum ListviewSortDirection
    {
        Descending = 0,
        Ascending = 1
    }
    public partial class ControlLogForm : FMBaseForm
    {
        public string UserID = string.Empty;
        private int columnSorted;
        private ListviewSortDirection columnSortDirection = ListviewSortDirection.Descending;

        private Font printFont;
        private Font printFontUnderline;
        private Font printFontBold;
        private Font printFontTitle;
        private int printIndex;
        private int printPage;

        private List<ControllerLogClass> controllerLogCollection;

        public ControlLogForm()
        {
            this.GetSecurity();
            this.InitializeComponent();
        }

        void ControllersLogListViewDoubleClick(object sender, EventArgs e)
        {
            try
            {
                if (this.ControllersLogListView.SelectedItems.Count > 0)
                {
                    string selecteditemtext = this.ControllersLogListView.SelectedItems[0].Text;
                    var addMemo = new AddMemoForm
                                  {
	                                  UserID = this.UserID,
	                                  EditedItemGuid = Guid.Parse(selecteditemtext)
                                  };

	                addMemo.ShowDialog(this);
                }
            }
            catch (Exception except)
            {
                this.ErrorHandler(except);
            }
        }

        void ControlLogFormResize(object sender, EventArgs e)
        {
            try
            {
                this.ControllersLogListView.Width = this.Width - 125;
                this.ControllersLogListView.Height = this.Height - this.ControllersLogListView.Top - 50;
            }
            catch (Exception except)
            {
                this.ErrorHandler(except);
            }
        }

        void StopDatePickerTextChanged(object sender, EventArgs e)
        {
            try
            {
                this.UpdateData();
            }
            catch (Exception except)
            {
                this.ErrorHandler(except);
            }
        }

        void StartDatePickerTextChanged(object sender, EventArgs e)
        {
            try
            {
                this.UpdateData();
            }
            catch (Exception except)
            {
                this.ErrorHandler(except);
            }
        }

        private void InitializeListViewDisplay()
        {
            this.ControllersLogListView.Clear();
            this.ControllersLogListView.View = View.Details;

            this.ControllersLogListView.Columns.Add("Index", 0, HorizontalAlignment.Left);
            this.ControllersLogListView.Columns.Add("Date/Time", 140, HorizontalAlignment.Left);
            this.ControllersLogListView.Columns.Add("Controller", 100, HorizontalAlignment.Left);
            this.ControllersLogListView.Columns.Add("Memo", 600, HorizontalAlignment.Left);
        }

        private void OnAddMemo(object sender, EventArgs e)
        {
            if (!this.Security.HasRight(RIGHT.MODIFY_DISPATCH))
            {
                return;
            }

            var addMemo = new AddMemoForm { UserID = this.UserID, EditedItemGuid = Guid.Empty };
	        addMemo.ShowDialog(this);
            this.UpdateData();
        }

        private void OnCancelClicked(object sender, EventArgs e)
        {
            this.Close();
        }

        private void UpdateData()
        {
            try
            {
                bool sorted = false;
                this.InitializeListViewDisplay();

                if (this.ControllersLogListView.ListViewItemSorter != null)
                {
                    sorted = true;
                    this.ControllersLogListView.ListViewItemSorter = null;
                }

	            if (this.StartDatePicker.Value > this.StopDatePicker.Value)
	            {
		            this.StopDatePicker.Value = this.StartDatePicker.Value;
	            }

                var security = AppDomain.CurrentDomain.GetData("Security") as SecurityClass;

                if (security == null)
		        {
                    throw new Exception("Security not in AppDomain");
		        }

	            DateTime startDate = new DateTime(	this.StartDatePicker.Value.Year,
													this.StartDatePicker.Value.Month, 
													this.StartDatePicker.Value.Day, 
													0, 0, 0);

				DateTime stopDate = new DateTime(	this.StopDatePicker.Value.Year,
													this.StopDatePicker.Value.Month,
													this.StopDatePicker.Value.Day,
													23, 59, 59);

		        this.controllerLogCollection =
					FMChannelHelper.MakeCall<IControllerLogs, List<ControllerLogClass>>(
						x =>
						x.EnumerateByStartStopTime(
							security, StartDatePicker.Value, StopDatePicker.Value, ShowDeletedcheckBox.Checked));

				foreach (ControllerLogClass controllerLogData in this.controllerLogCollection)
                {
                    controllerLogData._EventTime.Format = this.SiteDateTimeFormatInfo;
                    string str = controllerLogData.EventTime;

                    ListViewItem li = this.ControllersLogListView.Items.Add(controllerLogData.IdentityGuid.ToString());
                    li.SubItems.Add(str);
                    li.SubItems.Add(controllerLogData.Controller);
                    li.SubItems.Add(controllerLogData.Memo);
                }

                // reapply the sort
                if (sorted)
		        {
			        this.ControllersLogListView.ListViewItemSorter = new ListViewItemComparer(this.columnSorted, 
																							this.columnSortDirection, 
																							this.SiteDateTimeFormatInfo);
		        }
            }
            catch (Exception exception)
            {
                MessageBox.Show(this, exception.Message, this.Text);
            }
        }

        private void OnEditClicked(object sender, EventArgs e)
        {
            if (this.ControllersLogListView.SelectedItems.Count == 0)
                return;

            string selecteditemtext = this.ControllersLogListView.SelectedItems[0].Text;
            var addMemo = new AddMemoForm { UserID = this.UserID, EditedItemGuid = Guid.Parse(selecteditemtext) };
	        addMemo.ShowDialog(this);

            this.UpdateData();
        }

        private void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            if (!this.Security.HasRight(RIGHT.MODIFY_DISPATCH))
            {
                return;
            }

            try
            {
                int iLoop;
                DialogResult result;

	            if (this.ControllersLogListView.SelectedItems.Count == 0)
	            {
		            return;
	            }

                var selectedItems = new List<Guid>();
                var security = AppDomain.CurrentDomain.GetData("Security") as SecurityClass;

	            if (security == null)
	            {
		            throw new Exception("Security not in AppDomain");
	            }

                // due to the way .net works opening the following dialog will cayse an intem selection change notification
                // to the listview resulting in all items being unselected
                // here we will cheat and set the selected items into an int array
                for (iLoop = 0; iLoop < this.ControllersLogListView.SelectedItems.Count; iLoop++)
                {
	                selectedItems.Add(Guid.Parse(this.ControllersLogListView.SelectedItems[iLoop].Text));
                }

	            if (this.ShowDeletedcheckBox.Checked == false)
	            {
		            result = MessageBox.Show(
											this,
											"Are You Sure You Want To Delete These Log(s)?",
											"Controllers Log",
											MessageBoxButtons.YesNo);
	            }
	            else
	            {
		            result = MessageBox.Show(this, 
											"Are You Sure You Want To Un-Delete These Log(s)?", 
											"Controllers Log", 
											MessageBoxButtons.YesNo);
	            }

	            if (result == DialogResult.Yes)
	            {
					UpdateData();
					throw new NotImplementedException("Complete merge for FMBusinessObject");
					//FMChannelHelper.MakeCall<IClientDispatchService>(
						//x => x.DeleteControllerLogs(security, selectedItems, this.ShowDeletedcheckBox.Checked));


		            
	            }

            }
            catch (Exception exception)
            {
                MessageBox.Show(this, exception.Message, this.Text);
            }
        }

        private void OnShowDeletedItemsCheckBoxStateChanged(object sender, EventArgs e)
        {
            if (this.ShowDeletedcheckBox.Checked == false)
            {
                this.Deletebutton.Text = "Delete";
                this.Editbutton.Enabled = false;
                this.Deletebutton.Enabled = false;

                if (this.Security.HasRight(RIGHT.MODIFY_DISPATCH))
                {
                    this.AddButton.Enabled = true;
                }
                else
                {
                    this.AddButton.Enabled = false;
                }
            }
            else
            {
                this.Deletebutton.Text = "Un-Delete";
                this.Editbutton.Enabled = false;
                this.Deletebutton.Enabled = false;
                this.AddButton.Enabled = false;
            }

            this.UpdateData();
        }

        private void OnControllerLogColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (this.columnSorted != e.Column)
            {
                this.columnSorted = e.Column;
                this.columnSortDirection = ListviewSortDirection.Ascending;
            }
            else
            {
	            if (this.columnSortDirection == ListviewSortDirection.Ascending)
	            {
		            this.columnSortDirection = ListviewSortDirection.Descending;
	            }
	            else
	            {
		            this.columnSortDirection = ListviewSortDirection.Ascending;
	            }
            }
            this.ControllersLogListView.ListViewItemSorter = new ListViewItemComparer(e.Column, 
																					this.columnSortDirection, 
																					this.SiteDateTimeFormatInfo);
        }

        private void SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.ControllersLogListView.SelectedItems.Count == 1)
            {
	            if (this.ShowDeletedcheckBox.Checked)
	            {
		            this.Editbutton.Enabled = false;
	            }
	            else
	            {
		            this.Editbutton.Enabled = true;
	            }

                if (this.Security.HasRight(RIGHT.MODIFY_DISPATCH))
                {
                    this.Deletebutton.Enabled = true;
                }
                else
                {
                    this.Deletebutton.Enabled = false;
                }
            }
            else if (this.ControllersLogListView.SelectedItems.Count > 1)
            {
                this.Editbutton.Enabled = false;

                if (this.Security.HasRight(RIGHT.MODIFY_DISPATCH))
                {
                    this.Deletebutton.Enabled = true;
                }
                else
                {
                    this.Deletebutton.Enabled = false;
                }
            }
            else
            {
                this.Editbutton.Enabled = false;
                this.Deletebutton.Enabled = false;
            }
        }

        private void OnColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
        {
            // do not allow the user to display the index column
            if (e.ColumnIndex == 0)
            {
	            if (this.ControllersLogListView.Columns[e.ColumnIndex].Width > 0)
	            {
		            this.ControllersLogListView.Columns[e.ColumnIndex].Width = 0;
	            }
            }

        }

        private void PrintButtonClick(object sender, EventArgs e)
        {
            try
            {
                var previewDialog = new PrintPreviewDialog
                                    {
	                                    Document = this.GetPrintDocument(),
	                                    Height = 600,
	                                    Width = 800
                                    };

	            previewDialog.ShowDialog(this);
            }
            catch (Exception except)
            {
                this.ErrorHandler(except);
            }
        }

        private PrintDocument GetPrintDocument()
        {
            var printDocument = new PrintDocument();
            printDocument.DefaultPageSettings.Margins.Top = 50;
            printDocument.DefaultPageSettings.Margins.Bottom = 50;
            printDocument.DefaultPageSettings.Margins.Left = 50;
            printDocument.DefaultPageSettings.Margins.Right = 50;

            printDocument.BeginPrint += this.PrintDocumentBeginPrint;
            printDocument.PrintPage += this.PrintDocumentPrintPage;

            return printDocument;
        }

        void PrintDocumentBeginPrint(object sender, PrintEventArgs e)
        {
            try
            {
                this.printPage = 0;
                this.printIndex = 0;
                this.printFont = new Font("Arial", 8);
                this.printFontUnderline = new Font("Arial", 8, FontStyle.Underline | FontStyle.Bold);
                this.printFontBold = new Font("Arial", 10, FontStyle.Bold);
                this.printFontTitle = new Font("Arial", 14, FontStyle.Bold);

            }
            catch (Exception except)
            {
                this.ErrorHandler(except);
            }

        }


        void CalculateAndSetColumnWidth(int columnNumber, 
										ListView controllersLogListView, 
										PrintPageEventArgs ev, 
										Font inPrintFontBold, 
										int defaultColumnWidth)
        {
            int columnWidth = defaultColumnWidth;
	        int numOfItems = controllersLogListView.Items.Count;

            for (int i = 0; i < numOfItems; i++)
            {
                ListViewItem item = controllersLogListView.Items[i];
                string strColumnText = item.SubItems[columnNumber].Text;
                SizeF sizeOfText = ev.Graphics.MeasureString(strColumnText, inPrintFontBold);
                var current = (int)sizeOfText.Width;

                if (current >= columnWidth)
                {
                    columnWidth = current;
                }
            }

            // if the column text data is very short then at least make them as wide as the header.
			ColumnHeader columnHeader = ControllersLogListView.Columns[columnNumber];
			SizeF sizeOfColumnHeader = ev.Graphics.MeasureString(columnHeader.Text, inPrintFontBold);
			var widthOfColumnHeader = (int) sizeOfColumnHeader.Width;

			if (widthOfColumnHeader > columnWidth)
			{
				const int PaddingBetweenColumns = 10;
				columnWidth = widthOfColumnHeader + PaddingBetweenColumns;
			}

	        ControllersLogListView.Columns[columnNumber].Width = columnWidth;
        }

        void PrintDocumentPrintPage(object sender, PrintPageEventArgs ev)
        {
            try
            {
	            int count = 0;
                float leftMargin = ev.MarginBounds.Left;
                float topMargin = ev.MarginBounds.Top;
                int index;
                int columnNum = 1;  // Skip Column 0 which is the id do not print it. 
                const int DefaultColumnWidth = 50;

                this.CalculateAndSetColumnWidth(columnNum,
                                           this.ControllersLogListView,
                                           ev,
                                           this.printFontBold,
                                           DefaultColumnWidth);

                columnNum = 2;
                 this.CalculateAndSetColumnWidth(columnNum,
                                            this.ControllersLogListView,
                                            ev,
                                            this.printFontBold,
											DefaultColumnWidth);

                columnNum = 3;
                this.CalculateAndSetColumnWidth(columnNum,
                                           this.ControllersLogListView,
                                           ev,
                                           this.printFontBold,
                                           DefaultColumnWidth);




                ++this.printPage;

                // Calculate the number of lines per page.
                float linesPerPage = ev.MarginBounds.Height / this.printFont.GetHeight(ev.Graphics) - 1;

                float yPos = topMargin - 10;
                this.CenterLine("Unclassified/For Official Use Only", ev, yPos, this.printFontBold);
                ++count;

                yPos = topMargin + (count * this.printFont.GetHeight(ev.Graphics));
                ev.Graphics.DrawString("Controllers Log", this.printFontTitle, Brushes.Black, leftMargin, yPos, new StringFormat());
                count += 2;

                yPos = topMargin + (count * this.printFont.GetHeight(ev.Graphics));
                this.PrintHeader(ev.Graphics, leftMargin, yPos);
                ++count;

                for (index = this.printIndex; index < this.ControllersLogListView.Items.Count && count < linesPerPage; ++index)
                {
                    ListViewItem item = this.ControllersLogListView.Items[index];

                    yPos = topMargin + (count * this.printFont.GetHeight(ev.Graphics));
                    int lineCount = this.PrintLine(ev.Graphics, leftMargin, yPos, item, ev.MarginBounds);

                    count += lineCount;

                }

                if (index >= this.ControllersLogListView.Items.Count - 1)
                {
                    ev.HasMorePages = false;
                }
                else
                {
                    this.printIndex = index;
                    ev.HasMorePages = true;
                }

                yPos = ev.PageBounds.Bottom - 40;

	            var site =
					FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(Security, Security.SiteGuid, false, false, false));

				var siteTimeConverter = new SiteTimeConverter(site);

				this.LeftLine(siteTimeConverter.Now().DateTime.ToShortDateString(), ev, yPos, this.printFont);
                this.CenterLine("Unclassified/For Official Use Only", ev, yPos, this.printFontBold);
                this.RightLine(this.printPage.ToString(CultureInfo.InvariantCulture), ev, yPos, this.printFont);

            }
            catch (Exception except)
            {
                this.ErrorHandler(except);
            }

        }

        private void LeftLine(string p, PrintPageEventArgs ev, float yPos, Font printFont)
        {
            ev.Graphics.DrawString(p, printFont, Brushes.Black, ev.PageBounds.Left + 50, yPos, new StringFormat());
        }

        private void CenterLine(string p, PrintPageEventArgs ev, float yPos, Font inPrintFont)
        {
            SizeF sizeOfText = ev.Graphics.MeasureString(p, inPrintFont);

            float xPos = (ev.PageBounds.Width / 2) - (sizeOfText.Width / 2);

            ev.Graphics.DrawString(p, inPrintFont, Brushes.Black, xPos, yPos, new StringFormat());
        }

        private void RightLine(string p, PrintPageEventArgs ev, float yPos, Font inPrintFont)
        {
            SizeF sizeOfText = ev.Graphics.MeasureString(p, inPrintFont);

            float xPos = ev.PageBounds.Right - 50 - sizeOfText.Width;

            ev.Graphics.DrawString(p, inPrintFont, Brushes.Black, xPos, yPos, new StringFormat());
        }

        private int PrintLine(Graphics graphics, float leftMargin, float yPos, ListViewItem item, Rectangle marginBounds)
        {
	        ColumnHeader header = this.ControllersLogListView.Columns[1];
            graphics.DrawString(item.SubItems[1].Text, this.printFont, Brushes.Black, leftMargin, yPos, new StringFormat());
            leftMargin += header.Width + 5;

            header = this.ControllersLogListView.Columns[2];
            graphics.DrawString(item.SubItems[2].Text, this.printFont, Brushes.Black, leftMargin, yPos, new StringFormat());
            leftMargin += header.Width + 5;

			StringFormat sf = StringFormat.GenericTypographic;
			sf.Alignment = StringAlignment.Near;
			sf.LineAlignment = StringAlignment.Near;
			sf.FormatFlags = StringFormatFlags.LineLimit;
			sf.Trimming = StringTrimming.Word;

            header = this.ControllersLogListView.Columns[3];
            string memoText = item.SubItems[3].Text;

			// this is all for word wrapping
			float width = header.Width;

			if (marginBounds.Width > (marginBounds.Size.Width - leftMargin))
            {
				width = marginBounds.Size.Width - leftMargin;
            }

			SizeF writeSize = graphics.MeasureString(memoText, this.printFont, new SizeF(width, marginBounds.Size.Height), sf);
			graphics.DrawString(memoText, this.printFont, Brushes.Black, new RectangleF(leftMargin, yPos, width, marginBounds.Size.Height), sf);

			// Note: The reason for rounding by addeing 0.5 and then casting to int is because the graphics height can be different for the view (screen) 
			// vs the printer. The division can be sightly different and the cast to the int cuts off the fraction part of the floating number.
			// you must round up before you cast to get the correct number of lines. If you don't do this the screen may look ok when you view it but 
			// when you go to print the lines may not increase thus printing on top of the previous line making it un readable.

			var numberOfLines = (int)((writeSize.Height / printFont.GetHeight(graphics)) + 0.5);

            return numberOfLines;
        }

        private void PrintHeader(Graphics graphics, float leftMargin, float yPos)
        {
            for (int index = 1; index < this.ControllersLogListView.Columns.Count; ++index)
            {
                ColumnHeader header = this.ControllersLogListView.Columns[index];
                graphics.DrawString(header.Text, this.printFontUnderline, Brushes.Black, leftMargin, yPos, new StringFormat());
                leftMargin += header.Width + 5;
            }
        }

        private void ControlLogFormLoad(object sender, EventArgs e)
        {
			SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(Security, Security.SiteGuid, false, false, false));
			var siteTimeConverter = new SiteTimeConverter(site);

			// format date controls based on site configuration (IGO 2010-Aug-13)
			this.GetSiteDateTimeFormatInfo();

			this.StartDatePicker.CustomFormat	= this.SiteDateTimeFormatInfo.ShortDatePattern;
			this.StartDatePicker.Format			= DateTimePickerFormat.Custom;
			this.StartDatePicker.Value			= siteTimeConverter.Now().DateTime;
			this.StopDatePicker.CustomFormat	= this.SiteDateTimeFormatInfo.ShortDatePattern;
			this.StopDatePicker.Format			= DateTimePickerFormat.Custom;
			this.StopDatePicker.Value			= siteTimeConverter.Now().DateTime;
			this.ShowDeletedcheckBox.Checked	= false;
			this.Editbutton.Enabled				= false;
			this.Deletebutton.Enabled			= false;

			this.StartDatePicker.TextChanged	+= this.StartDatePickerTextChanged;
			this.StopDatePicker.TextChanged		+= this.StopDatePickerTextChanged;
			this.Resize							+= this.ControlLogFormResize;

			this.ControlLogFormResize(null, null);
			this.ControllersLogListView.DoubleClick += this.ControllersLogListViewDoubleClick;

			this.OnControllerLogColumnClick(this.ControllersLogListView, new ColumnClickEventArgs(1));

            bool enable = this.Security.HasRight(RIGHT.MODIFY_DISPATCH);

            this.AddButton.Enabled = this.AddButton.Enabled && enable;
            this.Editbutton.Enabled = this.Editbutton.Enabled && enable;
            this.Deletebutton.Enabled = this.Deletebutton.Enabled && enable;

            this.UpdateData();
        }
    }

    class ListViewItemComparer : System.Collections.IComparer
    {
        private readonly int col;

	    readonly ListviewSortDirection columnSortDirection;
        protected DateTimeFormatInfo SiteDateTimeFormatInfo = null;

        public ListViewItemComparer(DateTimeFormatInfo dateTimeFormatInfo)
        {
            this.SiteDateTimeFormatInfo = dateTimeFormatInfo;
            this.col = 0;
        }

        public ListViewItemComparer(int column, ListviewSortDirection sortDirection, DateTimeFormatInfo dateTimeFormatInfo)
        {
            this.col = column;
            this.columnSortDirection = sortDirection;
            this.SiteDateTimeFormatInfo = dateTimeFormatInfo;
        }

        public int Compare(object x, object y)
        {
            if (this.col != 1)
            {
	            if (this.columnSortDirection == ListviewSortDirection.Ascending)
	            {
		            return String.Compare(((ListViewItem)x).SubItems[this.col].Text, ((ListViewItem)y).SubItems[this.col].Text);
	            }
	            
				return String.Compare(((ListViewItem)y).SubItems[this.col].Text, ((ListViewItem)x).SubItems[this.col].Text);
            }
	        
			DateTime xdatetime = Convert.ToDateTime(((ListViewItem)x).SubItems[this.col].Text, this.SiteDateTimeFormatInfo);
	        DateTime ydatetime = Convert.ToDateTime(((ListViewItem)y).SubItems[this.col].Text, this.SiteDateTimeFormatInfo);
	        long xvalue = xdatetime.Ticks;
	        long yvalue = ydatetime.Ticks;

	        if (this.columnSortDirection == ListviewSortDirection.Ascending)
	        {
		        return String.Compare(xvalue.ToString(CultureInfo.InvariantCulture), yvalue.ToString(CultureInfo.InvariantCulture));
	        }
	        
			return String.Compare(yvalue.ToString(CultureInfo.InvariantCulture), xvalue.ToString(CultureInfo.InvariantCulture));
        }
    }
}
