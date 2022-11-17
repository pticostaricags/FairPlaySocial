namespace FairPlaySocial.Common
{
    public class UserState
    {
        public static UserContext UserContext { get; set; } = new();
    }
    public class UserContext
    {
        public string? Name { get; set; }
        public string? UserIdentifier { get; set; }
        public bool IsLoggedOn { get; set; }
        public string? GivenName { get; set; }
        public string? FamilyName { get; set; }
        public string? Province { get; set; }
        public string? PostalCode { get; set; }
        public string? Country { get; set; }
        public string? EmailAddress { get; set; }
        public string? JobTitle { get; set; }
        public string? StreetAddress { get; set; }
        public string? City { get; set; }
        public string? AccessToken { get; set; }
        public string? FullName { get; set; }
    }
}
