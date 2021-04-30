using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Windows.Forms;
using System.IO;
using AutomationData.Encryptor.Data;
using EncryptionBusiness.Core;

namespace Encryptor.UI
{
    /// <summary>
    /// Encryption and Decryption technique.
    /// </summary>
    public partial class EncryptDecryptForm : Form
    {
        private Tests _tests;
        private IDictionary<string, string> _credentials;

        /// <summary>
        /// Name of the test file used to load/save the customer object.
        /// </summary>
        private string XML_FILE_NAME = string.Empty;

        /// <summary>
        /// Initializes a new instance of EncryptDecryptForm class.
        /// </summary>

        public EncryptDecryptForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Loads environment parameters
        /// </summary>
        /// <param name="sender">A control</param>
        /// <param name="e">An object of EventArgs class</param>
        private void LoadObjectButton_Click(object sender, EventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                RestoreDirectory = true,
                Filter = "All xml files (*.xml) | *.XML"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                XML_FILE_NAME = openFileDialog.FileName;
            }

            // Load the environment variables from the existing XML file (if any)...
            if (File.Exists(XML_FILE_NAME))
            {
                try
                {
                    //Display full path of xml file.
                    txtFilePath.Text = XML_FILE_NAME;

                    // Load the environment variables from the XML file using XmlHelper class...
                    _tests = XmlHelper<Tests>.Load(XML_FILE_NAME);

                    if (_tests == null)
                        MessageBox.Show("Unable to load xml object from file '" + XML_FILE_NAME + "'!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else  // Load customer properties into the form...
                        this.LoadCredentialsIntoForm();
                }
                catch (Exception)
                {
                    MessageBox.Show("Loaded file '" + XML_FILE_NAME + "' is not appropriate!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
                MessageBox.Show(this.CreateFileDoesNotExistMsg(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Loads environment parameters and check on encryption or decryption radio button depending on EncryptCredentials value.
        /// </summary>
        private void LoadCredentialsIntoForm()
        {
            _credentials = _tests.Credentials.Parameters.ToDictionary(x => x.Name, y => y.Value);
            if (_tests.Credentials.IsEncrypted)
                rbEncryption.Checked = true;
            else
                rbDecryption.Checked = true;
        }

        /// <summary>
        /// Serializes data into xml file.
        /// </summary>
        /// <param name="sender">A control.</param>
        /// <param name="e">An object of EventArgs class.</param>
        private void SaveObjectButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(XML_FILE_NAME))
            {
                errProviderEncryptDecrypt.SetError(txtFilePath,this.CreateFileDoesNotExistMsg());
                return;
            }

            if (!this.EnvironmentVariableIsValid()) return;

            // Create environment variables based on Form values.
            if (CreateCredentials())
            {

                //Save environment variables to XML file using our XmlHelper class...
                try
                {
                    if (MessageBox.Show("Do you want to save ?", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {

                        XmlHelper<Tests>.Save(_tests, XML_FILE_NAME);
                        MessageBox.Show("Updated Successfully.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unable to save xml object!" + Environment.NewLine + Environment.NewLine + ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                ClearAll();
            }
        }

        /// <summary>
        /// Checks for empty value.
        /// </summary>
        /// <returns></returns>
        private bool EnvironmentVariableIsValid()
        {
            if (_credentials.Values.Contains(null))
            {
                MessageBox.Show("Value must not be empty!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
            return true;
        }
        /// <summary>
        /// Encrypt or decrypt parameters.
        /// </summary>
        /// <returns>Create successfully or not.</returns>
        private bool CreateCredentials()
        {
            try
            {
                string encryptionKey = txtEncryptionKey.Text.Trim();
                Param[] parameters = _tests.Credentials.Parameters;
                if (!string.IsNullOrEmpty(encryptionKey))
                {
                    if (rbEncryption.Checked && !_tests.Credentials.IsEncrypted)
                    {
                        foreach (var param in parameters)
                            parameters[IndexOf(parameters, param.Name)].Value = EncryptionAlgorithm.EncryptData(param.Value, encryptionKey);
                        _tests.Credentials.IsEncrypted = true;
                    }
                    if (rbDecryption.Checked && _tests.Credentials.IsEncrypted)
                    {
                        foreach (var param in parameters)
                            parameters[IndexOf(parameters, param.Name)].Value = EncryptionAlgorithm.DecryptString(param.Value, encryptionKey);
                        _tests.Credentials.IsEncrypted = false;
                    }
                    return true;
                }
                errProviderEncryptDecrypt.SetError(txtEncryptionKey, "Must not be Empty.");
            }
            catch (CryptographicException ex)
            {
                MessageBox.Show(@"Invalid encryption key.", ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtEncryptionKey.Focus();
            }
            return false;
        }

        /// <summary>
        /// Gets index of the paramters in XML file.
        /// </summary>
        /// <param name="param">An object of Param.</param>
        /// <param name="paramName">An environment parameters name.</param>
        /// <returns>Index of parameters.</returns>
        private int IndexOf(Param[] param, string paramName)
        {
            for (int i = 0; i < param.Length; i++)
                if (string.Compare(param[i].Name, paramName, StringComparison.OrdinalIgnoreCase) == 0)
                    return i;
            return -1;
        }

        /// <summary>
        /// Handles file not exist.
        /// </summary>
        /// <returns>A message.</returns>
        private string CreateFileDoesNotExistMsg()
        {
            return "The XML file " + XML_FILE_NAME + " does not exist.";
        }

        /// <summary>
        /// Clears all field.
        /// </summary>
        private void ClearAll()
        {
            XML_FILE_NAME = txtFilePath.Text = txtEncryptionKey.Text = string.Empty;
            rbEncryption.Checked = rbDecryption.Checked = false;
        }

        /// <summary>
        /// Opens a dialogue box to load data.
        /// </summary>
        /// <param name="sender">A control.</param>
        /// <param name="e">An object of EventArgs class.</param>
        private void txtFilePath_Click(object sender, EventArgs e)
        {
            this.LoadObjectButton_Click(sender, e);
        }

        /// <summary>
        /// View the customer XML file in the default web browser (if any)...
        /// </summary>
        /// <param name="sender">A control.</param>
        /// <param name="e">An object of EventArgs class.</param>
        private void ViewXMLFileButton_Click(object sender, EventArgs e)
        {
            if (File.Exists(XML_FILE_NAME) == true)
            {
                System.Diagnostics.Process.Start(XML_FILE_NAME);
            }
            else
            {
                MessageBox.Show(this.CreateFileDoesNotExistMsg(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Application exit.
        /// </summary>
        /// <param name="sender">A control.</param>
        /// <param name="e">An object of EventArgs class.</param>
        private void ExitButton_Click(object sender, EventArgs e)
        {
            var dialogResult = MessageBox.Show(@"Do you want to exit ?", this.Text, MessageBoxButtons.YesNo,
                                         MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
                Application.Exit();
        }

        /// <summary>
        /// Clears error provider.
        /// </summary>
        /// <param name="sender">A textbox control.</param>
        /// <param name="e">An object of EventArgs class.</param>
        private void ClearErrorProvider_TextChanged(object sender, EventArgs e)
        {
            var txtbox = (TextBox)sender;
            if (!string.IsNullOrEmpty(txtbox.Text.Trim()))
                errProviderEncryptDecrypt.Clear();
        }
    }
}
