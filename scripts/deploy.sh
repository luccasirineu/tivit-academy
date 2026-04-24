#!/bin/bash

# Script de deploy automatizado para Tivit Academy
# Uso: ./scripts/deploy.sh [opcoes]

set -e

# Cores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Funções auxiliares
log_info() {
    echo -e "${GREEN}[INFO]${NC} $1"
}

log_warn() {
    echo -e "${YELLOW}[WARN]${NC} $1"
}

log_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Verificar se Docker está rodando
check_docker() {
    if ! docker info > /dev/null 2>&1; then
        log_error "Docker não está rodando. Inicie o Docker Desktop e tente novamente."
        exit 1
    fi
    log_info "Docker está rodando ✓"
}

# Verificar se arquivo .env existe
check_env() {
    if [ ! -f .env ]; then
        log_warn "Arquivo .env não encontrado. Criando a partir do .env.example..."
        cp .env.example .env
        log_warn "Configure o arquivo .env antes de continuar!"
        exit 1
    fi
    log_info "Arquivo .env encontrado ✓"
}

# Pull das mudanças do GitHub
pull_changes() {
    log_info "Fazendo pull das mudanças do GitHub..."
    git pull origin main || {
        log_warn "Não foi possível fazer pull. Continuando com código local..."
    }
}

# Parar containers
stop_containers() {
    log_info "Parando containers..."
    docker-compose down
}

# Rebuild imagens
rebuild_images() {
    local no_cache=$1
    if [ "$no_cache" = "true" ]; then
        log_info "Rebuilding imagens (sem cache)..."
        docker-compose build --no-cache
    else
        log_info "Rebuilding imagens..."
        docker-compose build
    fi
}

# Subir containers
start_containers() {
    log_info "Iniciando containers..."
    docker-compose up -d
}

# Aguardar SQL Server
wait_sqlserver() {
    log_info "Aguardando SQL Server inicializar..."
    sleep 10
    
    local max_attempts=30
    local attempt=1
    
    while [ $attempt -le $max_attempts ]; do
        if docker-compose exec -T sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "${SA_PASSWORD}" -C -Q "SELECT 1" > /dev/null 2>&1; then
            log_info "SQL Server está pronto ✓"
            return 0
        fi
        log_warn "Tentativa $attempt/$max_attempts - Aguardando SQL Server..."
        sleep 2
        attempt=$((attempt + 1))
    done
    
    log_error "SQL Server não inicializou no tempo esperado"
    return 1
}

# Executar migrations
run_migrations() {
    log_info "Executando migrations..."
    docker-compose exec -T backend dotnet ef database update || {
        log_error "Falha ao executar migrations"
        return 1
    }
    log_info "Migrations executadas com sucesso ✓"
}

# Mostrar logs
show_logs() {
    log_info "Mostrando logs (Ctrl+C para sair)..."
    docker-compose logs -f
}

# Mostrar status
show_status() {
    log_info "Status dos containers:"
    docker-compose ps
    echo ""
    log_info "Aplicação disponível em:"
    echo "  Frontend: http://localhost:3000"
    echo "  Backend:  http://localhost:5000"
    echo "  Swagger:  http://localhost:5000/swagger"
}

# Deploy completo
full_deploy() {
    local no_cache=$1
    
    log_info "=== Iniciando deploy completo ==="
    
    check_docker
    check_env
    pull_changes
    stop_containers
    rebuild_images "$no_cache"
    start_containers
    wait_sqlserver
    run_migrations
    show_status
    
    log_info "=== Deploy concluído com sucesso! ==="
}

# Deploy rápido (sem pull, sem parar)
quick_deploy() {
    log_info "=== Iniciando deploy rápido ==="
    
    check_docker
    check_env
    
    log_info "Atualizando containers..."
    docker-compose up -d --build
    
    show_status
    
    log_info "=== Deploy rápido concluído! ==="
}

# Deploy apenas backend
backend_deploy() {
    log_info "=== Atualizando apenas backend ==="
    
    check_docker
    docker-compose up -d --build backend
    sleep 5
    run_migrations
    
    log_info "=== Backend atualizado! ==="
}

# Deploy apenas frontend
frontend_deploy() {
    log_info "=== Atualizando apenas frontend ==="
    
    check_docker
    docker-compose up -d --build frontend
    
    log_info "=== Frontend atualizado! ==="
}

# Menu de ajuda
show_help() {
    cat << EOF
Script de Deploy - Tivit Academy

Uso: ./scripts/deploy.sh [comando] [opcoes]

Comandos:
  full          Deploy completo (pull + rebuild + migrations)
  full --no-cache   Deploy completo sem cache
  quick         Deploy rápido (apenas rebuild)
  backend       Atualiza apenas backend
  frontend      Atualiza apenas frontend
  logs          Mostra logs dos containers
  status        Mostra status dos containers
  stop          Para todos os containers
  help          Mostra esta mensagem

Exemplos:
  ./scripts/deploy.sh full
  ./scripts/deploy.sh full --no-cache
  ./scripts/deploy.sh quick
  ./scripts/deploy.sh backend
  ./scripts/deploy.sh logs

EOF
}

# Main
main() {
    local command=${1:-help}
    local option=$2
    
    case $command in
        full)
            if [ "$option" = "--no-cache" ]; then
                full_deploy true
            else
                full_deploy false
            fi
            ;;
        quick)
            quick_deploy
            ;;
        backend)
            backend_deploy
            ;;
        frontend)
            frontend_deploy
            ;;
        logs)
            show_logs
            ;;
        status)
            show_status
            ;;
        stop)
            stop_containers
            ;;
        help|*)
            show_help
            ;;
    esac
}

main "$@"
