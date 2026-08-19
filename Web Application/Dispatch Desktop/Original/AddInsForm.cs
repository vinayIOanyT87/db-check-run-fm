using System;
using System.Configuration;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using FMBusinessObjects.DataObjects;

namespace DispatchPrototype
{
	public partial class AddInsForm : FMBaseForm
	{
		public AddInItemsCollectionClass AddInItemsCollection = new AddInItemsCollectionClass();
		public AddInsForm()
		{
			InitializeComponent();

            base.GetSecurity();
		}

		private void Closebutton_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void ReadConfigFile()
		{
			string AppMenuItem = "";
			string AppPathItem = "";
			bool ExitWhile = false;

			int iLoop = 0;
			while (ExitWhile == false)
			{
				AppMenuItem = "MenuItem" + iLoop.ToString();
				AppPathItem = "AppPath" + iLoop.ToString();

				string LVText = ConfigurationManager.AppSettings[AppMenuItem];
				string LVText1 = ConfigurationManager.AppSettings[AppPathItem];
				if (LVText != null &&
					LVText1 != null)
				{
					AddInItemClass AddInItem = new AddInItemClass();

					AddInItem.MenuItem = LVText;
					AddInItem.Application = LVText1;

					AddInItemsCollection.Add(AddInItem);
				}
				else
				{
					ExitWhile = true;
					break;
				}
				++iLoop;
			}
			
		}

		private void OnLoadDialog(object sender, EventArgs e)
		{
			Addbutton.Enabled = false;
			Modifybutton.Enabled = false;
			Deletebutton.Enabled = false;
			ReadConfigFile();
			PopulateListviewDisplay();
		}

		private void PopulateListviewDisplay()
		{
			AssignedAddInslistView.Clear();

			AssignedAddInslistView.View = View.Details;

			AssignedAddInslistView.Columns.Add("Menu Item", 100, HorizontalAlignment.Left);

			AssignedAddInslistView.Columns.Add("Application", 400, HorizontalAlignment.Left);

			foreach (AddInItemClass AddInItem in AddInItemsCollection)
			{
				ListViewItem Li = new ListViewItem();
				Li.Text = AddInItem.MenuItem;
				Li.SubItems.Add(AddInItem.Application);
				AssignedAddInslistView.Items.Add(Li);
			}

		}

