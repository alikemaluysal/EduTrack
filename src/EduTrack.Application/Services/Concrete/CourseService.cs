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



    public async Task<Result<List<CourseListDto>>> GetAllCoursesAsync()
    {

        var courses = await courseRepository.GetListAsync(
        include: c => c.Include(c => c.Instructor).Include(c => c.Students)
        );

        var result = courses
            .Select(c => new CourseListDto
            {
                Id = c.Id,
                Title = c.Title,
                Description = c.Description,
                InstructorFullName = $"{c.Instructor.FirstName} {c.Instructor.LastName}",
                StudentCount = c.Students.Count,
                Code = c.Code
            }).ToList();

        return Result<List<CourseListDto>>.Ok(result);
    }

    public async Task<Result<List<CourseListDto>>> GetAllCoursesForInstructorAsync(Guid instructorId)
    {
        var courses = await courseRepository.GetListAsync(
        predicate: c => c.InstructorId == instructorId,
        include: c => c.Include(c => c.Instructor).Include(c => c.Students)
        );

        var result = courses
            .Select(c => new CourseListDto
            {
                Id = c.Id,
                Title = c.Title,
                Description = c.Description,
                InstructorFullName = $"{c.Instructor.FirstName} {c.Instructor.LastName}",
                StudentCount = c.Students.Count,
                Code = c.Code
            }).ToList();

        return Result<List<CourseListDto>>.Ok(result);
    }

    public async Task<Result<List<CourseListDto>>> GetAllCoursesForStudentAsync(Guid studentId)
    {
        var courses = await courseRepository.GetListAsync(
        predicate: c=> c.Students.Any(s => s.UserId == studentId),
        include: c => c.Include(c => c.Instructor).Include(c => c.Students)
        );

        var result = courses
            .Select(c => new CourseListDto
            {
                Id = c.Id,
                Title = c.Title,
                Description = c.Description,
                InstructorFullName = $"{c.Instructor.FirstName} {c.Instructor.LastName}",
                StudentCount = c.Students.Count,
                Code = c.Code
            }).ToList();

        return Result<List<CourseListDto>>.Ok(result);
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
