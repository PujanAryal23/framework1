using System.Linq;
using System;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;
using Encryptor.Data;
using EncryptionBusiness.Core;

namespace Encryptor.UI
{
    /// <summary>
    /// Test form to demonstrate loading and saving an object to XML using the ObjectXMLSerializer class.
    /// </summary>
    public class MainForm : Form
    {
        private EncryptionKeyForm _environmentSelectorForm;
        readonly IDictionary<string, string> _webBrowserDriver = new Dictionary<string, string> { { "Chrome", "chrome" }, { "Internet Explorer", "ie" }, { "Firefox", "firefox" } };
        readonly IDictionary<string, string> _testClient = new Dictionary<string, string> { { "Reserved for Automated Tests (Keep Out!)", "UITST" }, { "EBMC", "EBMC" }, { "Public Employee Health Plans", "PEHP" }, { "TallTree Administrators", "TTREE" }, { "RPE Testing", "RPE" } };
        readonly IDictionary<string, string> _userType = new Dictionary<string, string> { { "HCIAdmin", "HCIADMIN" }, { "HCINormal", "HCINORMAL" }, { "Client", "CLIENT" } };

        private String _encryptionKey;
        private String _dataSource;
        private String _userId;
        private String _password;

        /// <summary>
        /// Name of the test file used to load/save the customer object.
        /// </summary>
        private string XML_FILE_NAME = "";

        private Button LoadObjectButton;
        private Button SaveObjectButton;
        private Button ViewXMLFileButton;
        private Label lblHciAdminUserName;
        private TextBox txtHciAdminUserName;
        private Panel BannerPanel;
        private GroupBox OptionsGroupBox;
        private GroupBox EnvironmentVariablesGroupBox;
        private Label lblEnvironmentVariables;
        private Label lblHciAdminPassword;
        private TextBox txtHciAdminPassword;
        private Label lblHciUsername;
        private TextBox txtHciUserName;
        private Label lblHciPassword;
        private TextBox txtHciPassword;
        private Label lblClientUserName;
        private TextBox txtClientUserName;
        private Label lblClientPassword;
        private TextBox txtClientPassword;
        private Label lblUserType;
        private Label lblIsInvoicePresent;
        private CheckBox chkIsInvoicePresent;
        private Label lblIsHciUserAuthorizedToManagerAppeals;
        private CheckBox chkIsHciUserAuthorizedToManageAppeals;
        private Label lblIsClientUserAuthorizedToManageAppeals;
        private CheckBox chkIsClientUserAuthorizedToManageAppeals;
        private TextBox txtDataSource;
        private Label lblTestClient;
        private Label lblApplicationUrl;
        private TextBox txtApplicationUrl;
        private Label lblWebBrowserDriver;
        private ComboBox cmbWebBrowserDriver;
        private ComboBox cmbTestClient;
        private ComboBox cmbUserType;
        private GroupBox grpConnectionString;
        private Label lblDataSource;
        private Label lblPassword;
        private Label lblUserId;
        private TextBox txtPassword;
        private TextBox txtUserId;
        private ToolTip encryptToolTip;
        private Label lblMessage;
        private Button ExitButton;
        private TextBox txtFilePath;
        private GroupBox grpManageAppealUsers;
        private GroupBox grpDefaultUser;
        private GroupBox grpUrlAndBrowser;
        private GroupBox grpUserTypeAndClient;
        private Label lblHciUserWithReadWrite;
        private Label label4;
        private Label label2;
        private Label lblHciPasswordWithReadWrite;
        private Label lblClientUserWithReadWrite;
        private Label lblHciUserWithReadOnly;
        private TextBox txtClientPasswordWithReadWrite;
        private TextBox txtClientUserWithReadWrite;
        private TextBox txtHciPasswordWithReadOnly;
        private TextBox txtHciUserWithReadOnly;
        private TextBox txtHciPasswordWithReadWrite;
        private TextBox txtHciUserWithReadWrite;
        private System.ComponentModel.IContainer components;

        //IDictionary<ComboBox, IDictionary<string, string>> loadComboBox = new Dictionary<ComboBox, IDictionary<string, string>> { { cmbWebBrowserDriver, _webBrowserDriver } };

