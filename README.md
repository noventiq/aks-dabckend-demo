## Instalar azure-cli
Ejecutar desde powershell como administrador

`$ProgressPreference = 'SilentlyContinue'; Invoke-WebRequest -Uri https://aka.ms/installazurecliwindows -OutFile .\AzureCLI.msi; Start-Process msiexec.exe -Wait -ArgumentList '/I AzureCLI.msi /quiet'; Remove-Item .\AzureCLI.msi`

Instalar actualización de azure-cli

`az upgrade`

Instalar cliente de aks

`az aks install-cli`

Referencia: https://learn.microsoft.com/en-us/cli/azure/install-azure-cli


## Autentificarse en los servicios de azure

### Iniciar sesión y usar suscripción azure
`az login`

`az account set --subscription 07fbccc0-2218-4dfe-84b9-17f4d8322419`

## Iniciar sesión en azure container registry
`az acr login --name yyyyyyyyyy`

`docker login yyyyyyyyyy.azurecr.io`

## Iniciar sesión en cluster AKS
`az aks get-credentials --resource-group "aks_rg" --name "aks_cluster"`

## Crear grupo de recursos

## Crear redes

### red 1

Especial considerar en la red y subred

![vnet-1](/sk8/images/myvm1-vnet-1.png)

![vnet-subnet-2](/sk8/images/myvm1-vnet-2.png)

### red 2
![vnet-2](/sk8/images/myaks-vnet-1.png)

![vnet-subnet-2](/sk8/images/myaks-vnet-2.png)


### Configuración de máquina virtual en red 1



#### Firewall en Windows server
Abrir firewall para el servicio ICMPv4 para poder hacer pruebas de conectividad con `ping` 

Desde powershell ejecutar:

```
New-NetFirewallRule `
-Name 'ICMPv4' `
-DisplayName 'ICMPv4' `
-Description 'Allow ICMPv4' `
-Profile Any `
-Direction Inbound `
-Action Allow `
-Protocol ICMPv4 `
-Program Any `
-LocalAddress Any `
-RemoteAddress Any
```
Referencia: https://www.server-world.info/en/note?os=Windows_Server_2022&p=initial_conf&f=6

#### Servicio de Base de Datos SQL server
Validar que el motor de base de datos soporte la conexión por IP y autentificación SQL

![SqlServer allow ip connection](/sk8/images/sqlserver-config-allow-ip.png)

![SqlServer allow sql connection](/sk8/images/sqlserver-config-allow-sql-connetion.png)


### Emparejamiento de redes

![emparejamiento](/sk8/images/peer-1.png)

| Configuración                        | Valor                                                                                                                                                          |
|--------------------------------------|----------------------------------------------------------------------------------------------------------------------------------------------------------------|
| Esta red virtual                     |
| Nombre del vínculo de emparejamiento | Introduzca myaks_vnet__myvm1_vnet como nombre del emparejamiento de myVirtualNetwork1 a la red virtual remota.                                    |
| Red virtual remota                   |
| Nombre del vínculo de emparejamiento | Introduzca myVirtualNetwork2-myVirtualNetwork1 como nombre del emparejamiento de la red virtual remota a myVirtualNetwork1.                                    |
| Suscripción                          | Seleccione la suscripción de la red virtual remota.                                                                                                            |
| Virtual network                      | Seleccione myVirtualNetwork2 como nombre de la red virtual remota. La red virtual remota puede estar en la misma región de myVirtualNetwork1 o en otra región. |


Referencia: https://learn.microsoft.com/es-es/azure/virtual-network/tutorial-connect-virtual-networks-portal

## Creación y configuración AKS y ACR


Referencia: https://learn.microsoft.com/es-es/azure/aks/configure-azure-cni

## Generación de imagen docker
```
docker build . -t webapi:1.0 --> docker hub

docker build . -t yyyyyyyyyy.azurecr.io/webapibrt:1.0 <-- acr
```


# subir imagen a acr
```
docker push yyyyyyyyyy.azurecr.io/webapibrt:1.0.0
```

# imagen para validar conectividad
```
kubectl apply -f network-multitool.yaml
kubectl get pods
kubectl exec -it network-multitool -- /bin/sh
```

