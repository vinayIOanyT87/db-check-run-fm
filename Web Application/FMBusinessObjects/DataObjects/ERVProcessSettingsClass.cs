// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ERVProcessSettingsClass.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the ERVProcessSettingsClass type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.DataObjects
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Runtime.Serialization;


    [DataContract]
    [Serializable]
    public class ERVProcessSettingsClass : BaseDataObject
    {


        [DataMember]
        public bool InhibitGlobalFieldsProcessing { get; set; }


        public void Load(DataRow row)
        {
            if (row == null)
                throw new ArgumentNullException(nameof(row));
            this.Reset();

            this.InhibitGlobalFieldsProcessing = DataObject.getValue<bool>(row["InhibitGlobalFieldsProcessing"], false);
            this._UpdatedBy = DataObject.getValue<string>(row["UpdatedBy"], ADMIN);
            this._UpdatedDate = DataObject.getValue<DateTimeOffset>(row["UpdatedDate"], DateTimeOffset.Now);

        }


		public override void Load(object o)
		{
			if (typeof(DataSet).IsInstanceOfType(o))
			{
				var dataSet = o as DataSet;

				if (dataSet == null)
				{
					throw new ArgumentNullException("Set");
				}

				Reset();

				DataTable table = dataSet.Tables[0];

				if (table.Rows.Count == 0)
				{
					return;
				}

				Load(table.Rows[0]);
			}
			else
			{
				throw new Exception("ERVProcessSettings : Unknown object type on load");
			}
		}


	}
}
