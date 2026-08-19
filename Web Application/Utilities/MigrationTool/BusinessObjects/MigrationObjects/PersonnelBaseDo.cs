namespace BusinessObjects.MigrationObjects
{
    using System;
    using System.Data;
    using System.Data.SqlClient;

    public class PersonnelBaseDo : MigrationBaseDo
    {
        #region Constructors
        /// <summary>
        /// This is the initializer constructor
        /// </summary>
        public PersonnelBaseDo(string sourceDbName, string targetDbName)
        {
            this.SourceDbName = sourceDbName;
            this.TargetDbName = targetDbName;
            this.Init();
        }

        /// <summary>
        /// This is the default constructor
        /// </summary>
        public PersonnelBaseDo()
        {
            this.Init();
        }
        #endregion

        #region Properties
        public int? PersonIndex { get; set; }
        public int? SiteIndex { get; set; }
        public string PersonId { get; set; }
        public string CardNumber { get; set; }
        public int? UserIndex { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
        public string Department { get; set; }
        public int? SupervisorIndex { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public DateTime? AssignmentDate { get; set; }
        public DateTime? SupervisionDate { get; set; }
        public string SSAN { get; set; }
        public DateTime? BirthDate { get; set; }
        public decimal? PayRate { get; set; }
        public double? LaborRate1 { get; set; }
        public double? LaborRate2 { get; set; }
        public double? LaborRate3 { get; set; }
        public double? LaborRate4 { get; set; }
        public short? Status { get; set; }
        public string Email { get; set; }
        public bool ResponsibleOfficer { get; set; }
        public short? Shift { get; set; }
        public string PinNumber { get; set; }
        public bool PinRequired { get; set; }
        public bool LockedOut { get; set; }
        public string LockedOutReason { get; set; }
        public DateTime? LockedOutDate { get; set; }
        public DateTime? LastActivityDate { get; set; }
        public bool CardedIn { get; set; }
        public string ShortCardNumber { get; set; }
        public int? AssignedEquipmentIndex { get; set; }
        public byte[] OnFileSignature { get; set; }
        public bool InhibitInactivityLockout { get; set; }
        public string UserData1 { get; set; }
        public string UserData2 { get; set; }
        public string UserData3 { get; set; }
        public string UserData4 { get; set; }
        public string UserData5 { get; set; }
        public string UserData6 { get; set; }
        public string UserData7 { get; set; }
        public string UserData8 { get; set; }
        public string UserData9 { get; set; }
        public string UserData10 { get; set; }
        public string UserData11 { get; set; }
        public string UserData12 { get; set; }
        public string UserData13 { get; set; }
        public string UserData14 { get; set; }
        public string UserData15 { get; set; }
        public string UserData16 { get; set; }
        public string UserData17 { get; set; }
        public string UserData18 { get; set; }
        public string UserData19 { get; set; }
        public string UserData20 { get; set; }
        public string UserData21 { get; set; }
        public string UserData22 { get; set; }
        public string UserData23 { get; set; }
        public string UserData24 { get; set; }
        public string EquipmentId { get; set; }

        public string SourceDbName { get; set; }
        public string TargetDbName { get; set; }
        #endregion

        #region public methods
        /// <summary>
        /// This method creates the sql command string to retrieve the data.
        /// </summary>
        /// <param name="command">SQL command object.</param>
        public virtual void EnumeratePersonnelSql(SqlCommand command, int siteIndex)
        {
            if (string.IsNullOrEmpty(this.SourceDbName))
            {
                return;
            }

            string select = " SELECT P.*"
                            + " , S.ID AS SiteID"
                            + " , E.ID AS EquipmentID";
            string from = " FROM " + this.SourceDbName + ".dbo.tblPersonnel P INNER JOIN "
                            + this.SourceDbName + ".dbo.tblSites S ON P.SiteIndex = S.SiteIndex "
                            + " LEFT JOIN " + this.SourceDbName + ".dbo.tblEquipment E ON E.EquipmentIndex = P.AssignedEquipmentIndex";

            string where = " WHERE P.SiteIndex = " + siteIndex;


            command.CommandText = select + from + where;
        }

        /// <summary>
        /// This method will load one row.
        /// </summary>
        /// <param name="row">The row to be loaded.</param>
        public virtual void Load(DataRow row)
        {
            this.PersonIndex                = row.IsNull("PersonIndex") ? null : (int?)row["PersonIndex"];
            this.SiteIndex                  = row.IsNull("SiteIndex") ? null : (int?)row["SiteIndex"];
            this.PersonId                   = row.IsNull("PersonID") ? string.Empty : (string)row["PersonID"];
            this.CardNumber                 = row.IsNull("CardNumber") ? string.Empty : (string)row["CardNumber"];
            this.UserIndex                  = row.IsNull("UserIndex") ? null : (int?)row["UserIndex"];
            this.FirstName                  = row.IsNull("FirstName") ? string.Empty : (string)row["FirstName"];
            this.MiddleName                 = row.IsNull("MiddleName") ? string.Empty : (string)row["MiddleName"];
            this.LastName                   = row.IsNull("LastName") ? string.Empty : (string)row["LastName"];
            this.Title                      = row.IsNull("Title") ? string.Empty : (string)row["Title"];
            this.Department                 = row.IsNull("Department") ? string.Empty : (string)row["Department"];
            this.SupervisorIndex            = row.IsNull("SupervisorIndex") ? null : (int?)row["SupervisorIndex"];
            this.Address1                   = row.IsNull("Address1") ? string.Empty : (string)row["Address1"];
            this.Address2                   = row.IsNull("Address2") ? string.Empty : (string)row["Address2"];
            this.City                       = row.IsNull("City") ? string.Empty : (string)row["City"];
            this.State                      = row.IsNull("State") ? string.Empty : (string)row["State"];
            this.Zip                        = row.IsNull("Zip") ? string.Empty : (string)row["Zip"];
            this.Country                    = row.IsNull("Country") ? string.Empty : (string)row["Country"];
            this.Phone1                     = row.IsNull("Phone1") ? string.Empty : (string)row["Phone1"];
            this.Phone2                     = row.IsNull("Phone2") ? string.Empty : (string)row["Phone2"];
            this.AssignmentDate             = row.IsNull("AssignmentDate") ? null : (DateTime?)row["AssignmentDate"];
            this.SupervisionDate            = row.IsNull("SupervisionDate") ? null : (DateTime?)row["SupervisionDate"];
            this.SSAN                       = row.IsNull("SSAN") ? string.Empty : (string)row["SSAN"];
            this.BirthDate                  = row.IsNull("BirthDate") ? null : (DateTime?)row["BirthDate"];
            this.PayRate                    = row.IsNull("PayRate") ? null : (decimal?)row["PayRate"];
            this.LaborRate1                 = row.IsNull("LaborRate1") ? null : (double?)row["LaborRate1"];
            this.LaborRate2                 = row.IsNull("LaborRate2") ? null : (double?)row["LaborRate2"];
            this.LaborRate3                 = row.IsNull("LaborRate3") ? null : (double?)row["LaborRate3"];
            this.LaborRate4                 = row.IsNull("LaborRate4") ? null : (double?)row["LaborRate4"];
            this.Status                     = row.IsNull("Status") ? null : (short?)row["Status"];
            this.Email                      = row.IsNull("Email") ? string.Empty : (string)row["Email"];
            this.ResponsibleOfficer         = row.IsNull("ResponsibleOfficer") ? false : (bool)row["ResponsibleOfficer"];
            this.Shift                      = row.IsNull("Shift") ? null : (short?)row["Shift"];
            this.PinNumber                  = row.IsNull("PinNumber") ? string.Empty : (string)row["PinNumber"];
            this.PinRequired                = row.IsNull("PinRequired") ? false : (bool)row["PinRequired"];
            this.LockedOut                  = row.IsNull("LockedOut") ? false : (bool)row["LockedOut"];
            this.LockedOutReason            = row.IsNull("LockedOutReason") ? string.Empty : (string)row["LockedOutReason"];
            this.LockedOutDate              = row.IsNull("LockedOutDate") ? null : (DateTime?)row["LockedOutDate"];
            this.LastActivityDate           = row.IsNull("LastActivityDate") ? null : (DateTime?)row["LastActivityDate"];
            this.CardedIn                   = row.IsNull("CardedIn") ? false : (bool)row["CardedIn"];
            this.ShortCardNumber            = row.IsNull("ShortCardNumber") ? string.Empty : (string)row["ShortCardNumber"];
            this.AssignedEquipmentIndex     = row.IsNull("AssignedEquipmentIndex") ? null : (int?)row["AssignedEquipmentIndex"];
            this.OnFileSignature            = row.IsNull("OnFileSignature") ? null : (byte[])row["OnFileSignature"];
            this.InhibitInactivityLockout   = row.IsNull("InhibitInactivityLockout") ? false : (bool)row["InhibitInactivityLockout"];
            this.UserData1                  = row.IsNull("UserData1") ? string.Empty : (string)row["UserData1"];
            this.UserData2                  = row.IsNull("UserData2") ? string.Empty : (string)row["UserData2"];
            this.UserData3                  = row.IsNull("UserData3") ? string.Empty : (string)row["UserData3"];
            this.UserData4                  = row.IsNull("UserData4") ? string.Empty : (string)row["UserData4"];
            this.UserData5                  = row.IsNull("UserData5") ? string.Empty : (string)row["UserData5"];
            this.UserData6                  = row.IsNull("UserData6") ? string.Empty : (string)row["UserData6"];
            this.UserData7                  = row.IsNull("UserData7") ? string.Empty : (string)row["UserData7"];
            this.UserData8                  = row.IsNull("UserData8") ? string.Empty : (string)row["UserData8"];
            this.UserData9                  = row.IsNull("UserData9") ? string.Empty : (string)row["UserData9"];
            this.UserData10                 = row.IsNull("UserData10") ? string.Empty : (string)row["UserData10"];
            this.UserData11                 = row.IsNull("UserData11") ? string.Empty : (string)row["UserData11"];
            this.UserData12                 = row.IsNull("UserData12") ? string.Empty : (string)row["UserData12"];
            this.UserData13                 = row.IsNull("UserData13") ? string.Empty : (string)row["UserData13"];
            this.UserData14                 = row.IsNull("UserData14") ? string.Empty : (string)row["UserData14"];
            this.UserData15                 = row.IsNull("UserData15") ? string.Empty : (string)row["UserData15"];
            this.UserData16                 = row.IsNull("UserData16") ? string.Empty : (string)row["UserData16"];
            this.UserData17                 = row.IsNull("UserData17") ? string.Empty : (string)row["UserData17"];
            this.UserData18                 = row.IsNull("UserData18") ? string.Empty : (string)row["UserData18"];
            this.UserData19                 = row.IsNull("UserData19") ? string.Empty : (string)row["UserData19"];
            this.UserData20                 = row.IsNull("UserData20") ? string.Empty : (string)row["UserData20"];
            this.UserData21                 = row.IsNull("UserData21") ? string.Empty : (string)row["UserData21"];
            this.UserData22                 = row.IsNull("UserData22") ? string.Empty : (string)row["UserData22"];
            this.UserData23                 = row.IsNull("UserData23") ? string.Empty : (string)row["UserData23"];
            this.UserData24                 = row.IsNull("UserData24") ? string.Empty : (string)row["UserData24"];
            this.EquipmentId                = row.IsNull("EquipmentID") ? string.Empty : (string)row["EquipmentID"];
        }
        #endregion

        #region Protected methods
        /// <summary>
        /// This method initializes the object to its initial state.
        /// </summary>
        protected void Init()
        {
            this.PersonIndex                = null;
            this.SiteIndex                  = null;
            this.PersonId                   = string.Empty;
            this.CardNumber                 = string.Empty;
            this.UserIndex                  = null;
            this.FirstName                  = string.Empty;
            this.MiddleName                 = string.Empty;
            this.LastName                   = string.Empty;
            this.Title                      = string.Empty;
            this.Department                 = string.Empty;
            this.SupervisorIndex            = null;
            this.Address1                   = string.Empty;
            this.Address2                   = string.Empty;
            this.City                       = string.Empty;
            this.State                      = string.Empty;
            this.Zip                        = string.Empty;
            this.Country                    = string.Empty;
            this.Phone1                     = string.Empty;
            this.Phone2                     = string.Empty;
            this.AssignmentDate             = null;
            this.SupervisionDate            = null;
            this.SSAN                       = string.Empty;
            this.BirthDate                  = null;
            this.PayRate                    = null;
            this.LaborRate1                 = null;
            this.LaborRate2                 = null;
            this.LaborRate3                 = null;
            this.LaborRate4                 = null;
            this.Status                     = null;
            this.Email                      = string.Empty;
            this.ResponsibleOfficer         = false;
            this.Shift                      = null;
            this.PinNumber                  = string.Empty;
            this.PinRequired                = false;
            this.LockedOut                  = false;
            this.LockedOutReason            = string.Empty;
            this.LockedOutDate              = null;
            this.LastActivityDate           = null;
            this.CardedIn                   = false;
            this.ShortCardNumber            = string.Empty;
            this.AssignedEquipmentIndex     = null;
            this.OnFileSignature            = null;
            this.InhibitInactivityLockout   = false;
            this.UserData1                  = string.Empty;
            this.UserData2                  = string.Empty;
            this.UserData3                  = string.Empty;
            this.UserData4                  = string.Empty;
            this.UserData5                  = string.Empty;
            this.UserData6                  = string.Empty;
            this.UserData7                  = string.Empty;
            this.UserData8                  = string.Empty;
            this.UserData9                  = string.Empty;
            this.UserData10                 = string.Empty;
            this.UserData11                 = string.Empty;
            this.UserData12                 = string.Empty;
            this.UserData13                 = string.Empty;
            this.UserData14                 = string.Empty;
            this.UserData15                 = string.Empty;
            this.UserData16                 = string.Empty;
            this.UserData17                 = string.Empty;
            this.UserData18                 = string.Empty;
            this.UserData19                 = string.Empty;
            this.UserData20                 = string.Empty;
            this.UserData21                 = string.Empty;
            this.UserData22                 = string.Empty;
            this.UserData23                 = string.Empty;
            this.UserData24                 = string.Empty;
            this.EquipmentId                = string.Empty;
        }
        #endregion
    }
}
