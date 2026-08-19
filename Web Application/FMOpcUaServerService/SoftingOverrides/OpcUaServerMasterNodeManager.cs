// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OpcUaServerMasterNodeManager.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FMOpcUaServerService
{
	using System.Collections.Generic;
	using Opc.Ua;
	using Opc.Ua.Server;


	public class OpcUaServerMasterNodeManager : MasterNodeManager
	{
		public OpcUaServerMasterNodeManager(IServerInternal server, ApplicationConfiguration configuration, string dynamicNamespaceUri, params INodeManager[] additionalManagers)
			: base(server, configuration, dynamicNamespaceUri, additionalManagers)
		{
        }


		public override void Browse(
			OperationContext context,
			ViewDescription view,
			uint maxReferencesPerNode,
			BrowseDescriptionCollection nodesToBrowse,
			out BrowseResultCollection results,
			out DiagnosticInfoCollection diagnosticInfos)
		{
            results = new BrowseResultCollection();
			diagnosticInfos = new DiagnosticInfoCollection();

			int index = 0;

			foreach (var node in nodesToBrowse)
			{
				BrowseResultCollection intermediateResults = null;
				DiagnosticInfoCollection intermediateDiagnosticInfos = null;

				if (node.NodeId.NamespaceIndex < 2)
				{
					base.Browse(context, view, maxReferencesPerNode, new BrowseDescriptionCollection() { node }, out intermediateResults, out intermediateDiagnosticInfos);
					if (intermediateResults != null)
					{
						foreach (var browseResult in intermediateResults)
						{
							results.Add(browseResult);
						}
					}

					if (intermediateDiagnosticInfos != null)
					{
						foreach (var diagnosticInfo in intermediateDiagnosticInfos)
						{
							diagnosticInfos.Add(diagnosticInfo);
						}
					}

				}

				if (node.NodeId.NamespaceIndex == 2
				|| node.NodeId == ObjectIds.ObjectsFolder)
				{
					if (results.Count <= index)
					{
						results.Add(new BrowseResult());
					}

					var continuationPoint = new ContinuationPoint()
					                        {
						                        NodeToBrowse = node,
						                        BrowseDirection = node.BrowseDirection,
						                        Manager = this.CoreNodeManager,
						                        MaxResultsToReturn = (uint)(maxReferencesPerNode - results[index].References.Count),
						                        NodeClassMask = node.NodeClassMask,
						                        ReferenceTypeId = node.ReferenceTypeId,
						                        Index = 0
					                        };

					// Add refereces to ObjectIds.ObjectsFolder based upon access
					this.NodeManagers[2].Browse(context, ref continuationPoint, results[index].References);
				}

				index++;
			}
		}
	}
}