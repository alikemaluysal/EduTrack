using Core.Results;
using EduTrack.Application.DTOs.CourseMaterial;
using EduTrack.Application.Services.Abstract;

namespace EduTrack.Application.Services.Concrete;

public class CourseMaterialService(IFileService fileService) : ICourseMaterialService
{
    public Task<Result<CourseMaterialDto>> AddCourseMaterialAsync(CreateCourseMaterialRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<Result<CourseMaterialDto>> GetCourseMaterialByIdAsync(Guid courseMaterialId)
    {
        throw new NotImplementedException();
    }

    public Task<Result<List<CourseMaterialDto>>> GetCourseMaterialsByCourseIdAsync(Guid courseId)
    {
        throw new NotImplementedException();
    }
}
