@echo off
setlocal

set exename=%1
set runtimedir=%2
shift
shift

if "%1" == "" goto EndArgLoop
set remainder=%1
shift
:ArgLoop
if "%1" == "" goto EndArgLoop
set remainder=%remainder% %1
shift
goto ArgLoop

:EndArgLoop

:FindExeLoc
if exist %runtimedir%%exename%.exe (
    if exist %runtimedir%CoreRun.exe (
        set execallstring=%runtimedir%CoreRun.exe %runtimedir%%exename%.exe
    ) ELSE (
        set execallstring=%runtimedir%%exename%.exe
    )
    goto :InvokeExe
)
    echo Error, executable not found: %runtimedir%%exename%.exe
    exit /b

:InvokeExe
%execallstring% %remainder%
