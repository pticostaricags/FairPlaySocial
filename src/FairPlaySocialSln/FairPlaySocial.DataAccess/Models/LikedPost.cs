﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FairPlaySocial.DataAccess.Models;

[Index("PostId", "LikingApplicationUserId", Name = "UI_LikedPost_PoistId_LikingApplicationUserId", IsUnique = true)]
public partial class LikedPost
{
    [Key]
    public long LikedPostId { get; set; }

    public long PostId { get; set; }

    public long LikingApplicationUserId { get; set; }

    [ForeignKey("LikingApplicationUserId")]
    [InverseProperty("LikedPost")]
    public virtual ApplicationUser LikingApplicationUser { get; set; }

    [ForeignKey("PostId")]
    [InverseProperty("LikedPost")]
    public virtual Post Post { get; set; }
}