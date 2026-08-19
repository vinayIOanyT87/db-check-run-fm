namespace BusinessObjects.DataObjects
{
    using System;

    public class PersonnelSupervisorDo
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public PersonnelSupervisorDo()
        {
            this.Init();
        }
        #endregion

        #region Properties
        public int SupervisorIndex { get; set; }
        public Guid SupervisorGuid { get; set; }
        public string SupervisorId { get; set; }
        #endregion

        #region Private methods
        /// <summary>
        /// This method initializes the object to its initial state.
        /// </summary>
        private void Init()
        {
            this.SupervisorGuid = Guid.Empty;
            this.SupervisorId = string.Empty;
            this.SupervisorIndex = 0;
        }
        #endregion
    }
}
