module BasicAuthenticationHandler

open Microsoft.AspNetCore.Authentication
open Microsoft.Extensions.Logging
open Microsoft.Extensions.Options
open System.Threading.Tasks
open Models
open System.Net.Http.Headers
open System
open System.Linq
open Microsoft.EntityFrameworkCore
open System.Security.Claims

type BasicAuthenticationHandler(options: IOptionsMonitor<AuthenticationSchemeOptions>, 
                                logger: ILoggerFactory,
                                encoder: System.Text.Encodings.Web.UrlEncoder,
                                clock: ISystemClock,
                                highscoresContext: HighscoresContext) =
  inherit AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder, clock)

  override this.HandleAuthenticateAsync(): Task<AuthenticateResult> =
    let request = this.Request
    async {
      if not (request.Headers.ContainsKey("Authorization")) then
        return AuthenticateResult.Fail("Missing Authorization Header")
      else
        let authHeader = AuthenticationHeaderValue.Parse(request.Headers.["Authorization"].ToString());
        let credentialBytes = Convert.FromBase64String(authHeader.Parameter);
        let credentials = System.Text.Encoding.UTF8.GetString(credentialBytes).Split(":", 2);
        let (success, username) = System.Guid.TryParse(credentials.[0]);
        let password = credentials.[1];

        if not success then
          return AuthenticateResult.Fail("Invalid client id or key")
        else
          let! clients =
            highscoresContext.Clients
              .Where(fun c -> c.id = username && c.secret = password)
              .ToListAsync()
              |> Async.AwaitTask

          if clients |> Seq.length <> 1 then
            return AuthenticateResult.Fail("Invalid client id or key")
          else
            let identity = new ClaimsIdentity([Claim(ClaimTypes.NameIdentifier, username.ToString())], this.Scheme.Name)
            let principal = new ClaimsPrincipal(identity)
            let ticket = new AuthenticationTicket(principal, this.Scheme.Name)
            return AuthenticateResult.Success(ticket)
    } |> Async.StartAsTask