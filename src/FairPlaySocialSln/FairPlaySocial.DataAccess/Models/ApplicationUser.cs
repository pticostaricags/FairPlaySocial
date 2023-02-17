﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FairPlaySocial.DataAccess.Models;

[Index("AzureAdB2cobjectId", Name = "UI_ApplicationUser_AzureAdB2CObjectId", IsUnique = true)]
[Index("EmailAddress", Name = "UI_ApplicationUser_EmailAddress", IsUnique = true)]
public partial class ApplicationUser
{
    [Key]
    public long ApplicationUserId { get; set; }

    [Required]
    [StringLength(150)]
    public string FullName { get; set; }

    [Required]
    [StringLength(150)]
    public string EmailAddress { get; set; }

    public DateTimeOffset LastLogIn { get; set; }

    [Column("AzureAdB2CObjectId")]
    public Guid AzureAdB2cobjectId { get; set; }

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

    [InverseProperty("FollowedApplicationUser")]
    public virtual ICollection<ApplicationUserFollow> ApplicationUserFollowFollowedApplicationUser { get; } = new List<ApplicationUserFollow>();

    [InverseProperty("FollowerApplicationUser")]
    public virtual ICollection<ApplicationUserFollow> ApplicationUserFollowFollowerApplicationUser { get; } = new List<ApplicationUserFollow>();

    [InverseProperty("ApplicationUser")]
    public virtual ICollection<ApplicationUserRole> ApplicationUserRole { get; } = new List<ApplicationUserRole>();

    [InverseProperty("DislikingApplicationUser")]
    public virtual ICollection<DislikedPost> DislikedPost { get; } = new List<DislikedPost>();

    [InverseProperty("OwnerApplicationUser")]
    public virtual ICollection<Group> Group { get; } = new List<Group>();

    [InverseProperty("MemberApplicationUser")]
    public virtual ICollection<GroupMember> GroupMember { get; } = new List<GroupMember>();

    [InverseProperty("ModeratorApplicationUser")]
    public virtual ICollection<GroupModerator> GroupModerator { get; } = new List<GroupModerator>();

    [InverseProperty("LikingApplicationUser")]
    public virtual ICollection<LikedPost> LikedPost { get; } = new List<LikedPost>();

    [InverseProperty("OwnerApplicationUser")]
    public virtual ICollection<Post> Post { get; } = new List<Post>();

    [InverseProperty("ReachedByApplicationUser")]
    public virtual ICollection<PostReach> PostReach { get; } = new List<PostReach>();

    [InverseProperty("VisitedApplicationUser")]
    public virtual ICollection<ProfileVisitor> ProfileVisitorVisitedApplicationUser { get; } = new List<ProfileVisitor>();

    [InverseProperty("VisitorApplicationUser")]
    public virtual ICollection<ProfileVisitor> ProfileVisitorVisitorApplicationUser { get; } = new List<ProfileVisitor>();

    [InverseProperty("FromApplicationUser")]
    public virtual ICollection<UserMessage> UserMessageFromApplicationUser { get; } = new List<UserMessage>();

    [InverseProperty("ToApplicationUser")]
    public virtual ICollection<UserMessage> UserMessageToApplicationUser { get; } = new List<UserMessage>();

    [InverseProperty("ApplicationUser")]
    public virtual UserPreference UserPreference { get; set; }

    [InverseProperty("ApplicationUser")]
    public virtual UserProfile UserProfile { get; set; }

    [InverseProperty("ApplicationUser")]
    public virtual ICollection<VisitorTracking> VisitorTracking { get; } = new List<VisitorTracking>();
}