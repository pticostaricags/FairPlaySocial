﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FairPlaySocial.DataAccess.Models;

[Index("PostId", "DislikingApplicationUserId", Name = "UI_DislikedPost_PoistId_DislikingApplicationUserId", IsUnique = true)]
public partial class DislikedPost
{
    [Key]
    public long DislikedPostId { get; set; }

    public long PostId { get; set; }

    public long DislikingApplicationUserId { get; set; }

    [ForeignKey("DislikingApplicationUserId")]
    [InverseProperty("DislikedPost")]
    public virtual ApplicationUser DislikingApplicationUser { get; set; }

    [ForeignKey("PostId")]
    [InverseProperty("DislikedPost")]
    public virtual Post Post { get; set; }
}