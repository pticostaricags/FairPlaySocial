﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FairPlaySocial.DataAccess.Models;

[Index("Url", Name = "UI_ForbiddenUrl_Url", IsUnique = true)]
public partial class ForbiddenUrl
{
    [Key]
    public long ForbiddenUrlId { get; set; }

    [Required]
    [StringLength(1000)]
    public string Url { get; set; }
}