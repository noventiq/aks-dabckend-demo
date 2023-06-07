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

`az account set --subscription 07fbccc0-2218-4dfe-xxxx-xxxxxxxxxxxx`

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
docker build . -t webapibrt:1.0 --> docker hub

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
```
kubectl apply -f .\sk8\pod-webapibrt.yaml
kubectl exec -it webapibrt -- /bin/sh
apt update <-- opcional, si el pod no tiene instalado curl
apt install curl <-- opcional, si el pod no tiene instalado curl
curl -v -X 'GET' 'http://localhost/v1/Customer?name=a' -H 'accept: application/json'
```

# desplegar
```
kubectl apply -f .\sk8\deploy-webapi-cloud.yaml

kubectl get services,deployments,pods
```