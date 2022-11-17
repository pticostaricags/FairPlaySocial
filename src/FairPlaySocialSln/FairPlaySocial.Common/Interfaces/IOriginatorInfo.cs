namespace FairPlaySocial.Common.Interfaces
{
    public interface IOriginatorInfo
    {
        string SourceApplication { get; set; }
        string OriginatorIpaddress { get; set; }
        DateTimeOffset RowCreationDateTime { get; set; }
        string RowCreationUser { get; set; }
    }
}
