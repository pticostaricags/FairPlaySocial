using FairPlaySocial.Common.CustomAttributes.Localization;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace FairPlaySocial.Models.Group
{
    public class CreateGroupModel
    {

        [Required(
            ErrorMessageResourceName = nameof(CreateGroupModelLocalizer.NameRequired),
            ErrorMessageResourceType = typeof(CreateGroupModelLocalizer))]
        [StringLength(50,
            ErrorMessageResourceName = nameof(CreateGroupModelLocalizer.NameStringLength),
            ErrorMessageResourceType = typeof(CreateGroupModelLocalizer))]
        [Display(
            Name = nameof(CreateGroupModelLocalizer.NameDisplay),
            ResourceType = typeof(CreateGroupModelLocalizer))]
        public string? Name { get; set; }

        [Required(
            ErrorMessageResourceName = nameof(CreateGroupModelLocalizer.DescriptionRequired),
            ErrorMessageResourceType = typeof(CreateGroupModelLocalizer))]
        [StringLength(250,
            ErrorMessageResourceName = nameof(CreateGroupModelLocalizer.DescriptionRequired),
            ErrorMessageResourceType = typeof(CreateGroupModelLocalizer))]
        [Display(
            Name = nameof(CreateGroupModelLocalizer.DescriptionDisplay),
            ResourceType = typeof(CreateGroupModelLocalizer))]
        public string? Description { get; set; }

        [Required(
            ErrorMessageResourceName = nameof(CreateGroupModelLocalizer.TopicTagRequired),
            ErrorMessageResourceType = typeof(CreateGroupModelLocalizer))]
        [StringLength(100,
            ErrorMessageResourceName = nameof(CreateGroupModelLocalizer.NameRequired),
            ErrorMessageResourceType = typeof(CreateGroupModelLocalizer))]
        [Display(
            Name = nameof(CreateGroupModelLocalizer.TopicTagDisplay),
            ResourceType = typeof(CreateGroupModelLocalizer))]
        public string? TopicTag { get; set; }
    }

    public class CreateGroupModelLocalizer
    {
        public static IStringLocalizer<CreateGroupModelLocalizer>? Localizer { get; set; }
        public static string NameRequired => Localizer![NameRequiredTextKey];
        public static string NameStringLength => Localizer![NameStringLengthTextKey];
        public static string NameDisplay => Localizer![NameDisplayTextKey];
        public static string DescriptionRequired => Localizer![DescriptionRequiredTextKey];
        public static string DescriptionStringLength => Localizer![DescriptionStringLengthTextKey];
        public static string DescriptionDisplay => Localizer![DescriptionDisplayTextKey];
        public static string TopicTagRequired => Localizer![TopicTagRequiredTextKey];
        public static string TopicTagStringLength => Localizer![TopicTagStringLengthTextKey];
        public static string TopicTagDisplay => Localizer![TopicTagDisplayTextKey];
        #region Resource Keys
        [ResourceKey(defaultValue: "Name is required")]
        public const string NameRequiredTextKey = "NameRequiredText";
        [ResourceKey(defaultValue: "Name")]
        public const string NameDisplayTextKey = "NameDisplayText";
        [ResourceKey(defaultValue: "Name can have a maximum of {1} characters")]
        public const string NameStringLengthTextKey = "NameStringLengthText";
        [ResourceKey(defaultValue: "Description is required")]
        public const string DescriptionRequiredTextKey = "DescriptionRequiredText";
        [ResourceKey(defaultValue: "Description can have a maximum of {1} characters")]
        public const string DescriptionStringLengthTextKey = "DescriptionStringLengthText";
        [ResourceKey(defaultValue: "Description")]
        public const string DescriptionDisplayTextKey = "DescriptionDisplayText";
        [ResourceKey(defaultValue: "Topic Tag is required")]
        public const string TopicTagRequiredTextKey = "TopicTagRequiredText";
        [ResourceKey(defaultValue: "Topic Tag can have a maximum of {1} characters")]
        public const string TopicTagStringLengthTextKey = "TopicTagStringLengthText";
        [ResourceKey(defaultValue: "Topic Tag")]
        public const string TopicTagDisplayTextKey = "TopicTagDisplayText";
        #endregion Resource Keys
    }
}
