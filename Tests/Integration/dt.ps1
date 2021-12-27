
$base_url = " https://localhost:7001"

$path = "/api/User"

$url = "$base_url$path"
$headers = @{
    "Content-Type" = "application/json"
}


$mock_data = Import-Csv .\MOCK_DATA.csv 
foreach ($item in $mock_data) {
    $item
    $body = @{
        "username" = $item.username
        "password" = $item.password
    } | ConvertTo-Json -Compress
    
    $response = Invoke-WebRequest -Method 'Post' -Uri $url -Headers $headers -Body $body
}