using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web_bán_hàng__đồ_án_.Models.ViewModel
{
    public class LoginMV
    {
        [Required(ErrorMessage = "Tên người dùng không được để trống.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }    
    }
}