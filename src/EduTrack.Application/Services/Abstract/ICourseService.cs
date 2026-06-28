using Core.Results;
using EduTrack.Application.DTOs.Course;

namespace EduTrack.Application.Services.Abstract;

public interface ICourseService
{
    Task<Result<List<CourseDto>>> GetAllCoursesAsync();
    Task<Result<List<CourseDto>>> GetAllCoursesForInstructorAsync(Guid instructorId);
    Task<Result<List<CourseDto>>> GetAllCoursesForStudentAsync(Guid studentId);
}
