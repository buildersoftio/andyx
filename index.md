## Welcome to Buildersoft Andy X

Buildersoft Andy X is an open-source distributed streaming platform designed to deliver the best performance possible for high-performance data pipelines, streaming analytics, streaming between microservices and data integrations.

# Deploying Andy X on Docker Compose
## Development environment
### Windows
Generate certificate and configure local machine using .NET Core CLI

         dotnet dev-certs https -ep %USERPROFILE%\.aspnet\https\{your_domain}_private_key.pfx -p {your_password}
         dotnet dev-certs https --trust

### Linux or MacOs
Generate certificate and configure local machine using .NET Core CLI

         dotnet dev-certs https -ep ${HOME}/.aspnet/https/{your_domain}_private_key.pfx  -p {your_password}
         dotnet dev-certs https --trust

## Docker-compose file for Andy X Data Storage and Andy X

      version: '3.4'
      
      services:
        andyxstorage:
          container_name: andyxstorage
          image: buildersoftdev/andyxstorage
          ports:
            - 9002:443
          environment:
            - ASPNETCORE_ENVIRONMENT=Development
            - ASPNETCORE_URLS=https://+:443
            - ASPNETCORE_Kestrel__Certificates__Default__Password={your_password}
            - ASPNETCORE_Kestrel__Certificates__Default__Path=/https/{your_domain}_private_key.pfx
          volumes:
            - ~/.aspnet/https:/https:ro
          links:
            - andyx
          networks:
            - local
          
        andyx:
          container_name: andyxnode
          image: buildersoftdev/andyx
          ports:
            - 9001:443
          environment:
            - ASPNETCORE_ENVIRONMENT=Development
            - ASPNETCORE_URLS=https://+:443
            - ASPNETCORE_Kestrel__Certificates__Default__Password={your_password}
            - ASPNETCORE_Kestrel__Certificates__Default__Path=/https/{your_domain}_private_key.pfx
          volumes:
            - ~/.aspnet/https:/https:ro
          networks:
            - local
            
      networks:
        local:
          driver: bridge
