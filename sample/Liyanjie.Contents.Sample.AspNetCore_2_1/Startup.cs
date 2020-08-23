using Liyanjie.Modularization.AspNetCore;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Liyanjie.Contents.Sample.AspNetCore_2_1
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            Env = env;
        }

        public IHostingEnvironment Env { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddModularization()
                .AddExplore(options =>
                {
                    options.RootDirectory = Env.WebRootPath;
                    options.Directories = new[] { "images", "temps" };
                    options.SerializeToResponseAsync = serializeToResponse;
                    //options.ReturnAbsolutePath = true;
                })
                .AddUpload(options =>
                {
                    options.RootDirectory = Env.WebRootPath;
                    options.SerializeToResponseAsync = serializeToResponse;
                }, "fileupload")
                .AddImage(options =>
                {
                    options.RootDirectory = Env.WebRootPath;
                    options.DeserializeFromRequestAsync = deserializeFromRequest;
                    options.SerializeToResponseAsync = serializeToResponse;
                }, resizeRouteTemplates: new[]
                .AddUpload(options => options.RootDirectory = Env.WebRootPath, "fileupload")
                .AddImage(options => options.RootDirectory = Env.WebRootPath, resizeRouteTemplates: new[]
                {
                    "images/{filename}.{size}.{extension}",
                    "images/{directory}/{filename}.{size}.{extension}"
                });

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();

            app.UseStaticFiles();
            app.UseModularization();
            app.UseMvcWithDefaultRoute();
        }
    }
}
