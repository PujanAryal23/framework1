using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Encryptor.UI
{
    public partial class EncryptionKeyForm : Form
    {
        private const string Caption = @"Encryption Key";
        public EncryptionKeyForm()
        {
            InitializeComponent();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            var key = txtEncryptionKey.Text.Trim();
            if (!string.IsNullOrEmpty(key))
            {
                Program.EncryptionKey = key;
                Program.ShowMainForm();
            }
            else
            {
                MessageBox.Show(@"Please enter valid key.", Caption, MessageBoxButtons.OKCancel,
                                MessageBoxIcon.Warning);
            }
            this.Visible = true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("Do you want to exit ?",Caption, MessageBoxButtons.YesNo,MessageBoxIcon.Information) == DialogResult.Yes)
                this.Close();
        }
    }
}
