using System;
using System.Collections.Generic;
using System.Text;

namespace FMBackupUtility
{
	[Serializable ( )]
	public class CreateDirectoryException : Exception
	{
		public CreateDirectoryException ( )
		{
		}
		public CreateDirectoryException ( string message )
			: base ( message )
		{
		}
		public CreateDirectoryException ( string message, Exception inner )
			: base ( message, inner )
		{
		}
	}
}
