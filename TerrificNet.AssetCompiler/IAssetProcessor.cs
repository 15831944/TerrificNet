﻿
using System.Collections.Generic;
using System.Threading.Tasks;
using TerrificNet.AssetCompiler.Processors;

namespace TerrificNet.AssetCompiler
{
    public interface IAssetProcessor
    {
        Task<string> ProcessAsync(KeyValuePair<string, string[]> assetConfig, ProcessorFlags flags);
    }
}
