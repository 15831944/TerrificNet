﻿using System.Collections.Generic;
using Newtonsoft.Json;

namespace TerrificNet.Models
{
    public class DataVariationCollectionModel
    {
        [JsonProperty("variations")]
        public IList<DataVariationModel> Variations { get; set; }
    }
}
