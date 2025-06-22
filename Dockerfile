# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Tell .NET to listen on port 80 instead of 8080
ENV ASPNETCORE_HTTP_PORTS=80

COPY --from=build /app/out .

ENTRYPOINT ["dotnet", "MyAPI.dll"]
