// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GasboyStationProduct.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the GasboyStationProductCollection type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.Afss.Module.Gasboy.BusinessObjects.DataObjects
{
    using System;
    using System.Runtime.Serialization;
    using System.Xml.Serialization;


    [XmlType("GasboyStationProduct")]
    [DataContract]
    [Serializable]
    public class GasboyStationProduct
    {
        #region Data Members
        #endregion Data Members

        #region Properties
        /// <summary>
        /// Identifies the external station which this transaction came from
        /// </summary>
        [DataMember]
        public Guid ExternalStationGuid { get; set; }

        /// <summary>
        /// The user friendly ID the external station which this transaction came from
        /// </summary>
        [DataMember]
        public string ExternalStationID { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public int IdentityId { get; set; }

        [DataMember]
        public int CodeId { get; set; }

        [DataMember]
        public int StatusIndex { get; set; }

        [DataMember]
        public int ProductTypeIndex { get; set; }

        [DataMember]
        public int Color { get; set; }

        [DataMember]
        public double Density { get; set; }

        [DataMember]
        public double Price{ get; set; }

        [DataMember]
        public double LevelLowA { get; set; }

        [DataMember]
        public double LevelLowADeadband { get; set; }

        [DataMember]
        public int LevelLowADeadbandTypeIndex{ get; set; }

        [DataMember]
        public string Code2 { get; set; }

        [DataMember]
        public string Code3 { get; set; }

        #endregion Properties

        #region Constructors
        /// <summary>
        /// This is the default constructor for the GasboyStationProduct class.
        /// </summary>
        public GasboyStationProduct()
            : base()
        {
            this.Reset();
        }

        #endregion Constructors

        #region Public methods
        public void Reset()
        {
        }
        #endregion Public Methods
    }
}
