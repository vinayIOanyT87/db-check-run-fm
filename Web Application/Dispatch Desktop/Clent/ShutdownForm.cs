namespace Dispatch
{
	using System;

	public partial class ShutdownForm : FMBaseForm
	{
		private int countDown = 10;

		public string ErrorMessage
		{
			set
			{
				this.ErrorMessageTextBox.Text = value;
			}
		}

		public ShutdownForm()
		{
			this.InitializeComponent();
		}

		private void Timer1Tick(object sender, EventArgs e)
		{
			try
			{
				--this.countDown;
				this.ShowCountDownMessage();

				if (this.countDown == 0)
				{
					this.Close();
				}

			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void ShowCountDownMessage()
		{
			this.CountDownLabel.Text = string.Format("Application will exit in {0} second(s)", this.countDown);
		}

		private void ShutdownFormLoad(object sender, EventArgs e)
		{
			try
			{
				this.ShowCountDownMessage();
				this.timer1.Enabled = true;
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void ShutDownNowButtonClick(object sender, EventArgs e)
		{
			this.Close();
		}
	}
}
