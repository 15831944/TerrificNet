using System.Collections.Generic;
using Veil.Helper;

namespace TerrificNet.ViewEngine.ViewEngines.TemplateHandler.Grid
{
	internal class GridHelperHandler : IBlockHelperHandler, ITerrificHelperHandler
	{
		private RenderingContext _context;

		private static readonly Dictionary<string, double> DefaultRatioTable = new Dictionary<string, double>
		{
			{"1/4", 0.25},
			{"1/2", 0.5},
			{"3/4", 0.75},
			{"1/3", 0.33},
			{"2/3", 0.66}
		};

		public bool IsSupported(string name)
		{
			return name.StartsWith("grid-cell");
		}

		public void Evaluate(object model, string name, IDictionary<string, string> parameters)
		{
			var gridStack = GridStack.FromContext(_context);
			double ratio = GetValue(parameters, "ratio", 1);
			double margin = GetValue(parameters, "margin", 0);
			double padding = GetValue(parameters, "padding", 0);
			double width = GetValue(parameters, "width", gridStack.Current.Width);

			gridStack.Push((int)(((width - margin) * ratio) - padding));
		}

		private static double GetValue(IDictionary<string, string> parameters, string key, double defaultValue)
		{
			double result = defaultValue;
			string value;
			if (parameters.TryGetValue(key, out value))
			{
				if (!double.TryParse(value, out result) && !DefaultRatioTable.TryGetValue(value, out result))
					result = defaultValue;
			}
			return result;
		}

		public void Leave(object model, string name, IDictionary<string, string> parameters)
		{
			var gridStack = GridStack.FromContext(_context);
			gridStack.Pop();
		}

		public void SetContext(RenderingContext context)
		{
			_context = context;
		}
	}
}