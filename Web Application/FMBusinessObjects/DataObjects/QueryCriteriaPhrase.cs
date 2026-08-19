using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FMBusinessObjects.DataObjects
{
	[Serializable]
	[CollectionDataContract]
	public class QueryCriteriaPhraseCollection : List<QueryCriteriaPhrase> { }

	public enum QueryAndOr
	{
		AND,
		OR
	}

	public enum QueryOperator
	{
		Equals,
		GreaterThan,
		LessThan,
		GreaterThanEqual,
		LessThanEqual,
		NotEqual,
		Like,
		NotLike,
		Contains,
		IN,
		Between, // hidden type for use by date page filters
		NullOrEmpty,
		NotNullOrEmpty
	}

	public enum QueryCriteriaType
	{
		Phrase,
		StartGroup,
		EndGroup
	}

	[Serializable]
	[DataContract]
	[XMLObject(NodeName = "QueryCriteriaPhrase")]
	public class QueryCriteriaPhrase
	{
		[DataMember]
		public QueryWriterTopic Topic { get; set; }
		[DataMember]
		public QueryWriterField Field { get; set; }
		[DataMember]
		public string Value2 { get; set; }

		[XMLProperty]
		[DataMember]
		public string Value { get; set; }
		[XMLProperty]
		[DataMember]
		public QueryCriteriaType Type { get; set; }
		[XMLProperty]
		[DataMember]
		public QueryOperator Operator { get; set; }
		[XMLProperty]
		[DataMember]
		public QueryAndOr Conjunction { get; set; }

		[XMLProperty]
		public Type TopicObjectType
		{
			get { return Topic.ObjectType; }
		}

		[XMLProperty]
		public string FieldName
		{
			get
			{
				if (Field != null)
				{
					return Field.FieldName;
				}

				return string.Empty;
			}
		}

		[XMLProperty]
		public string DbFieldName
		{
			get
			{
				if (Field != null)
				{
					return Field.DBFieldName;
				}

				return string.Empty;
			}
		}

		public QueryCriteriaPhrase()
		{
			this.Reset();
		}

		public QueryCriteriaPhrase(QueryWriterTopic topic)
		{
			this.Reset();
			this.Topic = topic;
		}

		public QueryCriteriaPhrase(QueryCriteriaType type)
		{
			this.Reset();
			this.Type = type;
		}

		protected void Reset()
		{
			this.Operator		= QueryOperator.Equals;
			this.Conjunction	= QueryAndOr.AND;
			this.Type			= QueryCriteriaType.Phrase;
			this.Value			= string.Empty;
			this.Field			= null;
		}

	}

}
