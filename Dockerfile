# Fase de compilação
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app
COPY . ./
RUN dotnet restore
RUN dotnet publish -c Release -o out

# Fase de execução
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/out .

# O Render utiliza a porta 8080 por defeito nos planos gratuitos
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# Iniciar a aplicação
ENTRYPOINT ["dotnet", "Base_de_dados_Fonoaudiologia.dll"]