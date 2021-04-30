using System;
using System.Collections.Generic;
using System.Web.Http;
using NewRelic.WebReporting.Repository;
using NewRelic.WebReporting.Models;
namespace NewRelic.WebReporting.Controllers
{
    public class ReportController : ApiController
    {  
        // GET api/<controller>
        public List<Reports> Get()
        {
            NewRelicRepository repo = new NewRelicRepository();
            return repo.GetReports();
        }
    }
}
