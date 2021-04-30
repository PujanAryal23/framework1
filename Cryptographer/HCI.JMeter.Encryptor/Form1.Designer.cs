namespace CredentialsEncryptor
{
    partial class CredentialsEncryptor
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
            this.LoadButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.csvFileLocation = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.encryptionRdoBtn = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.txtEncryptionKey = new System.Windows.Forms.TextBox();
            this.exitButton = new System.Windows.Forms.Button();
            this.SaveButton = new System.Windows.Forms.Button();
            this.toolTipMessage = new System.Windows.Forms.ToolTip(this.components);
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.newFilecreated = new System.Windows.Forms.NotifyIcon(this.components);
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // LoadButton
            // 
            this.LoadButton.Location = new System.Drawing.Point(11, 47);
            this.LoadButton.Name = "LoadButton";
            this.LoadButton.Size = new System.Drawing.Size(75, 23);
            this.LoadButton.TabIndex = 0;
            this.LoadButton.Text = "Load";
            this.toolTipMessage.SetToolTip(this.LoadButton, "Click to laod csv file.");
            this.LoadButton.UseVisualStyleBackColor = true;
            this.LoadButton.Click += new System.EventHandler(this.LoadButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.SystemColors.Info;
            this.label1.Location = new System.Drawing.Point(11, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(532, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Click on \"Load\"  button to select CSV file, enter Encryption Key. To encrypt the " +
    "credentials set the \"Encryption\"";
            // 
            // csvFileLocation
            // 
            this.csvFileLocation.BackColor = System.Drawing.SystemColors.Window;
            this.csvFileLocation.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.csvFileLocation.Location = new System.Drawing.Point(113, 49);
            this.csvFileLocation.Name = "csvFileLocation";
            this.csvFileLocation.Size = new System.Drawing.Size(417, 20);
            this.csvFileLocation.TabIndex = 2;
            this.csvFileLocation.Click += new System.EventHandler(this.csvFileLocation_Click);
            this.csvFileLocation.TextChanged += new System.EventHandler(this.ClearErrorProvider_TextChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.csvFileLocation);
            this.groupBox1.Controls.Add(this.LoadButton);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(555, 85);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Load Csv File";
            // 
            // encryptionRdoBtn
            // 
            this.encryptionRdoBtn.AutoSize = true;
            this.encryptionRdoBtn.Location = new System.Drawing.Point(152, 43);
            this.encryptionRdoBtn.Name = "encryptionRdoBtn";
            this.encryptionRdoBtn.Size = new System.Drawing.Size(75, 17);
            this.encryptionRdoBtn.TabIndex = 4;
            this.encryptionRdoBtn.Text = "Encryption";
            this.toolTipMessage.SetToolTip(this.encryptionRdoBtn, "Check to encrypt file ");
            this.encryptionRdoBtn.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.SystemColors.Control;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(11, 17);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(117, 17);
            this.label2.TabIndex = 3;
            this.label2.Text = "Encryption Key";
            // 
            // txtEncryptionKey
            // 
            this.txtEncryptionKey.BackColor = System.Drawing.SystemColors.Window;
            this.txtEncryptionKey.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtEncryptionKey.Location = new System.Drawing.Point(152, 17);
            this.txtEncryptionKey.Name = "txtEncryptionKey";
            this.txtEncryptionKey.Size = new System.Drawing.Size(297, 20);
            this.txtEncryptionKey.TabIndex = 3;
            this.toolTipMessage.SetToolTip(this.txtEncryptionKey, "Insert encryption key to be used");
            this.txtEncryptionKey.TextChanged += new System.EventHandler(this.ClearErrorProvider_TextChanged);
            // 
            // exitButton
            // 
            this.exitButton.Location = new System.Drawing.Point(491, 187);
            this.exitButton.Name = "exitButton";
            this.exitButton.Size = new System.Drawing.Size(75, 23);
            this.exitButton.TabIndex = 4;
            this.exitButton.Text = "Exit";
            this.toolTipMessage.SetToolTip(this.exitButton, "Click to exit Application");
            this.exitButton.UseVisualStyleBackColor = true;
            this.exitButton.Click += new System.EventHandler(this.exitButton_Click);
            // 
            // SaveButton
            // 
            this.SaveButton.Location = new System.Drawing.Point(399, 187);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(75, 23);
            this.SaveButton.TabIndex = 5;
            this.SaveButton.Text = "Save";
            this.toolTipMessage.SetToolTip(this.SaveButton, "Click to perform encryption/decryption.");
            this.SaveButton.UseVisualStyleBackColor = true;
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Location = new System.Drawing.Point(270, 43);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(76, 17);
            this.radioButton1.TabIndex = 7;
            this.radioButton1.Text = "Decryption";
            this.toolTipMessage.SetToolTip(this.radioButton1, "Check to encrypt file ");
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            // 
            // newFilecreated
            // 
            this.newFilecreated.Text = "notifyIcon1";
            this.newFilecreated.Visible = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtEncryptionKey);
            this.groupBox2.Controls.Add(this.radioButton1);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.encryptionRdoBtn);
            this.groupBox2.Location = new System.Drawing.Point(12, 103);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(555, 76);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Encrypt/Decrypt";
            // 
            // CredentialsEncryptor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(575, 217);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.SaveButton);
            this.Controls.Add(this.exitButton);
            this.Controls.Add(this.groupBox1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CredentialsEncryptor";
            this.Text = "Credentials Encrypt-Decrypt";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button LoadButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox csvFileLocation;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton encryptionRdoBtn;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtEncryptionKey;
        private System.Windows.Forms.ToolTip toolTipMessage;
        private System.Windows.Forms.Button exitButton;
        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.ErrorProvider errorProvider;
        private System.Windows.Forms.NotifyIcon newFilecreated;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.GroupBox groupBox2;

    }
}

