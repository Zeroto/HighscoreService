namespace HighscoreService.Controllers

open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Logging
open Models
open System.Linq
open System
open Microsoft.EntityFrameworkCore

[<ApiController>]
[<Route("[controller]")>]
type HighscoresController (logger : ILogger<HighscoresController>, highscoresContext: HighscoresContext) =
  inherit ControllerBase()

  [<HttpGet>]
  member __.Get ([<FromQuery>]count: Nullable<int>) =
    let count = max 1 (min 100 (defaultArg (count |> Option.ofNullable) 10)) // clamp count between 1 and 100
    async {
      return! highscoresContext.Scores
        .OrderByDescending(fun s -> s.value)
        .Take(count)
        .ToListAsync()
        |> Async.AwaitTask
    } |> Async.StartAsTask
    

  [<HttpPost>]
  member __.Post (score: Models.Score) =
    async {
      do! highscoresContext.Scores.AddAsync(score) |> Async.AwaitTask |> Async.Ignore
      do! highscoresContext.SaveChangesAsync() |> Async.AwaitTask |> Async.Ignore
    } |> Async.StartAsTask