# validar desde el pod que esté respondiendo el API desde localhost
```ps
kubectl apply -f .\sk8\pod-webapibrt.yaml
kubectl exec -it pod/app-ping-85c85c6d5d-l5dns -- /bin/sh
apt update <-- opcional, si el pod no tiene instalado curl
apt install curl <-- opcional, si el pod no tiene instalado curl
curl -v -X 'GET' 'http://localhost/v1/Customer?name=a' -H 'accept: application/json'
curl -v -X 'GET' 'http://api-ping.default.svc.cluster.local/api/v1/ping' -H 'accept: application/json'
curl -v -X 'GET' 'http://api-pong.default.svc.cluster.local/api/v1/pong' -H 'accept: application/json'
```

# desplegar
```ps
kubectl apply -f .\sk8\deploy-webapi-cloud.yaml

kubectl get services,deployments,pods
```

```dockerfile
.
.
.
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
ENV ConnectionStrings__SqlConnection="desde docker por defecto"
WORKDIR /app
.
.
.
```

Construimos la imagen
```ps
docker build . -t webapi:1.0
```

Ponemos en marcha el contenedor `sin argumento`
```ps
docker run -it -p 8090:80 webapi:1.0
```

Luego probamos para ver el valor que tomó la variable

```ps
curl http://localhost:8090/v1/Customer?name=a
```

Ponemos en marcha el contenedor `sin argumento`
```ps
docker run -it -e ConnectionStrings__SqlConnection="desde docker env" -p 8090:80 webapi:1.0
```

Luego probamos para ver el valor que tomó la variable

```ps
curl http://localhost:8090/v1/Customer?name=a
```

kubectl describe ingress example-ingress

### Instale el `ingress controller NGINX`
Escoja la versión correcta de nginx controller `https://github.com/kubernetes/ingress-nginx#supported-versions-table`

Instalará el controlador en el espacio de nombres ingress-nginx, creando ese espacio de nombres si aún no existe.

```
kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/controller-v1.8.0/deploy/static/provider/cloud/deploy.yaml
```

### Compruebe que el módulo `ingress controller` se está ejecutando

`https://github.com/kubernetes/ingress-nginx#supported-versions-table`

```
kubectl get pods --namespace ingress-nginx
```

### Verifique que al `ingress controller NGINX` se le haya asignado una dirección IP pública

```
kubectl get service ingress-nginx-controller --namespace=ingress-nginx
```

### Configure una aplicación web básica para probar nuestro nuevo `ingress controller`
Reemplazar el texto [DNS_NAME] con tu dominio correcto, ejemplo: www.estudio.pe

```
kubectl create ingress demo --class=nginx --rule [DNS_NAME]/=demo:80
```

### Configurar sus aplicaciones web

aks-application-one.yaml

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: webapi
  labels:
    app: weather-forecast
spec:
  replicas: 1
  selector:
    matchLabels:
      service: webapi
  template:
    metadata:
      labels:
        app: weather-forecast
        service: webapi
    spec:
      containers:
        - name: webapi
          image: isaiasmayonh/webapi:1.0
          imagePullPolicy: IfNotPresent
          ports:
            - containerPort: 80
              protocol: TCP
          env:
            - name: ASPNETCORE_URLS
              value: http://+:80
            - name: ConnectionStrings__SqlConnection
              value: "SqlConnection desde k8s"
---
apiVersion: v1
kind: Service
metadata:
  name: webapi
  labels:
    app: weather-forecast
    service: webapi
spec:
  type: ClusterIP
  ports:
    - port: 80
      targetPort: 80
      protocol: TCP
  selector:
    service: webapi
