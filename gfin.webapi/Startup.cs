using System.Text;
using gfin.webapi.Api.Helpper;
using gfin.webapi.Dados;
using gfin.webapi.Negocio.Listeners;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace gfin.webapi
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            
            var dbGFinConnection = Configuration.GetConnectionString("GFinContext");
            services.AddDbContextPool<GFinContext>(options => options.UseSqlServer(dbGFinConnection));
            services.AddScoped<IClienteListener, ClienteConfig>();
            //services.AddSingleton<IConfiguration>(Configuration);
            services.AddSwaggerGen(swagger =>
            {
                swagger .SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Gerenciador Financeiro", 
                    Version = "v1",
                    Description = "APIs de backend da aplicação do Gerenciador Financeiro."
                });
                swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()  
                {  
                    Name = "Authorization",  
                    Type = SecuritySchemeType.ApiKey,  
                    Scheme = "Bearer",  
                    BearerFormat = "JWT",  
                    In = ParameterLocation.Header,  
                    Description = "JWT Authorization no header utilizando o Bearer. \r\n\r\n Informe 'Bearer'+{espaço}+{token de autenticação do login}.\r\n\r\nExemplo: \"Bearer {tokeJWT}\"",  
                }); 
                swagger.AddSecurityRequirement(new OpenApiSecurityRequirement  
                {  
                    {  
                        new OpenApiSecurityScheme  
                        {  
                            Reference = new OpenApiReference  
                            {  
                                Type = ReferenceType.SecurityScheme,  
                                Id = "Bearer"  
                            }  
                        },  
                        new string[] {}  
                    }  
                });
            });
            
            services.AddAuthentication(option =>  
            {  
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;  
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;  
  
            }).AddJwtBearer(options =>  
            {  
                options.TokenValidationParameters = new TokenValidationParameters  
                {  
                    ValidateIssuer = true,  
                    ValidateAudience = true,  
                    ValidateLifetime = false,  
                    ValidateIssuerSigningKey = true,  
                    ValidIssuer = Configuration["Jwt:Issuer"],  
                    ValidAudience = Configuration["Jwt:Issuer"],  
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))  
                };  
            });
            //services.AddSingleton<IValidationAttributeAdapterProvider,CustomValidationAttributeAdapterProvider>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Api.GerenciadorFinanceiro.V1");
                });
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}