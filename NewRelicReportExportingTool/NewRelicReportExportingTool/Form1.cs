using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using NewRelicTestFramework.DataBase;
using NewRelicTestFramework;
using NewRelicTestFramework.Repositories;
using NewRelicTestFramework.Common;


namespace NewRelicReportExportingTool
{
    public partial class Form1 : Form
    {
        private List<ErrorsList> errorList;
        private List<TransactionTraces> transactionList;
        private readonly BackgroundWorker _bw = new BackgroundWorker();

        public Form1()
        { 
            InitializeComponent();
            
        }
      
        struct DataParameter
        {
            public List<ErrorsList> errorList;
            public List<TransactionTraces> transactionList;
            public ReportType report_type;
            
        }

        DataParameter _inputParameter;
        private void ReportTypeCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            gridEmptyMessage.Visible = false;
            ComboBox comboBox = (ComboBox)sender;
            if (comboBox.SelectedIndex == -1)
            {
                reportValidateLabel.Visible = true;
                return;
            }
            else
                reportValidateLabel.Visible = false;
        }
        private void EnvironmentCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            gridEmptyMessage.Visible = false;
            ComboBox comboBox = (ComboBox)sender;
            if (comboBox.SelectedIndex == -1)
            {
                environmentErrorLabel.Visible = true;
                return;
            }
            else
                environmentErrorLabel.Visible = false;
        }

        private void ReportDate_SelectedIndexChanged(object sender, EventArgs e)
        {
            gridEmptyMessage.Visible = false;
            ComboBox comboBox = (ComboBox)sender;
            if (comboBox.SelectedIndex == -1)
            {
                dateErrorLabel.Visible = true;
                return;
            }
            else
                dateErrorLabel.Visible = false;
        }

        private List<string> LoadReportDate()
        {
            
            return ErrorReportRepository.GetReportDates();
        }

      
        private void Go_Click(object sender, EventArgs e)
        {
            transactionList = new List<TransactionTraces>();
            errorList = new List<ErrorsList>();
            ReportTypeCombo.SelectedIndexChanged += new EventHandler(ReportTypeCombo_SelectedIndexChanged);
            EnvironmentCombo.SelectedIndexChanged += new EventHandler(EnvironmentCombo_SelectedIndexChanged);
            ReportDate.SelectedIndexChanged += new EventHandler(ReportDate_SelectedIndexChanged);
            if (!Validation())
            {
                return;
            }  
            
            if (SelectedReportType == ReportType.Error)
            {
                errorList=ErrorReportRepository.GetErrorReport(SelectedReportType, SelectedEnvironment, SelectedReportDate);
                
                ReportDataGridView.DataSource = errorList;
                ReportDataGridView.Columns["BeginDate"].Visible = false;
                ReportDataGridView.Columns["EndDate"].Visible = false;
            }
            else if(SelectedReportType == ReportType.Transaction)
            {
                transactionList = TransactionReportRepository.GetTransactionReport(SelectedReportType, SelectedEnvironment, SelectedReportDate);
                ReportDataGridView.DataSource = transactionList;
                ReportDataGridView.Columns["IssueType"].Width = 200;
                ReportDataGridView.Columns["SlowPage"].Width = 250;
                ReportDataGridView.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }                  
                     
        }

       
        public ReportType SelectedReportType
        {
            get
            {
                return (ReportType)ReportTypeCombo.SelectedItem;
            }
            set
            {
                SelectedReportType = value;
            }
        }

        public NewRelicTestFramework.Environment SelectedEnvironment
        {
            get
            {
                return (NewRelicTestFramework.Environment)EnvironmentCombo.SelectedItem;
            }
            set
            {
                SelectedEnvironment = value;
            }
        }

        public object SelectedReportDate
        {
            get
            {
                return ReportDate.SelectedItem;
            }
            set
            {
                SelectedReportDate = value;
            }
        }
      
        private bool Validation()
        {
            bool isValidated = true;
            if (ReportTypeCombo.SelectedIndex == -1 && EnvironmentCombo.SelectedIndex == -1 && ReportDate.SelectedIndex == -1)
            {
                reportValidateLabel.Visible = true;
                environmentErrorLabel.Visible = true;
                dateErrorLabel.Visible = true;
                MessageBox.Show("You must select a Report type, Environemnt and Date", "Error");
                isValidated = false;
            }

            else
            {
                if (ReportTypeCombo.SelectedIndex == -1)
                {
                    reportValidateLabel.Visible = true;
                    MessageBox.Show("You must select a Report type.", "Error");
                    isValidated = false;
                }
                 if (EnvironmentCombo.SelectedIndex == -1)
                {
                    environmentErrorLabel.Visible = true;
                    MessageBox.Show("You must select a Environment.", "Error");
                    isValidated = false;
                }
                 if (ReportDate.SelectedIndex == -1)
                {
                    dateErrorLabel.Visible = true;
                    MessageBox.Show("You must select a Report date.", "Error");
                    isValidated = false;
                }
            }

            return isValidated;
        }

     
        public void LoadReportType()
        {
            List<ReportType> reportTypeList = new List<ReportType>();
            reportTypeList.Add(ReportType.Error);
            reportTypeList.Add(ReportType.Transaction);
            ReportTypeCombo.DataSource = reportTypeList;

        }

        public void LoadEnvironmentType()
        {
            List<NewRelicTestFramework.Environment> environmentList = new List<NewRelicTestFramework.Environment>();
            environmentList.Add(NewRelicTestFramework.Environment.PROD);
            environmentList.Add(NewRelicTestFramework.Environment.UAT);
            EnvironmentCombo.DataSource = environmentList;
            
        }

        private void ExportToExcel_Click(object sender, EventArgs e)
        {
            if (backgroundWorker.IsBusy)
                return;
            ReportTypeCombo.SelectedIndexChanged += new EventHandler(ReportTypeCombo_SelectedIndexChanged);
            EnvironmentCombo.SelectedIndexChanged += new EventHandler(EnvironmentCombo_SelectedIndexChanged);
            ReportDate.SelectedIndexChanged += new EventHandler(ReportDate_SelectedIndexChanged);
            if (!Validation())
            {
                return;
            }
            progressBar.Minimum = 0;
            progressBar.Value = 0;
            lblStatus.Visible = true;
            if (SelectedReportType == ReportType.Transaction)
            {
             
                if (transactionList != null)
                {
                    _inputParameter.transactionList = transactionList;
                }
                else
                    _inputParameter.transactionList = TransactionReportRepository.GetTransactionReport(SelectedReportType, SelectedEnvironment,SelectedReportDate);
                _inputParameter.report_type = SelectedReportType;               
                
               
            }
            else if (SelectedReportType == ReportType.Error)
            {    
                _inputParameter.report_type = SelectedReportType;
                if (errorList != null)
                {
                    _inputParameter.errorList = errorList;
                }
                else
                    _inputParameter.errorList = ErrorReportRepository.GetErrorReport(SelectedReportType, SelectedEnvironment, SelectedReportDate);               
                   
            }
            backgroundWorker.RunWorkerAsync(_inputParameter);

        }

        private void Clear_button_Click(object sender, EventArgs e)
        {
            ReportTypeCombo.SelectedIndex = -1;
            EnvironmentCombo.SelectedIndex = -1;
            ReportDate.SelectedIndex = -1;
            ReportDataGridView.DataSource = null;
            lblStatus.Visible = false;
            progressBar.Value = 0;
        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
            lblStatus.Text = string.Format("Progressing...{0}", e.ProgressPercentage);
            progressBar.Update();
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if(e.Error==null)
            {
                Thread.Sleep(100);
                lblStatus.Text = "Your data has been successfully exported.";
            }
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var _event = (DataParameter)e.Argument;
            if (_event.report_type == ReportType.Error)
            {
                var list = _event.errorList as List<ErrorsList>;
                Excel.ExportErrorReportToExcel(list, this.backgroundWorker,SelectedEnvironment);
            }
            else
            {
               var transactionList = _event.transactionList as List<TransactionTraces>;
               Excel.ExportTransactionReportToExcel(transactionList, this.backgroundWorker,SelectedEnvironment);
            }
        }

        private void Start_Click(object sender, EventArgs e)
        {
            
            var reportsList = ErrorReportRepository.GetReports();     
            var selectedReportDate = dateTimePicker.Value;
            //if (reportsList.Exists(c => c.ReportDate >= reportDate || c.ReportDate.AddDays(7) >= reportDate))
            //{
            //    MessageBox.Show("Select Date greater than 7 days than Previous Report Date. ", "Message");
            //    return;
            //}
            var env = "";
            var reportType = "";

            if (rbProdError.Checked)
            {
                env = NewRelicTestFramework.Environment.PROD.ToString();
                reportType = StringEnumerator.GetStringValue(ReportType.Error);
                if (IsDublicateReport(reportsList, selectedReportDate, env, reportType))
                {
                    MessageBox.Show("Report for this Date, Environment and Report Type has already been generated.", "Error");
                    return;
                }
                else if (IsDateGreaterOrLessThan7Days(reportsList, env, reportType, selectedReportDate))
                {
                    MessageBox.Show("Select Date greater than 7 days than Previous Report Date. ", "Message");
                    return;
                }         

                else
                {
                    Pages.GenerateProductionErrorReport(selectedReportDate);
                    ReportDate.DataSource = LoadReportDate();
                    ReportDate.SelectedIndex = -1;
                }
            }
            else if (rbProdTran.Checked)
            {
                env = NewRelicTestFramework.Environment.PROD.ToString();
                reportType =StringEnumerator.GetStringValue(ReportType.Transaction);
                if (IsDublicateReport(reportsList, selectedReportDate, env, reportType))
                {
                    MessageBox.Show("Report for this Date, Environment and Report Type has already been generated.", "Error");
                }
                else if (IsDateGreaterOrLessThan7Days(reportsList, env, reportType, selectedReportDate))
                {
                    MessageBox.Show("Select Date greater than 7 days than Previous Report Date. ", "Message");
                    return;
                }
                else
                {
                    Pages.GenerateProdTransactionReport(selectedReportDate);
                    ReportDate.DataSource = LoadReportDate();
                    ReportDate.SelectedIndex = -1;
                }
            }
            else if (rbUATError.Checked)
            {
                env = NewRelicTestFramework.Environment.UAT.ToString();
                reportType =StringEnumerator.GetStringValue(ReportType.Error);
                if (IsDublicateReport(reportsList, selectedReportDate, env, reportType))
                {
                    MessageBox.Show("Report for this Date, Environment and Report Type has already been generated.", "Error");
                }
                else if (IsDateGreaterOrLessThan7Days(reportsList, env, reportType, selectedReportDate))
                {
                    MessageBox.Show("Select Date greater than 7 days than Previous Report Date. ", "Message");
                    return;
                }
                else
                {
                    Pages.GenerateUATErrorReport(selectedReportDate);
                    ReportDate.DataSource = LoadReportDate();
                    ReportDate.SelectedIndex = -1;
                }
            }
            else if (rbUATTran.Checked)
            {
                env = NewRelicTestFramework.Environment.UAT.ToString();
                reportType =StringEnumerator.GetStringValue(ReportType.Transaction);
                if (IsDublicateReport(reportsList, selectedReportDate, env, reportType))
                {
                    MessageBox.Show("Report for this Date, Environment and Report Type has already been generated.", "Error");
                }
                else if (IsDateGreaterOrLessThan7Days(reportsList, env, reportType, selectedReportDate))
                {
                    MessageBox.Show("Select Date greater than 7 days than Previous Report Date. ", "Message");
                    return;
                }
                else
                {
                    Pages.GenerateUATTransactionReport(selectedReportDate);
                    ReportDate.DataSource = LoadReportDate();
                    ReportDate.SelectedIndex = -1;
                }
            }
            else
            {
                
                MessageBox.Show("Please select report type!!!", "Error");
            }
        }

        private bool IsDateGreaterOrLessThan7Days(List<NewRelicReports> reportsList, string env, string reportType, DateTime selectedReportDate)
        {
            bool isTrue = false;
            var list = reportsList.Where(c => c.Environments == env && c.ReportType == reportType).OrderByDescending(c => c.ReportDate).FirstOrDefault();
            if (list.ReportDate >= selectedReportDate || list.ReportDate.AddDays(7) >= selectedReportDate)
            {
                isTrue = true;
            }
            return isTrue;
        }

        private static bool IsDublicateReport(List<NewRelicReports> reportsList, DateTime reportDate, string env, string reportType)
        {
            return reportsList.Exists(c => c.ReportDate.ToShortDateString() == reportDate.ToShortDateString() && c.Environments == env && c.ReportType == reportType);
        }

        private void Form1_Load(object sender, EventArgs e)
        {                   
            LoadReportType();
            ReportDate.DataSource= LoadReportDate();
            LoadEnvironmentType();
            ReportTypeCombo.SelectedIndex = -1;
            EnvironmentCombo.SelectedIndex = -1;
            ReportDate.SelectedIndex = -1;
            reportValidateLabel.Visible = false;
            environmentErrorLabel.Visible = false;
            dateErrorLabel.Visible = false;
            reportTypeTooltip.SetToolTip(reportValidateLabel, "Select Report Type");
            environmentTooltip.SetToolTip(environmentErrorLabel, "Select Environment");
            CreateDateTooltip.SetToolTip(dateErrorLabel, "Select Date");            
        }
      
    }
}
