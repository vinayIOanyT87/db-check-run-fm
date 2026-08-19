using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DispatchPrototype
{
	public partial class CommentForm : Form
	{
		public string CurrentComment = string.Empty;
		public string Forstring = string.Empty;
		public CommentForm()
		{
			InitializeComponent();
		}

		private void CancelCommentForm_Load(object sender, EventArgs e)
		{
			CancelCommentTextBox.Text = CurrentComment;
			Fortextbox.Text = Forstring;
			CancelCommentTextBox.Focus();
		}

		private void okbutton_clicked(object sender, EventArgs e)
		{
			CurrentComment = CancelCommentTextBox.Text;
			DialogResult = DialogResult.OK;
		}
	}
}
