using FairPlaySocial.Common.Interfaces;
using NetTopologySuite.Geometries;

namespace FairPlaySocial.DataAccess.Models
{
    public partial class Post : IOriginatorInfo
    {
        //Check https://docs.microsoft.com/en-us/ef/core/modeling/spatial
        public Point? CreatedAtGeoLocation { get; set; }
    }
}
