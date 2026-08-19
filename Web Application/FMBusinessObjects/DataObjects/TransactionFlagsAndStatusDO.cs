namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Data;
	using System.Data.SqlClient;
	using System.Runtime.Serialization;
	using System.Text;

	[DataContract]
	[Serializable]
	public class TransactionFlagsAndStatusDO : DataObject
	{
		#region Constructors
		/// <summary>
		/// This is the constructor for the transaciton flags and status
		/// data object.
		/// </summary>
		/// <param name="transID"></param>
		public TransactionFlagsAndStatusDO( string transID )
		{
			this.TransID = transID;
		}

		public TransactionFlagsAndStatusDO()
		{
		}
		#endregion

		#region Properties
		[DataMember]
		public string TransID { get; set; }

		[DataMember]
		public bool? Flag01 { get; set; }

		[DataMember]
		public bool? Flag02 { get; set; }

		[DataMember]
		public bool? Flag03 { get; set; }

		[DataMember]
		public bool? Flag04 { get; set; }

		[DataMember]
		public bool? Flag05 { get; set; }

		[DataMember]
		public bool? Flag06 { get; set; }

		[DataMember]
		public DateTimeOffset? Date01 { get; set; }

		[DataMember]
		public DateTimeOffset? Date02 { get; set; }

		[DataMember]
		public DateTimeOffset? Date03 { get; set; }

		[DataMember]
		public DateTimeOffset? Date04 { get; set; }

		[DataMember]
		public bool? ErrorFlag { get; set; }

		[DataMember]
		public TransactionStatus? TransStatus { get; set; }

		[DataMember]
		public DateTimeOffset? TimeIn { get; set; }

		[DataMember]
		public DateTimeOffset? FST { get; set; }

		[DataMember]
		public DateTimeOffset? TimeEnd { get; set; }

		[DataMember]
		public long? TransVersion { get; set; }
		#endregion

		#region Public methods
		/// <summary>
		/// This method resets the object to its initial state by setting
		/// all data members to null.
		/// </summary>
		public void Reset()
		{
			this.Init();
		}

		/// <summary>
		/// This method will populate the SQL Command with the appropriate
		/// SQL and values.
		/// </summary>
		/// <param name="sqlCommand"></param>
		public void UpdateSQLCommand( SqlCommand sqlCommand )
		{
			if (sqlCommand != null  &&
				this.HasValue() &&
				string.IsNullOrEmpty( this.TransID ) == false)
			{
				var parm = new SqlParameter( "@TransID", SqlDbType.NVarChar, 64 ) { Value = this.TransID };
				sqlCommand.Parameters.Add( parm );

				// Start by updating the accounting sequences for transactions so other clients are notified of changes.
				var stringBuilder = new StringBuilder();

				// Now set the transaction table update
				stringBuilder.AppendLine("UPDATE tblTransactions SET  ");

				if ( this.Flag01 != null )
				{
					stringBuilder.Append("Flag01 = @Flag01,");

					parm = new SqlParameter( "@Flag01", SqlDbType.Bit ) { Value = this.Flag01.Value ? 1 : 0 };
					sqlCommand.Parameters.Add( parm );
				}

				if ( this.Flag02 != null )
				{
					stringBuilder.Append("Flag02 = @Flag02,");

					parm = new SqlParameter( "@Flag02", SqlDbType.Bit ) { Value = this.Flag02.Value ? 1 : 0 };
					sqlCommand.Parameters.Add( parm );
				}

				if ( this.Flag03 != null )
				{
					stringBuilder.Append("Flag03 = @Flag03,");

					parm = new SqlParameter( "@Flag03", SqlDbType.Bit ) { Value = this.Flag03.Value ? 1 : 0 };
					sqlCommand.Parameters.Add( parm );
				}

				if ( this.Flag04 != null )
				{
					stringBuilder.Append("Flag04 = @Flag04,");

					parm = new SqlParameter( "@Flag04", SqlDbType.Bit ) { Value = this.Flag04.Value ? 1 : 0 };
					sqlCommand.Parameters.Add( parm );
				}

				if ( this.Flag05 != null )
				{
					stringBuilder.Append("Flag05 = @Flag05,");

					parm = new SqlParameter( "@Flag05", SqlDbType.Bit ) { Value = this.Flag05.Value ? 1 : 0 };
					sqlCommand.Parameters.Add( parm );
				}

				if ( this.Flag06 != null )
				{
					stringBuilder.Append("Flag06 = @Flag06,");

					parm = new SqlParameter( "@Flag06", SqlDbType.Bit ) { Value = this.Flag06.Value ? 1 : 0 };
					sqlCommand.Parameters.Add( parm );
				}

				if (this.Date01 != null)
				{
					stringBuilder.Append("Date01 = @Date01, ");

					parm = new SqlParameter("@Date01", SqlDbType.DateTimeOffset) { Value = this.Date01.Value };
					sqlCommand.Parameters.Add(parm);
				}

				if (this.Date02 != null)
				{
					stringBuilder.Append("Date02 = @Date02, ");

					parm = new SqlParameter("@Date02", SqlDbType.DateTimeOffset) { Value = this.Date02.Value };
					sqlCommand.Parameters.Add(parm);
				}

				if (this.Date03 != null)
				{
					stringBuilder.Append("Date03 = @Date03, ");

					parm = new SqlParameter("@Date03", SqlDbType.DateTimeOffset) { Value = this.Date03.Value };
					sqlCommand.Parameters.Add(parm);
				}

				if (this.Date04 != null)
				{
					stringBuilder.Append("Date04 = @Date04, ");

					parm = new SqlParameter("@Date04", SqlDbType.DateTimeOffset) { Value = this.Date04.Value };
					sqlCommand.Parameters.Add(parm);
				}

				if ( this.ErrorFlag != null )
				{
					stringBuilder.Append("ErrorFlag = @ErrorFlag,");

					parm = new SqlParameter( "@ErrorFlag", SqlDbType.Bit ) { Value = this.ErrorFlag.Value ? 1 : 0 };
					sqlCommand.Parameters.Add( parm );
				}

				if ( this.TransStatus != null )
				{
					stringBuilder.Append("LookupTransactionStatusIndex = @TransStatus,");

					parm = new SqlParameter( "@TransStatus", SqlDbType.Int ) { Value = this.TransStatus.Value };
					sqlCommand.Parameters.Add( parm );
				}

				if ( this.TimeIn != null )
				{
					stringBuilder.Append("TimeIn = @TimeInValue,");

					parm = new SqlParameter( "@TimeInValue", SqlDbType.DateTimeOffset ) { Value = this.TimeIn };
					sqlCommand.Parameters.Add( parm );
				}

				if ( this.FST != null )
				{
					stringBuilder.Append("FST = @FSTValue,");

					parm = new SqlParameter( "@FSTValue", SqlDbType.DateTimeOffset ) { Value = this.FST };
					sqlCommand.Parameters.Add( parm );
				}

				if ( this.TimeEnd != null )
				{
					stringBuilder.Append("TimeEnd = @TimeEndValue,");

					parm = new SqlParameter( "@TimeEndValue", SqlDbType.DateTimeOffset ) { Value = this.TimeEnd };
					sqlCommand.Parameters.Add( parm );
				}

				if (this.TransVersion != null)
				{
					stringBuilder.Append("TransVersion = @TransVersion,");

					parm = new SqlParameter("@TransVersion", SqlDbType.BigInt) { Value = this.TransVersion };
					sqlCommand.Parameters.Add(parm);
				}

                stringBuilder.Append("UpdatedDate = SYSDATETIMEOFFSET() ");
			    parm = new SqlParameter("@UpdatedDate", SqlDbType.DateTimeOffset) { Value = DateTimeOffset.Now };
				sqlCommand.Parameters.Add(parm);

				string sql = stringBuilder.ToString();

				sql = sql + " WHERE TransID = @TransID ";
				sqlCommand.CommandText = sql;
			}
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method initializes the transaction flags and status object
		/// to its initial state.
		/// </summary>
		private void Init()
		{
			this.Flag01			= null;
			this.Flag02			= null;
			this.Flag03			= null;
			this.Flag04			= null;
			this.Flag05			= null;
			this.Flag06			= null;
			this.Date01			= null;
			this.Date02			= null;
			this.Date03			= null;
			this.Date04			= null;
			this.ErrorFlag		= null;
			this.TransStatus	= null;
			this.TransID		= null;
			this.TimeIn			= null;
			this.FST			= null;
			this.TimeEnd		= null;
			this.TransVersion	= null;
		}

		/// <summary>
		/// This method returns true if any of the flags or status are set.
		/// Otherwise, it returns false.
		/// </summary>
		/// <returns></returns>
		private bool HasValue()
		{
			bool hasValue = (this.Flag01 != null) 
							|| (this.Flag02 != null) 
							|| (this.Flag03 != null) 
							|| (this.Flag04 != null)
							|| (this.Flag05 != null) 
							|| (this.Flag06 != null) 
							|| (this.Date01 != null) 
							|| (this.Date02 != null)
							|| (this.Date03 != null) 
							|| (this.Date04 != null) 
							|| (this.ErrorFlag != null)
							|| (this.TransStatus != null) 
							|| (this.FST != null) 
							|| (this.TimeEnd != null)
							|| (this.TimeIn != null) 
							|| (this.TransVersion != null);

			return hasValue;
		}
		#endregion

		#region Override Methods
		override public string getSelectCommand()
		{
			return null;
		}

		override public string getInsertCommand()
		{
			return null;
		}

		override public string getDeleteCommand()
		{
			return null;
		}

		override public string getUpdateCommand()
		{
			return null;
		}
		#endregion
	}
}
