using System;

namespace FMBusinessServices.InternalClasses.FMLogger
{
	/// <summary>
	/// Summary description for BaseTarget.
	/// </summary>
	abstract public class BaseTarget : System.IComparable
	{
		#region Attributes
		private int refCount;
		protected DateTimeOffset lastAccessed;
		protected string appName;
		#endregion Attributes

		#region Properties
		internal string AppName
		{
			get { return appName; }
		}
		internal int RefCount
		{
			get { return refCount; }
		}
		protected internal DateTimeOffset LastAccessed
		{
			get { return lastAccessed; }
		}
		#endregion Properties
		public BaseTarget(string appName)
		{
			refCount = 0;
			this.appName = appName;
		}

		internal void AddRef()
		{
			refCount++;
			lastAccessed = DateTimeOffset.Now;
		}

		//		internal void RemoveRef()
		//		{
		//			refCount--;
		//		}

		virtual internal void Log(LogMessage message)
		{
			Write(Format(message));
			lastAccessed = DateTimeOffset.Now;
		}

		virtual protected string Format(LogMessage message)
		{
			string s = message.Time + "|" + message.LogLevel + "|" + message.Message;
			return s;
		}
		abstract protected void Write(string s);
		abstract internal void RollLog();
		abstract internal void Close();
		#region IComparer Members

		public int CompareTo(object other)
		{
			BaseTarget x = (BaseTarget)other;


			int result = (-this.lastAccessed.CompareTo(x.lastAccessed));
			if (result == 0)
			{
				return this.appName.CompareTo(x.appName);
			}
			return result;
		}

		#endregion
	}
}
