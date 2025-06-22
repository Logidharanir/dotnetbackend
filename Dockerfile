# -------- Stage 1 : Build ---------------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# copy csproj and restore
COPY *.csproj ./
RUN dotnet restore

# copy everything and publish
COPY . ./
RUN dotnet publish -c Release -o out

# -------- Stage 2 : Runtime -------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# make ASP.NET listen on port 80 (Render routes ⇢ 80)
ENV ASPNETCORE_HTTP_PORTS=80

# copy compiled output from the previous stage
COPY --from=build /app/out .

# start the API
ENTRYPOINT ["dotnet", "MyAPI.dll"]   # change name if your DLL differs
