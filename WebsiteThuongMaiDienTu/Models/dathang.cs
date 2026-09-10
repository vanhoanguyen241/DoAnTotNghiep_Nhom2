namespace WebsiteThuongMaiDienTu.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("dathang")]
    public partial class dathang
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public dathang()
        {
            chitietdathangs = new HashSet<chitietdathang>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int madathang { get; set; }

        public int makh { get; set; }

        [Column(TypeName = "date")]
        public DateTime ngaydathang { get; set; }

        [Column(TypeName = "date")]
        public DateTime ngaygiaohang { get; set; }

        public bool giaotannoi { get; set; }

        public byte trangthai { get; set; }

        [StringLength(1000)]
        public string ghichu { get; set; }

        public int? mahoadon { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<chitietdathang> chitietdathangs { get; set; }

        public virtual hoadon hoadon { get; set; }

        public virtual khachhang khachhang { get; set; }
    }
}
