using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Dynamic;
using Dapper.FastCrud;

namespace MVCData.Models{
    [Table("products")]
    public class SanPham
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("product_id")]
        public int ProductID {get; set;}
        [Column("product_code")]
        public string ProductCode {get;set;}="";
        [Column("product_name")]
        public string ProductName {get; set;}="";
        [Column("brand")]
        public string Brand {get; set;} ="";
        [Column("price")]
        public double Price {get; set;}
        [Column("quantity")]
        public int Quantity{get; set;}
        [Column("image_url")]
        public string? ImageUrl {get; set;}="";
        [Column("description")]
        public string Description {get ; set;}="";
        [Column("status")]
        public string Status {get;set;}="";
        public  SanPham()
        { 
        }
        public SanPham( int productid,string product_code,string product_name, string brand,double price,
        int quantity,string image_url,string description , string status){
            ProductID= productid;
            ProductCode=product_code;
            ProductName=product_name;
            Brand=brand;
            Price=price;
            Quantity=quantity;
            ImageUrl=image_url;
            Description=description;
            Status=status;
        }
    }
}