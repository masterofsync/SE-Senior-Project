using System.Web.Mvc;
using hamroktmMainClient.Adapter;
using hamroktmMainClient.Proxies;
using Microsoft.Practices.Unity;
using Unity.Mvc5;

namespace hamroktmMainClient
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();
            
            // register all your components with the container here
            // it is NOT necessary to register your controllers
            
            // e.g. container.RegisterType<ITestService, TestService>();

            container.RegisterType<IAccountProxy, AccountProxy>();
            container.RegisterType<IAdminProxy, AdminProxy>();
            container.RegisterType<IAdProxy, AdProxy>();
            container.RegisterType<ICategoryProxy, CategoryProxy>();
            container.RegisterType<ICommentProxy,CommentProxy>();
            container.RegisterType<ISearchProxy,SearchProxy>();
            container.RegisterType<ISendEmailProxy,SendEmailProxy>();
            container.RegisterType<IProxyAdapter,ProxyAdapter>();
    
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}