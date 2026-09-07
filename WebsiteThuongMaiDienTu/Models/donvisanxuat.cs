namespace WebsiteThuongMaiDienTu.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("donvisanxuat")]
    public partial class donvisanxuat
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public donvisanxuat()
        {
            sanphams = new HashSet<sanpham>();
        }

        [Key]
        [StringLength(20)]
        public string madonvisanxuat { get; set; }

        [Required]
        [StringLength(100)]
        public string tendonvisanxuat { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<sanpham> sanphams { get; set; }
    }
}
