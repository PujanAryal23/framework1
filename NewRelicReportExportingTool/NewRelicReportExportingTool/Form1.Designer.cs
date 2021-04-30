namespace NewRelicReportExportingTool
{
    partial class Form1
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
            this.ReportDate = new System.Windows.Forms.ComboBox();
            this.ReportDataGridView = new System.Windows.Forms.DataGridView();
            this.ExportToExcel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.Go = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.ReportTypeCombo = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.EnvironmentCombo = new System.Windows.Forms.ComboBox();
            this.reportValidateLabel = new System.Windows.Forms.Label();
            this.Clear_button = new System.Windows.Forms.Button();
            this.environmentErrorLabel = new System.Windows.Forms.Label();
            this.dateErrorLabel = new System.Windows.Forms.Label();
            this.gridEmptyMessage = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.reportTypeTooltip = new System.Windows.Forms.ToolTip(this.components);
            this.environmentTooltip = new System.Windows.Forms.ToolTip(this.components);
            this.CreateDateTooltip = new System.Windows.Forms.ToolTip(this.components);
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.Start = new System.Windows.Forms.Button();
            this.rbProdError = new System.Windows.Forms.RadioButton();
            this.rbProdTran = new System.Windows.Forms.RadioButton();
            this.rbUATError = new System.Windows.Forms.RadioButton();
            this.rbUATTran = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.dateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.ReportDataGridView)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // ReportDate
            // 
            this.ReportDate.FormattingEnabled = true;
            this.ReportDate.Items.AddRange(new object[] {
            "19-Dec-16",
            "26-Dec-16",
            "29-Dec-16",
            "30-Dec-16"});
            this.ReportDate.Location = new System.Drawing.Point(94, 65);
            this.ReportDate.Name = "ReportDate";
            this.ReportDate.Size = new System.Drawing.Size(121, 21);
            this.ReportDate.TabIndex = 0;
            // 
            // ReportDataGridView
            // 
            this.ReportDataGridView.AllowUserToDeleteRows = false;
            this.ReportDataGridView.AllowUserToOrderColumns = true;
            this.ReportDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ReportDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ReportDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.ReportDataGridView.Location = new System.Drawing.Point(4, 105);
            this.ReportDataGridView.Name = "ReportDataGridView";
            this.ReportDataGridView.Size = new System.Drawing.Size(886, 260);
            this.ReportDataGridView.TabIndex = 1;
            // 
            // ExportToExcel
            // 
            this.ExportToExcel.Location = new System.Drawing.Point(753, 371);
            this.ExportToExcel.Name = "ExportToExcel";
            this.ExportToExcel.Size = new System.Drawing.Size(137, 23);
            this.ExportToExcel.TabIndex = 2;
            this.ExportToExcel.Text = "Export to Excel";
            this.ExportToExcel.UseVisualStyleBackColor = true;
            this.ExportToExcel.Click += new System.EventHandler(this.ExportToExcel_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 72);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Report Date";
            // 
            // Go
            // 
            this.Go.Location = new System.Drawing.Point(248, 65);
            this.Go.Name = "Go";
            this.Go.Size = new System.Drawing.Size(75, 23);
            this.Go.TabIndex = 4;
            this.Go.Text = "Search";
            this.Go.UseVisualStyleBackColor = true;
            this.Go.Click += new System.EventHandler(this.Go_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Report Type";
            // 
            // ReportTypeCombo
            // 
            this.ReportTypeCombo.AllowDrop = true;
            this.ReportTypeCombo.FormattingEnabled = true;
            this.ReportTypeCombo.Location = new System.Drawing.Point(94, 16);
            this.ReportTypeCombo.Name = "ReportTypeCombo";
            this.ReportTypeCombo.Size = new System.Drawing.Size(121, 21);
            this.ReportTypeCombo.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 45);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Environment";
            // 
            // EnvironmentCombo
            // 
            this.EnvironmentCombo.AllowDrop = true;
            this.EnvironmentCombo.FormattingEnabled = true;
            this.EnvironmentCombo.Location = new System.Drawing.Point(94, 41);
            this.EnvironmentCombo.Name = "EnvironmentCombo";
            this.EnvironmentCombo.Size = new System.Drawing.Size(121, 21);
            this.EnvironmentCombo.TabIndex = 8;
            // 
            // reportValidateLabel
            // 
            this.reportValidateLabel.AutoSize = true;
            this.reportValidateLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.reportValidateLabel.ForeColor = System.Drawing.Color.Red;
            this.reportValidateLabel.Location = new System.Drawing.Point(224, 21);
            this.reportValidateLabel.Name = "reportValidateLabel";
            this.reportValidateLabel.Size = new System.Drawing.Size(11, 13);
            this.reportValidateLabel.TabIndex = 9;
            this.reportValidateLabel.Text = "!";
            // 
            // Clear_button
            // 
            this.Clear_button.Location = new System.Drawing.Point(329, 65);
            this.Clear_button.Name = "Clear_button";
            this.Clear_button.Size = new System.Drawing.Size(75, 23);
            this.Clear_button.TabIndex = 10;
            this.Clear_button.Text = "Clear";
            this.Clear_button.UseVisualStyleBackColor = true;
            this.Clear_button.Click += new System.EventHandler(this.Clear_button_Click);
            // 
            // environmentErrorLabel
            // 
            this.environmentErrorLabel.AutoSize = true;
            this.environmentErrorLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.environmentErrorLabel.ForeColor = System.Drawing.Color.Red;
            this.environmentErrorLabel.Location = new System.Drawing.Point(224, 46);
            this.environmentErrorLabel.Name = "environmentErrorLabel";
            this.environmentErrorLabel.Size = new System.Drawing.Size(11, 13);
            this.environmentErrorLabel.TabIndex = 11;
            this.environmentErrorLabel.Text = "!";
            // 
            // dateErrorLabel
            // 
            this.dateErrorLabel.AutoSize = true;
            this.dateErrorLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dateErrorLabel.ForeColor = System.Drawing.Color.Red;
            this.dateErrorLabel.Location = new System.Drawing.Point(224, 72);
            this.dateErrorLabel.Name = "dateErrorLabel";
            this.dateErrorLabel.Size = new System.Drawing.Size(11, 13);
            this.dateErrorLabel.TabIndex = 12;
            this.dateErrorLabel.Text = "!";
            // 
            // gridEmptyMessage
            // 
            this.gridEmptyMessage.AutoSize = true;
            this.gridEmptyMessage.ForeColor = System.Drawing.Color.Red;
            this.gridEmptyMessage.Location = new System.Drawing.Point(9, 91);
            this.gridEmptyMessage.Name = "gridEmptyMessage";
            this.gridEmptyMessage.Size = new System.Drawing.Size(83, 13);
            this.gridEmptyMessage.TabIndex = 13;
            this.gridEmptyMessage.Text = "Data Not Found";
            this.gridEmptyMessage.Visible = false;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.ForeColor = System.Drawing.Color.Red;
            this.lblStatus.Location = new System.Drawing.Point(346, 408);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(88, 13);
            this.lblStatus.TabIndex = 15;
            this.lblStatus.Text = "Progressing ...0%";
            this.lblStatus.Visible = false;
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(4, 371);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(743, 23);
            this.progressBar.TabIndex = 14;
            // 
            // backgroundWorker
            // 
            this.backgroundWorker.WorkerReportsProgress = true;
            this.backgroundWorker.WorkerSupportsCancellation = true;
            this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_DoWork);
            this.backgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker_ProgressChanged);
            this.backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker_RunWorkerCompleted);
            // 
            // Start
            // 
            this.Start.Location = new System.Drawing.Point(8, 67);
            this.Start.Name = "Start";
            this.Start.Size = new System.Drawing.Size(78, 23);
            this.Start.TabIndex = 16;
            this.Start.Text = "Start";
            this.Start.UseVisualStyleBackColor = true;
            this.Start.Click += new System.EventHandler(this.Start_Click);
            // 
            // rbProdError
            // 
            this.rbProdError.AutoSize = true;
            this.rbProdError.Checked = true;
            this.rbProdError.Location = new System.Drawing.Point(16, 19);
            this.rbProdError.Name = "rbProdError";
            this.rbProdError.Size = new System.Drawing.Size(72, 17);
            this.rbProdError.TabIndex = 17;
            this.rbProdError.TabStop = true;
            this.rbProdError.Text = "Prod Error";
            this.rbProdError.UseVisualStyleBackColor = true;
            // 
            // rbProdTran
            // 
            this.rbProdTran.AutoSize = true;
            this.rbProdTran.Location = new System.Drawing.Point(94, 19);
            this.rbProdTran.Name = "rbProdTran";
            this.rbProdTran.Size = new System.Drawing.Size(106, 17);
            this.rbProdTran.TabIndex = 18;
            this.rbProdTran.TabStop = true;
            this.rbProdTran.Text = "Prod Transaction";
            this.rbProdTran.UseVisualStyleBackColor = true;
            // 
            // rbUATError
            // 
            this.rbUATError.AutoSize = true;
            this.rbUATError.Location = new System.Drawing.Point(222, 18);
            this.rbUATError.Name = "rbUATError";
            this.rbUATError.Size = new System.Drawing.Size(72, 17);
            this.rbUATError.TabIndex = 19;
            this.rbUATError.TabStop = true;
            this.rbUATError.Text = "UAT Error";
            this.rbUATError.UseVisualStyleBackColor = true;
            // 
            // rbUATTran
            // 
            this.rbUATTran.AutoSize = true;
            this.rbUATTran.Location = new System.Drawing.Point(312, 18);
            this.rbUATTran.Name = "rbUATTran";
            this.rbUATTran.Size = new System.Drawing.Size(106, 17);
            this.rbUATTran.TabIndex = 20;
            this.rbUATTran.TabStop = true;
            this.rbUATTran.Text = "UAT Transaction";
            this.rbUATTran.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.dateTimePicker);
            this.groupBox1.Controls.Add(this.rbProdError);
            this.groupBox1.Controls.Add(this.rbUATTran);
            this.groupBox1.Controls.Add(this.rbProdTran);
            this.groupBox1.Controls.Add(this.rbUATError);
            this.groupBox1.Controls.Add(this.Start);
            this.groupBox1.Location = new System.Drawing.Point(4, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(434, 96);
            this.groupBox1.TabIndex = 21;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Automation";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 47);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(96, 13);
            this.label4.TabIndex = 22;
            this.label4.Text = "Report End Date : ";
            // 
            // dateTimePicker
            // 
            this.dateTimePicker.Location = new System.Drawing.Point(113, 43);
            this.dateTimePicker.Name = "dateTimePicker";
            this.dateTimePicker.Size = new System.Drawing.Size(200, 20);
            this.dateTimePicker.TabIndex = 21;
            // 
            // groupBox3
            // 
            this.groupBox3.AutoSize = true;
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.ReportDate);
            this.groupBox3.Controls.Add(this.ReportDataGridView);
            this.groupBox3.Controls.Add(this.lblStatus);
            this.groupBox3.Controls.Add(this.ExportToExcel);
            this.groupBox3.Controls.Add(this.progressBar);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.gridEmptyMessage);
            this.groupBox3.Controls.Add(this.Go);
            this.groupBox3.Controls.Add(this.dateErrorLabel);
            this.groupBox3.Controls.Add(this.ReportTypeCombo);
            this.groupBox3.Controls.Add(this.environmentErrorLabel);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.Clear_button);
            this.groupBox3.Controls.Add(this.EnvironmentCombo);
            this.groupBox3.Controls.Add(this.reportValidateLabel);
            this.groupBox3.Location = new System.Drawing.Point(0, 105);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(5);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(5);
            this.groupBox3.Size = new System.Drawing.Size(900, 439);
            this.groupBox3.TabIndex = 23;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Reporting";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(319, 47);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(84, 13);
            this.label5.TabIndex = 23;
            this.label5.Text = "(7 Days Interval)";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Gainsboro;
            this.ClientSize = new System.Drawing.Size(900, 544);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Name = "Form1";
            this.Text = "New Relic Report Automating Tool";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ReportDataGridView)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox ReportDate;
        private System.Windows.Forms.DataGridView ReportDataGridView;
        private System.Windows.Forms.Button ExportToExcel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button Go;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox ReportTypeCombo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox EnvironmentCombo;
        private System.Windows.Forms.Label reportValidateLabel;
        private System.Windows.Forms.Button Clear_button;
        private System.Windows.Forms.Label environmentErrorLabel;
        private System.Windows.Forms.Label dateErrorLabel;
        private System.Windows.Forms.Label gridEmptyMessage;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ToolTip reportTypeTooltip;
        private System.Windows.Forms.ToolTip environmentTooltip;
        private System.Windows.Forms.ToolTip CreateDateTooltip;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.ComponentModel.BackgroundWorker backgroundWorker;
        private System.Windows.Forms.Button Start;
        private System.Windows.Forms.RadioButton rbUATTran;
        private System.Windows.Forms.RadioButton rbUATError;
        private System.Windows.Forms.RadioButton rbProdTran;
        private System.Windows.Forms.RadioButton rbProdError;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DateTimePicker dateTimePicker;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
    }
}

