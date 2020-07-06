using System;
using System.Threading.Tasks;
using BlazorWasmGrpcWithAuth0.Server.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using ProtoBuf.Grpc.Server;

namespace BlazorWasmGrpcWithAuth0.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    Configuration.Bind("auth0", options);

                    options.Events = new JwtBearerEvents
                    {
                        OnForbidden = ctx =>
                        {
                            Console.WriteLine("OnForbidden");
                            return Task.CompletedTask;
                        },

                        OnChallenge = ctx =>
                        {
                            // invalid_token , https://github.com/auth0-samples/auth0-aspnetcore-webapi-samples/issues/13
                            Console.WriteLine("OnChallenge Error: " + ctx.Error);
                            Console.WriteLine("OnChallenge AuthenticateFailure.Message: " + ctx.AuthenticateFailure?.Message);
                            return Task.CompletedTask;
                        },

                        OnAuthenticationFailed = ctx =>
                        {
                            Console.WriteLine("OnAuthenticationFailed: " + ctx.Exception.Message);
                            return Task.CompletedTask;
                        },

                        OnMessageReceived = ctx =>
                        {
                            Console.WriteLine("OnMessageReceived: " + ctx.Token);
                            return Task.CompletedTask;
                        },

                        OnTokenValidated = ctx =>
                        {
                            Console.WriteLine("OnTokenValidated: " + ctx.SecurityToken.Id);
                            return Task.CompletedTask;
                        }
                    };

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        RequireAudience = true,
                        ValidAudience = options.Audience
                    };
                });

            services.AddGrpc();
            services.AddControllers();
            services.AddControllersWithViews();
            services.AddRazorPages();

            services.AddCodeFirstGrpc(config =>
            {
                config.ResponseCompressionLevel = System.IO.Compression.CompressionLevel.Optimal;
            });
            services.AddCodeFirstGrpcReflection();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication(); // UseAuthentication must come before UseAuthorization
            app.UseAuthorization();

            app.UseGrpcWeb(); // Must be added between UseRouting and UseEndpoints

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<UploadFileService>().EnableGrpcWeb();
                //endpoints.MapGrpcService<WeatherService>().EnableGrpcWeb();

                endpoints.MapGrpcService<CounterService>().EnableGrpcWeb();

                endpoints.MapGrpcService<CalculatorService>().EnableGrpcWeb();
                endpoints.MapCodeFirstGrpcReflectionService();

                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html");
            });
        }
    }
}