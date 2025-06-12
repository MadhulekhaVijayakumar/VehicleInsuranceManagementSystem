
using InsuranceAPI.Context;
using InsuranceAPI.Interfaces;
using InsuranceAPI.Misc;
using InsuranceAPI.Models;
using InsuranceAPI.Repositories;
using InsuranceAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using QuestPDF.Infrastructure;
using System.Security.Claims;
using System.Text;

namespace InsuranceAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(option =>
            {
                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer ",
                });
                option.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            builder.Services.AddHttpContextAccessor();

            builder.Logging.AddLog4Net();

            #region CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder =>
                    {
                        builder.WithOrigins("http://localhost:5173")
                               .AllowAnyMethod()
                               .AllowAnyHeader();
                    });
            });
            #endregion

            #region Context
            builder.Services.AddDbContext<InsuranceManagementContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });
            #endregion
            #region Repositories
            builder.Services.AddScoped<IRepository<int,Client>,ClientRepository>();
            builder.Services.AddScoped<IRepository<string,User>,UserRepository>();
            builder.Services.AddScoped<IRepository<int,Admin>,AdminRepository>();
            builder.Services.AddScoped<IRepository<int,Proposal>,ProposalRepository>();
            builder.Services.AddScoped<IRepository<int,Vehicle>,VehicleRepository>();
            builder.Services.AddScoped<IRepository<int,InsuranceDetails>,InsuranceDetailsRepository>();
            builder.Services.AddScoped<IRepository<string, Insurance>, InsuranceRepository>();
            builder.Services.AddScoped<IRepository<int,Payment>,PaymentRepository>();
            builder.Services.AddScoped<IRepository<int, Document>, DocumentRepository>();
            builder.Services.AddScoped<IRepository<int, InsuranceClaim>, InsuranceClaimRepository>();



            #endregion

            #region Mapper
            builder.Services.AddAutoMapper(typeof(Client));
            #endregion
            #region Services
            builder.Services.AddScoped<IClientService,ClientService>();
            builder.Services.AddScoped<IAdminService,AdminService>();
            builder.Services.AddScoped<IAuthenticationService,AuthenticationService>();
            builder.Services.AddScoped<ITokenService,TokenService>();
            builder.Services.AddScoped<IVehicleService,VehicleService>();
            builder.Services.AddScoped<IProposalService,ProposalService>();
            builder.Services.AddScoped<IPremiumCalculatorService, PremiumCalculatorService>();
            builder.Services.AddScoped<IInsuranceService, InsuranceService>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();
            builder.Services.AddScoped<IDocumentService, DocumentService>();
            builder.Services.AddScoped<IInsuranceClaimService, InsuranceClaimService>();
            builder.Services.AddScoped<InsurancePolicyNumberGenerator>();
            builder.Services.AddScoped<IPolicyDocumentService, PolicyDocumentService>();
            builder.Services.AddScoped<IAdminDashboardService, AdminDashboardService>();
            builder.Services.AddScoped<IActivityLogService, ActivityLogService>();
            builder.Services.AddScoped<IQuotePdfGenerator, QuotePdfGenerator>();
            builder.Services.AddScoped<IQuoteService, QuoteService>();

            #endregion

            #region Authentication
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Keys:JwtToken"])),
                        RoleClaimType=ClaimTypes.Role
                    };
                });
            #endregion
            QuestPDF.Settings.License = LicenseType.Community;


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseCors("AllowAll");

            app.UseAuthentication();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
