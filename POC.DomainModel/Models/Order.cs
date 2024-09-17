﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace POC.DataLayer.Models;

[Index("ProductId", Name = "IX_Orders_ProductId")]
public partial class Order
{
    [Key]
    public int OrderId { get; set; }

    public DateTime OrderDate { get; set; }

    public int UserId { get; set; }

    public int ProductId { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal OrderPrice { get; set; }

    [Column("isDelivered")]
    public bool IsDelivered { get; set; }

    [Column("isCancelled")]
    public bool IsCancelled { get; set; }

    public int ProductQuantity { get; set; }

    [Column("AddressID")]
    public int? AddressId { get; set; }

    [Unicode(false)]
    public string ProductList { get; set; }

    [Unicode(false)]
    public string ProductQuantityList { get; set; }
}