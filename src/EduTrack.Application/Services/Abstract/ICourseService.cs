using Core.Results;
using EduTrack.Application.DTOs.Course;

namespace EduTrack.Application.Services.Abstract;

public interface ICourseService
{
    Task<Result<List<CourseListDto>>> GetUserCoursesAsync(Guid userId);
    Task<Result<CourseCreatedResponse>> CreateCourseAsync(CreateCourseRequest request);
    Task<Result> JoinCourseAsync(JoinCourseRequest request);

}
