using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.IO;
using System.Configuration;

using BsmeInterfaceLibrary.EBS.IDocs;

namespace MockEBSService
{
	public partial class Form1 : Form
	{

		ResponseThread respThread = new ResponseThread();
		ReceiveThread recThread = new ReceiveThread();

		bool respRunning = false;
		bool recRunning = false;

		public Form1()
		{
			InitializeComponent();
		}

		private void rboSuccess_CheckedChanged(object sender, EventArgs e)
		{
			grpFailure.Visible = false;
			grpSuccess.Visible = true;
		}

		private void rboFailure_CheckedChanged(object sender, EventArgs e)
		{
			grpFailure.Visible = true;
			grpSuccess.Visible = false;

		}

		private void btnCreate_Click(object sender, EventArgs e)
		{

			if (rboSuccess.Checked)
			{
				CreateIDoc(true, txtSuccessTransID.Text, txtMilStrip.Text, txtSAPDoc.Text);
			}
			else
			{
				CreateIDoc(false, txtFailTransID.Text, txtParam.Text, txtMessage.Text);
			}

			//ZSV_FMD_ACK ret = new ZSV_FMD_ACK();
			//ZSV_FMD_ACKIDOCZ1SV_FMD_ACK content = new ZSV_FMD_ACKIDOCZ1SV_FMD_ACK();
			//ret.IDOC = new ZSV_FMD_ACKIDOC();
			//ret.IDOC.Z1SV_FMD_ACK = content;

			//content.PROCESS_DT = DateTime.Now.ToString("MM/dd/yyyy");

			//if (rboSuccess.Checked)
			//{
			//   content.RECORD_ID = txtSuccessTransID.Text;
			//   content.MILSTRIP_DOC_NBR = txtMilStrip.Text;
			//   content.SAP_DOCUMENT_NBR =  txtSAPDoc.Text;
			//   content.MESSAGE_TEXT = "success";
			//}
			//else
			//{
			//   content.RECORD_ID = txtFailTransID.Text;
			//   content.MESSAGE_TEXT = txtMessage.Text;
			//   content.PARAMETER_NAME = txtParam.Text;
			//}

			//string dirPath = ConfigurationManager.AppSettings["sendpath"];
			//string fileName = string.Format("ToSend_{0}_{1}.xml", DateTime.Now.ToString("yyyyddMM-hhmmss"), Guid.NewGuid());
			//string filePath = System.IO.Path.Combine(dirPath, fileName);

			//using (StreamWriter outfile = new StreamWriter(filePath, true))
			//{
			//   outfile.Write(ToXML(ret));
			//}

			//Console.WriteLine("created a new ack to send when fmd connects: " + filePath);
		}


