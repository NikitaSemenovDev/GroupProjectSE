using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using GroupProjectSE.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace GroupProjectSE.ExternalServices
{
    public class ImageProcessorService
    {
        private HttpClient Client { get; }

        private ILogger Logger { get; }

        public ImageProcessorService(IConfiguration configuration, HttpClient client, ILogger logger)
        {
            Client = client;
            Client.BaseAddress = new Uri(configuration.GetSection("ExternalServices:ImageProcessorServiceUrl").Value);
            Logger = logger;
        }


        /// <summary>
        /// Получение результата обработки изображения
        /// </summary>
        /// <param name="image">Поток байт изображения</param>
        /// <param name="fileName">Название изображения</param>
        /// <returns>Результат обработки изображения</returns>
        public async Task<IEnumerable<double>> GetImageResult(MemoryStream image, string fileName)
        {
            try
            {
                var content = new MultipartFormDataContent();
                var imageContent = new ByteArrayContent(image.ToArray());
                content.Add(imageContent, "image_data", fileName);
                var response = await Client.PostAsync(Client.BaseAddress + "GetPrediction", content);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception("Не удалось получить результат обработки");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var predictions = JsonConvert.DeserializeObject<IEnumerable<double>>(responseContent);
                return predictions;
            }
            catch (Exception e)
            {
                Logger.Error(e.ToString());
                return new List<double>();
            }
        }
    }
}
