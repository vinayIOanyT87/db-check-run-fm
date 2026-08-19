using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ADOFMSImport.DataObjects;
using Accounting;
using FMBusinessObjects.DataObjects;

namespace ADOFMSImport.Transformers
{
	public class TransformerManager
	{
		#region Attributes
		protected static TransformerManager m_instance = null;
		protected static Object m_singleton = new object ( );

		protected Hashtable m_transformerRegister = new Hashtable ( );
		#endregion // Attributes

		#region Construction
		internal TransformerManager ( Defaults a_defaults )
		{
			RegisterTransformerHandle ( new SalesTransformer ( a_defaults ) );
			RegisterTransformerHandle ( new IssuesTransformer ( a_defaults ) );
		}
		#endregion // Construction

		#region Public Methods
		public void RegisterTransformerHandle ( Transformer a_transformer )
		{
			// hmm this method didn't work out as planned
			m_transformerRegister[a_transformer.GetTransformingType ( )] = a_transformer;
		}

		public Transformer GetTransformer ( Type a_transformerType )
		{
			Transformer result = null;

			if (m_transformerRegister.Contains ( a_transformerType ))
				result = m_transformerRegister[a_transformerType] as Transformer;

			return result;
		}

		public TransactionDOCollection Transform ( CSVObject a_csv )
		{
			TransactionDOCollection result = new TransactionDOCollection ( );

			Transformer transformer = GetTransformer ( a_csv.GetType ( ) );
			if (transformer != null)
			{
				transformer.Transform ( a_csv );
				result = transformer.GetTransformedTransactions ( );
			}

			return result;
		}
		#endregion // Public Methods
	}
}
