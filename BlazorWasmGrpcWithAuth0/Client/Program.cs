using System;
using System.Net.Http;
using System.Threading.Tasks;
using BlazorWasmGrpcWithAuth0.Client.Services;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorWasmGrpcWithAuth0.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddHttpClient("BlazorWasmGrpcWithAuth0.ServerAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
                .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

            // Supply HttpClient instances that include access tokens when making requests to the server project
            builder.Services.AddTransient(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("BlazorWasmGrpcWithAuth0.ServerAPI"));

            builder.Services.AddAuth0Authentication(options =>
            {
                // Configure your authentication provider options here. For more information, see https://aka.ms/blazor-standalone-auth
                builder.Configuration.Bind("auth0", options.ProviderOptions);

                Console.WriteLine("Audience = " + options.ProviderOptions.Audience);
                // The callback url is : https://localhost:5001/authentication/login-callback
                // Make sure to add this to the Auth0 allowed callback urls !
            });

            builder.Services.AddGrpcBearerTokenProvider();

            builder.Services.AddScoped(services =>
            {
                // Create a channel with a GrpcWebHandler that is addressed to the backend server. GrpcWebText is used because server streaming requires it.
                var httpHandler = new GrpcWebHandler(GrpcWebMode.GrpcWebText, new HttpClientHandler());
                return GrpcChannel.ForAddress(builder.HostEnvironment.BaseAddress, new GrpcChannelOptions { HttpHandler = httpHandler });
            });

            await builder.Build().RunAsync();
        }
    }
}
