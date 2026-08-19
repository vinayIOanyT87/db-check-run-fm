namespace Dispatch
{
	using System;
	using System.Configuration;
	using System.Runtime.InteropServices;

	public partial class EmbeddedBrowser : FMBaseForm
	{
		[DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool InternetSetCookie(string lpszUrlName, string lbszCookieName, string lpszCookieData);
		public string TargetUrl
		{
			get;
			set;
		}
		public static string Password = string.Empty;

		protected override void OnShown(EventArgs e)
		{
			try
			{
				base.OnShown(e);
				var token = AppDomain.CurrentDomain.GetData("Token") as string;

				if (string.IsNullOrEmpty(token))
				{
					throw new Exception("Embedded Browser invalid token in AppDomain");
				}

				string webAppAddress = ConfigurationManager.AppSettings["WebAppAddress"];

				if (String.IsNullOrEmpty(webAppAddress))
				{
					throw new ApplicationException("WebAppAddress not set in configuration.");
				}

				LoginForm.UpdateSession(); // Update or create the record in tblsessions so the browser works
	
				if (!InternetSetCookie(webAppAddress, "Token", token))
				{
					string errorMessage = new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error()).Message;
					throw new ApplicationException(string.Format("Embedded Browser InternetSetCookie error. {0}. WebAppAddress='{1}'  token='{2}'", errorMessage, webAppAddress, token) );
				}
				throw new NotImplementedException("Needs FMBusinessObjects to be merged");
				//if (this.TargetUrl.Contains("?"))
				//{
				//	//this.webBrowser1.Url = new Uri(this.TargetUrl + "&" + this.Security.CSRFTokenWithParamName);
				//}
				//else
				//{
				//	//this.webBrowser1.Url = new Uri(this.TargetUrl + "?" + this.Security.CSRFTokenWithParamName);
				//}

			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		public EmbeddedBrowser(string targetUrl)
		{
			try
			{
				this.GetSecurity();

				this.InitializeComponent();

				this.webBrowser1.NewPopUp += this.WebBrowser1NewPopUp;
				this.Resize += this.EmbeddedBrowserResize;

				this.EmbeddedBrowserResize(null, null);
				this.TargetUrl = targetUrl;
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		void WebBrowser1NewPopUp(object sender, ExtendedWebBrowser.NewWindowEventArgs e)
		{
			e.Processed = true;
			var browser = new EmbeddedBrowser(e.URL);
			browser.ShowDialog(this);
		}

		void EmbeddedBrowserQuit(object sender, EventArgs e)
		{
			this.Close();
		}

		void EmbeddedBrowserResize(object sender, EventArgs e)
		{
			try
			{
				this.webBrowser1.Width = this.Width - 10;
				this.webBrowser1.Height = this.Height - 10;
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}
	}
}
