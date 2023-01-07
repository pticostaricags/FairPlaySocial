﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FairPlaySocial.DataAccess.Models;

[Index("Name", Name = "UI_Group_Name", IsUnique = true)]
public partial class Group
{
    [Key]
    public long GroupId { get; set; }

    public long OwnerApplicationUserId { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; }

    [Required]
    [StringLength(250)]
    public string Description { get; set; }

    [Required]
    [StringLength(100)]
    public string TopicTag { get; set; }

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

    [InverseProperty("Group")]
    public virtual ICollection<GroupModerator> GroupModerator { get; } = new List<GroupModerator>();

    [ForeignKey("OwnerApplicationUserId")]
    [InverseProperty("Group")]
    public virtual ApplicationUser OwnerApplicationUser { get; set; }
}