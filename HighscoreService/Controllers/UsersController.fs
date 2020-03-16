namespace HighscoreService.Controllers

open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Logging
open Models
open System.Linq
open Microsoft.EntityFrameworkCore
open System.ComponentModel.DataAnnotations
open System.Security.Claims

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
  member this.Get(id: System.Guid) =
    async {
      let! users =
        highscoresContext.Users
          .Where(fun u -> u.id = id)
          .Include(fun u -> u.scores)
          .Take(1)
          .ToListAsync()
        |> Async.AwaitTask
      let user = users |> Seq.tryHead
      let result: ActionResult =
        match user with
        | None -> upcast this.NotFound()
        | Some u -> upcast this.Ok({| id = u.id
                                      name = u.name
                                      scores = u.scores
                                    |})
      return result
    } |> Async.StartAsTask
  
  [<HttpPost>]
  member this.Create(user: NewUserRequest) =
    let clientId = this.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value |> System.Guid.Parse

    let newUser = {
      id = System.Guid.NewGuid()
      clientId = clientId
      client = Client.none
      name = user.name
      scores = ResizeArray<Score>()
    }
    async {
      do! highscoresContext.Users.AddAsync(newUser).AsTask() |> Async.AwaitTask |> Async.Ignore
      do! highscoresContext.SaveChangesAsync() |> Async.AwaitTask |> Async.Ignore

      return this.Created(sprintf "/Users/%O" newUser.id, {|id = newUser.id; name = newUser.name|})
    } |> Async.StartAsTask