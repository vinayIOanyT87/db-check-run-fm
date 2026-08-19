///***************************************************************************
/// Module Name:  FMEventLog
/// Author:       Developer
/// Copyright (c) Varec, Inc.  All rights reserved.
///***************************************************************************
namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using System.ServiceModel;
	using System.Web;

	using FMBusinessObjects.Constants;
	using FMBusinessObjects.BusinessInterfaces;

	/// <summary>
	///  Provides the ability to write messages into the EventLog via the 
	///  FuelsManager framework and convention.
	/// </summary>
	public class FMEventLog : IFMEventLog
	{
		// The actual limit is 32766 per MSDN - ArguementException will be thrown.
		// However Windows 2008 Server and R2 appear to have a 31885 byte limit somewhere in the lower
		// callstack.  A Win32 Parameter Exception is thrown instead and will bubble up to your app.
		// (Windows 2003 Server has a 32000 byte limit)
		// I (georgep) have set FMEventLog to start splitting large messages at 31500 bytes.
		private const int MAX_EVENTLOG_MESSAGE_LENGTH = 31500;

		public FMEventLog()
		{
		}

		private static void MyTrace(string message, FMEventLogEntryType entryType )
		{
			switch ( entryType)
			{
				case FMEventLogEntryType.Error:
					Trace.TraceError(message);
					break;
				case FMEventLogEntryType.Warning:
					Trace.TraceWarning(message);
					break;
				default:
					Trace.TraceInformation(message);
					break;

			}
		}

		public void WriteEntry( string message, FMEventLogEntryType entryType )
		{
			HardwareKeyClass hardwareKey = new HardwareKeyClass();

			// Daniel - Not sure whether Trace will work. It didn't work in my emulator.
			// so in the mean time, try to write to both Trace and EventLog
			try
			{
				// else
				{
					using (EventLog eventLog = new EventLog("Application", ".", "FuelsManager"))
					{
						if (message.Length <= MAX_EVENTLOG_MESSAGE_LENGTH)
						{
							eventLog.WriteEntry(message, (EventLogEntryType)entryType);
						}
						else
						{
							// If the incoming message exceeds the configured max message size, break the
							// message down into smaller messages that can be written.
							ArrayList messageList = SplitLargeMessage(message, MAX_EVENTLOG_MESSAGE_LENGTH, true);

							foreach (string currentMessage in messageList)
							{
								eventLog.WriteEntry(currentMessage, (EventLogEntryType)entryType);
							}
						}
					}
				}
			}
			catch (Exception error)
			{
				MyTrace(error.Message, FMEventLogEntryType.Error);
			}
		}

		/// <summary>
		/// Splits the passed in <c>message</c> into an array of smaller messages.
		/// <para>If the size of the original message exceeds the specified <c>maximumMessageSize</c>, the method will split the message into smaller messages.  
		/// However if the original message doesn't exceed the <c>maximumMessageSize</c>, the original message will be returned as a single array element.</para>
		/// </summary>
		/// <param name="message">The original message to split.</param>
		/// <param name="maximumMessageSize">The maximum length for a single message.  Ie: The EventLog can only accept 32677 bytes per entry.</param>
		/// <param name="addMessageSeparator">Prefix each smaller message with seperator text.</param>
		/// <returns>Array of smaller messages that can be iterated and processed individually.</returns>
		/// <remarks>The caller can request that a message separator be added to each smaller message.  
		/// A separator such as --- Message 1 of 5 --- is added to the top of each message.
		/// </remarks>
		public static ArrayList SplitLargeMessage(string message, int maximumMessageSize, bool addMessageSeparator)
		{
			string MESSAGE_SEPARATOR_FORMAT = @"--- Message {0} of {1} ---\r\n{2}";

			// If requested, make room to insert a divider string.  Simply taking the length of the format string 
			// gives us room to handle 000 to 999, no need to get fancy.
			int maxAdjustedMessageSize = (addMessageSeparator) ? (maximumMessageSize - MESSAGE_SEPARATOR_FORMAT.Length) : maximumMessageSize;

			int offset = 0;
			int currentMessageCount = 1;
			int totalMessageCount = ((message.Length >= maxAdjustedMessageSize) ? (message.Length / maxAdjustedMessageSize) + 1 : 1);

			ArrayList messageList = new ArrayList();

			// If the message fits within a single line then don't attempt to break the message apart.
			while (currentMessageCount < totalMessageCount)
			{
				messageList.Add(string.Format(
					MESSAGE_SEPARATOR_FORMAT,
					currentMessageCount,
					totalMessageCount,
					message.Substring(offset, maxAdjustedMessageSize)));

				currentMessageCount++;
				offset += maxAdjustedMessageSize;
			}

			// Add the last portion of the message (could be the entire message).
			messageList.Add(string.Format(
					MESSAGE_SEPARATOR_FORMAT,
					currentMessageCount,
					totalMessageCount,
					message.Substring(offset)));

			return (messageList);
		}
	}
}