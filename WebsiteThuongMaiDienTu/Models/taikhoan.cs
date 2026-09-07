namespace WebsiteThuongMaiDienTu.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("taikhoan")]
    public partial class taikhoan
    {
        [Key]
        [StringLength(50)]
        public string tendangnhap { get; set; }

        [Required]
        [StringLength(200)]
        public string matkhau { get; set; }

        public int? manv { get; set; }

        public int? makh { get; set; }

        public virtual khachhang khachhang { get; set; }

        public virtual nhanvien nhanvien { get; set; }
    }
}
