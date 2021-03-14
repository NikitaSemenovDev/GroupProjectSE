using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GroupProject.Database.Models
{
    public class ImageProcessingResult
    {
        public int Id { get; set; }

        public int? AccountId { get; set; }

        public Account Account { get; set; }

        /// <summary>
        /// JSON-массив чисел int
        /// </summary>
        public string Size { get; set; }

        public byte[] Image { get; set; }

        public DateTime ProcessingDateTime { get; set; }

        /// <summary>
        /// JSON-массив результатов обработки регионов { probabilities, coordinates }
        /// </summary>
        public string RegionsPredictions { get; set; }

        /// <summary>
        /// JSON-массив результата обработки изображения
        /// </summary>
        public string ImagePredictions { get; set; }

        /// <summary>
        /// JSON-массив заболеваний { en_title, ru_title }
        /// </summary>
        public string DiseasesNames { get; set; }
    }
}
