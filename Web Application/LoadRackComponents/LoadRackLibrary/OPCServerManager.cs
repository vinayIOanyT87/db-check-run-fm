/******************************************************************************

	FILE NAME:		OPCServerManager.cs


	PURPOSE:			OPCServerManagerClass


	COMMENTS:

		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2000

		This file shall not be copied or reproduced in any form without
				the express written consent of Endress+Hauser.


	AUTHOR(S):	W. Gray


	VERSION:		1.0.0  Current version



	MODIFICATION HISTORY:
		Date:			By:			Reason:
		---------	----------  -------------------------------------------
		10/10/2007	W.Gray		7.1.1.1 - Revised OnDataChange to assigne value
										only when OPC Quality is good (CSI 5271)

		07/23/2009	W.Gray		7.4.6.0 - Revised OnDataChange to update
										all process variables prior to calling invoke
										for each one.  This is to improve data cohenrency
 
		12/1/2009	W.Gray		7.4.6.1 - Revised to add and remove items from the
										scan thread only.  This is to prevent a deadlock wiht
										the call back. (WI 9637)

		12/3/2009	W.Gray		7.4.6.3 - Revised Read methods to set item Active = false (WI 9637)

		12/22/2009	W.Gray		7.4.6.4 - Revised to perform Update as part of Scan Thread (WI 10217)

		01/18/2010	W.Gray		7.4.6.5 - Revised to add items inactive and then activate
										using modify items.  This is to ensure the add items has completed
										processing prior to any server call to OnDataChange for the items.
										This change was necessitated by the OPC.NET AddItems which would
										not set the ClientHandle until after the call to server AddItems. (WI 10343)

*******************************************************************************/
using System;
using System.ServiceProcess;
using System.Collections;
using System.Diagnostics;
using Opc;
using Opc.Da;
using OpcCom.Da;
using System.Net;
using System.Threading;
using System.Runtime.InteropServices;

using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.Exceptions;
using FMBusinessObjects.ServiceRequests;
using FMBusinessObjects.UtilityObjects;

namespace LoadRackLibrary
{
	public delegate void InvokeEventHandler(ProcessVariableClass PV);

	[Serializable()]
	public class OpcException : Exception
	{
		public OpcException() { }
		public OpcException(string message) : base(message) { }
	}

	/// <summary>
	/// Summary description for OPCServerManagerClass.
	/// </summary>
	public class OPCServerManagerClass
	{
		protected bool AlreadyDisposed = false;
		protected EventLog EventLog;
		protected SubscriptionCollection SubscriptionCollection = new SubscriptionCollection();
		protected ManualResetEvent OPCServerScanKillEvent = null;
		protected AutoResetEvent OPCServerItemEvent = null;
		protected AutoResetEvent OPCServerUpdateEvent = null;
		protected AutoResetEvent OPCServerCOSEvent = null;
		protected Thread OPCServerScanThread = null;
		protected ProcessVariableCollectionCollectionClass AddPVCollectionCollection = new ProcessVariableCollectionCollectionClass();
		protected ProcessVariableCollectionCollectionClass RemovePVCollectionCollection = new ProcessVariableCollectionCollectionClass();

		public event InvokeEventHandler Invoke;


		public OPCServerManagerClass(EventLog EventLog)
		{
			this.EventLog = EventLog;

			ThreadStart OPCServerScanStart = new ThreadStart(OPCServerScan);

			OPCServerScanKillEvent = new ManualResetEvent(false);
			OPCServerItemEvent = new AutoResetEvent(false);
			OPCServerUpdateEvent = new AutoResetEvent(false);
			OPCServerCOSEvent = new AutoResetEvent(false);

			OPCServerScanThread = new Thread(OPCServerScanStart);
			OPCServerScanThread.Start();

		}

		~OPCServerManagerClass()
		{
			Dispose();
		}

		public void Dispose()
		{
			if (!AlreadyDisposed)
			{
				// Terminate the Scan Thread
				if (OPCServerScanKillEvent != null)
					OPCServerScanKillEvent.Set();
				if (OPCServerScanThread != null)
					OPCServerScanThread.Join();

				GC.SuppressFinalize(this);
				AlreadyDisposed = true;
			}
		}

