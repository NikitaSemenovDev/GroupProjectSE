using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GroupProject.ActionResults
{
    public class ProjectJsonResult : IActionResult
    {
        private object Data { get; }

        public ProjectJsonResult(object data)
        {
            Data = data;
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            string serializedData = JsonConvert.SerializeObject(Data);
            context.HttpContext.Response.ContentType = "application/json";
            await context.HttpContext.Response.WriteAsync(serializedData);
        }
    }
}
