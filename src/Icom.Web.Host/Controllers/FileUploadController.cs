using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Icom.Common;
using Icom.Controllers;
using Icom.Helpers;
using Icom.Web.Host.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Icom.Web.Host.Controllers
{
    [Route("api/[controller]")]
    public class FileUploadController : IcomControllerBase
    {
        private readonly IConfiguration _configuration;
        private CloudinarySettings _cloudinarySettings;
        private Cloudinary _cloudinary;

        //private readonly string _studentsFolderId;
        //private readonly GoogleDriveService _googleDriveService;
        //private readonly IConfiguration _configuration;
        public FileUploadController(GoogleDriveService googleDriveService, IConfiguration configuration)
        {
            _configuration = configuration;
            _cloudinarySettings = _configuration.GetSection("CloudinarySettings").Get<CloudinarySettings>();
            var account = new Account(_cloudinarySettings.CloudName, _cloudinarySettings.ApiKey, _cloudinarySettings.ApiSecret);
            _cloudinary = new Cloudinary(account);
            //_googleDriveService = googleDriveService;
            //_studentsFolderId = _configuration["IComStorage:StudentsFolderId"];

        }
        
        //Google Drive
        //[HttpPost("upload-image")]
        //public async Task<IActionResult> UploadImage()
        //{
        //    var file = Request.Form.Files[0];
        //    if (file == null || file.Length == 0)
        //    {
        //        return BadRequest("No file selected.");
        //    }

        //    var driveService =
        //        await _googleDriveService.GetDriveServiceAsync();

        //    var fileMetadata = new Google.Apis.Drive.v3.Data.File
        //    {
        //        Name = file.FileName,
        //        MimeType = file.ContentType,
        //        Parents = new[] { _studentsFolderId }
        //    };

        //    var stream = file.OpenReadStream();

        //    var request = driveService.Files.Create(
        //        fileMetadata,
        //        stream,
        //        file.ContentType);

        //    request.Fields =
        //        "id,name,mimeType,size,webViewLink";

        //    var result = await request.UploadAsync();

        //    if (result.Status !=
        //        Google.Apis.Upload.UploadStatus.Completed)
        //    {
        //        return BadRequest(new
        //        {
        //            success = false,
        //            error = result.Exception?.Message
        //        });
        //    }

        //    var uploadedFile = request.ResponseBody;

        //    return Ok(new
        //    {
        //        success = true,
        //        fileId = uploadedFile.Id,
        //        fileName = uploadedFile.Name,
        //        mimeType = uploadedFile.MimeType,
        //        size = uploadedFile.Size,
        //        url = uploadedFile.WebViewLink
        //    });
        //}

        [HttpPost("upload-file")]
        public async Task<MediaInfoDto> FileUpload()
        {
            var file = Request.Form.Files[0];
            //var uploadResult = new ImageUploadResult();
            try
            {
                if (file.Length > 0)
                {
                    if (file.Name == "image")
                    {
                        using (var stream = file.OpenReadStream())
                        {
                            var uploadParams = new ImageUploadParams()
                            {
                                Folder = "IcomStudents",
                                File = new FileDescription(file.Name, stream)
                            };
                            var uploadResult = _cloudinary.Upload(uploadParams);
                            return new MediaInfoDto()
                            {
                                FullFilePath = uploadResult.SecureUrl.ToString()
                            };
                        }
                    }
                    if (file.Name == "pdf")
                    {
                        using (var stream = file.OpenReadStream())
                        {
                            var uploadParams = new ImageUploadParams()
                            {
                                Folder = "JobApplications",
                                File = new FileDescription(file.Name, stream)
                            };
                            var uploadResult = _cloudinary.Upload(uploadParams);
                            return new MediaInfoDto()
                            {
                                FullFilePath = uploadResult.SecureUrl.ToString()
                            };
                        }
                    }
                    else
                    {
                        using (var stream = file.OpenReadStream())
                        {
                            var uploadParams = new VideoUploadParams()
                            {
                                Folder = "",
                                File = new FileDescription(file.Name, stream)
                            };
                            var uploadResult = _cloudinary.UploadLarge(uploadParams);
                            return new MediaInfoDto()
                            {
                                FullFilePath = uploadResult.SecureUrl.ToString()
                            };
                        }
                    }


                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return new MediaInfoDto()
            {
                FullFilePath = ""
            };
        }

        //GoogleDrive
        //[HttpGet("Image/{fileId}")]
        //public async Task<IActionResult> GetImage(string fileId)
        //{
        //    try
        //    {
        //        var driveService =
        //        await _googleDriveService.GetDriveServiceAsync();

        //        var metadataRequest =
        //            driveService.Files.Get(fileId);

        //        metadataRequest.Fields =
        //            "id,name,mimeType";

        //        var metadata =
        //            await metadataRequest.ExecuteAsync();

        //        var memoryStream = new MemoryStream();

        //        var downloadRequest =
        //            driveService.Files.Get(fileId);

        //        await downloadRequest.DownloadAsync(
        //            memoryStream);

        //        return File(
        //            memoryStream.ToArray(),
        //            metadata.MimeType,
        //            metadata.Name);
        //    }
        //    catch(Exception ex)
        //    {
        //        throw ex;
        //    }
            
        //}

        
    }
}
