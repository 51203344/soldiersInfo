using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SoldiersInfo.Models
{
    public class Soldier
    {
        public int ID { get; set; }

        [Display(Name = "Họ")]
        [Required(ErrorMessage = "Bạn phải nhập họ của chiến sĩ!")]
        public String lastName { get; set; }

        [Display(Name = "Tên lót")]
        public String middleName { get; set; }

        [Required(ErrorMessage = "Bạn phải nhập tên của chiến sĩ!")]
        [Display(Name = "Tên")]
        public String firstName { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Ngày sinh không đúng!")]
        [Required(ErrorMessage = "Bạn phải nhập ngày sinh của chiến sĩ!")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Ngày sinh")]
        public DateTime birthday { get; set; }

        [Required(ErrorMessage = "Bạn phải nhập đơn vị công tác của chiến sĩ!")]
        [Display(Name = "Đơn vị công tác")]
        public String company { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Ngày vào CAND không đúng!")]
        [Required(ErrorMessage = "Bạn phải nhập ngày vào CAND của chiến sĩ!")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Ngày vào CAND")]
        public DateTime servingDate { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Ngày hưởng khỏi điểm không đúng! ")]
        [Required(ErrorMessage = "Bạn phải nhập ngày hưởng khởi điểm của chiến sĩ!")]
        [Display(Name = "Thời điểm hưởng khởi điểm")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime pointDate { get; set; }

        [Display(Name = "Ghi chú")]
        public String note { get; set; }

        public enum Annoucement
        {
            C, R
        }

        [Display(Name = "Thông báo")]
        public Annoucement annouce { get; set; }

        [ScaffoldColumn(false)]
        public bool isDisplay { get; set; }
    }
}