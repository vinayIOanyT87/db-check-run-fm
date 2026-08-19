// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StorageTransferDO.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
// Represents a storage transfer transaction
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Runtime.Serialization;

	[Serializable]
	[DataContract]
    public class StorageTransferDO : TransactionDO
    {
        public StorageTransferDO()
        {
        }
    }
}
