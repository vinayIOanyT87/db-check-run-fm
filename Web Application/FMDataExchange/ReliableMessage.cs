using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace FMDataExchange
{
	/// <summary>
	/// This is the Exception class used by the MessageInspector 
	/// </summary>
	public class ReliableMessageError : MessageInspectorError
	{
		public ReliableMessageError(string message)
			: base(message)
		{
		}
	}

	/// <summary>
	/// See comment from MessageInspectionBehaviorExtension class
	/// </summary>
	internal class ReliableMessage
	{
		private static Dictionary<string, ReliableMessage> AllMessages { get; set; }
		internal static int MessageValidPeriodMinutes {get; set;}

		internal string ID { get; private set; }
		internal DateTime CreatedTime { get; private set; }
		internal DateTime ExpiresTime { get; private set; }

		#region static 
		/// <summary>
		/// static construttor to initialize the message list and pull the configuration settings
		/// </summary>
		static ReliableMessage()
		{
			const string MessageValidPeriodKey = "MessageValidPeriod";
			try
			{
				try
				{
					string messageValidPeriodMinutesString = ConfigurationManager.AppSettings[MessageValidPeriodKey];
					MessageValidPeriodMinutes = int.Parse(messageValidPeriodMinutesString);
				}
				catch
				{
					MessageValidPeriodMinutes = 5;
				}

				AllMessages = new Dictionary<string, ReliableMessage>();
			}
			catch (Exception error)
			{
				throw new ReliableMessageError("Unexpected error in ReliableMessage static ctor : " + error.Message);
			}
		}
		#endregion static

		internal ReliableMessage(string messageID, DateTime newCreatedTime, DateTime newExpiresTime)
		{
			ID = messageID;
			CreatedTime = newCreatedTime;
			ExpiresTime = newExpiresTime;
		}

		/// <summary>
		/// Is the message expired
		/// </summary>
		internal bool IsExpired
		{
			get
			{
				DateTime currentTime = DateTime.Now;
				return (currentTime > ExpiresTime)
						||
						(currentTime > CreatedTime.AddMinutes(MessageValidPeriodMinutes));
			}
		}

		/// <summary>
		/// Remove expired messages
		/// </summary>
		internal void DoMaintenance()
		{
			List<string> expiredMessageIDs = (from msg in AllMessages 
													where msg.Value.IsExpired 
													select msg.Key).ToList();
			foreach (string msgID in expiredMessageIDs)
			{
				AllMessages.Remove(msgID);
			}
		}

		/// <summary>
		/// make sure message is not expired and not a duplicate
		/// </summary>
		internal void Validate()
		{
			if (IsExpired)
			{
				throw new ReliableMessageError("The message is expired. Make sure the clocks on the server and the client are in sync.");
			}
			if (AllMessages == null)
			{
				throw new ReliableMessageError("Unexpected error in ReliableMessage.Validate(Static Ctor failed).");
			}
			if (AllMessages.ContainsKey(ID))
			{
				throw new ReliableMessageError("Duplicated message detected.");
			}

			DoMaintenance();

			// Add the current message to the list
			AllMessages.Add(this.ID, this);
		}
	}
}