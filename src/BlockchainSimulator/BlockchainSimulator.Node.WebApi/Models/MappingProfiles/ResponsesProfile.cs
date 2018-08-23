using AutoMapper;
using BlockchainSimulator.Common.Models.Responses;
using BlockchainSimulator.Node.BusinessLogic.Model.Responses;

namespace BlockchainSimulator.Node.WebApi.Models.MappingProfiles
{
    /// <summary>
    /// The profile for the responses
    /// </summary>
    public class ResponsesProfile : Profile
    {
        /// <summary>
        /// The constructor
        /// </summary>
        public ResponsesProfile()
        {
            CreateMap(typeof(BaseResponse<>), typeof(BaseResponse))
                .Include(typeof(SuccessResponse<>), typeof(SuccessResponse))
                .Include(typeof(ErrorResponse<>), typeof(ErrorResponse));

            CreateMap(typeof(SuccessResponse<>), typeof(SuccessResponse));
            CreateMap(typeof(ErrorResponse<>), typeof(ErrorResponse));
        }
    }
}