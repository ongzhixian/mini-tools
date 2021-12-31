# A simple way to encode an arbitrary string into integer; 
# Note: This algorith allow collisions (because the underlying hash algorithm allow collisions)

$a = "MiniTools.Web.Controllers.LoginController"

if ($args.Length -gt 0)
{
    
    $a = $args[0]
    Write-Host "Have args; [$a]"
}

$bytes = [Text.Encoding]::UTF8.GetBytes($a)

$b = [System.Security.Cryptography.SHA1CryptoServiceProvider]::new().ComputeHash($bytes)

$ans = 0
for ($i = 0; $i -lt $b.Length; $i++) {
    # Write-Host $($b[$i] * ($i + 1))
    $ans = $ans + $($b[$i] * ($i + 1))
}

"$ans$($ans % 11)" | Set-Clipboard
Write-Host "Final: $ans$($ans % 11)" 

# [Numerics.BigInteger]::new([Text.Encoding]::UTF8.GetBytes("MiniTools.Web.Controllers.LoginController"))

# [System.Security.Cryptography.SHA1CryptoServiceProvider]

# [System.Security.Cryptography.SHA1CryptoServiceProvider]::new()

# [System.BitConverter]::ToString($ab);

# 32-bit int (4 bytes) 
# 20 bytes
# EF-88-03-F0-3E-51-F4-BE-17-C0-0D-A0-9B-BA-09-B7-46-B6-2B-7C
# Which means we have groups of 5 bytes
# EF-88-03-F0-3E
# 51-F4-BE-17-C0
# 0D-A0-9B-BA-09
# B7-46-B6-2B-7C