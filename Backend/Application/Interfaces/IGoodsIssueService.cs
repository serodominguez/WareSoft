using Application.Commons.Bases.Request;
using Application.Commons.Bases.Response;
using Application.Dtos.Request.GoodsIssue;
using Application.Dtos.Response.GoodsIssue;

namespace Application.Interfaces
{
    public interface IGoodsIssueService
    {
        Task<BaseResponse<IEnumerable<GoodsIssueResponseDto>>> ListGoodsIssue(BaseFiltersRequest filters);
        Task<BaseResponse<GoodsIssueWithDetailsResponseDto>> GoodsIssueById(int issueId);
        Task<BaseResponse<bool>> RegisterGoodsIssue(int authenticatedUserId, GoodsIssueRequestDto requestDto);
        Task<BaseResponse<bool>> CancelGoodsIssue(int authenticatedUserId, int issueId);
    }
}
