namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Runtime.Serialization;
	using System.Xml.Serialization;

	/// <summary>
	/// Represents a collection of additive profiles
	/// </summary>
	[CollectionDataContract]
	[KnownType(typeof(AdditiveProfileClass))]
	[Serializable()]
	public class AdditiveProfileCollectionClass : CollectionBase
	{

		public void Add(AdditiveProfileClass AdditiveProfile)
		{
			List.Add(AdditiveProfile);
		}

		public void Remove(int index)
		{
			if (index > Count - 1 || index < 0)
			{
				throw (new Exception("Invalid Index"));
			}
			else
			{
				List.RemoveAt(index);
			}
		}

		public void Remove(AdditiveProfileClass AdditiveProfile)
		{
			int index = 0;
			foreach (AdditiveProfileClass Item in List)
			{
				if (Item.IdentityGuid == AdditiveProfile.IdentityGuid)
				{
					List.RemoveAt(index);
					return;
				}
				index++;
			}
		}

		public AdditiveProfileClass this[int Index]
		{
			get { return (AdditiveProfileClass)List[Index]; }
			set { List[Index] = value; }
		}

		public AdditiveProfileClass Items(int Index)
		{
			return (AdditiveProfileClass)List[Index];
		}
	}

	/// <summary>
	/// An additive profile contains one or more additives and the amount of each additive that should be added 
	/// </summary>
	[DataContract]
	[Serializable()]
	public class AdditiveProfileClass : BaseDataObject
	{
		[DataMember]
		string description;

		[DataMember]
		public ProductMapCollectionClass AdditiveCollection;

		public override string ID { get { return _ID; } set { SetString("ID", 30, value, ref _ID); } }
		public string Description { get { return description; } set { SetString("Description", 50, value, ref description); } }


		[XmlIgnoreAttribute]
		public override ENTITY_TYPE EntityType
		{
			get
			{
				return ENTITY_TYPE.ADDITIVE_PROFILE;
			}
		}

		[XmlIgnoreAttribute]
		public override ENTITY_TYPE ParentEntityType
		{
			get
			{
				return ENTITY_TYPE.NONE;
			}
		}

		public AdditiveProfileClass()
		{
			Initialize();
		}

		public override string ToString()
		{
			return ID;
		}

		private void Initialize()
		{
			description = string.Empty;
			AdditiveCollection = new ProductMapCollectionClass();
		}

		public override void Reset()
		{
			base.Reset();
			Initialize();
		}
	}
}
