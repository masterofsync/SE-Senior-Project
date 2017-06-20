using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using hamroktmMainServer.Controllers;
using hamroktmMainServer.Database.Public;
using Microsoft.AspNet.Identity;

namespace hamroktmMainServer.Repositories
{
    public abstract class BaseRepository
    {
        internal hamroktmContext hamroktmDb;

        //internal string currentUser;

        internal BaseRepository()
        {
            hamroktmDb = new hamroktmContext();
            //currentUser = RequestContext.Principal.Identity.GetUserName();
        }

        internal HttpStatusCodeResult SaveChanges()
        {
            var isSaved = hamroktmDb.SaveChanges();

            return isSaved > 0
                ? new HttpStatusCodeResult(HttpStatusCode.OK)
                : new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
    }
}