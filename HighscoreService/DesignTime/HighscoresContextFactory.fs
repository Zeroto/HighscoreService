namespace DesignTime

open Microsoft.EntityFrameworkCore.Design
open Models
open Microsoft.EntityFrameworkCore

type HighscoresContextFactory() =
  interface IDesignTimeDbContextFactory<HighscoresContext> with
    member __.CreateDbContext(args) =
      let optionsBuilder = new DbContextOptionsBuilder<HighscoresContext>()
      optionsBuilder.UseNpgsql("Host=localhost;Database=highscores;Username=postgres") |> ignore

      new HighscoresContext(optionsBuilder.Options);
