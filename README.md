# 🐾 PetShop Manager API

API RESTful para o sistema de gestão de um pet shop, desenvolvida com .NET 8 e C#, seguindo os princípios da Clean Architecture e Domain-Driven Design (DDD).

Este projeto serve como o back-end completo para a administração de clientes (donos), pets, agendamento de serviços, controle financeiro e autenticação de usuários. Ele utiliza um **Worker Service** e o **RabbitMQ** para processar tarefas assíncronas (como envio de emails) sem bloquear a API principal, garantindo uma resposta rápida para o usuário.

---

## ✨ Features

* **Autenticação e Autorização:** Sistema completo com JWT, Refresh Tokens e autorização baseada em Roles (Papéis).
* **Fluxo de Confirmação de Email:** Processo de registro com envio de email de confirmação para ativação da conta.
* **Processamento Assíncrono:** Uso do RabbitMQ para enfileirar tarefas de longa duração (envio de emails), garantindo que a API permaneça rápida.
* **Gestão de Donos, Pets e Raças:** CRUDs completos com validações ricas.
* **Observabilidade com Datadog:** Integração completa com APM (Tracing) e Logs Estruturados.
* **Soft Delete:** Registros não são permanentemente deletados, permitindo recuperação e auditoria.

---

## 🚀 Tecnologias e Padrões Utilizados

* **Framework:** .NET 8, ASP.NET Core
* **Linguagem:** C# 12
* **Segurança:** Autenticação com JWT (JSON Web Tokens)
* **Mensageria:** RabbitMQ
* **Banco de Dados:** PostgreSQL
* **ORM:** Entity Framework Core 8
* **Observabilidade:**
    * **Monitoramento de Performance (APM):** Datadog
    * **Logging Estruturado:** Serilog com Sink para Datadog
* **Testes:**
    * **Testes de Unidade:** xUnit
    * **Mocks:** Moq
* **Padrões de Arquitetura e Design:**
    * **Clean Architecture:** Separação clara das camadas de Domínio, Aplicação, Infraestrutura e API.
    * **Arquitetura Orientada a Eventos (EDA):** Desacoplamento de serviços usando um Message Broker.
    * **Domain-Driven Design (DDD):** Uso de Entidades ricas, Value Objects (`PhoneNumber`, `Email`) e Factory Methods.
    * **Repository Pattern & Unit of Work:** Abstração da camada de acesso a dados.
    * **API RESTful:** Design de endpoints seguindo as melhores práticas, com uso de DTOs e status codes HTTP semânticos.
    * **Injeção de Dependência (DI):** Usada extensivamente em todo o projeto.

---

## 🏁 Começando (Getting Started)

Siga os passos abaixo para configurar e executar o projeto localmente.

### Pré-requisitos

Antes de começar, garanta que você tem os seguintes softwares instalados:

