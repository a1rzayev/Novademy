using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Novademy.Application.Data.EFCore;
using Novademy.Application.ExternalServices.Cloudinary;
using Novademy.Application.Models;
using Novademy.Application.Repositories.Abstract;

namespace Novademy.Application.Repositories.Concrete;

public class LessonRepository : ILessonRepository
{
    private readonly AppDbContext _context;
    private readonly ICloudinaryService _cloudinaryService;
    
    public LessonRepository(AppDbContext context, ICloudinaryService cloudinaryService)
    {
        _context = context;
        _cloudinaryService = cloudinaryService;
    }
    
    #region Create
    
    public async Task<Lesson> CreateAsync(Lesson lesson, IFormFile video, IFormFile? image)
    {
        var videoUploadResult = await _cloudinaryService.UploadVideoAsync(video, "lessons_videos");
        lesson.VideoUrl = videoUploadResult.SecureUrl.ToString();
        
        if (image is not null)
        {
            var imageUploadResult = await _cloudinaryService.UploadImageAsync(image, "lessons_images");
            lesson.ImageUrl = imageUploadResult.SecureUrl.ToString();
        }

        lesson.CreatedAt = DateTime.UtcNow;
        lesson.UpdatedAt = DateTime.UtcNow;

        _context.Lessons.Add(lesson);
        await _context.SaveChangesAsync();

        return lesson;
    }
    
    #endregion
    
    #region Read
    
    public async Task<IEnumerable<Lesson>> GetByCourseIdAsync(Guid courseId)
    {
        var lessons = await _context.Lessons
            .Where(l => l.CourseId == courseId)
            .ToListAsync();
        
        return lessons;
    }
    
    public async Task<Lesson?> GetByIdAsync(Guid id)
    {
        var lesson = await _context.Lessons.FindAsync(id);
        
        if (lesson == null)
        {
            throw new KeyNotFoundException("Invalid Lesson ID.");
        }
        
        return lesson;
    }
    
    #endregion
    
    #region Update
    
    public async Task<Lesson?> UpdateAsync(Lesson lesson, IFormFile video, IFormFile? image)
    {
        var videoUrl = lesson.VideoUrl;
        if (!string.IsNullOrEmpty(videoUrl))
        {
            await _cloudinaryService.DeleteFileAsync(videoUrl, ResourceType.Video);
        }
        var videoUploadResult = await _cloudinaryService.UploadVideoAsync(video, "lessons_videos");
        lesson.VideoUrl = videoUploadResult.SecureUrl.ToString();
        
        if (image is not null)
        {
            var imageUrl = lesson.ImageUrl;
            if (!string.IsNullOrEmpty(imageUrl))
            {
                await _cloudinaryService.DeleteFileAsync(imageUrl, ResourceType.Image);
            }
            var imageUploadResult = await _cloudinaryService.UploadVideoAsync(video, "lessons_images");
            lesson.ImageUrl = imageUploadResult.SecureUrl.ToString();
        }

        lesson.UpdatedAt = DateTime.UtcNow;

        _context.Lessons.Update(lesson);
        await _context.SaveChangesAsync();

        return lesson;
    }
    
    #endregion
    
    #region Delete
    
    public async Task DeleteAsync(Guid id)
    {
        var lesson = await _context.Lessons.FindAsync(id);
        if (lesson == null)
        {
            throw new KeyNotFoundException("Invalid Lesson ID.");
        }

        var videoUrl = lesson.VideoUrl;
        if (!string.IsNullOrEmpty(videoUrl))
        {
            await _cloudinaryService.DeleteFileAsync(videoUrl, ResourceType.Video);
        }
        
        var imageUrl = lesson.ImageUrl;
        if (!string.IsNullOrEmpty(imageUrl))
        {
            await _cloudinaryService.DeleteFileAsync(imageUrl, ResourceType.Image);
        }

        _context.Lessons.Remove(lesson);
        await _context.SaveChangesAsync();
    }
    
    #endregion
}