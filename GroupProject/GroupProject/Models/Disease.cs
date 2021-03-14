using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GroupProject.Models
{
    /// <summary>
    /// Заболевание
    /// </summary>
    public class Disease
    {
        /// <summary>
        /// Название на английском языке
        /// </summary>
        [JsonProperty("en")]
        public string TitleInEnglish { get; set; }

        /// <summary>
        /// Название на русском языке
        /// </summary>
        [JsonProperty("ru")]
        public string TitleInRussian { get; set; }
    }
}
