using AuctionServices.DTO;
using AuctionServices.Entities;
using AutoMapper;

namespace AuctionServices.RequestHelper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Auction, AuctionDTO>().IncludeMembers(a => a.Item);
            CreateMap<Item, AuctionDTO>();
            CreateMap<CreateAuctionDTO, Auction>().ForMember(o => o.Item, d => d.MapFrom(d => d));
            CreateMap<CreateAuctionDTO, Item>();
        }
    }
}
