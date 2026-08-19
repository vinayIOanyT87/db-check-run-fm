namespace FMPointService.OpcClient
{
	using System.Collections.Generic;
	using System.Linq;
	using Softing.Opc.Ua.Client;
	using System;

	public class SubscriptionDictionary
	{
		//SiteGuid/OpcUaServerEndPoint/PublishingInterval/Subscription Dictionary
		protected Dictionary<Guid, Dictionary<Guid, Dictionary<int, ClientSubscription>>> subscriptionDictionary = new Dictionary<Guid, Dictionary<Guid, Dictionary<int, ClientSubscription>>>();

		public void Clear()
		{
				this.subscriptionDictionary.Clear();
		}

		public ClientSubscription GetSubscription(Guid siteGuid, Guid opcUaServerGuid, int? publishingInterval)
		{
			if (siteGuid == Guid.Empty)
			{
				return null;
			}

			if (opcUaServerGuid == Guid.Empty)
			{
				return null;
			}

			int pubInterval = 0;
			if (publishingInterval != null)
			{
				pubInterval = (int) publishingInterval;
			}

			Dictionary<Guid, Dictionary<int, ClientSubscription>> siteSubscriptionDictionary;
			if (this.subscriptionDictionary.TryGetValue(siteGuid, out siteSubscriptionDictionary))
			{
				Dictionary<int, ClientSubscription> subsetSubcriptionDicationary;
				if (siteSubscriptionDictionary.TryGetValue(opcUaServerGuid, out subsetSubcriptionDicationary))
				{
					ClientSubscription subscription;
					if (subsetSubcriptionDicationary.TryGetValue(pubInterval, out subscription))
					{
							return subscription;
					}
				}
			}
			return null;
		}

		public List<ClientSession> GetSessionList()
		{
			var ret = new List<ClientSession>();

			foreach (var siteSubscriptionDictionary in this.subscriptionDictionary.Values)
			{
				foreach (var subsetSubcriptionDicationary in siteSubscriptionDictionary)
				{
					if (subsetSubcriptionDicationary.Value.Count > 0)
					{
							ret.Add(subsetSubcriptionDicationary.Value.ElementAt(0).Value.Session);
					}
				}
			}
			return ret;
		}

		public ClientSession GetSession(Guid siteGuid, Guid opcUaServerGuid)
		{
			Dictionary<Guid, Dictionary<int, ClientSubscription>> siteSubscriptionDictionary;
			if (this.subscriptionDictionary.TryGetValue(siteGuid, out siteSubscriptionDictionary))
			{
				Dictionary<int, ClientSubscription> subsetSubcriptionDictionary;
				if (siteSubscriptionDictionary.TryGetValue(opcUaServerGuid, out subsetSubcriptionDictionary))
				{

					if (subsetSubcriptionDictionary.Count > 0)
					{
						return subsetSubcriptionDictionary.ElementAt(0).Value.Session;
					}
				}
			}
			return null;
		}

		public void AddSubscription(Guid siteGuid, Guid opcUaServerGuid, int? publishingInterval, ClientSubscription subscription)
		{
			if (siteGuid == Guid.Empty)
			{
				return;
			}

			if (opcUaServerGuid == Guid.Empty)
			{
				return;
			}

			if (subscription == null)
			{
				return;
			}

			int pubInterval = 0;
			if (publishingInterval != null)
			{
				pubInterval = (int)publishingInterval;
			}
			Dictionary<Guid, Dictionary<int, ClientSubscription>> siteSubscriptionDictionary;
			Dictionary<int, ClientSubscription> subsetSubcriptionDicationary;
			if (!this.subscriptionDictionary.TryGetValue(siteGuid, out siteSubscriptionDictionary))
			{
				siteSubscriptionDictionary = new Dictionary<Guid, Dictionary<int, ClientSubscription>>();
				this.subscriptionDictionary.Add(siteGuid, siteSubscriptionDictionary);
				subsetSubcriptionDicationary = new Dictionary<int, ClientSubscription>();
				siteSubscriptionDictionary.Add(opcUaServerGuid, subsetSubcriptionDicationary);
				subsetSubcriptionDicationary.Add(pubInterval, subscription);
			}
			else
			{
				if (!siteSubscriptionDictionary.TryGetValue(opcUaServerGuid, out subsetSubcriptionDicationary))
				{
					subsetSubcriptionDicationary = new Dictionary<int, ClientSubscription>();
					siteSubscriptionDictionary.Add(opcUaServerGuid, subsetSubcriptionDicationary);
					subsetSubcriptionDicationary.Add(pubInterval, subscription);
				}
				else
				{
					ClientSubscription subscriptionref;
					if (!subsetSubcriptionDicationary.TryGetValue(pubInterval, out subscriptionref))
					{
							subsetSubcriptionDicationary.Add(pubInterval, subscription);
					}
				}
			}
		}

		public void CleanupSubscription(Guid siteGuid, Guid opcUaServerGuid, int? publishingInterval)
		{
			if (siteGuid == Guid.Empty)
			{
				return;
			}

			if (opcUaServerGuid == Guid.Empty)
			{
				return;
			}

			int pubInterval = 0;
			if (publishingInterval != null)
			{
				pubInterval = (int)publishingInterval;
			}

			Dictionary<Guid, Dictionary<int, ClientSubscription>> siteSubscriptionDictionary;
			if (this.subscriptionDictionary.TryGetValue(siteGuid, out siteSubscriptionDictionary))
			{
				Dictionary<int, ClientSubscription> subsetSubcriptionDictionary;
				if (siteSubscriptionDictionary.TryGetValue(opcUaServerGuid, out subsetSubcriptionDictionary))
				{
					ClientSubscription subscription;
					if (subsetSubcriptionDictionary.TryGetValue(pubInterval, out subscription))
					{
						if (subscription.MonitoredItems.Count <= 0)
						{
							subsetSubcriptionDictionary.Remove(pubInterval);
							if (subsetSubcriptionDictionary.Count <= 0)
							{
								siteSubscriptionDictionary.Remove(opcUaServerGuid);
								if (siteSubscriptionDictionary.Count <= 0)
								{
									this.subscriptionDictionary.Remove(siteGuid);
								}
							}
							return;
						}
					}
				}
			}

			return;
		}
	}
}
