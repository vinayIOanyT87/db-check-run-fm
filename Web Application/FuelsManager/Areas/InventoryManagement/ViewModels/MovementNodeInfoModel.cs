
namespace FuelsManager.Areas.InventoryManagement.ViewModels
{
    using System;

    [Serializable]
    public class MovementNodeInfoModel
    {
        #region Constructors
        public MovementNodeInfoModel()
        {
            this.Init();
        }
        #endregion

        #region Properties
        public Guid MovementNodeGuid { get; set; }
        public string TransferDirection { get; set; }
        public double? TransferTarget { get; set; }
        public string TransferMode { get; set; }
        public Guid IndividualNodeControlTagGuid { get; set; }
        public Guid ModeTagGuid { get; set; }
        public Guid StatusTagGuid { get; set; }
        public Guid TargetTagGuid { get; set; }
        public Guid MovementIDTagGuid { get; set; }
        public Guid QuantityMovedTagGuid { get; set; }
        public Guid MovementNodeSettingsGuid { get; set; }
        public string MovementPointId { get; set; }
        public Guid MovementPointGuid { get; set; }
        public string MovementNodeId { get; set; }
        public bool IndividualNodeControl { get; set; }
        #endregion

        #region Private methods
        /// <summary>
        /// This method initializes the object to its initial state.
        /// </summary>
        private void Init()
        {
            this.MovementNodeGuid               = Guid.Empty;
            this.TransferDirection              = string.Empty;
            this.TransferTarget                 = 0;
            this.TransferMode                   = string.Empty;
            this.IndividualNodeControlTagGuid   = Guid.Empty;
            this.ModeTagGuid                    = Guid.Empty;
            this.StatusTagGuid                  = Guid.Empty;
            this.TargetTagGuid                  = Guid.Empty;
            this.MovementIDTagGuid              = Guid.Empty;
            this.QuantityMovedTagGuid           = Guid.Empty;
            this.MovementNodeSettingsGuid       = Guid.Empty;
            this.MovementPointId                = string.Empty;
            this.MovementPointGuid              = Guid.Empty;
            this.MovementNodeId                 = string.Empty;
            this.IndividualNodeControl          = false;
        }
        #endregion
    }
}