        public MainForm(EncryptionKeyForm environmentSelectorForm)
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
            _environmentSelectorForm = environmentSelectorForm;
            _environmentSelectorForm.Visible = false;
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.LoadObjectButton = new System.Windows.Forms.Button();
            this.SaveObjectButton = new System.Windows.Forms.Button();
            this.ViewXMLFileButton = new System.Windows.Forms.Button();
            this.lblHciAdminUserName = new System.Windows.Forms.Label();
            this.txtHciAdminUserName = new System.Windows.Forms.TextBox();
            this.BannerPanel = new System.Windows.Forms.Panel();
            this.lblMessage = new System.Windows.Forms.Label();
            this.txtFilePath = new System.Windows.Forms.TextBox();
            this.OptionsGroupBox = new System.Windows.Forms.GroupBox();
            this.ExitButton = new System.Windows.Forms.Button();
            this.EnvironmentVariablesGroupBox = new System.Windows.Forms.GroupBox();
            this.grpManageAppealUsers = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblHciPasswordWithReadWrite = new System.Windows.Forms.Label();
            this.lblClientUserWithReadWrite = new System.Windows.Forms.Label();
            this.lblHciUserWithReadOnly = new System.Windows.Forms.Label();
            this.lblHciUserWithReadWrite = new System.Windows.Forms.Label();
            this.txtClientPasswordWithReadWrite = new System.Windows.Forms.TextBox();
            this.txtClientUserWithReadWrite = new System.Windows.Forms.TextBox();
            this.txtHciPasswordWithReadOnly = new System.Windows.Forms.TextBox();
            this.txtHciUserWithReadOnly = new System.Windows.Forms.TextBox();
            this.txtHciPasswordWithReadWrite = new System.Windows.Forms.TextBox();
            this.txtHciUserWithReadWrite = new System.Windows.Forms.TextBox();
            this.grpDefaultUser = new System.Windows.Forms.GroupBox();
            this.lblHciAdminPassword = new System.Windows.Forms.Label();
            this.lblHciUsername = new System.Windows.Forms.Label();
            this.txtHciPassword = new System.Windows.Forms.TextBox();
            this.txtHciAdminPassword = new System.Windows.Forms.TextBox();
            this.txtHciUserName = new System.Windows.Forms.TextBox();
            this.txtClientPassword = new System.Windows.Forms.TextBox();
            this.lblHciPassword = new System.Windows.Forms.Label();
            this.lblClientPassword = new System.Windows.Forms.Label();
            this.lblClientUserName = new System.Windows.Forms.Label();
            this.txtClientUserName = new System.Windows.Forms.TextBox();
            this.grpUrlAndBrowser = new System.Windows.Forms.GroupBox();
            this.lblApplicationUrl = new System.Windows.Forms.Label();
            this.txtApplicationUrl = new System.Windows.Forms.TextBox();
            this.cmbWebBrowserDriver = new System.Windows.Forms.ComboBox();
            this.lblWebBrowserDriver = new System.Windows.Forms.Label();
            this.lblEnvironmentVariables = new System.Windows.Forms.Label();
            this.lblUserType = new System.Windows.Forms.Label();
            this.lblIsInvoicePresent = new System.Windows.Forms.Label();
            this.chkIsInvoicePresent = new System.Windows.Forms.CheckBox();
            this.lblIsHciUserAuthorizedToManagerAppeals = new System.Windows.Forms.Label();
            this.chkIsHciUserAuthorizedToManageAppeals = new System.Windows.Forms.CheckBox();
            this.lblIsClientUserAuthorizedToManageAppeals = new System.Windows.Forms.Label();
            this.chkIsClientUserAuthorizedToManageAppeals = new System.Windows.Forms.CheckBox();
            this.txtDataSource = new System.Windows.Forms.TextBox();
            this.lblTestClient = new System.Windows.Forms.Label();
            this.cmbTestClient = new System.Windows.Forms.ComboBox();
            this.cmbUserType = new System.Windows.Forms.ComboBox();
            this.grpConnectionString = new System.Windows.Forms.GroupBox();
            this.lblPassword = new System.Windows.Forms.Label();
            this.lblUserId = new System.Windows.Forms.Label();
            this.lblDataSource = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.txtUserId = new System.Windows.Forms.TextBox();
            this.encryptToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.grpUserTypeAndClient = new System.Windows.Forms.GroupBox();
            this.BannerPanel.SuspendLayout();
            this.OptionsGroupBox.SuspendLayout();
            this.EnvironmentVariablesGroupBox.SuspendLayout();
            this.grpManageAppealUsers.SuspendLayout();
            this.grpDefaultUser.SuspendLayout();
            this.grpUrlAndBrowser.SuspendLayout();
            this.grpConnectionString.SuspendLayout();
            this.grpUserTypeAndClient.SuspendLayout();
            this.SuspendLayout();
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
            this.encryptToolTip.SetToolTip(this.LoadObjectButton, "Click on Load button to populate environment variables from *.xml file.");
            this.LoadObjectButton.Click += new System.EventHandler(this.LoadObjectButton_Click);
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
            this.encryptToolTip.SetToolTip(this.SaveObjectButton, "Click on Save button for saving environment variables. Load *.xml file before sav" +
        "ing.");
            this.SaveObjectButton.Click += new System.EventHandler(this.SaveObjectButton_Click);
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
            this.encryptToolTip.SetToolTip(this.ViewXMLFileButton, "Click on View button to display environment variables on windows form control. Lo" +
        "ad *.xml file before displaying data.");
            this.ViewXMLFileButton.Click += new System.EventHandler(this.ViewXMLFileButton_Click);
            // 
            // lblHciAdminUserName
            // 
            this.lblHciAdminUserName.AutoSize = true;
            this.lblHciAdminUserName.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHciAdminUserName.Location = new System.Drawing.Point(5, 20);
            this.lblHciAdminUserName.Name = "lblHciAdminUserName";
            this.lblHciAdminUserName.Size = new System.Drawing.Size(140, 13);
            this.lblHciAdminUserName.TabIndex = 22;
            this.lblHciAdminUserName.Text = "HCI Admin Username :";
            this.lblHciAdminUserName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtHciAdminUserName
            // 
            this.txtHciAdminUserName.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtHciAdminUserName.Location = new System.Drawing.Point(181, 17);
            this.txtHciAdminUserName.Name = "txtHciAdminUserName";
            this.txtHciAdminUserName.Size = new System.Drawing.Size(191, 21);
            this.txtHciAdminUserName.TabIndex = 0;
            this.encryptToolTip.SetToolTip(this.txtHciAdminUserName, "Enter hci admin username.");
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
            this.BannerPanel.Size = new System.Drawing.Size(787, 75);
            this.BannerPanel.TabIndex = 0;
            // 
            // lblMessage
            // 
            this.lblMessage.AutoSize = true;
            this.lblMessage.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMessage.Location = new System.Drawing.Point(19, 6);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(735, 28);
            this.lblMessage.TabIndex = 17;
            this.lblMessage.Text = "Click on \"Load\"  button to select XML file and input all the required test enviro" +
    "nment variables. To encrypt the credentials\r\nset the \"Encrypt Credentials\" check" +
    "box checked.\r\n";
            // 
            // txtFilePath
            // 
            this.txtFilePath.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtFilePath.Location = new System.Drawing.Point(159, 45);
            this.txtFilePath.Name = "txtFilePath";
            this.txtFilePath.ReadOnly = true;
            this.txtFilePath.Size = new System.Drawing.Size(595, 21);
            this.txtFilePath.TabIndex = 0;
            this.encryptToolTip.SetToolTip(this.txtFilePath, "Full path of loaded xml file. Double click on this textbox to load xml file.");
            this.txtFilePath.DoubleClick += new System.EventHandler(this.txtFilePath_DoubleClick);
            // 
            // OptionsGroupBox
            // 
            this.OptionsGroupBox.Controls.Add(this.SaveObjectButton);
            this.OptionsGroupBox.Controls.Add(this.ExitButton);
            this.OptionsGroupBox.Controls.Add(this.ViewXMLFileButton);
            this.OptionsGroupBox.Location = new System.Drawing.Point(12, 621);
            this.OptionsGroupBox.Name = "OptionsGroupBox";
            this.OptionsGroupBox.Size = new System.Drawing.Size(763, 43);
            this.OptionsGroupBox.TabIndex = 34;
            this.OptionsGroupBox.TabStop = false;
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
            this.encryptToolTip.SetToolTip(this.ExitButton, "Click on Exit button to close the form.");
            this.ExitButton.Click += new System.EventHandler(this.ExitButton_Click);
            // 
            // EnvironmentVariablesGroupBox
            // 
            this.EnvironmentVariablesGroupBox.Controls.Add(this.grpManageAppealUsers);
            this.EnvironmentVariablesGroupBox.Controls.Add(this.grpDefaultUser);
            this.EnvironmentVariablesGroupBox.Controls.Add(this.grpUrlAndBrowser);
            this.EnvironmentVariablesGroupBox.Location = new System.Drawing.Point(12, 78);
            this.EnvironmentVariablesGroupBox.Name = "EnvironmentVariablesGroupBox";
            this.EnvironmentVariablesGroupBox.Size = new System.Drawing.Size(763, 540);
            this.EnvironmentVariablesGroupBox.TabIndex = 0;
            this.EnvironmentVariablesGroupBox.TabStop = false;
            // 
            // grpManageAppealUsers
            // 
            this.grpManageAppealUsers.Controls.Add(this.label4);
            this.grpManageAppealUsers.Controls.Add(this.label2);
            this.grpManageAppealUsers.Controls.Add(this.lblHciPasswordWithReadWrite);
            this.grpManageAppealUsers.Controls.Add(this.lblClientUserWithReadWrite);
            this.grpManageAppealUsers.Controls.Add(this.lblHciUserWithReadOnly);
            this.grpManageAppealUsers.Controls.Add(this.lblHciUserWithReadWrite);
            this.grpManageAppealUsers.Controls.Add(this.txtClientPasswordWithReadWrite);
            this.grpManageAppealUsers.Controls.Add(this.txtClientUserWithReadWrite);
            this.grpManageAppealUsers.Controls.Add(this.txtHciPasswordWithReadOnly);
            this.grpManageAppealUsers.Controls.Add(this.txtHciUserWithReadOnly);
            this.grpManageAppealUsers.Controls.Add(this.txtHciPasswordWithReadWrite);
            this.grpManageAppealUsers.Controls.Add(this.txtHciUserWithReadWrite);
            this.grpManageAppealUsers.Location = new System.Drawing.Point(11, 204);
            this.grpManageAppealUsers.Name = "grpManageAppealUsers";
            this.grpManageAppealUsers.Size = new System.Drawing.Size(746, 145);
            this.grpManageAppealUsers.TabIndex = 38;
            this.grpManageAppealUsers.TabStop = false;
            this.grpManageAppealUsers.Text = "Manage Appeal Users";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(409, 105);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(70, 13);
            this.label4.TabIndex = 22;
            this.label4.Text = "Password :";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(409, 68);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 13);
            this.label2.TabIndex = 22;
            this.label2.Text = "Password :";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblHciPasswordWithReadWrite
            // 
            this.lblHciPasswordWithReadWrite.AutoSize = true;
            this.lblHciPasswordWithReadWrite.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHciPasswordWithReadWrite.Location = new System.Drawing.Point(409, 32);
            this.lblHciPasswordWithReadWrite.Name = "lblHciPasswordWithReadWrite";
            this.lblHciPasswordWithReadWrite.Size = new System.Drawing.Size(70, 13);
            this.lblHciPasswordWithReadWrite.TabIndex = 22;
            this.lblHciPasswordWithReadWrite.Text = "Password :";
            this.lblHciPasswordWithReadWrite.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblClientUserWithReadWrite
            // 
            this.lblClientUserWithReadWrite.AutoSize = true;
            this.lblClientUserWithReadWrite.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblClientUserWithReadWrite.Location = new System.Drawing.Point(2, 105);
            this.lblClientUserWithReadWrite.Name = "lblClientUserWithReadWrite";
            this.lblClientUserWithReadWrite.Size = new System.Drawing.Size(176, 13);
            this.lblClientUserWithReadWrite.TabIndex = 22;
            this.lblClientUserWithReadWrite.Text = "Client User With Read/Write :";
            this.lblClientUserWithReadWrite.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblHciUserWithReadOnly
            // 
            this.lblHciUserWithReadOnly.AutoSize = true;
            this.lblHciUserWithReadOnly.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHciUserWithReadOnly.Location = new System.Drawing.Point(3, 68);
            this.lblHciUserWithReadOnly.Name = "lblHciUserWithReadOnly";
            this.lblHciUserWithReadOnly.Size = new System.Drawing.Size(156, 13);
            this.lblHciUserWithReadOnly.TabIndex = 22;
            this.lblHciUserWithReadOnly.Text = "HCI User With ReadOnly :";
            this.lblHciUserWithReadOnly.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblHciUserWithReadWrite
            // 
            this.lblHciUserWithReadWrite.AutoSize = true;
            this.lblHciUserWithReadWrite.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHciUserWithReadWrite.Location = new System.Drawing.Point(3, 32);
            this.lblHciUserWithReadWrite.Name = "lblHciUserWithReadWrite";
            this.lblHciUserWithReadWrite.Size = new System.Drawing.Size(165, 13);
            this.lblHciUserWithReadWrite.TabIndex = 22;
            this.lblHciUserWithReadWrite.Text = "HCI User With Read/Write :";
            this.lblHciUserWithReadWrite.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtClientPasswordWithReadWrite
            // 
            this.txtClientPasswordWithReadWrite.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtClientPasswordWithReadWrite.Location = new System.Drawing.Point(540, 102);
            this.txtClientPasswordWithReadWrite.Name = "txtClientPasswordWithReadWrite";
            this.txtClientPasswordWithReadWrite.PasswordChar = '*';
            this.txtClientPasswordWithReadWrite.Size = new System.Drawing.Size(191, 21);
            this.txtClientPasswordWithReadWrite.TabIndex = 5;
            this.encryptToolTip.SetToolTip(this.txtClientPasswordWithReadWrite, "Enter client user with read write to manage appeals password.");
            this.txtClientPasswordWithReadWrite.UseSystemPasswordChar = true;
            // 
            // txtClientUserWithReadWrite
            // 
            this.txtClientUserWithReadWrite.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtClientUserWithReadWrite.Location = new System.Drawing.Point(178, 102);
            this.txtClientUserWithReadWrite.Name = "txtClientUserWithReadWrite";
            this.txtClientUserWithReadWrite.Size = new System.Drawing.Size(190, 21);
            this.txtClientUserWithReadWrite.TabIndex = 4;
            this.encryptToolTip.SetToolTip(this.txtClientUserWithReadWrite, "Enter client username with read write to manage appeals.");
            // 
            // txtHciPasswordWithReadOnly
            // 
            this.txtHciPasswordWithReadOnly.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtHciPasswordWithReadOnly.Location = new System.Drawing.Point(540, 65);
            this.txtHciPasswordWithReadOnly.Name = "txtHciPasswordWithReadOnly";
            this.txtHciPasswordWithReadOnly.PasswordChar = '*';
            this.txtHciPasswordWithReadOnly.Size = new System.Drawing.Size(191, 21);
            this.txtHciPasswordWithReadOnly.TabIndex = 3;
            this.encryptToolTip.SetToolTip(this.txtHciPasswordWithReadOnly, "Enter hci user with readonly to manage appeals password.");
            this.txtHciPasswordWithReadOnly.UseSystemPasswordChar = true;
            // 
            // txtHciUserWithReadOnly
            // 
            this.txtHciUserWithReadOnly.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtHciUserWithReadOnly.Location = new System.Drawing.Point(178, 65);
            this.txtHciUserWithReadOnly.Name = "txtHciUserWithReadOnly";
            this.txtHciUserWithReadOnly.Size = new System.Drawing.Size(190, 21);
            this.txtHciUserWithReadOnly.TabIndex = 2;
            this.encryptToolTip.SetToolTip(this.txtHciUserWithReadOnly, "Enter hci username with readonly to manage appeals.");
            // 
            // txtHciPasswordWithReadWrite
            // 
            this.txtHciPasswordWithReadWrite.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtHciPasswordWithReadWrite.Location = new System.Drawing.Point(540, 24);
            this.txtHciPasswordWithReadWrite.Name = "txtHciPasswordWithReadWrite";
            this.txtHciPasswordWithReadWrite.PasswordChar = '*';
            this.txtHciPasswordWithReadWrite.Size = new System.Drawing.Size(191, 21);
            this.txtHciPasswordWithReadWrite.TabIndex = 1;
            this.encryptToolTip.SetToolTip(this.txtHciPasswordWithReadWrite, "Enter hci user with read write to manage appeals password.");
            this.txtHciPasswordWithReadWrite.UseSystemPasswordChar = true;
            // 
            // txtHciUserWithReadWrite
            // 
            this.txtHciUserWithReadWrite.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtHciUserWithReadWrite.Location = new System.Drawing.Point(178, 29);
            this.txtHciUserWithReadWrite.Name = "txtHciUserWithReadWrite";
            this.txtHciUserWithReadWrite.Size = new System.Drawing.Size(190, 21);
            this.txtHciUserWithReadWrite.TabIndex = 0;
            this.encryptToolTip.SetToolTip(this.txtHciUserWithReadWrite, "Enter hci username with read write to manage appeals.");
            // 
            // grpDefaultUser
            // 
            this.grpDefaultUser.Controls.Add(this.lblHciAdminUserName);
            this.grpDefaultUser.Controls.Add(this.txtHciAdminUserName);
            this.grpDefaultUser.Controls.Add(this.lblHciAdminPassword);
            this.grpDefaultUser.Controls.Add(this.lblHciUsername);
            this.grpDefaultUser.Controls.Add(this.txtHciPassword);
            this.grpDefaultUser.Controls.Add(this.txtHciAdminPassword);
            this.grpDefaultUser.Controls.Add(this.txtHciUserName);
            this.grpDefaultUser.Controls.Add(this.txtClientPassword);
            this.grpDefaultUser.Controls.Add(this.lblHciPassword);
            this.grpDefaultUser.Controls.Add(this.lblClientPassword);
            this.grpDefaultUser.Controls.Add(this.lblClientUserName);
            this.grpDefaultUser.Controls.Add(this.txtClientUserName);
            this.grpDefaultUser.Location = new System.Drawing.Point(7, 70);
            this.grpDefaultUser.Name = "grpDefaultUser";
            this.grpDefaultUser.Size = new System.Drawing.Size(750, 129);
            this.grpDefaultUser.TabIndex = 37;
            this.grpDefaultUser.TabStop = false;
            this.grpDefaultUser.Text = "Default Users";
            // 
            // lblHciAdminPassword
            // 
            this.lblHciAdminPassword.AutoSize = true;
            this.lblHciAdminPassword.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHciAdminPassword.Location = new System.Drawing.Point(413, 20);
            this.lblHciAdminPassword.Name = "lblHciAdminPassword";
            this.lblHciAdminPassword.Size = new System.Drawing.Size(70, 13);
            this.lblHciAdminPassword.TabIndex = 23;
            this.lblHciAdminPassword.Text = "Password :";
            this.lblHciAdminPassword.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblHciUsername
            // 
            this.lblHciUsername.AutoSize = true;
            this.lblHciUsername.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHciUsername.Location = new System.Drawing.Point(6, 51);
            this.lblHciUsername.Name = "lblHciUsername";
            this.lblHciUsername.Size = new System.Drawing.Size(100, 13);
            this.lblHciUsername.TabIndex = 24;
            this.lblHciUsername.Text = "HCI Username :";
            this.lblHciUsername.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtHciPassword
            // 
            this.txtHciPassword.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtHciPassword.Location = new System.Drawing.Point(544, 48);
            this.txtHciPassword.Name = "txtHciPassword";
            this.txtHciPassword.PasswordChar = '*';
            this.txtHciPassword.Size = new System.Drawing.Size(191, 21);
            this.txtHciPassword.TabIndex = 3;
            this.encryptToolTip.SetToolTip(this.txtHciPassword, "Enter hci password.");
            this.txtHciPassword.UseSystemPasswordChar = true;
            // 
            // txtHciAdminPassword
            // 
            this.txtHciAdminPassword.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtHciAdminPassword.Location = new System.Drawing.Point(544, 17);
            this.txtHciAdminPassword.Name = "txtHciAdminPassword";
            this.txtHciAdminPassword.PasswordChar = '*';
            this.txtHciAdminPassword.Size = new System.Drawing.Size(191, 21);
            this.txtHciAdminPassword.TabIndex = 1;
            this.encryptToolTip.SetToolTip(this.txtHciAdminPassword, "Enter hci admin password.");
            this.txtHciAdminPassword.UseSystemPasswordChar = true;
            // 
            // txtHciUserName
            // 
            this.txtHciUserName.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtHciUserName.Location = new System.Drawing.Point(181, 51);
            this.txtHciUserName.Name = "txtHciUserName";
            this.txtHciUserName.Size = new System.Drawing.Size(191, 21);
            this.txtHciUserName.TabIndex = 2;
            this.encryptToolTip.SetToolTip(this.txtHciUserName, "Enter hci username.");
            // 
            // txtClientPassword
            // 
            this.txtClientPassword.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtClientPassword.Location = new System.Drawing.Point(544, 85);
            this.txtClientPassword.Name = "txtClientPassword";
            this.txtClientPassword.PasswordChar = '*';
            this.txtClientPassword.Size = new System.Drawing.Size(191, 21);
            this.txtClientPassword.TabIndex = 5;
            this.encryptToolTip.SetToolTip(this.txtClientPassword, "Enter client password.");
            this.txtClientPassword.UseSystemPasswordChar = true;
            // 
            // lblHciPassword
            // 
            this.lblHciPassword.AutoSize = true;
            this.lblHciPassword.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHciPassword.Location = new System.Drawing.Point(413, 51);
            this.lblHciPassword.Name = "lblHciPassword";
            this.lblHciPassword.Size = new System.Drawing.Size(70, 13);
            this.lblHciPassword.TabIndex = 25;
            this.lblHciPassword.Text = "Password :";
            this.lblHciPassword.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblClientPassword
            // 
            this.lblClientPassword.AutoSize = true;
            this.lblClientPassword.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblClientPassword.Location = new System.Drawing.Point(413, 88);
            this.lblClientPassword.Name = "lblClientPassword";
            this.lblClientPassword.Size = new System.Drawing.Size(70, 13);
            this.lblClientPassword.TabIndex = 27;
            this.lblClientPassword.Text = "Password :";
            this.lblClientPassword.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblClientUserName
            // 
            this.lblClientUserName.AutoSize = true;
            this.lblClientUserName.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblClientUserName.Location = new System.Drawing.Point(5, 88);
            this.lblClientUserName.Name = "lblClientUserName";
            this.lblClientUserName.Size = new System.Drawing.Size(111, 13);
            this.lblClientUserName.TabIndex = 26;
            this.lblClientUserName.Text = "Client Username :";
            this.lblClientUserName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtClientUserName
            // 
            this.txtClientUserName.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtClientUserName.Location = new System.Drawing.Point(182, 88);
            this.txtClientUserName.Name = "txtClientUserName";
            this.txtClientUserName.Size = new System.Drawing.Size(191, 21);
            this.txtClientUserName.TabIndex = 4;
            this.encryptToolTip.SetToolTip(this.txtClientUserName, "Enter client username.");
            // 
            // grpUrlAndBrowser
            // 
            this.grpUrlAndBrowser.Controls.Add(this.lblApplicationUrl);
            this.grpUrlAndBrowser.Controls.Add(this.txtApplicationUrl);
            this.grpUrlAndBrowser.Controls.Add(this.cmbWebBrowserDriver);
            this.grpUrlAndBrowser.Controls.Add(this.lblWebBrowserDriver);
            this.grpUrlAndBrowser.Location = new System.Drawing.Point(7, 16);
            this.grpUrlAndBrowser.Name = "grpUrlAndBrowser";
            this.grpUrlAndBrowser.Size = new System.Drawing.Size(750, 45);
            this.grpUrlAndBrowser.TabIndex = 36;
            this.grpUrlAndBrowser.TabStop = false;
            this.grpUrlAndBrowser.Text = "Url And Browser";
            // 
            // lblApplicationUrl
            // 
            this.lblApplicationUrl.AutoSize = true;
            this.lblApplicationUrl.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblApplicationUrl.Location = new System.Drawing.Point(8, 21);
            this.lblApplicationUrl.Name = "lblApplicationUrl";
            this.lblApplicationUrl.Size = new System.Drawing.Size(98, 13);
            this.lblApplicationUrl.TabIndex = 20;
            this.lblApplicationUrl.Text = "Application Url :";
            this.lblApplicationUrl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtApplicationUrl
            // 
            this.txtApplicationUrl.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtApplicationUrl.Location = new System.Drawing.Point(112, 18);
            this.txtApplicationUrl.Name = "txtApplicationUrl";
            this.txtApplicationUrl.Size = new System.Drawing.Size(278, 21);
            this.txtApplicationUrl.TabIndex = 1;
            this.encryptToolTip.SetToolTip(this.txtApplicationUrl, "Enter application url of an environment.");
            // 
            // cmbWebBrowserDriver
            // 
            this.cmbWebBrowserDriver.FormattingEnabled = true;
            this.cmbWebBrowserDriver.Location = new System.Drawing.Point(544, 18);
            this.cmbWebBrowserDriver.Name = "cmbWebBrowserDriver";
            this.cmbWebBrowserDriver.Size = new System.Drawing.Size(191, 21);
            this.cmbWebBrowserDriver.TabIndex = 2;
            this.encryptToolTip.SetToolTip(this.cmbWebBrowserDriver, "Select web browser driver - Chrome, Internet Explorer and Firefox.");
            // 
            // lblWebBrowserDriver
            // 
            this.lblWebBrowserDriver.AutoSize = true;
            this.lblWebBrowserDriver.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWebBrowserDriver.Location = new System.Drawing.Point(417, 21);
            this.lblWebBrowserDriver.Name = "lblWebBrowserDriver";
            this.lblWebBrowserDriver.Size = new System.Drawing.Size(128, 13);
            this.lblWebBrowserDriver.TabIndex = 21;
            this.lblWebBrowserDriver.Text = "WebBrowser Driver :";
            this.lblWebBrowserDriver.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblEnvironmentVariables
            // 
            this.lblEnvironmentVariables.AutoSize = true;
            this.lblEnvironmentVariables.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEnvironmentVariables.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.lblEnvironmentVariables.Location = new System.Drawing.Point(8, 78);
            this.lblEnvironmentVariables.Name = "lblEnvironmentVariables";
            this.lblEnvironmentVariables.Size = new System.Drawing.Size(155, 13);
            this.lblEnvironmentVariables.TabIndex = 1;
            this.lblEnvironmentVariables.Text = "Environment Variables";
            // 
            // lblUserType
            // 
            this.lblUserType.AutoSize = true;
            this.lblUserType.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUserType.Location = new System.Drawing.Point(17, 29);
            this.lblUserType.Name = "lblUserType";
            this.lblUserType.Size = new System.Drawing.Size(70, 13);
            this.lblUserType.TabIndex = 28;
            this.lblUserType.Text = "UserType :";
            this.lblUserType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblIsInvoicePresent
            // 
            this.lblIsInvoicePresent.AutoSize = true;
            this.lblIsInvoicePresent.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblIsInvoicePresent.Location = new System.Drawing.Point(17, 89);
            this.lblIsInvoicePresent.Name = "lblIsInvoicePresent";
            this.lblIsInvoicePresent.Size = new System.Drawing.Size(120, 13);
            this.lblIsInvoicePresent.TabIndex = 30;
            this.lblIsInvoicePresent.Text = "Is Invoice Present :";
            this.lblIsInvoicePresent.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // chkIsInvoicePresent
            // 
            this.chkIsInvoicePresent.AutoSize = true;
            this.chkIsInvoicePresent.Location = new System.Drawing.Point(163, 88);
            this.chkIsInvoicePresent.Name = "chkIsInvoicePresent";
            this.chkIsInvoicePresent.Size = new System.Drawing.Size(15, 14);
            this.chkIsInvoicePresent.TabIndex = 2;
            this.encryptToolTip.SetToolTip(this.chkIsInvoicePresent, "Check mark for invoice is present and uncheck for invoice is not present.");
            this.chkIsInvoicePresent.UseVisualStyleBackColor = true;
            // 
            // lblIsHciUserAuthorizedToManagerAppeals
            // 
            this.lblIsHciUserAuthorizedToManagerAppeals.AutoSize = true;
            this.lblIsHciUserAuthorizedToManagerAppeals.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblIsHciUserAuthorizedToManagerAppeals.Location = new System.Drawing.Point(17, 112);
            this.lblIsHciUserAuthorizedToManagerAppeals.Name = "lblIsHciUserAuthorizedToManagerAppeals";
            this.lblIsHciUserAuthorizedToManagerAppeals.Size = new System.Drawing.Size(263, 13);
            this.lblIsHciUserAuthorizedToManagerAppeals.TabIndex = 31;
            this.lblIsHciUserAuthorizedToManagerAppeals.Text = "Is HCI User Authorized To Manage Appeals :";
            this.lblIsHciUserAuthorizedToManagerAppeals.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // chkIsHciUserAuthorizedToManageAppeals
            // 
            this.chkIsHciUserAuthorizedToManageAppeals.AutoSize = true;
            this.chkIsHciUserAuthorizedToManageAppeals.Location = new System.Drawing.Point(297, 112);
            this.chkIsHciUserAuthorizedToManageAppeals.Name = "chkIsHciUserAuthorizedToManageAppeals";
            this.chkIsHciUserAuthorizedToManageAppeals.Size = new System.Drawing.Size(15, 14);
            this.chkIsHciUserAuthorizedToManageAppeals.TabIndex = 3;
            this.encryptToolTip.SetToolTip(this.chkIsHciUserAuthorizedToManageAppeals, "Check mark for authorized and uncheck for not authorized hci user to manage appea" +
        "ls.");
            this.chkIsHciUserAuthorizedToManageAppeals.UseVisualStyleBackColor = true;
            // 
            // lblIsClientUserAuthorizedToManageAppeals
            // 
            this.lblIsClientUserAuthorizedToManageAppeals.AutoSize = true;
            this.lblIsClientUserAuthorizedToManageAppeals.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblIsClientUserAuthorizedToManageAppeals.Location = new System.Drawing.Point(17, 136);
            this.lblIsClientUserAuthorizedToManageAppeals.Name = "lblIsClientUserAuthorizedToManageAppeals";
            this.lblIsClientUserAuthorizedToManageAppeals.Size = new System.Drawing.Size(274, 13);
            this.lblIsClientUserAuthorizedToManageAppeals.TabIndex = 32;
            this.lblIsClientUserAuthorizedToManageAppeals.Text = "Is Client User Authorized To Manage Appeals :";
            this.lblIsClientUserAuthorizedToManageAppeals.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // chkIsClientUserAuthorizedToManageAppeals
            // 
            this.chkIsClientUserAuthorizedToManageAppeals.AutoSize = true;
            this.chkIsClientUserAuthorizedToManageAppeals.Location = new System.Drawing.Point(297, 136);
            this.chkIsClientUserAuthorizedToManageAppeals.Name = "chkIsClientUserAuthorizedToManageAppeals";
            this.chkIsClientUserAuthorizedToManageAppeals.Size = new System.Drawing.Size(15, 14);
            this.chkIsClientUserAuthorizedToManageAppeals.TabIndex = 4;
            this.encryptToolTip.SetToolTip(this.chkIsClientUserAuthorizedToManageAppeals, "Check mark for authorized and uncheck for not authorized client user to manage ap" +
        "peals.");
            this.chkIsClientUserAuthorizedToManageAppeals.UseVisualStyleBackColor = true;
            // 
            // txtDataSource
            // 
            this.txtDataSource.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDataSource.Location = new System.Drawing.Point(153, 35);
            this.txtDataSource.Name = "txtDataSource";
            this.txtDataSource.Size = new System.Drawing.Size(191, 21);
            this.txtDataSource.TabIndex = 0;
            this.encryptToolTip.SetToolTip(this.txtDataSource, "Enter data source for connectionstring.");
            // 
            // lblTestClient
            // 
            this.lblTestClient.AutoSize = true;
            this.lblTestClient.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTestClient.Location = new System.Drawing.Point(17, 60);
            this.lblTestClient.Name = "lblTestClient";
            this.lblTestClient.Size = new System.Drawing.Size(73, 13);
            this.lblTestClient.TabIndex = 29;
            this.lblTestClient.Text = "TestClient :";
            this.lblTestClient.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cmbTestClient
            // 
            this.cmbTestClient.FormattingEnabled = true;
            this.cmbTestClient.Location = new System.Drawing.Point(164, 57);
            this.cmbTestClient.Name = "cmbTestClient";
            this.cmbTestClient.Size = new System.Drawing.Size(191, 21);
            this.cmbTestClient.TabIndex = 1;
            this.encryptToolTip.SetToolTip(this.cmbTestClient, "Select a test client to run ui automation test on.");
            // 
            // cmbUserType
            // 
            this.cmbUserType.FormattingEnabled = true;
            this.cmbUserType.Location = new System.Drawing.Point(164, 26);
            this.cmbUserType.Name = "cmbUserType";
            this.cmbUserType.Size = new System.Drawing.Size(191, 21);
            this.cmbUserType.TabIndex = 0;
            this.encryptToolTip.SetToolTip(this.cmbUserType, "Select HCIAdmin for hci admin user, HCINormal for hci user and Client for client " +
        "user.");
            // 
            // grpConnectionString
            // 
            this.grpConnectionString.Controls.Add(this.lblPassword);
            this.grpConnectionString.Controls.Add(this.lblUserId);
            this.grpConnectionString.Controls.Add(this.lblDataSource);
            this.grpConnectionString.Controls.Add(this.txtPassword);
            this.grpConnectionString.Controls.Add(this.txtUserId);
            this.grpConnectionString.Controls.Add(this.txtDataSource);
            this.grpConnectionString.Location = new System.Drawing.Point(410, 431);
            this.grpConnectionString.Name = "grpConnectionString";
            this.grpConnectionString.Size = new System.Drawing.Size(359, 163);
            this.grpConnectionString.TabIndex = 5;
            this.grpConnectionString.TabStop = false;
            this.grpConnectionString.Text = "ConnectionString";
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPassword.Location = new System.Drawing.Point(22, 121);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(70, 13);
            this.lblPassword.TabIndex = 28;
            this.lblPassword.Text = "Password :";
            this.lblPassword.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblUserId
            // 
            this.lblUserId.AutoSize = true;
            this.lblUserId.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUserId.Location = new System.Drawing.Point(22, 77);
            this.lblUserId.Name = "lblUserId";
            this.lblUserId.Size = new System.Drawing.Size(58, 13);
            this.lblUserId.TabIndex = 28;
            this.lblUserId.Text = "User Id :";
            this.lblUserId.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblDataSource
            // 
            this.lblDataSource.AutoSize = true;
            this.lblDataSource.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDataSource.Location = new System.Drawing.Point(22, 38);
            this.lblDataSource.Name = "lblDataSource";
            this.lblDataSource.Size = new System.Drawing.Size(87, 13);
            this.lblDataSource.TabIndex = 28;
            this.lblDataSource.Text = "Data Source :";
            this.lblDataSource.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtPassword
            // 
            this.txtPassword.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPassword.Location = new System.Drawing.Point(153, 118);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(191, 21);
            this.txtPassword.TabIndex = 2;
            this.encryptToolTip.SetToolTip(this.txtPassword, "Enter password for connectionstring.");
            this.txtPassword.UseSystemPasswordChar = true;
            // 
            // txtUserId
            // 
            this.txtUserId.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtUserId.Location = new System.Drawing.Point(153, 74);
            this.txtUserId.Name = "txtUserId";
            this.txtUserId.Size = new System.Drawing.Size(191, 21);
            this.txtUserId.TabIndex = 1;
            this.encryptToolTip.SetToolTip(this.txtUserId, "Enter user id for connectionstring.");
            // 
            // encryptToolTip
            // 
            this.encryptToolTip.IsBalloon = true;
            this.encryptToolTip.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.encryptToolTip.ToolTipTitle = "Test Environment Parameters Input Form";
            // 
            // grpUserTypeAndClient
            // 
            this.grpUserTypeAndClient.Controls.Add(this.cmbUserType);
            this.grpUserTypeAndClient.Controls.Add(this.lblIsClientUserAuthorizedToManageAppeals);
            this.grpUserTypeAndClient.Controls.Add(this.cmbTestClient);
            this.grpUserTypeAndClient.Controls.Add(this.lblUserType);
            this.grpUserTypeAndClient.Controls.Add(this.chkIsClientUserAuthorizedToManageAppeals);
            this.grpUserTypeAndClient.Controls.Add(this.lblIsInvoicePresent);
            this.grpUserTypeAndClient.Controls.Add(this.chkIsHciUserAuthorizedToManageAppeals);
            this.grpUserTypeAndClient.Controls.Add(this.lblTestClient);
            this.grpUserTypeAndClient.Controls.Add(this.chkIsInvoicePresent);
            this.grpUserTypeAndClient.Controls.Add(this.lblIsHciUserAuthorizedToManagerAppeals);
            this.grpUserTypeAndClient.Location = new System.Drawing.Point(23, 431);
            this.grpUserTypeAndClient.Name = "grpUserTypeAndClient";
            this.grpUserTypeAndClient.Size = new System.Drawing.Size(381, 163);
            this.grpUserTypeAndClient.TabIndex = 4;
            this.grpUserTypeAndClient.TabStop = false;
            this.grpUserTypeAndClient.Text = "UserType And TestClient";
            // 
            // MainForm
            // 
            this.AcceptButton = this.LoadObjectButton;
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
            this.CancelButton = this.ExitButton;
            this.ClientSize = new System.Drawing.Size(787, 667);
            this.Controls.Add(this.lblEnvironmentVariables);
            this.Controls.Add(this.OptionsGroupBox);
            this.Controls.Add(this.grpConnectionString);
            this.Controls.Add(this.BannerPanel);
            this.Controls.Add(this.grpUserTypeAndClient);
            this.Controls.Add(this.EnvironmentVariablesGroupBox);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Test Environment Parameters Input Form";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.BannerPanel.ResumeLayout(false);
            this.BannerPanel.PerformLayout();
            this.OptionsGroupBox.ResumeLayout(false);
            this.EnvironmentVariablesGroupBox.ResumeLayout(false);
            this.grpManageAppealUsers.ResumeLayout(false);
            this.grpManageAppealUsers.PerformLayout();
            this.grpDefaultUser.ResumeLayout(false);
            this.grpDefaultUser.PerformLayout();
            this.grpUrlAndBrowser.ResumeLayout(false);
            this.grpUrlAndBrowser.PerformLayout();
            this.grpConnectionString.ResumeLayout(false);
            this.grpConnectionString.PerformLayout();
            this.grpUserTypeAndClient.ResumeLayout(false);
            this.grpUserTypeAndClient.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private void MainForm_Load(object sender, EventArgs e)
        {
            _encryptionKey = Program.EncryptionKey;
            Load_ComboBox(cmbWebBrowserDriver, _webBrowserDriver);
            Load_ComboBox(cmbUserType, _userType);
            Load_ComboBox(cmbTestClient, _testClient);

            // Load the customer object from the existing XML file (if any)...)
            if (File.Exists(XML_FILE_NAME))
                LoadObjectButton_Click(this, null);
        }

