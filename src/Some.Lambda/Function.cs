using System;
using OpenTelemetry;
using OpenTelemetry.Instrumentation.AWSLambda;
using OpenTelemetry.Trace;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Microsoft.Extensions.DependencyInjection;
using Overleaf.Lambda;
using System.IO;
using Overleaf.Configuration;
using System.Reflection;


// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace Some.Lambda
{
    public class Function : LambdaBase
    {
        private static readonly TracerProvider _tracerProvider;

        public Function()
        {
            Init();
        }

        static Function()
        {
            _tracerProvider = BuildTraceProvider();
        }

        protected override ServiceProvider ConfigureServices(IServiceCollection services)
        {
            return Bootstrapper.ConfigureServices(services, configuration, logger);
        }

        public async Task<string> FunctionHandler(SQSEvent queueEvent, ILambdaContext context)
            => await AWSLambdaWrapper.TraceAsync(_tracerProvider, HandleAsync, queueEvent, context);
        
        public async Task<string> HandleAsync(SQSEvent queueEvent, ILambdaContext context)
        {
            if (!queueEvent.Records.Any())
            {
                throw new ArgumentException($"{nameof(queueEvent.Records)} ");
            }
            
            // DO YOUR THING
            await Task.Delay(100);
            
            return DateTime.Now.ToString();
        }

        // We should move this method to Lambda.Overleaf later.
        private static TracerProvider BuildTraceProvider()
        {
            var traceProvider = Sdk.CreateTracerProviderBuilder()
                .AddHttpClientInstrumentation(options =>
                {
                    // Ignore lambda runtime HTTP API calls:
                    // http://127.0.0.1:9001/2018-06-01/runtime/invocation/next and so on
                    options.FilterHttpRequestMessage = requestMessage =>
                            requestMessage.RequestUri?.IsLoopback == false;
                })
                .AddAWSInstrumentation()
                .AddOtlpExporter()
                .AddAWSLambdaConfigurations()
                .Build();

            if (traceProvider == null)
                throw new NotSupportedException("TracerProvider should be not null.");

            return traceProvider;
        }
    }
}