namespace FMActiveDirectoryManageService
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Xml.Serialization;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;

    public class ReadApi
    {
        #region Data members
        private readonly EventLog FMEventLog;
        private readonly SecurityClass security;
        #endregion

        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public ReadApi(SecurityClass inSecurity, EventLog inFmEventLog)
        {
            this.security = inSecurity;
            this.FMEventLog = inFmEventLog;
        }
        #endregion

        #region Read Active Directory data methods
        /// <summary>
        /// This method will read the test file to be used to update users info.
        /// </summary>
        /// <returns></returns>
        public List<ActiveDirectoryUserDTO> ReadTestFile(string fileName)
        {
            try
            {
                List<ActiveDirectoryUserDTO> adUserCollection;

                using (Stream fileStream = File.Open(fileName, FileMode.Open, FileAccess.Read))
                {
                    var xmlSerializer = new XmlSerializer(typeof(List<ActiveDirectoryUserDTO>));
                    adUserCollection = xmlSerializer.Deserialize(fileStream) as List<ActiveDirectoryUserDTO>;
                    fileStream.Close();
                    fileStream.Dispose();
                }

                return adUserCollection;
            }
            catch (Exception ex)
            {
                const string ErrMsg = AdManageThread.MessagePrefixKey + " Could not read the test mode file. ";
                this.FMEventLog.WriteEntry(ErrMsg + ex.Message, EventLogEntryType.Error);
                return null;
            }
        }

        /// <summary>
        /// This method will read the Active Directory Service API for the user/site and
        /// user/user group collections.
        /// </summary>
        /// <returns>Returns a collection of users which contains site and user group collections.</returns>
        public List<ActiveDirectoryUserDTO> ReadActiveDirectoryApi()
        {
            try
            {
                var adUserCollection = FMChannelHelper.MakeCall<IActiveDirectoryService, List<ActiveDirectoryUserDTO>>(
                                                    x => x.GetUsersAndGroupAssociations(this.security));

                return adUserCollection;
            }
            catch (Exception ex)
            {
                const string ErrMsg = AdManageThread.MessagePrefixKey + " Could not read Active DirectoryService. ";
                this.FMEventLog.WriteEntry(ErrMsg + ex.Message, EventLogEntryType.Error);
                return null;
            }
        }

        /// <summary>
        /// This method is temporary, it is only used to create a default test file that contains the structure
        /// of the AD user site/user group for testing.
        /// </summary>
        public void WriteTempTestFile(string fileName)
        {
            var userCollection = new List<ActiveDirectoryUserDTO>();
            var siteList = new List<string> { "AZI", "CHAMAN", "NSPA" };
            var userList = new List<string> { "Accounting-Grp", "Administrator" };

            var adUserDo = new ActiveDirectoryUserDTO { UserName = "Testuser01", Sites = siteList, UserGroups = userList };
            userCollection.Add(adUserDo);

            adUserDo = new ActiveDirectoryUserDTO { UserName = "Testuser02", Sites = siteList, UserGroups = userList };
            userCollection.Add(adUserDo);

            using (Stream fileStream = File.Open(fileName, FileMode.Open, FileAccess.Write))
            {
                var xmlSerializer = new XmlSerializer(userCollection.GetType());
                xmlSerializer.Serialize(fileStream, userCollection);

                fileStream.Flush();
                fileStream.Close();
                fileStream.Dispose();
            }
        }
        #endregion
    }
}
