using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;
using Microsoft.Data.Entity.Metadata;

namespace MessyLabAdmin.Migrations
{
    public partial class AddedAssignmentTestAndResults : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_Action_Student_StudentID", table: "Action");
            migrationBuilder.DropForeignKey(name: "FK_AssignmentVariant_Assignment_AssignmentID", table: "AssignmentVariant");
            migrationBuilder.DropForeignKey(name: "FK_PasswordReset_Student_StudentID", table: "PasswordReset");
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
                name: "AssignmentTest",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AssignmentVariantID = table.Column<int>(nullable: false),
                    ExpectedOutput = table.Column<string>(nullable: true),
                    GivenInput = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssignmentTest", x => x.ID);
                    table.ForeignKey(
                        name: "FK_AssignmentTest_AssignmentVariant_AssignmentVariantID",
                        column: x => x.AssignmentVariantID,
                        principalTable: "AssignmentVariant",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.CreateTable(
                name: "AssignmentTestResult",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AssignmentTestID = table.Column<int>(nullable: false),
                    CalculatedOutput = table.Column<string>(nullable: true),
                    CreatedTime = table.Column<DateTime>(nullable: false),
                    IsSuccess = table.Column<bool>(nullable: false),
                    SolutionID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssignmentTestResult", x => x.ID);
                    table.ForeignKey(
                        name: "FK_AssignmentTestResult_AssignmentTest_AssignmentTestID",
                        column: x => x.AssignmentTestID,
                        principalTable: "AssignmentTest",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssignmentTestResult_Solution_SolutionID",
                        column: x => x.SolutionID,
                        principalTable: "Solution",
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
                name: "FK_AssignmentVariant_Assignment_AssignmentID",
                table: "AssignmentVariant",
                column: "AssignmentID",
                principalTable: "Assignment",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_PasswordReset_Student_StudentID",
                table: "PasswordReset",
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
                onDelete: ReferentialAction.NoAction);
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
            migrationBuilder.DropForeignKey(name: "FK_AssignmentVariant_Assignment_AssignmentID", table: "AssignmentVariant");
            migrationBuilder.DropForeignKey(name: "FK_PasswordReset_Student_StudentID", table: "PasswordReset");
            migrationBuilder.DropForeignKey(name: "FK_Solution_Assignment_AssignmentID", table: "Solution");
            migrationBuilder.DropForeignKey(name: "FK_Solution_Student_StudentID", table: "Solution");
            migrationBuilder.DropForeignKey(name: "FK_StudentAssignment_Assignment_AssignmentID", table: "StudentAssignment");
            migrationBuilder.DropForeignKey(name: "FK_StudentAssignment_Student_StudentID", table: "StudentAssignment");
            migrationBuilder.DropForeignKey(name: "FK_IdentityRoleClaim<string>_IdentityRole_RoleId", table: "AspNetRoleClaims");
            migrationBuilder.DropForeignKey(name: "FK_IdentityUserClaim<string>_ApplicationUser_UserId", table: "AspNetUserClaims");
            migrationBuilder.DropForeignKey(name: "FK_IdentityUserLogin<string>_ApplicationUser_UserId", table: "AspNetUserLogins");
            migrationBuilder.DropForeignKey(name: "FK_IdentityUserRole<string>_IdentityRole_RoleId", table: "AspNetUserRoles");
            migrationBuilder.DropForeignKey(name: "FK_IdentityUserRole<string>_ApplicationUser_UserId", table: "AspNetUserRoles");
            migrationBuilder.DropTable("AssignmentTestResult");
            migrationBuilder.DropTable("AssignmentTest");
            migrationBuilder.AddForeignKey(
                name: "FK_Action_Student_StudentID",
                table: "Action",
                column: "StudentID",
                principalTable: "Student",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_AssignmentVariant_Assignment_AssignmentID",
                table: "AssignmentVariant",
                column: "AssignmentID",
                principalTable: "Assignment",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_PasswordReset_Student_StudentID",
                table: "PasswordReset",
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
