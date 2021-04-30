using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Encryptor.UI;

namespace Encryptor
{
    static class Program
    {

        public static string EncryptionKey;
        private static EncryptionKeyForm _environmentSelectorForm;
        private static EncryptDecryptForm _mainForm;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new EncryptDecryptForm());
        }

        public static void ShowMainForm()
        {
            _mainForm = new EncryptDecryptForm();
            _mainForm.ShowDialog();
        }
    }
}
