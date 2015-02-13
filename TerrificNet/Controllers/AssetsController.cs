using System.Net.Http;
using System.Web.Http;
using TerrificNet.Configuration;
using TerrificNet.ViewEngine;
using TerrificNet.ViewEngine.Config;

namespace TerrificNet.Controllers
{
    public class AssetsController : StaticFileController
    {
        private readonly ITerrificNetConfig _config;

        public AssetsController(ITerrificNetConfig config, IFileSystem fileSystem, ServerConfiguration serverConfiguration) 
			: base(fileSystem, serverConfiguration)
        {
            _config = config;
        }

        [HttpGet]
        public override HttpResponseMessage Get(string path)
        {
            return GetInternal(path);
        }

        protected override string FilePath
        {
            get { return _config.AssetPath; }
        }
    }
}