using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

namespace C_ServiceLayer.Abstractions;

public interface IUploadImgService
{
    Task<ImageUploadResult?> UploadImg(IFormFile file);
    Task<DeletionResult?> DeleteImg(string imgId);
    
}