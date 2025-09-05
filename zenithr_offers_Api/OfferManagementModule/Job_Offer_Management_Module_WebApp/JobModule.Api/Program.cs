
using JobModule.Domain.RepoInterfaces;
using JobModule.Repository.General;
using JobModule.Repository.Repositories;
using JobModule.Services.CommonServices;
using JobModule.Services.Dtos;
using JobModule.Services.ServiceInterface;
using JobModule.Services.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using QuestPDF.Infrastructure;
using System;

namespace JobModule.Api
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			builder.Services.AddDbContext<ApplicationDbContext>(options =>
			options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

			builder.Services.AddScoped<IJobOfferRepository, JobOfferRepository>();
			builder.Services.AddScoped<IJobOfferService, JobOfferService>();

			builder.Services.Configure<PdfStorageOptions>(builder.Configuration.GetSection("PdfStorage"));
			builder.Services.AddScoped<IPdfGeneratorService, PDFGenerator>();

			builder.Services.Configure<DocuSignSettings>(builder.Configuration.GetSection("DocuSign"));
			builder.Services.AddScoped<IDocuSignAuthService, DocuSignAuthService>();

			builder.Services.AddControllers();
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            var app = builder.Build();

			QuestPDF.Settings.License = LicenseType.Community;

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}
			app.UseStaticFiles(new StaticFileOptions
			{
				FileProvider = new PhysicalFileProvider(
				Path.Combine(Directory.GetCurrentDirectory(), "OfferLetterStaticFile")),
						RequestPath = "/offerletters"
			}
			);

			app.UseHttpsRedirection();

			app.UseAuthorization();

            app.UseCors("AllowAll");

            app.MapControllers();

			app.Run();
		}
	}
}
