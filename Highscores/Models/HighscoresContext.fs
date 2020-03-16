namespace Models

open Microsoft.EntityFrameworkCore

type [<CLIMutable>] Client = {
  id: System.Guid
  name: string
  secret: string
} with
  static member none = {
    id = System.Guid.Empty
    name = ""
    secret = ""
  }

type [<CLIMutable>] User = {
  id: System.Guid
  clientId: System.Guid
  client: Client
  name: string
  scores: ResizeArray<Score>
}

and [<CLIMutable>] Score = {
  id: System.Guid
  clientId: System.Guid
  client: Client
  user: User
  userId: System.Guid
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

  [<DefaultValue>]
  val mutable clients: DbSet<Client>
  member this.Clients with get() = this.clients and set v = this.clients <- v

  override this.OnModelCreating(modelBuilder) =
    modelBuilder.Entity<User>().HasKey(fun u -> {|clientId = u.clientId; id = u.id|} :> obj) |> ignore
    modelBuilder.Entity<Score>().HasKey(fun s -> {|clientId = s.clientId; id = s.id|} :> obj) |> ignore
