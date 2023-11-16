﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Tahseen.Data.IRepositories;
using Tahseen.Domain.Entities.AudioBooks;
using Tahseen.Domain.Entities.Narrators;
using Tahseen.Service.DTOs.FileUpload;
using Tahseen.Service.DTOs.Narrators;
using Tahseen.Service.Exceptions;
using Tahseen.Service.Interfaces.IFileUploadService;
using Tahseen.Service.Interfaces.INurratorServices;

namespace Tahseen.Service.Services.Narrators;

public class NarratorService : INarratorService
{
    private readonly IMapper _mapper;
    private readonly IRepository<Narrator> _repository;
    private readonly IRepository<AudioBook> _audioBookRepository;
    private readonly IFileUploadService _fileUploadService;

    public NarratorService(
        IMapper mapper,
        IRepository<Narrator> repository, 
        IRepository<AudioBook> audioBookRepository,
        IFileUploadService fileUploadService)
    {
        _mapper = mapper;
        _repository = repository;
        _audioBookRepository = audioBookRepository;
        _fileUploadService = fileUploadService;
    }
    public async Task<NarratorForResultDto> AddAsync(NarratorForCreationDto dto)
    {
        var narrator = await _repository.SelectAll()
            .Where(n => n.FirstName.ToLower() == dto.FirstName.ToLower() &&
            n.LastName.ToLower() == dto.LastName.ToLower() &&
            n.IsDeleted == false)
            .FirstOrDefaultAsync();

        if (narrator is not null)
            throw new TahseenException(409, "Narrator is already exist");

        var FileUploadForCreation = new FileUploadForCreationDto
        {
            FolderPath = "NarratorsAssets",
            FormFile = dto.Image,
        };
        var FileResult = await this._fileUploadService.FileUploadAsync(FileUploadForCreation);

        var mapped = _mapper.Map<Narrator>(dto);
        mapped.Image = Path.Combine("Assets", $"{FileResult.FolderPath}", FileResult.FileName);

        var result = await _repository.CreateAsync(mapped);

        return _mapper.Map<NarratorForResultDto>(result);
    }

    public async Task<NarratorForResultDto> ModifyAsync(long id, NarratorForUpdateDto dto)
    {
        var narrator = await _repository.SelectAll()
            .Where(n => n.Id ==  id && n.IsDeleted == false)
            .FirstOrDefaultAsync();

        if (narrator is null)
            throw new TahseenException(404, "Narrator is not found");

        await _fileUploadService.FileDeleteAsync(narrator.Image);

        var FileUploadForCreation = new FileUploadForCreationDto
        {
            FolderPath = "NarratorsAssets",
            FormFile = dto.Image,
        };
        var FileResult = await this._fileUploadService.FileUploadAsync(FileUploadForCreation);

        var mapped = _mapper.Map(dto, narrator);
        mapped.UpdatedAt = DateTime.UtcNow;
        mapped.Image = Path.Combine("Assets", $"{FileResult.FolderPath}", FileResult.FileName);

        var result = await _repository.UpdateAsync(mapped);

        return _mapper.Map<NarratorForResultDto>(result);
    }

    public async Task<bool> RemoveAsync(long id)
    {
        var narrator = await _repository.SelectAll()
            .Where(n => n.Id == id && n.IsDeleted == false)
            .FirstOrDefaultAsync();

        if (narrator is null)
            throw new TahseenException(404, "Narrator is not found");

        await _fileUploadService.FileDeleteAsync (narrator.Image);

        return await _repository.DeleteAsync(id);
    }

    public async Task<IEnumerable<NarratorForResultDto>> RetrieveAllAsync()
    {
        var narrators = await _repository.SelectAll()   
            .Where(n => n.IsDeleted == false)
            .Include(a => a.AudioBooks)
            .AsNoTracking()
            .ToListAsync();
        foreach (var narrator in narrators)
        {
            narrator.Image = $"https://localhost:7020/{narrator.Image.Replace('\\', '/').TrimStart('/')}";
        }
        return _mapper.Map<IEnumerable<NarratorForResultDto>>(narrators);
    }

    public async Task<NarratorForResultDto> RetrieveByIdAsync(long id)
    {
        var narrator = await _repository.SelectAll()
            .Where(n => n.Id == id && n.IsDeleted == false)
            .Include(a => a.AudioBooks)
            .AsNoTracking()
            .FirstOrDefaultAsync();

        if (narrator is null)
            throw new TahseenException(404, "Narrator is not found");

        narrator.Image = $"https://localhost:7020/{narrator.Image.Replace('\\', '/').TrimStart('/')}";

        return _mapper.Map<NarratorForResultDto>(narrator);
    }
}