		private void OPCServerScan()
		{
			WaitHandle[] Events = { OPCServerScanKillEvent, OPCServerUpdateEvent, OPCServerCOSEvent, OPCServerItemEvent };

			int WaitResult;
			while (0 != (WaitResult = WaitHandle.WaitAny(Events, 10000, true)))
			{
				try
				{

					switch (WaitResult)
					{
						case 1:
						case 2:
							{
								foreach (Opc.Da.Subscription Subscription in SubscriptionCollection)
								{
									ArrayList ItemValues = new ArrayList();

									foreach (Opc.Da.Item Item in Subscription.Items)
									{
										ProcessVariableClass PV = Item.ClientHandle as ProcessVariableClass;
										if (PV == null
									|| PV.Input)
											continue;

										if (WaitResult == 2
									&& !PV.DataChanged
									&& !PV.OutputFailed)
											continue;

										ItemValue Value = new ItemValue(Item);
										Value.ItemName = PV.OPCItemID;
										Value.Value = PV.ServerValue;
										Value.ClientHandle = PV;

										ItemValues.Add(Value);
									}

									if (ItemValues.Count == 0)
										continue;

									IdentifiedResult[] IdentifiedResults = Subscription.Write((Opc.Da.ItemValue[])ItemValues.ToArray(typeof(Opc.Da.ItemValue)));

									int Index = 0;
									foreach (IdentifiedResult Result in IdentifiedResults)
									{
										ProcessVariableClass ResultPV = ((ItemValue)ItemValues[Index++]).ClientHandle as ProcessVariableClass;
										if (Result.ResultID == ResultID.S_OK)
										{
											ResultPV.OutputFailed = false;
											continue;
										}

										if (!ResultPV.OutputFailed)
										{
											string ErrorText = "OPCServerManager Update Error : " + Result.ItemName + " " + IdentifiedResults[0].ResultID.ToString();
											ResultPV.OutputFailed = true;
											EventLog.WriteEntry(ErrorText, EventLogEntryType.Error);
										}
									}
								}
								break;
							}

						case 3:
						case WaitHandle.WaitTimeout:
							{
								ProcessVariableCollectionClass PVCollection = null;

								while (RemovePVCollectionCollection.Count > 0)
								{
									// Remove Items requested for removal
									Monitor.Enter(this);

									if (RemovePVCollectionCollection.Count > 0)
									{
										PVCollection = RemovePVCollectionCollection.Item(0);
										RemovePVCollectionCollection.Remove(0);
									}
									else
										PVCollection = null;

									Monitor.Exit(this);

									if (PVCollection != null)
									{
										foreach (Opc.Da.Subscription Subscription in SubscriptionCollection)
										{
											if (Subscription.Server.Url.ToString() == PVCollection[0].URL)
											{

												ArrayList items = new ArrayList();
												foreach (ProcessVariableClass PV in PVCollection)
												{
													foreach (Item item in Subscription.Items)
													{
														if (item.ItemName == PV.OPCItemID)
														{
															ItemIdentifier itemIdentifier = new ItemIdentifier(item.ItemName);
															itemIdentifier.ClientHandle = PV;
															itemIdentifier.ServerHandle = item.ServerHandle;
															items.Add(itemIdentifier);
															break;
														}
													}
												}

												if (items.Count > 0)
												{
													IdentifiedResult[] ItemResults = Subscription.RemoveItems((Opc.ItemIdentifier[])items.ToArray(typeof(Opc.ItemIdentifier)));

													foreach (IdentifiedResult ItemResult in ItemResults)
													{
														if (ItemResult.ResultID != ResultID.S_OK)
															EventLog.WriteEntry("OPCServerManager RemoveItem Error : " + ItemResult.ItemName + " " + ItemResult.ResultID.ToString(), EventLogEntryType.Error);
													}
												}

												if (Subscription.Items.Length == 0)
												{
													SubscriptionCollection.Remove(Subscription);
													Subscription.Dispose();
												}

												break;
											}
										}
									}
								}

								// Add Items requested for addition
								Monitor.Enter(this);

								if (AddPVCollectionCollection.Count > 0)
								{
									PVCollection = AddPVCollectionCollection.Item(0);
									AddPVCollectionCollection.Remove(0);
								}
								else
									PVCollection = null;

								Monitor.Exit(this);

								if (PVCollection != null)
								{
									int Count = 0;
									foreach (Opc.Da.Subscription ExistingSubscription in SubscriptionCollection)
									{
										if (ExistingSubscription.Server.Url.ToString() == PVCollection[0].URL)
											break;
										Count++;
									}

									Opc.Da.Subscription Subscription = null;

									if (Count == SubscriptionCollection.Count)
									{
										try
										{
											Opc.Da.Server Server = new Opc.Da.Server(new OpcCom.Factory(), new URL(PVCollection[0].URL));
											NetworkCredential Credentials = null;
											Server.Connect(new ConnectData(Credentials));
											Opc.Da.SubscriptionState State = new SubscriptionState();
											State.ClientHandle = Guid.NewGuid().ToString();
											State.Active = true;
											State.UpdateRate = 1000;
											Subscription = (Opc.Da.Subscription)Server.CreateSubscription(State);
											SubscriptionCollection.Add(Subscription);
											Subscription.DataChanged += new DataChangedEventHandler(OnDataChanged);
										}
										catch
										{
										}
									}
									else
										Subscription = SubscriptionCollection[Count];


									if (Subscription == null)
									{
										foreach (ProcessVariableClass PV in PVCollection)
											AddProcessVariable(PV);
									}

									else
									{
										// Add Items Inactive to prevent call back prior to completion of AddItems
										ArrayList Items = new ArrayList();
										foreach (ProcessVariableClass PV in PVCollection)
										{
											Item Item = new Item(new ItemIdentifier(PV.OPCItemID));
											Item.ClientHandle = PV;
											Item.Active = false;
											Item.ActiveSpecified = true;
											Item.ReqType = GetType(PV.DataType);
											Items.Add(Item);
										}

										ItemResult[] ItemResults = Subscription.AddItems((Opc.Da.Item[])Items.ToArray(typeof(Opc.Da.Item)));

										foreach (ItemResult ItemResult in ItemResults)
										{
											if (ItemResult.ResultID != ResultID.S_OK)
												EventLog.WriteEntry("OPCServerManager AddItem Error : " + ItemResult.ItemName + " " + ItemResult.ResultID.ToString(), EventLogEntryType.Error);
										}

										// Activate Items
										Items = new ArrayList();
										foreach (Item item in Subscription.Items)
										{
											ProcessVariableClass PV = item.ClientHandle as ProcessVariableClass;
											if (!PV.Input)
												continue;

											if (item.Active)
												continue;

											item.Active = true;
											item.ActiveSpecified = true;

											Items.Add(item);
										}

										if (Items.Count > 0)
										{
											ItemResults = Subscription.ModifyItems((int)StateMask.Active, (Opc.Da.Item[])Items.ToArray(typeof(Opc.Da.Item)));

											foreach (ItemResult ItemResult in ItemResults)
											{
												if (ItemResult.ResultID != ResultID.S_OK)
													EventLog.WriteEntry("OPCServerManager AddItem Error : " + ItemResult.ItemName + " " + ItemResult.ResultID.ToString(), EventLogEntryType.Error);
											}
										}
									}
								}


								// Check the status of each Subscription
								for (int Index = 0; Index < SubscriptionCollection.Count; Index++)
								{
									Opc.Da.Subscription Subscription = SubscriptionCollection[Index];

									ProcessVariableClass PV = null;

									try
									{
										PV = (ProcessVariableClass)Subscription.Items[0].ClientHandle;

										ServerStatus Status = Subscription.Server.GetStatus();

										// If prior failure then set values to uncertain and refresh
										if (PV.OPCQuality == new Quality(qualityBits.badNotConnected).GetCode())
										{
											ItemValueResult[] Values = new ItemValueResult[Subscription.Items.Length];

											int Count = 0;
											foreach (Item Item in Subscription.Items)
											{
												ItemValueResult Value = new ItemValueResult();
												Value.Timestamp = DateTime.UtcNow;
												Value.Quality = new Quality(qualityBits.uncertain);
												Value.ClientHandle = Item.ClientHandle;
												Values[Count] = Value;
												Count++;
											}

											OnDataChanged(null, null, Values);
											Subscription.Refresh();
										}
									}
									catch
									{
										// Subscription Failed Try to Reconnect
										try
										{
											Opc.Da.Server Server = new Opc.Da.Server(new OpcCom.Factory(), Subscription.Server.Url);
											NetworkCredential Credentials = null;
											Server.Connect(new ConnectData(Credentials));

											Opc.Da.SubscriptionState State = new SubscriptionState();
											State.ClientHandle = Guid.NewGuid().ToString();
											State.Active = true;
											State.UpdateRate = 1000;
											Opc.Da.Subscription NewSubscription = (Opc.Da.Subscription)Server.CreateSubscription(State);

											NewSubscription.DataChanged += new DataChangedEventHandler(OnDataChanged);

											foreach (Item item in Subscription.Items)
											{
												item.Active = false;
												item.ActiveSpecified = true;
											}

											ItemResult[] ItemResults = NewSubscription.AddItems(Subscription.Items);

											ArrayList Items = new ArrayList();

											foreach (ItemResult ItemResult in ItemResults)
											{
												if (ItemResult.ResultID != ResultID.S_OK)
													EventLog.WriteEntry("OPCServerManager AddItem Error : " + ItemResult.ItemName + " " + ItemResult.ResultID.ToString(), EventLogEntryType.Error);
											}

											// Activate Items
											Items = new ArrayList();
											foreach (Item item in Subscription.Items)
											{
												PV = item.ClientHandle as ProcessVariableClass;
												if (!PV.Input)
													continue;

												item.Active = true;
												item.ActiveSpecified = true;

												Items.Add(item);
											}

											if (Items.Count > 0)
											{
												ItemResults = NewSubscription.ModifyItems((int)StateMask.Active, (Opc.Da.Item[])Items.ToArray(typeof(Opc.Da.Item)));

												foreach (ItemResult ItemResult in ItemResults)
												{
													if (ItemResult.ResultID != ResultID.S_OK)
														EventLog.WriteEntry("OPCServerManager AddItem Error : " + ItemResult.ItemName + " " + ItemResult.ResultID.ToString(), EventLogEntryType.Error);
												}
											}

											SubscriptionCollection[Index] = NewSubscription;
										}

											// Can't reconnect
										catch
										{
											ItemValueResult[] Values = new ItemValueResult[Subscription.Items.Length];

											if (PV.OPCQuality != new Quality(qualityBits.badNotConnected).GetCode())
											{
												int Count = 0;
												foreach (Item Item in Subscription.Items)
												{
													ItemValueResult Value = new ItemValueResult();
													Value.Timestamp = DateTime.UtcNow;
													Value.Quality = new Quality(qualityBits.badNotConnected);
													Value.ClientHandle = Item.ClientHandle;
													Values[Count] = Value;
													Count++;
												}

												OnDataChanged(null, null, Values);
											}
										}
									}
								}
								break;
							}

						default:
							break;
					}
				}

				catch (Exception e)
				{
					EventLog.WriteEntry("OPCServerManager OPCServerScan Error : " + e.Message, EventLogEntryType.Error);
				}
			}

			CancelSubscriptions();
		}

