using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            Client.DefaultRequestHeaders.Accept.Clear();
            Client.BaseAddress = new Uri(configuration.GetSection("ExternalServices:ImageProcessorServiceUrl").Value);
            Logger = logger;
        }


        /// <summary>
        /// Получение результата обработки изображения
        /// </summary>
        /// <param name="image">Поток байт изображения</param>
        /// <returns>Результат обработки изображения</returns>
        public async Task<IEnumerable<double>> GetImageResult(byte[] byts)
        {
            try
            {
                var content = new MultipartFormDataContent();
                var imageContent = new ByteArrayContent(byts, 0, byts.Length);
                imageContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpg");
                content.Add(imageContent, "image_data", "image.jpg");
                var response = await Client.PostAsync(Client.BaseAddress + "GetPrediction", content);
                var responseContent = response.Content.ReadAsStringAsync().Result;
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
