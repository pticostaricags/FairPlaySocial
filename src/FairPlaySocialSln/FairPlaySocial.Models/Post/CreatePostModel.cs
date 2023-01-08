using FairPlaySocial.Common.CustomAttributes.Localization;
using FairPlaySocial.CustomValidation.CustomValidationAttributes;
using FairPlaySocial.Models.Photo;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;

namespace FairPlaySocial.Models.Post
{
    public class CreatePostModel
    {
        private const string PATTERN_JUST_ONE_WORD = @"^\b[a-zA-Z0-9_]+\b$";
        public long? GroupId { get; set; }
        [Required(ErrorMessageResourceName = nameof(CreatePostModelLocalizer.TextRequired),
            ErrorMessageResourceType = typeof(CreatePostModelLocalizer))]
        [StringLength(500,
            ErrorMessageResourceName = nameof(CreatePostModelLocalizer.TextStringLength),
            ErrorMessageResourceType = typeof(CreatePostModelLocalizer))]
        [ProhibitHashTags(ErrorMessage = "Text cannot contain HashTags",
            ErrorMessageResourceName = nameof(CreatePostModelLocalizer.TextProhibitHashTags),
            ErrorMessageResourceType = typeof(CreatePostModelLocalizer))]
        [ProhibitUrls(ErrorMessage = "Text cannot contain Urls",
            ErrorMessageResourceName = nameof(CreatePostModelLocalizer.TextProhibitUrls),
            ErrorMessageResourceType = typeof(CreatePostModelLocalizer))]
        [Display(Name = nameof(CreatePostModelLocalizer.TextDisplayName),
            ResourceType = typeof(CreatePostModelLocalizer))]
        public string? Text { get; set; }
        [Required]
        [ValidateComplexType]
        public CreatePhotoModel? Photo { get; set; }
        [Required(ErrorMessageResourceName = nameof(CreatePostModelLocalizer.TagRequired),
            ErrorMessageResourceType = typeof(CreatePostModelLocalizer))]
        [RegularExpression(PATTERN_JUST_ONE_WORD,
            ErrorMessageResourceName = nameof(CreatePostModelLocalizer.InvalidTagPattern),
            ErrorMessageResourceType = typeof(CreatePostModelLocalizer))]
        public string? Tag1 { get; set; }
        [Required(ErrorMessageResourceName = nameof(CreatePostModelLocalizer.TagRequired),
            ErrorMessageResourceType = typeof(CreatePostModelLocalizer))]
        [RegularExpression(PATTERN_JUST_ONE_WORD,
            ErrorMessageResourceName = nameof(CreatePostModelLocalizer.InvalidTagPattern),
            ErrorMessageResourceType = typeof(CreatePostModelLocalizer))]
        public string? Tag2 { get; set; }
        [Required(
            ErrorMessageResourceName = nameof(CreatePostModelLocalizer.TagRequired),
            ErrorMessageResourceType = typeof(CreatePostModelLocalizer))]
        [RegularExpression(PATTERN_JUST_ONE_WORD,
            ErrorMessageResourceName = nameof(CreatePostModelLocalizer.InvalidTagPattern),
            ErrorMessageResourceType = typeof(CreatePostModelLocalizer))]
        public string? Tag3 { get; set; }
        [Url(
            ErrorMessageResourceName = nameof(CreatePostModelLocalizer.UrlValidFormatRequired),
            ErrorMessageResourceType = typeof(CreatePostModelLocalizer))]
        public string? Url { get; set; }
        [Required]
        [Range(1, 2)]
        public short? PostVisibilityId { get; set; }
        public double? CreatedAtLatitude { get; set; }
        public double? CreatedAtLongitude { get; set; }
        [ProhibitDuplicateStrings(ErrorMessage = "Tags must not be repeated")]
        public string?[] CombinedTags => new string?[] { Tag1, Tag2, Tag3 };
    }

    public class CreatePostModelLocalizer
    {
        public static IStringLocalizer<CreatePostModelLocalizer>? Localizer { get; set; }
        public static string TextRequired => Localizer![TextRequiredTextKey];
        public static string TextStringLength => Localizer![TextStringLengthTextKey];
        public static string TextProhibitHashTags => Localizer![TextProhibitHashTagsTextKey];
        public static string InvalidTagPattern => Localizer![InvalidTagPatternTextKey];
        public static string TextProhibitUrls => Localizer![TextProhibitUrlsTextKey];
        public static string TagRequired => Localizer![TagRequiredTextKey];
        public static string UrlValidFormatRequired => Localizer![UrlValidFormatRequiredTextKey];
        public static string TextDisplayName => Localizer![TextDisplayNameTextKey];
        #region Resource Keys
        [ResourceKey(defaultValue: "Text is required")]
        public const string TextRequiredTextKey = "TextRequiredText";
        [ResourceKey(defaultValue: "Text can have a maximum of {1} characters")]
        public const string TextStringLengthTextKey = "TextStringLengthText";
        [ResourceKey(defaultValue: "Text cannot have hashtags")]
        public const string TextProhibitHashTagsTextKey = "TextProhibitHashTagsText";
        [ResourceKey(defaultValue: "Text cannot have hashtags")]
        public const string TextProhibitUrlsTextKey = "TextProhibitUrlsText";
        [ResourceKey(defaultValue: "Tags can only contain one word composed of only letters and numbers")]
        public const string InvalidTagPatternTextKey = "InvalidTagPatternText";
        [ResourceKey(defaultValue: "Tag is required")]
        public const string TagRequiredTextKey = "TagRequiredText";
        [ResourceKey(defaultValue: "Urls must have a valid format")]
        public const string UrlValidFormatRequiredTextKey = "UrlValidFormatRequiredText";
        [ResourceKey(defaultValue: "Post Text")]
        public const string TextDisplayNameTextKey = "TextDisplayNameText";
        #endregion Resource Keys
    }
}
