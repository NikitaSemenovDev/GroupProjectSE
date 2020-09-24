using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
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
        /// <returns>Результат обработки изображения</returns>
        public async Task<IEnumerable<double>> GetImageResult(Stream image)
        {
            try
            {
                var content = new MultipartFormDataContent();
                content.Add(new StreamContent(image), "image_data");
                var response = await Client.PostAsync(Client.BaseAddress + "/GetPrediction", content);
                var responseContent = await response.Content.ReadAsStringAsync();
                var predictions = JsonConvert.DeserializeObject<List<double>>(responseContent);
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
