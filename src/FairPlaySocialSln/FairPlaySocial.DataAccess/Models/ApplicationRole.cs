﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FairPlaySocial.DataAccess.Models
{
    [Index("Name", Name = "UI_ApplicationRole_Name", IsUnique = true)]
    public partial class ApplicationRole
    {
        public ApplicationRole()
        {
            ApplicationUserRole = new HashSet<ApplicationUserRole>();
        }

        [Key]
        public short ApplicationRoleId { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        [Required]
        [StringLength(250)]
        public string Description { get; set; }

        [InverseProperty("ApplicationRole")]
        public virtual ICollection<ApplicationUserRole> ApplicationUserRole { get; set; }
    }
}