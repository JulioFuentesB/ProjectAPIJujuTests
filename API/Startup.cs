using AutoMapper;
using Business.Common.Interfaces.Services;
using Business.Common.Mappings;
using Business.Services;
using DataAccess.Data;
using DataAccess.Interfaces;
using DataAccess.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;

namespace ProjectAPI.API
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


            //Agregar cadena de conexion al contexto
            services.AddDbContext<JujuTestContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("Development")));

            //Servicios                      
            services.AddScoped<JujuTestContext, JujuTestContext>();
            //services.AddScoped<BaseService<Customer>, BaseService<Customer>>();
            //services.AddScoped<BaseModel<Customer>, BaseModel<Customer>>();
            //services.AddScoped<BaseService<Post>, BaseService<Post>>();            
            //services.AddScoped<BaseModel<Post>, BaseModel<Post>>();

            services.AddScoped<JujuTestContext, JujuTestContext>();

            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            // Configuración de Repository
            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            // Configuración de Servicios
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IPostService, PostService>();

            // Agrega AutoMapper al contenedor de dependencias
            services.AddAutoMapper(typeof(MappingProfile).Assembly);
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);


            // ======== CONFIGURACIÓN DE SWAGGER =========
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "TestAPI", Version = "v1" });
            });
            services.AddHttpContextAccessor();
            services.AddSession();
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // ======== CONFIGURACIÓN DE SWAGGER =========
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                string swaggerJsonBasePath = string.IsNullOrWhiteSpace(c.RoutePrefix) ? "." : "..";
                c.SwaggerEndpoint($"{swaggerJsonBasePath}/swagger/v1/swagger.json", "TestAPI v1");
            });

            app.UseCors(options => options
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseAuthentication();


            app.UseSession();
            app.UseMvc();
        }
    }
}