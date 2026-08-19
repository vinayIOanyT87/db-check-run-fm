namespace MigrationTool
{
    using System.Windows.Forms;

    public partial class MigrationToolForm : Form
    {
        private bool readDbConfiguration;
        private const string MigrationVersionNone = "None";
        private const int MigrationVersionNoneIndex = 0;
        private const string MigrationVersion753ToFmV12 = "Version 7.5.3 To FM v12";
        private const int MigrationVersion753ToFmV12Index = 1;

        public MigrationToolForm()
        {
            InitializeComponent();

            // Initialize Database Connection Configuration tab.
            this.InitializeDbConnectionTab();

            // Initialize Personnel Tool tab.
            this.InitializePersonnelToolTab();
        }
    }
}
