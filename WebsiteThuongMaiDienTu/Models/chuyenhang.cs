namespace WebsiteThuongMaiDienTu.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("chuyenhang")]
    public partial class chuyenhang
    {
        [Key]
        public int machuyenhang { get; set; }

        public int mahoadon { get; set; }

        public int? manv { get; set; }

        [Column(TypeName = "date")]
        public DateTime? ngaygiao { get; set; }

        public bool daxuli { get; set; }

        public virtual hoadon hoadon { get; set; }

        public virtual nhanvien nhanvien { get; set; }
    }
}
