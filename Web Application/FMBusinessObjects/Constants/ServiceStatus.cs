// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceStatus.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Represents various states of a WCF Service call
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMBusinessObjects.Constants
{
    /// <summary>
    /// Enum ServiceStatus
    /// </summary>
    public enum ServiceStatus
    {
        /// <summary>
        /// The idle
        /// </summary>
        Idle = 0,
        /// <summary>
        /// The busy
        /// </summary>
        Busy = 1,
        /// <summary>
        /// The executing
        /// </summary>
        Executing = 2,
        /// <summary>
        /// The completed
        /// </summary>
        Completed = 3,
        /// <summary>
        /// The faulted
        /// </summary>
        Faulted = 4,
        /// <summary>
        /// The canceled
        /// </summary>
        Canceled = 5
    }
}
