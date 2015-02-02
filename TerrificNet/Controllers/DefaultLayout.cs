using System.Collections.Generic;
using TerrificNet.Models;
using TerrificNet.ViewEngine.ViewEngines.TemplateHandler;

namespace TerrificNet.Controllers
{
    static internal class DefaultLayout
    {
        public static ViewDefinition WithDefaultLayout(params ViewDefinition[] content)
        {
            var viewDefinition = new ViewDefinition
            {
                Template = "views/_layouts/_layout",
                Placeholder = new PlaceholderDefinitionCollection
                {
                    {
                        "content", new ViewDefinition[]
                        {
                            new NavigationGroupModel
                            {
                                Template = "views/_layouts/page",
                                Placeholder = new PlaceholderDefinitionCollection
                                {
                                    {
                                        "phContent", content
                                    }
                                }
                            }
                        }
                    }
                }
            };
            return viewDefinition;
        }

        public static ViewDefinition AddAction(this ViewDefinition viewDefinition, ActionModel actionModel)
        {
            var model = (NavigationGroupModel)viewDefinition.Placeholder["content"][0];
            if (model.Actions == null)
                model.Actions = new List<ActionModel>();

            model.Actions.Add(actionModel);

            return viewDefinition;
        }

        public static ViewDefinition IncludeScript(this ViewDefinition viewDefintion, string scriptSource)
        {
            var script = new ScriptImport {Src = scriptSource};

            if (viewDefintion.Scripts == null)
                viewDefintion.Scripts = new List<ScriptImport>();

            viewDefintion.Scripts.Add(script);

            return viewDefintion;
        }

        public static ViewDefinition IncludeStyle(this ViewDefinition viewDefintion, string styleSource)
        {
            var script = new StyleImport { Href = styleSource };

            if (viewDefintion.Styles == null)
                viewDefintion.Styles = new List<StyleImport>();

            viewDefintion.Styles.Add(script);

            return viewDefintion;
        }
    }
}