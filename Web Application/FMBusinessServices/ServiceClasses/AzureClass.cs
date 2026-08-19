// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AzureClass.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the AzureClass type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.ServiceClasses
{
	using System.ServiceModel;
	using FMBusinessObjects.BusinessInterfaces;
	using Microsoft.WindowsAzure.ServiceRuntime;

	/// <summary>
	/// The AzureClass provides support functions for working with the Azure environment.
	/// </summary>
	public class AzureClass : IAzure
	{
		#region Public Methods and Operators

		/// <summary>
		/// Gets the communication channel.
		/// </summary>
		/// <returns>A status string describing the communication channel.</returns>
		public string GetCommunicationChannel()
		{
			return string.Format("We are talking via {0}", OperationContext.Current.Channel.LocalAddress.Uri);
		}

		/// <summary>
		/// Gets the role info from the current instance.
		/// </summary>
		/// <returns>A status string describing the role of the current instance.</returns>
		public string GetRoleInfo()
		{
			RoleInstance currentRoleInstance = RoleEnvironment.CurrentRoleInstance;

			string roleName = currentRoleInstance.Role.Name;
			string roleInstanceID = currentRoleInstance.Id;

			return string.Format("You are talking to role {0}, instance ID {1}\n", roleName, roleInstanceID);
		}

		#endregion
	}
}