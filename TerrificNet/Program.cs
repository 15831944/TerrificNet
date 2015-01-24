﻿using System;
using System.IO;
using System.Linq;
using Microsoft.Owin.Hosting;
using Microsoft.Practices.Unity;
using TerrificNet.Configuration;
using TerrificNet.Controllers;
using TerrificNet.ModelProviders;
using TerrificNet.UnityModules;
using TerrificNet.ViewEngine;
using TerrificNet.ViewEngine.ViewEngines;

namespace TerrificNet
{
    class Program
    {
        private const string PathArgumentPrefix = "--path=";

        static void Main(string[] args)
        {
            const string baseAddress = "http://+:9000/";

            var path = args.FirstOrDefault(i => i.StartsWith(PathArgumentPrefix));
            if (!string.IsNullOrEmpty(path))
                path = path.Substring(PathArgumentPrefix.Length);
            else
                path = string.Empty;

            var container = new UnityContainer();
            container.RegisterType<ITerrificTemplateHandlerFactory, GenericUnityTerrificTemplateHandlerFactory<DefaultTerrificTemplateHandler>>();
            container.RegisterType<INamingRule, NamingRule>();

            new DefaultUnityModule().Configure(container);

            var configuration = TerrificNetHostConfigurationLoader.LoadConfiguration(Path.Combine(path, "application.json"));
            foreach (var item in configuration.Applications.Values)
            {
                var childContainer = container.CreateChildContainer();

                var app = DefaultUnityModule.RegisterForApplication(childContainer, Path.Combine(path, item.BasePath), item.ApplicationName, item.Section);

                childContainer.RegisterType<IModelProvider, ApplicationOverviewModelProvider>();

                // Replacement for container.RegisterInstance(item.ApplicationName, app)
                container.RegisterType<TerrificNetApplication>(item.ApplicationName, new InjectionFactory(u => app));
            }

            new TerrificBundleUnityModule().Configure(container);

            // Start OWIN host
            using (WebApp.Start(baseAddress, builder => new Startup().Configuration(builder, container)))
            {
                Console.WriteLine("Started on " + baseAddress);
                Console.ReadLine();
            }
        }
    }
}
