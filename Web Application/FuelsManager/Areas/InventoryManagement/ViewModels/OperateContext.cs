namespace FuelsManager.Areas.InventoryManagement.ViewModels
{
	using System;

	[Serializable]
	public class OperateContext
	{
		public const string SessionKey = "OperateContextKey";

		public OperateModel Model { get; set; }

		public OperateContext()
		{
		}

		public OperateContext( OperateModel model )
		{
			this.Model = model;
		}
	}
}
