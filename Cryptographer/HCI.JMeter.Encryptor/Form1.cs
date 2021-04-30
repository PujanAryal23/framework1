using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Security.Cryptography;

namespace CredentialsEncryptor
{
    public partial class CredentialsEncryptor : Form
    {
        private string CSV_FILE_NAME = string.Empty;
        private string CSV_OLD_FILE_NAME = string.Empty;
        private string ENCRYPTION_KEY = string.Empty;

        public CredentialsEncryptor()
        {
            InitializeComponent();
        }

        private void LoadButton_Click(object sender, EventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                RestoreDirectory = true,
                Filter = "All xml files (*.csv) | *.CSV"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                CSV_FILE_NAME = openFileDialog.FileName;
            }
            csvFileLocation.Text = CSV_FILE_NAME;
        }

        private void csvFileLocation_Click(object sender, EventArgs e)
        {
            this.LoadButton_Click(sender, e);
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(CSV_FILE_NAME))
            {
                errorProvider.SetError(csvFileLocation, "File doesn't exist.");
                return;
            }

            ENCRYPTION_KEY = txtEncryptionKey.Text.Trim();

            if(string.IsNullOrEmpty(ENCRYPTION_KEY))
            {
                errorProvider.SetError(txtEncryptionKey, "Key is empty");
                return;
            }

            if(!encryptionRdoBtn.Checked && !radioButton1.Checked)
            {
                MessageBox.Show(@"Select either Encryption or Decryption Option.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                if (MessageBox.Show("Do you want to continue ?", this.Text, MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (encryptionRdoBtn.Checked)
                    {
                        EncryptCsvFile(CSV_FILE_NAME);
                        DeleteFile(CSV_OLD_FILE_NAME);
                    }
                    else
                    {
                        DecryptCsvFile(CSV_FILE_NAME);
                        DeleteFile(CSV_OLD_FILE_NAME);
                    }
                    MessageBox.Show("Updated Successfully.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CSV_FILE_NAME = CSV_OLD_FILE_NAME = txtEncryptionKey.Text = csvFileLocation.Text = String.Empty;
                    encryptionRdoBtn.Checked = false;
                    radioButton1.Checked = false;
                }
            }
            catch (FormatException ex)
            {
                MessageBox.Show(@"Data cannot be decrypted.", ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetToDefault();
                txtEncryptionKey.Focus();
            }
            catch(CryptographicException ex)
            {
                MessageBox.Show(@"Invalid encryption key.", ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetToDefault();
                txtEncryptionKey.Focus();
            }
        }

        private void SetToDefault()
        {
            try
            {
                DeleteFile(CSV_FILE_NAME);
                RenameFileName(CSV_OLD_FILE_NAME, CSV_FILE_NAME);
            }
            catch(Exception)
            {
            }
        }

        private void EncryptCsvFile(string fileName)
        {
            CSV_OLD_FILE_NAME = GetFileDirectory(fileName) + "\\Credentials2.csv";
            RenameFileName(fileName, CSV_OLD_FILE_NAME);
            using (var csvFileReader = new CsvFileReader(CSV_OLD_FILE_NAME))
            {
                using (var csvFileWriter = new CsvFileWriter(fileName))
                {
                    bool isFirst = true;
                    var csvRow = new CsvRow();
                    while (csvFileReader.ReadRow(csvRow))
                    {
                        if (!isFirst)
                        {
                            if (csvRow.Count != 2)
                                throw new Exception("More tha two values are present");
                            csvRow[0] = EncryptionBusinessJMeter.EncryptData(csvRow[0], ENCRYPTION_KEY);
                            csvRow[1] = EncryptionBusinessJMeter.EncryptData(csvRow[1], ENCRYPTION_KEY);
                        }
                        csvFileWriter.WriteRow(csvRow);
                        isFirst = false;
                        csvRow.Clear();
                    }
                }
            }
        }

        private void DecryptCsvFile(string fileName)
        {
            CSV_OLD_FILE_NAME = GetFileDirectory(fileName) + "\\Credentials2.csv";
            RenameFileName(fileName, CSV_OLD_FILE_NAME);
            using (var csvFileReader = new CsvFileReader(CSV_OLD_FILE_NAME))
            {
                using (var csvFileWriter = new CsvFileWriter(fileName))
                {
                    bool isFirst = true;
                    var csvRow = new CsvRow();
                    while (csvFileReader.ReadRow(csvRow))
                    {
                        if (!isFirst)
                        {
                            if (csvRow.Count != 2)
                                throw new Exception("More tha two values are present");
                            csvRow[0] = EncryptionBusinessJMeter.DecryptString(csvRow[0], ENCRYPTION_KEY);
                            csvRow[1] = EncryptionBusinessJMeter.DecryptString(csvRow[1], ENCRYPTION_KEY);
                        }
                        csvFileWriter.WriteRow(csvRow);
                        isFirst = false;
                        csvRow.Clear();
                    }
                }
            }
        }

        private bool DeleteFile(string fileName)
        {
            try
            {
                File.Delete(fileName);
            }
            catch(Exception)
            {
                return false;
            }
            return true;
        }

        public static string GetFileDirectory(string fileName)
        {
            return new FileInfo(fileName).DirectoryName;
        }

        public static void RenameFileName(String oldName, string renamedFileName)
        {
            File.Move(oldName, renamedFileName);
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            var dialogResult = MessageBox.Show(@"Do you want to exit ?", this.Text, MessageBoxButtons.YesNo,
                                         MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
                Application.Exit();
        }

        private void ClearErrorProvider_TextChanged(object sender, EventArgs e)
        {
            var txtbox = (TextBox)sender;
            if (!string.IsNullOrEmpty(txtbox.Text.Trim()))
                errorProvider.Clear();
        }
    }
}