		public ItemValueResult[] Read(URL Url, Item[] Items)
		{
			if (Url == null
				|| Items == null)
				return null;

			Opc.Da.Server Server = new Opc.Da.Server(new OpcCom.Factory(), Url);
			NetworkCredential Credentials = null;
			Server.Connect(new ConnectData(Credentials));

			foreach (Item item in Items)
			{
				item.Active = false;
				item.ActiveSpecified = true;
			}

			ItemValueResult[] Values = Server.Read(Items);

			Server.Disconnect();
			Server.Dispose();

			return Values;
		}


		public void Read(ProcessVariableClass PV)
		{
			if (PV == null)
				return;

			Opc.Da.Server Server = new Opc.Da.Server(new OpcCom.Factory(), new URL(PV.URL));
			NetworkCredential Credentials = null;
			Server.Connect(new ConnectData(Credentials));

			Item[] Items = { new Item(new ItemIdentifier(PV.OPCItemID)) };
			Items[0].Active = false;
			Items[0].ActiveSpecified = true;

			ItemValueResult[] Values = Server.Read(Items);

			if (Values.Length == 1
				&& Values[0].ResultID != ResultID.S_OK)
			{
				Server.Disconnect();
				Server.Dispose();
				throw new OpcException("Read Error : " + Values[0].ItemName + " " + Values[0].ResultID.ToString());
			}

			Server.Disconnect();
			Server.Dispose();

			PV.ServerValue = Values[0].Value;
			PV.OPCQuality = Values[0].Quality.GetCode();
			PV.DateTimeStamp = Values[0].Timestamp;

		}

