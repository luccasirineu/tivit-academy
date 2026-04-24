# Docker Setup - Tivit Academy

## Pré-requisitos

- Docker Desktop instalado
- Docker Compose instalado
- Mínimo 4GB RAM disponível

## Estrutura

```
.
├── docker-compose.yml          # Orquestração dos containers
├── backend/tivitApi/
│   ├── Dockerfile             # Build do backend .NET
│   └── .dockerignore
├── frontend/
│   ├── Dockerfile             # Build do frontend React
│   ├── nginx.conf             # Configuração do Nginx
│   └── .dockerignore
└── .env.example               # Variáveis de ambiente
```

## Como usar

### 1. Configurar variáveis de ambiente

```bash
cp .env.example .env
# Edite o .env com suas credenciais
```

### 2. Subir todos os containers

```bash
docker-compose up -d
```

### 3. Verificar status

```bash
docker-compose ps
```

### 4. Ver logs

```bash
# Todos os serviços
docker-compose logs -f

# Apenas backend
docker-compose logs -f backend

# Apenas frontend
docker-compose logs -f frontend
```

### 5. Executar migrations

```bash
docker-compose exec backend dotnet ef database update
```

## Acessar a aplicação

- **Frontend**: http://localhost:3000
- **Backend API**: http://localhost:5000
- **Swagger**: http://localhost:5000/swagger
- **SQL Server**: localhost:1433

## Scripts de Deploy Automatizado

Criamos scripts para facilitar o deploy. Escolha o script de acordo com seu sistema:

### Windows (PowerShell)
```powershell
# Deploy completo (recomendado após push no GitHub)
.\scripts\deploy.ps1 full

# Deploy completo sem cache (força rebuild total)
.\scripts\deploy.ps1 full -NoCache

# Deploy rápido (apenas rebuild)
.\scripts\deploy.ps1 quick

# Atualizar apenas backend
.\scripts\deploy.ps1 backend

# Atualizar apenas frontend
.\scripts\deploy.ps1 frontend

# Ver logs
.\scripts\deploy.ps1 logs

# Ver status
.\scripts\deploy.ps1 status
```

### Linux/Mac (Bash)
```bash
# Dar permissão de execução (primeira vez)
chmod +x scripts/deploy.sh

# Deploy completo (recomendado após push no GitHub)
./scripts/deploy.sh full

# Deploy completo sem cache
./scripts/deploy.sh full --no-cache

# Deploy rápido
./scripts/deploy.sh quick

# Atualizar apenas backend
./scripts/deploy.sh backend

# Atualizar apenas frontend
./scripts/deploy.sh frontend

# Ver logs
./scripts/deploy.sh logs
```

## Atualizar containers após mudanças no código (Manual)

### Cenário 1: Você alterou o código localmente

```bash
# Rebuild e restart apenas o serviço alterado
docker-compose up -d --build backend    # Se alterou backend
docker-compose up -d --build frontend   # Se alterou frontend

# Ou rebuild tudo
docker-compose up -d --build
```

### Cenário 2: Você fez push no GitHub e quer atualizar no servidor

```bash
# 1. Fazer pull das mudanças
git pull origin main

# 2. Parar os containers
docker-compose down

# 3. Rebuild as imagens (força reconstrução sem cache)
docker-compose build --no-cache

# 4. Subir novamente
docker-compose up -d

# 5. Verificar logs
docker-compose logs -f
```

### Cenário 3: Atualização rápida (sem limpar cache)

```bash
git pull origin main
docker-compose up -d --build
```

### Cenário 4: Atualização com migrations

```bash
# 1. Atualizar código
git pull origin main

# 2. Rebuild backend
docker-compose up -d --build backend

# 3. Executar migrations
docker-compose exec backend dotnet ef database update

# 4. Verificar
docker-compose logs -f backend
```

## Comandos úteis

### Parar containers
```bash
docker-compose down
```

### Parar e remover volumes (limpa banco de dados)
```bash
docker-compose down -v
```

### Rebuild containers
```bash
docker-compose up -d --build
```

### Rebuild sem cache (força reconstrução completa)
```bash
docker-compose build --no-cache
docker-compose up -d
```

### Atualizar apenas um serviço
```bash
docker-compose up -d --build backend
docker-compose up -d --build frontend
```

### Ver imagens criadas
```bash
docker images | grep tivit
```

### Remover imagens antigas (liberar espaço)
```bash
docker image prune -a
```

### Acessar shell do container
```bash
# Backend
docker-compose exec backend bash

# SQL Server
docker-compose exec sqlserver bash
```

### Conectar no SQL Server
```bash
docker-compose exec sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P ${SA_PASSWORD} -C
```

## Troubleshooting

### Backend não conecta no SQL Server
- Aguarde o healthcheck do SQL Server completar
- Verifique logs: `docker-compose logs sqlserver`

### Erro de memória no SQL Server
- Aumente memória do Docker Desktop para 4GB+

### Frontend não carrega
- Verifique se o build completou: `docker-compose logs frontend`
- Rebuild: `docker-compose up -d --build frontend`

### Migrations não aplicadas
```bash
docker-compose exec backend dotnet ef database update
```

## Produção

Para produção, considere:

1. Usar secrets do Docker para credenciais
2. Configurar volumes persistentes para SQL Server
3. Adicionar reverse proxy (Traefik/Nginx)
4. Configurar SSL/TLS
5. Implementar health checks
6. Usar registry privado para imagens
