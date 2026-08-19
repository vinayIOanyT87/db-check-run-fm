namespace FuelsManager.Areas.InventoryManagement.ViewModels
{
    using System;

    [Serializable]
    public class DrawContext
    {
        public const string SessionKey = "DrawContextKey";

        public DrawModel Model { get; set; }

        public DrawContext()
        {
        }

        public DrawContext(DrawModel model)
        {
            this.Model = model;
        }
    }
}