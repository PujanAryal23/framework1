using System.Collections;
using Nucleus.Service.Support.Enum;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Nucleus.Service.PageObjects.Appeal;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;

namespace Nucleus.Service.Support.Common
{
    public sealed class FileUploadPage
    {
        #region Page OBJECT Properties
        #region Upload Section

        
        public const string AppealDocumentUploadEnabledXPath =            
         "//li[span[contains(@class,'add_icon' )] and contains(@class,'is_active')]";
        public const string ClaimDocumentUploadEnabledXPath = "//section[contains(@class,'upload_form')]//li[span[contains(@class,'add_icon' )] and contains(@class,'is_active')]";
        public const string AppealDocumentUploadDisabledXPath =
            "//li[span[contains(@class,'add_icon' )] and contains(@class,'is_disabled')]";
        public const string FileTypeAvailableValueXPathTemplate =
            "//section[contains(@class,'multi_select_wrap')]/ul/section/li[text()='{0}']";
        public const string FileToUploadSection = "//section[contains(@class, 'appeal_upload_form')]/div/ul[3],//section[contains(@class, 'appeal_create_form')]/div/ul[3]";
        public const string AppealDocumentsDivCssLocator = "div.listed_document";
        public const string ListsofAppealDocumentsCssLocator = "div.listed_document >ul:nth-of-type(1) >li:nth-of-type(4) span ";
        public const string DocumentUploadSectionCssLocator = "section.appeal_upload_form,section.upload_form ";
       
        public const string AppealDocumentUploadFileBrowseCssLocator = "section.appeal_upload_form,section.upload_form input.file_upload_field,section.appeal_create_form input.file_upload_field,input.file_upload_field";
        public const string CIWFileUploadFileBrowseCssLocator = "input.file_upload_field";
        public const string LogoUploadFileBrowserCssLocator = "input[multiple].file_upload_field";

        public const string AppealUploaderFieldLabelCssLocator =
            "div.basic_input_component.uploader  div:nth-of-type({0}) label ";

        public const string AppealUploaderFieldValueCssLocator =
            "div.basic_input_component.uploader  div:nth-of-type({0}) input ";

        public const string FileTypeCssLocator = "section.form_component section.multi_select_wrap >input";
        public const string FileTypeToggleIconCssLocator = "section.multi_select_wrap >span";
        public const string FileTypeSelectedValueListCssLocator = "section.multi_select_wrap >ul>section.selected_options>li";
        public const string FileTypeValueListCssLocator = "section.multi_select_wrap >ul>section.available_options>li";
        public const string CancelAppealUploadButtonCssLocator = "section.appeal_upload_form,section.upload_form span.span_link";
        public const string DisabledCancelButtonCssLocator = "section.upload_form span.is_disabled";
        public const string AddDocumentButtonCssLocator = "//button[text()='Add File']";
        public const string DisabledAddDocumentButtonCssLocator = "section.appeal_upload_form,section.upload_form button.work_button.basic_button_component.is_disabled,section.appeal_create_form button.work_button.basic_button_component.is_disabled";
        public const string SaveAppealUploadButtonCssLocator = "section.appeal_upload_form,section.upload_form div.form_buttons button.work_button";
        public const string FileToUploadDetailsCssLocator = "section.upload_form ul li:nth-of-type({0})  li:nth-of-type({1})> span,section.appeal_create_form ul li:nth-of-type({0})  li:nth-of-type({1})> span";
        public const string ClaimFileToUploadDetailsCssLocator = "section.upload_form ul li:nth-of-type({0})  li:nth-of-type({1})> span,section.appeal_create_form ul li:nth-of-type({0})  li:nth-of-type({1})> span";// "//section[contains(@class,'upload_form')]/../ul/div[{0}]/ul/li[{1}]/span";
        public const string FileListRowXPath = "(//header[.//text()='Files To Upload']/../li)[{0}]";
        public const string DeleteFileDocumentInAppealSummaryCssLocator = "div.listed_document:nth-of-type({0}) >ul:nth-of-type(1) span.small_delete_icon ";
        public const string DeleteIconForFileListCssLocator = "div.listed_document >ul:nth-of-type(1) span.small_delete_icon";
        public const string DeleteIconDisabledForFileListCssLocator = "div.listed_document >ul:nth-of-type(1) span.small_delete_icon.is_disabled";
        public const string AppealDocumentsListAttributeValueCssTemplate = "div.listed_document:nth-of-type({0}) >ul:nth-of-type({1}) >li:nth-of-type({2}) span ";
        public const string FilesToUploadListXpath = "//label[text()='Files To Upload']/../../../li";
        public const string AppealDocumentFileXpathTempalte =
            "//span[contains(@title, '{0}')]";
        public const string AppealDocumentDeleteForFileNameXpathTempalte =
            "//span[contains(@title, '{0}')]/../..//span[contains(@class, 'small_delete_icon')]";
        public const string FieldInputXpath = "//label[text()='{0}']/../input";
        public const string SelectedFilesInputClassLocator = "file_upload_field";
        public const string DocumentUploadHeaderCssLocator = "section.claim_secondary_view label.document_form_header_left";
        public const string AddDocumentIconDisabledXPath = "//span[text()='Claim Document Uploader']/../../li[contains(@class,'is_disabled')]";
        public const string AddDocumentIconEnabledXPath = "//span[text()='Claim Document Uploader']/../../li[contains(@class,'is_active')]";
        public const string DeleteIconForUploadedDocumentsCssSelector = "ul.document_list div.listed_document:nth-of-type({0}) span.small_delete_icon";

