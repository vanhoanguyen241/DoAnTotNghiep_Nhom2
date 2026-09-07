namespace WebsiteThuongMaiDienTu.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("loaisanpham")]
    public partial class loaisanpham
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public loaisanpham()
        {
            sanphams = new HashSet<sanpham>();
        }

        [Key]
        [StringLength(20)]
        public string maloaisanpham { get; set; }

        [Required]
        [StringLength(100)]
        public string tenloaisanpham { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<sanpham> sanphams { get; set; }
    }
}
