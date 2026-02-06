# aspnet versiyonunun docker image adresi veriliyor

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base

# container içindeki işlemlerin root yerine belirtilen kullanıcı kimliğiyle çalışmasını sağlar. Güvenlik içindir; zorunlu değildir. Yazma izni, port, dosya erişimi gibi konularda sorun yaşamıyorsan kaldırabilirsin

USER $APP_UID

# Docker içinde hangi klasörde çalışacağı seçiliyor

WORKDIR /app

# dışarı açılacak portu

EXPOSE 8080

# kullanılan sdk versiyonunun docker image adresi veriliyor

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

# build configuration argümanı tanımlanıyor - sonra kullanılacak

ARG BUILD_CONFIGURATION=Release

# Docker içinde hangi klasörde çalışacağı seçiliyor

WORKDIR /src

# buradaki csproj dosyası src klasörüne "Microservice.ProductWebAPI/" içine atılıyor

COPY ["Microservice.ProductWebAPI/Microservice.ProductWebAPI.csproj", "Microservice.ProductWebAPI/"]

# src içine atılan csproj üzerinde "dotnet restore" komutu çalıştırılıp kütüphaneler indiriliyor - container içindeki global NuGet cache’e indirilir

RUN dotnet restore "./Microservice.ProductWebAPI/Microservice.ProductWebAPI.csproj"

# bilgisayardaki tüm proje dosyaları container içindeki src dosyasına kopyalanır

COPY . .

# Docker içinde hangi klasörde çalışacağı seçiliyor

WORKDIR "/src/Microservice.ProductWebAPI"

# cs proj dosyası üzerinde "dotnet build" komutu çalıştırılıyor

RUN dotnet build "./Microservice.ProductWebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

# build adıyla indirilip işaretlenen sdk image seçiliyor

FROM build AS publish

# build configuration argümanı tanımlanıyor - sonra kullanılacak

ARG BUILD_CONFIGURATION=Release

# üstte workdir içine seçilen klasörde csproj üzerinde "dotnet publish" komutu uygulanıyor ve app klasörüne publis dosyaları oluşturuluyor

RUN dotnet publish "./Microservice.ProductWebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# base adıyla indirilip işaretlenen aspnet image seçiliyor

FROM base AS final

# çalışma klasörü belirleniyor

WORKDIR /app

# publish edilen dosyalar ana klasöre taşınıyor

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Microservice.ProductWebAPI.dll"]
