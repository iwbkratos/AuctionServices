using AuctionServices.Data;
using AuctionServices.DTO;
using AuctionServices.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using System.Security.Principal;

namespace AuctionServices.Controllers
{
    [ApiController]
    [Route("api/auctions")]
    public class AuctionsController : ControllerBase
    {
        private readonly AuctionDbContext _dbContext;
        private readonly IMapper _mapper;

        public AuctionsController(AuctionDbContext dbContext, IMapper mapper) {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<AuctionDTO>>> GetAllAuctions() {
            var auctions = await _dbContext.Auctions
                .Include(a => a.Item)
                .OrderBy(i => i.Item.Make)
                .ToListAsync();

            return _mapper.Map<List<AuctionDTO>>(auctions);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AuctionDTO>> GetAuctionById(Guid id) {
            var auction = await _dbContext.Auctions
                .FirstOrDefaultAsync(a => a.Id == id);
            if (auction == null)
                return NotFound();
            return Ok(_mapper.Map<AuctionDTO>(auction));
        }

        [HttpPost]
        public async Task<ActionResult<AuctionDTO>> CreateAuction(CreateAuctionDTO auctionDTO) {

            var auction = _mapper.Map<Auction>(auctionDTO);
            auction.Seller = "test";

            _dbContext.Auctions.Add(auction);
            var result = _dbContext.SaveChanges() > 0;

            if (!result)
                BadRequest("Could not create auction!");

            return CreatedAtAction(nameof(GetAuctionById), new { auction.Id }, _mapper.Map<AuctionDTO>(auction));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<AuctionDTO>> UpdateAuction(Guid id, UpdateAuctionDto updateAuctionDto) {

            var auction =await _dbContext.Auctions
                .Include(a => a.Item)
                .FirstOrDefaultAsync();

            if (auction == null) 
                return NotFound($"There is no record for this id : {id}");

            auction.Item.Make = updateAuctionDto.Make ?? auction.Item.Make;
            auction.Item.Model = updateAuctionDto.Model ?? auction.Item.Model;
            auction.Item.Color = updateAuctionDto.Color ?? auction.Item.Color;
            auction.Item.Mileage = updateAuctionDto.Mileage ?? auction.Item.Mileage;
            auction.Item.Mileage = updateAuctionDto.Mileage ?? auction.Item.Year;

            _dbContext.Auctions.Update(auction);
            var result = _dbContext.SaveChanges() > 0;

            if (!result)
                return BadRequest("Problem in saving changes");

            return _mapper.Map<AuctionDTO>(auction);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAuction(Guid id)
        {
            var auction = await _dbContext.Auctions.FindAsync(id);
            if (auction == null)
                return NotFound($"There is no such record for this id:{id}");

            _dbContext.Auctions.Remove(auction);
            var result = await _dbContext.SaveChangesAsync() > 0;

            if (!result)
                return BadRequest("Problem in deleting the record");
            return Ok();
        }
    }
}
