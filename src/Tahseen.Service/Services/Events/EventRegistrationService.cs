using AutoMapper;
using Tahseen.Data.IRepositories;
using Tahseen.Domain.Entities.Events;
using Tahseen.Service.DTOs.Events.EventRegistration;
using Tahseen.Service.DTOs.Events.Events;

namespace Tahseen.Service.Services.Events;

public class EventRegistrationService
{
    private readonly IMapper _mapper;
    private readonly IRepository<EventRegistration> _repository;

    public EventRegistrationService(IMapper mapper, IRepository<EventRegistration> repository)
    {
        this._mapper = mapper;
        this._repository = repository;
    }

    public async Task<EventRegistrationForResultDto> AddAsync(EventRegistrationForCreationDto dto)
    {
        var eventRegistration = _mapper.Map<EventRegistration>(dto);
        var result= await _repository.CreateAsync(eventRegistration);
        return _mapper.Map<EventRegistrationForResultDto>(result);
    }

    public async Task<EventRegistrationForResultDto> ModifyAsync(long id, EventRegistrationForUpdateDto dto)
    {
        var eventRegistration = await _repository.SelectByIdAsync(id);
        if (eventRegistration is not null && !eventRegistration.IsDeleted)
        {
            var mappedEventRegistration = _mapper.Map<EventRegistration>(dto);
            var result = await _repository.UpdateAsync(mappedEventRegistration);
            result.UpdatedAt = DateTime.UtcNow;
            return _mapper.Map<EventRegistrationForResultDto>(result);
        }
        throw new Exception("EventRegistration not found");
    }

    public async Task<bool> RemoveAsync(long id)
    {
        return await _repository.DeleteAsync(id);
    }

    public async ValueTask<EventRegistrationForResultDto> RetrieveByIdAsync(long id)
    {
        var eventRegistration = await _repository.SelectByIdAsync(id);
        if (eventRegistration is not null && !eventRegistration.IsDeleted)
            return _mapper.Map<EventRegistrationForResultDto>(eventRegistration);
        
        throw new Exception("EventRegistration not found");
    }
}