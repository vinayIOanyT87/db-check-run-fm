using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FMBusinessObjects.UtilityObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.DataObjects;

namespace VCEFDecrypter
{
    public partial class Form1 : Form
    {

        private const string DECCertificateRegKey = "SOFTWARE\\Varec\\BSME Interfaces\\Enterprise\\Certificates";
        private const string DECCertificateRegValueName = "EnterpriseCertificateName";

        public Form1()
        {
            InitializeComponent();
         
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult drResult = openFileDialog1.ShowDialog(this);

            if (drResult == DialogResult.OK)
            {
                txtFile.Text = openFileDialog1.FileName;
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "VCEF File (*.vcef)|*.vcef|All Files|*.*";
            saveFileDialog1.Filter = "Text file (*.txt)|*.txt|Other file|*.*";
            saveFileDialog1.OverwritePrompt = true;
            saveFileDialog1.SupportMultiDottedExtensions = true;
            cboEncoding.Items.Add("Unicode");
            cboEncoding.Items.Add("ASCII");

            cboEncoding.SelectedIndex = 0;
            saveFileDialog1.FileName = "";
            openFileDialog1.FileName = "";

        }

        private System.IO.MemoryStream EncryptAndCompress(string data)
        {
	        var security = new SecurityClass();

			// get certificate name
			string certificateName =
				FMChannelHelper.MakeCall<IConfigurationSettings, string>(
					x => x.GetKeyValueByKey(security, ConfigurationSettingDOClass.Key_InstallDetails_EnterpriseCertificateName));


            // Compress and encrypt the stream
            var compressionProcessor = new CompressionProcessor();
            var encryption = new Encryption(this.cboEncoding.SelectedIndex == 0 ? Encoding.Unicode : Encoding.ASCII );

            encryption.CertificateName = certificateName;
            return encryption.Package(compressionProcessor.Compress(data));
        }

        private void btnDecrypt_Click(object sender, EventArgs e)
        {

            if (txtFile.Text.Trim() == "")
            {
                ShowError("No file was selected to decrypt.");
                return;
            }

            if (false == System.IO.File.Exists(txtFile.Text))
            {
                ShowError("The file selected to decrypt does not exist.");
                return;
            }

            saveFileDialog1.FileName = txtFile.Text + "_decrypted.txt";
            DialogResult drResult = saveFileDialog1.ShowDialog(this);


            if (drResult == DialogResult.Cancel)
            {
                return;                
            }

            try
            {
                Cursor.Current = Cursors.WaitCursor;

                System.IO.FileStream fileStream = System.IO.File.OpenRead(txtFile.Text);

				var security = new SecurityClass();

				// get certificate name
				string certificateName =
					FMChannelHelper.MakeCall<IConfigurationSettings, string>(
						x => x.GetKeyValueByKey(security, ConfigurationSettingDOClass.Key_InstallDetails_EnterpriseCertificateName));

				Decryption decryption = new Decryption(this.cboEncoding.SelectedIndex == 0 ? Encoding.Unicode : Encoding.ASCII);

	            decryption.CertificateName = certificateName;

                byte[] decrypted = null;

                try
                {
                    decrypted = decryption.Unpackage(fileStream);

                    var decompressor = new DecompressionProcessor();
                    byte[] decompressed = decompressor.Decompress(decrypted);
                    string data = System.Text.Encoding.ASCII.GetString(decompressed);
                    System.IO.File.WriteAllText(this.saveFileDialog1.FileName, data);
                }
                catch (Exception ee)
                {
                    if (ee.Message.Contains("Keyset does not exist"))
                    {
                        ShowError("There was a problem withe decrypting, please verify the \ncerificate is installed and that this program is run with administrative privileges.");
                        return;
                    }
                    else throw;
                }


                if (MessageBox.Show("The file has been saved to " + Environment.NewLine + saveFileDialog1.FileName + Environment.NewLine + "Would you like to open it?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start("notepad", saveFileDialog1.FileName);
                }
            }
            catch (System.Exception ex)
            {
                ShowError(ex.Message);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void ShowError(string msg)
        {
            MessageBox.Show(msg, "Error...", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (txtFile.Text.Trim() == "")
            {
                ShowError("No file was selected to encrypt.");
                return;
            }

            if (false == System.IO.File.Exists(txtFile.Text))
            {
                ShowError("The file selected to encrypt does not exist.");
                return;
            }

            saveFileDialog1.FileName = txtFile.Text + "_encrypted.vcef";
            DialogResult drResult = saveFileDialog1.ShowDialog(this);


            if (drResult == DialogResult.Cancel)
            {
                return;
            }

            try
            {
                Cursor.Current = Cursors.WaitCursor;

                System.IO.StreamReader fileStream = new System.IO.StreamReader(txtFile.Text);

                try
                {
                    string decrypted = fileStream.ReadToEnd();

                    //decrypted = decryption.Unpackage(fileStream, (cboEncoding.SelectedIndex == 0 ? System.Text.Encoding.Unicode : System.Text.Encoding.ASCII));

                    System.IO.MemoryStream ms = EncryptAndCompress(decrypted);

                    System.IO.FileStream file = new System.IO.FileStream(saveFileDialog1.FileName, System.IO.FileMode.Create);

                    ms.WriteTo(file);
                    file.Flush();
                    file.Close();
                }
                catch (Exception ee)
                {
                    if (ee.Message.Contains("Keyset does not exist"))
                    {
                        ShowError("There was a problem withe decrypting, please verify the \ncerificate is installed and that this program is run with administrative privileges.");
                        return;
                    }
                    else throw;
                }


                MessageBox.Show("The file has been saved to " + Environment.NewLine + saveFileDialog1.FileName);

            }
            catch (System.Exception ex)
            {
                ShowError(ex.Message);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }
    }
}
