using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using TodoApi.Models;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    public class ItemsController : Controller
    {
		private static List<TodoItem> items;
		private static int counter;

		private static readonly JsonSerializer Serializer = new JsonSerializer
		{
			ContractResolver = new CamelCasePropertyNamesContractResolver()
		};

		// GET api/values
		[HttpGet]
        public TodoItem[] Get()
        {
			if (items == null)
			{
				items = new List<TodoItem> {
					new TodoItem { Id=1, Name="TODO Item 1", Notes="notes"},
					new TodoItem { Id=2, Name="TODO Item 2", Notes="notes"},
					new TodoItem { Id=0, Name="TODO Item 0", Notes="notes"},
				};
				counter = 3;
			}

			//var stringContent = JToken.FromObject(todoitems, Serializer).ToString();
			return items.ToArray();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public TodoItem Get(int id)
        {
            return items.FirstOrDefault(i => i.Id == id);
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]TodoItem item)
        {
			item.Id = counter++;
			items.Add(item);
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]TodoItem value)
        {
			var item = items.FirstOrDefault(i => i.Id==id);
			if (item != null)
			{
				var currIndex = items.IndexOf(item);
				items[currIndex] = value;
			}
		}

		// DELETE api/values/5
		[HttpDelete("{id}")]
        public void Delete(int id)
        {
			var item = items.FirstOrDefault(i => i.Id == id);
			if (item != null)
			{
				var currIndex = items.IndexOf(item);
				items.RemoveAt(currIndex);
			}
		}
    }
}