		public void Write(URL Url, ItemValue[] ItemValues)
		{
			if (Url == null
				|| ItemValues == null)
				return;

			Opc.Da.Server Server = new Opc.Da.Server(new OpcCom.Factory(), Url);
			NetworkCredential Credentials = null;
			Server.Connect(new ConnectData(Credentials));

			IdentifiedResult[] IdentifiedResults = Server.Write(ItemValues);

			foreach (IdentifiedResult identifiedResult in IdentifiedResults)
			{
				if (identifiedResult.ResultID != ResultID.S_OK)
				{
					Server.Disconnect();
					Server.Dispose();
					throw new OpcException("Write Error : " + identifiedResult.ItemName + " " + identifiedResult.ResultID.ToString());
				}
			}

			Server.Disconnect();
			Server.Dispose();
		}

		public void Write(ProcessVariableClass PV)
		{
			if (PV == null)
				return;

			Opc.Da.Server Server = new Opc.Da.Server(new OpcCom.Factory(), new URL(PV.URL));
			NetworkCredential Credentials = null;
			Server.Connect(new ConnectData(Credentials));
			ItemValue ItemValue = new ItemValue();
			ItemValue.ItemName = PV.OPCItemID;
			ItemValue.Value = PV.ServerValue;
			ItemValue[] ItemValues = { ItemValue };

			IdentifiedResult[] IdentifiedResults = Server.Write(ItemValues);

			if (IdentifiedResults.Length == 1
				&& IdentifiedResults[0].ResultID != ResultID.S_OK)
			{
				string ErrorText = "OPCServerManager Write Error : " + IdentifiedResults[0].ItemName + " " + IdentifiedResults[0].ResultID.ToString();

				Server.Disconnect();
				Server.Dispose();
				throw new OpcException(ErrorText);
			}
			else
				PV.OutputFailed = false;

			Server.Disconnect();
			Server.Dispose();
		}

