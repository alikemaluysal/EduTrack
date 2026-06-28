
using Core.Results;
using EduTrack.Application.DTOs.Course;
using EduTrack.Application.Services.Abstract;
using EduTrack.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EduTrack.Application.Services.Concrete;

public class CourseService(AppDbContext context) : ICourseService
{
    public async Task<Result<List<CourseDto>>> GetAllCoursesAsync()
    {
        var courses = await context.Courses
            .Include(c => c.Instructor)
            .Include(c => c.Students)
            .Select(c => new CourseDto
            {
                Id = c.Id,
                Title = c.Title,
                Description = c.Description,
                InstructorFullName = $"{c.Instructor.FirstName} {c.Instructor.LastName}",
                StudentCount = c.Students.Count,
            })
            .ToListAsync();

        return Result<List<CourseDto>>.Ok(courses);
    }

    public async Task<Result<List<CourseDto>>> GetAllCoursesForInstructorAsync(Guid instructorId)
    {
        var courses = await context.Courses
            .Include(c => c.Instructor)
            .Include(c => c.Students)
            .Where(c => c.InstructorId == instructorId)
            .Select(c => new CourseDto
            {
                Id = c.Id,
                Title = c.Title,
                Description = c.Description,
                InstructorFullName = $"{c.Instructor.FirstName} {c.Instructor.LastName}",
                StudentCount = c.Students.Count,
            })
            .ToListAsync();

        return Result<List<CourseDto>>.Ok(courses);
    }

    public async Task<Result<List<CourseDto>>> GetAllCoursesForStudentAsync(Guid studentId)
    {
        var courses = await context.Courses
            .Include(c => c.Instructor)
            .Include(c => c.Students)
            .Where(c => c.Students.Any(s => s.Id == studentId))
            .Select(c => new CourseDto
            {
                Id = c.Id,
                Title = c.Title,
                Description = c.Description,
                InstructorFullName = $"{c.Instructor.FirstName} {c.Instructor.LastName}",
                StudentCount = c.Students.Count,
            })
            .ToListAsync();

        return Result<List<CourseDto>>.Ok(courses);
    }
}
