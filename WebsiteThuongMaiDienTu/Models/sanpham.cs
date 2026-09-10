namespace WebsiteThuongMaiDienTu.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("sanpham")]
    public partial class sanpham
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public sanpham()
        {
            chitietdathangs = new HashSet<chitietdathang>();
            chitiethoadons = new HashSet<chitiethoadon>();
        }

        [Key]
        [StringLength(20)]
        public string masanpham { get; set; }

        [Required]
        [StringLength(20)]
        public string maloaisanpham { get; set; }

        [Required]
        [StringLength(20)]
        public string madonvisanxuat { get; set; }

        [Required]
        [StringLength(150)]
        public string tensanpham { get; set; }

        [Required]
        [StringLength(255)]
        public string hinh { get; set; }

        [Required]
        [StringLength(1000)]
        public string mota { get; set; }

        public int thoigianbaohanh { get; set; }

        public decimal dongia { get; set; }

        public int soluonghienco { get; set; }

        public bool? quangcao { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<chitietdathang> chitietdathangs { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<chitiethoadon> chitiethoadons { get; set; }

        public virtual donvisanxuat donvisanxuat { get; set; }

        public virtual loaisanpham loaisanpham { get; set; }
    }
}