		public void Update(bool ChangeOfState)
		{
			if (ChangeOfState)
				OPCServerCOSEvent.Set();
			else
				OPCServerUpdateEvent.Set();
		}

		static System.Type GetType(VarEnum input)
		{
			switch (input)
			{
				case VarEnum.VT_EMPTY: return null;
				case VarEnum.VT_I1: return typeof(sbyte);
				case VarEnum.VT_UI1: return typeof(byte);
				case VarEnum.VT_I2: return typeof(short);
				case VarEnum.VT_UI2: return typeof(ushort);
				case VarEnum.VT_I4: return typeof(int);
				case VarEnum.VT_UI4: return typeof(uint);
				case VarEnum.VT_I8: return typeof(long);
				case VarEnum.VT_UI8: return typeof(ulong);
				case VarEnum.VT_R4: return typeof(float);
				case VarEnum.VT_R8: return typeof(double);
				case VarEnum.VT_CY: return typeof(decimal);
				case VarEnum.VT_BOOL: return typeof(bool);
				case VarEnum.VT_DATE: return typeof(DateTime);
				case VarEnum.VT_BSTR: return typeof(string);
				case VarEnum.VT_ARRAY | VarEnum.VT_I1: return typeof(sbyte[]);
				case VarEnum.VT_ARRAY | VarEnum.VT_UI1: return typeof(byte[]);
				case VarEnum.VT_ARRAY | VarEnum.VT_I2: return typeof(short[]);
				case VarEnum.VT_ARRAY | VarEnum.VT_UI2: return typeof(ushort[]);
				case VarEnum.VT_ARRAY | VarEnum.VT_I4: return typeof(int[]);
				case VarEnum.VT_ARRAY | VarEnum.VT_UI4: return typeof(uint[]);
				case VarEnum.VT_ARRAY | VarEnum.VT_I8: return typeof(long[]);
				case VarEnum.VT_ARRAY | VarEnum.VT_UI8: return typeof(ulong[]);
				case VarEnum.VT_ARRAY | VarEnum.VT_R4: return typeof(float[]);
				case VarEnum.VT_ARRAY | VarEnum.VT_R8: return typeof(double[]);
				case VarEnum.VT_ARRAY | VarEnum.VT_CY: return typeof(decimal[]);
				case VarEnum.VT_ARRAY | VarEnum.VT_BOOL: return typeof(bool[]);
				case VarEnum.VT_ARRAY | VarEnum.VT_DATE: return typeof(DateTime[]);
				case VarEnum.VT_ARRAY | VarEnum.VT_BSTR: return typeof(string[]);
				case VarEnum.VT_ARRAY | VarEnum.VT_VARIANT: return typeof(object[]);
				default: return Opc.Type.ILLEGAL_TYPE;
			}
		}

