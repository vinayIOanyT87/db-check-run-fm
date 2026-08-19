using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ADOFMSImport.Transformers.Interfaces;
using ADOFMSImport.DataObjects;

using Accounting;
using FMBusinessObjects.DataObjects;

namespace ADOFMSImport.Transformers
{
	public class Transformer : ITransformer
	{
		#region Attributes
		protected TransactionDOCollection m_transactionCollection = null;
		#endregion // Attributes

		public Transformer ( )
		{
			//TransformerManager.GetInstance().RegisterTransformerHandle(this);
		}

		public TransactionDOCollection GetTransformedTransactions ( )
		{
			return m_transactionCollection;
		}

		#region ITransformer members
		public virtual bool Transform ( CSVObject a_csv )
		{
			throw new NotImplementedException ( MethodBase.GetCurrentMethod ( ).ToString ( ) );
		}

		public virtual Type GetTransformingType ( )
		{
			throw new NotImplementedException ( MethodBase.GetCurrentMethod ( ).ToString ( ) );
		}
		#endregion // ITransformer members
	}
}
