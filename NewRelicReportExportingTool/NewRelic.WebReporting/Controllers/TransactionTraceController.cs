using System;
using System.Collections.Generic;
using System.Web.Http;
using NewRelic.WebReporting.Repository;
using NewRelic.WebReporting.Models;

namespace NewRelic.WebReporting.Controllers
{
    public class TransactionTraceController : ApiController
    {
        // GET api/<controller>
        public List<TransactionTraces> Get()
        {
            NewRelicRepository repo = new NewRelicRepository();
            return repo.GetTransactionTraces();
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}