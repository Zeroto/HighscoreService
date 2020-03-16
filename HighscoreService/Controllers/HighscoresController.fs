namespace HighscoreService.Controllers

open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Logging
open Models
open System.Linq
open System
open Microsoft.EntityFrameworkCore
open System.Security.Claims

type ScoreResponse = {
  id: System.Guid
  value: int
  user:
    {| id: System.Guid
       name: string
    |}
}

[<ApiController>]
[<Route("[controller]")>]
type HighscoresController (logger : ILogger<HighscoresController>, highscoresContext: HighscoresContext) =
  inherit ControllerBase()

  [<HttpGet>]
  member __.Get ([<FromQuery>]count: Nullable<int>) =
    let count = max 1 (min 100 (defaultArg (count |> Option.ofNullable) 10)) // clamp count between 1 and 100
    async {
      let! scores =
        (highscoresContext.Scores
          .OrderByDescending(fun s -> s.value)
          .Take(count)
          .Include(fun s -> s.user)
          .ToListAsync()) |> Async.AwaitTask

      return
        scores
        |> Seq.map (fun s -> 
          { id = s.id
            value = s.value
            user =
              {|
                id = s.userId
                name = s.user.name
              |}
          }
        )
    } |> Async.StartAsTask
    

  [<HttpPost>]
  member this.Post (score: Models.Score) =
    let clientId = this.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value |> System.Guid.Parse
    let newScore = {score with clientId = clientId}
    async {
      do! highscoresContext.Scores.AddAsync(newScore).AsTask() |> Async.AwaitTask |> Async.Ignore
      do! highscoresContext.SaveChangesAsync() |> Async.AwaitTask |> Async.Ignore
    } |> Async.StartAsTask