        public const string WorkingAjaxMessageCssLocator = "div.small_loading";
        #endregion

        #region PageObject propertiest

        [FindsBy(How = How.CssSelector, Using = CancelAppealUploadButtonCssLocator)]
        public Link CancelBtn;
        #endregion

        private readonly INewNavigator _navigator;
        private readonly ISiteDriver SiteDriver;
        private readonly IJavaScriptExecutors JavaScriptExecutor;

        #endregion

        #region CONSTRUCTOR
        public FileUploadPage(ISiteDriver siteDriver, IJavaScriptExecutors javaScriptExecutor)
         {
             SiteDriver = siteDriver;
             JavaScriptExecutor = javaScriptExecutor;
         }
        #endregion

        #region PUBLIC METHODS
         #region Upload Section

         public bool IsUploadInputSectionPresent() =>
             SiteDriver.IsElementPresent(AppealDocumentUploadFileBrowseCssLocator, How.CssSelector);
         public bool IsAppealDocumentUploadEnabled()
         {
             return SiteDriver.IsElementPresent(AppealDocumentUploadEnabledXPath, How.XPath);
         }


        public bool IsClaimDocumentUploadEnabled()
        {
            return SiteDriver.IsElementPresent(ClaimDocumentUploadEnabledXPath, How.XPath);
        }
         public string GetAppealDocumentUploadIconToolTip()
         {
             return SiteDriver.FindElement(AppealDocumentUploadEnabledXPath, How.XPath).GetAttribute("title"); 
         }
         public bool IsAppealDocumentUploadDisabled()
         {
             return SiteDriver.IsElementPresent(AppealDocumentUploadDisabledXPath, How.XPath);
         }

         public void ClickOnAppealDocumentUploadIcon()
         {
             JavaScriptExecutor.ExecuteClick(AppealDocumentUploadEnabledXPath, How.XPath);
             Console.WriteLine("Clicked Document Upload Icon");
         }

        public void ClickOnClaimDocumentUploadIcon()
        {
            JavaScriptExecutor.ExecuteClick(ClaimDocumentUploadEnabledXPath, How.XPath);
            Console.WriteLine("Clicked Claim Document Upload Icon");
        }
        public bool IsDocumentUploadSectionPresent()
         {
             return SiteDriver.IsElementPresent(DocumentUploadSectionCssLocator, How.CssSelector);
         }
       

        public bool IsAddFileButtonDisabled()
         {
             return SiteDriver.IsElementPresent(DisabledAddDocumentButtonCssLocator, How.CssSelector);
         }


         public void PassFilePathForDocumentUpload()
         {
             SiteDriver.LocalFileDetect();
             string localDoc = Path.GetFullPath("Documents/Test.txt");
             Console.WriteLine("Upload File Path of localDoc  " + localDoc);
             SiteDriver.FindElement(AppealDocumentUploadFileBrowseCssLocator, How.CssSelector).SendKeys(localDoc);
         }

