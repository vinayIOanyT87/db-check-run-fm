using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Printing;

namespace FMBackupUtilityConfiguration
{
    class TextDocument : PrintDocument
    {
        private string[] text;
        private int pageNumber;
        private int offset;

        public string[] Text
        {
            get { return text; }
            set { text = value; }
        }

        public int PageNumber
        {
            get { return pageNumber; }
            set { pageNumber = value; }
        }

        public int Offset
        {
            get { return offset; }
            set { offset = value; }
        }

        public TextDocument(string[] text)
        {
            this.Text = text;
        }
    }
}
