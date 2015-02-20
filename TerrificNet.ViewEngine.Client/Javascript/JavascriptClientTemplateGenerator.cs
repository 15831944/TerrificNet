using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TerrificNet.ViewEngine.Client.Javascript
{
	public class JavascriptClientTemplateGenerator
	{
		private readonly string _templateRepository;
		private readonly ClientTemplateGenerator _templateGenerator;

		public JavascriptClientTemplateGenerator(string templateRepository, ClientTemplateGenerator templateGenerator)
		{
			_templateRepository = templateRepository;
			_templateGenerator = templateGenerator;
		}

		public string Generate(TemplateInfo templateInfo)
		{
			var builder = new StringBuilder();
			using (var writer = new StringWriter(builder))
			{
				var ns = GetFunctionQualifier(_templateRepository);
				var nsBuilder = new StringBuilder();
				writer.Write("var ");
				foreach (var part in ns)
				{
					nsBuilder.Append(part);
					writer.WriteLine("{0} = {0} || {{}};", nsBuilder);
					nsBuilder.Append('.');
				}

				nsBuilder.Remove(nsBuilder.Length - 1, 1);
				writer.Write("{0}[\"{1}\"] = {{ render: function(model) {{ var out = \"\";", nsBuilder, templateInfo.Id);
				var clientContext = new JavascriptClientContext(writer);
				var model = new JavascriptClientModel("model");
				_templateGenerator.GenerateForTemplate(templateInfo, clientContext, model);

				writer.Write("return out; }}");	
			}

			return builder.ToString();
		}

		private IEnumerable<string> GetFunctionQualifier(string typename)
		{
			return typename.Split('.');
		}
	}
}