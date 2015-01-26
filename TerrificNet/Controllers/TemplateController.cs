﻿using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;
using TerrificNet.ViewEngine;
using TerrificNet.ViewEngine.Config;
using TerrificNet.ViewEngine.ViewEngines;
using TerrificNet.ViewEngine.ViewEngines.TemplateHandler;

namespace TerrificNet.Controllers
{
	public class TemplateController : TemplateControllerBase
	{
		private readonly IViewEngine _viewEngine;
		private readonly IModelProvider _modelProvider;
		private readonly ITemplateRepository _templateRepository;
	    private readonly ITerrificNetConfig _configuration;
	    private readonly IFileSystem _fileSystem;

	    public TemplateController(IViewEngine viewEngine, IModelProvider modelProvider, ITemplateRepository templateRepository,
            ITerrificNetConfig configuration, IFileSystem fileSystem)
		{
			_viewEngine = viewEngine;
			_modelProvider = modelProvider;
			_templateRepository = templateRepository;
		    _configuration = configuration;
	        _fileSystem = fileSystem;
		}

		[HttpGet]
		public HttpResponseMessage Get(string path, string skin = null, string data = null)
		{
		    path = path ?? "index";

			IView view;
			TemplateInfo templateInfo;
		    if (!_templateRepository.TryGetTemplate(path, skin, out templateInfo) ||
		        !_viewEngine.TryCreateView(templateInfo, out view))
		    {
		        var fileName = Path.ChangeExtension(Path.Combine(_configuration.ViewPath, path), "html.json");
		        if (_fileSystem.FileExists(fileName))
		        {
                    using (var reader = new JsonTextReader(_fileSystem.OpenRead(fileName)))
                    {
                        var viewDefinition = new JsonSerializer().Deserialize<ViewDefinition>(reader);
                        if (viewDefinition != null)
                        {
                            return Get(viewDefinition.Template, viewDefinition);
                        }
                    }
		        }

		        return new HttpResponseMessage(HttpStatusCode.NotFound);
		    }

            object model;
            if (!string.IsNullOrEmpty(data))
                model = _modelProvider.GetModelForTemplate(templateInfo, data);
            else
                model = _modelProvider.GetDefaultModelForTemplate(templateInfo);

			return View(view, model);
		}

	    private HttpResponseMessage Get(string path, object data)
	    {
	        IView view;
			TemplateInfo templateInfo;
	        if (!_templateRepository.TryGetTemplate(path, null, out templateInfo) ||
	            !_viewEngine.TryCreateView(templateInfo, out view))
	            return new HttpResponseMessage(HttpStatusCode.NotFound);

	        return View(view, data);
	    }
	}
}
