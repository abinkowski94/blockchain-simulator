using BlockchainSimulator.BusinessLogic.Model.Responses;
using BlockchainSimulator.WebApi.Controllers;
using BlockchainSimulator.WebApi.Models.Responses;

namespace BlockchainSimulator.WebApi.Extensions
{
    /// <summary>
    /// The extensions for the base response
    /// </summary>
    public static class BaseResponseExtension
    {
        /// <summary>
        /// Gets the mapped base response
        /// </summary>
        /// <typeparam name="TSource">The type of the source result</typeparam>
        /// <typeparam name="TDestination">The destination type</typeparam>
        /// <param name="baseResponse">The response</param>
        /// <param name="controller">The controller</param>
        /// <returns>Mapped response</returns>
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