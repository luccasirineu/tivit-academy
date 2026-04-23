# Script de deploy automatizado para Tivit Academy (Windows PowerShell)
# Uso: .\scripts\deploy.ps1 [comando]

param(
    [Parameter(Position=0)]
    [string]$Command = "help",
    
    [Parameter(Position=1)]
    [switch]$NoCache
)

# Cores para output
function Write-Info {
    param([string]$Message)
    Write-Host "[INFO] $Message" -ForegroundColor Green
}

function Write-Warn {
    param([string]$Message)
    Write-Host "[WARN] $Message" -ForegroundColor Yellow
}

function Write-Error-Custom {
    param([string]$Message)
    Write-Host "[ERROR] $Message" -ForegroundColor Red
}

# Verificar se Docker está rodando
function Test-Docker {
    try {
        docker info | Out-Null
        Write-Info "Docker está rodando ✓"
        return $true
    } catch {
        Write-Error-Custom "Docker não está rodando. Inicie o Docker Desktop e tente novamente."
        exit 1
    }
}

# Verificar se arquivo .env existe
function Test-EnvFile {
    if (-not (Test-Path .env)) {
        Write-Warn "Arquivo .env não encontrado. Criando a partir do .env.example..."
        Copy-Item .env.example .env
        Write-Warn "Configure o arquivo .env antes de continuar!"
        exit 1
    }
    Write-Info "Arquivo .env encontrado ✓"
}

# Pull das mudanças do GitHub
function Get-Changes {
    Write-Info "Fazendo pull das mudanças do GitHub..."
    try {
        git pull origin main
    } catch {
        Write-Warn "Não foi possível fazer pull. Continuando com código local..."
    }
}

# Parar containers
function Stop-Containers {
    Write-Info "Parando containers..."
    docker-compose down
}

# Rebuild imagens
function Build-Images {
    param([bool]$UseNoCache)
    
    if ($UseNoCache) {
        Write-Info "Rebuilding imagens (sem cache)..."
        docker-compose build --no-cache
    } else {
        Write-Info "Rebuilding imagens..."
        docker-compose build
    }
}

# Subir containers
function Start-Containers {
    Write-Info "Iniciando containers..."
    docker-compose up -d
}

# Aguardar SQL Server
function Wait-SqlServer {
    Write-Info "Aguardando SQL Server inicializar..."
    Start-Sleep -Seconds 10
    
    $maxAttempts = 30
    $attempt = 1
    
    while ($attempt -le $maxAttempts) {
        try {
            $result = docker-compose exec -T sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$env:SA_PASSWORD" -C -Q "SELECT 1" 2>&1
            if ($LASTEXITCODE -eq 0) {
                Write-Info "SQL Server está pronto ✓"
                return $true
            }
        } catch {}
        
        Write-Warn "Tentativa $attempt/$maxAttempts - Aguardando SQL Server..."
        Start-Sleep -Seconds 2
        $attempt++
    }
    
    Write-Error-Custom "SQL Server não inicializou no tempo esperado"
    return $false
}

# Executar migrations
function Invoke-Migrations {
    Write-Info "Executando migrations..."
    try {
        docker-compose exec -T backend dotnet ef database update
        Write-Info "Migrations executadas com sucesso ✓"
        return $true
    } catch {
        Write-Error-Custom "Falha ao executar migrations"
        return $false
    }
}

# Mostrar logs
function Show-Logs {
    Write-Info "Mostrando logs (Ctrl+C para sair)..."
    docker-compose logs -f
}

# Mostrar status
function Show-Status {
    Write-Info "Status dos containers:"
    docker-compose ps
    Write-Host ""
    Write-Info "Aplicação disponível em:"
    Write-Host "  Frontend: http://localhost:3000"
    Write-Host "  Backend:  http://localhost:5000"
    Write-Host "  Swagger:  http://localhost:5000/swagger"
}

# Deploy completo
function Start-FullDeploy {
    param([bool]$UseNoCache)
    
    Write-Info "=== Iniciando deploy completo ==="
    
    Test-Docker
    Test-EnvFile
    Get-Changes
    Stop-Containers
    Build-Images -UseNoCache $UseNoCache
    Start-Containers
    Wait-SqlServer
    Invoke-Migrations
    Show-Status
    
    Write-Info "=== Deploy concluído com sucesso! ==="
}

# Deploy rápido
function Start-QuickDeploy {
    Write-Info "=== Iniciando deploy rápido ==="
    
    Test-Docker
    Test-EnvFile
    
    Write-Info "Atualizando containers..."
    docker-compose up -d --build
    
    Show-Status
    
    Write-Info "=== Deploy rápido concluído! ==="
}

# Deploy apenas backend
function Start-BackendDeploy {
    Write-Info "=== Atualizando apenas backend ==="
    
    Test-Docker
    docker-compose up -d --build backend
    Start-Sleep -Seconds 5
    Invoke-Migrations
    
    Write-Info "=== Backend atualizado! ==="
}

# Deploy apenas frontend
function Start-FrontendDeploy {
    Write-Info "=== Atualizando apenas frontend ==="
    
    Test-Docker
    docker-compose up -d --build frontend
    
    Write-Info "=== Frontend atualizado! ==="
}

# Menu de ajuda
function Show-Help {
    Write-Host @"
Script de Deploy - Tivit Academy

Uso: .\scripts\deploy.ps1 [comando] [-NoCache]

Comandos:
  full          Deploy completo (pull + rebuild + migrations)
  full -NoCache Deploy completo sem cache
  quick         Deploy rápido (apenas rebuild)
  backend       Atualiza apenas backend
  frontend      Atualiza apenas frontend
  logs          Mostra logs dos containers
  status        Mostra status dos containers
  stop          Para todos os containers
  help          Mostra esta mensagem

Exemplos:
  .\scripts\deploy.ps1 full
  .\scripts\deploy.ps1 full -NoCache
  .\scripts\deploy.ps1 quick
  .\scripts\deploy.ps1 backend
  .\scripts\deploy.ps1 logs

"@
}

# Main
switch ($Command.ToLower()) {
    "full" {
        Start-FullDeploy -UseNoCache $NoCache
    }
    "quick" {
        Start-QuickDeploy
    }
    "backend" {
        Start-BackendDeploy
    }
    "frontend" {
        Start-FrontendDeploy
    }
    "logs" {
        Show-Logs
    }
    "status" {
        Show-Status
    }
    "stop" {
        Stop-Containers
    }
    default {
        Show-Help
    }
}
