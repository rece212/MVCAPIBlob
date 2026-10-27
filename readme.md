# Running commands
## Docker Azurite run command 
```bash
docker run -p 10000:10000 -v azurite_data:/data --name azurite -d mcr.microsoft.com/azure-storage/azurite azurite-blob --blobHost 0.0.0.0 --location /data --debug /data/debug.log --skipApiVersionCheck
```
## API side
### Docker create build API
```bash
docker build -t blobapi -f APIBlob/Dockerfile .
```
### Docker run of the API
```bash
docker run -d --name apiblob-app -p 5000:8080 --add-host=host.docker.internal:host-gateway blobapi
```
## MVC Side 
### Docker create build MVC
```bash
docker build -t blobmvc -f MVCBlob/Dockerfile .
```
### Docker run of the MVC
```bash
docker run -d --name mvcblob-app -p 80:8080 --add-host=host.docker.internal:host-gateway blobmvc
```

## Clean  command
```bash
docker rm -f azurite apiblob-app mvcblob-app
```

