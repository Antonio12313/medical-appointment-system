# Sistema de Agendamento Médico

Sistema de agendamento médico desenvolvido em ASP.NET Core e Nuxt.js, permitindo que médicos gerenciem seus horários e pacientes realizem agendamentos.

## Pré-requisitos

- Docker Desktop instalado ([https://www.docker.com/products/docker-desktop](https://www.docker.com/products/docker-desktop))
- Git instalado
- Porta 3000 (frontend) e 8080 (backend) disponíveis

## Tecnologias Utilizadas

### Backend
- .NET 8.0
- ASP.NET Core Web API
- Entity Framework Core 8.0.10
- PostgreSQL 16
- JWT Authentication
- BCrypt para hash de senhas

### Frontend
- Nuxt.js 3.13.0
- Vue.js 3.5.21
- Tailwind CSS 3.4.1
- Node.js 20

## Instruções para Rodar o Sistema

### 1. Clone o repositório

```bash
git clone https://github.com/Antonio12313/medical-appointment-system
```

### 2. Configure as variáveis de ambiente

Crie um arquivo `.env` na raiz do projeto com o seguinte conteúdo:

```env
# Database
POSTGRES_USER=postgres
POSTGRES_PASSWORD=postgres123
POSTGRES_DB=medical_appointment

# API
JWT_KEY=sua-chave-api-com-pelo-menos-32caracteres

# Frontend
API_BASE_URL=http://localhost:8080
```

### 3. Execute o Docker Compose

```bash
docker-compose up -d --build
```

Este comando irá:
- Criar e iniciar o container do PostgreSQL
- Criar e iniciar o container da API Backend (.NET)
- Criar e iniciar o container do Frontend (Nuxt.js)

### 4. Aplique as Migrations do Banco de Dados

Aguarde para o banco inicializar, depois execute:

```bash
docker exec -it medical-appointment-api dotnet ef database update
```

Este comando criará todas as tabelas necessárias no banco de dados.

### 5. Acesse o sistema

Aguarde alguns segundos para que todos os serviços estabilizem.

- **Frontend (Interface do Usuário):** [http://localhost:3000](http://localhost:3000)
- **Backend API (Swagger):** [http://localhost:8080/swagger](http://localhost:8080/swagger)

## Configuração do Banco de Dados

O banco de dados PostgreSQL é configurado automaticamente através do Docker Compose com as seguintes credenciais:

- **Host:** postgres (dentro do container) / localhost:5432 (acesso externo)
- **Database:** medical_appointment
- **Username:** postgres
- **Password:** postgres123

As migrations devem ser aplicadas manualmente após a inicialização dos containers (ver passo 4 das instruções).

### Estrutura do Banco

O sistema possui as seguintes tabelas principais:
- **Usuarios:** Dados de login e perfil
- **Medicos:** Informações específicas de médicos (CRM, Especialidade)
- **Pacientes:** Informações específicas de pacientes (CPF, Telefone)
- **HorariosDisponiveis:** Horários cadastrados pelos médicos
- **Agendamentos:** Consultas agendadas pelos pacientes

## Uso do Sistema

### Cadastro de Usuário

1. Acesse [http://localhost:3000](http://localhost:3000)
2. Clique em "Cadastrar-se"
3. Escolha o tipo de conta (Médico ou Paciente)
4. Preencha os dados solicitados
5. Faça login após o cadastro

### Fluxo do Médico

1. Faça login como médico
2. No dashboard, clique em "Criar Horário"
3. Selecione data e hora de início e fim
4. Visualize seus horários cadastrados
5. Acompanhe os agendamentos realizados
6. Cancele agendamentos se necessário

### Fluxo do Paciente

1. Faça login como paciente
2. Na aba "Agendar Consulta":
    - Selecione um médico
    - Escolha um horário disponível
    - Confirme o agendamento
3. Visualize seus agendamentos na aba "Meus Agendamentos"
4. Cancele agendamentos se necessário

## Decisões Técnicas

### Arquitetura

- **Padrão Repository/Service:** Separação de responsabilidades entre camadas
- **DTOs:** Transferência de dados otimizada e desacoplada dos modelos
- **Migrations:** Controle de versão do banco de dados
- **Docker Multi-stage Build:** Otimização do tamanho das imagens

### Autenticação e Autorização

- **JWT (JSON Web Tokens):** Autenticação stateless
- **Claims-based Authorization:** Controle de acesso baseado em perfis
- **BCrypt:** Hash seguro de senhas

### Frontend

- **SSR desabilitado:** Melhor performance para SPA
- **Middleware de autenticação:** Proteção de rotas
- **Composables:** Reutilização de lógica
- **Tailwind CSS:** Estilização rápida e responsiva

### Segurança Implementada

- Hash de senhas com BCrypt
- Tokens JWT com expiração de 7 dias
- Validação de entrada nos DTOs
- Proteção contra SQL Injection (Entity Framework parameterizado)
- CORS configurado
- Autorização baseada em claims

## Estrutura do Projeto

```
.
├── Controllers/          # Endpoints da API
├── DTOs/                # Data Transfer Objects
├── Enums/               # Enumerações
├── Models/              # Entidades do banco
├── Migrations/          # Migrations do EF Core
├── Services/            # Lógica de negócio
├── front-end/           # Aplicação Nuxt.js
│   ├── pages/          # Páginas da aplicação
│   ├── composables/    # Lógica reutilizável
│   └── middleware/     # Middleware de autenticação
├── Dockerfile          # Imagem do backend
└── README.md           # Este arquivo
```
### Erro nas migrations

Se houver erro ao aplicar migrations:

```bash
# Entre no container da API
docker exec -it medical-appointment-api bash

# Execute as migrations manualmente
dotnet ef database update
```

## Contato

Para dúvidas ou problemas, entre em contato através do repositório.