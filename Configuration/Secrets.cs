﻿using System.Security.Policy;
using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Ipe.Domain.Models;

namespace Ipe.Configuration;
public static class Secrets
{
    public static void Configure(WebApplicationBuilder builder)
    {
        var VaultUri = Environment.GetEnvironmentVariable("VAULT_URL");

        if (VaultUri == null)
            return;

        var options = new SecretClientOptions()
        {
            Retry = {
                Delay = TimeSpan.FromSeconds(2),
                MaxDelay = TimeSpan.FromSeconds(16),
                MaxRetries = 5,
                Mode = RetryMode.Exponential
            }
        };

        var client = new SecretClient(new Uri(VaultUri), new DefaultAzureCredential(), options);

        var DatabaseConnectionString = client.GetSecret("Trees-Databases-Cosmos-ConnectionString").Value.Value;
        var DatabaseName = client.GetSecret("Trees-Databases-Cosmos-Ipe-Name").Value.Value;

        var StorageConnectionString = client.GetSecret("Trees-Storage-ConnectionString").Value.Value;
        var StorageContainerName = client.GetSecret("Trees-Storage-Container-Name").Value.Value;

        var JwtKey = client.GetSecret("Trees-Ipe-Jwt-Key").Value.Value;

        var EmailApiKey = client.GetSecret("Trees-Email-ApiKey").Value.Value;
        var EmailFromEmail = client.GetSecret("Trees-Email-From-Email").Value.Value;
        var EmailFromName = client.GetSecret("Trees-Email-From-Name").Value.Value;
        var EmailTemplateWelcome = client.GetSecret("Trees-Email-Templates-Welcome").Value.Value;
        var EmailTemplatePasswordReset = client.GetSecret("Trees-Email-Templates-PasswordReset").Value.Value;
        var EmailTemplatePlantSuccess = client.GetSecret("Trees-Email-Templates-PlantSuccess").Value.Value;
        var EmailTemplateFirstPlant = client.GetSecret("Trees-Email-Templates-FirstPlant").Value.Value;
        var EmailUrlsWelcome = client.GetSecret("Trees-Email-Urls-Welcome").Value.Value;
        var EmailUrlsPasswordReset = client.GetSecret("Trees-Email-Urls-PasswordReset").Value.Value;
        var EmailUrlsMyForest = client.GetSecret("Trees-Email-Urls-My-Forest").Value.Value;

        var PaymentServiceUrl = client.GetSecret("Trees-Bonsai-Url").Value.Value;
        var PaymentToken = client.GetSecret("Trees-Ipe-Bonsai-X-Seed-Key").Value.Value;
        var PortalConfiguration = client.GetSecret("Trees-Portal-Configuration").Value.Value;

        var GoogleLoginUrl = client.GetSecret("Trees-Google-Url").Value.Value;
        var GoogleLoginClientId = client.GetSecret("Trees-Google-Client-Id").Value.Value;



        builder.Configuration["Database:ConnectionString"] = DatabaseConnectionString;
        builder.Configuration["Database:Name"] = DatabaseName;

        builder.Configuration["Storage:ConnectionString"] = StorageConnectionString;
        builder.Configuration["Storage:TreeContainerName"] = StorageContainerName;

        builder.Configuration["Jwt:Key"] = JwtKey;

        builder.Configuration["Email:ApiKey"] = EmailApiKey;
        builder.Configuration["Email:FromEmail"] = EmailFromEmail;
        builder.Configuration["Email:FromName"] = EmailFromName;
        builder.Configuration["Email:Templates:WelcomeEmail"] = EmailTemplateWelcome;
        builder.Configuration["Email:Templates:PasswordResetEmail"] = EmailTemplatePasswordReset;
        builder.Configuration["Email:Templates:PlantSuccessEmail"] = EmailTemplatePlantSuccess;
        builder.Configuration["Email:Templates:FirstPlantEmail"] = EmailTemplateFirstPlant;
        builder.Configuration["Email:Urls:WelcomeEmail"] = EmailUrlsWelcome;
        builder.Configuration["Email:Urls:PasswordResetEmail"] = EmailUrlsPasswordReset;
        builder.Configuration["Email:Urls:MyForest"] = EmailUrlsMyForest;

        builder.Configuration["Payment:BaseUrl"] = PaymentServiceUrl;
        builder.Configuration["Payment:Token"] = PaymentToken;
        builder.Configuration["Portal:Configuration"] = PortalConfiguration;

        builder.Configuration["Google:ClientID"] = GoogleLoginClientId;
        builder.Configuration["Google:Login:Url"] = GoogleLoginUrl;
    }
}