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
    public partial class WizardSidebar : UserControl
    {
        public WizardSidebar()
        {
            this.Dock = DockStyle.Left;
            InitializeComponent();
            // Set a default image.
            Bitmap image = new Bitmap(this.GetType(), "Bitmaps.inst_Def1.bmp");
            this.BackgroundImage = image;

            // Avoid getting the focus.
            this.SetStyle(ControlStyles.Selectable, false);
        }
        
    }
}
