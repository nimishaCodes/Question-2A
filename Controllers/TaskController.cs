using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Producer88.Models;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Producer88.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TaskController : ControllerBase
    {

        [HttpPost]
        public void PostTask([FromBody] TaskModel taskModel)
        {
           /* var obj = new CredentialsModel();
            obj.email = taskModel.email;
            obj.password = taskModel.password;
            VerifyCredentials(VerifyCredentials);*/


            var factory = new ConnectionFactory()
            {
                 HostName = "localhost",
                 Port = 31672
                //HostName = Environment.GetEnvironmentVariable("RABBITMQ_HOST"),
               // Port = Convert.ToInt32(Environment.GetEnvironmentVariable("RABBITMQ_PORT"))
            };
            
                Console.WriteLine(factory.HostName + ":" + factory.Port);
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "TaskQueue",   //create a queue with this name, ahead of doing this
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

                    string message = taskModel.task;
                    var body = System.Text.Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: "",
                                         routingKey: "TaskQueue",
                                         basicProperties: null,
                                         body: body);

                }
        }
        //end verify method
        //private static async Task<string> VerifyCredentials(CredentialsModel credentials)
           public static async Task<string> VerifyCredentials(CredentialsModel credentials)
            {
            var response = string.Empty;
            //Uri uri = new Uri("https://reqres.in/api/login");
            var uri = "https://reqres.in/api/login";
            using (var client = new HttpClient())
            {
                var json = JsonConvert.SerializeObject(credentials);
                var stringContent = new StringContent(json, System.Text.UnicodeEncoding.UTF8, "application/json"); // use MediaTypeNames.Application.Json in Core 3.0+ and Standard 2.1+
                HttpResponseMessage result = await client.PostAsync(uri, stringContent);
                if (result.IsSuccessStatusCode)
                {
                    if (result.Content.Equals("QpwL5tke4Pnpja7X4"))
                        // response = result.StatusCode.ToString();
                        response = "Super";
                }
            }
            return response;
        }
        //end all
    }
}