         public void PassGivenFileNameFilePathForDocumentUpload(string filename)
         {
             SiteDriver.LocalFileDetect();
             string localDoc = Path.GetFullPath("Documents/" + filename);
             Console.WriteLine("Upload File Path of localDoc  " + localDoc);
             SiteDriver.FindElement(AppealDocumentUploadFileBrowseCssLocator, How.CssSelector).SendKeys(localDoc);
         }
         public void PassEmptyFileNameFilePathForDocumentUpload()
         {
             SiteDriver.FindElement(AppealDocumentUploadFileBrowseCssLocator, How.CssSelector).SendKeys("");
         }
         public void PassGivenFileNameFilePathForDocumentUpload(string[] filename)
         {
             string localDoc = null;
             foreach (var file in filename)
             {
                 SiteDriver.LocalFileDetect();
                 localDoc = (String.IsNullOrEmpty(localDoc)) ? Path.GetFullPath("Documents/" + file) :
                  localDoc+"\n"+Path.GetFullPath("Documents/" + file);
                 Console.WriteLine("Upload File Path of localDoc  " + localDoc);
             }
             SiteDriver.FindElement(AppealDocumentUploadFileBrowseCssLocator, How.CssSelector).SendKeys(localDoc); 
         }
         public string GetFileUploaderFieldLabel(int row)
         {
             return SiteDriver.FindElement(
                 string.Format(AppealUploaderFieldLabelCssLocator, row), How.CssSelector).Text;
         }
         public void SetFileUploaderFieldValue(string description, int row)
         {
             SiteDriver.FindElement(string.Format(AppealUploaderFieldValueCssLocator, row), How.CssSelector)
                 .SendKeys(description);
         }
         public void SetFileUploaderFieldValue(string fieldname, string description)
         {
             SiteDriver.FindElement(string.Format(FieldInputXpath, fieldname), How.XPath)
                 .ClearElementField();
             SiteDriver.FindElement(string.Format(FieldInputXpath, fieldname), How.XPath).Click();
             SiteDriver.FindElement(string.Format(FieldInputXpath, fieldname), How.XPath).SendKeys(description);
         }
         public string GetFileUploaderFieldValue(int row)
         {
             return SiteDriver.FindElement(
                 string.Format(AppealUploaderFieldValueCssLocator, row), How.CssSelector).GetAttribute("value");
         }
         public List<String> GetSelectedFilesValue()
         {
             var filesList = JavaScriptExecutor.GetUploadFileListObject(SelectedFilesInputClassLocator);
             return ((IEnumerable)filesList).Cast<string>()
                                    .Select(x => x == null ? null : x.ToString())
                                    .ToList();
         }
         ///<summary>
         /// Click on add file button
         /// </summary>
         public void ClickOnAddFileBtn()
         {
             SiteDriver.FindElement(AddDocumentButtonCssLocator, How.XPath).Click();
             Console.WriteLine("Add document in  Appeal Document Upload");
         }
        /// <summary>
        /// count of delete icon enabled in the listed files
        /// </summary>
        /// <returns>enabled delete icon count</returns>
         public int CountOfDeleteIconForFileList()
         {
             return SiteDriver.FindElementsCount(DeleteIconForFileListCssLocator, How.CssSelector);
         }
         public bool IsAppealDocumentDeleteDisabled()
         {
             return SiteDriver.IsElementPresent(DeleteIconDisabledForFileListCssLocator, How.CssSelector);
         }
         /// <summary>
         /// Click on delete file button
         /// </summary>
         public void ClickOnDeleteFileBtn(int row)
         {
             SiteDriver.FindElement(string.Format(DeleteFileDocumentInAppealSummaryCssLocator, row), How.CssSelector).Click();

             Console.WriteLine("Delete document in  Appeal Document ");
         }
         /// <summary>
         /// Click on delete file button for given filename
         /// </summary>
         public void ClickOnDeleteFileBtn(string filename)
         {
             SiteDriver.FindElement(string.Format(AppealDocumentDeleteForFileNameXpathTempalte, filename), How.XPath).Click();

             Console.WriteLine("Delete document for following filename: {0} in  Appeal Document ", filename);
         }
        
