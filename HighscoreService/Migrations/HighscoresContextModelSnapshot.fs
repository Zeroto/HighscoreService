namespace HighscoreService.Migrations

open System
open Microsoft.EntityFrameworkCore
open Microsoft.EntityFrameworkCore.Infrastructure
open Microsoft.EntityFrameworkCore.Metadata
open Microsoft.EntityFrameworkCore.Metadata.Internal
open Microsoft.EntityFrameworkCore.Migrations
open Microsoft.EntityFrameworkCore.Storage
open Microsoft.EntityFrameworkCore.Storage.ValueConversion
open Models
open Npgsql.EntityFrameworkCore.PostgreSQL.Metadata
open Npgsql.EntityFrameworkCore.PostgreSQL.Storage.Internal.Mapping

[<DbContext(typeof<HighscoresContext>)>]
type HighscoresContextModelSnapshot() =
    inherit ModelSnapshot()

    override this.BuildModel(modelBuilder: ModelBuilder) =
        modelBuilder
            .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
            .HasAnnotation("ProductVersion", "2.2.6-servicing-10079") |> ignore

        modelBuilder.Entity("Models.Score", (fun b ->

            b.Property<int>("id")
                .ValueGeneratedOnAdd() |> ignore

            b.Property<int>("userId")
                .IsRequired() |> ignore

            b.Property<int>("value")
                .IsRequired() |> ignore


            b.HasKey("id") |> ignore


            b.HasIndex("userId") |> ignore


            b.ToTable("Scores") |> ignore
        )) |> ignore

        modelBuilder.Entity("Models.User", (fun b ->

            b.Property<int>("id")
                .ValueGeneratedOnAdd() |> ignore

            b.Property<string>("name") |> ignore


            b.HasKey("id") |> ignore


            b.ToTable("Users") |> ignore
        )) |> ignore

        modelBuilder.Entity("Models.Score", (fun b ->

            b.HasOne("Models.User", "user")
                .WithMany("scores")
                .HasForeignKey("userId")
                .OnDelete(DeleteBehavior.Cascade)|> ignore
        )) |> ignore

