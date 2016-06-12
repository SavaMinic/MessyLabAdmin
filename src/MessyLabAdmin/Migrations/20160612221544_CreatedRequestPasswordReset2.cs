using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;
using Microsoft.Data.Entity.Metadata;

namespace MessyLabAdmin.Migrations
{
    public partial class CreatedRequestPasswordReset2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_Action_Student_StudentID", table: "Action");
            migrationBuilder.DropForeignKey(name: "FK_Solution_Assignment_AssignmentID", table: "Solution");
            migrationBuilder.DropForeignKey(name: "FK_Solution_Student_StudentID", table: "Solution");
            migrationBuilder.DropForeignKey(name: "FK_StudentAssignment_Assignment_AssignmentID", table: "StudentAssignment");
            migrationBuilder.DropForeignKey(name: "FK_StudentAssignment_Student_StudentID", table: "StudentAssignment");
            migrationBuilder.DropForeignKey(name: "FK_IdentityRoleClaim<string>_IdentityRole_RoleId", table: "AspNetRoleClaims");
            migrationBuilder.DropForeignKey(name: "FK_IdentityUserClaim<string>_ApplicationUser_UserId", table: "AspNetUserClaims");
            migrationBuilder.DropForeignKey(name: "FK_IdentityUserLogin<string>_ApplicationUser_UserId", table: "AspNetUserLogins");
            migrationBuilder.DropForeignKey(name: "FK_IdentityUserRole<string>_IdentityRole_RoleId", table: "AspNetUserRoles");
            migrationBuilder.DropForeignKey(name: "FK_IdentityUserRole<string>_ApplicationUser_UserId", table: "AspNetUserRoles");
            migrationBuilder.CreateTable(
                name: "PasswordReset",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedTime = table.Column<DateTime>(nullable: false),
                    IsUsed = table.Column<bool>(nullable: false),
                    RequestCode = table.Column<string>(nullable: false),
                    StudentID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasswordReset", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PasswordReset_Student_StudentID",
                        column: x => x.StudentID,
                        principalTable: "Student",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.AddForeignKey(
                name: "FK_Action_Student_StudentID",
                table: "Action",
                column: "StudentID",
                principalTable: "Student",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_Solution_Assignment_AssignmentID",
                table: "Solution",
                column: "AssignmentID",
                principalTable: "Assignment",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_Solution_Student_StudentID",
                table: "Solution",
                column: "StudentID",
                principalTable: "Student",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_StudentAssignment_Assignment_AssignmentID",
                table: "StudentAssignment",
                column: "AssignmentID",
                principalTable: "Assignment",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_StudentAssignment_Student_StudentID",
                table: "StudentAssignment",
                column: "StudentID",
                principalTable: "Student",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityRoleClaim<string>_IdentityRole_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityUserClaim<string>_ApplicationUser_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityUserLogin<string>_ApplicationUser_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityUserRole<string>_IdentityRole_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityUserRole<string>_ApplicationUser_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_Action_Student_StudentID", table: "Action");
            migrationBuilder.DropForeignKey(name: "FK_Solution_Assignment_AssignmentID", table: "Solution");
            migrationBuilder.DropForeignKey(name: "FK_Solution_Student_StudentID", table: "Solution");
            migrationBuilder.DropForeignKey(name: "FK_StudentAssignment_Assignment_AssignmentID", table: "StudentAssignment");
            migrationBuilder.DropForeignKey(name: "FK_StudentAssignment_Student_StudentID", table: "StudentAssignment");
            migrationBuilder.DropForeignKey(name: "FK_IdentityRoleClaim<string>_IdentityRole_RoleId", table: "AspNetRoleClaims");
            migrationBuilder.DropForeignKey(name: "FK_IdentityUserClaim<string>_ApplicationUser_UserId", table: "AspNetUserClaims");
            migrationBuilder.DropForeignKey(name: "FK_IdentityUserLogin<string>_ApplicationUser_UserId", table: "AspNetUserLogins");
            migrationBuilder.DropForeignKey(name: "FK_IdentityUserRole<string>_IdentityRole_RoleId", table: "AspNetUserRoles");
            migrationBuilder.DropForeignKey(name: "FK_IdentityUserRole<string>_ApplicationUser_UserId", table: "AspNetUserRoles");
            migrationBuilder.DropTable("PasswordReset");
            migrationBuilder.AddForeignKey(
                name: "FK_Action_Student_StudentID",
                table: "Action",
                column: "StudentID",
                principalTable: "Student",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_Solution_Assignment_AssignmentID",
                table: "Solution",
                column: "AssignmentID",
                principalTable: "Assignment",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_Solution_Student_StudentID",
                table: "Solution",
                column: "StudentID",
                principalTable: "Student",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_StudentAssignment_Assignment_AssignmentID",
                table: "StudentAssignment",
                column: "AssignmentID",
                principalTable: "Assignment",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_StudentAssignment_Student_StudentID",
                table: "StudentAssignment",
                column: "StudentID",
                principalTable: "Student",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityRoleClaim<string>_IdentityRole_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityUserClaim<string>_ApplicationUser_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityUserLogin<string>_ApplicationUser_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityUserRole<string>_IdentityRole_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityUserRole<string>_ApplicationUser_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
