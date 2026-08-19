
namespace FuelsManager.FMReportWebMain
{
	using System;
	using System.Collections;

	using Microsoft.Reporting.WebForms;

	public class ParameterParser
	{
		#region private attributes
		private readonly ReportParameterInfoCollection rptParmCollection;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the parameter parser object.
		/// </summary>
		/// <param name="rptParmCollection"></param>
		public ParameterParser(ReportParameterInfoCollection rptParmCollection)
		{
			if ((rptParmCollection == null) || (rptParmCollection.Count < 1))
			{
				this.rptParmCollection = null;
			}
			else
			{
				this.rptParmCollection = rptParmCollection;
			}

		}
		#endregion

		#region public methods
		/// <summary>
		/// This method will parse through the selected report parameters and set the parameters to a know state.
		/// The Site, SiteGuid, LoginSiteGuid, and UserGuid parameters are set to the what the FM system contains.
		/// Any other parameters that are non-prompt parameters are set to a default value. This is required by 
		/// report viewer object.
		/// </summary>
		/// <param name="requestParms"></param>
		/// <returns></returns>
		public ReportParameter[] ParseParameters(Hashtable requestParms)
		{
			string prompt;
			int index = 0;
			int count = 0;

			// Return null if the report parameter collection is null;
			if (this.rptParmCollection == null)
			{
				return null;
			}

			// Discover the total count of non-prompt parameters
			foreach (ReportParameterInfo rptParmInfo in this.rptParmCollection)
			{
				prompt = rptParmInfo.Prompt;

				if (rptParmInfo.AreDefaultValuesQueryBased)
				{
					continue;
				}

				if (string.IsNullOrEmpty(prompt))
				{
					count++;
				}
			}

			// Create the parameter array to the size of all the non-prompt report parameters.
			var parameters = new ReportParameter[count];

			// Loop through all the report parameters checking for non-prompt parameters. If there is a non-prompt
			// parameter, then determine if it will be set to a system value or a defaul value.
			foreach (ReportParameterInfo rptParmInfo in this.rptParmCollection)
			{
				if (rptParmInfo.AreDefaultValuesQueryBased)
				{
					continue;
				}

				string parmDataType = rptParmInfo.DataType.ToString().ToUpper();
				string parmName = rptParmInfo.Name;
				prompt = rptParmInfo.Prompt;

				// Determine the report parameter is non-prompt (user does not set or see this parameter).
				if (string.IsNullOrEmpty(prompt))
				{
					// If this is a system needed parameter such as site, site guid, login site guid, or user
					// guid, then set the parameter to the system value. Else, set the parameter to a default value.
					if (requestParms.Contains(parmName) )
					{
						parameters[index] = new ReportParameter(parmName, requestParms[parmName].ToString());
						index++;
					}
					else
					{
						if (rptParmInfo.Values.Count > 0)
						{
							parameters[index] = new ReportParameter(parmName, rptParmInfo.Values[0]);
							index++;
						}
						else
						{
							if (parmDataType.Equals("STRING"))
							{
								parameters[index] = new ReportParameter(parmName, " ");
								index++;
							}

							if (parmDataType.Equals("INTEGER"))
							{
								parameters[index] = new ReportParameter(parmName, "-99");
								index++;
							}

							if (parmDataType.Equals("DATETIME"))
							{
								DateTimeOffset currentDate = DateTimeOffset.Now;
								parameters[index] = new ReportParameter(parmName, currentDate.ToString());
								index++;
							}

							if (parmDataType.Equals("BOOLEAN"))
							{
								parameters[index] = new ReportParameter(parmName, "true");
								index++;
							}
						}
					}
				}
			}

			// Return the array of parameters with their values.
			return parameters;
		}
		#endregion
	}
}
