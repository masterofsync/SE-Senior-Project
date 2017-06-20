using Microsoft.Practices.Unity;
using System.Web.Http;
using hamroktmMainServer.Controllers;
using hamroktmMainServer.Repositories;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler;
using Unity.WebApi;

namespace hamroktmMainServer
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();
            
            // register all your components with the container here
            // it is NOT necessary to register your controllers
            
            // e.g. container.RegisterType<ITestService, TestService>();

            container.RegisterType<IAccountRepository, AccountRepository>();
            container.RegisterType<IAdminRepository,AdminRepository>();
            container.RegisterType<IAdRepository,AdRepository>();
            container.RegisterType<ICategoryRepository,CategoryRepository>();
            container.RegisterType<ICommentRepository,CommentRepository>();
            container.RegisterType<ISearchRepository,SearchRepository>();
            
            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
            GlobalConfiguration.Configuration.DependencyResolver = new Unity.WebApi.UnityDependencyResolver(container);
        }
    }
}