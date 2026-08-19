namespace FuelsManager.Areas.InventoryManagement.ViewModels
{
    using System;

    [Serializable]
    public class MovementUserDataEditorModel
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public MovementUserDataEditorModel()
        {
            this.Init();
        }
        #endregion

        #region Properties
        public Guid MovementPointGuid { get; set; }
        public string UserData01 { get; set; }
        public string UserData02 { get; set; }
        public string UserData03 { get; set; }
        public string UserData04 { get; set; }
        public string UserData05 { get; set; }
        public string UserData06 { get; set; }
        public string UserData07 { get; set; }
        public string UserData08 { get; set; }
        public string UserData09 { get; set; }
        public string UserData10 { get; set; }
        public string PointId { get; set; }
        public string PointPropertyId { get; set; }
        #endregion

        #region Private methods
        /// <summary>
        /// This method initializes the object to its initial state.
        /// </summary>
        private void Init()
        {
            this.MovementPointGuid = Guid.Empty;

            this.UserData01         = string.Empty;
            this.UserData02         = string.Empty;
            this.UserData03         = string.Empty;
            this.UserData04         = string.Empty;
            this.UserData05         = string.Empty;
            this.UserData06         = string.Empty;
            this.UserData07         = string.Empty;
            this.UserData08         = string.Empty;
            this.UserData09         = string.Empty;
            this.UserData10         = string.Empty;
            this.PointId            = string.Empty;
            this.PointPropertyId    = string.Empty;
        }
        #endregion
    }
}