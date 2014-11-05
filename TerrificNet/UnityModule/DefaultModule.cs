﻿using System.IO;
using System.Net.Http;
using Microsoft.Practices.Unity;
using TerrificNet.Generator;
using TerrificNet.ViewEngine;
using TerrificNet.ViewEngine.Config;
using TerrificNet.ViewEngine.ModelProviders;
using TerrificNet.ViewEngine.SchemaProviders;
using TerrificNet.ViewEngine.ViewEngines;

namespace TerrificNet.UnityModule
{
	public class DefaultUnityModule : IUnityModue
	{
		private readonly string _path;

		public DefaultUnityModule(string path)
		{
			_path = path;
		}

	    public void Configure(IUnityContainer container)
	    {
			container.RegisterInstance<ITerrificNetConfig>("config", new TerrificNetConfig
			{
				BasePath = _path,
				ViewPath = Path.Combine(_path, "views"),
				AssetPath = Path.Combine(_path, "assets"),
				DataPath = Path.Combine(_path, "project/data"),
			});

            container.RegisterInstance<ITerrificNetConfig>("internalConfig", new TerrificNetConfig
            {
                BasePath = _path,
                ViewPath = "Web/views",
                AssetPath = "Web/assets",
                DataPath = "Web/data",
            });

		    container.RegisterType<ITerrificNetConfig>(new InjectionFactory(c =>
		    {
		        var message = c.Resolve<HttpRequestMessage>();
		        var routeData = message.GetRouteData();

		        if ((string) routeData.Values["section"] == "web/")
		            return c.Resolve<ITerrificNetConfig>("internalConfig");

                return c.Resolve<ITerrificNetConfig>("config");
		    }));

			container.RegisterType<IViewEngine, NustachePhysicalViewEngine>();
			container.RegisterType<IModelProvider,JsonModelProvier>();
			container.RegisterType<ISchemaProvider, SchemaMergeProvider>(new InjectionConstructor(new ResolvedParameter<NustacheViewSchemaProvider>(), new ResolvedParameter<BaseSchemaProvider>()));
			container.RegisterType<IJsonSchemaCodeGenerator, JsonSchemaCodeGenerator>();
		}

		private class TerrificNetConfig : ITerrificNetConfig
		{
			public string BasePath { get; set; }
			public string ViewPath { get; set; }
			public string AssetPath { get; set; }
			public string DataPath { get; set; }
		}
	}

	public interface IUnityModue
	{
		void Configure(IUnityContainer container);
	}
}
