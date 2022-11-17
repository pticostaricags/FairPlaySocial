namespace FairPlaySocial.Models.CustomExceptions
{
    public class CustomValidationException : Exception
    {
        public CustomValidationException(string message) : base(message)
        {

        }
    }
}