		protected static string ToXML(object objToSerialize)
		{
			XmlSerializer serializer = null;
			//FileStream stream = null;
			try
			{
				StringBuilder sb = new StringBuilder();
				StringWriter output = new StringWriter(sb);
				output.NewLine = String.Empty;
				serializer = new XmlSerializer(objToSerialize.GetType());
				serializer.Serialize(output, objToSerialize);
				return output.ToString();
			}
			catch
			{
				return "";
			}
			finally
			{

			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			if (!recRunning)
			{
				Console.WriteLine("starting receive services.");
				recThread = new ReceiveThread();
				recThread.Start();
				recRunning = true;
				lblRecStatus.BackColor = Color.Green;
				lblRecStatus.Text = "Running...";
				button1.Text = "Stop Receive";
				
			}
			else
			{
				Console.WriteLine("stopping receive services.");
				recThread.Stop();
				recRunning = false;
				lblRecStatus.BackColor = Color.Red;
				button1.Text = "Start Receive";
				lblRecStatus.Text = "Stopped...";
				Console.WriteLine("Receieve services stopped.");

			}
		}


		private void button2_Click(object sender, EventArgs e)
		{
			if (!respRunning)
			{
				Console.WriteLine("starting response services.");
				respThread = new ResponseThread();
				respThread.Start();
				respRunning = true;
				lblRespStatus.BackColor = Color.Green;
				lblRespStatus.Text = "Running...";
				button2.Text = "Stop Response";

			}
			else
			{
				Console.WriteLine("stopping response services.");
				respThread.Stop();
				respRunning = false;
				lblRespStatus.BackColor = Color.Red;
				button2.Text = "Start Response";
				lblRespStatus.Text = "Stopped...";
				Console.WriteLine("Response services stopped.");

			}
		}

		private void Form1_Load(object sender, EventArgs e)
		{

		}

		private void Form1_FormClosed(object sender, FormClosedEventArgs e)
		{
			if (respRunning)
				respThread.Stop();

			if (recRunning)
				recThread.Stop();
		}


		private void dataGridView1_KeyUp(object sender, KeyEventArgs e)
		{
			if (tabControl1.SelectedIndex == 2)
			{
				//if user clicked Shift+Ins or Ctrl+V (paste from clipboard)
				if ((e.Shift && e.KeyCode == Keys.Insert) || (e.Control && e.KeyCode == Keys.V))
				{
					dataGridView1.Rows.Clear();
					char[] rowSplitter = { '\r', '\n' };
					char[] columnSplitter = { '\t' };
					//get the text from clipboard
					IDataObject dataInClipboard = Clipboard.GetDataObject();
					string stringInClipboard = (string)dataInClipboard.GetData(DataFormats.Text);
					if (stringInClipboard != null)
					{
						//split it into lines
						string[] rowsInClipboard = stringInClipboard.Split(rowSplitter, StringSplitOptions.RemoveEmptyEntries);
						//get the row and column of selected cell in grid
						int r = 0;
						//int c = 0;
						//add rows into grid to fit clipboard lines
						if (dataGridView1.Rows.Count < (r + rowsInClipboard.Length) + 1)
						{
							dataGridView1.Rows.Add(1 + r + rowsInClipboard.Length - dataGridView1.Rows.Count);
						}
						// loop through the lines, split them into cells and place the values in the corresponding cell.
						for (int iRow = 0; iRow < rowsInClipboard.Length; iRow++)
						{
							//split row into cell values
							string[] valuesInRow = rowsInClipboard[iRow].Split(columnSplitter);

							if (valuesInRow.Length < 4)
							{
								dataGridView1.Rows.Clear();
								MessageBox.Show("The data being pasted does not fit the grid.  It should have four columns(TransID, MilStip, SAPDOC, Message)");
								return;
							}

							dataGridView1.Rows[r + iRow].Cells[1].Value = valuesInRow[0].Trim(); //transid

							if (valuesInRow[1].Trim() != string.Empty && valuesInRow[2].Trim() != string.Empty)
							{
								dataGridView1.Rows[r + iRow].Cells[0].Value = "Success";
								dataGridView1.Rows[r + iRow].Cells[2].Value = valuesInRow[1].Trim(); // milstrip
								dataGridView1.Rows[r + iRow].Cells[3].Value = valuesInRow[2].Trim(); //sap doc num
							}
							else
							{
								dataGridView1.Rows[r + iRow].Cells[0].Value = "Failure";
								dataGridView1.Rows[r + iRow].Cells[2].Value = valuesInRow[1].Trim(); //milstrip
								dataGridView1.Rows[r + iRow].Cells[4].Value = valuesInRow[3].Trim(); //message
							}

							////cycle through cell values
							//for (int iCol = 0; iCol < valuesInRow.Length; iCol++)
							//{
							//   //assign cell value, only if it within columns of the grid
							//   if (dataGridView1.ColumnCount - 1 >= c + iCol)
							//   {
							//      dataGridView1.Rows[r + iRow].Cells[c + iCol].Value = valuesInRow[iCol];
							//   }
							//}
						}

						btnCreateMulti.Enabled = true;
					}
				}
			}
		}

		private void btnCreateMulti_Click(object sender, EventArgs e)
		{
			while (dataGridView1.Rows.Count > 1)
			{
				DataGridViewRow dr = dataGridView1.Rows[dataGridView1.Rows.GetFirstRow(DataGridViewElementStates.None)];

				if (dr.Cells[0].Value.ToString() == "Success")
				{
					CreateIDoc(true, dr.Cells[1].Value.ToString(), dr.Cells[2].Value.ToString(), dr.Cells[3].Value.ToString());
				}
				else
				{
					CreateIDoc(false, dr.Cells[1].Value.ToString(), dr.Cells[2].Value.ToString(), dr.Cells[4].Value.ToString());
				}

				dataGridView1.Rows.Remove(dr);
				dataGridView1.Update();
			}
			btnCreateMulti.Enabled = false;
		}

		private void CreateIDoc(bool isSuccess, string record_id, string field1, string field2)
		{
			ZSV_FMD_ACK ret = new ZSV_FMD_ACK();
			ZSV_FMD_ACKIDOCZ1SV_FMD_ACK content = new ZSV_FMD_ACKIDOCZ1SV_FMD_ACK();
			ret.IDOC = new ZSV_FMD_ACKIDOC();
			ret.IDOC.Z1SV_FMD_ACK = content;
            ret.IDOC.EDI_DC40 = new ZSV_FMD_ACKIDOCEDI_DC40();
			ret.IDOC.EDI_DC40.SNDPOR = "";
			ret.IDOC.EDI_DC40.SNDPRN = "";
			ret.IDOC.EDI_DC40.SNDPRT = "";
			ret.IDOC.EDI_DC40.RCVPOR = "";
			ret.IDOC.EDI_DC40.RCVPRN = "";

			content.PROCESS_DT = DateTime.Now.ToString("MMddyyyy");

            if (field1 == "") field1 = null;
            if (field2 == "") field2 = null;

			if (isSuccess)
			{
                content.PARAMETER_NAME = "53"; //success
				content.RECORD_ID = record_id;
				content.MILSTRIP_DOC_NBR = field1;
				content.SAP_DOCUMENT_NBR = field2;
				content.MESSAGE_TEXT = "success";
			}
			else
			{
				content.RECORD_ID = record_id;
				content.MESSAGE_TEXT = field2;


                content.MILSTRIP_DOC_NBR = field1;
			}

			string dirPath = ConfigurationManager.AppSettings["sendpath"];
			string fileName = string.Format("ToSend_{0}_{1}.xml", DateTime.Now.ToString("yyyyddMM-hhmmss"), Guid.NewGuid());
			string filePath = System.IO.Path.Combine(dirPath, fileName);

			using (StreamWriter outfile = new StreamWriter(filePath, true))
			{
				outfile.Write(ToXML(ret));
			}

			Console.WriteLine("created a new ack to send when fmd connects: " + filePath);
		}

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

	}
}
