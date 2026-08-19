namespace FuelsManager.Areas.InventoryManagement.Controllers
{
	using System;
	using System.Collections.Generic;
	using System.Web.Mvc;
	using FuelsManager.Areas.Controllers;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.BusinessInterfaces;
	using FuelsManager.Areas.InventoryManagement.ViewModels;

	public class MovementDisabledByController : FMBaseControllerEx
	{
		#region Data members
		const string ErrorMsgPrefix = "MovementDisabledByView: ";
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public MovementDisabledByController()
		{
		}

		#endregion

		#region Public static methods
		#endregion

		#region Public methods

		/// <summary>
		/// This method retrieves the movement user data editor model based on the movement point GUID.
		/// </summary>
		/// <param name="movementPointGuid">The movement point GUID</param>
		/// <returns>Returns the Movement User Data Editor model.</returns>
		[HttpGet]
		public ActionResult MovementDisabledBy(string movementPointId, Guid movementPointGuid)
		{
			try
			{
				var model = this.GetMovementDisabledByModel(movementPointId, movementPointGuid);
				return base.PartialViewWithErrorMessages("MovementDisabledBy", model, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				string msgBasic = this.GetTranslatedText("Error Getting Movement Disabled By.");
				string msgEventLog = ErrorMsgPrefix + msgBasic + " " + ex.Message;
				FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(msgEventLog, FMBusinessObjects.Constants.FMEventLogEntryType.Error));

				base.OnError(new Exception(msgBasic));
				return base.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
		}


		#endregion


		#region Private methods

		/// <summary>
		/// This method will retrieve the movement disabled by data based on the movement point Guid.
		/// </summary>
		/// <param name="movementPointGuid">The movement point Guid used to retrieved the data.</param>
		/// <returns>Return a movement start data editor model.</returns>
		private MovementDisabledByModel GetMovementDisabledByModel(string movementPointId, Guid movementPointGuid)
		{
			var interlockedActiveModementList = new List<string>();

			if (movementPointGuid != null
			&& movementPointGuid != Guid.Empty)
			{
				interlockedActiveModementList = FMChannelHelper.MakeCall<IMovementService,List<string>>(x => x.CheckForActiveInterlockedMovements(this.Security, movementPointGuid));
			}

			var movementDisabledByModel = new MovementDisabledByModel()
			{
				MovementPointId = movementPointId,
				MovementPointGuid = movementPointGuid,
				InterlockedActiveMovementList = interlockedActiveModementList
			};



			return movementDisabledByModel;
		}

		#endregion



	}
	}