		static VarEnum GetType(System.Type input)
		{
			if (input == null) return VarEnum.VT_EMPTY;
			if (input == typeof(sbyte)) return VarEnum.VT_I1;
			if (input == typeof(byte)) return VarEnum.VT_UI1;
			if (input == typeof(short)) return VarEnum.VT_I2;
			if (input == typeof(ushort)) return VarEnum.VT_UI2;
			if (input == typeof(int)) return VarEnum.VT_I4;
			if (input == typeof(uint)) return VarEnum.VT_UI4;
			if (input == typeof(long)) return VarEnum.VT_I8;
			if (input == typeof(ulong)) return VarEnum.VT_UI8;
			if (input == typeof(float)) return VarEnum.VT_R4;
			if (input == typeof(double)) return VarEnum.VT_R8;
			if (input == typeof(decimal)) return VarEnum.VT_CY;
			if (input == typeof(bool)) return VarEnum.VT_BOOL;
			if (input == typeof(DateTime)) return VarEnum.VT_DATE;
			if (input == typeof(string)) return VarEnum.VT_BSTR;
			if (input == typeof(object)) return VarEnum.VT_EMPTY;
			if (input == typeof(sbyte[])) return VarEnum.VT_ARRAY | VarEnum.VT_I1;
			if (input == typeof(byte[])) return VarEnum.VT_ARRAY | VarEnum.VT_UI1;
			if (input == typeof(short[])) return VarEnum.VT_ARRAY | VarEnum.VT_I2;
			if (input == typeof(ushort[])) return VarEnum.VT_ARRAY | VarEnum.VT_UI2;
			if (input == typeof(int[])) return VarEnum.VT_ARRAY | VarEnum.VT_I4;
			if (input == typeof(uint[])) return VarEnum.VT_ARRAY | VarEnum.VT_UI4;
			if (input == typeof(long[])) return VarEnum.VT_ARRAY | VarEnum.VT_I8;
			if (input == typeof(ulong[])) return VarEnum.VT_ARRAY | VarEnum.VT_UI8;
			if (input == typeof(float[])) return VarEnum.VT_ARRAY | VarEnum.VT_R4;
			if (input == typeof(double[])) return VarEnum.VT_ARRAY | VarEnum.VT_R8;
			if (input == typeof(decimal[])) return VarEnum.VT_ARRAY | VarEnum.VT_CY;
			if (input == typeof(bool[])) return VarEnum.VT_ARRAY | VarEnum.VT_BOOL;
			if (input == typeof(DateTime[])) return VarEnum.VT_ARRAY | VarEnum.VT_DATE;
			if (input == typeof(string[])) return VarEnum.VT_ARRAY | VarEnum.VT_BSTR;
			if (input == typeof(object[])) return VarEnum.VT_ARRAY | VarEnum.VT_VARIANT;

			// check for special types.
			if (input == Opc.Type.ILLEGAL_TYPE) return (VarEnum)Enum.ToObject(typeof(VarEnum), 0x7FFF);
			if (input == typeof(System.Type)) return VarEnum.VT_I2;
			if (input == typeof(Opc.Da.Quality)) return VarEnum.VT_I2;
			if (input == typeof(Opc.Da.accessRights)) return VarEnum.VT_I4;
			if (input == typeof(Opc.Da.euType)) return VarEnum.VT_I4;

			return VarEnum.VT_EMPTY;
		}



