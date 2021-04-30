using System;

namespace Encryptor.Data
{
    [Serializable]
    public static class EnvironmentParameters
    {
        public static string ApplicationUrl { get; set; }

        public static string WebBrowserDriver { get; set; }

        public static string HciAdminUsername { get; set; }

        public static string HciAdminPassword { get; set; }
        
        public static string HciUsername { get; set; }
        
        public static string HciPassword { get; set; }
        
        public static string ClientUserName { get; set; }
        
        public static string ClientPassword { get; set; }

        public static string HciUserWithReadWriteManageAppeals { get; set; }

        public static string HciUserWithReadWriteManageAppealsPassword { get; set; }

        public static string HciUserWithReadOnlyManageAppeals { get; set; }

        public static string HciUserWithReadOnlyManageAppealsPassword { get; set; }

        public static string ClientUserWithReadWriteManageAppeals { get; set; }

        public static string ClientUserWithReadWriteManageAppealsPassword { get; set; }

        public static string UserType { get; set; }
        
        public static string IsInvoicePresent { get; set; }
        
        public static string IsHciUserAuthorizedToManageAppeals { get; set; }
        
        public static string IsClientUserAuthorizedToManageAppeals { get; set; }

        public static string ConnectionString { get; set; }

        public static string TestClient { get; set; }

        public static bool EncryptCredentials{ get; set; }
    }
}
