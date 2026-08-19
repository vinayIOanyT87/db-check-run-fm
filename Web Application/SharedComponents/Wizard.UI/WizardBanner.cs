using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Wizard.UI
{
    public partial class WizardBanner : UserControl
    {
        public WizardBanner()
        {
            InitializeComponent();

            Bitmap image = new Bitmap(this.GetType(), "Bitmaps.Banner_Defense.bmp");
            this.BackgroundImage = image;

            this.SetStyle(ControlStyles.Selectable, false);
        }       
    }
}
