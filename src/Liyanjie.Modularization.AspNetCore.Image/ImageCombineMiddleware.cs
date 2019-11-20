﻿using System;
using System.IO;
using System.Threading.Tasks;

using Liyanjie.Contents.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Liyanjie.Modularization.AspNetCore
{
    /// <summary>
    /// 
    /// </summary>
    public class ImageCombineMiddleware : IMiddleware
    {
        readonly ImageModuleOptions options;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        public ImageCombineMiddleware(IOptions<ImageModuleOptions> options)
        {
            this.options = options.Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var request = context.Request;
            var model = (await options.DeserializeFromRequestAsync(request, typeof(ImageCombineModel))) as ImageCombineModel;
            var imagePath = (await model?.CombineAsync(options))?.Replace(Path.DirectorySeparatorChar, '/');

            if (options.ReturnAbsolutePath)
                imagePath = $"//{request.Host}/{imagePath}";

            await options.SerializeToResponseAsync(context.Response, imagePath);
        }
    }
}