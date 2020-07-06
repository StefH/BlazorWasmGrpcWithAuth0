using System;
using System.Threading.Tasks;
using BlazorWasmGrpcWithAuth0.Client.Services;
using BlazorWasmGrpcWithAuth0.Shared.Calculator;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using ProtoBuf.Grpc.Client;

namespace BlazorWasmGrpcWithAuth0.Client.Pages
{
    public partial class Calculator
    {
        [Inject]
        public GrpcChannel Channel { get; set; }

        private int Result;
        private string Error;

        [Inject]
        public GrpcBearerTokenProvider GrpcBearerTokenProvider { get; set; }

        private async Task Start()
        {
            string token = null;
            try
            {
                token = await GrpcBearerTokenProvider.GetTokenAsync();
                Error = token;
                Console.WriteLine("token = " + token);
            }
            catch (AccessTokenNotAvailableException a)
            {
                a.Redirect();
            }

            var calculator = Channel.CreateGrpcService<ICalculator>();
            if (calculator == null)
            {
                Error = "calculator == null";
                Console.WriteLine("calculator = " + calculator);
            }

            //var options = new CallOptions
            //{
            //    Headers = { { "Authorization", $"Bearer {token}" } }
            //};

            var headers = new Metadata
            {
                { "Authorization", $"Bearer {token}" }
            };
            var options = new CallOptions(headers);

            var multiplyResult = await calculator.MultiplyAsync(new MultiplyRequest { X = 12, Y = 4 }, options);

            Result = multiplyResult.Result;
            Console.WriteLine("Result = " + Result);
        }
    }
}
