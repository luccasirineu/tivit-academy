# CI/CD Pipeline

Este projeto usa GitHub Actions para automação de CI/CD.

## Workflows

### ci-cd.yml
Pipeline principal que executa em cada push/PR:

1. **Testes Backend** - Roda testes do .NET
2. **Testes Frontend** - Roda testes e linter do React
3. **Build e Push** - Cria imagens Docker e envia para GitHub Container Registry
4. **Deploy** - Deploy automático para servidor (apenas branch main)

## Configuração necessária

### Secrets do GitHub
Adicione em: Settings → Secrets and variables → Actions

```
SERVER_HOST       - IP ou domínio do servidor
SERVER_USER       - Usuário SSH
SSH_PRIVATE_KEY   - Chave privada SSH para acesso ao servidor
```

### Habilitar GitHub Container Registry
1. Settings → Packages
2. Tornar o pacote público ou configurar acesso

## Como funciona

### Em Pull Requests
- Roda apenas testes
- Não faz deploy

### Em Push para main
- Roda testes
- Build das imagens Docker
- Push para registry
- Deploy automático para servidor

## Ambientes

Você pode adicionar ambientes diferentes:

```yaml
# Exemplo: Deploy para staging
deploy-staging:
  if: github.ref == 'refs/heads/develop'
  environment: staging
  # ...

# Exemplo: Deploy para produção com aprovação
deploy-production:
  if: github.ref == 'refs/heads/main'
  environment: production  # Requer aprovação manual
  # ...
```

## Monitoramento

Veja o status dos workflows em:
- Actions tab no GitHub
- Badge no README: `![CI/CD](https://github.com/seu-usuario/seu-repo/workflows/CI%2FCD%20Pipeline/badge.svg)`