		public void AddProcessVariable(ProcessVariableClass PV)
		{
			Monitor.Enter(this);

			try
			{
				ProcessVariableCollectionClass PVCollection = null;
				int Count;

				for (Count = 0; Count < RemovePVCollectionCollection.Count; Count++)
				{
					PVCollection = RemovePVCollectionCollection.Item(Count);

					if (PVCollection[0].URL == PV.URL)
					{
						for (int Item = 0; Item < PVCollection.Count; Item++)
						{
							if (PVCollection[Item] == PV)
							{
								PVCollection.Remove(Item);
								if (PVCollection.Count == 0)
									RemovePVCollectionCollection.Remove(Count);

								return;
							}
						}
					}
				}

				for (Count = 0; Count < AddPVCollectionCollection.Count; Count++)
				{
					PVCollection = AddPVCollectionCollection.Item(Count);

					if (PVCollection[0].URL == PV.URL)
					{
						PVCollection.Add(PV);
						OPCServerItemEvent.Set();
						break;
					}
				}

				if (Count == AddPVCollectionCollection.Count)
				{
					PVCollection = new ProcessVariableCollectionClass();
					AddPVCollectionCollection.Add(PVCollection);
					PVCollection.Add(PV);
					OPCServerItemEvent.Set();
				}
			}
			finally
			{
				Monitor.Exit(this);
			}
		}

		public void RemoveProcessVariable(ProcessVariableClass PV)
		{
			Monitor.Enter(this);

			try
			{
				ProcessVariableCollectionClass PVCollection = null;
				int Count;

				for (Count = 0; Count < AddPVCollectionCollection.Count; Count++)
				{
					PVCollection = AddPVCollectionCollection.Item(Count);

					if (PVCollection[0].URL == PV.URL)
					{
						for (int Item = 0; Item < PVCollection.Count; Item++)
						{
							if (PVCollection[Item] == PV)
							{
								PVCollection.Remove(Item);
								if (PVCollection.Count == 0)
									AddPVCollectionCollection.Remove(Count);

								return;
							}
						}
					}
				}

				for (Count = 0; Count < RemovePVCollectionCollection.Count; Count++)
				{
					PVCollection = RemovePVCollectionCollection.Item(Count);

					if (PVCollection[0].URL == PV.URL)
					{
						PVCollection.Add(PV);
						OPCServerItemEvent.Set();
					}
				}

				if (Count == RemovePVCollectionCollection.Count)
				{
					PVCollection = new ProcessVariableCollectionClass();
					RemovePVCollectionCollection.Add(PVCollection);
					PVCollection.Add(PV);
					OPCServerItemEvent.Set();
				}
			}
			finally
			{
				Monitor.Exit(this);
			}
		}

		public void CancelSubscriptions()
		{
			Monitor.Enter(this);

			foreach (Opc.Da.Subscription Subscription in SubscriptionCollection)
			{
				try
				{
					Subscription.DataChanged -= new DataChangedEventHandler(OnDataChanged);
					IdentifiedResult[] IdentifiedResults = Subscription.RemoveItems(Subscription.Items);

					foreach (IdentifiedResult IdentifiedResult in IdentifiedResults)
						if (IdentifiedResult.ResultID != ResultID.S_OK)
							EventLog.WriteEntry("OPCServerManager RemoveItem Error : " + IdentifiedResult.ItemName + " " + IdentifiedResult.ResultID.ToString(), EventLogEntryType.Error);

					Opc.Da.Server Server = Subscription.Server;
					Server.CancelSubscription(Subscription);
					Server.Disconnect();
					Server.Dispose();
				}
				catch
				{
					EventLog.WriteEntry("OPCServerManager CancelSubscriptions Error", EventLogEntryType.Error);
				}
			}
			SubscriptionCollection.Clear();

			foreach (ProcessVariableCollectionClass PVCollection in AddPVCollectionCollection)
				PVCollection.Clear();

			AddPVCollectionCollection.Clear();

			foreach (ProcessVariableCollectionClass PVCollection in RemovePVCollectionCollection)
				PVCollection.Clear();

			RemovePVCollectionCollection.Clear();


			Monitor.Exit(this);
		}

		private void OnDataChanged(object subscriptionHandle, object requestHandle, ItemValueResult[] values)
		{
			try
			{
				foreach (ItemValueResult value in values)
				{
					ProcessVariableClass PV = (ProcessVariableClass)value.ClientHandle;

					PV.OPCQuality = value.Quality.GetCode();

					if (PV.IsQualityGood)
						PV.ServerValue = value.Value;

					PV.DateTimeStamp = value.Timestamp;
				}

				foreach (ItemValueResult value in values)
				{
					ProcessVariableClass PV = (ProcessVariableClass)value.ClientHandle;

					Invoke(PV);
				}
			}
			catch (Exception e)
			{
				EventLog.WriteEntry(e.ToString(), EventLogEntryType.Error);
			}
		}
	}
}
