@ECHO OFF
SET NAMESPACE=meta
SET PROJECT=example-private-api
SET CONFIG_PATH=../../../../src/Example/Hosts/Private/Api/appsettings.Docker.json

echo ----------------------------------------
echo %PROJECT%
echo ----------------------------------------


kubectl create namespace meta

kubectl delete -n %NAMESPACE% --now deployment %PROJECT% --cascade=background
kubectl delete -n %NAMESPACE% service %PROJECT% --cascade=background
kubectl delete -n %NAMESPACE% ingress %PROJECT% --cascade=background
kubectl delete -n %NAMESPACE% configmap %PROJECT%-configmap --cascade=background

kubectl apply -n %NAMESPACE% -f deployment.yaml
kubectl apply -n %NAMESPACE% -f service.yaml
kubectl apply -n %NAMESPACE% -f ingress.yaml
kubectl create configmap -n %NAMESPACE% %PROJECT%-configmap --from-file=%CONFIG_PATH%

if %ERRORLEVEL% == 1 pause