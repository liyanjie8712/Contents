﻿using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Extensions.DependencyInjection;

namespace Liyanjie.Modularization.AspNetCore
{
    /// <summary>
    /// 
    /// </summary>
    public static class ImageModuleTableExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="moduleTable"></param>
        /// <param name="configureOptions"></param>
        /// <param name="combineRouteTemplate"></param>
        /// <param name="concatenateRouteTemplate"></param>
        /// <param name="qrcodeRouteTemplate"></param>
        /// <param name="resizeRouteTemplates"></param>
        /// <returns></returns>
        public static ModularizationModuleTable AddImage(this ModularizationModuleTable moduleTable,
            Action<ImageModuleOptions> configureOptions,
            string combineRouteTemplate = "image/combine",
            string concatenateRouteTemplate = "image/concatenate",
            string cropRouteTemplate = "image/crop",
            string qrcodeRouteTemplate = "image/qrcode",
            params string[] resizeRouteTemplates)
        {
            moduleTable.Services.AddSingleton<ImageCombineMiddleware>();
            moduleTable.Services.AddSingleton<ImageConcatenateMiddleware>();
            moduleTable.Services.AddSingleton<ImageCropMiddleware>();
            moduleTable.Services.AddSingleton<ImageQRCodeMiddleware>();
            moduleTable.Services.AddSingleton<ImageResizeMiddleware>();

            var middlewares = new List<ModularizationModuleMiddleware>
            {
                new ModularizationModuleMiddleware
                {
                    HttpMethods = new[] { "POST" },
                    RouteTemplate = combineRouteTemplate,
                    HandlerType = typeof(ImageCombineMiddleware),
                },
                new ModularizationModuleMiddleware
                {
                    HttpMethods = new[] { "POST" },
                    RouteTemplate = concatenateRouteTemplate,
                    HandlerType =  typeof(ImageConcatenateMiddleware),
                }, 
                new ModularizationModuleMiddleware
                {
                    HttpMethods = new[] { "POST" },
                    RouteTemplate = cropRouteTemplate,
                    HandlerType =  typeof(ImageCropMiddleware),
                },
                new ModularizationModuleMiddleware
                {
                    HttpMethods = new[] { "GET" },
                    RouteTemplate = qrcodeRouteTemplate,
                    HandlerType = typeof(ImageQRCodeMiddleware),
                },
            };
            if (!resizeRouteTemplates.IsNullOrEmpty())
                foreach (var routeTemplate in resizeRouteTemplates)
                {
                    middlewares.Add(new ModularizationModuleMiddleware
                    {
                        HttpMethods = new[] { "GET" },
                        RouteTemplate = routeTemplate,
                        HandlerType = typeof(ImageResizeMiddleware),
                    });
                }

            moduleTable.AddModule("ImageModule", middlewares.ToArray(), configureOptions);

            return moduleTable;
        }
    }
}
