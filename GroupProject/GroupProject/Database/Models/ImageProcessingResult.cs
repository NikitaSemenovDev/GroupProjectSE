using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GroupProject.Database.Models
{
    public class ImageProcessingResult
    {
        public int Id { get; set; }

        public int? PersonId { get; set; }

        public Person Person { get; set; }

        public byte[] Image { get; set; }

        public DateTime ProcessingDateTime { get; set; }

        /// <summary>
        /// JSON-массив чисел
        /// </summary>
        public string ProcessingResult { get; set; }
    }
}
