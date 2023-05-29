using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using BCXN.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BCXN.Services;
using BCXN.Repositories;
using AutoMapper;
using BCXN.Models;
using BCXN.Statics;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Security.Claims;
using BCXN.Repositories;
using BCXN.Services;
using Hangfire;
using Epayment.Services;
using Epayment.Repositories;
using BCXN.MapperProfile;
using BCXN.ViewModels;
using Newtonsoft.Json;

namespace BCXN
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
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.User.AllowedUserNameCharacters = "\\abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_";
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            services.AddMemoryCache();
            // services.AddHangfire(configuration => configuration
            //.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
            //.UseSimpleAssemblyNameTypeSerializer()
            //.UseRecommendedSerializerSettings()
            //.UseSqlServerStorage(Configuration.GetConnectionString("HangfireConnection"), new Hangfire.SqlServer.SqlServerStorageOptions
            //{
            //    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
            //    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
            //    QueuePollInterval = TimeSpan.Zero,
            //    UseRecommendedIsolationLevel = true,
            //    DisableGlobalLocks = true
            //}));

            // Add the processing server as IHostedService
            //services.AddHangfireServer(options =>
            //{
            //    options.Queues = new[] { "alpha", "beta", "default" };
            //});

            services.Configure<IISServerOptions>(options =>
            {
                options.MaxRequestBodySize = int.MaxValue;
            });
            services.Configure<IdentityOptions>(options =>
                {
                    // Password settings
                    options.Password.RequireDigit = false;
                    options.Password.RequiredLength = 8;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = true;
                    options.Password.RequireLowercase = false;
                });


            // ===== Add Jwt Authentication ========
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); // => remove default claims
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

                })
                .AddJwtBearer(cfg =>
                {
                    cfg.RequireHttpsMetadata = false;
                    cfg.SaveToken = true;
                    cfg.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = Configuration["JwtIssuer"],
                        ValidAudience = Configuration["JwtIssuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtKey"])),
                        ClockSkew = TimeSpan.Zero // remove delay of token when expire
                    };
                });
            services.AddAuthorization(config =>
                        {
                            // config.AddPolicy(Policies.YCBC_GET, Policies.YcbgGetPolicy());
                            config.AddPolicy(Policies.YCBC_CREATE, Policies.YcbcCreatePolicy());
                            config.AddPolicy(Policies.YCBC_UPDATE, Policies.YcbcUpdatePolicy());
                            config.AddPolicy(Policies.YCBC_XACNHAN, Policies.YcbcXacnhanPolicy());
                            config.AddPolicy(Policies.YCBC_DELETE, Policies.YcbcDeletePolicy());
                            config.AddPolicy(Policies.YCBC_DETAIL, Policies.YcbcDetailPolicy());
                            config.AddPolicy(Policies.YCBC_HISTORY, Policies.YcbcHistoryPolicy());
                            config.AddPolicy(Policies.YCBC_VIEW_THONGKE, Policies.YcbcThongkePolicy());
                        });
            services.AddControllersWithViews();
            services.AddRazorPages();

            //Add Automapper
            services.AddAutoMapper(typeof(AutoMappingProfile).Assembly);
            services.AddTransient<ILichSuXacNhanService, LichSuXacNhanService>();
            services.AddTransient<ILichSuXacNhanRepository, LichSuXacNhanRepository>();
            services.AddTransient<ILinhVucBaoCaoService, LinhVucBaoCaoService>();
            services.AddTransient<ILinhVucBaoCaoRepository, LinhVucBaoCaoRepository>();
            services.AddTransient<IBaoCaoService, BaoCaoService>();
            services.AddTransient<IBaoCaoRepository, BaoCaoRepository>();
            services.AddTransient<IYeuCauBaoCaoService, YeuCauBaoCaoService>();
            services.AddTransient<IYeuCauBaoCaoRepository, YeuCauBaoCaoRepository>();
            services.AddTransient<IDonViService, DonViService>();
            services.AddTransient<IDonViRepository, DonViRepository>();
            services.AddTransient<IChucNangService, ChucNangService>();
            services.AddTransient<IChucNangRepository, ChucNangRepository>();
            services.AddTransient<ITableauService, TableauService>();
            services.AddTransient<ITableAuRepository, TableAuRepository>();
            services.AddTransient<IGiayToService, GiayToService>();
            services.AddTransient<IGiayToRepository, GiayToRepository>();
            services.AddTransient<ILoaiHoSoService, LoaiHoSoService>();
            services.AddTransient<ILoaiHoSoRepository, LoaiHoSoRepository>();
            services.AddTransient<IHoSoThanhToanService, HoSoThanhToanService>();
            services.AddTransient<IHoSoThanhToanRepository, HoSoThanhToanRepository>();
            services.AddTransient<IGiayToLoaiHoSoService, GiayToLoaiHoSoService>();
            services.AddTransient<IGiayToLoaiHoSoRepository, GiayToLoaiHoSoRepository>();
            services.AddTransient<INguoiHuongThuService, NguoiHuongThuService>();
            services.AddTransient<INguoiHuongThuRepository, NguoiHuongThuRepository>();
            services.AddTransient<ILichSuHoSoTTService, LichSuHoSoTTService>();
            services.AddTransient<ILichSuHoSoTTRepository, LichSuHoSoTTRepository>();
            services.AddTransient<INganHangService, NganHangService>();
            services.AddTransient<INganHangRepository, NganHangRepository>();
            services.AddTransient<ICauHinhNganHangService, CauHinhNganHangService>();
            services.AddTransient<ICauHinhNganHangRepository, CauHinhNganHangRepository>();
            services.AddTransient<IChiTietGiayToHSTTService, ChiTietGiayToHSTTService>();
            services.AddTransient<IChiTietGiayToHSTTRepository, ChiTietGiayToHSTTRepository>();
            services.AddTransient<IYeuCauChiTienRepository, YeuCauChiTienRepository>();
            services.AddTransient<ILuongPheDuyetService , LuongPheDuyetService>();
            services.AddTransient<ILuongPheDuyetRepository, LuongPheDuyetRepository>();
            services.AddTransient<ILichSuChiTietGiayToService, LichSuChiTietGiayToService>();
            services.AddTransient<ILichSuChiTietGiayToRepository, LichSuChiTietGiayToRepository>();
            services.AddTransient<IChiTietHachToanRepository, ChiTietHachToanRepository>();
            services.AddTransient<ITrangThaiHoSoService, TrangThaiHoSoService>();
            services.AddTransient<ITrangThaiHoSoRepository, TrangThaiHoSoRepository>();
            services.AddTransient<IChungTuRepository, ChungTuRepository>();
            services.AddTransient<IChungTuService, ChungTuService>();
            services.AddTransient<IYeuCauChiTienService, YeuCauChiTienService>();
            services.AddTransient<IYeuCauChiTienRepository, YeuCauChiTienRepository>();
            services.AddTransient<IMaHoaPGDService, MaHoaPGDService>();
            services.AddTransient<IGeneratePDFService, GeneratePDFService>();
            services.AddTransient<IKySoTaiLieuService, KySoTaiLieuService>();
            services.AddTransient<IKySoTaiLieuRepository, KySoTaiLieuRepository>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IAccountDofficeService, AccountDofficeService>();
            services.AddTransient<IVCBService, VCBService>();
            services.AddTransient<IUploadToFtpService, UploadToFtpService>();
            services.AddTransient<IBIDVService, BIDVService>();
            services.AddTransient<IVTBService, VTBService>();
            services.AddTransient<IUtilsService, UtilsService>();

            //quy trình phê duyệt động
            services.AddTransient<QuyTrinhPheDuyetRepository>();
            services.AddTransient<BuocPheDuyetRepository>();
            services.AddTransient<QuaTrinhPheDuyetRepository>();
            services.AddTransient<ThaoTacBuocPheDuyetRepository>();
            services.AddTransient<QuyTrinhPheDuyetService>();

            services.AddTransient<ITinhThanhPhoService, TinhThanhPhoService>();
            services.AddTransient<ITinhThanhPhoRepository, TinhThanhPhoRepository>();

            services.AddTransient<IQuocGiaService, QuocGiaService>();
            services.AddTransient<IQuocGiaRepository, QuocGiaRepository>();
        }
        private static void UpdateDatabase(IServiceProvider serviceProvider)
        {
            using (var context = serviceProvider.GetService<ApplicationDbContext>())
            {
                context.Database.Migrate();
            }
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider, ITableauService tab)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            //Autocheck Migration to update database
            UpdateDatabase(serviceProvider);
            // app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot/FileUpload")),
                RequestPath = new PathString("/wwwroot/FileUpload")
            });
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot/image")),
                RequestPath = new PathString("/wwwroot/image")
            });
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot/fileTableAu")),
                RequestPath = new PathString("/wwwroot/fileTableAu")
            });
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot/template")),
                RequestPath = new PathString("/wwwroot/template")
            });
            //app.UseHangfireDashboard();
            // RecurringJob.RemoveIfExists("easyjob");
            //RecurringJob.AddOrUpdate("easyjob", () => tab.DownloadAutoBaoCaoTableau(), "0 0 1,15 * *");//nua dem vao ngay 15 va 01 hang thang tai file ve
            //RecurringJob.AddOrUpdate("easyjob", () => tab.DownloadAutoBaoCaoTableau(), "*/59 * * *");//60 tai 1 lan
            //RecurringJob.AddOrUpdate("easyjob", () => tab.DownloadAutoBaoCaoTableau(), "0 0 * * * *");//hang ngay tai 1 lan
            //backgroundJobs.Enqueue(() => Console.WriteLine("Hello world from Hangfire!"));
            // Aspose.Slides.License license = new Aspose.Slides.License();
            // license.SetLicense("Aspose.Slides.lic");
            app.UseRouting();

            app.Use(async (context, next) =>
            {
                await next();

                if (context.Response.StatusCode == (int)System.Net.HttpStatusCode.Forbidden)
                {
                    Response r = new Response(message:"Bạn không có quyền sử dụng api này", data:null, errorcode:"001", success:false);
                    context.Response.ContentType = "text/plain; charset=utf-8";
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(r));
                }
            });

            app.UseCors(x => x
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => true)
                .AllowCredentials());
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                //endpoints.MapHangfireDashboard();
            });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllerRoute(
                 name: "default",
                 pattern: "{controller=Product}/{action=Index}/{id?}");
            });

        }
    }
}
