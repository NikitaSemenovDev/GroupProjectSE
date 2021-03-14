using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GroupProject.Models
{
    /// <summary>
    /// Результат обработки региона
    /// </summary>
    public class RegionPrediction
    {
        /// <summary>
        /// Вероятности
        /// </summary>
        [JsonProperty(PropertyName = "disease_probabilities")]
        public double[] Probabilities { get; set; }

        /// <summary>
        /// Координаты региона
        /// </summary>
        [JsonProperty(PropertyName = "coordinates")]
        public double[] Coordinates { get; set; }
    }
}
