﻿using System.Text;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using Newtonsoft.Json;
using Miya.Core.Entities.Log;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Miya.Core.MessageQueue.RabbitMQ
{
    public class PageEntryLogPublisher
    {
        private readonly IConfiguration _configuration;
        //private readonly HttpContext _context;
        private PageAccessLogModel _pageAccessLogModel;
        public PageEntryLogPublisher(IConfiguration configuration,
                                     PageAccessLogModel pageAccesslogModel
                                    /*, HttpContext context*/)
        {
            _configuration = configuration;
            _pageAccessLogModel = pageAccesslogModel;
            //_context = context;
        }

        public  void PageEntryLogPublish(ActionExecutingContext context)
        {
            var pageAccessLog = JsonConvert.SerializeObject(_pageAccessLogModel);
            var value = _configuration.GetConnectionString("RedisInstanceName");
            var queueName = _configuration.GetSection("RabbitMQLog").GetSection("PageLogQueue").Value;
            var hostName = _configuration.GetSection("RabbitMQLog").GetSection("HostName").Value;
            var factory = new ConnectionFactory()
            {
                HostName = hostName
            };

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: queueName,
                                    durable: false,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);
                    //var user = context.HttpContext.Session.Get<SessionUserModel>("CurrentUser");
                    //string message = "Hello World!";
                    //var body = Encoding.UTF8.GetBytes(message);
                    var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(_pageAccessLogModel));
                    channel.BasicPublish(exchange: "",
                                                    routingKey: queueName,
                                                    basicProperties: null,
                                                    body: body);

                }
            }

        }
    }
}
