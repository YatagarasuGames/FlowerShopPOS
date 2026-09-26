using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlowerShop.Migrations
{
    /// <inheritdoc />
    public partial class NewAdministrationStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "flowers_shop");

            migrationBuilder.CreateTable(
                name: "audit_logs",
                schema: "flowers_shop",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    Username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Action = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    EntityName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    EntityId = table.Column<string>(type: "text", nullable: true),
                    Details = table.Column<string>(type: "text", nullable: false),
                    IpAddress = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_audit_logs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "permissions",
                schema: "flowers_shop",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "products",
                schema: "flowers_shop",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CurrentPrice = table.Column<float>(type: "real", nullable: false),
                    Stock = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    ImageUrl = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_products", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                schema: "flowers_shop",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "role_permissions",
                schema: "flowers_shop",
                columns: table => new
                {
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    PermissionId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_role_permissions", x => new { x.RoleId, x.PermissionId });
                    table.ForeignKey(
                        name: "FK_role_permissions_permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalSchema: "flowers_shop",
                        principalTable: "permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_role_permissions_roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "flowers_shop",
                        principalTable: "roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "users",
                schema: "flowers_shop",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_users_roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "flowers_shop",
                        principalTable: "roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "orders",
                schema: "flowers_shop",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CashierId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    TotalAmount = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_orders_users_CashierId",
                        column: x => x.CashierId,
                        principalSchema: "flowers_shop",
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "price_history",
                schema: "flowers_shop",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    OldPrice = table.Column<float>(type: "real", nullable: false),
                    NewPrice = table.Column<float>(type: "real", nullable: false),
                    ChangedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    ChangedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_price_history", x => x.Id);
                    table.ForeignKey(
                        name: "FK_price_history_products_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "flowers_shop",
                        principalTable: "products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_price_history_users_ChangedBy",
                        column: x => x.ChangedBy,
                        principalSchema: "flowers_shop",
                        principalTable: "users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "supplies",
                schema: "flowers_shop",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReceivedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    PurchasePrice = table.Column<float>(type: "real", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_supplies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_supplies_products_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "flowers_shop",
                        principalTable: "products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_supplies_users_ReceivedByUserId",
                        column: x => x.ReceivedByUserId,
                        principalSchema: "flowers_shop",
                        principalTable: "users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "compositions",
                schema: "flowers_shop",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    AssemblyPrice = table.Column<float>(type: "real", nullable: false, defaultValue: 0f),
                    TotalPrice = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_compositions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_compositions_orders_OrderId",
                        column: x => x.OrderId,
                        principalSchema: "flowers_shop",
                        principalTable: "orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "product_batches",
                schema: "flowers_shop",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    SupplyId = table.Column<Guid>(type: "uuid", nullable: true),
                    PurchasePrice = table.Column<float>(type: "real", nullable: false),
                    SellingPrice = table.Column<float>(type: "real", nullable: false),
                    InitialQuantity = table.Column<int>(type: "integer", nullable: false),
                    RemainingQuantity = table.Column<int>(type: "integer", nullable: false),
                    ReceivedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_batches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_product_batches_products_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "flowers_shop",
                        principalTable: "products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_product_batches_supplies_SupplyId",
                        column: x => x.SupplyId,
                        principalSchema: "flowers_shop",
                        principalTable: "supplies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "order_items",
                schema: "flowers_shop",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    CompositionId = table.Column<Guid>(type: "uuid", nullable: true),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductBatchId = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    PurchasePrice = table.Column<float>(type: "real", nullable: false),
                    BaseUnitPrice = table.Column<float>(type: "real", nullable: false),
                    DiscountType = table.Column<int>(type: "integer", nullable: false),
                    DiscountValue = table.Column<float>(type: "real", nullable: false),
                    FinalUnitPrice = table.Column<float>(type: "real", nullable: false),
                    TotalPrice = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_order_items_compositions_CompositionId",
                        column: x => x.CompositionId,
                        principalSchema: "flowers_shop",
                        principalTable: "compositions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_order_items_orders_OrderId",
                        column: x => x.OrderId,
                        principalSchema: "flowers_shop",
                        principalTable: "orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_order_items_product_batches_ProductBatchId",
                        column: x => x.ProductBatchId,
                        principalSchema: "flowers_shop",
                        principalTable: "product_batches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_order_items_products_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "flowers_shop",
                        principalTable: "products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "write_offs",
                schema: "flowers_shop",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductBatchId = table.Column<Guid>(type: "uuid", nullable: false),
                    CashierId = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    PurchasePriceAtWriteOff = table.Column<float>(type: "real", nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_write_offs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_write_offs_product_batches_ProductBatchId",
                        column: x => x.ProductBatchId,
                        principalSchema: "flowers_shop",
                        principalTable: "product_batches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_write_offs_products_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "flowers_shop",
                        principalTable: "products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_write_offs_users_CashierId",
                        column: x => x.CashierId,
                        principalSchema: "flowers_shop",
                        principalTable: "users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_audit_logs_CreatedAt",
                schema: "flowers_shop",
                table: "audit_logs",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_audit_logs_UserId",
                schema: "flowers_shop",
                table: "audit_logs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_compositions_OrderId",
                schema: "flowers_shop",
                table: "compositions",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_order_items_CompositionId",
                schema: "flowers_shop",
                table: "order_items",
                column: "CompositionId");

            migrationBuilder.CreateIndex(
                name: "IX_order_items_OrderId",
                schema: "flowers_shop",
                table: "order_items",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_order_items_ProductBatchId",
                schema: "flowers_shop",
                table: "order_items",
                column: "ProductBatchId");

            migrationBuilder.CreateIndex(
                name: "IX_order_items_ProductId",
                schema: "flowers_shop",
                table: "order_items",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_orders_CashierId",
                schema: "flowers_shop",
                table: "orders",
                column: "CashierId");

            migrationBuilder.CreateIndex(
                name: "IX_permissions_Name",
                schema: "flowers_shop",
                table: "permissions",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_price_history_ChangedBy",
                schema: "flowers_shop",
                table: "price_history",
                column: "ChangedBy");

            migrationBuilder.CreateIndex(
                name: "IX_price_history_ProductId",
                schema: "flowers_shop",
                table: "price_history",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_product_batches_ProductId_RemainingQuantity_ReceivedAt",
                schema: "flowers_shop",
                table: "product_batches",
                columns: new[] { "ProductId", "RemainingQuantity", "ReceivedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_product_batches_SupplyId",
                schema: "flowers_shop",
                table: "product_batches",
                column: "SupplyId");

            migrationBuilder.CreateIndex(
                name: "IX_role_permissions_PermissionId",
                schema: "flowers_shop",
                table: "role_permissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_roles_Name",
                schema: "flowers_shop",
                table: "roles",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_supplies_ProductId",
                schema: "flowers_shop",
                table: "supplies",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_supplies_ReceivedByUserId",
                schema: "flowers_shop",
                table: "supplies",
                column: "ReceivedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_users_RoleId",
                schema: "flowers_shop",
                table: "users",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_users_Username",
                schema: "flowers_shop",
                table: "users",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_write_offs_CashierId",
                schema: "flowers_shop",
                table: "write_offs",
                column: "CashierId");

            migrationBuilder.CreateIndex(
                name: "IX_write_offs_ProductBatchId",
                schema: "flowers_shop",
                table: "write_offs",
                column: "ProductBatchId");

            migrationBuilder.CreateIndex(
                name: "IX_write_offs_ProductId",
                schema: "flowers_shop",
                table: "write_offs",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "audit_logs",
                schema: "flowers_shop");

            migrationBuilder.DropTable(
                name: "order_items",
                schema: "flowers_shop");

            migrationBuilder.DropTable(
                name: "price_history",
                schema: "flowers_shop");

            migrationBuilder.DropTable(
                name: "role_permissions",
                schema: "flowers_shop");

            migrationBuilder.DropTable(
                name: "write_offs",
                schema: "flowers_shop");

            migrationBuilder.DropTable(
                name: "compositions",
                schema: "flowers_shop");

            migrationBuilder.DropTable(
                name: "permissions",
                schema: "flowers_shop");

            migrationBuilder.DropTable(
                name: "product_batches",
                schema: "flowers_shop");

            migrationBuilder.DropTable(
                name: "orders",
                schema: "flowers_shop");

            migrationBuilder.DropTable(
                name: "supplies",
                schema: "flowers_shop");

            migrationBuilder.DropTable(
                name: "products",
                schema: "flowers_shop");

            migrationBuilder.DropTable(
                name: "users",
                schema: "flowers_shop");

            migrationBuilder.DropTable(
                name: "roles",
                schema: "flowers_shop");
        }
    }
}
