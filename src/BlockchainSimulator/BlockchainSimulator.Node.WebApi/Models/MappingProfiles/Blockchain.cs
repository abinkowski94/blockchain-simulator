using AutoMapper;
using BlockchainSimulator.Node.WebApi.Models.Blockchain;

namespace BlockchainSimulator.Node.WebApi.Models.MappingProfiles
{
    /// <summary>
    /// The blockchain profile
    /// </summary>
    public class BlockchainProfile : Profile
    {
        /// <summary>
        /// The constructor
        /// </summary>
        public BlockchainProfile()
        {
            CreateMap<BlockchainMetadata, DataAccess.Model.BlockchainMetadata>().ReverseMap();
        }
    }
}