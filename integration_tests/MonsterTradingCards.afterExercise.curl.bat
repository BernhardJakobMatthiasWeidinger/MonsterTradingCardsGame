@echo off
echo 1) show stacks
curl -X GET http://localhost:10001/cards --header "Authorization: Basic kienboec-mtcgToken"
echo.
curl -X GET http://localhost:10001/cards?format=plain --header "Authorization: Basic altenhof-mtcgToken"
echo. 
echo.

echo 2) show decks
curl -X GET http://localhost:10001/deck --header "Authorization: Basic kienboec-mtcgToken"
echo.
curl -X GET http://localhost:10001/deck?format=plain --header "Authorization: Basic altenhof-mtcgToken"
echo.
echo. 

echo 3) get user data
echo.
curl -X GET http://localhost:10001/users/kienboec --header "Authorization: Basic kienboec-mtcgToken"
echo.
curl -X GET http://localhost:10001/users/altenhof --header "Authorization: Basic altenhof-mtcgToken"
echo.
echo. 

echo 4) get stats
curl -X GET http://localhost:10001/stats --header "Authorization: Basic kienboec-mtcgToken"
echo.
curl -X GET http://localhost:10001/stats?format=plain --header "Authorization: Basic altenhof-mtcgToken"
echo.
echo. 

echo 5) scoreboard
curl -X GET http://localhost:10001/score?format=plain --header "Authorization: Basic kienboec-mtcgToken"
echo.
echo. 

echo 6) get trades
curl -X GET http://localhost:10001/tradings?format=plain --header "Authorization: Basic kienboec-mtcgToken"
echo.
echo. 

echo 7) get friends
echo.
curl -X GET http://localhost:10001/friends --header "Authorization: Basic kienboec-mtcgToken"
echo.
curl -X GET http://localhost:10001/friends?format=plain --header "Authorization: Basic altenhof-mtcgToken"
echo.
echo. 