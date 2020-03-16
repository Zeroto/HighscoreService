namespace HighscoreService

open System
open System.Collections.Generic
open System.Linq
open System.Threading.Tasks
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.HttpsPolicy;
open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Models
open Microsoft.EntityFrameworkCore
open Microsoft.AspNetCore.Authentication
open BasicAuthenticationHandler

type Startup private () =
    new (configuration: IConfiguration) as this =
        Startup() then
        this.Configuration <- configuration

    // This method gets called by the runtime. Use this method to add services to the container.
    member this.ConfigureServices(services: IServiceCollection): unit =
        // Add framework services.
        services.AddControllers() |> ignore
        services.AddDbContext<HighscoresContext>(fun options -> options.UseNpgsql(this.Configuration.GetConnectionString("HighscoreDatabase"), fun options -> options.MigrationsAssembly("Migrations") |> ignore) |> ignore) |> ignore
        services.AddAuthentication("BasicAuthentication")
          .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null) |> ignore

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    member this.Configure(app: IApplicationBuilder, env: IWebHostEnvironment) =
        if (env.IsDevelopment()) then
            app.UseDeveloperExceptionPage() |> ignore

        app.UseHttpsRedirection() |> ignore
        app.UseRouting() |> ignore

        app.UseAuthentication() |> ignore
        app.UseAuthorization() |> ignore

        app.UseEndpoints(fun endpoints ->
            endpoints.MapControllers().RequireAuthorization() |> ignore
            ) |> ignore

    member val Configuration : IConfiguration = null with get, set
