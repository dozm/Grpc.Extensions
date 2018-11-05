using Grpc.Core;
using Sample.Messages;
using System;
using static Sample.Services.Service1;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Sample.Grpc.Server.Interceptors;
using Grpc.Core.Interceptors;
using System.Collections.Generic;
using System.Linq;
using Sample.Services;

namespace Sample.Client
{
    class Program
    {
        static IServiceProvider ServiceProvider = BuildServices();
        static void Main(string[] args)
        {
            var channel = GetChannel();
            var interceptors = ServiceProvider.GetService<IEnumerable<Interceptor>>().ToArray();

            // 在 channel 上使用拦截器，再将返回的 CallInvoker 作为客户端的构造参数。
            var callInvoke = channel.Intercept(interceptors);



            //var client = new Service1Client(channel);

            // 客端拦截 创建客户端的方式。
            var client = new Service1Client(callInvoke);
                       

            //var response = callInvoke.AsyncUnaryCall(new Method<Request1, Response1>(MethodType.Unary, "Sample.GrpcService.Service1", "API1",
            //     Marshallers.Create((arg) => Google.Protobuf.MessageExtensions.ToByteArray(arg), Request1.Parser.ParseFrom),
            //     Marshallers.Create((arg) => Google.Protobuf.MessageExtensions.ToByteArray(arg), Response1.Parser.ParseFrom)),
            //     "127.0.0.1", new CallOptions(), new Request1() { Message = "aaaa" }).GetAwaiter().GetResult();



            var response = client.API1(new Request1() { Message = "Hi, Server." });

            Console.WriteLine("response: " + response.Message);

            response = client.API1Async(new Request1() { Message = "Hi, Server." }).GetAwaiter().GetResult();

            Console.WriteLine("response: " + response.Message);

            channel.ShutdownAsync().Wait();

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }


        private static Channel GetChannel()
        {
            var crt = ChannelCredentials.Insecure;

            var target = "172.20.10.12:9001";
            target = "[::1]:9001";
            //target = "127.0.0.1:9001";
            //target = "localhost:9001";
            //fe80::995f:ece5: 2370:f4

            //Console.WriteLine($"\nSelected  {target}\n");

            Channel channel = new Channel(target, crt);

            return channel;

        }

        static IServiceProvider BuildServices()
        {
            var services = new ServiceCollection();
            services.AddLogging(b =>
            {
                b.SetMinimumLevel(LogLevel.Information)
                .AddConsole();
            })
            .AddSingleton<Interceptor, AccessInterceptor>()
            ;

            return services.BuildServiceProvider();
        }
    }
}
