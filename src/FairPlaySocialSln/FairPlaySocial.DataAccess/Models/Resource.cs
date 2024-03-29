﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FairPlaySocial.DataAccess.Models;

[Index("Type", "Key", "CultureId", Name = "UI_Resource_Type_Key_CultureId", IsUnique = true)]
public partial class Resource
{
    [Key]
    public int ResourceId { get; set; }

    [Required]
    [StringLength(1500)]
    public string Type { get; set; }

    [Required]
    [StringLength(50)]
    public string Key { get; set; }

    [Required]
    [Column(TypeName = "text")]
    public string Value { get; set; }

    public int CultureId { get; set; }

    [ForeignKey("CultureId")]
    [InverseProperty("Resource")]
    public virtual Culture Culture { get; set; }
}