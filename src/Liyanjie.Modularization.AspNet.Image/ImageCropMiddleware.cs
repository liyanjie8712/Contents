﻿using System;
using System.IO;
using System.Threading.Tasks;
using System.Web;

using Liyanjie.Content.Models;

using Microsoft.Extensions.Options;

namespace Liyanjie.Modularization.AspNet
{
    /// <summary>
    /// 
    /// </summary>
    public class ImageCropMiddleware
    {
        readonly IOptions<ImageModuleOptions> options;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        public ImageCropMiddleware(IOptions<ImageModuleOptions> options)
        {
            this.options = options ?? throw new ArgumentNullException(nameof(options));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public async Task InvokeAsync(HttpContext context)
        {
            var options = this.options.Value;

            if (options.RequestConstrainAsync is not null)
                if (!await options.RequestConstrainAsync.Invoke(context))
                    return;

            var request = context.Request;

            var model = (await options.DeserializeFromRequestAsync(request, typeof(ImageCropModel))) as ImageCropModel;
            if (model is not null)
            {
                var imagePath = (await model?.CropAsync(options))?.Replace(Path.DirectorySeparatorChar, '/');
                if (options.ReturnAbsolutePath)
                {
                    var port = request.Url.IsDefaultPort ? null : $":{request.Url.Port}";
                    imagePath = $"{request.Url.Scheme}://{request.Url.Host}{port}/{imagePath}";
                }

                await options.SerializeToResponseAsync(context.Response, imagePath);

                context.Response.End();
            }
        }
    }
}
