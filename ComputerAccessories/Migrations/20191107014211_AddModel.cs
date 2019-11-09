using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ComputerAccessories.Migrations
{
    public partial class AddModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tbl_Brand",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BrandName = table.Column<string>(maxLength: 50, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Logo = table.Column<string>(maxLength: 100, nullable: true),
                    Status = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_Brand", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_Category",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(maxLength: 50, nullable: true),
                    ParentCateId = table.Column<int>(nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Status = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_Category", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_Province",
                columns: table => new
                {
                    ProvinceId = table.Column<int>(nullable: false),
                    ProvinceName = table.Column<string>(maxLength: 100, nullable: true),
                    ProvinceType = table.Column<string>(maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_Province", x => x.ProvinceId);
                });

            migrationBuilder.CreateTable(
                name: "tbl_Roles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_Users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DisplayName = table.Column<string>(maxLength: 100, nullable: true),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    Password = table.Column<string>(nullable: true),
                    IsActivated = table.Column<bool>(nullable: true),
                    PasswordSalt = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CodeConfirm = table.Column<string>(maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_Attribute",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AttributeName = table.Column<string>(maxLength: 50, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CategoryId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_Attribute", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tbl_Attribute_tbl_Category",
                        column: x => x.CategoryId,
                        principalTable: "tbl_Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tbl_Product",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductName = table.Column<string>(maxLength: 100, nullable: true),
                    Code = table.Column<string>(maxLength: 20, nullable: true),
                    ShortDescription = table.Column<string>(maxLength: 200, nullable: true),
                    FullDescription = table.Column<string>(maxLength: 500, nullable: true),
                    Quantity = table.Column<int>(nullable: true, defaultValueSql: "((0))"),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Origin = table.Column<string>(maxLength: 50, nullable: true),
                    Color = table.Column<string>(maxLength: 50, nullable: true),
                    Price = table.Column<decimal>(type: "money", nullable: true, defaultValueSql: "((0))"),
                    CategoryId = table.Column<int>(nullable: false),
                    BrandId = table.Column<int>(nullable: false),
                    PromotionPrice = table.Column<decimal>(type: "money", nullable: true),
                    Thumnail = table.Column<string>(maxLength: 100, nullable: true),
                    IsAvailable = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_Product", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tbl_Product_tbl_Brand",
                        column: x => x.BrandId,
                        principalTable: "tbl_Brand",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbl_Product_tbl_Category",
                        column: x => x.CategoryId,
                        principalTable: "tbl_Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tbl_District",
                columns: table => new
                {
                    DistrictId = table.Column<int>(nullable: false),
                    DistrictName = table.Column<string>(maxLength: 100, nullable: true),
                    DistrictType = table.Column<string>(maxLength: 20, nullable: true),
                    ProvinceId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_District", x => x.DistrictId);
                    table.ForeignKey(
                        name: "FK_tbl_District_tbl_Province",
                        column: x => x.ProvinceId,
                        principalTable: "tbl_Province",
                        principalColumn: "ProvinceId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tbl_User_Role",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    RoleId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_User_Role", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "tbl_Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "tbl_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tbl_Product_Attributes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(nullable: true),
                    AttributeId = table.Column<int>(nullable: true),
                    Value = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_Product_Attributes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tbl_Product_Attributes_tbl_Attribute",
                        column: x => x.AttributeId,
                        principalTable: "tbl_Attribute",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbl_Product_Attributes_tbl_Product",
                        column: x => x.ProductId,
                        principalTable: "tbl_Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tbl_Product_Images",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(nullable: true),
                    ImageUrl = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_Product_Images", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tbl_Product_Images_tbl_Product",
                        column: x => x.ProductId,
                        principalTable: "tbl_Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tbl_Ward",
                columns: table => new
                {
                    WardId = table.Column<int>(nullable: false),
                    WardName = table.Column<string>(maxLength: 100, nullable: true),
                    WardType = table.Column<string>(maxLength: 20, nullable: true),
                    DistrictId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_Ward", x => x.WardId);
                    table.ForeignKey(
                        name: "FK_tbl_Ward_tbl_District",
                        column: x => x.DistrictId,
                        principalTable: "tbl_District",
                        principalColumn: "DistrictId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tbl_User_Address",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    ProvinceId = table.Column<int>(nullable: false),
                    DistrictId = table.Column<int>(nullable: false),
                    WardId = table.Column<int>(nullable: false),
                    PlaceDetail = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_User_Address", x => new { x.UserId, x.ProvinceId, x.DistrictId, x.WardId });
                    table.ForeignKey(
                        name: "FK_tbl_User_Address_tbl_District",
                        column: x => x.DistrictId,
                        principalTable: "tbl_District",
                        principalColumn: "DistrictId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbl_User_Address_tbl_Province",
                        column: x => x.ProvinceId,
                        principalTable: "tbl_Province",
                        principalColumn: "ProvinceId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbl_User_Address_tbl_Users",
                        column: x => x.UserId,
                        principalTable: "tbl_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbl_User_Address_tbl_Ward",
                        column: x => x.WardId,
                        principalTable: "tbl_Ward",
                        principalColumn: "WardId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tbl_Attribute_CategoryId",
                table: "tbl_Attribute",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_District_ProvinceId",
                table: "tbl_District",
                column: "ProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_Product_BrandId",
                table: "tbl_Product",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_Product_CategoryId",
                table: "tbl_Product",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_Product_Attributes_AttributeId",
                table: "tbl_Product_Attributes",
                column: "AttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_Product_Attributes_ProductId",
                table: "tbl_Product_Attributes",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_Product_Images_ProductId",
                table: "tbl_Product_Images",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_User_Address_DistrictId",
                table: "tbl_User_Address",
                column: "DistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_User_Address_ProvinceId",
                table: "tbl_User_Address",
                column: "ProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_User_Address_WardId",
                table: "tbl_User_Address",
                column: "WardId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_User_Role_RoleId",
                table: "tbl_User_Role",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_Ward_DistrictId",
                table: "tbl_Ward",
                column: "DistrictId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tbl_Product_Attributes");

            migrationBuilder.DropTable(
                name: "tbl_Product_Images");

            migrationBuilder.DropTable(
                name: "tbl_User_Address");

            migrationBuilder.DropTable(
                name: "tbl_User_Role");

            migrationBuilder.DropTable(
                name: "tbl_Attribute");

            migrationBuilder.DropTable(
                name: "tbl_Product");

            migrationBuilder.DropTable(
                name: "tbl_Ward");

            migrationBuilder.DropTable(
                name: "tbl_Roles");

            migrationBuilder.DropTable(
                name: "tbl_Users");

            migrationBuilder.DropTable(
                name: "tbl_Brand");

            migrationBuilder.DropTable(
                name: "tbl_Category");

            migrationBuilder.DropTable(
                name: "tbl_District");

            migrationBuilder.DropTable(
                name: "tbl_Province");
        }
    }
}
