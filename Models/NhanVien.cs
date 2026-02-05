using System;
using System.ComponentModel.DataAnnotations;

namespace MVCData.Models
{
    public class NhanVien
    {
        public int ID { get; set; } 
        [Required(ErrorMessage ="Họ tên không được để trống")]
        public string HoTen { get; set; } = "";
        [Required(ErrorMessage ="Ngày sinh không được để trống")]
        [DataType(DataType.Date)]
        public DateTime NgaySinh { get; set; }
        public string SoDienThoai { get; set; } = "";
        public string DiaChi { get; set; } = "";
        public string ChucVu { get; set; } = "";
        public int SoNamCongTac { get; set; }
        public int? PhongBanId { get; set; }
        public string TenPhongBan { get; set; }="";

        public NhanVien() { }

        public NhanVien(int id, string hoTen, DateTime ngaySinh,
                        string soDienThoai, string diaChi,
                        string chucVu, int soNamCongTac,int phongBanId, string tenPhongBan)
        {
            ID = id;
            HoTen = hoTen;
            NgaySinh = ngaySinh;
            SoDienThoai = soDienThoai;
            DiaChi = diaChi;
            ChucVu = chucVu;
            SoNamCongTac = soNamCongTac;
            PhongBanId = phongBanId;
            TenPhongBan = tenPhongBan;
        }
    }
}
