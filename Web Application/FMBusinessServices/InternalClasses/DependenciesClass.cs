// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DependenciesClass(security).cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FMBusinessServices.InternalClasses
{
	using System;
	using System.Reflection;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;

	using FMBusinessServices.ServiceClasses;

	public interface IDependency
	{
		#region Public Methods and Operators

		void Insert(SecurityClass Security, BaseDataObject Object, bool preOperation);

		void Purge(SecurityClass Security, BaseDataObject Object);

		void Update(SecurityClass Security, BaseDataObject Object);

		#endregion
	}

	/// <summary>
	///     Summary description for DependenciesClass(security).
	/// </summary>
	public class DependenciesClass
	{
		#region Constants and Fields

		private const string MESSAGE_01 = "The IDependencyAssemblies setting is missing or the value is empty";

		private string[] dependencyAssemblyList;

		#endregion

		#region Constructors and Destructors

		public DependenciesClass(SecurityClass security)
		{
			var configSettings = new ConfigurationSettingsClass();

			ConfigurationSettingDOClass configSetting = configSettings.GetByKey(
				security, ConfigurationSettingDOClass.Key_IDependencyAssemblies);

			if (string.IsNullOrEmpty(configSetting.SettingValue) == false)
			{
				dependencyAssemblyList = configSetting.GetStringArray();
			}
		}

		#endregion

		#region Enums

		private enum OperationType
		{
			Insert,

			Update,

			Purge
		}

		#endregion

		#region Public Methods and Operators

		public void Insert(SecurityClass security, BaseDataObject Object, bool preOperation)
		{
			this.Operate(OperationType.Insert, security, Object, preOperation);
		}

		public void Purge(SecurityClass security, BaseDataObject Object)
		{
			this.Operate(OperationType.Purge, security, Object, false);
		}

		public void Update(SecurityClass security, BaseDataObject Object)
		{
			this.Operate(OperationType.Update, security, Object, false);
		}

		#endregion

		#region Methods

		private void Operate(OperationType opType, SecurityClass security, BaseDataObject Object, bool preOperation)
		{
			if (dependencyAssemblyList == null)
			{
				throw new ApplicationException(MESSAGE_01);
			}

			string baseDirectory = AppDomain.CurrentDomain.BaseDirectory + "\\bin\\";

			foreach (string assemblyName in dependencyAssemblyList)
			{
				Assembly dll = null;

				if (!AssemblyDictionary.ContainsKey(assemblyName.ToLower()))
				{
					try
					{
						dll = Assembly.LoadFrom(baseDirectory + assemblyName);
					}
					catch
					{
						try
						{
							dll = Assembly.Load(assemblyName);
						}
						catch (Exception ex)
						{
							string message = "Assembly Load Error in IDependency Operate. " + ex.Message;
							FMEventLog eventLog = new FMEventLog();
							eventLog.WriteEntry(message, FMEventLogEntryType.Warning);
						}
					}

					if (dll != null)
						AssemblyDictionary.Add(assemblyName.ToLower(), dll);
				}
				else
				{
					dll = AssemblyDictionary.Get(assemblyName.ToLower());
				}

				if (dll == null)
					continue;

				try
				{
					Type[] types = dll.GetTypes();

					foreach (Type module in types)
					{
						if (module.GetInterface("IDependency") != null)
						{
							var dependentObject = Activator.CreateInstance(module) as IDependency;

							if (dependentObject != null)
							{
								switch (opType)
								{
									case OperationType.Insert:
										dependentObject.Insert(security, Object, preOperation);
										break;
									case OperationType.Update:
										dependentObject.Update(security, Object);
										break;
									case OperationType.Purge:
										dependentObject.Purge(security, Object);
										break;
								}
							}
						}
					}
				}
				catch { } // Try: Type[] types = dll.GetTypes()
			}
		}

		#endregion
	}
}