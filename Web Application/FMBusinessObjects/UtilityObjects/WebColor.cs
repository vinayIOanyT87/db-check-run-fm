using System;
using System.Drawing;
using System.Globalization;

namespace FMBusinessObjects.UtilityObjects
{
    public sealed class WebColorClass
    {
        private Color _color { get; set; }

        private void Initialize(byte alpha, byte red, byte green, byte blue)
        {
            this._color = Color.FromArgb(alpha, red, green, blue);
        }

        public WebColorClass(string colorString)
        {
            this.WebColorString = colorString;
        }
        public WebColorClass(int alpha, int red, int green, int blue)
        {
            this.Initialize((byte)alpha,(byte)red,(byte)green,(byte)blue);
        }

        public WebColorClass(int red, int green, int blue)
        {
            this.Initialize((byte)0, (byte)red, (byte)green, (byte)blue);
        }

        public WebColorClass(int rgba)
        {
            this._color = Color.FromArgb(rgba);
        }

        public string WebColorString 
        {
            get
            {
                return this._color.ToArgb().ToString("X06");
            }
            set
            {
                var intValue = 0;
                if (int.TryParse(
                    value,
                    System.Globalization.NumberStyles.HexNumber,
                    CultureInfo.InvariantCulture,
                    out intValue))
                {
                    this._color = Color.FromArgb(intValue);
                }
                else
                {
                    this._color = Color.Black;
                }
            }
        }

        public int WebColorValue
        {
            get
            {
                return this._color.ToArgb();
            }
            set
            {
                this._color = Color.FromArgb(value);
            }
        }

        public Color Color => this._color;

       
    }
}
