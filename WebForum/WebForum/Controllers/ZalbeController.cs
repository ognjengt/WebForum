using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebForum.Helpers;
using WebForum.Models;

namespace WebForum.Controllers
{
    public class ZalbeController : ApiController
    {
        DbOperater dbOperater = new DbOperater();

        [HttpPost]
        [ActionName("PriloziZalbuNaPodforum")]
        public bool PriloziZalbuNaPodforum([FromBody]Zalba zalba)
        {

            return true;
        }

    }
}
