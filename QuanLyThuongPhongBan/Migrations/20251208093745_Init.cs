using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyThuongPhongBan.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tb_phong_ban",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ten_phong_ban = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    mo_ta = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_phong_ban", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tb_thuong_du_an",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nam_thuong = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    gia_tri_hop_dong = table.Column<decimal>(type: "decimal(22,6)", nullable: false, defaultValue: 0m),
                    quyet_toan = table.Column<decimal>(type: "decimal(22,6)", nullable: false, defaultValue: 0m),
                    tong_gia_tri_dieu_chinh_dot_1 = table.Column<decimal>(type: "decimal(22,6)", nullable: false, defaultValue: 0m),
                    tong_ti_le_dieu_chinh_dot_1 = table.Column<decimal>(type: "decimal(22,6)", nullable: false, defaultValue: 0m),
                    tong_gia_tri_dieu_chinh_dot_2 = table.Column<decimal>(type: "decimal(22,6)", nullable: false, defaultValue: 0m),
                    tong_ti_le_dieu_chinh_dot_2 = table.Column<decimal>(type: "decimal(22,6)", nullable: false, defaultValue: 0m),
                    tong_nghiem_thu = table.Column<decimal>(type: "decimal(22,6)", nullable: false, defaultValue: 0m),
                    tong_thu_hoi_cong_no = table.Column<decimal>(type: "decimal(22,6)", nullable: false, defaultValue: 0m),
                    tong_gia_tri_thuong_du_an = table.Column<decimal>(type: "decimal(22,6)", nullable: false, defaultValue: 0m),
                    tong_ti_le_thuong_du_an = table.Column<decimal>(type: "decimal(22,6)", nullable: false, defaultValue: 0m),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_thuong_du_an", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tb_thuong_smb",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    quy_nam_thuong = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    doanh_thu_smb = table.Column<decimal>(type: "decimal(22,6)", nullable: false, defaultValue: 0m),
                    doanh_thu_xuat_hoa_don = table.Column<decimal>(type: "decimal(22,6)", nullable: false, defaultValue: 0m),
                    doanh_thu_thu_hoi_cong_no = table.Column<decimal>(type: "decimal(22,6)", nullable: false, defaultValue: 0m),
                    tong_ti_le_thuong_smb = table.Column<decimal>(type: "decimal(22,6)", nullable: false, defaultValue: 0m),
                    tong_gia_tri_thuong_smb = table.Column<decimal>(type: "decimal(22,6)", nullable: false, defaultValue: 0m),
                    tong_ti_le_dot_1 = table.Column<decimal>(type: "decimal(22,6)", nullable: false, defaultValue: 0m),
                    tong_gia_tri_dot_1 = table.Column<decimal>(type: "decimal(22,6)", nullable: false, defaultValue: 0m),
                    tong_thu_hoi_cong_no = table.Column<decimal>(type: "decimal(22,6)", nullable: false, defaultValue: 0m),
                    tong_nghiem_thu = table.Column<decimal>(type: "decimal(22,6)", nullable: false, defaultValue: 0m),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_thuong_smb", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tb_tai_khoan",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ten_dang_nhap = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    mat_khau = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ho_ten = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    id_phong_ban = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_tai_khoan", x => x.id);
                    table.UniqueConstraint("AK_tb_tai_khoan_ten_dang_nhap", x => x.ten_dang_nhap);
                    table.ForeignKey(
                        name: "FK_tb_tai_khoan_tb_phong_ban_id_phong_ban",
                        column: x => x.id_phong_ban,
                        principalTable: "tb_phong_ban",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tb_thuong_dai_doan_du_an",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_phong_ban = table.Column<int>(type: "int", nullable: false),
                    id_thuong_du_an = table.Column<int>(type: "int", nullable: false),
                    gia_tri_tong_goi = table.Column<decimal>(type: "decimal(22,6)", nullable: false, defaultValue: 0m),
                    ti_le_tong_goi = table.Column<decimal>(type: "decimal(22,6)", nullable: false, defaultValue: 0m),
                    gia_tri_dieu_chinh_dot_1 = table.Column<decimal>(type: "decimal(22,6)", nullable: false, defaultValue: 0m),
                    ti_le_dieu_chinh_dot_1 = table.Column<decimal>(type: "decimal(22,6)", nullable: false, defaultValue: 0m),
                    gia_tri_dieu_chinh_dot_2 = table.Column<decimal>(type: "decimal(22,6)", nullable: false, defaultValue: 0m),
                    ti_le_dieu_chinh_dot_2 = table.Column<decimal>(type: "decimal(22,6)", nullable: false, defaultValue: 0m),
                    nghiem_thu = table.Column<decimal>(type: "decimal(22,6)", nullable: false, defaultValue: 0m),
                    thu_hoi_cong_no = table.Column<decimal>(type: "decimal(22,6)", nullable: false, defaultValue: 0m),
                    doanh_thu_xuat_hoa_don = table.Column<decimal>(type: "decimal(22,6)", nullable: false, defaultValue: 0m),
                    doanh_thu_hop_dong = table.Column<decimal>(type: "decimal(22,6)", nullable: false, defaultValue: 0m),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_thuong_dai_doan_du_an", x => x.id);
                    table.ForeignKey(
                        name: "FK_tb_thuong_dai_doan_du_an_tb_phong_ban_id_phong_ban",
                        column: x => x.id_phong_ban,
                        principalTable: "tb_phong_ban",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tb_thuong_dai_doan_du_an_tb_thuong_du_an_id_thuong_du_an",
                        column: x => x.id_thuong_du_an,
                        principalTable: "tb_thuong_du_an",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tb_thuong_du_an_chi_tiet",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_thuong_du_an = table.Column<int>(type: "int", nullable: true),
                    du_an = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    chu_dau_tu = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    hop_dong_so = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ngay_thang = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ghi_chu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    doanh_thu_hop_dong = table.Column<decimal>(type: "decimal(22,6)", nullable: false, defaultValue: 0m),
                    doanh_thu_quyet_toan = table.Column<decimal>(type: "decimal(22,6)", nullable: false, defaultValue: 0m),
                    doanh_thu_da_xuat_hoa_don = table.Column<decimal>(type: "decimal(22,6)", nullable: false, defaultValue: 0m),
                    doanh_thu_chua_xuat_hoa_don = table.Column<decimal>(type: "decimal(22,6)", nullable: false, defaultValue: 0m),
                    da_thanh_toan = table.Column<decimal>(type: "decimal(22,6)", nullable: false, defaultValue: 0m),
                    chua_thanh_toan = table.Column<decimal>(type: "decimal(22,6)", nullable: false, defaultValue: 0m),
                    hsda = table.Column<decimal>(type: "decimal(22,6)", nullable: false, defaultValue: 0m),
                    ktk = table.Column<decimal>(type: "decimal(22,6)", nullable: false, defaultValue: 0m),
                    po = table.Column<decimal>(type: "decimal(22,6)", nullable: false, defaultValue: 0m),
                    ttdvkt = table.Column<decimal>(type: "decimal(22,6)", nullable: false, defaultValue: 0m),
                    tvgp = table.Column<decimal>(type: "decimal(22,6)", nullable: false, defaultValue: 0m),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_thuong_du_an_chi_tiet", x => x.id);
                    table.ForeignKey(
                        name: "FK_tb_thuong_du_an_chi_tiet_tb_thuong_du_an_id_thuong_du_an",
                        column: x => x.id_thuong_du_an,
                        principalTable: "tb_thuong_du_an",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tb_thuong_dai_doan_smb",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_phong_ban = table.Column<int>(type: "int", nullable: false),
                    id_thuong_smb = table.Column<int>(type: "int", nullable: false),
                    doanh_thu_smb = table.Column<decimal>(type: "decimal(22,6)", nullable: false, defaultValue: 0m),
                    doanh_thu_xuat_hoa_don = table.Column<decimal>(type: "decimal(22,6)", nullable: false, defaultValue: 0m),
                    doanh_thu_thu_hoi_cong_no = table.Column<decimal>(type: "decimal(22,6)", nullable: false, defaultValue: 0m),
                    ti_le_dot_1 = table.Column<decimal>(type: "decimal(22,6)", nullable: false, defaultValue: 0m),
                    gia_tri_dot_1 = table.Column<decimal>(type: "decimal(22,6)", nullable: false, defaultValue: 0m),
                    ti_le_tong_smb = table.Column<decimal>(type: "decimal(22,6)", nullable: false, defaultValue: 0m),
                    gia_tri_tong_smb = table.Column<decimal>(type: "decimal(22,6)", nullable: false, defaultValue: 0m),
                    ti_le_thu_hoi_cong_no = table.Column<decimal>(type: "decimal(22,6)", nullable: false, defaultValue: 0m),
                    thu_hoi_cong_no = table.Column<decimal>(type: "decimal(22,6)", nullable: false, defaultValue: 0m),
                    nghiem_thu = table.Column<decimal>(type: "decimal(22,6)", nullable: false, defaultValue: 0m),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_thuong_dai_doan_smb", x => x.id);
                    table.ForeignKey(
                        name: "FK_tb_thuong_dai_doan_smb_tb_phong_ban_id_phong_ban",
                        column: x => x.id_phong_ban,
                        principalTable: "tb_phong_ban",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tb_thuong_dai_doan_smb_tb_thuong_smb_id_thuong_smb",
                        column: x => x.id_thuong_smb,
                        principalTable: "tb_thuong_smb",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tb_nhat_ky",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_tai_khoan = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    hanh_dong = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    mo_ta = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_nhat_ky", x => x.id);
                    table.ForeignKey(
                        name: "FK_tb_nhat_ky_tb_tai_khoan_id_tai_khoan",
                        column: x => x.id_tai_khoan,
                        principalTable: "tb_tai_khoan",
                        principalColumn: "ten_dang_nhap",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tb_nhat_ky_id_tai_khoan",
                table: "tb_nhat_ky",
                column: "id_tai_khoan");

            migrationBuilder.CreateIndex(
                name: "IX_tb_phong_ban_ten_phong_ban",
                table: "tb_phong_ban",
                column: "ten_phong_ban",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tb_tai_khoan_id_phong_ban",
                table: "tb_tai_khoan",
                column: "id_phong_ban");

            migrationBuilder.CreateIndex(
                name: "IX_tb_tai_khoan_ten_dang_nhap",
                table: "tb_tai_khoan",
                column: "ten_dang_nhap",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tb_thuong_dai_doan_du_an_id_phong_ban",
                table: "tb_thuong_dai_doan_du_an",
                column: "id_phong_ban");

            migrationBuilder.CreateIndex(
                name: "IX_tb_thuong_dai_doan_du_an_id_thuong_du_an",
                table: "tb_thuong_dai_doan_du_an",
                column: "id_thuong_du_an");

            migrationBuilder.CreateIndex(
                name: "IX_tb_thuong_dai_doan_smb_id_phong_ban",
                table: "tb_thuong_dai_doan_smb",
                column: "id_phong_ban");

            migrationBuilder.CreateIndex(
                name: "IX_tb_thuong_dai_doan_smb_id_thuong_smb",
                table: "tb_thuong_dai_doan_smb",
                column: "id_thuong_smb");

            migrationBuilder.CreateIndex(
                name: "IX_tb_thuong_du_an_chi_tiet_id_thuong_du_an",
                table: "tb_thuong_du_an_chi_tiet",
                column: "id_thuong_du_an");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tb_nhat_ky");

            migrationBuilder.DropTable(
                name: "tb_thuong_dai_doan_du_an");

            migrationBuilder.DropTable(
                name: "tb_thuong_dai_doan_smb");

            migrationBuilder.DropTable(
                name: "tb_thuong_du_an_chi_tiet");

            migrationBuilder.DropTable(
                name: "tb_tai_khoan");

            migrationBuilder.DropTable(
                name: "tb_thuong_smb");

            migrationBuilder.DropTable(
                name: "tb_thuong_du_an");

            migrationBuilder.DropTable(
                name: "tb_phong_ban");
        }
    }
}
