// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResponseFilterClass.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   The purpose of the ResponseFilterClass class is filter HTTPResponse stream
//		and insert CSRF tokens to point in served pages such that with each request 
//		the assigned CSRF token is returned. Global will check for this token
//		and raise an exception if it is missing, at which point session will be terminated.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Diagnostics;


using FMBusinessObjects.DataObjects;

using Opc;

public class ResponseFilterClass : Stream
{
	#region Constants

	private const int BUFFER_SIZE = 2048;
	private bool isJsonAndError500 = false;

	#endregion

	#region Fields

	public HttpResponse Response = null;

	private readonly StringBuilder cache = new StringBuilder( BUFFER_SIZE * 20 );

	private readonly Stream originalStream;

	private HttpSessionState _Session;

	#endregion

	#region Constructors and Destructors

	public ResponseFilterClass( Stream stream )
	{
		this.originalStream = stream;
	}

	#endregion

	#region Public Properties

	public override bool CanRead
	{
		get
		{
			return this.originalStream.CanRead;
		}
	}

	public override bool CanSeek
	{
		get
		{
			return this.originalStream.CanSeek;
		}
	}

	public override bool CanWrite
	{
		get
		{
			return this.originalStream.CanWrite;
		}
	}

	public override long Length
	{
		get
		{
			return this.originalStream.Length;
		}
	}

	public override long Position
	{
		get
		{
			return this.originalStream.Position;
		}

		set
		{
			this.originalStream.Position = value;
		}
	}

	public HttpSessionState Session
	{
		set
		{
			try
			{
				this._Session = value;
			}
			catch
			{
			}
		}
	}

	#endregion

	#region Public Methods and Operators
	public override void Close()
	{

		if (this.isJsonAndError500 == true)
		{
			const string msg = "{\"Message\":\"Invalid web service call\"}";
			string x = this.cache.ToString();
			using (EventLog eventLog = new EventLog("Application", ".", "FuelsManager"))
			{
				eventLog.WriteEntry(x, EventLogEntryType.Error);

			}
			x = msg;

			byte[] newBuf = Encoding.UTF8.GetBytes(x);

			for (int i = 0; i < newBuf.Length; i += BUFFER_SIZE)
			{
				if (i + BUFFER_SIZE < newBuf.Length)
				{
					this.originalStream.Write(newBuf, i, BUFFER_SIZE);
				}
				else
				{
					this.originalStream.Write(newBuf, i, newBuf.Length - i);
				}
			}
		}

		this.originalStream.Close();
		base.Close();
	}

	public override void Flush()
	{
		this.originalStream.Flush();
	}

	public override int Read( byte[] buffer, int offset, int count )
	{
		return this.originalStream.Read( buffer, offset, count );
	}

	public override long Seek( long offset, SeekOrigin origin )
	{
		return this.originalStream.Seek( offset, origin );
	}

	public override void SetLength( long value )
	{
		this.originalStream.SetLength( value );
	}

	public override void Write( byte[] buffer, int offset, int count )
	{
		if (this.Response.StatusCode == 500 && this.Response.ContentType.Contains("application/json" ))
		{
			// Task 71564 - Web Server Misconfiguration: Server Error Message
			this.Response.StatusCode = 400; //Bad request
			this.isJsonAndError500 = true;
			this.cache.Append(Encoding.UTF8.GetString(buffer, offset, count));
			return;
		}
		if (this.isJsonAndError500 == false)
		{
			this.originalStream.Write(buffer, offset, count);
		}

	}

	#endregion


}
