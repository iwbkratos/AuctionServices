using System.ComponentModel.DataAnnotations.Schema;

namespace AuctionServices.Entities
{
    public enum Status
    {
        Live,
        Finished,
        ReserveNotMet
    }
}
