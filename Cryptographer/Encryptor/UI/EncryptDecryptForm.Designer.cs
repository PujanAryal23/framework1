namespace Encryptor.UI
{
    partial class EncryptDecryptForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EncryptDecryptForm));
            this.BannerPanel = new System.Windows.Forms.Panel();
            this.lblMessage = new System.Windows.Forms.Label();
            this.LoadObjectButton = new System.Windows.Forms.Button();
            this.txtFilePath = new System.Windows.Forms.TextBox();
            this.OptionsGroupBox = new System.Windows.Forms.GroupBox();
            this.SaveObjectButton = new System.Windows.Forms.Button();
            this.ExitButton = new System.Windows.Forms.Button();
            this.ViewXMLFileButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbDecryption = new System.Windows.Forms.RadioButton();
            this.rbEncryption = new System.Windows.Forms.RadioButton();
            this.tTpEncryptDecrypt = new System.Windows.Forms.ToolTip(this.components);
            this.notifyFileCreated = new System.Windows.Forms.NotifyIcon(this.components);
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtEncryptionKey = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.errProviderEncryptDecrypt = new System.Windows.Forms.ErrorProvider(this.components);
            this.BannerPanel.SuspendLayout();
            this.OptionsGroupBox.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errProviderEncryptDecrypt)).BeginInit();
            this.SuspendLayout();
            // 
            // BannerPanel
            // 
            this.BannerPanel.BackColor = System.Drawing.Color.White;
            this.BannerPanel.Controls.Add(this.lblMessage);
            this.BannerPanel.Controls.Add(this.LoadObjectButton);
            this.BannerPanel.Controls.Add(this.txtFilePath);
            this.BannerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.BannerPanel.Location = new System.Drawing.Point(0, 0);
            this.BannerPanel.Name = "BannerPanel";
            this.BannerPanel.Size = new System.Drawing.Size(700, 75);
            this.BannerPanel.TabIndex = 1;
            // 
            // lblMessage
            // 
            this.lblMessage.AutoSize = true;
            this.lblMessage.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMessage.Location = new System.Drawing.Point(19, 6);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(675, 28);
            this.lblMessage.TabIndex = 17;
            this.lblMessage.Text = resources.GetString("lblMessage.Text");
            // 
            // LoadObjectButton
            // 
            this.LoadObjectButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.LoadObjectButton.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LoadObjectButton.Location = new System.Drawing.Point(23, 40);
            this.LoadObjectButton.Name = "LoadObjectButton";
            this.LoadObjectButton.Size = new System.Drawing.Size(121, 30);
            this.LoadObjectButton.TabIndex = 1;
            this.LoadObjectButton.Text = "&Load";
            this.tTpEncryptDecrypt.SetToolTip(this.LoadObjectButton, "Click on Load button to populate environment variables from *.xml file.");
            this.LoadObjectButton.Click += new System.EventHandler(this.LoadObjectButton_Click);
            // 
            // txtFilePath
            // 
            this.txtFilePath.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtFilePath.Location = new System.Drawing.Point(159, 45);
            this.txtFilePath.Name = "txtFilePath";
            this.txtFilePath.ReadOnly = true;
            this.txtFilePath.Size = new System.Drawing.Size(438, 21);
            this.txtFilePath.TabIndex = 0;
            this.tTpEncryptDecrypt.SetToolTip(this.txtFilePath, "Full path of loaded xml file. Double click on this textbox to load xml file.");
            this.txtFilePath.Click += new System.EventHandler(this.txtFilePath_Click);
            this.txtFilePath.TextChanged += new System.EventHandler(this.ClearErrorProvider_TextChanged);
            // 
            // OptionsGroupBox
            // 
            this.OptionsGroupBox.Controls.Add(this.SaveObjectButton);
            this.OptionsGroupBox.Controls.Add(this.ExitButton);
            this.OptionsGroupBox.Controls.Add(this.ViewXMLFileButton);
            this.OptionsGroupBox.Location = new System.Drawing.Point(11, 220);
            this.OptionsGroupBox.Name = "OptionsGroupBox";
            this.OptionsGroupBox.Size = new System.Drawing.Size(681, 43);
            this.OptionsGroupBox.TabIndex = 35;
            this.OptionsGroupBox.TabStop = false;
            // 
            // SaveObjectButton
            // 
            this.SaveObjectButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.SaveObjectButton.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SaveObjectButton.Location = new System.Drawing.Point(331, 14);
            this.SaveObjectButton.Name = "SaveObjectButton";
            this.SaveObjectButton.Size = new System.Drawing.Size(88, 23);
            this.SaveObjectButton.TabIndex = 1;
            this.SaveObjectButton.Text = "&Save";
            this.tTpEncryptDecrypt.SetToolTip(this.SaveObjectButton, "Click on Save button for saving environment variables. Load *.xml file before sav" +
        "ing.");
            this.SaveObjectButton.Click += new System.EventHandler(this.SaveObjectButton_Click);
            // 
            // ExitButton
            // 
            this.ExitButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.ExitButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.ExitButton.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ExitButton.Location = new System.Drawing.Point(436, 14);
            this.ExitButton.Name = "ExitButton";
            this.ExitButton.Size = new System.Drawing.Size(88, 23);
            this.ExitButton.TabIndex = 2;
            this.ExitButton.Text = "&Exit";
            this.tTpEncryptDecrypt.SetToolTip(this.ExitButton, "Click on Exit button to close the form.");
            this.ExitButton.Click += new System.EventHandler(this.ExitButton_Click);
            // 
            // ViewXMLFileButton
            // 
            this.ViewXMLFileButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.ViewXMLFileButton.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ViewXMLFileButton.Location = new System.Drawing.Point(223, 14);
            this.ViewXMLFileButton.Name = "ViewXMLFileButton";
            this.ViewXMLFileButton.Size = new System.Drawing.Size(88, 23);
            this.ViewXMLFileButton.TabIndex = 0;
            this.ViewXMLFileButton.Text = "&View";
            this.tTpEncryptDecrypt.SetToolTip(this.ViewXMLFileButton, "Click on View button to display environment variables on windows form control. Lo" +
        "ad *.xml file before displaying data.");
            this.ViewXMLFileButton.Click += new System.EventHandler(this.ViewXMLFileButton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbDecryption);
            this.groupBox1.Controls.Add(this.rbEncryption);
            this.groupBox1.Location = new System.Drawing.Point(11, 145);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(681, 69);
            this.groupBox1.TabIndex = 36;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Select Encrypt or Decrypt";
            // 
            // rbDecryption
            // 
            this.rbDecryption.AutoSize = true;
            this.rbDecryption.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbDecryption.Location = new System.Drawing.Point(389, 24);
            this.rbDecryption.Name = "rbDecryption";
            this.rbDecryption.Size = new System.Drawing.Size(128, 28);
            this.rbDecryption.TabIndex = 1;
            this.rbDecryption.TabStop = true;
            this.rbDecryption.Text = "Decryption";
            this.tTpEncryptDecrypt.SetToolTip(this.rbDecryption, "Select for decryption.");
            this.rbDecryption.UseVisualStyleBackColor = true;
            // 
            // rbEncryption
            // 
            this.rbEncryption.AutoSize = true;
            this.rbEncryption.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbEncryption.Location = new System.Drawing.Point(201, 24);
            this.rbEncryption.Name = "rbEncryption";
            this.rbEncryption.Size = new System.Drawing.Size(128, 28);
            this.rbEncryption.TabIndex = 0;
            this.rbEncryption.TabStop = true;
            this.rbEncryption.Text = "Encryption";
            this.tTpEncryptDecrypt.SetToolTip(this.rbEncryption, "Select for encryption. ");
            this.rbEncryption.UseVisualStyleBackColor = true;
            // 
            // tTpEncryptDecrypt
            // 
            this.tTpEncryptDecrypt.IsBalloon = true;
            this.tTpEncryptDecrypt.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.tTpEncryptDecrypt.ToolTipTitle = "Encrypt - Decrypt";
            // 
            // notifyFileCreated
            // 
            this.notifyFileCreated.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyFileCreated.Icon")));
            this.notifyFileCreated.Visible = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtEncryptionKey);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Location = new System.Drawing.Point(12, 81);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(680, 58);
            this.groupBox2.TabIndex = 37;
            this.groupBox2.TabStop = false;
            // 
            // txtEncryptionKey
            // 
            this.txtEncryptionKey.Location = new System.Drawing.Point(191, 23);
            this.txtEncryptionKey.Name = "txtEncryptionKey";
            this.txtEncryptionKey.Size = new System.Drawing.Size(348, 20);
            this.txtEncryptionKey.TabIndex = 1;
            this.txtEncryptionKey.TextChanged += new System.EventHandler(this.ClearErrorProvider_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(47, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(122, 19);
            this.label1.TabIndex = 0;
            this.label1.Text = "Encryption Key :";
            // 
            // errProviderEncryptDecrypt
            // 
            this.errProviderEncryptDecrypt.ContainerControl = this;
            // 
            // EncryptDecryptForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(700, 267);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.OptionsGroupBox);
            this.Controls.Add(this.BannerPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "EncryptDecryptForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Encrypt - Decrypt";
            this.BannerPanel.ResumeLayout(false);
            this.BannerPanel.PerformLayout();
            this.OptionsGroupBox.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errProviderEncryptDecrypt)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel BannerPanel;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.Button LoadObjectButton;
        private System.Windows.Forms.TextBox txtFilePath;
        private System.Windows.Forms.GroupBox OptionsGroupBox;
        private System.Windows.Forms.Button SaveObjectButton;
        private System.Windows.Forms.Button ExitButton;
        private System.Windows.Forms.Button ViewXMLFileButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbDecryption;
        private System.Windows.Forms.RadioButton rbEncryption;
        private System.Windows.Forms.ToolTip tTpEncryptDecrypt;
        private System.Windows.Forms.NotifyIcon notifyFileCreated;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtEncryptionKey;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ErrorProvider errProviderEncryptDecrypt;
    }
}