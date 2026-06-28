using Core.Results;
using EduTrack.Application.DTOs.Course;

namespace EduTrack.Application.Services.Abstract;

public interface ICourseService
{
    Task<Result<List<CourseListDto>>> GetAllCoursesAsync();
    Task<Result<List<CourseListDto>>> GetAllCoursesForInstructorAsync(Guid instructorId);
    Task<Result<List<CourseListDto>>> GetAllCoursesForStudentAsync(Guid studentId);
    Task<Result<CourseCreatedResponse>> CreateCourseAsync(CreateCourseRequest request);
    Task<Result> JoinCourseAsync(JoinCourseRequest request);

}
