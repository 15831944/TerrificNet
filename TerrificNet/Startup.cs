using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dependencies;
using System.Web.Http.Dispatcher;
using System.Web.Http.Routing;
using Microsoft.Practices.Unity;
using Owin;
using TerrificNet.UnityModule;
using Unity.WebApi;

namespace TerrificNet
{
	public class Startup
	{
		// This code configures Web API. The Startup class is specified as a type
		// parameter in the WebApp.Start method.
		public void Configuration(IAppBuilder appBuilder, IUnityContainer container)
		{
			// Configure Web API for self-host. 
			var config = new HttpConfiguration();

			//config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "ViewIndex",
                routeTemplate: "web/index.html",
                defaults: new { controller = "viewIndex", section="web/" }
                );

            config.Routes.MapHttpRoute(
                name: "ViewIndexDefault",
                routeTemplate: "",
                defaults: new { controller = "viewIndex", section = "web/" }
            );


            MapArea(config, "web/");
            MapArea(config);


            config.DependencyResolver = new UnityDependencyResolver(container);
		    config.MessageHandlers.Add(new InjectHttpRequestMessageToContainerHandler());
            config.Services.Replace(typeof(IHttpControllerActivator), new ApplicationSpecificControllerActivator(container));

			appBuilder.UseWebApi(config);
		}

	    private static void MapArea(HttpConfiguration config, string section = null)
	    {
	        IHttpRoute route;
	        config.Routes.MapHttpRoute(
	            name: "ModelRoot" + section,
	            routeTemplate: section + "model/{*path}",
	            defaults: new {controller = "model", section = section }
	            );
	        config.Routes.MapHttpRoute(
	            name: "SchemaRoot" + section,
                routeTemplate: section + "schema/{*path}",
                defaults: new { controller = "schema", section = section }
	            );
			config.Routes.MapHttpRoute(
				name: "GenerateRoot" + section,
				routeTemplate: section + "generate/{*path}",
				defaults: new { controller = "generate", section = section }
				);
			config.Routes.MapHttpRoute(
				name: "AssetsRoot" + section,
				routeTemplate: section + "assets/{*path}",
				defaults: new { controller = "assets", section = section }
				);
			config.Routes.MapHttpRoute(
				name: "BundleRoot" + section,
				routeTemplate: section + "bundle_{name}",
				defaults: new { controller = "bundle", section = section }
				);
	        config.Routes.MapHttpRoute(
                name: section + "TemplateRoot" + section,
	            routeTemplate: section + "{*path}",
                defaults: new { controller = "template", section = section }
	            );
	    }
	}

    public class ApplicationSpecificControllerActivator : IHttpControllerActivator
    {
        private readonly IUnityContainer _container;

        public ApplicationSpecificControllerActivator(IUnityContainer container)
        {
            _container = container;
        }

        public IHttpController Create(HttpRequestMessage request, HttpControllerDescriptor controllerDescriptor, Type controllerType)
        {
            var applications = _container.ResolveAll<TerrificNetApplication>();
            var section = (string) request.GetRouteData().Route.Defaults["section"] ?? string.Empty;

            var application = applications.FirstOrDefault(a => a.Configuration.Section == section);
            if (application == null)
                throw new InvalidOperationException(string.Format("Could not find a application for the section '{0}'.", section));

            return (IHttpController) application.Container.Resolve(controllerType);
        }
    }

    public class InjectHttpRequestMessageToContainerHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var container = request.GetDependencyScope().GetService(typeof(IUnityContainer)) as IUnityContainer;
            if (container != null)
                container.RegisterInstance(request);

            return base.SendAsync(request, cancellationToken);
        }
    }
}