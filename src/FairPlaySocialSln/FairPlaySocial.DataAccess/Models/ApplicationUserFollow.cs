﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FairPlaySocial.DataAccess.Models;

[Index("FollowerApplicationUserId", "FollowedApplicationUserId", Name = "UI_ApplicationUserFollow_FollowerApplicationUserId_FollowedApplicationUserId", IsUnique = true)]
public partial class ApplicationUserFollow
{
    [Key]
    public long ApplicationUserFollowId { get; set; }

    public long FollowerApplicationUserId { get; set; }

    public long FollowedApplicationUserId { get; set; }

    public DateTimeOffset RowCreationDateTime { get; set; }

    [Required]
    [StringLength(256)]
    public string RowCreationUser { get; set; }

    [Required]
    [StringLength(250)]
    public string SourceApplication { get; set; }

    [Required]
    [Column("OriginatorIPAddress")]
    [StringLength(100)]
    public string OriginatorIpaddress { get; set; }

    [ForeignKey("FollowedApplicationUserId")]
    [InverseProperty("ApplicationUserFollowFollowedApplicationUser")]
    public virtual ApplicationUser FollowedApplicationUser { get; set; }

    [ForeignKey("FollowerApplicationUserId")]
    [InverseProperty("ApplicationUserFollowFollowerApplicationUser")]
    public virtual ApplicationUser FollowerApplicationUser { get; set; }
}