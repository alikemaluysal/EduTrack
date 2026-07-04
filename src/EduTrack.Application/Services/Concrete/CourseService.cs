using Core.Exceptions;
using Core.Results;
using EduTrack.Application.BusinessRules;
using EduTrack.Application.DTOs.Course;
using EduTrack.Application.Repositories;
using EduTrack.Application.Services.Abstract;
using EduTrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EduTrack.Application.Services.Concrete;

public class CourseService(
    IUserRepository userRepository, 
    ICourseRepository courseRepository,
    ICourseStudentRepository courseStudentRepository,
    CourseBusinessRules rules) : ICourseService
{

    public async Task<Result<List<CourseListDto>>> GetUserCoursesAsync(Guid userId)
    {
        var courses = new List<CourseListDto>();

        var instructorCourses = await  courseRepository.GetListAsync(c => c.InstructorId == userId, include: c => c.Include(c => c.Instructor).Include(c => c.Students));
        var studentCourses = await courseRepository.GetListAsync(c => c.Students.Any(s => s.UserId == userId), include: c => c.Include(c => c.Instructor).Include(c => c.Students));

        courses.AddRange(instructorCourses.Select(c => new CourseListDto
        {
            Id = c.Id,
            Title = c.Title,
            Description = c.Description,
            InstructorFullName = $"{c.Instructor.FirstName} {c.Instructor.LastName}",
            IsInstructor = true,
            StudentCount = c.Students.Count,
            Code = c.Code
        }));

        courses.AddRange(studentCourses.Select(c => new CourseListDto
        {
            Id = c.Id,
            Title = c.Title,
            Description = c.Description,
            InstructorFullName = $"{c.Instructor.FirstName} {c.Instructor.LastName}",
            IsInstructor = false,
            StudentCount = c.Students.Count,
            Code = c.Code
        }));

        return Result<List<CourseListDto>>.Ok(courses);
    }


    public async Task<Result<CourseCreatedResponse>> CreateCourseAsync(CreateCourseRequest request)
    {


        try
        {
            var instructor = await userRepository.GetAsync(u => u.Id == request.InstructorId);

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


            await courseRepository.AddAsync(course);

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




    public async Task<Result> JoinCourseAsync(JoinCourseRequest request)
    {

        try
        {
            var student = await userRepository.GetAsync(u => u.Id == request.StudentId);
            rules.CheckUserExists(student);

            var course = await courseRepository.GetAsync(c => c.Code == request.Code);
            rules.CheckCourseExists(course);

            await rules.CheckStudentAlreadyEnrolled(course, student);

            var courseStudent = new CourseStudent
            {
                CourseId = course!.Id,
                UserId = student!.Id,
                EnrolledAt = DateTime.Now
            };

            await courseStudentRepository.AddAsync(courseStudent);

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
        } while (await courseRepository.AnyAsync(c => c.Code == code));
        return code;
    }
}
