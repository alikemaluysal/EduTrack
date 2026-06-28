using EduTrack.Application.DTOs.Course;
using EduTrack.Web.Models.Course;

namespace EduTrack.Web.Models.Home;

public class HomeViewModel
{
    public List<CourseListDto> Courses { get; set; } = new();
    public CreateCourseViewModel? CreateCourseViewModel { get; set; }
    public JoinCourseViewModel? JoinCourseViewModel { get; set; }
}
