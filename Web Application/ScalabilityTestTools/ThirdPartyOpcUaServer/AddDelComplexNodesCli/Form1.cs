using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AddDelComplexNodesCli
{
	using System.Configuration;

	using FMBusinessObjects.DataObjects;

	using FMUAAlarmPluginInterface;

	public partial class Form1 : Form
	{
		protected AddDelAlarmsCli comms = new AddDelAlarmsCli(ConfigurationManager.AppSettings["NodMgrSrvAddr"]);

		protected MasterDynaicEntityFactory mdef = new MasterDynaicEntityFactory(Environment.CurrentDirectory);
		public Form1()
		{
			InitializeComponent();
			PopulateTypesList();
		}

		protected void PopulateTypesList()
		{
			List<string> dnetypes = mdef.GetFactoryTypes();
			foreach (var dnetype in dnetypes)
			{
				dynamicEntityTypeListView.Items.Add(new ListViewItem(dnetype));
			}

		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			openNodeDefFileDialog.Filter = "XML Files (.XML)|*.XML;*.xml|All Files (*.*)|*.*";
			DialogResult result = openNodeDefFileDialog.ShowDialog(); // Show the dialog.
			if (result == DialogResult.OK) // Test result.
			{
				string file = openNodeDefFileDialog.FileName;
				var nodesToAdd = AddNodeCollectionClass.FromFile(file);
				foreach (var node in nodesToAdd)
				{
					comms.AddNodes(node);
				}
			}

		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}

		private void addButton_Click(object sender, EventArgs e)
        {
            if (dynamicEntityTypeListView.SelectedItems.Count < 1)
            {
                MessageBox.Show("You Must Select a Dynamic Entity Type");
                return;
            }
            var addNodeRequest = new AddNodeRequestClass();
            addNodeRequest.DynamicEntityType = dynamicEntityTypeListView.SelectedItems[0].Text;
            addNodeRequest.InputParameters = ParameterCollection.FromXML(this.nodeXMLTextBox.Text);
            addNodeRequest.Sender = "AddDelComplexNodesCli";
            comms.AddNodes(this.addNodeIdTextBox.Text, this.nodeNameTextBox.Text, addNodeRequest.ToXML());
				MessageBox.Show("Operation Complete!");
        }

		private void deleteButton_Click(object sender, EventArgs e)
		{
			comms.DeleteNodes(this.delNodeIDTextBox.Text);
		}

		private void dynamicEntityTypeListView_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (dynamicEntityTypeListView.SelectedItems.Count > 0)
			{
				this.nodeXMLTextBox.Text = mdef.GetDefaultParameters(dynamicEntityTypeListView.SelectedItems[0].Text).ToXML();
			}
		}
	}
}
