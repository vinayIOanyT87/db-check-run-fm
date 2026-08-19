namespace Dispatch
{
	using System;
	using System.Windows.Forms;

	public partial class CommentForm : Form
	{
		public string CurrentComment = string.Empty;
		public string Forstring = string.Empty;
		public CommentForm()
		{
			this.InitializeComponent();
		}

		private void CancelCommentFormLoad(object sender, EventArgs e)
		{
			this.CancelCommentTextBox.Text = this.CurrentComment;
			this.Fortextbox.Text = this.Forstring;
			this.CancelCommentTextBox.Focus();
		}

		private void OkbuttonClicked(object sender, EventArgs e)
		{
			this.CurrentComment = this.CancelCommentTextBox.Text;
			this.DialogResult = DialogResult.OK;
		}
	}
}
