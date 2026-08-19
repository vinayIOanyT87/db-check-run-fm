namespace GenerateRightScripts
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;

    public partial class GenerateRightScriptsForm : Form
    {
        #region Data Members
        private OpenFileDialog openInputFileDialog;
        private OpenFileDialog openOutputFileDialog;
        #endregion

        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public GenerateRightScriptsForm()
        {
            InitializeComponent();
            this.Init();
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// This method will initialize the object to its initial state.
        /// </summary>
        private void Init()
        {
            this.ClearFields();
            this.SetRunBtnState();
        }

        /// <summary>
        /// This method will clear all the text fields.
        /// </summary>
        private void ClearFields()
        {
            this.InputFileNameTB.Text = string.Empty;
            this.OutFileNameTB.Text = string.Empty;
            this.ResultTB.Text = string.Empty;
        }

        /// <summary>
        /// This method will set the state of the Run button to enabled
        /// or disabled.
        /// </summary>
        private void SetRunBtnState()
        {
            this.RunBtn.Enabled = false;

            if (string.IsNullOrEmpty(this.InputFileNameTB.Text) == false &&
                string.IsNullOrEmpty(this.OutFileNameTB.Text) == false)
            {
                this.RunBtn.Enabled = true;
            }
        }
        #endregion

        #region Handle events methods
        /// <summary>
        /// This method will handle the running of the script generation.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RunBtnClick(object sender, EventArgs e)
        {
            var updateList = new List<string>();
            var fileHander = new FileHandler();
            string fileName = this.InputFileNameTB.Text;

            List<RightsClass> rightsList = fileHander.ReadInputFile(fileName);

            if (rightsList.Count == 0)
            {
                this.ResultTB.Text = this.ResultTB.Text + "\n" + "No data found.";
            }

            foreach (RightsClass rightObj in rightsList)
            {
                string updateSql = this.BuildSqlString(rightObj);
                updateList.Add(updateSql);
            }

            if (updateList.Count == 0)
            {
                this.ResultTB.Text = this.ResultTB.Text + "\n" + "No update SQL created.";
            }

            try
            {
                fileHander.WriteOutput(this.OutFileNameTB.Text, updateList);
                this.ResultTB.Text = "Created " + updateList.Count + " SQL update statements.";
            }
            catch (Exception ex)
            {
                this.ResultTB.Text = this.ResultTB.Text + "\n" + "Failed writing SQL scripts to file '" +
                                     this.OutFileNameTB.Text + "'. " + ex.Message;
            }
        }

        /// <summary>
        /// This method will build the update SQL.
        /// </summary>
        /// <param name="rightsObj"></param>
        /// <returns></returns>
        private string BuildSqlString(RightsClass rightsObj)
        {
            string updateSql = "UPDATE lookup.tblRight SET RightDescription = '" + rightsObj.RightDescription + "' ";
            string whereSql = "WHERE RightIndex = " + rightsObj.RightIndexStr;

            updateSql = updateSql + whereSql;
            return updateSql;
        }

        /// <summary>
        /// This method will open the input file name dialog to select an
        /// input file name.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InputFileNameBrowseBtnClick(object sender, EventArgs e)
        {
            if (this.openInputFileDialog == null)
            {
                this.openInputFileDialog = new OpenFileDialog();
                this.openInputFileDialog.FileOk += this.SetInputFileNameOkEvent;
            }

            // Set filter options and filter index.
            this.openInputFileDialog.Filter = "Excel Files (*.xlsx)|*.xlsx";
            this.openInputFileDialog.FilterIndex = 1;
            this.openInputFileDialog.Multiselect = false;

            this.openInputFileDialog.ShowDialog();          
        }

        /// <summary>
        /// This method handles the ok button event for the input file name dialog.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetInputFileNameOkEvent(object sender, EventArgs e)
        {
            this.InputFileNameTB.Text = this.openInputFileDialog.FileName;
        }

        /// <summary>
        /// This method will open the output file name dialog to select an
        /// output file name.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OutFileNameBrowseBtnClick(object sender, EventArgs e)
        {
            if (this.openOutputFileDialog == null)
            {
                this.openOutputFileDialog = new OpenFileDialog();
                this.openOutputFileDialog.FileOk += this.SetOutputFileNameOkEvent;
            }

            // Set filter options and filter index.
            this.openOutputFileDialog.Filter = "SQL Files (*.sql)|*.sql";
            this.openOutputFileDialog.FilterIndex = 1;
            this.openOutputFileDialog.Multiselect = false;

            this.openOutputFileDialog.ShowDialog();
        }

        /// <summary>
        /// This method handles the ok button event for the output file name dialog.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetOutputFileNameOkEvent(object sender, EventArgs e)
        {
            this.OutFileNameTB.Text = this.openOutputFileDialog.FileName;
        }

        /// <summary>
        /// This method will handle the Clear button event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearBtnClick(object sender, EventArgs e)
        {
            this.ClearFields();
        }

        /// <summary>
        /// This method will handle the close button event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseBtnClick(object sender, EventArgs e)
        {
            Application.Exit();
        }

        /// <summary>
        /// This method will handle the input file name textbox change.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InputFileNameTbChange(object sender, EventArgs e)
        {
            this.SetRunBtnState();
        }

        /// <summary>
        /// This method will handle the output file name textbox change.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OutFileNameTbChange(object sender, EventArgs e)
        {
            this.SetRunBtnState();
        }
        #endregion
    }
}
