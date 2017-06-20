using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using contracts.Models;
using hamroktmMainServer.Database.Public;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using Db = hamroktmMainServer.Database.Public;


namespace hamroktmMainServer.Repositories
{
    public interface IAdminRepository
    {
        
    }
    public class AdminRepository : BaseRepository,IAdminRepository
    {
        private DbSet<Db.Category> CategoriesDbSet { get; set; }
        private DbSet<Db.SubCategory> SubCategoryDbSet { get; set; }
        private DbSet<Db.Category_SubCategory> Category_SubCategoryDbSet { get; set; }

        public AdminRepository()
        {
            CategoriesDbSet = hamroktmDb.Categories;
            SubCategoryDbSet = hamroktmDb.SubCategories;
            Category_SubCategoryDbSet = hamroktmDb.Category_SubCategory;
        }
    }
}