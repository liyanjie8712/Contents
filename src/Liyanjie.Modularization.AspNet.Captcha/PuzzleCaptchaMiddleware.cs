﻿using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;

using Liyanjie.Content.Models;

namespace Liyanjie.Modularization.AspNet
{
    /// <summary>
    /// 
    /// </summary>
    public class PuzzleCaptchaMiddleware
    {
        readonly CaptchaModuleOptions options;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        public PuzzleCaptchaMiddleware(CaptchaModuleOptions options)
        {
            this.options = options;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context)
        {
            if (options.RequestConstrainAsync != null)
                if (!await options.RequestConstrainAsync(context))
                    return;

            var query = context.Request.QueryString;
            var model = query.AllKeys
                .ToDictionary(_ => _.ToLower(), _ => query[_] as object)
                .BuildModel<PuzzleCaptchaModel>();
            var (blockIndexes, imageOrigin, imageBlocks) = await model.GenerateAsync(options);

            await options.SerializeToResponseAsync(context.Response, new
            {
                BlockIndexes = blockIndexes,
                ImageOrigin = imageOrigin.Encode(ImageFormat.Png),
                ImageBlocks = imageBlocks.Select(_ => _.Encode(ImageFormat.Png)),
            });

            context.Response.End();
        }
    }
}
