using Application.Commons.Bases.Request;
using Application.Commons.Bases.Response;
using Application.Dtos.Request.Modules;
using Application.Dtos.Response.Modules;

namespace Application.Interfaces
{
    public interface IModulesService
    {
        Task<BaseResponse<IEnumerable<ModulesResponseDto>>> ListModules(BaseFiltersRequest filters);
        Task<BaseResponse<ModulesResponseDto>> ModuleById(int moduleId);
        Task<BaseResponse<bool>> RegisterModule(int authenticatedUserId, ModulesRequestDto requestDto);
        Task<BaseResponse<bool>> EditModule(int authenticatedUserId, int moduleId, ModulesRequestDto requestDto);
        Task<BaseResponse<bool>> EnableModule(int authenticatedUserId, int moduleId);
        Task<BaseResponse<bool>> DisableModule(int authenticatedUserId, int moduleId);
        Task<BaseResponse<bool>> RemoveModule(int authenticatedUserId, int moduleId);
    }
}
    