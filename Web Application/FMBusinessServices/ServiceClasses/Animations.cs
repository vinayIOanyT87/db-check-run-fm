
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
	public class Animations : IAnimations
	{
		public ConsolidatedDAClass ConsolidatedDa = new ConsolidatedDAClass();

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, AnimationClass animation)
		{
			var animationList = new List<AnimationClass>();
			animationList.Add(animation);
			this.AddModifyAnimations(security, animationList, true, false);
			return animation.IdentityGuid;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, AnimationClass animation)
		{
			var animationList = new List<AnimationClass>();
			animationList.Add(animation);
			this.AddModifyAnimations(security, animationList, false, true);
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid animationGuid)
		{
			var animationGuidList = new List<Guid>();
			animationGuidList.Add(animationGuid);
			this.DeleteAnimations(security, animationGuidList);
		}

		public AnimationClass Get(SecurityClass security, Guid animationGuid)
		{
			var animationGuidList = new List<Guid>();
			animationGuidList.Add(animationGuid);
			var animationDictionary = this.EnumerateByAnimationGuids(security, animationGuidList);
			AnimationClass animation;
			if (animationDictionary.TryGetValue(animationGuid, out animation) == false)
			{
				return null;
			}
			return animation;
		}

		public Dictionary<Guid, AnimationClass> EnumerateByAnimationGuids(SecurityClass security, List<Guid> animationGuidList)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			//Add Security rights

			if (animationGuidList == null || animationGuidList.Count < 1)
			{
				return new Dictionary<Guid, AnimationClass>();
			}

			DataSet dataSet = null;

			using (SqlCommand cmd = new SqlCommand())
			{
				AnimationClass.EnumerateByAnimationGuidListSQL(cmd, animationGuidList);
				dataSet = this.ConsolidatedDa.GetDataSet(cmd, security);
			}

			var animationDictionary = new Dictionary<Guid, AnimationClass>();

			DataTable table = dataSet.Tables[0];

			foreach (DataRow row in table.Rows)
			{
				var animation = new AnimationClass();

				animation.AutoLoad(row);
				animationDictionary.Add(animation.IdentityGuid, animation);
			}
			return animationDictionary;
		}

		public Dictionary<Guid, AnimationClass> EnumerateAnimationsBySiteGuid(SecurityClass security, Guid siteGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			//Add Security rights

			DataSet dataSet = null;

			using (SqlCommand cmd = new SqlCommand())
			{
				AnimationClass.EnumerateBySiteGuidSQL(cmd, siteGuid);
				dataSet = this.ConsolidatedDa.GetDataSet(cmd, security);
			}

			var animationsBySiteGuid = new Dictionary<Guid, AnimationClass>();

			DataTable table = dataSet.Tables[0];

			foreach (DataRow row in table.Rows)
			{
				var animation = new AnimationClass();

				animation.AutoLoad(row);
				animationsBySiteGuid.Add(animation.IdentityGuid, animation);
			}
			return animationsBySiteGuid;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void AddModifyAnimations(SecurityClass security, List<AnimationClass> animationList, bool enableAdd, bool enableModify)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			//Add Security Rights

			if (animationList == null || animationList.Count < 1)
			{
				return;
			}

			using (SqlCommand cmd = new SqlCommand())
			{
				AnimationClass.AddModifyStoredProcedure(cmd, animationList, security, enableAdd, enableModify);
				this.ConsolidatedDa.ExecuteQuery(security, cmd);
			}
		}

		protected void DelAnimationToDrawingMaps(SecurityClass security, List<Guid> animationGuidList)
		{
			var mapAnimationsToDrawings = new AnimationDrawingMaps();
			mapAnimationsToDrawings.DeleteAnimationToDrawingMapsByAnimationGuidList(security, animationGuidList);
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void DeleteAnimations(SecurityClass security, List<Guid> animationGuidList)
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

			this.DelAnimationToDrawingMaps(security,animationGuidList);

			using (SqlCommand cmd = new SqlCommand())
			{
				AnimationClass.DeleteListSQL(cmd, animationGuidList);
				this.ConsolidatedDa.ExecuteQuery(security, cmd);
			}
		}

	}
}
