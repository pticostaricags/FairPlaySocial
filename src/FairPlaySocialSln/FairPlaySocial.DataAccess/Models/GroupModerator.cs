﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FairPlaySocial.DataAccess.Models;

public partial class GroupModerator
{
    [Key]
    public long GroupModeratorId { get; set; }

    public long GroupId { get; set; }

    public long ModeratorApplicationUserId { get; set; }

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

    [ForeignKey("GroupId")]
    [InverseProperty("GroupModerator")]
    public virtual Group Group { get; set; }

    [ForeignKey("ModeratorApplicationUserId")]
    [InverseProperty("GroupModerator")]
    public virtual ApplicationUser ModeratorApplicationUser { get; set; }
}