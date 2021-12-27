# 1.  Describe    (the test scenario)         -- I am testing ... HTTP GET /country
# 2.  Context     (condition; when-clauses)   -- When I       ... do not pass parameters 
# 3.  It          (test; should-clauses)      -- It should    ... return a status code of 200

# Mini's UserAccount API

$base_url = " https://localhost:7001"

# Tests
# POST   /api/AppUser
# GET    /api/AppUser
# PUT    /api/AppUser   
# DELETE /api/AppUser

$path = "/api/User"

Describe "HTTP POST $path" -Tags @("HTTP", "POST", $path) {
    Context "pass valid body" {

        $url = "$base_url$path"
        $headers = @{
            "Content-Type" = "application/json"
        }
        $body = @{
            "username" = "testuser1"
            "password" = "testpassw0rd"
        } | ConvertTo-Json -Compress

        $response = Invoke-WebRequest -Method 'Post' -Uri $url -Headers $headers -Body $body

        It "return status code 201" {
            $response.StatusCode | Should Be 201 # HTTP 201 Created
        }
    }
}

$path = "/api/User"

Describe "HTTP GET $path" -Tags @("HTTP", "GET", $path) {
    
    Context "pass no parameters" {

        # Arrange
        $url = "$base_url$path/?page=1&pagesize=10"
        $headers = @{
            "Content-Type" = "application/json"
        }

        Write-Host $url

        # Act
        # $response = { Invoke-WebRequest -Method 'Get' -Uri $url -Headers $headers }
        $response = Invoke-WebRequest -Method 'Get' -Uri $url -Headers $headers 

        # Assert(s)
        # It "return a status code of 404" {
        #     $response | Should Throw "Response status code does not indicate success: 404 (Not Found)."
        #     # Write-Host "Response: ", $response, $response.StatusCode
        # }
        Write-Host "Response: ", $response, $response.StatusCode
        It "should return a status code of 200" {
            $response.StatusCode | Should Be 200 # HTTP 200 OK
        }
    }
}

$path = "/api/User/{id}"

Describe "HTTP GET $path" -Tags @("HTTP", "GET", $path) {

    Context "pass an existing id as parameter" {

        $path = "/api/AppUser/61b1a7fcf66c04dfe1a830d9"
        $url = "$base_url$path"
        $headers = @{
            "Content-Type" = "application/json"
        }

        $response = Invoke-WebRequest -Method 'Get' -Uri $url -Headers $headers
        
        It "return a status code of 200" {
            $response.StatusCode | Should Be 200
        }
    }

    Context "pass an non-existing id as parameter" {

        $path = "/api/AppUser/61b1a7fcf66c04dfe1a830d8"
        $url = "$base_url$path"
        $headers = @{
            "Content-Type" = "application/json"
        }

        $response = { Invoke-WebRequest -Method 'Get' -Uri $url -Headers $headers }
        
        It "return a status code of 404" {
            $response | Should Throw "Response status code does not indicate success: 404 (Not Found)."
        }
    }

}
