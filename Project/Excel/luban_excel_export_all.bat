set WORKSPACE=..
set LUBAN_DLL=%WORKSPACE%\Tools\Luban\Luban.dll
set CONF_ROOT=.
set OUT_DIR=%WORKSPACE%\AIRebot\Assets\Resources\@Config\

dotnet %LUBAN_DLL% ^
    -t all ^
    -d protobuf2-bin ^
    --conf %CONF_ROOT%\luban.conf ^
    -x outputDataDir=%OUT_DIR%

pause