
using Core.Exceptions;
using Core.Results;
using EduTrack.Application.BusinessRules;
using EduTrack.Application.DTOs.Course;
using EduTrack.Application.Services.Abstract;
using EduTrack.Domain.Entities;
using EduTrack.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EduTrack.Application.Services.Concrete;

public class CourseService(AppDbContext context, CourseBusinessRules rules) : ICourseService
{
    public async Task<Result<CourseCreatedResponse>> CreateCourseAsync(CreateCourseRequest request)
    {

        try
        {
            var instructor = await context.Users
                      .FirstOrDefaultAsync(u => u.Id == request.InstructorId);

            rules.CheckUserExists(instructor);

            var course = new Course
            {
                Title = request.Title,
                Description = request.Description,
                InstructorId = request.InstructorId,
                IsActive = true,
                CreatedAt = DateTime.Now,
                Code = await GenerateUniqueCourseCode()
            };

            context.Courses.Add(course);
            await context.SaveChangesAsync();

            var response = new CourseCreatedResponse
            {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description,
                IsActive = course.IsActive,
                Code = course.Code,
                InstructorId = course.InstructorId,
                CreatedAt = course.CreatedAt
            };

            return Result<CourseCreatedResponse>.Ok(response);

        }
        catch (BusinessException ex)
        {
            return Result<CourseCreatedResponse>.Fail(ex.Message);
        }
    }



    public async Task<Result<List<CourseListDto>>> GetAllCoursesAsync()
    {
        var courses = await context.Courses
            .Include(c => c.Instructor)
            .Include(c => c.Students)
            .Select(c => new CourseListDto
            {
                Id = c.Id,
                Title = c.Title,
                Description = c.Description,
                InstructorFullName = $"{c.Instructor.FirstName} {c.Instructor.LastName}",
                StudentCount = c.Students.Count,
                Code = c.Code
            })
            .ToListAsync();

        return Result<List<CourseListDto>>.Ok(courses);
    }

    public async Task<Result<List<CourseListDto>>> GetAllCoursesForInstructorAsync(Guid instructorId)
    {
        var courses = await context.Courses
            .Include(c => c.Instructor)
            .Include(c => c.Students)
            .Where(c => c.InstructorId == instructorId)
            .Select(c => new CourseListDto
            {
                Id = c.Id,
                Title = c.Title,
                Description = c.Description,
                InstructorFullName = $"{c.Instructor.FirstName} {c.Instructor.LastName}",
                StudentCount = c.Students.Count,
                Code = c.Code
            })
            .ToListAsync();

        return Result<List<CourseListDto>>.Ok(courses);
    }

    public async Task<Result<List<CourseListDto>>> GetAllCoursesForStudentAsync(Guid studentId)
    {
        var courses = await context.Courses
            .Include(c => c.Instructor)
            .Include(c => c.Students)
            .Where(c => c.Students.Any(s => s.UserId == studentId))
            .Select(c => new CourseListDto
            {
                Id = c.Id,
                Title = c.Title,
                Description = c.Description,
                InstructorFullName = $"{c.Instructor.FirstName} {c.Instructor.LastName}",
                StudentCount = c.Students.Count,
            })
            .ToListAsync();

        return Result<List<CourseListDto>>.Ok(courses);
    }

    public async Task<Result> JoinCourseAsync(JoinCourseRequest request)
    {

        try
        {
            var student = await context.Users.FirstOrDefaultAsync(u => u.Id == request.StudentId);
            rules.CheckUserExists(student);

            var course = await context.Courses.FirstOrDefaultAsync(c => c.Code == request.Code);
            rules.CheckCourseExists(course);

            await rules.CheckStudentAlreadyEnrolled(course, student);

            var courseStudent = new CourseStudent
            {
                CourseId = course!.Id,
                UserId = student!.Id,
                EnrolledAt = DateTime.Now
            };

            context.CourseStudent.Add(courseStudent);
            await context.SaveChangesAsync();

            return Result.Ok();

        }
        catch (BusinessException ex)
        {
            return Result.Fail(ex.Message);
        }

    }


    private async Task<string> GenerateUniqueCourseCode()
    {
        string code;
        do
        {
            code = Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
        } while (await context.Courses.AnyAsync(c => c.Code == code));
        return code;
    }
}
