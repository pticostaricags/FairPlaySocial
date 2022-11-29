﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FairPlaySocial.DataAccess.Models;

[Index("PostId", "Tag", Name = "UI_PostTag_PostId_Tag", IsUnique = true)]
public partial class PostTag
{
    [Key]
    public long PostTagId { get; set; }

    public long PostId { get; set; }

    [Required]
    [StringLength(50)]
    public string Tag { get; set; }

    [ForeignKey("PostId")]
    [InverseProperty("PostTag")]
    public virtual Post Post { get; set; }
}