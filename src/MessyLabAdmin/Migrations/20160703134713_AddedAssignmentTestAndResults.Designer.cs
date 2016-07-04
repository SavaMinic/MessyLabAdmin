using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Migrations;
using MessyLabAdmin.Models;

namespace MessyLabAdmin.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20160703134713_AddedAssignmentTestAndResults")]
    partial class AddedAssignmentTestAndResults
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0-rc1-16348")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("MessyLabAdmin.Models.Action", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedTime");

                    b.Property<string>("Data");

                    b.Property<int>("StudentID");

                    b.Property<int>("Type");

                    b.HasKey("ID");
                });

            modelBuilder.Entity("MessyLabAdmin.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id");

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("NormalizedUserName")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasAnnotation("MaxLength", 256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasAnnotation("Relational:Name", "EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .HasAnnotation("Relational:Name", "UserNameIndex");

                    b.HasAnnotation("Relational:TableName", "AspNetUsers");
                });

            modelBuilder.Entity("MessyLabAdmin.Models.Assignment", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CreatedById");

                    b.Property<DateTime>("CreatedTime");

                    b.Property<string>("Description")
                        .IsRequired();

                    b.Property<DateTime>("EndTime");

                    b.Property<bool>("IsActive");

                    b.Property<int>("SelectEnrollmentNumberModulo");

                    b.Property<int?>("SelectEnrollmentYear");

                    b.Property<int?>("SelectStatus");

                    b.Property<DateTime>("StartTime");

                    b.Property<string>("Title")
                        .IsRequired();

                    b.HasKey("ID");
                });

            modelBuilder.Entity("MessyLabAdmin.Models.AssignmentTest", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AssignmentVariantID");

                    b.Property<string>("ExpectedOutput");

                    b.Property<string>("GivenInput");

                    b.HasKey("ID");
                });

            modelBuilder.Entity("MessyLabAdmin.Models.AssignmentTestResult", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AssignmentTestID");

                    b.Property<string>("CalculatedOutput");

                    b.Property<DateTime>("CreatedTime");

                    b.Property<bool>("IsSuccess");

                    b.Property<int>("SolutionID");

                    b.HasKey("ID");
                });

            modelBuilder.Entity("MessyLabAdmin.Models.AssignmentVariant", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AssignmentID");

                    b.Property<string>("Text");

                    b.HasKey("ID");
                });

            modelBuilder.Entity("MessyLabAdmin.Models.PasswordReset", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedTime");

                    b.Property<bool>("IsUsed");

                    b.Property<string>("RequestCode")
                        .IsRequired();

                    b.Property<int>("StudentID");

                    b.HasKey("ID");
                });

            modelBuilder.Entity("MessyLabAdmin.Models.Solution", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AssignmentID");

                    b.Property<string>("Code")
                        .IsRequired();

                    b.Property<DateTime>("CreatedTime");

                    b.Property<bool>("IsEvaluated");

                    b.Property<int>("StudentID");

                    b.HasKey("ID");
                });

            modelBuilder.Entity("MessyLabAdmin.Models.Student", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("EnrollmentNumber");

                    b.Property<int>("EnrollmentYear");

                    b.Property<string>("FirstName")
                        .IsRequired();

                    b.Property<string>("InitialPassword");

                    b.Property<bool>("IsActive");

                    b.Property<long>("LastLoginTimestamp");

                    b.Property<string>("LastName")
                        .IsRequired();

                    b.Property<string>("PasswordHash")
                        .IsRequired();

                    b.Property<string>("Username")
                        .IsRequired();

                    b.HasKey("ID");
                });

            modelBuilder.Entity("MessyLabAdmin.Models.StudentAssignment", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AssignmentID");

                    b.Property<int>("AssignmentVariantIndex");

                    b.Property<int?>("SolutionID");

                    b.Property<int>("StudentID");

                    b.HasKey("ID");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityRole", b =>
                {
                    b.Property<string>("Id");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("NormalizedName")
                        .HasAnnotation("MaxLength", 256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .HasAnnotation("Relational:Name", "RoleNameIndex");

                    b.HasAnnotation("Relational:TableName", "AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:TableName", "AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:TableName", "AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasAnnotation("Relational:TableName", "AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasAnnotation("Relational:TableName", "AspNetUserRoles");
                });

            modelBuilder.Entity("MessyLabAdmin.Models.Action", b =>
                {
                    b.HasOne("MessyLabAdmin.Models.Student")
                        .WithMany()
                        .HasForeignKey("StudentID");
                });

            modelBuilder.Entity("MessyLabAdmin.Models.Assignment", b =>
                {
                    b.HasOne("MessyLabAdmin.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("CreatedById");
                });

            modelBuilder.Entity("MessyLabAdmin.Models.AssignmentTest", b =>
                {
                    b.HasOne("MessyLabAdmin.Models.AssignmentVariant")
                        .WithMany()
                        .HasForeignKey("AssignmentVariantID");
                });

            modelBuilder.Entity("MessyLabAdmin.Models.AssignmentTestResult", b =>
                {
                    b.HasOne("MessyLabAdmin.Models.AssignmentTest")
                        .WithMany()
                        .HasForeignKey("AssignmentTestID");

                    b.HasOne("MessyLabAdmin.Models.Solution")
                        .WithMany()
                        .HasForeignKey("SolutionID");
                });

            modelBuilder.Entity("MessyLabAdmin.Models.AssignmentVariant", b =>
                {
                    b.HasOne("MessyLabAdmin.Models.Assignment")
                        .WithMany()
                        .HasForeignKey("AssignmentID");
                });

            modelBuilder.Entity("MessyLabAdmin.Models.PasswordReset", b =>
                {
                    b.HasOne("MessyLabAdmin.Models.Student")
                        .WithMany()
                        .HasForeignKey("StudentID");
                });

            modelBuilder.Entity("MessyLabAdmin.Models.Solution", b =>
                {
                    b.HasOne("MessyLabAdmin.Models.Assignment")
                        .WithMany()
                        .HasForeignKey("AssignmentID")
						.OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("MessyLabAdmin.Models.Student")
                        .WithMany()
                        .HasForeignKey("StudentID");
                });

            modelBuilder.Entity("MessyLabAdmin.Models.StudentAssignment", b =>
                {
                    b.HasOne("MessyLabAdmin.Models.Assignment")
                        .WithMany()
                        .HasForeignKey("AssignmentID");

                    b.HasOne("MessyLabAdmin.Models.Solution")
                        .WithMany()
                        .HasForeignKey("SolutionID");

                    b.HasOne("MessyLabAdmin.Models.Student")
                        .WithMany()
                        .HasForeignKey("StudentID");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNet.Identity.EntityFramework.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("MessyLabAdmin.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("MessyLabAdmin.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNet.Identity.EntityFramework.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId");

                    b.HasOne("MessyLabAdmin.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId");
                });
        }
    }
}
