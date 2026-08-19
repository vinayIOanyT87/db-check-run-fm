namespace GenerateRightScripts
{
    public class RightsClass
    {
        #region
        public string RightDescription { get; set; }
        public string RightCode { get; set; }
        public int RightIndex { get; set; }

        public string RightIndexStr
        {
            get { return this.RightIndex.ToString(); }
            set
            {
                int newIndex;
                this.RightIndex = 0;

                if (int.TryParse(value, out newIndex))
                {
                    this.RightIndex = newIndex;
                }
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// This is the default constructor
        /// </summary>
        public RightsClass()
        {
            this.Init();
        }
        #endregion

        #region Private methods
        /// <summary>
        /// This method will initialize the object to its initial state.
        /// </summary>
        private void Init()
        {
            this.RightIndex = 0;
            this.RightCode = string.Empty;
            this.RightDescription = string.Empty;
        }
        #endregion
    }
}
