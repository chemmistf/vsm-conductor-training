FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY VSMTraining.API/VSMTraining.API.csproj VSMTraining.API/
COPY VSMTraining.Application/VSMTraining.Application.csproj VSMTraining.Application/
COPY VSMTraining.Domain/VSMTraining.Domain.csproj VSMTraining.Domain/
COPY VSMTraining.Infrastructure/VSMTraining.Infrastructure.csproj VSMTraining.Infrastructure/
RUN dotnet restore VSMTraining.API/VSMTraining.API.csproj

COPY . .
RUN dotnet publish VSMTraining.API/VSMTraining.API.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "VSMTraining.API.dll"]