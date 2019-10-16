﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Liyanjie.AspNetCore.Contents.Upload
{
    /// <summary>
    /// 
    /// </summary>
    public class UploadController : ControllerBase
    {
        readonly string webrootPath;
        readonly UploadOptions options;
        readonly ILogger<UploadController> logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hostingEnvironment"></param>
        /// <param name="options"></param>
        /// <param name="logger"></param>
        public UploadController(
            IHostingEnvironment hostingEnvironment,
            IOptions<UploadOptions> options,
            ILogger<UploadController> logger)
        {
            this.webrootPath = hostingEnvironment.WebRootPath;
            this.options = options?.Value ?? new UploadOptions();
            this.logger = logger;
        }

        /// <summary>
        /// 上传
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        [HttpPost()]
        public async Task<IActionResult> Post(string dir = "temps")
        {
            dir = dir.TrimStart(new[] { '/', '\\' }).Replace('/', Path.DirectorySeparatorChar);
            dir = Regex.Replace(dir, $@"\:|\*|\?|{'"'}|\<|\>|\||\s", string.Empty);

            var directory = Path.Combine(webrootPath, dir);

            logger?.LogInformation($"[FileUpload]=>Count:{Request.Form.Files.Count},Directory:{directory}");

            Directory.CreateDirectory(directory);

            var paths = new List<string>();
            foreach (var file in Request.Form.Files)
            {
                if (file.Length > options.AllowedMaximumSize)
                {
                    paths.Add($"File \"{file.FileName}\" is too large.");
                    continue;
                }

                var fileExtension = Path.GetExtension(file.FileName).ToLower();
                if (options.AllowedExtensions.IndexOf(fileExtension) < 0)
                {
                    paths.Add($"File \"{file.FileName}\" is not allowed.");
                    continue;
                }

                var fileName = $"{Guid.NewGuid().ToString("N")}{fileExtension}";

                var filePhysicalPath = Path.Combine(directory, fileName);
                using var fs = System.IO.File.Create(filePhysicalPath);
                await file.CopyToAsync(fs);
                await fs.FlushAsync();

                var filePath = Path.Combine(dir, fileName).Replace(Path.DirectorySeparatorChar, '/');
                if (options.ReturnAbsolutePath)
                    filePath = $"{Request.Scheme}://{Request.Host}/{filePath}";

                paths.Add(filePath);
            }

            return Ok(paths);
        }
    }
}