param(
  [string]$BaseUrl = "http://localhost:5164"
)

$ErrorActionPreference = 'Stop'

function Write-Step($msg) { Write-Host "[STEP] $msg" -ForegroundColor Cyan }
function Write-Ok($msg) { Write-Host "[ OK ] $msg" -ForegroundColor Green }
function Write-Fail($msg) { Write-Host "[FAIL] $msg" -ForegroundColor Red }

function Assert-Status($resp, [int[]]$expected) {
  $code = [int]$resp.StatusCode
  if (-not ($expected -contains $code)) {
    Write-Fail "Expected status $expected but got $code"
    if ($resp.Content) { Write-Host $resp.Content }
    exit 1
  }
}

function Login([string]$email, [string]$password) {
  $sess = New-Object Microsoft.PowerShell.Commands.WebRequestSession
  $body = @{ Email = $email; Password = $password } | ConvertTo-Json
  $resp = Invoke-WebRequest -UseBasicParsing -Method Post -Uri "$BaseUrl/auth/login" -ContentType 'application/json' -Body $body -WebSession $sess
  Assert-Status $resp 200
  return $sess
}

function Try-Create-Team([object]$sess, [string]$name, [string]$coachEmail) {
  $teamBody = @{ Name = $name; Location = 'City'; IsPublic = $true; CoachEmail = $coachEmail } | ConvertTo-Json
  return Invoke-WebRequest -UseBasicParsing -Method Post -Uri "$BaseUrl/api/teams/" -ContentType 'application/json' -Body $teamBody -WebSession $sess -ErrorAction Stop
}

function Try-Update-Team([object]$sess, [int]$teamId, [string]$newName) {
  $body = @{ Name = $newName } | ConvertTo-Json
  return Invoke-WebRequest -UseBasicParsing -Method Put -Uri ("$BaseUrl/api/teams/{0}" -f $teamId) -ContentType 'application/json' -Body $body -WebSession $sess -ErrorAction Stop
}

function Try-Delete-Team([object]$sess, [int]$teamId) {
  return Invoke-WebRequest -UseBasicParsing -Method Delete -Uri ("$BaseUrl/api/teams/{0}" -f $teamId) -WebSession $sess -ErrorAction Stop
}

function Get-Goals() {
  return Invoke-RestMethod -Method Get -Uri "$BaseUrl/api/player-goals/" -ErrorAction Stop
}

function Try-Update-Goal([object]$sess, [int]$goalId, [int]$value) {
  $body = @{ CurrentValue = $value } | ConvertTo-Json
  return Invoke-WebRequest -UseBasicParsing -Method Put -Uri ("$BaseUrl/api/player-goals/{0}" -f $goalId) -ContentType 'application/json' -Body $body -WebSession $sess -ErrorAction Stop
}

# Health check
Write-Step "Health check"
$health = Invoke-WebRequest -UseBasicParsing -Method Get -Uri "$BaseUrl/health"
Assert-Status $health 200
Write-Ok "Health: 200"

# Coach flow: create, update, delete team
Write-Step "Coach login"
$coachSess = Login 'coach.johnson@nextup.com' 'password123'
Write-Ok "Coach login success"

Write-Step "Coach create team"
$respCreate = Try-Create-Team $coachSess 'Smoke Team' 'coach.johnson@nextup.com'
Assert-Status $respCreate 201
$createdObj = $null
try { $createdObj = $respCreate.Content | ConvertFrom-Json } catch {}
$teamId = if ($createdObj -and $createdObj.teamId) { [int]$createdObj.teamId } elseif ($createdObj -and $createdObj.TeamId) { [int]$createdObj.TeamId } else { 0 }
if ($teamId -le 0) { Write-Fail "Could not parse created team id"; Write-Host $respCreate.Content; exit 1 }
Write-Ok "Team created id=$teamId"

Write-Step "Coach update team"
$respUpdate = Try-Update-Team $coachSess $teamId 'Smoke Team Updated'
Assert-Status $respUpdate 200
Write-Ok "Team update OK"

Write-Step "Coach delete team"
$respDelete = Try-Delete-Team $coachSess $teamId
Assert-Status $respDelete 200
Write-Ok "Team delete OK"

# Player flow
Write-Step "Player login (Jake)"
$playerSess = Login 'jake.thompson@student.com' 'player123'
Write-Ok "Player login success"

Write-Step "Player attempt to create team (expect 403)"
try {
  $resp = Try-Create-Team $playerSess 'Player Team' 'coach.williams@nextup.com'
  Write-Fail ("Unexpected success creating team as player: " + [int]$resp.StatusCode)
  exit 1
} catch {
  if ($_.Exception.Response) {
    $status = $_.Exception.Response.StatusCode.value__
    if ($status -ne 403) { Write-Fail ("Expected 403, got $status"); exit 1 } else { Write-Ok "Player blocked as expected (403)" }
  } else { Write-Fail "No response for player create team"; exit 1 }
}

Write-Step "List player goals"
$goals = Get-Goals
if (-not $goals -or $goals.Count -lt 2) { Write-Fail "Expected at least 2 goals"; exit 1 }
$my = $goals | Where-Object { $_.Player.Name -like 'Jake Thompson*' } | Select-Object -First 1
if (-not $my) { Write-Fail "Could not find Jake's goal"; exit 1 }
$other = $goals | Where-Object { $_.Player.Name -ne $my.Player.Name } | Select-Object -First 1

Write-Step "Player updates own goal"
$respOwn = Try-Update-Goal $playerSess ([int]$my.PlayerGoalId) 999
Assert-Status $respOwn 200
Write-Ok "Player own goal update OK"

if ($other) {
  Write-Step "Player attempts to update someone else's goal (expect 403)"
  try {
    $respOther = Try-Update-Goal $playerSess ([int]$other.PlayerGoalId) 123
    Write-Fail ("Unexpected success updating other's goal: " + [int]$respOther.StatusCode)
    exit 1
  } catch {
    if ($_.Exception.Response) {
      $status2 = $_.Exception.Response.StatusCode.value__
      if ($status2 -ne 403) { Write-Fail ("Expected 403, got $status2"); exit 1 } else { Write-Ok "Player blocked from updating other's goal (403)" }
    } else { Write-Fail "No response for player updating other's goal"; exit 1 }
  }
}

Write-Host "\nAll smoke tests passed" -ForegroundColor Green
exit 0
