using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Web_bán_hàng__đồ_án_.Models
{
    public class Watch
    {
        [Key]
        public int Mã { get; set; }

        [Required]
        [StringLength(100)]
        public string Tên { get; set; }

        [Required]
        [StringLength(500)]
        public string Thông_tin { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal Giá { get; set; }

        [Required]
        [StringLength(200)]
        public string Ảnh { get; set; }


        [Required]
        [StringLength(100)]
        public string TenKH { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Ngay { get; set; }


        public int sl { get; set; }
    }
}