# Docker

# tldr;

docker pull mcr.microsoft.com/dotnet/runtime:6.0
docker pull mcr.microsoft.com/dotnet/sdk:6.0
docker pull mcr.microsoft.com/dotnet/aspnet:6.0
<manual: create a file named `Dockerfile` with contents>

docker build -t testapp .
docker build -t testapp:0.1 .
(Without `:0.1` defaults to `:latest`)

docker build --build-arg RUNTIME_SERVICE=SOME_RTS_VALUE -t testapp .
docker build --build-arg RUNTIME_SERVICE=MiniTools.HostApp.Services.ExampleBackgroundService -t testapp .

docker run -it --rm --env RUNTIME_SERVICE=SOME_RTS_VALUE testapp

For console apps, run with `-it` (so that we can see console output)
docker run -it --rm testapp

Set working directory of container to `/app` (which by default defined in our Dockerfile, it should already by `/app`; so redundant)
docker run -it --rm -w /app testapp  

Map current directory to `/data` in container
docker run -it --rm -w /app -v "$(pwd):/data" testapp

Map `data` in current directory to `/data` in container
docker run -it --rm -w /app -v "$(pwd)/data:/data" testapp

docker-compose build
docker-compose build --build-arg RUNTIME_SERVICE=SOME_RTS_VALUE222

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
FROM mcr.microsoft.com/dotnet/runtime:6.0
WORKDIR /app
EXPOSE 80
EXPOSE 443
COPY --from=build /app .
ARG RUNTIME_SERVICE=default_value
ENV RUNTIME_SERVICE=$RUNTIME_SERVICE
ENTRYPOINT ["dotnet", "MiniTools.HostApp.dll"]


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

```yml
version: '3.4'

services: 

  testApp:
    image: testapp
    build: 
      context: MiniTools.HostApp
      dockerfile: Dockerfile
    environment: 
      - backendUrl=http://backend
    ports: 
      - "5900:80"
    volumes:
       - todo-mysql-data:/data
       - type: bind
         source: ./applocal
         target: /data

volumes:
  todo-mysql-data:
```




```yml
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


```yml
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


```yml: docker-compose with bind mount and volumes
version: "3.2"
services:
  web:
    image: httpd:latest
    volumes:
      - type: bind
        source: $HOST/location
        target: /container/location
      - type: volume
        source: mydata
        target: /container/location
volumes:
  mydata:
``` 


ARG buildtime_variable=default_value # <- this one's new
ENV env_var_name=$buildtime_variable # we reference it directly

docker build --build-arg buildtime_variable=a_value # ... the rest of the build command is omitted


# References

https://github.com/dotnet/dotnet-docker
https://hub.docker.com/_/microsoft-dotnet


https://docs.docker.com/develop/develop-images/dockerfile_best-practices/

https://docs.docker.com/compose/compose-file/compose-file-v3/#volumes

https://benfoster.io/blog/optimising-dotnet-docker-images/

https://docs.docker.com/samples/dotnetcore/
