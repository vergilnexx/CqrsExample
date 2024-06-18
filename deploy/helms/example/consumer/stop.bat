@ECHO OFF
SET NAMESPACE=meta
SET PROJECT=example-consumer

echo ----------------------------------------
echo %PROJECT%
echo ----------------------------------------

kubectl delete -n %NAMESPACE% --now deployment %PROJECT% --cascade=background
if %ERRORLEVEL% == 1 goto end

kubectl delete -n %NAMESPACE% service %PROJECT% --cascade=background
if %ERRORLEVEL% == 1 goto end

kubectl delete -n %NAMESPACE% configmap %PROJECT% --cascade=background
if %ERRORLEVEL% == 1 goto end

:end
pause