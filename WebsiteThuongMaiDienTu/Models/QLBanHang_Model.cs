using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace WebsiteThuongMaiDienTu.Models
{
    public partial class QLBanHang_Model : DbContext
    {
        public QLBanHang_Model()
            : base("name=QLBanHang_Model")
        {
        }

        public virtual DbSet<chitiethoadon> chitiethoadons { get; set; }
        public virtual DbSet<chuyenhang> chuyenhangs { get; set; }
        public virtual DbSet<donvisanxuat> donvisanxuats { get; set; }
        public virtual DbSet<hoadon> hoadons { get; set; }
        public virtual DbSet<khachhang> khachhangs { get; set; }
        public virtual DbSet<loaisanpham> loaisanphams { get; set; }
        public virtual DbSet<nhanvien> nhanviens { get; set; }
        public virtual DbSet<sanpham> sanphams { get; set; }
        public virtual DbSet<taikhoan> taikhoans { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<chitiethoadon>()
                .Property(e => e.masanpham)
                .IsUnicode(false);

            modelBuilder.Entity<donvisanxuat>()
                .Property(e => e.madonvisanxuat)
                .IsUnicode(false);

            modelBuilder.Entity<donvisanxuat>()
                .HasMany(e => e.sanphams)
                .WithRequired(e => e.donvisanxuat)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<hoadon>()
                .HasMany(e => e.chuyenhangs)
                .WithRequired(e => e.hoadon)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<khachhang>()
                .Property(e => e.SDT)
                .IsUnicode(false);

            modelBuilder.Entity<khachhang>()
                .HasMany(e => e.hoadons)
                .WithRequired(e => e.khachhang)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<loaisanpham>()
                .Property(e => e.maloaisanpham)
                .IsUnicode(false);

            modelBuilder.Entity<loaisanpham>()
                .HasMany(e => e.sanphams)
                .WithRequired(e => e.loaisanpham)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<nhanvien>()
                .Property(e => e.dienthoai)
                .IsUnicode(false);

            modelBuilder.Entity<nhanvien>()
                .HasMany(e => e.hoadons)
                .WithOptional(e => e.nhanvien)
                .HasForeignKey(e => e.nguoilap);

            modelBuilder.Entity<sanpham>()
                .Property(e => e.masanpham)
                .IsUnicode(false);

            modelBuilder.Entity<sanpham>()
                .Property(e => e.maloaisanpham)
                .IsUnicode(false);

            modelBuilder.Entity<sanpham>()
                .Property(e => e.madonvisanxuat)
                .IsUnicode(false);

            modelBuilder.Entity<sanpham>()
                .HasMany(e => e.chitiethoadons)
                .WithRequired(e => e.sanpham)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<taikhoan>()
                .Property(e => e.tendangnhap)
                .IsUnicode(false);

            modelBuilder.Entity<taikhoan>()
                .Property(e => e.matkhau)
                .IsUnicode(false);
        }
    }
}
