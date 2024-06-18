@ECHO OFF
SET ENVIRONMENT=localhost
SET NAMESPACE=meta
SET PROJECT=example-consumer
SET CONFIG_PATH=../../../../src/Example/Hosts/Consumers/appsettings.json

echo ----------------------------------------
echo %PROJECT%
echo ----------------------------------------


kubectl create namespace meta

kubectl delete -n %NAMESPACE% --now deployment %PROJECT% --cascade=background
kubectl delete -n %NAMESPACE% service %PROJECT% --cascade=background
kubectl delete -n %NAMESPACE% configmap %PROJECT% --cascade=background

kubectl create configmap -n %NAMESPACE% %PROJECT% --from-file=%CONFIG_PATH% --dry-run=client -o yaml > charts/templates/configmap.yaml
if %ERRORLEVEL% == 1 goto end

helm upgrade --install -n %NAMESPACE% -f charts/values.%ENVIRONMENT%.yaml %PROJECT% ./charts
if %ERRORLEVEL% == 1 goto end

:end
pause