```

aks-application-two.yaml

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: webapi
  labels:
    app: weather-forecast
spec:
  replicas: 1
  selector:
    matchLabels:
      service: webapi
  template:
    metadata:
      labels:
        app: weather-forecast
        service: webapi
    spec:
      containers:
        - name: webapi
          image: isaiasmayonh/webapi:1.0
          imagePullPolicy: IfNotPresent
          ports:
            - containerPort: 80
              protocol: TCP
          env:
            - name: ASPNETCORE_URLS
              value: http://+:80
            - name: ConnectionStrings__SqlConnection
              value: "SqlConnection desde k8s"
---
apiVersion: v1
kind: Service
metadata:
  name: webapi
  labels:
    app: weather-forecast
    service: webapi
spec:
  type: ClusterIP
  ports:
    - port: 80
      targetPort: 80
      protocol: TCP
  selector:
    service: webapi
```

Aplicar la configuración de tus aplicaciones web:

```
kubectl apply -f aks-application-one.yaml --namespace ingress-nginx
kubectl apply -f aks-application-two.yaml --namespace ingress-nginx
```

Revisar que estén ejecutandose las aplicaciones

```
kubectl get pods --namespace ingress-nginx
```

### Configure Ingress para enrutar el tráfico entre las dos aplicaciones

Crear archivo example-ingerss.yaml

```yaml
apiVersion: networking.k8s.io/v1
kind: Ingress
metadata:
  name: example-ingress
  annotations:
    nginx.ingress.kubernetes.io/ssl-redirect: "false"
    nginx.ingress.kubernetes.io/use-regex: "true"
    nginx.ingress.kubernetes.io/rewrite-target: /$2
spec:
  ingressClassName: nginx
  rules:
  - http:
      paths:
      - path: /api(/|$)(.*)
        pathType: Prefix
        backend:
          service:
            name: webapi
            port: 
              number: 80
```

### Crear ingress 

```
kubectl apply -f hello-world-ingress.yaml --namespace ingress-nginx
```

Referencia: https://spacelift.io/blog/kubernetes-ingress


### Crear secret 

```ps
kubectl create secret tls proxy-xxxxxxx-xxx-xxxx-tls-secret --key brxxxxx.key --cert STAR_xxxx_xxxx_pe.crt

kubectl get secret
```

### Acceso a powershell
https://learn.microsoft.com/es-es/azure/aks/control-kubeconfig-access

### Azure DevOps Service: Pipeline CI

```yaml
# Docker
# Build a Docker image
# https://docs.microsoft.com/azure/devops/pipelines/languages/docker

trigger:
- master

resources:
- repo: self

variables:
  ImageName: 'isaiasmayonh/webapi:$(Build.BuildId)'
 
stages:
- stage: Build
  displayName: Build image
  jobs:  
  - job: Build
    displayName: Build and push Docker image
    steps:
    - task: Docker@1
      displayName: 'Build the Docker image'
      inputs:
        containerregistrytype: 'Container Registry'
        dockerRegistryEndpoint: 'DockerConnection'
        command: 'Build an image'
        dockerFile: 'Dockerfile'
        imageName: '$(ImageName)'
        includeLatestTag: true
        useDefaultContext: false
        buildContext: '.'
     
    - task: Docker@1
      displayName: 'Push the Docker image to Dockerhub'
      inputs:
        containerregistrytype: 'Container Registry'
        dockerRegistryEndpoint: 'DockerConnection'
        command: 'Push an image'
        imageName: '$(ImageName)'
      condition: and(succeeded(), ne(variables['Build.Reason'], 'PullRequest'))

    - task: CopyFiles@2
      displayName: 'Copiar archivo yaml'
      inputs:
        SourceFolder: './k8s/micro'
        Contents: 'deploy-webapi.yml'
        TargetFolder: '$(Build.ArtifactStagingDirectory)'
        OverWrite: true

    - task: PublishBuildArtifacts@1
      inputs:
        PathtoPublish: '$(Build.ArtifactStagingDirectory)'
        ArtifactName: 'drop'
        publishLocation: 'Container'

```


### Azure DevOps Service: Pipeline CD
![Azure DevOps Service: Pipeline CD](/k8s/images/az-devops-pipeline-cd-1.png)

![Azure DevOps Service: Pipeline CD](/k8s/images/az-devops-pipeline-cd-2.png)

![Azure DevOps Service: Pipeline CD](/k8s/images/az-devops-pipeline-cd-3.png)

![Azure DevOps Service: Pipeline CD](/k8s/images/az-devops-pipeline-cd-4.png)