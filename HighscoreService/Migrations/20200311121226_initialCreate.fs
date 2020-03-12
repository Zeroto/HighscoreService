﻿// <auto-generated />
namespace HighscoreService.Migrations

open Models
open System
open System.Collections.Generic
open Microsoft.EntityFrameworkCore
open Microsoft.EntityFrameworkCore.Infrastructure
open Microsoft.EntityFrameworkCore.Metadata
open Microsoft.EntityFrameworkCore.Migrations
open Microsoft.EntityFrameworkCore.Migrations.Operations
open Microsoft.EntityFrameworkCore.Migrations.Operations.Builders
open Microsoft.EntityFrameworkCore.Storage.ValueConversion
open Npgsql.EntityFrameworkCore.PostgreSQL.Metadata


type private ScoresTable = {
    id: OperationBuilder<AddColumnOperation>
    name: OperationBuilder<AddColumnOperation>
    value: OperationBuilder<AddColumnOperation>
}

[<DbContext(typeof<HighscoresContext>)>]
[<Migration("20200311121226_initialCreate")>]
type initialCreate() =
    inherit Migration()

    override this.Up(migrationBuilder:MigrationBuilder) =
        migrationBuilder.CreateTable(
            name = "Scores"
            ,columns = (fun table -> 
            {
                id = table.Column<int>(nullable = false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                name = table.Column<string>(nullable = true)
                value = table.Column<int>(nullable = false)
            })
            ,constraints =
                (fun table -> 
                    table.PrimaryKey("PK_Scores", (fun x -> (x.id :> obj))) |> ignore
                ) 
            ) |> ignore


    override this.Down(migrationBuilder:MigrationBuilder) =
        migrationBuilder.DropTable(
            name = "Scores"
            ) |> ignore


    override this.BuildTargetModel(modelBuilder: ModelBuilder) =
        modelBuilder
            .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
            .HasAnnotation("ProductVersion", "2.2.6-servicing-10079") |> ignore

        modelBuilder.Entity("Models.Score", (fun b ->

            b.Property<int>("id")
                .ValueGeneratedOnAdd() |> ignore

            b.Property<string>("name") |> ignore

            b.Property<int>("value")
                .IsRequired() |> ignore


            b.HasKey("id") |> ignore


            b.ToTable("Scores") |> ignore
        )) |> ignore
