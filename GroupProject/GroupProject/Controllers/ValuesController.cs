using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using GroupProject.Logging;
using System.IO;
using System.Runtime.CompilerServices;
using GroupProject.Database;
using GroupProject.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace GroupProject.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private ILogger Logger { get; }
        private DatabaseContext Context { get; }

        public ValuesController(ILogger logger, DatabaseContext context)
        {
            Logger = logger;
            Context = context;
        }

        /// <summary>
        /// Get random values
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///     GET /
        /// </remarks>
        /// <returns></returns>
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet]
        [Route("get-all-users")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Person>>> GetUsers()
        {
            return await Context.People.ToListAsync();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}