using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace DispatchPrototype
{
	public partial class EmbeddedBrowser : FMBaseForm
	{
		[DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool InternetSetCookie(string lpszUrlName, string lbszCookieName, string lpszCookieData);

		public string TargetURL { get; set; }

		protected override void OnShown(EventArgs e)
		{
			try
			{
				base.OnShown(e);

				string token = AppDomain.CurrentDomain.GetData("Token") as string;
				if (token == null)
				{
					throw new Exception("Embedded Browser invalid token in AppDomain");
				}

				string WebAppAddress = ConfigurationManager.AppSettings["WebAppAddress"];
				if (String.IsNullOrEmpty(WebAppAddress))
				{
					throw new ApplicationException("WebAppAddress not set in configuration.");
				}

				if(!InternetSetCookie(WebAppAddress, "Token", token))
				{
					throw new ApplicationException("Embedded Browser InternetSetCookie error.");
				}
				

				webBrowser1.Url = new Uri(TargetURL);
			}
			catch (Exception except)
			{
				ErrorHandler(except);
			}

		}

		public EmbeddedBrowser(string targetURL)
		{
			try
			{
				GetSecurity();

				InitializeComponent();

				Resize += new EventHandler(EmbeddedBrowser_Resize);
				EmbeddedBrowser_Resize(null, null);

				TargetURL = targetURL;

			}
			catch (Exception except)
			{
				ErrorHandler(except);
			}

		}

		void EmbeddedBrowser_Quit(object sender, EventArgs e)
		{
			Close();
		}

		void EmbeddedBrowser_Resize(object sender, EventArgs e)
		{
			try
			{
				webBrowser1.Width = Width - 10;
				webBrowser1.Height = Height - 10;
			}
			catch (Exception except)
			{
				ErrorHandler(except);
			}

		}

	}

}
