﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FairPlaySocial.DataAccess.Models;

[Index("PostId", "Phrase", Name = "UI_PostKeyPhrase_PostId_Phrase", IsUnique = true)]
public partial class PostKeyPhrase
{
    [Key]
    public long PostKeyPhraseId { get; set; }

    public long PostId { get; set; }

    [Required]
    [StringLength(100)]
    public string Phrase { get; set; }

    [ForeignKey("PostId")]
    [InverseProperty("PostKeyPhrase")]
    public virtual Post Post { get; set; }
}