         public bool IsFileToUploadPresent()
         {
             return SiteDriver.IsElementPresent(FileToUploadSection, How.XPath);
         }
         public string FileToUploadDocumentValue(int row, int col)
         {
             return SiteDriver.FindElement(
                 string.Format(FileToUploadDetailsCssLocator, row, col), How.CssSelector).Text;
         }

        public string ClaimFileToUploadDocumentValue(int row, int col)
        {
            var data= SiteDriver.FindElement(
                string.Format(ClaimFileToUploadDetailsCssLocator, row, col), How.CssSelector).Text;
            return data;
        }

        public void ClickOnDeleteIconInFilesToUpload(int row, int col=1)
        {
            SiteDriver.FindElementAndClickOnCheckBox(string.Format(ClaimFileToUploadDetailsCssLocator, row, col), How.CssSelector);
            SiteDriver.WaitToLoadNew(500);
        }
        public string GetFileToUploadTooltipValue(int row,int col)
        {

            return SiteDriver.FindElement(string.Format(ClaimFileToUploadDetailsCssLocator, row, col), How.CssSelector).GetAttribute("title");
             
        }
        public List<string> GetAvailableFileTypeList()
         {
             SiteDriver.FindElement(FileTypeToggleIconCssLocator, How.CssSelector).Click();
             SiteDriver.WaitToLoadNew(200);
             return JavaScriptExecutor.FindElements(FileTypeValueListCssLocator);

         }
         public string GetPlaceHolderValue()
         {
             return SiteDriver.FindElement(FileTypeCssLocator, How.CssSelector)
                 .GetAttribute("placeholder");
         }
         public void SetFileTypeListVlaue(string fileType)
         {
             JavaScriptExecutor.ExecuteClick(FileTypeToggleIconCssLocator, How.CssSelector);
             SiteDriver.FindElement(FileTypeCssLocator, How.CssSelector).SendKeys(fileType);
             JavaScriptExecutor.ExecuteClick(string.Format(FileTypeAvailableValueXPathTemplate, fileType), How.XPath);
             JavaScriptExecutor.ExecuteMouseOut(string.Format(FileTypeAvailableValueXPathTemplate, fileType), How.XPath);
             Console.WriteLine("File Type Selected: <{0}>", fileType);
         }

         public List<string> GetSelectedFileTypeList()
         {
             JavaScriptExecutor.ExecuteClick(FileTypeToggleIconCssLocator, How.CssSelector);
             return JavaScriptExecutor.FindElements(FileTypeSelectedValueListCssLocator);

         }
         public List<DateTime> GetAddedDocumentList()
         {
             return JavaScriptExecutor.FindElements(ListsofAppealDocumentsCssLocator, How.CssSelector, "Text").Select(DateTime.Parse).ToList();

         }

        //public int GetAddedDocumentCount()
        //{
        //    return SiteDriver.FindElementsCount(ListsofAppealDocumentsCssLocator, How.CssSelector);

        //}
        public string AppealDocumentsListAttributeValue(int docrow, int ulrow, int col) //ul 1: 2 :filename, 3:file type, 4: date, ul 2, 2: doc descp
         {
             return SiteDriver.FindElement(string.Format(AppealDocumentsListAttributeValueCssTemplate, docrow, ulrow, col), How.CssSelector).Text;
         }

        public List<string> GetFileTypeAttributeListVaues(int docrow, int ulrow, int col) //ul 1: 2 :filename, 3:file type, 4: date, ul 2, 2: doc descp
        {
            var list = SiteDriver
                .FindElement(string.Format(AppealDocumentsListAttributeValueCssTemplate, docrow, ulrow, col),
                    How.CssSelector).Text.Split(',').Select(x => x.Trim());
            return list.ToList();

        }
        public string GetAppealDocumentAttributeToolTip(int docrow, int ulrow, int col) //ul 1: 2 :filename, 3:file type, 4: date, ul 2, 2: doc descp
         {
             return SiteDriver.FindElement(string.Format(AppealDocumentsListAttributeValueCssTemplate, docrow, ulrow, col), How.CssSelector).GetAttribute("title");
         }

