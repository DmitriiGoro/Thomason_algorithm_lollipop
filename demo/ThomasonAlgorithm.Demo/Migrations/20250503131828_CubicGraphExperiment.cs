using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ThomasonAlgorithm.Demo.Migrations
{
    /// <inheritdoc />
    public partial class CubicGraphExperiment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "cubic_graph_experiments",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    vertices_number = table.Column<int>(type: "integer", nullable: false),
                    K_low = table.Column<int>(type: "integer", nullable: false),
                    K_up = table.Column<int>(type: "integer", nullable: false),
                    max_chord_length = table.Column<int>(type: "integer", nullable: false),
                    lollipop_steps_number = table.Column<int>(type: "integer", nullable: false),
                    chord_lengths = table.Column<Dictionary<int, int>>(type: "jsonb", nullable: false),
                    adjacency_matrix = table.Column<int[,]>(type: "jsonb", nullable: false),
                    timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cubic_graph_experiments", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cubic_graph_experiments");
        }
    }
}