		private void Browsebutton_Click(object sender, EventArgs e)
		{
			string SelectedFileAndPath = "";

			OpenFileDialog openFileDialog1 = new OpenFileDialog();

			openFileDialog1.InitialDirectory = "c:\\";
			openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
			openFileDialog1.FilterIndex = 2;
			openFileDialog1.RestoreDirectory = true;
			openFileDialog1.CheckFileExists = true;
			openFileDialog1.CheckPathExists = true;
			openFileDialog1.Multiselect = false;

			if (openFileDialog1.ShowDialog() == DialogResult.OK)
			{
				try
				{
					SelectedFileAndPath = openFileDialog1.FileName;
					if (SelectedFileAndPath.Length > 0)
					{
						// add this to the selected file dialog
						ApplicationtextBox.Text = SelectedFileAndPath;
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show("Error: Could not read file from disk. error: " + ex.Message);
				}
			}
		}

		private void OnTextChanged(object sender, EventArgs e)
		{
			CheckIfAllInformationIsAvailable();
		}

		private void CheckIfAllInformationIsAvailable()
		{
			Addbutton.Enabled = false;
			Modifybutton.Enabled = false;
			if (MenuNametextBox.Text.Length < 1)
				return;
			if (ApplicationtextBox.Text.Length < 1)
				return;

            if (base.Security.HasRight(RIGHT.MODIFY_DISPATCH))
            {
			    Addbutton.Enabled = true;
            }
            else
            {
                Addbutton.Enabled = false;
            }

			if (AssignedAddInslistView.SelectedItems.Count > 0)
            {
                if (base.Security.HasRight(RIGHT.MODIFY_DISPATCH))
                {
				    Modifybutton.Enabled = true;
                }
                else
                {
                    Modifybutton.Enabled = false;
                }
            }
		}

		private void Addbutton_Click(object sender, EventArgs e)
		{
            // Check Security
            if (!base.Security.HasRight(RIGHT.MODIFY_DISPATCH))
            {
                return;
            }

			// check if this item already exists

			if (MenuNametextBox.Text.Length < 1 || ApplicationtextBox.Text.Length < 1)
			{
				Addbutton.Enabled = false;
				Modifybutton.Enabled = false;
				return;
			}

			if (AssignedAddInslistView.Items.Count > 0)
			{
				ListViewItem foundItem = AssignedAddInslistView.FindItemWithText(MenuNametextBox.Text, false, 0, true);
				if (foundItem != null)
				{
					MessageBox.Show("Item Already Exists");
					return;
				}
			}

			// add the item to the collection

			AddInItemClass AddInItem = new AddInItemClass();
			AddInItem.MenuItem = MenuNametextBox.Text;
			AddInItem.Application = ApplicationtextBox.Text;
			AddInItemsCollection.Add(AddInItem);
			PopulateListviewDisplay();
		}

		private void OKbutton_Click(object sender, EventArgs e)
		{
			string AppMenuItem = "";
			string AppPathItem = "";
			bool ExitWhileLoop = false;

			System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

			int iLoop = 0;
			// first delete all old records out of the file
			while (ExitWhileLoop == false)
			{
				AppMenuItem = "MenuItem" + iLoop.ToString();
				AppPathItem = "AppPath" + iLoop.ToString();
				string LVText = ConfigurationManager.AppSettings[AppMenuItem];
				string LVText1 = ConfigurationManager.AppSettings[AppPathItem];
				if (LVText != null &&
					LVText1 != null)
				{
					config.AppSettings.Settings.Remove(AppMenuItem);
					config.AppSettings.Settings.Remove(AppPathItem);
				}
				else
				{
					ExitWhileLoop = true;
					break;
				}
				++iLoop;
			}

			// now add the new records
			iLoop = 0;
			foreach (AddInItemClass AddInItem in AddInItemsCollection)
			{
				AppMenuItem = "MenuItem" + iLoop.ToString();
				AppPathItem = "AppPath" + iLoop.ToString();
				config.AppSettings.Settings.Add(AppMenuItem, AddInItem.MenuItem);
				config.AppSettings.Settings.Add(AppPathItem, AddInItem.Application);
				++iLoop;
			}
			// Save the configuration file.
			config.Save(ConfigurationSaveMode.Modified);

			// Force a reload of a changed section.
			ConfigurationManager.RefreshSection("appSettings");
			DialogResult = DialogResult.OK;
		}

		private void OnListViewSelectedIndexChanged(object sender, EventArgs e)
		{
			// user has made a selection so transfer the selection to the edit fields
			if (AssignedAddInslistView.SelectedItems.Count == 0)
				return;
			string selecteditemtext = AssignedAddInslistView.SelectedItems[0].Text;
			foreach (AddInItemClass AddInItem in AddInItemsCollection)
			{
				if (AddInItem.MenuItem == selecteditemtext)
				{
					MenuNametextBox.Text = AddInItem.MenuItem;
					ApplicationtextBox.Text = AddInItem.Application;
					break;
				}
			}
            // Check Security
            if (base.Security.HasRight(RIGHT.MODIFY_DISPATCH))
            {
                Modifybutton.Enabled = true;
                Deletebutton.Enabled = true;
            }
            else
            {
                Modifybutton.Enabled = false;
                Deletebutton.Enabled = false;
            }
		}

		private void Deletebutton_Click(object sender, EventArgs e)
		{
            // Check Security
            if (!base.Security.HasRight(RIGHT.MODIFY_DISPATCH))
            {
                return;
            }

			if (AssignedAddInslistView.SelectedItems.Count == 0)
				return;
			string selecteditemtext = AssignedAddInslistView.SelectedItems[0].Text;
			foreach (AddInItemClass AddInItem in AddInItemsCollection)
			{
				if (AddInItem.MenuItem == selecteditemtext)
				{
					AddInItemsCollection.Remove(AddInItem);
					break;
				}
			}
			PopulateListviewDisplay();
		}

		private void Modifybutton_Click(object sender, EventArgs e)
		{
            // Check Security
            if (!base.Security.HasRight(RIGHT.MODIFY_DISPATCH))
            {
                return;
            }
            
            if (AssignedAddInslistView.SelectedItems.Count == 0)
				return;
			if (MenuNametextBox.Text.Length < 1 || ApplicationtextBox.Text.Length < 1)
			{
				Addbutton.Enabled = false;
				Modifybutton.Enabled = false;
				return;
			}
			string selecteditemtext = AssignedAddInslistView.SelectedItems[0].Text;
			foreach (AddInItemClass AddInItem in AddInItemsCollection)
			{
				if (AddInItem.MenuItem == selecteditemtext)
				{
					AddInItemsCollection.Remove(AddInItem);
					AddInItem.MenuItem = MenuNametextBox.Text;
					AddInItem.Application = ApplicationtextBox.Text;
					AddInItemsCollection.Add(AddInItem);
					break;
				}
			}
			PopulateListviewDisplay();
		}

	}
}
