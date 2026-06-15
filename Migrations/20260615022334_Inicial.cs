using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaManejoBar.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "categoriaCoctel",
                columns: table => new
                {
                    idCategoria = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombreCategoria = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    descripcion = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__categori__8A3D240C8DCE5215", x => x.idCategoria);
                });

            migrationBuilder.CreateTable(
                name: "cristaleria",
                columns: table => new
                {
                    idCristaleria = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombreCristaleria = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    capacidadOz = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__cristale__80DCDE0C2E3D9921", x => x.idCristaleria);
                });

            migrationBuilder.CreateTable(
                name: "tipoIngrediente",
                columns: table => new
                {
                    idTipoIngrediente = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombreTipoIngrediente = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tipoIngr__17766B0815E208DF", x => x.idTipoIngrediente);
                });

            migrationBuilder.CreateTable(
                name: "coctel",
                columns: table => new
                {
                    idCoctel = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombreCoctel = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    instrucciones = table.Column<string>(type: "varchar(5000)", unicode: false, maxLength: 5000, nullable: false),
                    precioVenta = table.Column<int>(type: "int", nullable: false),
                    foto = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    idCategoria = table.Column<int>(type: "int", nullable: false),
                    idCristaleria = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__coctel__490EE0B8AC7D8D3D", x => x.idCoctel);
                    table.ForeignKey(
                        name: "FK__coctel__idCatego__5812160E",
                        column: x => x.idCategoria,
                        principalTable: "categoriaCoctel",
                        principalColumn: "idCategoria");
                    table.ForeignKey(
                        name: "FK__coctel__idCrista__59063A47",
                        column: x => x.idCristaleria,
                        principalTable: "cristaleria",
                        principalColumn: "idCristaleria");
                });

            migrationBuilder.CreateTable(
                name: "ingrediente",
                columns: table => new
                {
                    idIngrediente = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombreIngrediente = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    stock = table.Column<double>(type: "float", nullable: false),
                    unidad = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    idTipoIngrediente = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ingredie__563C0D33A647086D", x => x.idIngrediente);
                    table.ForeignKey(
                        name: "FK__ingredien__idTip__5441852A",
                        column: x => x.idTipoIngrediente,
                        principalTable: "tipoIngrediente",
                        principalColumn: "idTipoIngrediente");
                });

            migrationBuilder.CreateTable(
                name: "detalleReceta",
                columns: table => new
                {
                    idDetalleReceta = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    idCoctel = table.Column<int>(type: "int", nullable: false),
                    idIngrediente = table.Column<int>(type: "int", nullable: false),
                    cantidad = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__detalleR__7A7D8379DF2B628A", x => x.idDetalleReceta);
                    table.ForeignKey(
                        name: "FK__detalleRe__idCoc__5BE2A6F2",
                        column: x => x.idCoctel,
                        principalTable: "coctel",
                        principalColumn: "idCoctel");
                    table.ForeignKey(
                        name: "FK__detalleRe__idIng__5CD6CB2B",
                        column: x => x.idIngrediente,
                        principalTable: "ingrediente",
                        principalColumn: "idIngrediente");
                });

            migrationBuilder.CreateIndex(
                name: "UQ__categori__788BF0FA1773A1F2",
                table: "categoriaCoctel",
                column: "nombreCategoria",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_coctel_idCategoria",
                table: "coctel",
                column: "idCategoria");

            migrationBuilder.CreateIndex(
                name: "IX_coctel_idCristaleria",
                table: "coctel",
                column: "idCristaleria");

            migrationBuilder.CreateIndex(
                name: "UQ__coctel__119308C3376FCC7C",
                table: "coctel",
                column: "nombreCoctel",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__cristale__4CC6C0C8C376C714",
                table: "cristaleria",
                column: "nombreCristaleria",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_detalleReceta_idCoctel",
                table: "detalleReceta",
                column: "idCoctel");

            migrationBuilder.CreateIndex(
                name: "IX_detalleReceta_idIngrediente",
                table: "detalleReceta",
                column: "idIngrediente");

            migrationBuilder.CreateIndex(
                name: "IX_ingrediente_idTipoIngrediente",
                table: "ingrediente",
                column: "idTipoIngrediente");

            migrationBuilder.CreateIndex(
                name: "UQ__ingredie__92BF05284D116A0F",
                table: "ingrediente",
                column: "nombreIngrediente",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__tipoIngr__6D5D16D4B73CCDA2",
                table: "tipoIngrediente",
                column: "nombreTipoIngrediente",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "detalleReceta");

            migrationBuilder.DropTable(
                name: "coctel");

            migrationBuilder.DropTable(
                name: "ingrediente");

            migrationBuilder.DropTable(
                name: "categoriaCoctel");

            migrationBuilder.DropTable(
                name: "cristaleria");

            migrationBuilder.DropTable(
                name: "tipoIngrediente");
        }
    }
}
