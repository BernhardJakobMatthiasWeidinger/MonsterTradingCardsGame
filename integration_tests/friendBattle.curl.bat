@echo off

REM --------------------------------------------------
REM Monster Trading Cards Game
REM --------------------------------------------------
title Monster Trading Cards Game
echo CURL Testing for Monster Trading Cards Game
echo.

echo 23) friend battle
start /b "kienboec battle" curl -X POST http://localhost:10001/battles --header "Authorization: Basic kienboec-mtcgToken"
start /b "altenhof battle" curl -X POST http://localhost:10001/battles/kienboec --header "Authorization: Basic altenhof-mtcgToken"
ping localhost -n 5 >NUL 2>NUL

echo 2) scoreboard
curl -X GET http://localhost:10001/score?format=plain --header "Authorization: Basic kienboec-mtcgToken"
echo.