        public List<string> GetFileTypeAttributeListToolTip(int docrow, int ulrow, int col) //ul 1: 2 :filename, 3:file type, 4: date, ul 2, 2: doc descp
        {
            var list = SiteDriver
                .FindElement(string.Format(AppealDocumentsListAttributeValueCssTemplate, docrow, ulrow, col),
                    How.CssSelector).GetAttribute("title").Split(',').Select(x => x.Trim());
            return list.ToList();

        }

        public bool IsEllipsisPresentInFileType(int docrow)
        {
            return SiteDriver.FindElement(string.Format("div.listed_document:nth-of-type({0}) >ul:nth-of-type({1}) >li:nth-of-type({2})", docrow, 1, 3),
                How.CssSelector).GetCssValue("text-overflow").Equals("ellipsis");
        }

        public void AddFileForUpload(string fileName)
        {
            PassGivenFileNameFilePathForDocumentUpload(fileName); 
        }
        public void AddNoFileForUpload()
        {
            PassEmptyFileNameFilePathForDocumentUpload();
        }
        public void AddFileForUpload(string[] fileName)
        {
            PassGivenFileNameFilePathForDocumentUpload(fileName);
        }
        /// <summary>
        /// Add following fileName fil with Selected filetype  then verify it has been added under file list
        /// </summary>
        /// <param name="fileType">Type of file</param>
        /// <param name="row">what row reflects the file added under file docuemtns section</param>
        /// <param name="fileName">file to be uploaded</param>
        /// <returns>true or false whether file has been added</returns>
         public bool AddFileForUpload(string fileType, int row, string fileName)
         {
             PassGivenFileNameFilePathForDocumentUpload(fileName);
             SetFileTypeListVlaue(fileType);
             SetFileUploaderFieldValue("Description", "test appeal doc");
             ClickOnAddFileBtn();
             if (!IsFileToUploadPresent())
             {

                 Console.WriteLine("File not present for upload");
                 return false;
             }
             Console.WriteLine("File is present for upload");
             if (!FileToUploadDocumentValue(row, 1).Equals(fileName))
             {
                 Console.WriteLine("Added document doesn't correspond to uploaded doc details");
                 return false;
             }
             Console.WriteLine("Added document  correspond to uploaded doc details");

             return true;

         }
         /// <summary>
         /// Click on save button
         /// </summary>
         public void ClickOnSaveUploadBtn()
         {
             JavaScriptExecutor.ExecuteClick(SaveAppealUploadButtonCssLocator, How.CssSelector);
             Console.WriteLine("Save Appeal Document Upload");

             WaitForWorkingAjaxMessage();
         }
         public void WaitForWorkingAjaxMessage()
         {
             SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
             SiteDriver.WaitForPageToLoad();

         }
         /// <summary>
         /// Click on cancel button
         /// </summary>
         public void ClickOnCancelBtn()
         {
             SiteDriver.FindElement(CancelAppealUploadButtonCssLocator, How.CssSelector).Click();
             Console.WriteLine("Cancel Document Upload");
         }

        public bool IsCancelButtonDisabled()
        {
            return SiteDriver.IsElementPresent(DisabledCancelButtonCssLocator, How.CssSelector);            
        }
        /// <summary>
        /// Click on file row to remove file
        /// </summary>
        public void ClickOnFileRowToRemoveFile(int row)
         {

             //JavaScriptExecutors.ExecuteClick(string.Format(FileListRowXPath, row), How.XPath);
             SiteDriver.FindElement(string.Format(FileListRowXPath,row), How.XPath).Click();
             Console.WriteLine("Click on file row to remove file");
         }
         #endregion

        public bool IsDocumentDivPresent()
        {
            return SiteDriver.IsElementPresent(AppealDocumentsDivCssLocator, How.CssSelector);
        }
        public int DocumentCountOfFileList()
        {
            return SiteDriver.FindElementsCount(AppealDocumentsDivCssLocator, How.CssSelector);
           
        }
        public bool IsWorkingAjaxMessagePresent()
        {
            return SiteDriver.IsElementPresent(WorkingAjaxMessageCssLocator, How.CssSelector);
        }

