using AutoMapper;
using BlockchainSimulator.BusinessLogic.Model.Responses;
using BlockchainSimulator.WebApi.Models.Responses;

namespace BlockchainSimulator.WebApi.Models.MappingProfiles
{
    public class ResponsesProfile : Profile
    {
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