* **[.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)**
* **[PostgreSQL](https://www.postgresql.org/download/)** (Servidor rodando localmente)
* **[RabbitMQ Server](https://www.rabbitmq.com/install-windows.html)** (Rodando localmente ou via Docker)
* **[Ngrok](https://ngrok.com/download)** (Necessário para testar o fluxo de confirmação de email)
* **EF Core Tools:** A ferramenta de linha de comando do Entity Framework. Instale-a (ou atualize-a):
  ```bash
  dotnet tool install --global dotnet-ef
  ```
* **Datadog (Opcional):**
    * O [Datadog Agent](https://www.datadoghq.com/) instalado.
    * O [Datadog .NET Tracer](https://docs.datadoghq.com/tracing/trace_collection/dd_libraries/dotnet-core/).
* Uma IDE de sua preferência (Visual Studio, VS Code, Rider)

### 🔧 Instalação e Configuração

Este projeto é composto por múltiplos serviços. Você precisará configurar a **API** e o **Worker**.

#### **1. Configurando a API (`API_Banho_Tosa`)**

1.  **Navegue para a pasta da API:**
    ```bash
    cd API_Banho_Tosa/API_Banho_Tosa
    ```
2.  **Inicialize os User Secrets:**
    ```bash
    dotnet user-secrets init
    ```
3.  **Configure os Segredos da API:** Adicione seus segredos de banco, token para geração de JWTs e Datadog.
    ```bash
    dotnet user-secrets set "AppSettings:Token" "SUA_CHAVE_SUPER_SECRETA_GERADA_AQUI"
    dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=db_pet_control;Username=seu_usuario;Password=sua_senha"
    dotnet user-secrets set "Datadog:ApiKey" "sua_api_key_do_datadog"
    ```
4.  **Aplique as Migrations do Banco de Dados:**
    ```bash
    dotnet ef database update
    ```

#### **2. Configurando o Worker (`PetShop.Worker.Email`)**

1.  **Navegue para a pasta do Worker:**
    ```bash
    cd PetShop.Worker.Email
    ```
2.  **Inicialize os User Secrets:**
    ```bash
    dotnet user-secrets init
    ```
3.  **Configure os Segredos do Worker:** Adicione as credenciais do seu email SMTP.
    ```bash
    dotnet user-secrets set "SmtpSettings:EmailPassword" "sua_senha_secreta_de_email_aqui"
    ```
4.  **Configure a URL do Ngrok:** Para que o Worker possa criar os links de confirmação, ele precisa saber a URL pública da sua API.
    * Inicie o Ngrok em um terminal separado (veja a seção "Executando" abaixo).
    * Copie a URL `https://...` gerada por ele.
    * Cole essa URL no arquivo `appsettings.json` do seu projeto **Worker**:
    ```json
    // Em PetShop.Worker.Email/appsettings.json
    {
      "ApiSettings": {
        "BaseUrl": "https://[SUA-URL-DO-NGROK-AQUI].ngrok-free.dev"
      },
      // ... outras configurações
    }
    ```

### ▶️ Executando o Projeto Completo

Para testar o fluxo de registro de ponta a ponta, você precisará de **3 terminais** rodando simultaneamente.

**Terminal 1: Inicie o Ngrok**
(Necessário para que o email enviado pelo Worker possa chamar sua API local)
```bash
# Aponta o Ngrok para a porta HTTPS da sua API (verifique seu launchSettings.json)
ngrok http https://localhost:8080 --host-header="localhost:8080"
```
*Copie a URL `https_...` que ele gerar.*

**Terminal 2: Inicie o Worker de Email**
(Não se esqueça de atualizar o `appsettings.json` com a URL do Ngrok)
```bash
cd API_Banho_Tosa/PetShop.Worker.Email
dotnet run
```
*Você deverá ver os logs indicando que ele está conectado ao RabbitMQ e esperando por mensagens.*

**Terminal 3: Inicie a API Principal**
```bash
cd API_Banho_Tosa/API_Banho_Tosa
# Rode com o dd-trace para uma observabilidade completa
dd-trace -- dotnet run --launch-profile https
```
*A API estará rodando e o Swagger estará disponível em `https://localhost:8080/swagger`.*

**Agora, o sistema está pronto!**
1.  Vá ao Swagger e use o endpoint `POST /api/auth/register` para criar um novo usuário.
2.  Observe o **Terminal 3 (API)** registrar a publicação da mensagem no RabbitMQ.
3.  Observe o **Terminal 2 (Worker)** pegar a mensagem, logar o processamento e o envio do email.
4.  Verifique seu email, clique no link de confirmação (que usa a URL do Ngrok).
5.  O link baterá na sua **API** e seu usuário será ativado.

### 🧪 Executando os Testes

Para executar a suíte de testes de unidade, navegue para a pasta da solução e execute:
```bash
dotnet test
```

---

## 📄 Licença

Este projeto está sob a licença MIT. Veja o arquivo `LICENSE` para mais detalhes.