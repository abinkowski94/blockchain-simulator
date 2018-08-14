using BlockchainSimulator.BusinessLogic.Model.Responses;
using BlockchainSimulator.WebApi.Controllers;
using BlockchainSimulator.WebApi.Models.Responses;

namespace BlockchainSimulator.WebApi.Extensions
{
    public static class BaseResponseExtension
    {
        public static BaseResponse GetBaseResponse<TSource, TDestination>(this BaseResponse<TSource> baseResponse,
            BaseController controller)
        {
            var result = controller.LocalMapper.Map<TDestination>(baseResponse.Result);
            var response = controller.LocalMapper.Map<BaseResponse>(baseResponse);
            response.Result = result;

            return response;
        }
    }
}