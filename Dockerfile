# aspnet versiyonunun docker image adresi veriliyor
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
# container içindeki iþlemlerin root yerine belirtilen kullanýcý kimliðiyle çalýþmasýný saðlar. Güvenlik içindir; zorunlu deðildir. Yazma izni, port, dosya eriþimi gibi konularda sorun yaþamýyorsan kaldýrabilirsin
USER $APP_UID
# Docker içinde hangi klasörde çalýþacaðý seçiliyor
WORKDIR /app
# dýþarý açýlacak portu
EXPOSE 8080


# kullanýlan sdk versiyonunun docker image adresi veriliyor
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
# build configuration argümaný tanýmlanýyor - sonra kullanýlacak
ARG BUILD_CONFIGURATION=Release
# Docker içinde hangi klasörde çalýþacaðý seçiliyor
WORKDIR /src
# buradaki csproj dosyasý src klasörüne "Microservice.ProductWebAPI/" içine atýlýyor
COPY ["Microservice.ProductWebAPI/Microservice.ProductWebAPI.csproj", "Microservice.ProductWebAPI/"]
# src içine atýlan csproj üzerinde "dotnet restore" komutu çalýþtýrýlýp kütüphaneler indiriliyor - container içindeki global NuGet cache’e indirilir
RUN dotnet restore "./Microservice.ProductWebAPI/Microservice.ProductWebAPI.csproj"
# bilgisayardaki tüm proje dosyalarý container içindeki src dosyasýna kopyalanýr
COPY . .
# Docker içinde hangi klasörde çalýþacaðý seçiliyor
WORKDIR "/src/Microservice.ProductWebAPI"
# cs proj dosyasý üzerinde "dotnet build" komutu çalýþtýrýlýyor
RUN dotnet build "./Microservice.ProductWebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

# build adýyla indirilip iþaretlenen sdk image seçiliyor
FROM build AS publish
# build configuration argümaný tanýmlanýyor - sonra kullanýlacak
ARG BUILD_CONFIGURATION=Release
# üstte workdir içine seçilen klasörde csproj üzerinde "dotnet publish" komutu uygulanýyor ve app klasörüne publis dosyalarý oluþturuluyor
RUN dotnet publish "./Microservice.ProductWebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# base adýyla indirilip iþaretlenen aspnet image seçiliyor
FROM base AS final
# çalýþma klasörü belirleniyor
WORKDIR /app
# publish edilen dosyalar ana klasöre taþýnýyor
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Microservice.ProductWebAPI.dll"]