/// <summary>
/// File name:	ReportUrlDirDO.cs
/// Purpose:	To contain the report system information such as directory and URL.
///	Comments:	Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 
///				2000.  This file shall not be copied or reproduced in any form 
///				without the express written consent of Endress+Hauser.
///	Author(s):	Richard R. Panachida
///	Version:	1.0.0  Current version
///	
///	Modification History:
///		Date:			By:						Reason:
///		----------		--------------------	----------------------------------
///		2006-09-22		Richard Panachida		Initial version. Used to contain the URL and 
///												directory information for the reports.
/// </summary>
/// 
using System;

namespace ReportingServices
{
	public class ReportUrlDirDO : DataObjectBase
	{
		#region Private Attributes
		private string directory;
		private string url;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default contructor for the Report URL/Path data object.
		/// </summary>
		public ReportUrlDirDO()
		{
			this.Init();
		}
		#endregion

		#region Properties
		/// <summary>
		/// This property sets and returns the report system path information.
		/// </summary>
		public string Directory
		{
			get { return this.directory; }
			set { this.directory = value; }
		}

		/// <summary>
		/// This property sets and returns the report system URL information.
		/// </summary>
		public string URL
		{
			get { return this.url; }
			set { this.url = value; }
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method will initialize the object.
		/// </summary>
		private void Init()
		{
			this.directory = "/Standard+Reports";
			this.url       = "http://localhost/ReportServer/Pages/ReportViewer.aspx?";
		}
		#endregion
	}
}
