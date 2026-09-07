namespace WebsiteThuongMaiDienTu.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("hoadon")]
    public partial class hoadon
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public hoadon()
        {
            chitiethoadons = new HashSet<chitiethoadon>();
            chuyenhangs = new HashSet<chuyenhang>();
        }

        [Key]
        public int mahoadon { get; set; }

        public int makh { get; set; }

        public int? nguoilap { get; set; }

        [Column(TypeName = "date")]
        public DateTime ngaydathang { get; set; }

        [Column(TypeName = "date")]
        public DateTime ngaygiaohang { get; set; }

        public decimal tongtien { get; set; }

        public bool dathanhtoan { get; set; }

        public bool giaotannoi { get; set; }

        public byte trangthaidon { get; set; }

        [StringLength(1000)]
        public string ghichu { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<chitiethoadon> chitiethoadons { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<chuyenhang> chuyenhangs { get; set; }

        public virtual khachhang khachhang { get; set; }

        public virtual nhanvien nhanvien { get; set; }
    }
}
