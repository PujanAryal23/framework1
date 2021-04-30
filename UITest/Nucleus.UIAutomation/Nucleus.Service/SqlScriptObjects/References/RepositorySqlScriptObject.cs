using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nucleus.Service.SqlScriptObjects.References
{
    public class RepositorySqlScriptObject
    {
        public const string GetAddOnTableValueList =
            @"select * from REPOSITORY.PCI_EDIT_ADDON where code between '{0}' and '{1}' and dataset in ('CPT_SET' , 'CMS_SET')  and rownum=1";

        public const string GetAddOnProcTableValueList =
            @"select * from REPOSITORY.PCI_EDIT_ADDONPROC 
                where code between '{0}' and '{1}' and dataset in ('CPT_SET' , 'CMS_SET')  and rownum=1";

        public const string GetUnbTableValueList =
            @"SELECT * FROM (Select * from repository.pci_edit_unb where dataset IN ('{0}')) WHERE rownum = 1";

        public const string GetAddOnProcTableValueListForPrimaryProcCode =
            @"select * from REPOSITORY.PCI_EDIT_ADDONPROC 
                where prmproc = '{0}' and dataset in ('CPT_SET' , 'CMS_SET') order by dataset";



    }
}
