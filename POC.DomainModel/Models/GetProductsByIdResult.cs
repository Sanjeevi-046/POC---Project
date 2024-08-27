﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace POC.DomainModel.Models
{
    public partial class GetProductsByIdResult
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        [Column("Price", TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        public string Description { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsQuantityAvailable { get; set; }
        public int productAvailable { get; set; }
        public byte[] ProductImage { get; set; }
    }
}
