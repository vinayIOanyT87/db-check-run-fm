namespace FMBusinessObjects.Interfaces
{
	using System;

	using FMBusinessObjects.DataObjects;

	[CLSCompliant(false)]
	public interface ISecurityDiscovery
	{
		#region Public Methods and Operators

		RightCollectionClass GetSecurityRights(SecurityClass security, uint options, uint specialKeyCodes);

		#endregion
	}
}