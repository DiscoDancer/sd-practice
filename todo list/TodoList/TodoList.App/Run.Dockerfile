# Use .NET 9.0 ASP.NET runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime

# Set working directory
WORKDIR /app

# Copy published app from host machine into the container
COPY ./publish .

# Set environment to use Docker-specific appsettings
ENV ASPNETCORE_ENVIRONMENT=Docker

# Expose port (make sure this matches what's in launchSettings.json or appsettings)
EXPOSE 8080

# Start the application
ENTRYPOINT ["dotnet", "TodoList.App.dll"]