namespace FMBusinessObjects.DataObjects
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class ActiveDirectoryUserDTO
    {
        public string UserName { get; set; }
        public List<string> Sites { get; set; }
        public List<string> UserGroups { get; set; }

        public ActiveDirectoryUserDTO()
        {
            this.UserName = string.Empty;
            this.Sites = new List<string>();
            this.UserGroups = new List<string>();
        }
    }
}
