namespace WebsiteThuongMaiDienTu.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("chitietdathang")]
    public partial class chitietdathang
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int madathang { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(20)]
        public string masanpham { get; set; }

        public int soluongdat { get; set; }

        public int? soluongduyet { get; set; }

        public decimal dongia { get; set; }

        public decimal thanhtien { get; set; }

        public virtual dathang dathang { get; set; }

        public virtual sanpham sanpham { get; set; }
    }
}