        private void Load_ComboBox(ComboBox cmbBox, object dataSource)
        {
            cmbBox.DataSource = new BindingSource(dataSource, null);
            cmbBox.DisplayMember = "Key";
            cmbBox.ValueMember = "Value";
        }

        private void LoadObjectButton_Click(object sender, System.EventArgs e)
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
                    Tests tests = XmlHelper<Tests>.Load(XML_FILE_NAME);

                    if (tests == null)
                        MessageBox.Show("Unable to load xml object from file '" + XML_FILE_NAME + "'!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else  // Load customer properties into the form...
                        this.LoadEnvironmentVariablesIntoForm(tests);
                }
                catch (Exception)
                {
                    MessageBox.Show("Loaded file '" + XML_FILE_NAME + "' is not appropriate!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
                MessageBox.Show(this.CreateFileDoesNotExistMsg(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void LoadEnvironmentVariablesIntoForm(Tests tests)
        {
            IDictionary<string, string> evironmentVariables = tests.EnvironmentVariables.Parameters.ToDictionary(x => x.Name, y => y.Value);

            // Load environment variables in the properties of EnvironmentParameters class ...
            EnvironmentParameters.ApplicationUrl = evironmentVariables["ApplicationUrl"] ?? string.Empty;
            EnvironmentParameters.WebBrowserDriver = evironmentVariables["WebBrowserDriver"].ToLower();
            EnvironmentParameters.HciAdminUsername = evironmentVariables["HCIAdminUsername"] ?? string.Empty;
            EnvironmentParameters.HciAdminPassword = evironmentVariables["HCIAdminPassword"] ?? string.Empty;
            EnvironmentParameters.HciUsername = evironmentVariables["HCIUsername"] ?? string.Empty;
            EnvironmentParameters.HciPassword = evironmentVariables["HCIPassword"] ?? string.Empty;
            EnvironmentParameters.ClientUserName = evironmentVariables["ClientUserName"] ?? string.Empty;
            EnvironmentParameters.ClientPassword = evironmentVariables["ClientPassword"] ?? string.Empty;
            EnvironmentParameters.HciUserWithReadWriteManageAppeals = evironmentVariables["HCIUserWithReadWriteManageAppeals"] ?? string.Empty;
            EnvironmentParameters.HciUserWithReadWriteManageAppealsPassword = evironmentVariables["HCIUserWithReadWriteManageAppealsPassword"] ?? string.Empty;
            EnvironmentParameters.HciUserWithReadOnlyManageAppeals = evironmentVariables["HCIUserWithReadOnlyManageAppeals"] ?? string.Empty;
            EnvironmentParameters.HciUserWithReadOnlyManageAppealsPassword = evironmentVariables["HCIUserWithReadOnlyManageAppealsPassword"] ?? string.Empty;
            EnvironmentParameters.ClientUserWithReadWriteManageAppeals = evironmentVariables["ClientUserWithReadWriteManageAppeals"] ?? string.Empty;
            EnvironmentParameters.ClientUserWithReadWriteManageAppealsPassword = evironmentVariables["ClientUserWithReadWriteManageAppealsPassword"] ?? string.Empty;
            EnvironmentParameters.UserType = evironmentVariables["UserType"].ToUpper();
            EnvironmentParameters.TestClient = evironmentVariables["TestClient"].ToUpper();
            EnvironmentParameters.IsInvoicePresent = evironmentVariables["IsInvoicePresent"];
            EnvironmentParameters.IsHciUserAuthorizedToManageAppeals = evironmentVariables["IsHCIUserAuthorizedToManageAppeals"];
            EnvironmentParameters.IsClientUserAuthorizedToManageAppeals = evironmentVariables["IsClientUserAuthorizedToManageAppeals"];
            EnvironmentParameters.ConnectionString = evironmentVariables["ConnectionString"] ?? string.Empty;
            EnvironmentParameters.EncryptCredentials = Convert.ToBoolean(evironmentVariables["EncryptCredentials"]);

            //Check environment variables are encrypted in xml file ...
            if (!string.IsNullOrEmpty(_encryptionKey) && EnvironmentParameters.EncryptCredentials)
            {
                //Decrypt environment variables 
                EnvironmentParameters.HciAdminUsername = String.IsNullOrEmpty(EnvironmentParameters.HciAdminUsername) ? string.Empty : DecryptString(EnvironmentParameters.HciAdminUsername);
                EnvironmentParameters.HciAdminPassword = String.IsNullOrEmpty(EnvironmentParameters.HciAdminPassword) ? string.Empty : DecryptString(EnvironmentParameters.HciAdminPassword);
                EnvironmentParameters.HciUsername = String.IsNullOrEmpty(EnvironmentParameters.HciUsername) ? string.Empty : DecryptString(EnvironmentParameters.HciUsername);
                EnvironmentParameters.HciPassword = String.IsNullOrEmpty(EnvironmentParameters.HciPassword) ? string.Empty : DecryptString(EnvironmentParameters.HciPassword);
                EnvironmentParameters.ClientUserName = String.IsNullOrEmpty(EnvironmentParameters.ClientUserName) ? string.Empty : DecryptString(EnvironmentParameters.ClientUserName);
                EnvironmentParameters.ClientPassword = String.IsNullOrEmpty(EnvironmentParameters.ClientPassword) ? string.Empty : DecryptString(EnvironmentParameters.ClientPassword);
                EnvironmentParameters.HciUserWithReadWriteManageAppeals = String.IsNullOrEmpty(EnvironmentParameters.HciUserWithReadWriteManageAppeals) ? string.Empty : DecryptString(EnvironmentParameters.HciUserWithReadWriteManageAppeals);
                EnvironmentParameters.HciUserWithReadWriteManageAppealsPassword = String.IsNullOrEmpty(EnvironmentParameters.HciUserWithReadWriteManageAppealsPassword) ? string.Empty : DecryptString(EnvironmentParameters.HciUserWithReadWriteManageAppealsPassword);
                EnvironmentParameters.HciUserWithReadOnlyManageAppeals = String.IsNullOrEmpty(EnvironmentParameters.HciUserWithReadOnlyManageAppeals) ? string.Empty : DecryptString(EnvironmentParameters.HciUserWithReadOnlyManageAppeals);
                EnvironmentParameters.HciUserWithReadOnlyManageAppealsPassword = String.IsNullOrEmpty(EnvironmentParameters.HciUserWithReadOnlyManageAppealsPassword) ? string.Empty : DecryptString(EnvironmentParameters.HciUserWithReadOnlyManageAppealsPassword);
                EnvironmentParameters.ClientUserWithReadWriteManageAppeals = String.IsNullOrEmpty(EnvironmentParameters.ClientUserWithReadWriteManageAppeals) ? string.Empty : DecryptString(EnvironmentParameters.ClientUserWithReadWriteManageAppeals);
                EnvironmentParameters.ClientUserWithReadWriteManageAppealsPassword = String.IsNullOrEmpty(EnvironmentParameters.ClientUserWithReadWriteManageAppealsPassword) ? string.Empty : DecryptString(EnvironmentParameters.ClientUserWithReadWriteManageAppealsPassword);
                EnvironmentParameters.ConnectionString = String.IsNullOrEmpty(EnvironmentParameters.ConnectionString) ? string.Empty : DecryptString(EnvironmentParameters.ConnectionString);
            }

            //Split DataSource, UserId and Password of ConnectionString ...
            LoadConnectionString();

            //Load environment variables in form values ...
            txtApplicationUrl.Text = EnvironmentParameters.ApplicationUrl;
            cmbWebBrowserDriver.SelectedValue = EnvironmentParameters.WebBrowserDriver ?? _webBrowserDriver.FirstOrDefault().Value;
            txtHciAdminUserName.Text = EnvironmentParameters.HciAdminUsername;
            txtHciAdminPassword.Text = EnvironmentParameters.HciAdminPassword;
            txtHciUserName.Text = EnvironmentParameters.HciUsername;
            txtHciPassword.Text = EnvironmentParameters.HciPassword;
            txtClientUserName.Text = EnvironmentParameters.ClientUserName;
            txtClientPassword.Text = EnvironmentParameters.ClientPassword;
            txtHciUserWithReadWrite.Text = EnvironmentParameters.HciUserWithReadWriteManageAppeals;
            txtHciPasswordWithReadWrite.Text = EnvironmentParameters.HciUserWithReadWriteManageAppealsPassword;
            txtHciUserWithReadOnly.Text = EnvironmentParameters.HciUserWithReadOnlyManageAppeals;
            txtHciPasswordWithReadOnly.Text = EnvironmentParameters.HciUserWithReadOnlyManageAppealsPassword;
            txtClientUserWithReadWrite.Text = EnvironmentParameters.ClientUserWithReadWriteManageAppeals;
            txtClientPasswordWithReadWrite.Text = EnvironmentParameters.ClientUserWithReadWriteManageAppealsPassword;
            cmbUserType.SelectedValue = EnvironmentParameters.UserType ?? _userType.FirstOrDefault().Value;
            cmbTestClient.SelectedValue = EnvironmentParameters.TestClient ?? _testClient.FirstOrDefault().Value;
            chkIsInvoicePresent.Checked = Convert.ToBoolean(EnvironmentParameters.IsInvoicePresent);
            chkIsHciUserAuthorizedToManageAppeals.Checked = Convert.ToBoolean(EnvironmentParameters.IsHciUserAuthorizedToManageAppeals);
            chkIsClientUserAuthorizedToManageAppeals.Checked = Convert.ToBoolean(EnvironmentParameters.IsClientUserAuthorizedToManageAppeals);
            txtDataSource.Text = _dataSource ?? string.Empty;
            txtUserId.Text = _userId ?? string.Empty;
            txtPassword.Text = _password ?? string.Empty;
        }

        private string DecryptString(string message)
        {
            return EncryptionAlgorithm.DecryptString(message, _encryptionKey);
        }

        private void SaveObjectButton_Click(object sender, EventArgs e)
        {
            if (this.EnvironmentVariableIsValid())
            {
                // Create environment variables based on Form values.
                Tests tests = this.CreateEnvironmentVariables();

                //Save environment variables to XML file using our XmlHelper class...
                try
                {
                    if (MessageBox.Show("Do you want to save ?", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        XmlHelper<Tests>.Save(tests, XML_FILE_NAME);
                        MessageBox.Show("Updated Successfully.", this.Text, MessageBoxButtons.OK,MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unable to save xml object!" + Environment.NewLine + Environment.NewLine + ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private Tests CreateEnvironmentVariables()
        {
            try
            {
                Tests tests = XmlHelper<Tests>.Load(XML_FILE_NAME);
                Param[] param = tests.EnvironmentVariables.Parameters;

                int indexOfApplicationUrl = IndexOf(param, "ApplicationUrl");
                int indexOfWebBrowserDriver = IndexOf(param, "WebBrowserDriver");
                int indexOfHciAdminUserName = IndexOf(param, "HCIAdminUsername");
                int indexOfHciAdminPassword = IndexOf(param, "HCIAdminPassword");
                int indexOfHciUserName = IndexOf(param, "HCIUsername");
                int indexOfHciPassword = IndexOf(param, "HCIPassword");
                int indexOfClientUserName = IndexOf(param, "ClientUserName");
                int indexOfClientPassword = IndexOf(param, "ClientPassword");
                int indexOfHciUserWithReadWriteManageAppeals = IndexOf(param, "HCIUserWithReadWriteManageAppeals");
                int indexOfHciUserWithReadWriteManageAppealsPassword = IndexOf(param, "HCIUserWithReadWriteManageAppealsPassword");
                int indexOfHciUserWithReadOnlyManageAppeals = IndexOf(param, "HCIUserWithReadOnlyManageAppeals");
                int indexOfHciUserWithReadOnlyManageAppealsPassword = IndexOf(param, "HCIUserWithReadOnlyManageAppealsPassword");
                int indexOfClientUserWithReadWriteManageAppeals = IndexOf(param, "ClientUserWithReadWriteManageAppeals");
                int indexOfClientUserWithReadWriteManageAppealsPassword = IndexOf(param, "ClientUserWithReadWriteManageAppealsPassword");
                int indexOfUserType = IndexOf(param, "UserType");
                int indexOfTestClient = IndexOf(param, "TestClient");
                int indexOfIsInvoicePresent = IndexOf(param, "IsInvoicePresent");
                int indexOfIsHciUserAuthorizedToManageAppeals = IndexOf(param, "IsHCIUserAuthorizedToManageAppeals");
                int indexOfIsClientUserAuthorizedToManagerAppeals = IndexOf(param, "IsClientUserAuthorizedToManageAppeals");
                int indexOfConnectionString = IndexOf(param, "ConnectionString");
                int indexOfIsEncrypted = IndexOf(param, "EncryptCredentials");

                //Create connectionstring from datasource, userid and password ...
                CreateConnectionString();

                if (!string.IsNullOrEmpty(_encryptionKey))
                {
                    param[indexOfHciAdminUserName].Value = EncryptData(txtHciAdminUserName.Text.Trim());
                    param[indexOfHciAdminPassword].Value = EncryptData(txtHciAdminPassword.Text.Trim());
                    param[indexOfHciUserName].Value = EncryptData(txtHciUserName.Text.Trim());
                    param[indexOfHciPassword].Value = EncryptData(txtHciPassword.Text.Trim());
                    param[indexOfClientUserName].Value = EncryptData(txtClientUserName.Text.Trim());
                    param[indexOfClientPassword].Value = EncryptData(txtClientPassword.Text.Trim());
                    param[indexOfHciUserWithReadWriteManageAppeals].Value = EncryptData(txtHciUserWithReadWrite.Text.Trim());
                    param[indexOfHciUserWithReadWriteManageAppealsPassword].Value = EncryptData(txtHciPasswordWithReadWrite.Text.Trim());
                    param[indexOfHciUserWithReadOnlyManageAppeals].Value = EncryptData(txtHciUserWithReadOnly.Text.Trim());
                    param[indexOfHciUserWithReadOnlyManageAppealsPassword].Value = EncryptData(txtHciPasswordWithReadOnly.Text.Trim());
                    param[indexOfClientUserWithReadWriteManageAppeals].Value = EncryptData(txtClientUserWithReadWrite.Text.Trim());
                    param[indexOfClientUserWithReadWriteManageAppealsPassword].Value = EncryptData(txtClientPasswordWithReadWrite.Text.Trim());
                    param[indexOfConnectionString].Value = EncryptData(EnvironmentParameters.ConnectionString.Trim());
                    param[indexOfIsEncrypted].Value = true.ToString();

                    param[indexOfApplicationUrl].Value = txtApplicationUrl.Text.Trim();
                    param[indexOfWebBrowserDriver].Value = (string)cmbWebBrowserDriver.SelectedValue;
                    param[indexOfUserType].Value = (string)cmbUserType.SelectedValue;
                    param[indexOfTestClient].Value = (string)cmbTestClient.SelectedValue;
                    param[indexOfIsInvoicePresent].Value = chkIsInvoicePresent.Checked ? "true" : "false";
                    param[indexOfIsHciUserAuthorizedToManageAppeals].Value = chkIsHciUserAuthorizedToManageAppeals.Checked ? "true" : "false";
                    param[indexOfIsClientUserAuthorizedToManagerAppeals].Value = chkIsClientUserAuthorizedToManageAppeals.Checked ? "true" : "false";

                }
                else if (MessageBox.Show(@"Invalid encryption key. Do you want to reenter encryption key again ?", @"Encryptor", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    _environmentSelectorForm.ShowDialog();
                    this.Hide();
                }
                return tests;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return null;
        }

        private string EncryptData(string message)
        {
            return EncryptionAlgorithm.EncryptData(message, _encryptionKey);
        }

        private int IndexOf(Param[] param, string paramName)
        {
            for (int i = 0; i < param.Length; i++)
                if (string.Compare(param[i].Name, paramName, StringComparison.OrdinalIgnoreCase) == 0)
                    return i;
            return -1;
        }

        private bool EnvironmentVariableIsValid()
        {
            bool isValid = txtHciAdminUserName.Text.Length > 0 && txtHciPassword.Text.Length > 0 &&
                           txtHciUserName.Text.Length > 0 && txtHciPassword.Text.Length > 0 &&
                           txtClientUserName.Text.Length > 0 && txtClientPassword.Text.Length > 0 &&
                           txtHciUserWithReadWrite.Text.Length > 0 && txtHciPasswordWithReadWrite.Text.Length > 0 &&
                           txtHciUserWithReadOnly.Text.Length > 0 && txtHciPasswordWithReadOnly.Text.Length > 0 &&
                           txtClientUserWithReadWrite.Text.Length > 0 && txtClientPasswordWithReadWrite.Text.Length > 0 &&
                           txtDataSource.Text.Length > 0 && txtUserId.Text.Length > 0 && txtPassword.Text.Length > 0;
            if (isValid == false)
            {
                MessageBox.Show("You must enter a valid environment variable Name!",
                    this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            return isValid;
        }

        private void ViewXMLFileButton_Click(object sender, System.EventArgs e)
        {
            // View the customer XML file in the default web browser (if any)...
            if (File.Exists(XML_FILE_NAME) == true)
            {
                System.Diagnostics.Process.Start(XML_FILE_NAME);
            }
            else
            {
                MessageBox.Show(this.CreateFileDoesNotExistMsg(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }


        private string CreateFileDoesNotExistMsg()
        {
            return "The XML file " + XML_FILE_NAME + " does not exist.";
        }

        private void LoadConnectionString()
        {
            if (String.IsNullOrEmpty(EnvironmentParameters.ConnectionString))
            {
                _dataSource = _userId = _password = null;
                return;
            }
            string[] connectionString = EnvironmentParameters.ConnectionString.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            var splitByEqual = new char[] { '=' };
            _dataSource = connectionString[0].Split(splitByEqual, StringSplitOptions.RemoveEmptyEntries)[1];
            _userId = connectionString[1].Split(splitByEqual, StringSplitOptions.RemoveEmptyEntries)[1];
            _password = connectionString[2].Split(splitByEqual, StringSplitOptions.RemoveEmptyEntries)[1];
        }

        private void CreateConnectionString()
        {
            EnvironmentParameters.ConnectionString =
                string.Format(
                    "data source{0}" + txtDataSource.Text.Trim() + "{1}user id{0}" + txtUserId.Text.Trim() +
                    "{1}password{0}" + txtPassword.Text.Trim(), "=", ";");
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            var dialogResult = MessageBox.Show(@"Do you want to exit ?", this.Text, MessageBoxButtons.YesNo,
                                               MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
                Application.Exit();
        }

        private void txtFilePath_DoubleClick(object sender, EventArgs e)
        {
            this.LoadObjectButton_Click(sender, e);
        }
    }
}
