﻿using Microsoft.Practices.Unity;
using TerrificNet.Generator;
using TerrificNet.ViewEngine;
using TerrificNet.ViewEngine.Cache;
using TerrificNet.ViewEngine.Config;
using TerrificNet.ViewEngine.ModelProviders;
using TerrificNet.ViewEngine.SchemaProviders;
using TerrificNet.ViewEngine.ViewEngines;

namespace TerrificNet.UnityModules
{
	public class DefaultUnityModule : IUnityModule
	{
	    public void Configure(IUnityContainer container)
		{
            container.RegisterType<ITemplateRepository, TerrificTemplateRepository>();
		}

	    public static TerrificNetApplication RegisterForApplication(IUnityContainer childContainer, string basePath, string applicationName, string section)
	    {
	        var config = ConfigurationLoader.LoadTerrificConfiguration(basePath);
	        var application = new TerrificNetApplication(applicationName, section, config, childContainer);

            // Replacement for childContainer.RegisterInstance(application);
            childContainer.RegisterType<TerrificNetApplication>(new InjectionFactory(u => application));
	        RegisterForConfiguration(childContainer, config);
	        
            return application;
	    }

	    public static void RegisterForConfiguration(IUnityContainer container, ITerrificNetConfig item)
	    {
            // Replacement for container.RegisterInstance(item);
            container.RegisterType<ITerrificNetConfig>(new InjectionFactory(u => item));
	        RegisterApplicationSpecific(container);
	    }

	    private static void RegisterApplicationSpecific(IUnityContainer container)
		{
			container.RegisterType<IViewEngine, VeilViewEngine>();
			container.RegisterType<ICacheProvider, MemoryCacheProvider>();
			container.RegisterType<IModelProvider, DefaultModelProvider>();
            container.RegisterType<IModelProvider, JsonModelProvider>("json");
			container.RegisterType<ISchemaProvider, SchemaMergeProvider>(
				new InjectionConstructor(new ResolvedParameter<HandlebarsViewSchemaProvider>(),
					new ResolvedParameter<PhysicalSchemaProvider>()));
			container.RegisterType<IJsonSchemaCodeGenerator, JsonSchemaCodeGenerator>();
		}
	}
}
