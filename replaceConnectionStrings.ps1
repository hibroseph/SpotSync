Write-Output "Starting Program"

$server = $( Read-Host "Input server name")
$user = $( Read-Host "Input user name")
$password = $( Read-Host "Input password")


Write-Output $server

Get-ChildItem "." -Recurse -Filter *AppSettings.json | Foreach-Object {
    Write-Output $_.FullName 
    $content = Get-Content $_.FullName

    $connectionStringToUpdate = $content [regex]('(?<=Server=)dev-.*(?=_LSGlobalAdmin)')

    Write-Output $connectionStringToUpdate
    $content = $content -replace [regex]('(?<=Server=)dev-.*(?=_LSGlobalAdmin)'), $server
    $content = $content -replace [regex]('(?<=User=).*(?=;)'), $user
    $content = $content -replace [regex]('(?<=PW=).*(?=;)'), $password

  
}

Write-Output "Ending Program"