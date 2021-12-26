using Core.Domain.ServicesAbstractions;
using Core.Mappers;
using Core.RepositoryAbstractions;
using Core.Services;
using Infrastructure.DbContext;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;


namespace Reports.Presentation
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Reports.WebUI", Version = "v1" });
            });

            services.AddDbContext<ReportsDatabaseContext>(opt =>
            {
                opt.UseSqlServer(Configuration.GetConnectionString("MyServer"));
            });

            services.AddScoped<IEmployeeMapper, MyEmployeeToDtoMapper>();
            services.AddScoped<ITaskChangeMapper, MyTaskChangeToDtoMapper>();
            services.AddScoped<IJobTaskMapper, MyJobTaskToDtoMapper>();
            services.AddScoped<IReportMapper, MyReportToDtoMapper>();
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<IJobTaskRepository, JobTaskRepository>();
            services.AddScoped<ITaskChangesRepository, TaskChangesRepository>();
            services.AddScoped<IJobTaskService, JobTaskService>();
            services.AddScoped<IReportRepository, ReportRepository>();
            services.AddScoped<IReportService, ReportService>();

            services.AddControllers();
        }
        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Reports.WebUI v1"));
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}