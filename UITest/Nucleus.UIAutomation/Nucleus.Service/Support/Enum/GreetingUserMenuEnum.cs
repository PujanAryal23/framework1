using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIAutomation.Framework.Utils;

namespace Nucleus.Service.Support.Enum
{
    public enum UserGreetingMenuEnum
    {

        [StringValue("Dashboard", "Dashboard")]
        Dashboard = 0,

        [StringValue("Quicklinks", "Quicklinks")]
        Quicklinks = 1,

        [StringValue("Help Center", "Help Center")]
        HelpCenter = 2,

        [StringValue("My Profile", "My Profile")]
        MyProfile = 3,

        [StringValue("Sign Out", "Sign Out")]
        SignOut = 4

    }

    public enum QuickLinksEnum
    {

        [StringValue("Cotiviti Links", "Cotiviti Links")]
        CotivitiLinks = 0,

        [StringValue("CMS", "CMS")]
        CMS = 1,

        [StringValue("Cotiviti Flagged Providers", "Cotiviti Flagged Providers ")]
        CotivitiFlaggedProviders = 2,

        [StringValue("Client Flagged Providers", "Client Flagged Providers ")]
        ClientFlaggedProviders = 3
    }

    public enum CotivitiLinksEnum
    {

        [StringValue("About Cotiviti", "About Cotiviti")]
        AboutCotiviti = 0,

        [StringValue("Cotiviti Resources", "Cotiviti Resources")]
        CotivitiResources = 1,

        [StringValue("Cotiviti Blog", "Cotiviti Blog")]
        CotivitiBlog = 2
    }
}
