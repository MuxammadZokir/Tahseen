using AutoMapper;
using Tahseen.Data.IRepositories;
using Tahseen.Domain.Entities.Feedback;
using Tahseen.Service.DTOs.Feedbacks.Feedback;
using Tahseen.Service.Exceptions;
using Tahseen.Service.Interfaces.IFeedbackService;

namespace Tahseen.Service.Services.Feedbacks;

public class FeedbackService:IFeedbackService
{
    private readonly IMapper _mapper;
    private readonly IRepository<Feedback> _repository;

    public FeedbackService(IMapper mapper, IRepository<Feedback>repository)
    {
        _mapper = mapper;
        _repository = repository;
    }
    public async Task<FeedbackForResultDto> AddAsync(FeedbackForCreationDto dto)
    {
        var mapped = _mapper.Map<Feedback> (dto);
        var result = await _repository.CreateAsync(mapped);
        return _mapper.Map<FeedbackForResultDto>(result); 
    }

    public async Task<FeedbackForResultDto> ModifyAsync(long id, FeedbackForUpdateDto dto)
    {
        var bookReview = await _repository.SelectByIdAsync (id);
        if (bookReview == null || bookReview.IsDeleted)
            throw new TahseenException(404, "Feedback doesn't found");

        var mapped = _mapper.Map(dto, bookReview);

        var result = await _repository.UpdateAsync(mapped);
        return _mapper.Map<FeedbackForResultDto>(result);
    }

    public async Task<bool> RemoveAsync(long id)
        => await _repository.DeleteAsync(id);

    public async ValueTask<FeedbackForResultDto?> RetrieveByIdAsync(long id)
    {
        var bookReview = await _repository.SelectByIdAsync(id);
        if (bookReview == null || bookReview.IsDeleted)
            throw new TahseenException(404, "Feedback doesn't found");

        return _mapper.Map<FeedbackForResultDto>(bookReview);
    }

    public IQueryable<FeedbackForResultDto> RetrieveAll()
    {
        var booksReview =  _repository.SelectAll().Where(x=>!x.IsDeleted);
        
        return _mapper.Map<IQueryable<FeedbackForResultDto>>(booksReview);
    }
}