using System;
using System.ComponentModel.DataAnnotations;
namespace MVCData.Models
{
    public class PhongBan {
        public int ID {get;set;}
        public string TenPhongBan {get;set;}="";
        public PhongBan(){}
        public PhongBan(int id, string tenPhongBan)
    {
        ID=id;
        TenPhongBan=tenPhongBan;
    }
    }
    
}
