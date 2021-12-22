using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPBack.Data;
using TPBack.Helpers;
using TPBack.Mapper;
using TPBack.Models;
using TPBack.Repository;
using TPBack.Repository.IRepository;

namespace TPBack
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
            services.AddControllers();

            //register ApplicationDbContext.cs and connectionString
            services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("constr"))
            ) ;

            //add service of repository pattern
            services.AddScoped<IBookRepository, BookRepository>();
            services.AddScoped<IAuthServices, AuthServices>();

            // the service of mapping 
            services.AddAutoMapper(typeof(TPMappings));

            // the service of swagger
            services.AddSwaggerGen(option =>
            {
            option.SwaggerDoc("TPBackOpenApi",new Microsoft.OpenApi.Models.OpenApiInfo(){
                Title ="TP Apis",
                Version="1",
                Contact = new Microsoft.OpenApi.Models.OpenApiContact()
                 {
                     Email = "Ahmed.Hamdy2061995@gmail.com",
                     Name = "Ahmed Hamdy"
                 },
                Description = "TP Api Documentation"

            });
            }
            );

            // to map between class JWT in Helpers  and the section JWT in appsettings.json
            services.Configure<JWT>(Configuration.GetSection("JWT"));

            // to tell that the program will use identity
            services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();

            // to register JWT service
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
              .AddJwtBearer(op =>
              {
                  op.RequireHttpsMetadata = false;
                  op.SaveToken = false;
                  op.TokenValidationParameters = new TokenValidationParameters
                  {
                      ValidateIssuerSigningKey = true,
                      ValidateIssuer = true,
                      ValidateAudience = true,
                      ValidateLifetime = true,
                      ValidIssuer = Configuration["JWT:Issuer"],
                      ValidAudience = Configuration["JWT:Audience"],
                      IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Key"]))
                  };
              });

            // to all accessing data for outside origin (vue js)
            services.AddCors(options =>
            {
                options.AddPolicy("foo",
                builder =>
                {
                    // Not a permanent solution, but just trying to isolate the problem
                   builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                    
                });
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(option =>
            {
                option.SwaggerEndpoint("/swagger/TPBackOpenApi/swagger.json", "TPBack Api");
                option.RoutePrefix="";
            });

            // Use the CORS policy
            app.UseCors("foo");

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "Photos")),
                RequestPath = "/Photos"
            });
        }
    }
}
