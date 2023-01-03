namespace FairPlaySocial.AutomatedTests.BlazorWasmTests.Configuration
{
    public class ClientAppConfiguration
    {
        public string? PathBase { get; set; }
        public string? ContentRoot { get; set; }
        public Azureadb2cscopes? AzureAdB2CScopes { get; set; }
        public Azureadb2c? AzureAdB2C { get; set; }
    }

    public class Azureadb2cscopes
    {
        public string? DefaultScope { get; set; }
    }

    public class Azureadb2c
    {
        public string? Authority { get; set; }
        public string? ClientId { get; set; }
        public bool ValidateAuthority { get; set; }
    }

}
