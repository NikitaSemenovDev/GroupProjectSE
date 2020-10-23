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

        public byte[] Image { get; set; }

        public DateTime ProcessingDateTime { get; set; }

        /// <summary>
        /// JSON-массив чисел
        /// </summary>
        public string ProcessingResult { get; set; }
    }
}
