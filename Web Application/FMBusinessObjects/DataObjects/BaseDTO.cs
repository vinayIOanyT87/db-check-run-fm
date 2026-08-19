// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseDTO.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Extension methods for BaseDTO and related class - Persistence agnostic
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Globalization;
	using System.Linq;
	using System.Reflection;
	using System.Runtime.Serialization;
	using System.Xml;
	using System.Xml.Serialization;

	using FMBusinessObjects.UtilityObjects;

	[Serializable]
	[DataContract]
	public abstract class BaseDTO
	{
	    public const string ADMIN = BaseDataObject.ADMIN;

		[DataMember]
		protected Guid _IdentityGuid;

		[XmlIgnore]
		public Guid IdentityGuid { get { return _IdentityGuid; } set { _IdentityGuid = value; } }

		[DataMember]
		protected string _ID;

		[XmlIgnore]
		virtual public string ID { get { return _ID; } set { _ID = value; } }

		[DataMember]
		protected DateTimeOffset _CreatedDate;

		[XmlIgnore]
		public DateTimeOffset CreatedDate { get { return _CreatedDate; } set { _CreatedDate = value; } }

		[DataMember]
		protected string _CreatedBy;

		[XmlIgnore]
		public string CreatedBy { get { return _CreatedBy; } set { _CreatedBy = value; } }

		[DataMember]
		protected DateTimeOffset _UpdatedDate;

		[XmlIgnore]
		public DateTimeOffset UpdatedDate { get { return _UpdatedDate; } set { _UpdatedDate = value; } }

		[DataMember]
		protected string _UpdatedBy;

		[XmlIgnore]
		public string UpdatedBy { get { return _UpdatedBy; } set { _UpdatedBy = value; } }

		[DataMember]
		protected Guid _SiteGuid;

		[XmlIgnore]
		public Guid SiteGuid { get { return _SiteGuid; } set { _SiteGuid = value; } }

		[DataMember]
		protected string _SiteID;

		[XmlIgnore]
		virtual public string SiteID { get { return _SiteID; } set { _SiteID = value; } }

		[DataMember]
		protected bool _Deleted;

		[XmlIgnore]
		public bool Deleted { get { return _Deleted; } set { _Deleted = value; } }

		[DataMember]
		[XmlIgnore]
		public Byte[] RowVersion { get; set; }

		public static readonly Guid DUMMY_GUID = Guid.Empty;

		[XmlIgnore]
		public virtual ENTITY_TYPE EntityType { get { return ENTITY_TYPE.UNKNOWN; } set { ;} }

		[XmlIgnore]
		public virtual ENTITY_TYPE ParentEntityType { get { return ENTITY_TYPE.UNKNOWN; } set { ;} }

	    public BaseDTO()
	    {

	    }

        public BaseDTO(BaseDTO baseDto)
        {
            this._ID = string.IsNullOrEmpty(baseDto.ID) ? string.Empty : string.Copy(baseDto.ID);
            this._IdentityGuid = baseDto.IdentityGuid;
            this._CreatedDate = baseDto.CreatedDate;
            this._CreatedBy = string.IsNullOrEmpty(baseDto.CreatedBy) ? string.Empty : string.Copy(baseDto.CreatedBy);
            this._UpdatedDate = baseDto.UpdatedDate;
            this._UpdatedBy = string.IsNullOrEmpty(baseDto.UpdatedBy) ? string.Empty : string.Copy(baseDto.UpdatedBy);
            this._SiteID = string.IsNullOrEmpty(baseDto.SiteID) ? string.Empty : string.Copy(baseDto.SiteID);
            this._SiteGuid = baseDto.SiteGuid;
            this._Deleted = baseDto.Deleted;
        }

        public virtual void Reset()
		{
			_IdentityGuid = Guid.Empty;
			_SiteGuid = Guid.Empty;
			_ID = "";
			_SiteID = "";
			_CreatedDate = DateTimeOffset.Now;
			_CreatedBy = ADMIN;
			_UpdatedDate = CreatedDate;
			_UpdatedBy = ADMIN;
			_Deleted = false;
		}

		public void SetString(string propertyName, int limit, string value, ref string property)
		{
			if (string.IsNullOrEmpty(value))
			{
				property = "";
				return;
			}

			if (value.Length > limit)
			{
			    throw new Exception(string.Format("[{0}], [maximum length of] {1} [exceeded]", propertyName, limit));
			}

			property = value;
		}

		public void SetDate(string propertyName, string value, ref Date property)
		{
			try
			{
				if (!string.IsNullOrEmpty(value))
					property.Value = TimeConverter.ToDate(TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Parse(value, property.Format, DateTimeStyles.None),property.StandardName));
				else
					property.Value = DateTimeOffset.MinValue;
			}
			catch
			{
				throw new Exception(string.Format("[{0}], [invalid date format]", propertyName));
			}
		}

		public void SetDateAndTime(string propertyName, string value, ref DateAndTime property)
		{
			try
			{
				if (!string.IsNullOrEmpty(value))
					property.Value = DateTimeOffset.Parse(value, property.Format);
				else
					property.Value = DateTimeOffset.MinValue;
			}
			catch
			{
                throw new Exception(string.Format("[{0}], [invalid date time format]", propertyName));
			}
		}

		public void SetTime(string propertyName, string value, ref Time property)
		{
			try
			{
				if (!string.IsNullOrEmpty(value))
					property.Value = DateTimeOffset.Parse(value, property.Format);
				else
					property.Value = DateTimeOffset.MinValue;
			}
			catch
			{
                throw new Exception(string.Format("[{0}], [invalid time format]", propertyName));
			}
		}

		public void SetDouble(string propertyName, string value, ref double property)
		{
			try
			{
				property = Convert.ToDouble(value);
			}
			catch
			{
                throw new Exception(string.Format("[{0}], [invalid format]", propertyName));
			}
		}

		public void SetByte(string propertyName, string value, ref byte property)
		{
			try
			{
				property = Convert.ToByte(value);
			}
			catch
			{
                throw new Exception(string.Format("[{0}], [invalid format]", propertyName));
            }
		}

		public void SetInt(string propertyName, string value, ref int property)
		{
			try
			{
				property = Convert.ToInt32(value);
			}
			catch
			{
                throw new Exception(string.Format("[{0}], [invalid format]", propertyName));
            }
		}

		public void SetSIDouble(string propertyName, string value, ref SIDouble property)
		{
			try
			{
				property.Value = Convert.ToDouble(value, property.Format);
			}
			catch (Exception e)
			{
                throw new Exception(string.Format("[{0}] {1}", propertyName, e.Message));
			}
		}

		public string GetSIDouble(string propertyName, SIDouble property)
		{
			try
			{
				return property.ToString();
			}
			catch (Exception e)
			{
                throw new Exception(string.Format("[{0}] {1}", propertyName, e.Message));
            }

		}

		public void SetSIDifferential(string propertyName, string value, ref SIDifferential property)
		{
			try
			{
				property.Value = Convert.ToDouble(value, property.Format);
			}
			catch (Exception e)
			{
                throw new Exception(string.Format("[{0}] {1}", propertyName, e.Message));
            }
		}

		public string SetSIDifferential(string propertyName, SIDifferential property)
		{
			try
			{
				return property.ToString();
			}
			catch (Exception e)
			{
                throw new Exception(string.Format("[{0}] {1}", propertyName, e.Message));
            }

		}

		public void SetDecimal(string propertyName, string value, ref FMDecimal property)
		{
			try
			{
				property.Value = Convert.ToDecimal(value, property.Format);
			}
			catch
			{
                throw new Exception(string.Format("[{0}], [invalid decimal format]", propertyName));
			}
		}
	}

	[DataContract]
	[Serializable]
    public class BaseDTOWithUserData : BaseDTO
	{
		// Additional Data
		[DataMember]
		[XmlIgnore]
		public UserDataClass UserData;

        public BaseDTOWithUserData()
            : base()
        {
        }

        public BaseDTOWithUserData(BaseDTOWithUserData baseDTOWithUserData)
            : base((BaseDTO)baseDTOWithUserData)
	    {
	    }
	}
}
