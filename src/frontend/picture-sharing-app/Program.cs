using picture_sharing.Services;

namespace picture_sharing
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddHttpClient();
            // Replace <YourKeyVaultName> with your Key Vault name
            var keyVaultUri = $"https://kvdevpictureshare.vault.azure.net";
            builder.Services.AddSingleton(new KeyVaultService(keyVaultUri));

            // Allow cors
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                                  policy =>
                                  {
                                      policy.WithOrigins(
                                          "https://confirmationfileshare.blob.core.windows.net/",
                                          "http://localhost:7071/",
                                          "https://localhost:7071/");
                                  });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseCors(MyAllowSpecificOrigins);

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}