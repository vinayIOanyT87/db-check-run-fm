namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.Security;
	using System.ServiceModel;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.UtilityObjects;

	using FMBusinessServices.DataAccessLayer;
	using FMBusinessServices.InternalClasses;

	using FMCore;

	[SecuritySafeCritical]
	[ServiceBehavior( TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted )]
	public class Drawings : FMServiceBase, IDrawings, IDependency
	{
		private readonly ConsolidatedDAClass consolidatedDA = new ConsolidatedDAClass();

		protected void AddAnimationToDrawingMaps(SecurityClass security, List<Guid> animationGuidList, Guid drawingGuid)
		{
			var mapAnimationsToDrawings = new AnimationDrawingMaps();
			if (animationGuidList != null && animationGuidList.Count > 0)
			{
				var animationToDrawingMapList = new List<AnimationToDrawingMapClass>();
				foreach (var animationGuid in animationGuidList)
				{
					var animationToDrawingMap = new AnimationToDrawingMapClass
					{
						AnimationToDrawingGuid = Guid.NewGuid(),
						AnimationGuid = animationGuid,
						CreatedBy = security.UserID,
						CreatedDate = DateTimeOffset.Now,
						UpdatedBy = security.UserID,
						UpdatedDate = DateTimeOffset.Now,
						DrawingGuid = drawingGuid
					};
					animationToDrawingMapList.Add(animationToDrawingMap);
				}


				mapAnimationsToDrawings.AddModifyAnimationToDrawingMap(security, animationToDrawingMapList, true, true, true);
			} else {
				mapAnimationsToDrawings.DeleteAnimationToDrawingMapsByDrawingGuidList(security, new List<Guid> { drawingGuid });
			}
		}

		protected void DelAnimationToDrawingMaps(SecurityClass security, Guid drawingGuid)
		{
			var mapAnimationsToDrawings = new AnimationDrawingMaps();
			var drawingGuidList = new List<Guid>();
			drawingGuidList.Add(drawingGuid);
			mapAnimationsToDrawings.DeleteAnimationToDrawingMapsByDrawingGuidList(security, drawingGuidList);
		}

		protected void DelAnimationToDrawingMapsBySite(SecurityClass security, Guid siteGuid)
		{
			var drawingNameList = this.EnumerateDrawingNamesBySiteGuid(security,siteGuid);
			var drawingGuidList = new List<Guid>();
			foreach (var drawingName in drawingNameList)
			{
				drawingGuidList.Add(drawingName.DrawingGuid);
			}
			var mapAnimationsToDrawings = new AnimationDrawingMaps();
			mapAnimationsToDrawings.DeleteAnimationToDrawingMapsByDrawingGuidList(security, drawingGuidList);
		}

		[OperationBehavior( TransactionScopeRequired = true, TransactionAutoComplete = true )]
		public Drawing Add( SecurityClass security, Drawing drawing )
		{
				security.ThrowIfNull("security");

				// TODO: Check security rights

				drawing.SetCreationStamp( security );

			using ( var cmd = new SqlCommand() )
			{
				drawing.SetCreationStamp( security );
				drawing.AutoGenerateInsertProcSQL( cmd, "gsp_DrawingsInsertByPK" );
				cmd.Parameters["@DrawingGuid"].Direction = ParameterDirection.InputOutput;

				this.consolidatedDA.ExecuteQuery( security, cmd );

				drawing.IdentityGuid = new Guid( cmd.Parameters["@DrawingGuid"].Value.ToString() );
			}
			this.AddAnimationToDrawingMaps(security, drawing.AnimationGuidList, drawing.DrawingGuid);
			return drawing;

		}

		[OperationBehavior( TransactionScopeRequired = true, TransactionAutoComplete = true )]
		public void Modify( SecurityClass security, Drawing drawing )
		{
				security.ThrowIfNull("security");

				// TODO: Check security rights

				// Check to see if we need to do a saveas
				var existing = this.Get(security, drawing.DrawingGuid);
			if (existing == null)
			{
				throw new Exception("Existing drawing not found.");
			}

			if (existing.ID.NotEquals(drawing.ID, StringComparison.InvariantCultureIgnoreCase))
			{
				// the names do not match so save as new name
				drawing.DrawingGuid = Guid.Empty;
				this.Add(security, drawing);
				return;
			}

			drawing.SetCreationStamp( security );

			using ( var cmd = new SqlCommand() )
			{
				drawing.SetModifyStamp( security );
				drawing.AutoGenerateModifyProcSQL( cmd, "gsp_DrawingsUpdateByPK" );

				this.consolidatedDA.ExecuteQuery( security, cmd );
			}
			this.AddAnimationToDrawingMaps(security,drawing.AnimationGuidList,drawing.DrawingGuid);
		}

		[OperationBehavior( TransactionScopeRequired = true, TransactionAutoComplete = true )]
		public void Purge( SecurityClass security, Guid drawingGuid )
		{
				security.ThrowIfNull("security");

				// TODO: Check security rights

				var drawing = this.Get(security, drawingGuid);
			if ( drawing.IdentityGuid == Guid.Empty )
			{
				throw new Exception( "Drawing not found." );
			}

			this.DelAnimationToDrawingMaps(security, drawing.DrawingGuid);

			// Delete point
			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = "usp_DrawingsDeleteByRowGuid";
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@DrawingGuid", drawingGuid);
				try
				{
					this.consolidatedDA.ExecuteQuery(security, cmd);
				}
				catch (Exception error)
				{
					if (drawing.PanelType == PANELTYPE.Detail &&
						error.Message == "Database error")
					{
						throw new Exception("Drawing in use. Cannot be Deleted");
					}
					throw new Exception(error.Message);
				}
			}
		}


		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void PurgeBySite(SecurityClass security, Guid siteGuid)
		{
			security.ThrowIfNull("security");

			// TODO: Check security rights

			this.DelAnimationToDrawingMapsBySite(security, siteGuid);

			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = "DELETE FROM tblDrawings WHERE SiteGuid = @SiteGuid";
				cmd.CommandType = CommandType.Text;
				cmd.Parameters.AddWithValue("@SiteGuid", siteGuid);
				this.consolidatedDA.ExecuteQuery(security, cmd);
			}
		}


		public Drawing Get(SecurityClass security, Guid drawingGuid)
		{
				security.ThrowIfNull("security");

				// TODO: Check security rights

				DataSet set;
			var drawing = new Drawing() { DrawingGuid = drawingGuid };

			using (var cmd = new SqlCommand())
			{
				drawing.SelectSQL(cmd, drawingGuid);
				set = this.consolidatedDA.GetDataSet(cmd, security);
			}

			DataTable table = set.Tables[0];
			drawing = new Drawing();
			if (table.Rows.Count > 0)
			{
				drawing.AutoLoad(table.Rows[0]);
			}

			return drawing;
		}

		public Guid GetIdentityGuid(SecurityClass security, string id)
		{
			security.ThrowIfNull("security");

			// TODO: Check security rights

			DataSet set;
			var drawingGuid = Guid.Empty;

			using (var cmd = new SqlCommand())
			{
				Drawing.SelectByIdSQL(cmd, security.SiteGuid, id);
				set = this.consolidatedDA.GetDataSet(cmd, security);
			}

			if(set.Tables.Count == 1
			&& set.Tables[0].Rows.Count == 1)
			{
				DataTable table = set.Tables[0];
				drawingGuid = (Guid)table.Rows[0]["DrawingGuid"];
			}

			return drawingGuid;
		}

		public List<DrawingName> EnumerateAvailableDrawingNames(SecurityClass security)
		{
			security.ThrowIfNull("security");

			// TODO: Check security rights

			DataSet set;

			using (var cmd = new SqlCommand())
			{
				DrawingName.EnumerateAvailableDrawingNames(cmd, security,security.SiteGuid);
				set = this.consolidatedDA.GetDataSet(cmd, security);
			}

			DataTable table = set.Tables[0];
			var names = new List<DrawingName>();

			foreach (DataRow row in table.Rows)
			{
				var drawingName = new DrawingName();
				BaseDataObject.AutoLoad(drawingName, row);

				names.Add(drawingName);
			}

			return names;
		}

		public List<DrawingName> EnumerateDrawingNamesBySiteGuid(SecurityClass security, Guid siteGuid)
		{
			security.ThrowIfNull("security");

			// TODO: Check security rights

			DataSet set;

			using (var cmd = new SqlCommand())
			{
				DrawingName.EnumerateAvailableDrawingNames(cmd, security, siteGuid);
				set = this.consolidatedDA.GetDataSet(cmd, security);
			}

			DataTable table = set.Tables[0];
			var names = new List<DrawingName>();

			foreach (DataRow row in table.Rows)
			{
				var drawingName = new DrawingName();
				BaseDataObject.AutoLoad(drawingName, row);

				names.Add(drawingName);
			}

			return names;
		}

		public Dictionary<Guid,DrawingName> EnumerateByDrawingGuids(SecurityClass security, List<Guid> drawingGuidList )
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			//Add Security rights

			if (drawingGuidList == null || drawingGuidList.Count < 1)
			{
				return new Dictionary<Guid, DrawingName>();
			}

			DataSet dataSet = null;
			using (SqlCommand cmd = new SqlCommand())
			{
				DrawingName.EnumerateByDrawingGuidListSQL(cmd, drawingGuidList);
				dataSet = this.consolidatedDA.GetDataSet(cmd, security);
			}

			var drawingNameDictionary = new Dictionary<Guid, DrawingName>();

			DataTable table = dataSet.Tables[0];

			foreach (DataRow row in table.Rows)
			{
				var drawingName = new DrawingName();

				BaseDataObject.AutoLoad(drawingName,row);
				drawingNameDictionary.Add(drawingName.DrawingGuid, drawingName);
			}

			return drawingNameDictionary;
		}

		public List<DrawingName> EnumerateAvailableDrawingNamesByPanelType(SecurityClass security, List<PANELTYPE> panelTypes)
		{
				security.ThrowIfNull("security");

				// TODO: Check security rights

				DataSet set;

				using (var cmd = new SqlCommand())
				{
					DrawingName.EnumerateAvailableDrawingNamesByPanelType(cmd, security, panelTypes);
					set = this.consolidatedDA.GetDataSet(cmd, security);
				}

				DataTable table = set.Tables[0];
				var names = new List<DrawingName>();

				foreach (DataRow row in table.Rows)
				{
					var drawingName = new DrawingName();
					BaseDataObject.AutoLoad(drawingName, row);

					names.Add(drawingName);
				}

				return names;

		}

		public List<DrawingName> EnumerateAvailableDrawingNamesByPointTemplate(
			SecurityClass security,
			Guid pointTemplateGuid)
		{
				security.ThrowIfNull("security");

				// TODO: Check security rights

				DataSet set;

				using (var cmd = new SqlCommand())
				{
					DrawingName.EnumerateAvailableDrawingNamesByPointTemplate(cmd, security, pointTemplateGuid);
					set = this.consolidatedDA.GetDataSet(cmd, security);
				}

				DataTable table = set.Tables[0];
				var names = new List<DrawingName>();
				var blankDrawingName = new DrawingName
				{
					ID = "<None>",
					DrawingGuid = Guid.Empty
				};
				names.Add(blankDrawingName);
				foreach (DataRow row in table.Rows)
				{
					var drawingName = new DrawingName();
					BaseDataObject.AutoLoad(drawingName, row);

					names.Add(drawingName);
				}
				names.Sort((x, y) => x.ID.CompareTo(y.ID));

				return names;
		}

		// list all drawings for a point template across all sites
		public List<DrawingName> EnumerateAllAvailableDrawingNamesByPointTemplate(
			SecurityClass security,
			Guid pointTemplateGuid)
		{
			security.ThrowIfNull("security");

			// TODO: Check security rights

			DataSet set;

			using (var cmd = new SqlCommand())
			{
				DrawingName.EnumerateAllAvailableDrawingNamesByPointTemplate(cmd, security, pointTemplateGuid);
				set = this.consolidatedDA.GetDataSet(cmd, security);
			}

			DataTable table = set.Tables[0];
			var names = new List<DrawingName>();
			foreach (DataRow row in table.Rows)
			{
				var drawingName = new DrawingName();
				BaseDataObject.AutoLoad(drawingName, row);

				names.Add(drawingName);
			}
			return names;
		}

		public List<DrawingName> EnumerateAvailableDrawingNamesByPublished(SecurityClass security)
		{
			security.ThrowIfNull("security");

			// TODO: Check security rights

			DataSet set;

			using (var cmd = new SqlCommand())
			{
				DrawingName.EnumerateAvailableDrawingNamesByPublished(cmd, security);
				set = this.consolidatedDA.GetDataSet(cmd, security);
			}

			DataTable table = set.Tables[0];
			var names = new List<DrawingName>();

			foreach (DataRow row in table.Rows)
			{
				var drawingName = new DrawingName();
				BaseDataObject.AutoLoad(drawingName, row);

				names.Add(drawingName);
			}

			return names;

		}


		void IDependency.Insert(SecurityClass security, BaseDataObject Object, bool preOperation)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			if (Object == null)
			{
				throw new ArgumentNullException("Object");
			}
		}

		void IDependency.Purge(SecurityClass security, BaseDataObject Object)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			if (Object == null)
			{
				throw new ArgumentNullException("Object");
			}

			if (Object is SiteClass)
			{
				var site = (SiteClass)Object;
				this.PurgeBySite(security, site.IdentityGuid);
			}
		}

		void IDependency.Update(SecurityClass security, BaseDataObject Object)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			if (Object == null)
			{
				throw new ArgumentNullException("Object");
			}
		}
	}
}