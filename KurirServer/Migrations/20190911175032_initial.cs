using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace KurirServer.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DeliveryTypes",
                columns: table => new
                {
                    DeliveryTypeID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DeliveryTypeName = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryTypes", x => x.DeliveryTypeID);
                });

            migrationBuilder.CreateTable(
                name: "PaymentTypes",
                columns: table => new
                {
                    PaymentTypeID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PaymentTypeName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentTypes", x => x.PaymentTypeID);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    RoleID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RoleName = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.RoleID);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FirstName = table.Column<string>(nullable: false),
                    LastName = table.Column<string>(nullable: false),
                    Phone = table.Column<string>(nullable: false),
                    Mail = table.Column<string>(nullable: false),
                    Pass = table.Column<string>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    RegistrationDate = table.Column<DateTime>(nullable: false),
                    ActiveUserRoleID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserID);
                });

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    LocationID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Latitude = table.Column<double>(nullable: false),
                    Longitude = table.Column<double>(nullable: false),
                    Altitude = table.Column<double>(nullable: false),
                    DToffSet = table.Column<DateTimeOffset>(nullable: false),
                    UserID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.LocationID);
                    table.ForeignKey(
                        name: "FK_Locations_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserRoleID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserID = table.Column<int>(nullable: false),
                    RoleID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => x.UserRoleID);
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleID",
                        column: x => x.RoleID,
                        principalTable: "Roles",
                        principalColumn: "RoleID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Deliveries",
                columns: table => new
                {
                    DeliveryID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserID = table.Column<int>(nullable: false),
                    DispatcherID = table.Column<int>(nullable: false),
                    CourierID = table.Column<int>(nullable: false),
                    NameStart = table.Column<string>(nullable: true),
                    NameEnd = table.Column<string>(nullable: true),
                    PhoneOfStart = table.Column<string>(nullable: true),
                    PhoneOfEnd = table.Column<string>(nullable: true),
                    StartAddress = table.Column<string>(nullable: true),
                    EndAddress = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    StartLocationID = table.Column<int>(nullable: true),
                    EndLocationID = table.Column<int>(nullable: true),
                    ZoneStart = table.Column<int>(nullable: false),
                    ZoneEnd = table.Column<int>(nullable: false),
                    WaitingInMinutes = table.Column<int>(nullable: false),
                    DeliveryPrice = table.Column<decimal>(nullable: false),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    StartTime = table.Column<DateTime>(nullable: false),
                    EndTime = table.Column<DateTime>(nullable: false),
                    DeliveryTypeID = table.Column<int>(nullable: false),
                    PaymentTypeID = table.Column<int>(nullable: false),
                    DeliveryStatus = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Deliveries", x => x.DeliveryID);
                    table.ForeignKey(
                        name: "FK_Deliveries_DeliveryTypes_DeliveryTypeID",
                        column: x => x.DeliveryTypeID,
                        principalTable: "DeliveryTypes",
                        principalColumn: "DeliveryTypeID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Deliveries_Locations_EndLocationID",
                        column: x => x.EndLocationID,
                        principalTable: "Locations",
                        principalColumn: "LocationID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Deliveries_PaymentTypes_PaymentTypeID",
                        column: x => x.PaymentTypeID,
                        principalTable: "PaymentTypes",
                        principalColumn: "PaymentTypeID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Deliveries_Locations_StartLocationID",
                        column: x => x.StartLocationID,
                        principalTable: "Locations",
                        principalColumn: "LocationID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Deliveries_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "DeliveryTypes",
                columns: new[] { "DeliveryTypeID", "DeliveryTypeName" },
                values: new object[,]
                {
                    { 1, "Regular" },
                    { 2, "Return shipping" },
                    { 3, "Shoping" },
                    { 4, "Post service" }
                });

            migrationBuilder.InsertData(
                table: "PaymentTypes",
                columns: new[] { "PaymentTypeID", "PaymentTypeName" },
                values: new object[,]
                {
                    { 1, "Kes" },
                    { 2, "Preko racuna" },
                    { 3, "Kupon" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RoleID", "RoleName" },
                values: new object[,]
                {
                    { 1, "SuperAdmin" },
                    { 2, "Admin" },
                    { 3, "User" },
                    { 4, "Courier" },
                    { 5, "Dispatcher" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserID", "ActiveUserRoleID", "FirstName", "IsActive", "LastName", "Mail", "Pass", "Phone", "RegistrationDate" },
                values: new object[,]
                {
                    { 1, 0, "Marko", false, "Mirkovic", "marko@gmail.com", "lol", "023771642", new DateTime(2019, 9, 11, 19, 50, 32, 11, DateTimeKind.Local).AddTicks(882) },
                    { 2, 0, "Max", false, "Fast", "max@gmail.com", "lol", "023771642", new DateTime(2019, 9, 11, 19, 50, 32, 14, DateTimeKind.Local).AddTicks(5265) },
                    { 3, 0, "Matija", false, "Levai", "Matija@gmail.com", "lol", "023771642", new DateTime(2019, 9, 11, 19, 50, 32, 14, DateTimeKind.Local).AddTicks(5300) }
                });

            migrationBuilder.InsertData(
                table: "Deliveries",
                columns: new[] { "DeliveryID", "CourierID", "CreateTime", "DeliveryPrice", "DeliveryStatus", "DeliveryTypeID", "Description", "DispatcherID", "EndAddress", "EndLocationID", "EndTime", "NameEnd", "NameStart", "PaymentTypeID", "PhoneOfEnd", "PhoneOfStart", "StartAddress", "StartLocationID", "StartTime", "UserID", "WaitingInMinutes", "ZoneEnd", "ZoneStart" },
                values: new object[,]
                {
                    { 1, 2, new DateTime(2019, 9, 10, 12, 14, 0, 0, DateTimeKind.Unspecified), 160m, 0, 1, null, 3, "Temerinska 12/2", null, new DateTime(2019, 9, 11, 19, 50, 32, 15, DateTimeKind.Local).AddTicks(5992), "marko", "Nikola", 1, "0623339992", "0612889085", "Kosovska 1/2", null, new DateTime(2019, 9, 10, 12, 24, 0, 0, DateTimeKind.Unspecified), 1, 0, 1, 1 },
                    { 2, 2, new DateTime(2019, 9, 10, 12, 14, 0, 0, DateTimeKind.Unspecified), 160m, 0, 1, null, 3, "Kosovska 12/2", null, new DateTime(2019, 9, 11, 19, 50, 32, 15, DateTimeKind.Local).AddTicks(6502), "marija", "Nina", 1, "0623339992", "0612889085", "Temerinska 1/2", null, new DateTime(2019, 9, 10, 12, 24, 0, 0, DateTimeKind.Unspecified), 1, 0, 1, 1 }
                });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "UserRoleID", "RoleID", "UserID" },
                values: new object[,]
                {
                    { 6, 3, 1 },
                    { 7, 3, 2 },
                    { 8, 4, 2 },
                    { 9, 5, 2 },
                    { 1, 1, 3 },
                    { 2, 2, 3 },
                    { 3, 3, 3 },
                    { 4, 4, 3 },
                    { 5, 5, 3 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Deliveries_DeliveryTypeID",
                table: "Deliveries",
                column: "DeliveryTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Deliveries_EndLocationID",
                table: "Deliveries",
                column: "EndLocationID");

            migrationBuilder.CreateIndex(
                name: "IX_Deliveries_PaymentTypeID",
                table: "Deliveries",
                column: "PaymentTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Deliveries_StartLocationID",
                table: "Deliveries",
                column: "StartLocationID");

            migrationBuilder.CreateIndex(
                name: "IX_Deliveries_UserID",
                table: "Deliveries",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Locations_UserID",
                table: "Locations",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleID",
                table: "UserRoles",
                column: "RoleID");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_UserID",
                table: "UserRoles",
                column: "UserID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Deliveries");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "DeliveryTypes");

            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropTable(
                name: "PaymentTypes");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
