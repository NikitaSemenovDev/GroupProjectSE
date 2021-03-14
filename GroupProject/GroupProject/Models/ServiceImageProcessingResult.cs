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
        /// Размер изображения
        /// </summary>
        [JsonProperty(PropertyName = "size")]
        public IEnumerable<int> Size { get; set; }

        /// <summary>
        /// Обрабатываемое изображение
        /// </summary>
        [JsonProperty(PropertyName = "image", ItemConverterType = typeof(ImageBase64Converter))]
        public byte[] Image { get; set; }

        /// <summary>
        /// Время обработки изображения
        /// </summary>
        [JsonProperty(PropertyName = "datetime")]
        public DateTime ProcessingDateTime { get; set; }

        /// <summary>
        /// Результат обработки регионов 
        /// </summary>
        [JsonProperty(PropertyName = "prediction_by_crops")]
        public IEnumerable<RegionPrediction> RegionsPredictions { get; set; }

        /// <summary>
        /// Результат обработки изображения
        /// </summary>
        [JsonProperty(PropertyName = "prediction_for_image")]
        public IEnumerable<double> ImagePredictions { get; set; }

        /// <summary>
        /// Заболевания
        /// </summary>
        [JsonProperty(PropertyName = "disease_names")]
        public IEnumerable<Disease> DiseasesNames { get; set; }
    }
}
