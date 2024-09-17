﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace POC.DataLayer.Models;

public partial class Payment
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(16)]
    [Unicode(false)]
    public string CardNumber { get; set; }

    public byte ExpiryMonth { get; set; }

    public short ExpiryYear { get; set; }

    [Column("CVV")]
    public short Cvv { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Amount { get; set; }

    public int? UserId { get; set; }

    public int? OrderId { get; set; }

    [Column("CartOrderID")]
    public int? CartOrderId { get; set; }

    public bool PaymentReceived { get; set; }
}