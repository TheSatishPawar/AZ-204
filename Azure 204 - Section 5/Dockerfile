FROM mcr.microsoft.com/dotnet/aspnet:6.0
ENV ASPNETCORE_ENVIRONMENT="Production"
WORKDIR /app
EXPOSE 80
COPY  . .
ENTRYPOINT ["dotnet", "sqlapp.dll"]