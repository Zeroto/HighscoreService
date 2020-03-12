namespace Models

open Microsoft.EntityFrameworkCore

type [<CLIMutable>] User = {
  id: int
  name: string
  scores: ResizeArray<Score>
}

and [<CLIMutable>] Score = {
  id: int
  user: User
  userId: int
  value: int
}

type HighscoresContext =
  inherit DbContext

  new() = { inherit DbContext() }
  new(options: DbContextOptions<HighscoresContext>) = { inherit DbContext(options) }

  [<DefaultValue>]
  val mutable scores: DbSet<Score>
  member this.Scores with get() = this.scores and set v = this.scores <- v

  [<DefaultValue>]
  val mutable users: DbSet<User>
  member this.Users with get() = this.users and set v = this.users <- v
