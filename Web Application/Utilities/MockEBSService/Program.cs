using System;
using System.Text;
using System.Diagnostics;

using System.ServiceModel;
using System.ServiceModel.Description;
using System.Windows.Forms;

namespace MockEBSService
{
	class Program
	{


		//static Form1 newForm = null;
		
		//static void Main()
		//{
			
		//}
		[STAThread]
		static void Main(string[] args)
		{

			//newForm = new Form1();

			Application.EnableVisualStyles();
			Application.Run(new Form1()); 
			//newForm.FormClosed += new System.Windows.Forms.FormClosedEventHandler(newForm_FormClosed);
			
			//newForm.Show();

			//ResponseThread respThread = new ResponseThread();
			//ReceiveThread recThread = new ReceiveThread();

		}

		//static void newForm_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e)
		//{
		//   newForm = null;
		//}
	}
}
