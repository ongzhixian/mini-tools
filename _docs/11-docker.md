# Docker

# tldr;

docker pull mcr.microsoft.com/dotnet/runtime:6.0
docker pull mcr.microsoft.com/dotnet/sdk:6.0
docker pull mcr.microsoft.com/dotnet/aspnet:6.0
<manual: create a file named `Dockerfile` with contents>

docker build -t testapp .
docker build -t testapp:0.1 .
(Without `:0.1` defaults to `:latest`)


For console apps, run with `-it` (so that we can see console output)
docker run -it --rm testapp

Set working directory of container to `/app` (which by default defined in our Dockerfile, it should already by `/app`; so redundant)
docker run -it --rm -w /app testapp  

Map current directory to `/data` in container
docker run -it --rm -w /app -v "$(pwd):/data" testapp

Map `data` in current directory to `/data` in container
docker run -it --rm -w /app -v "$(pwd)/data:/data" testapp

docker-compose build
docker-compose up
docker-compose up -d

# dotnet docker images

dotnet/sdk          : .NET SDK
dotnet/aspnet       : ASP.NET Core Runtime
dotnet/runtime      : .NET Runtime
dotnet/runtime-deps : .NET Runtime Dependencies
dotnet/monitor      : .NET Monitor Tool
dotnet/samples      : .NET Samples


## Examples

```docker
# Build
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY MiniTools.HostApp.csproj .
RUN dotnet restore
COPY . .
RUN dotnet publish -c release -o /app

# Runtime entrypoint
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
EXPOSE 80
EXPOSE 443
COPY --from=build /app .
ENTRYPOINT ["dotnet", "backend.dll"]

```

Pull the mcr.microsoft.com/dotnet/sdk:6.0 image and name the image build
Set the working directory within the image to /src
Copy the file named backend.csproj found locally to the /src directory that was just created
Calls dotnet restore on the project
Copy everything in the local working directory to the image
Calls dotnet publish on the project

Pull the mcr.microsoft.com/dotnet/aspnet:6.0 image
Set the working directory within the image to /app
Exposes port 80 and 443
Copy everything from the /app directory of the build image created above into the app directory of this image
Sets the entrypoint of this image to dotnet and passes backend.dll as an argument


## docker-compose

```
version: '3.4'

services: 

  frontend:
    image: pizzafrontend
    build:
      context: frontend
      dockerfile: Dockerfile
    environment: 
      - backendUrl=http://backend
    ports:
      - "5902:80"
    depends_on: 
      - backend
  backend:
    image: pizzabackend
    build: 
      context: backend
      dockerfile: Dockerfile
    ports: 
      - "5900:80"
```


First, it creates the frontend website, naming it pizza frontend. 
The code tells Docker to build it, pointing to the Dockerfile found in the frontend folder.
Then the code sets an environment variable for the website: backendUrl=http://backend. 
Finally, this code opens a port and declares it depends on the backend service.

The backend service gets created next. 
It's named pizzabackend. 
It's built from the same Dockerfile you created in the previous exercise. The last command specifies which port to open.


```
version: "3.7"

services:
  app:
    image: node:12-alpine
    command: sh -c "yarn install && yarn run dev"
    ports:
      - 3000:3000
    working_dir: /app
    volumes:
      - ./:/app
    environment:
      MYSQL_HOST: mysql
      MYSQL_USER: root
      MYSQL_PASSWORD: secret
      MYSQL_DB: todos

  mysql:
    image: mysql:5.7
    volumes:
      - todo-mysql-data:/var/lib/mysql
    environment:
      MYSQL_ROOT_PASSWORD: secret
      MYSQL_DATABASE: todos

volumes:
  todo-mysql-data:
```


# References

https://github.com/dotnet/dotnet-docker
https://hub.docker.com/_/microsoft-dotnet


https://docs.docker.com/develop/develop-images/dockerfile_best-practices/