        public bool IsFollowingAppealDocumentPresent(string filename)
        {
            return SiteDriver.IsElementPresent(string.Format(AppealDocumentFileXpathTempalte, filename), How.XPath);
        }

        public string DocumentUploadHeaderText()
        {
            return SiteDriver.FindElement(DocumentUploadHeaderCssLocator, How.CssSelector).Text;
        }

        public int GetFilesToUploadCount()
        {
            return SiteDriver.FindElementsCount(FilesToUploadListXpath, How.XPath);
        }

        public bool IsAddDocumentIconEnabled()
        {
            return SiteDriver.IsElementPresent(AddDocumentIconEnabledXPath, How.XPath);
        }

        public bool IsAddDocumentIconDisabled()
        {
            return SiteDriver.IsElementPresent(AddDocumentIconDisabledXPath, How.XPath);
        }

        public bool UploadDocument(string fileType, string fileName, string descp, int row)
        {
            if (!IsAppealDocumentUploadEnabled())
            {
                Console.WriteLine("Uploader not enabled");
                return false;
            }
            ClickOnAppealDocumentUploadIcon();

            PassGivenFileNameFilePathForDocumentUpload(fileName);
            SetFileUploaderFieldValue(descp, 3);
            SetFileTypeListVlaue(fileType);
            ClickOnAddFileBtn();
            if (!IsFileToUploadPresent())
            {
                Console.WriteLine("File not added for upload.");
                return false;
            }
            if (!FileToUploadDocumentValue(row, 2).Equals(fileName))
            {
                Console.WriteLine("Added document doesn't correspond to uploaded doc details");
                return false;
            }
            ClickOnSaveUploadBtn();
            return true;
        }

        public bool UploadCIWDocument(string fileName)
        {
            SiteDriver.LocalFileDetect();
            string localDoc = Path.GetFullPath("Documents/" + fileName);
            Console.WriteLine("Upload File Path of localDoc  " + localDoc);
            SiteDriver.FindElement(CIWFileUploadFileBrowseCssLocator, How.CssSelector).SendKeys(localDoc);
            return true;
        }

        public bool UploadClientLogo(string firstfile,string secondfile=null)
        {
            SiteDriver.LocalFileDetect();
            string localDoc = Path.GetFullPath("Documents/" + firstfile);
            if (secondfile != null)
            {
                string secondlocalDoc = Path.GetFullPath("Documents/" + secondfile);
                Console.WriteLine("Upload File Path of following locations  " + localDoc + secondlocalDoc);
                SiteDriver.FindElement(LogoUploadFileBrowserCssLocator, How.CssSelector).SendKeys(localDoc + "\n" + secondlocalDoc);
            }
            else
            {
                Console.WriteLine("Upload File Path of following locations  " + localDoc);
                SiteDriver.FindElement(LogoUploadFileBrowserCssLocator, How.CssSelector).SendKeys(localDoc);

            }
            
            
            return true;
        }
        public bool UploadMultipleClientLogo(string filename, bool multiple =true)
        {
            SiteDriver.LocalFileDetect();
            if (multiple)
            {
                var files = filename.Split(',').ToList();
                List<string> locDoc=new List<string>();
                foreach (var file in files)
                {
                    locDoc.Add(Path.GetFullPath("Documents/" + file));
                }

                var filestoupload = string.Join("\n", locDoc);
                SiteDriver.FindElement(LogoUploadFileBrowserCssLocator, How.CssSelector).SendKeys(filestoupload);
            }
            else
            {
                var localDoc = Path.GetFullPath("Documents/" + filename);
                Console.WriteLine("Upload File Path of following locations  " + localDoc);
                SiteDriver.FindElement(LogoUploadFileBrowserCssLocator, How.CssSelector).SendKeys(localDoc);
            }
            


            return true;
        }

        /// <summary>
        /// Click on delete file button
        /// </summary>
        public void ClickOnDeleteUploadedDocumentIcon(int row)
        {
            JavaScriptExecutor.ExecuteClick(string.Format(DeleteIconForUploadedDocumentsCssSelector, row), How.CssSelector);
            Console.WriteLine("Delete uploaded document in Appeal Document");
        }
        #endregion
    }
}
