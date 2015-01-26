using Newtonsoft.Json.Linq;

namespace TerrificNet.ViewEngine.ViewEngines.TemplateHandler
{
    public class DefaultTerrificTemplateHandler : ITerrificTemplateHandler
    {
        private readonly IViewEngine _viewEngine;
        private readonly IModelProvider _modelProvider;
        private readonly ITemplateRepository _templateRepository;

        public DefaultTerrificTemplateHandler(IViewEngine viewEngine, IModelProvider modelProvider,
            ITemplateRepository templateRepository)
        {
            _viewEngine = viewEngine;
            _modelProvider = modelProvider;
            _templateRepository = templateRepository;
        }

        public void RenderPlaceholder(object model, string key, RenderingContext context)
        {
            ViewDefinition definition;
            var tmp = model as JObject;
            if (tmp != null)
            {
                definition = tmp.ToObject<ViewDefinition>();
            }
            else
            {
                definition = model as ViewDefinition;
            }

            if (definition == null || definition.Placeholder == null)
                return;

            var placeholder = definition.Placeholder;

            PlaceholderDefinition[] definitions;

            if (!placeholder.TryGetValue(key, out definitions))
                return;

            foreach (var placeholderConfig in definitions)
            {
                var templateName = placeholderConfig.Template;

                string skin = placeholderConfig.Skin;

                TemplateInfo templateInfo;
                IView view;
                if (_templateRepository.TryGetTemplate(templateName, skin, out templateInfo) &&
                    _viewEngine.TryCreateView(templateInfo, out view))
                {
                    var moduleModel = placeholderConfig.Data ?? _modelProvider.GetDefaultModelForTemplate(templateInfo) ?? placeholderConfig;
                    view.Render(moduleModel, context);
                }
                else
                    context.Writer.Write("Problem loading template " + templateName +
                              (!string.IsNullOrEmpty(skin) ? "-" + skin : string.Empty));
            }
        }

        public void RenderModule(string templateName, string skin, RenderingContext context)
        {
            TemplateInfo templateInfo;
            IView view;
            if (_templateRepository.TryGetTemplate(templateName, skin, out templateInfo) &&
                _viewEngine.TryCreateView(templateInfo, out view))
            {
                var moduleModel = _modelProvider.GetDefaultModelForTemplate(templateInfo);
                view.Render(moduleModel, context);
            }
            else
                context.Writer.Write("Problem loading template " + templateName + (!string.IsNullOrEmpty(skin) ? "-" + skin : string.Empty));
        }
    }
}