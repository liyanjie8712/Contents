﻿using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Liyanjie.Contents.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Liyanjie.Modularization.AspNetCore
{
    /// <summary>
    /// 
    /// </summary>
    public class SliderCodeMiddleware : IMiddleware
    {
        readonly VerificationCodeModuleOptions options;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        public SliderCodeMiddleware(IOptions<VerificationCodeModuleOptions> options)
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
            if (options.RequestConstrainAsync != null)
                if (!await options.RequestConstrainAsync(context))
                    return;

            var model = context.Request.Query
                .ToDictionary(_ => _.Key.ToLower(), _ => _.Value.FirstOrDefault() as object)
                .BuildModel<SliderCodeModel>();
            var (blockPoint, originImage, boardImage, blockImage) = await model.GenerateAsync(options);

            await options.SerializeToResponseAsync(context.Response, new
            {
                BlockPoint = blockPoint,
                OriginImage = originImage.Encode(ImageFormat.Png),
                BoardImage = boardImage.Encode(ImageFormat.Png),
                BlockImage = blockImage.Encode(ImageFormat.Png),
            });
        }
    }
}
