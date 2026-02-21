set WORKSPACE=..
set LUBAN_DLL=%WORKSPACE%\Tools\Luban\Luban.dll
set CONF_ROOT=.

dotnet %LUBAN_DLL% ^
    -t all ^
    -d protobuf2-bin ^
    --conf %CONF_ROOT%\luban.conf ^
    -x outputDataDir=output

pause