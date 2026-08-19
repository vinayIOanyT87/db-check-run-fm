/******************************************************************************

	FILE NAME:		FMCustomFieldStates.cs


	PURPOSE:			FMCustomFieldStates class implementation
					Uses late binding to set attributes on a web page as per
					custom requirements.

	COMMENTS:

		Copyright (C) Varec, Inc. (A Leidos Company) Norcross, GA, USA, 2008

		This file shall not be copied or reproduced in any form without
				the express written consent of Varec, Inc.

	AUTHOR(S):	Eric Simmons

	VERSION:		1.0.0  Current version

	MODIFICATION HISTORY:
		Date:			By:			Reason:
		---------	----------  -------------------------------------------
		10-9-2008	E. Simmons	Initial Revision
*******************************************************************************/
namespace FMControls
{
	using System;
	using System.Reflection;
	using System.Configuration;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Interfaces;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using System.Runtime.InteropServices;

	/// <summary>
	/// Summary description for FMCustomFieldStatesClass.
	/// </summary>
	public class FMCustomFieldStatesClass
	{
		#region Attributes
		string assemblyPath;
		static Assembly DLL;
		#endregion

		#region Construction
		public FMCustomFieldStatesClass ( )
		{
			assemblyPath = this.RegGetCustomTransactionFieldAssemblyPath ( );
			DLL = null;
		}
		#endregion

		#region Properties
		public string AssemblyPath
		{
			get { return assemblyPath; }
			set { assemblyPath = value; }
		}
		#endregion

		#region Private Methods
		//Eric Simmons - 10-09-2008
		//Added to support CSI #6153
		//This potentially needs to be modified to allow each site to determine which assembly to use to calculate
		//price.  Right now this is a system wide setting.
		// bds 23 Oct Added try catch so the registry entry not being present will not cause the application
		// to not function. This should be moved out of the registry and into the database or web.ini file
		// since we will need to give asp.net access to the local computer registry.
		private string RegGetCustomTransactionFieldAssemblyPath ( )
		{
			try
			{
				string customTxAssemPath = ConfigurationManager.AppSettings["CustomTransactionFieldAssemblyPath"];

				if (string.IsNullOrEmpty(customTxAssemPath) == true)
				{
					customTxAssemPath = "";
				}

				return customTxAssemPath;
			}
			catch
			{
				return null;
			}
		}
		#endregion

		#region Public Methods
		public void SetTransactionFieldStates ( SecurityClass security, System.Web.UI.Page page )
		{
			try
			{
				if (security == null)
				{
					throw new NullReferenceException ( "Null SecurityClass Object Reference" );
				}

				if (page == null)
				{
					throw new NullReferenceException ( "Null System.Web.UI.Page Object Reference" );
				}

				if (string.IsNullOrEmpty ( this.assemblyPath ) == true)
				{
					return;
				}

				if (DLL == null)
				{
					if (!AssemblyDictionary.ContainsKey(assemblyPath.ToLower()))
					{
						try
						{
							DLL = Assembly.LoadFrom(assemblyPath);
						}
						catch
						{
							try
							{
								DLL = Assembly.Load(assemblyPath);
							}
							catch (Exception ex)
							{
								string message = "Assembly Load Error in Set Transaction Field States. " + ex.Message;
								FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(message, FMEventLogEntryType.Warning));
							}
						}

						if (DLL != null)
							AssemblyDictionary.Add(assemblyPath.ToLower(), DLL);
					}
					else
					{
						DLL = AssemblyDictionary.Get(assemblyPath.ToLower());
					}
				}

				if (DLL == null)
					return;

				try
				{
					Type[] types = DLL.GetTypes();

					foreach (Type Module in types)
					{
						Type FMCustomFieldStatesInterface = Module.GetInterface("IFMCustomFieldStates");

						if (FMCustomFieldStatesInterface != null)
						{
							Object lateBoundObj = Activator.CreateInstance(Module);
							IFMCustomFieldStates customFieldsStates = (IFMCustomFieldStates)lateBoundObj;
							customFieldsStates.SetTransactionFieldStates(security, page);
							break;
						}
					}
				}
				catch { }
			}
			catch (Exception e)
			{
				throw e;
			}
		}
		public void SetTransactionFieldState ( SecurityClass security, System.Web.UI.WebControls.WebControl control )
		{
			try
			{
				if (security == null)
				{
					throw new NullReferenceException ( "Null SecurityClass Object Reference" );
				}

				if (control == null)
				{
					throw new NullReferenceException ( "Null System.Web.UI.WebControls.WebControl Object Reference" );
				}

				if (assemblyPath == null || assemblyPath.Length == 0)
					return;


				if (DLL == null)
				{
					if (!AssemblyDictionary.ContainsKey(assemblyPath.ToLower()))
					{
						try
						{
							DLL = Assembly.LoadFrom(assemblyPath);
						}
						catch
						{
							try
							{
								DLL = Assembly.Load(assemblyPath);
							}
							catch (Exception ex)
							{
								string message = "Assembly Load Error in Set Transaction Field State. " + ex.Message;
								FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(message, FMEventLogEntryType.Warning));
							}
						}

						if (DLL != null)
							AssemblyDictionary.Add(assemblyPath.ToLower(), DLL);
					}
					else
					{
						DLL = AssemblyDictionary.Get(assemblyPath.ToLower());
					}
				}

				if (DLL == null)
					return;

				try
				{
					Type[] types = DLL.GetTypes();
					foreach (Type Module in types)
					{
						Type FMCustomFieldStatesInterface = Module.GetInterface("IFMCustomFieldStates");
						if (FMCustomFieldStatesInterface != null)
						{
							Object lateBoundObj = Activator.CreateInstance(Module);
							IFMCustomFieldStates customFieldsStates = (IFMCustomFieldStates)lateBoundObj;
							customFieldsStates.SetTransactionFieldState(security, control);
							break;
						}
					}
				}
				catch { }
			}
			catch (Exception e)
			{
				throw e;
			}
		}
		#endregion
	}
}
