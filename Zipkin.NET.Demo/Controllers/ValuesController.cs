﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Threading.Tasks;
using DataService;
using Microsoft.AspNetCore.Mvc;
using Zipkin.NET.Instrumentation;
using Zipkin.NET.Instrumentation.WCF;

namespace Zipkin.NET.Demo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ValuesController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // GET api/values
        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> Get()
        {
            var httpClient = _httpClientFactory.CreateClient("tracingClient");
            var httpClient2 = _httpClientFactory.CreateClient("tracingClient2");
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, new Uri("https://jsonplaceholder.typicode.com/todos/1"));
            var httpRequest2 = new HttpRequestMessage(HttpMethod.Get, new Uri("https://jsonplaceholder.typicode.com/todos/2"));

            var resultTask = httpClient.SendAsync(httpRequest);
            var result2Task = httpClient2.SendAsync(httpRequest2);

            var result = await resultTask;
            var result2 = await result2Task;

	        var wcfClient = new DataServiceClient();
			wcfClient.Endpoint.Address = new EndpointAddress("http://localhost:54069/DataService.svc");
			wcfClient.Endpoint.EndpointBehaviors.Add(new ZipkinEndpointBehavior());
	        var wcfResult = await wcfClient.GetDataAsync(1);

            return new string[]
            {
                "result", await result2.Content.ReadAsStringAsync(),
                "result2", await result2.Content.ReadAsStringAsync(),
				"wcfResult", wcfResult
            };
        }
    }
}
