namespace Accounting
{
	public class ExSTARSView: AccountingWebFormView 
	{
		#region Constructor
		public ExSTARSView()
		{
		}
		#endregion

		#region Standard Methods
		protected override void OnInit( System.EventArgs e )
		{
			InitializeComponent();
			base.OnInit( e );
		}

		private void InitializeComponent()
		{
			this.Load += new System.EventHandler( ExSTARSView_Load );
		}

		private void ExSTARSView_Load( object sender, System.EventArgs e )
		{	
		}
		#endregion
	}
}
