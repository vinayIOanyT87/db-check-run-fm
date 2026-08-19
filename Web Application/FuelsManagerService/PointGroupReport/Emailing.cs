using FMBusinessObjects;
using FMBusinessObjects.DataObjects;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Web;
using System.Net.Mime;
using static FMBusinessObjects.DataObjects.PointGroupSchedule;

namespace FuelsManagerService.PointGroupReport
{
	static public class Emailing
	{

		static public bool EmailReport(
			string emailTo,
			string fileName,
			Stream memoryStream,
			SiteClass site,
			ExportFileType fileType,
			out string errorMsg)
		{

			uint rasConnectionHandle = 0;
			try
			{
				
				RasApi.RASDIALEXTENSIONS rasDialExtensions = new RasApi.RASDIALEXTENSIONS();
				rasDialExtensions.Size = (uint)Marshal.SizeOf(rasDialExtensions);
				RasApi.RASDIALPARAMS rasDialParams = new RasApi.RASDIALPARAMS();

				rasDialParams.Size = (uint)Marshal.SizeOf(rasDialParams);

                if (site != null)
                {
                    if (site.MailServer.Trim() == "" || site.MailFrom.Trim() == "")
                    {
                        throw new Exception("Email server not properly configured.");
                    }

                    SmtpClient mailClient;

                    Uri uri = new Uri("abcd://" + site.MailServer);

                    if (uri.Port != -1)
                    {
                        mailClient = new SmtpClient { Host = uri.Host, Port = uri.Port };

                        if (uri.Port == 587 || uri.Port == 465)
                            mailClient.EnableSsl = true;
                    }
                    else
                    {
                        mailClient = new SmtpClient { Host = site.MailServer };
                    }

                    if (mailClient.Host != site.MailServer || rasDialParams.EntryName != site.DialupName)
                    {
                        if (rasConnectionHandle != 0)
                        {
                            RasApi.RasHangUp(rasConnectionHandle);
                            rasConnectionHandle = 0;
                        }

                        rasDialParams.EntryName = site.DialupName;
					}
					MailMessage mailMessage = new MailMessage();

					mailMessage.From = new MailAddress(site.MailFrom);
					mailMessage.Body = "Please do not respond to this email, it is for notification only. Responses are not monitored.";

					mailMessage.Subject = "Point Group Report - " + fileName;

					memoryStream.Position = 0;     // read from the start of what was written

					mailMessage.Attachments.Add(new Attachment(
						memoryStream, 
						fileName,
						(fileType == ExportFileType.PDF ? System.Net.Mime.MediaTypeNames.Application.Pdf : System.Net.Mime.MediaTypeNames.Text.Plain)
						));

					// email addresses should be separated by semicolons
					var emailArray = emailTo.Split(';');
					foreach (string emailAddress in emailArray)
					{

						if (emailAddress.Trim() != "") {
							mailMessage.To.Add(emailAddress.Trim());
						}
					}

					if (site.MailConnectMode == MAIL_SERVER_CONNECT_MODE.DIALUP)
					{
						if (rasConnectionHandle == 0)
						{
							uint result;
							bool passwordFlag;

							if (0 != (result = RasApi.RasGetEntryDialParams(null, ref rasDialParams, out passwordFlag)))
							{
								throw new Exception("Error: RasGetEntryDialParams Result = " + result.ToString(CultureInfo.InvariantCulture));
							}
							else
							{
								if (!passwordFlag)
								{
									rasDialParams.UserName = site.MailUserName;
									rasDialParams.Password = site.MailPassword;
								}

								if (0 != (result = RasApi.RasDial(ref rasDialExtensions, null, ref rasDialParams, 0, null, ref rasConnectionHandle)))
								{
									throw new Exception("Error: RasDial Result = " + result.ToString(CultureInfo.InvariantCulture));
								}
								else
								{
									mailClient.Send(mailMessage);
								}
							}
						}
						else
						{
							mailClient.Send(mailMessage);
						}
					}
					else
					{
						mailClient.Send(mailMessage);
					}
				}
				
				if (rasConnectionHandle != 0)
				{
					RasApi.RasHangUp(rasConnectionHandle);
				}
				errorMsg = "";
				return true;
			}
			catch (System.Exception e)
			{
				if (rasConnectionHandle != 0)
				{
					RasApi.RasHangUp(rasConnectionHandle);
				}
				errorMsg = e.Message + " " + e.InnerException?.Message;
				return false;
			}
		}
	}
}