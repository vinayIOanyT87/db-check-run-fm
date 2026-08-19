using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Wizard.Controls
{
    public partial class EtchedLine : System.Windows.Forms.UserControl
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
       
        public EtchedLine()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            // Avoid receiving the focus.
            SetStyle(ControlStyles.Selectable, false);
        }
       
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		/// <param name="e">A <see cref="T:System.Windows.Forms.PaintEventArgs" /> that contains the event data.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

	        using (Brush lightBrush = new SolidBrush(this._lightColor), darkBrush = new SolidBrush(this._darkColor))
	        {
		        using (Pen lightPen = new Pen(lightBrush, 1), darkPen = new Pen(darkBrush, 1))
		        {
			        if (this.Edge == EtchEdge.Top)
			        {
				        e.Graphics.DrawLine(darkPen, 0, 0, this.Width, 0);
				        e.Graphics.DrawLine(lightPen, 0, 1, this.Width, 1);
			        }
			        else if (this.Edge == EtchEdge.Bottom)
			        {
				        e.Graphics.DrawLine(darkPen, 0, this.Height - 2, this.Width, this.Height - 2);
				        e.Graphics.DrawLine(lightPen, 0, this.Height - 1, this.Width, this.Height - 1);
			        }
		        }
	        }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            Refresh();
        }

        Color _darkColor = SystemColors.ControlDark;

        [Category("Appearance")]
        Color DarkColor
        {
            get { return _darkColor; }

            set
            {
                _darkColor = value;
                Refresh();
            }
        }

        Color _lightColor = SystemColors.ControlLightLight;

        [Category("Appearance")]
        Color LightColor
        {
            get { return _lightColor; }

            set
            {
                _lightColor = value;
                Refresh();
            }
        }

        EtchEdge _edge = EtchEdge.Top;

        [Category("Appearance")]
        public EtchEdge Edge
        {
            get
            {
                return _edge;
            }

            set
            {
                _edge = value;
                Refresh();
            }
        }
    }

    public enum EtchEdge
    {
        Top, Bottom
    }
}
