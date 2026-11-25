using System;

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcommerceDemo.Modules.Inquiries.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialInquiries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "inquiries");

            migrationBuilder.CreateTable(
                name: "free_class_requests",
                schema: "inquiries",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    owner_name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    phone = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    breed = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    dog_age = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    behavior_problem = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_free_class_requests", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_free_class_requests_created_at",
                schema: "inquiries",
                table: "free_class_requests",
                column: "created_at");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "free_class_requests",
                schema: "inquiries");
        }
    }
}
