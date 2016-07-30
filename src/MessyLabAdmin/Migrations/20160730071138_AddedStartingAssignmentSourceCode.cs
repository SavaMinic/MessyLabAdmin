using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;

namespace MessyLabAdmin.Migrations
{
    public partial class AddedStartingAssignmentSourceCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_Action_Student_StudentID", table: "Action");
            migrationBuilder.DropForeignKey(name: "FK_AssignmentTest_AssignmentVariant_AssignmentVariantID", table: "AssignmentTest");
            migrationBuilder.DropForeignKey(name: "FK_AssignmentTestResult_AssignmentTest_AssignmentTestID", table: "AssignmentTestResult");
            migrationBuilder.DropForeignKey(name: "FK_AssignmentTestResult_Solution_SolutionID", table: "AssignmentTestResult");
            migrationBuilder.DropForeignKey(name: "FK_AssignmentVariant_Assignment_AssignmentID", table: "AssignmentVariant");
            migrationBuilder.DropForeignKey(name: "FK_PasswordReset_Student_StudentID", table: "PasswordReset");
            migrationBuilder.DropForeignKey(name: "FK_Solution_Student_StudentID", table: "Solution");
            migrationBuilder.DropForeignKey(name: "FK_StudentAssignment_Assignment_AssignmentID", table: "StudentAssignment");
            migrationBuilder.DropForeignKey(name: "FK_StudentAssignment_Student_StudentID", table: "StudentAssignment");
            migrationBuilder.DropForeignKey(name: "FK_IdentityRoleClaim<string>_IdentityRole_RoleId", table: "AspNetRoleClaims");
            migrationBuilder.DropForeignKey(name: "FK_IdentityUserClaim<string>_ApplicationUser_UserId", table: "AspNetUserClaims");
            migrationBuilder.DropForeignKey(name: "FK_IdentityUserLogin<string>_ApplicationUser_UserId", table: "AspNetUserLogins");
            migrationBuilder.DropForeignKey(name: "FK_IdentityUserRole<string>_IdentityRole_RoleId", table: "AspNetUserRoles");
            migrationBuilder.DropForeignKey(name: "FK_IdentityUserRole<string>_ApplicationUser_UserId", table: "AspNetUserRoles");
            migrationBuilder.AlterColumn<int>(
                name: "AssignmentID",
                table: "Solution",
                nullable: true);
            migrationBuilder.AddColumn<string>(
                name: "StartingCode",
                table: "Assignment",
                nullable: false,
                defaultValue: "");
            migrationBuilder.AddForeignKey(
                name: "FK_Action_Student_StudentID",
                table: "Action",
                column: "StudentID",
                principalTable: "Student",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_AssignmentTest_AssignmentVariant_AssignmentVariantID",
                table: "AssignmentTest",
                column: "AssignmentVariantID",
                principalTable: "AssignmentVariant",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_AssignmentTestResult_AssignmentTest_AssignmentTestID",
                table: "AssignmentTestResult",
                column: "AssignmentTestID",
                principalTable: "AssignmentTest",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_AssignmentTestResult_Solution_SolutionID",
                table: "AssignmentTestResult",
                column: "SolutionID",
                principalTable: "Solution",
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
            migrationBuilder.DropForeignKey(name: "FK_AssignmentTest_AssignmentVariant_AssignmentVariantID", table: "AssignmentTest");
            migrationBuilder.DropForeignKey(name: "FK_AssignmentTestResult_AssignmentTest_AssignmentTestID", table: "AssignmentTestResult");
            migrationBuilder.DropForeignKey(name: "FK_AssignmentTestResult_Solution_SolutionID", table: "AssignmentTestResult");
            migrationBuilder.DropForeignKey(name: "FK_AssignmentVariant_Assignment_AssignmentID", table: "AssignmentVariant");
            migrationBuilder.DropForeignKey(name: "FK_PasswordReset_Student_StudentID", table: "PasswordReset");
            migrationBuilder.DropForeignKey(name: "FK_Solution_Student_StudentID", table: "Solution");
            migrationBuilder.DropForeignKey(name: "FK_StudentAssignment_Assignment_AssignmentID", table: "StudentAssignment");
            migrationBuilder.DropForeignKey(name: "FK_StudentAssignment_Student_StudentID", table: "StudentAssignment");
            migrationBuilder.DropForeignKey(name: "FK_IdentityRoleClaim<string>_IdentityRole_RoleId", table: "AspNetRoleClaims");
            migrationBuilder.DropForeignKey(name: "FK_IdentityUserClaim<string>_ApplicationUser_UserId", table: "AspNetUserClaims");
            migrationBuilder.DropForeignKey(name: "FK_IdentityUserLogin<string>_ApplicationUser_UserId", table: "AspNetUserLogins");
            migrationBuilder.DropForeignKey(name: "FK_IdentityUserRole<string>_IdentityRole_RoleId", table: "AspNetUserRoles");
            migrationBuilder.DropForeignKey(name: "FK_IdentityUserRole<string>_ApplicationUser_UserId", table: "AspNetUserRoles");
            migrationBuilder.DropColumn(name: "StartingCode", table: "Assignment");
            migrationBuilder.AlterColumn<int>(
                name: "AssignmentID",
                table: "Solution",
                nullable: false);
            migrationBuilder.AddForeignKey(
                name: "FK_Action_Student_StudentID",
                table: "Action",
                column: "StudentID",
                principalTable: "Student",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_AssignmentTest_AssignmentVariant_AssignmentVariantID",
                table: "AssignmentTest",
                column: "AssignmentVariantID",
                principalTable: "AssignmentVariant",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_AssignmentTestResult_AssignmentTest_AssignmentTestID",
                table: "AssignmentTestResult",
                column: "AssignmentTestID",
                principalTable: "AssignmentTest",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_AssignmentTestResult_Solution_SolutionID",
                table: "AssignmentTestResult",
                column: "SolutionID",
                principalTable: "Solution",
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
