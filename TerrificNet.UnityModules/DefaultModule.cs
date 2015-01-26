﻿using System;
using System.Runtime.Serialization;
using Microsoft.Practices.Unity;
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
	        try
	        {
	            var config = ConfigurationLoader.LoadTerrificConfiguration(basePath);
	            var application = new TerrificNetApplication(applicationName, section, config, childContainer);

	            childContainer.RegisterInstance(application);
	            RegisterForConfiguration(childContainer, config);

	            return application;
	        }
	        catch (ConfigurationException ex)
	        {
	            throw new InvalidApplicationException(string.Format("Could not load the configuration for application '{0}'.", applicationName), ex);
	        }
	    }

	    public static void RegisterForConfiguration(IUnityContainer container, ITerrificNetConfig item)
	    {
            container.RegisterInstance(item);
	        RegisterApplicationSpecific(container);
	    }

	    private static void RegisterApplicationSpecific(IUnityContainer container)
		{
			container.RegisterType<IViewEngine, VeilViewEngine>();
			container.RegisterType<ICacheProvider, MemoryCacheProvider>();
            container.RegisterType<IModelProvider, JsonModelProvider>("fallback");

            container.RegisterType<IModelProvider, DefaultModelProvider>(new ContainerControlledLifetimeManager(), new InjectionConstructor(new ResolvedParameter<IModelProvider>("fallback")));

			container.RegisterType<ISchemaProvider, SchemaMergeProvider>(
				new InjectionConstructor(new ResolvedParameter<HandlebarsViewSchemaProvider>(),
					new ResolvedParameter<PhysicalSchemaProvider>()));
			container.RegisterType<IJsonSchemaCodeGenerator, JsonSchemaCodeGenerator>();
		}
	}

    [Serializable]
    public class InvalidApplicationException : Exception
    {
        public InvalidApplicationException()
        {
        }

        public InvalidApplicationException(string message) : base(message)
        {
        }

        public InvalidApplicationException(string message, Exception inner) : base(message, inner)
        {
        }

        protected InvalidApplicationException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
