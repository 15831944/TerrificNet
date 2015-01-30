using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Microsoft.Practices.Unity;
using TerrificNet.Models;
using TerrificNet.UnityModules;
using TerrificNet.ViewEngine;
using TerrificNet.ViewEngine.ViewEngines.TemplateHandler;

namespace TerrificNet.Controllers
{
    public class DataEditController : TemplateControllerBase
    {
        private readonly TerrificNetApplication[] _applications;

        public DataEditController(TerrificNetApplication[] applications)
        {
            _applications = applications;
        }

        [HttpGet]
        public HttpResponseMessage Index(string id, string app)
        {
            var templateRepository = ResolveForApp<ITemplateRepository>(app);
            TemplateInfo templateInfo;
            if (!templateRepository.TryGetTemplate(id, null, out templateInfo))
                return null;

            var viewDefinition = IncludeResources(DefaultLayout.WithDefaultLayout(GetDataEditor()));

            return View(viewDefinition.Template, viewDefinition);
        }

        [HttpGet]
        public HttpResponseMessage IndexAdvanced()
        {
            var viewDefinition = IncludeResources(DefaultLayout.WithDefaultLayout(new ViewDefinition
            {
                Template = "components/modules/AdvancedEditor/AdvancedEditor",
                Placeholder = new PlaceholderDefinitionCollection
                {
                    {"rightPanel", new [] { GetDataEditor() } }
                }
            }));

            return View(viewDefinition.Template, viewDefinition);
        }

        private ViewDefinition GetDataEditor()
        {
            return new ViewDefinition
            {
                Template = "components/modules/DataEditor/DataEditor",
                Data = GetVariations()
            };
        }

        private static ViewDefinition IncludeResources(ViewDefinition layout)
        {
            return layout
                .IncludeScript("/web/assets/jsoneditor.min.js")
                .IncludeScript("/web/assets/common.js")
                .IncludeStyle("//cdnjs.cloudflare.com/ajax/libs/font-awesome/4.0.3/css/font-awesome.css");
        }

        private T ResolveForApp<T>(string applicationName)
        {
            applicationName = applicationName ?? string.Empty;
            var application = _applications.First(a => a.Section == applicationName);

            return application.Container.Resolve<T>();
        }

        private DataVariationCollectionModel GetVariations()
        {
            return new DataVariationCollectionModel
            {
                Variations = new List<DataVariationModel>
                {
                    new DataVariationModel
                    {
                        Link = "",
                        Name = "Variation1"
                    },
                    new DataVariationModel
                    {
                        Link = "",
                        Name = "Variation2"
                    }
                }
            };
        }

    }
}
