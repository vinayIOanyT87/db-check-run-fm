using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.DataObjects;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FuelsManager.FMWebApp
{
   public partial class CustomizeEmailMessageForm : FMAutoSubmitFormBase
   {
      protected string alarmAndEventID = string.Empty;

      protected void SaveBtn_OnClick(object sender, EventArgs e)
      {
      }

      protected void CancelBtn_OnClick(object sender, EventArgs e)
      {
      }

      protected void Page_Load(object sender, EventArgs e)
      {
         GetSecurity();
         if (this.IsPostBack == false)
         {
            string guidStr = this.Request.Params["guid"];
            if (string.IsNullOrWhiteSpace(guidStr) == false)
            {
               Guid guid = Guid.Empty;
               if (Guid.TryParse(guidStr, out guid))
               {
                  try
                  {
                     var alarmAndEvent = FMChannelHelper.MakeCall<IAlarmAndEvents, AlarmAndEventClass>(
                                             x => x.Get(this.Security, guid));
                     if (alarmAndEvent != null && alarmAndEvent.IdentityGuid != Guid.Empty)
                     {
                        this.alarmAndEventID = alarmAndEvent.ID;
                        this.SubjectTextBox.Text = this.alarmAndEventID;
                        this.BodyTextBox.Text = "${AlarmAndEvent.Data}";
                        Session["CustomizeEmailMessage.AlarmAndEventGuid"] = guid;
                        if (alarmAndEvent.EmailTemplate != null && alarmAndEvent.EmailTemplate.IdentityGuid != Guid.Empty)
                        {
                           this.SubjectTextBox.Text = alarmAndEvent.EmailTemplate.Subject;
                           this.BodyTextBox.Text= alarmAndEvent.EmailTemplate.Body;
                        }
                     }
                     else
                     {
                        this.SubjectTextBox.Text = string.Empty;
                        this.BodyTextBox.Text = string.Empty;
                     }
                  }
                  catch (Exception ex)
                  {
                     string msg = ex.Message;
                  }
               }
            }
         }

      }

      [WebMethod(EnableSession = true)]
      public static void Save(string subject, string body)
      {
         if (string.IsNullOrWhiteSpace(subject) || string.IsNullOrWhiteSpace(body))
         {
            return ;
         }
         var session = HttpContext.Current.Session;

         var security = session["Security"] as SecurityClass;

         if (security == null)
         {
            throw new System.ServiceModel.FaultException("Invalid session");
         }
         if (session["CustomizeEmailMessage.AlarmAndEventGuid"] != null)
         {
            Guid guid = (Guid) session["CustomizeEmailMessage.AlarmAndEventGuid"];
            try
            {
               var alarmAndEvent = FMChannelHelper.MakeCall<IAlarmAndEvents, AlarmAndEventClass>(
                        x => x.Get(security, guid));
               if (alarmAndEvent != null && alarmAndEvent.IdentityGuid != Guid.Empty)
               {
                  alarmAndEvent.EmailTemplate.Body = body;
                  alarmAndEvent.EmailTemplate.Subject = subject;
                  FMChannelHelper.MakeCall<IAlarmAndEvents>( x => x.Modify(security, alarmAndEvent));
               }
            }
            catch(Exception ex)
            {
               string message = ex.Message;
            }
         }

      }
   }
}