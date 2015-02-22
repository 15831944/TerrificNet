using System.IO;
using System.Web;

namespace TerrificNet.ViewEngine.Client.Javascript
{
	public class JavascriptClientContext : IClientContext
	{
		private readonly TextWriter _writer;

		public JavascriptClientContext(string templateId, TextWriter writer)
		{
		    TemplateId = templateId;
		    _writer = writer;
		}

	    public string TemplateId { get; private set; }

	    public void WriteLiteral(string content)
		{
			_writer.Write("out += \"");
			_writer.Write(HttpUtility.JavaScriptStringEncode(content));
			_writer.Write("\";");
		}

		public void WriteExpression(IClientModel model)
		{
			_writer.Write("out += ");
			_writer.Write(model.ToString());
			_writer.Write(";");
		}

		public IClientModel BeginIterate(IClientModel model)
		{
			var itemVariable = new JavascriptClientModel("item");
			_writer.Write("for (var i = 0; i < {1}.length; i++){{ var {0} = {1}[i]; ", itemVariable, model);

			return itemVariable;
		}

		public void EndIterate()
		{
			_writer.Write("}");
		}

		public void BeginIf(IClientModel model)
		{
			_writer.Write("if ({0}){{", model);
		}

		public void EndIf()
		{
			_writer.Write("}");
		}

		public void ElseIf()
		{
			_writer.Write("} else {");
		}
	}
}