using System;
using System.IO;
using Kerwin.Directory.Web.Models;
using Kerwin.Directory.Web.Models.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace Kerwin.Directory.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddSession();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseSession();


            //提供static下载
            app.UseStaticFiles(new StaticFileOptions
            {
                ServeUnknownFileTypes = true,
                DefaultContentType = "application/octet-stream",
                FileProvider = new PhysicalFileProvider(ConfigSettings.RootDir),
                RequestPath = "/" + ConfigSettings.DownloadRequestVirtualDir,
                OnPrepareResponse = OnPrepareResponse
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private void OnPrepareResponse(StaticFileResponseContext staticFileResponseContext)
        {
            var isLogin = staticFileResponseContext.Context.Session.GetInt32("islogin") == 1;
            var filePath = staticFileResponseContext.File.PhysicalPath;

            var token = staticFileResponseContext.Context.Request.Query.ContainsKey("token")
                ? staticFileResponseContext.Context.Request.Query["token"].ToString()
                : "";
            var expiredTime = staticFileResponseContext.Context.Request.Query.ContainsKey("expiredtime")
                ? Convert.ToDateTime(staticFileResponseContext.Context.Request.Query["expiredtime"].ToString())
                : DateTime.Now.AddMinutes(-1);

            var virtualPath = filePath.ToVirtualPath(ConfigSettings.RootDir);

            if (expiredTime >= DateTime.Now && virtualPath.CheckDlToken(expiredTime, token))
            {
                //有权限下载
            }
            else if (!isLogin && !AppSettings.AllowAccessPath(filePath, out var needPwd))
            {
                //需要密码
                staticFileResponseContext.Context.Response.Redirect("/?dir=" + virtualPath);
            }
        }
    }
}
