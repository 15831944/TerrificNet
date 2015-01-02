﻿using TerrificNet.ViewEngine.Config;

namespace TerrificNet.Configuration
{
	public class TerrificNetConfig : ITerrificNetConfig
	{
		public string BasePath { get; set; }
		public string ViewPath { get; set; }
		public string ModulePath { get; set; }
		public string AssetPath { get; set; }
		public string DataPath { get; set; }
	}
}