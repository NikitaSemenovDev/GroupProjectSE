using GroupProject.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace GroupProject.Models
{
    /// <summary>
    /// Модель результата обработки изображения в сервисе
    /// </summary>
    public class ServiceImageProcessingResult
    {
        /// <summary>
        /// Идентификатор результата обработки изображения
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public int? Id { get; set; }

        /// <summary>
        /// Время обработки изображения
        /// </summary>
        [JsonProperty(PropertyName = "datetime")]
        public DateTime ProcessingDateTime { get; set; }

        /// <summary>
        /// Обрабатываемое изображение
        /// </summary>
        [JsonProperty(PropertyName = "image", ItemConverterType = typeof(ImageBase64Converter))]
        public byte[] Image { get; set; }

        /// <summary>
        /// Результат обработки изображения
        /// </summary>
        [JsonProperty(PropertyName = "probabilities")]
        public double[] ProcessingResult { get; set; }
    }
}
