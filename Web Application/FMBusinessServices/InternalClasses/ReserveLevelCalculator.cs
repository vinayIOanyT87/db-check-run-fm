// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReserveLevelCalculator.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FMBusinessServices.InternalClasses
{
	using System;
	using System.Diagnostics;
	using System.Reflection;

	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.LogClient;
	using FMBusinessObjects.UtilityObjects;

	public class ReserveLevelCalculator
	{
		// private PropertyInfo piVolume = null;
		#region Constants and Fields

		private readonly Logger logger;

		private MethodInfo miAddProducts;

		private MethodInfo miCalculateVolume;

		private object reserveLevelCalculator;

		static Assembly asm = null;


		#endregion

		#region Constructors and Destructors

		public ReserveLevelCalculator()
		{
			this.logger = new Logger("ReserveLevelCalculator");
			this.CreateReserveLevelCalculator();
		}

		#endregion

		#region Public Methods and Operators

		/// <summary>
		/// Invoke reserve level method that determines total reserve volume and
		///     send email if go below the warning and minimum level.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="trans">
		/// The trans.
		/// </param>
		public void AddProducts(SecurityClass security, TransactionDO trans)
		{
			if (this.miAddProducts == null)
			{
				return;
			}

			this.miAddProducts.Invoke(this.reserveLevelCalculator, new object[] { security, trans });
		}

		public void CalculateVolume(SecurityClass security, DateTimeOffset inventoryDate, SaveTransactionsResultDO resultsDO)
		{
			if (this.miCalculateVolume == null)
			{
				return;
			}

			this.miCalculateVolume.Invoke(this.reserveLevelCalculator, new object[] { security, resultsDO, inventoryDate });
		}

		#endregion

		#region Methods

		/// <summary>
		///     Load assembly and establish invokation methods and properties.
		/// </summary>
		private void CreateReserveLevelCalculator()
		{
			try
			{
				if (asm == null)
				{
					var assemblyName = AppSettingsHelper.GetKeyValue<string>("ReserveLevelCalculator", null);

					if (string.IsNullOrEmpty(assemblyName))
					{
						return;
					}
					else
					{
						asm = Assembly.Load(assemblyName);
					}
				}

				if (asm != null)
				{
					this.reserveLevelCalculator = asm.CreateInstance("ADFComponents.ReserveLevelCalculator");
					if (this.reserveLevelCalculator != null)
					{
						Type calculatorType = this.reserveLevelCalculator.GetType();
						Type securityType = typeof(SecurityClass);
						Type transactionDOType = typeof(TransactionDO);
						Type resultsType = typeof(SaveTransactionsResultDO);
						Type inventoryDate = typeof(DateTimeOffset);

						this.miCalculateVolume = calculatorType.GetMethod("CalculateVolume", new[] { securityType, resultsType, inventoryDate });
						this.miAddProducts = calculatorType.GetMethod("AddProducts", new[] { securityType, transactionDOType });
					}
				}
			}
			catch (Exception exc)
			{
				this.logger.Debug(exc.Message);
			}
		}

		#endregion
	}
}