using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.Constants;
using FMBusinessObjects.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Web;

namespace FMBusinessServices.ServiceClasses
{	

	[SecuritySafeCriticalAttribute]
	public class QueryWriterTopics : IQueryWriterTopics
	{
		public const string QUERYWRITER_SETTING = "QueryWriterAssemblies";
		private static QueryWriterTopicCollection cachedTypeCollection = null;

		public QueryWriterTopicCollection Enumerate(SecurityClass Security)
		{
			CheckSecurity(Security);

			if (cachedTypeCollection == null)
			{
				LoadCache(Security);
			}

			QueryWriterTopicCollection queryTypeCollection = new QueryWriterTopicCollection();

			foreach (QueryWriterTopic queryWriterTopic in cachedTypeCollection)
			{
				QueryWriterTopicSecurityCollection securityCollection = queryWriterTopic.GetSecurityRights();

				if (securityCollection.HasRights(Security))
				{
					queryTypeCollection.Add(queryWriterTopic);
				}
			}

			return queryTypeCollection;
		}

		public QueryWriterTopic Get(SecurityClass Security, string objectType)
		{
			
			var topics = Enumerate(Security);
			

			var Topic = (from T in topics
					   where T.ObjectType.ToString() == objectType
					   select T)
				    .DefaultIfEmpty(null)
				    .FirstOrDefault();


			//the below section is used to load old queries that were not migrated in the xml to use the new client class
			if (Topic == null)
			{
				try
				{
					var classToCheckType = Type.GetType(objectType);

					if (classToCheckType != null)
					{
						
						Topic = (from T in topics
							    where T.ObjectType.IsSubclassOf(classToCheckType)
							    select T)
				    .DefaultIfEmpty(null)
				    .FirstOrDefault();
					}
				}
				catch
				{
					Topic = null;
				}
			}

			return Topic as QueryWriterTopic;

		}

		private void LoadCache(SecurityClass Security)
		{
			cachedTypeCollection = new QueryWriterTopicCollection();

			try
			{
				string assemblyPath = FMChannelHelper.MakeCall<IConfigurationSettings, string>(x => x.GetKeyValueByKey(Security, QUERYWRITER_SETTING));

				// Parse the list of assemblies
				char[] separator = { ';' };
				string[] sssemList = assemblyPath.Split(separator, StringSplitOptions.RemoveEmptyEntries);


				// Go through all the assemblies
				string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
				foreach (string assemblyName in sssemList)
				{
					try
					{
						Assembly dll = null;

						if (!AssemblyDictionary.ContainsKey(assemblyName.ToLower()))
						{
							try
							{
								dll = Assembly.LoadFrom(baseDirectory + "\\bin\\" + assemblyName);
							}
							catch
							{
								try
								{
									dll = Assembly.Load(assemblyName);
								}
								catch (Exception ex)
								{
									string message = "Assembly Load Error in Query Writer Topics Load Cache. " + ex.Message;
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
						{
							continue;
						}

						Type[] types;

						try
						{
							types = dll.GetTypes();

							foreach (Type Module in types)
							{
								GetQueryTypes(Module, cachedTypeCollection);
							}
						}
						catch
						{
							continue;
						}

					}
					catch
					{

					}
				}



				//var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(s => s.GetName().GetPublicKeyToken().SequenceEqual(Assembly.GetExecutingAssembly().GetName().GetPublicKeyToken()));
				//foreach (Assembly assem in assemblies)
				//{
				//	foreach (Type Module in assem.GetTypes())
				//	{
				//		GetQueryTypes(Module, cachedTypeCollection);
				//	}
				//}
			}
			catch (ReflectionTypeLoadException reflectionException)
			{
				throw new ApplicationException(BuildLoadExceptionMessage(reflectionException));
			}
		}

		private void GetQueryTypes(Type Module, QueryWriterTopicCollection queryTypeCollection)
		{
			foreach (QueryWriterTopic queryWriterTopic in Module.GetCustomAttributes(typeof(QueryWriterTopic), true))
			{
				if (queryTypeCollection.Count(x => queryWriterTopic.ObjectType.IsAssignableFrom(x.ObjectType)) > 0)
				{
					continue; //don't want parent classes added since there is a subclass already added that overrides the parent class
				}

				queryTypeCollection.RemoveAll(x => queryWriterTopic.ObjectType.IsSubclassOf(x.ObjectType)); //remove parent class since this one will override

				queryTypeCollection.Add(queryWriterTopic);
			}
		}

		private string BuildLoadExceptionMessage(ReflectionTypeLoadException reflectionException)
		{
			if (reflectionException == null)
			{
				throw new ArgumentNullException();
			}

			string Message = reflectionException.Message;

			foreach (Exception except in reflectionException.LoaderExceptions)
			{
				Message += "\n" + "===========" + "\n" + except.Message;
			}

			return Message;

		}

		private void CheckSecurity(SecurityClass Security)
		{
		}
	}
}