# Start Server
$APP = Start-Process -passthru -NoNewWindow dotnet -ArgumentList "run --no-build --no-restore --project src"

dotnet test ./tests

# echo "Stopping dotnet API"
Stop-Process -Force $APP.Id