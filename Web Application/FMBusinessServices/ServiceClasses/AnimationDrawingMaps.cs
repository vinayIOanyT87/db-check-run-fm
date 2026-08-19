
namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.Linq;
	using System.ServiceModel;
	using System.Text;
	using System.Threading.Tasks;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;

	using FMBusinessServices.DataAccessLayer;


	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class AnimationDrawingMaps : IAnimationDrawingMaps
	{
		public ConsolidatedDAClass ConsolidatedDa = new ConsolidatedDAClass();

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, AnimationToDrawingMapClass animationToDrawing)
		{
			var animationToDrawingList = new List<AnimationToDrawingMapClass>();
			animationToDrawingList.Add(animationToDrawing);
			this.AddModifyAnimationToDrawingMap(security, animationToDrawingList, true, false, false);
			return animationToDrawing.IdentityGuid;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, AnimationToDrawingMapClass animationToDrawing)
		{
			var animationToDrawingList = new List<AnimationToDrawingMapClass>();
			animationToDrawingList.Add(animationToDrawing);
			this.AddModifyAnimationToDrawingMap(security, animationToDrawingList, false, true, false);
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid animationToDrawingGuid)
		{
			var animationGuidList = new List<Guid>();
			animationGuidList.Add(animationToDrawingGuid);
			this.DeleteAnimationToDrawingMaps(security, animationGuidList);
		}

		public AnimationToDrawingMapClass Get(SecurityClass security, Guid animationToDrawingGuid)
		{
			var animationToDrawingGuidList = new List<Guid>();
			animationToDrawingGuidList.Add(animationToDrawingGuid);
			var animationDictionary = this.EnumerateByAnimationToDrawingGuids(security, animationToDrawingGuidList);
			AnimationToDrawingMapClass animation;
			if (animationDictionary.TryGetValue(animationToDrawingGuid, out animation) == false)
			{
				return null;
			}
			return animation;
		}

		public Dictionary<Guid, AnimationToDrawingMapClass> EnumerateByAnimationToDrawingGuids(SecurityClass security, List<Guid> animationToDrawingGuidList)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			//Add Security rights

			if (animationToDrawingGuidList == null || animationToDrawingGuidList.Count < 1)
			{
				return new Dictionary<Guid, AnimationToDrawingMapClass>();
			}

			DataSet dataSet = null;

			using (SqlCommand cmd = new SqlCommand())
			{
				AnimationToDrawingMapClass.EnumerateByAnimationToDrawingGuidListSQL(cmd, animationToDrawingGuidList);
				dataSet = this.ConsolidatedDa.GetDataSet(cmd, security);
			}

			var animationToDrawingDictionary = new Dictionary<Guid, AnimationToDrawingMapClass>();

			DataTable table = dataSet.Tables[0];

			foreach (DataRow row in table.Rows)
			{
				var animation = new AnimationToDrawingMapClass();

				animation.AutoLoad(row);
				animationToDrawingDictionary.Add(animation.IdentityGuid, animation);
			}
			return animationToDrawingDictionary;
		}

		public Dictionary<Guid, AnimationToDrawingMapClass> EnumerateByAnimationGuids(SecurityClass security, List<Guid> animationGuidList)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			//Add Security rights

			if (animationGuidList == null || animationGuidList.Count < 1)
			{
				return new Dictionary<Guid, AnimationToDrawingMapClass>();
			}

			DataSet dataSet = null;

			using (SqlCommand cmd = new SqlCommand())
			{
				AnimationToDrawingMapClass.EnumerateByAnimationGuidListSQL(cmd, animationGuidList);
				dataSet = this.ConsolidatedDa.GetDataSet(cmd, security);
			}

			var animationToDrawingDictionary = new Dictionary<Guid, AnimationToDrawingMapClass>();

			DataTable table = dataSet.Tables[0];

			foreach (DataRow row in table.Rows)
			{
				var animation = new AnimationToDrawingMapClass();

				animation.AutoLoad(row);
				animationToDrawingDictionary.Add(animation.IdentityGuid, animation);
			}
			return animationToDrawingDictionary;
		}

		public Dictionary<Guid, AnimationToDrawingMapClass> EnumerateByDrawingGuids(SecurityClass security, List<Guid> drawingGuidList)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			//Add Security rights

			if (drawingGuidList == null || drawingGuidList.Count < 1)
			{
				return new Dictionary<Guid, AnimationToDrawingMapClass>();
			}

			DataSet dataSet = null;

			using (SqlCommand cmd = new SqlCommand())
			{
				AnimationToDrawingMapClass.EnumerateByDrawingGuidListSQL(cmd, drawingGuidList);
				dataSet = this.ConsolidatedDa.GetDataSet(cmd, security);
			}

			var animationToDrawingDictionary = new Dictionary<Guid, AnimationToDrawingMapClass>();

			DataTable table = dataSet.Tables[0];

			foreach (DataRow row in table.Rows)
			{
				var animation = new AnimationToDrawingMapClass();

				animation.AutoLoad(row);
				animationToDrawingDictionary.Add(animation.IdentityGuid, animation);
			}
			return animationToDrawingDictionary;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void AddModifyAnimationToDrawingMap(SecurityClass security, List<AnimationToDrawingMapClass> animationToDrawingList, bool enableAdd, bool enableModify, bool enableDelete)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			//Add Security Rights

			if (animationToDrawingList == null || animationToDrawingList.Count < 1)
			{
				return;
			}

			using (SqlCommand cmd = new SqlCommand())
			{
				AnimationToDrawingMapClass.AddModifyStoredProcedure(cmd, animationToDrawingList, security, enableAdd, enableModify, enableDelete);
				this.ConsolidatedDa.ExecuteQuery(security, cmd);
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void DeleteAnimationToDrawingMaps(SecurityClass security, List<Guid> animationToDrawingGuidList)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			//Add Security Rights

			if (animationToDrawingGuidList == null || animationToDrawingGuidList.Count < 1)
			{
				return;
			}

			using (SqlCommand cmd = new SqlCommand())
			{
				AnimationToDrawingMapClass.DeleteListSQL(cmd, animationToDrawingGuidList);
				this.ConsolidatedDa.ExecuteQuery(security, cmd);
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void DeleteAnimationToDrawingMapsByDrawingGuidList(SecurityClass security, List<Guid> drawingGuidList)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			//Add Security Rights

			if (drawingGuidList == null || drawingGuidList.Count < 1)
			{
				return;
			}

			using (SqlCommand cmd = new SqlCommand())
			{
				AnimationToDrawingMapClass.DeleteByDrawingListSQL(cmd, drawingGuidList);
				this.ConsolidatedDa.ExecuteQuery(security, cmd);
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void DeleteAnimationToDrawingMapsByAnimationGuidList(SecurityClass security, List<Guid> animationGuidList)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			//Add Security Rights

			if (animationGuidList == null || animationGuidList.Count < 1)
			{
				return;
			}

			using (SqlCommand cmd = new SqlCommand())
			{
				AnimationToDrawingMapClass.DeleteByAnimationListSQL(cmd, animationGuidList);
				this.ConsolidatedDa.ExecuteQuery(security, cmd);
			}
		}
	}
}
