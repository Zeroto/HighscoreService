namespace HighscoreService.Controllers

open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Logging
open Models
open System.Linq
open Microsoft.EntityFrameworkCore
open System.ComponentModel.DataAnnotations

[<CLIMutable>]
type NewUserRequest = {
  [<Required>]
  name: string
}

[<ApiController>]
[<Route("[controller]")>]
type UsersController (logger : ILogger<UsersController>, highscoresContext: HighscoresContext) =
  inherit ControllerBase()

  [<HttpGet("{id}")>]
  member this.Get(id: int) =
    async {
      let! users =
        highscoresContext.Users
          .Where(fun u -> u.id = id)
          .Take(1)
          .ToListAsync()
        |> Async.AwaitTask
      let user = users |> Seq.tryHead
      let result: ActionResult =
        match user with
        | None -> upcast this.NotFound()
        | Some u -> upcast this.Ok(u)
      return result
    } |> Async.StartAsTask
  
  [<HttpPost>]
  member this.Create(user: NewUserRequest) =
    let newUser = {
      id = 0
      name = user.name
      scores = ResizeArray<Score>()
    }
    async {
      do! highscoresContext.Users.AddAsync(newUser) |> Async.AwaitTask |> Async.Ignore
      do! highscoresContext.SaveChangesAsync() |> Async.AwaitTask |> Async.Ignore

      return this.Created(sprintf "/Users/%d" newUser.id, newUser)
    } |> Async.StartAsTask