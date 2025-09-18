# 🐾 PetShop Manager API

API RESTful para o sistema de gestão de um pet shop, desenvolvida com .NET 8 e C#, seguindo os princípios da Clean Architecture e Domain-Driven Design (DDD).

Este projeto serve como o back-end completo para a administração de clientes (donos), pets, agendamento de serviços, controle financeiro e muito mais. Foi construído com foco em código limpo, testável e escalável.

---

## ✨ Features

* **Gestão de Donos (Owners):** CRUD completo com busca por múltiplos parâmetros.
* **Soft Delete:** Registros não são permanentemente deletados, permitindo recuperação e auditoria.
* **Endpoints Administrativos:** Rotas para visualizar todos os registros, incluindo os "deletados".
* **Validação Rica:** Lógica de negócio e validações encapsuladas no Domínio para garantir a consistência dos dados.
* _(Em breve: Gestão de Pets, Agendamento de Serviços, etc.)_

---

## 🚀 Tecnologias e Padrões Utilizados

Este projeto foi construído com as seguintes tecnologias e conceitos de arquitetura:

* **Framework:** .NET 8, ASP.NET Core
* **Linguagem:** C# 12
* **Segurança:** Autenticação com JWT (JSON Web Tokens)
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
    * **Domain-Driven Design (DDD):** Uso de Entidades ricas, Value Objects (`PhoneNumber`) e Factory Methods (`Owner.Create`).
    * **Repository Pattern & Unit of Work:** Abstração da camada de acesso a dados.
    * **API RESTful:** Design de endpoints seguindo as melhores práticas, com uso de DTOs e status codes HTTP semânticos.
    * **Injeção de Dependência (DI):** Usada extensivamente em todo o projeto.

---

## 🏁 Começando (Getting Started)

Siga os passos abaixo para configurar e executar o projeto localmente.

### Pré-requisitos

Antes de começar, garanta que você tem os seguintes softwares instalados:

* **[.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)**
* **[PostgreSQL](https://www.postgresql.org/download/)**
* **EF Core Tools:** A ferramenta de linha de comando do Entity Framework. Instale-a (ou atualize-a) com o seguinte comando no seu terminal:
  ```bash
  dotnet tool install --global dotnet-ef
  ```
* **Datadog (Opcional, para observabilidade):**
  * Uma conta no [Datadog](https://www.datadoghq.com/) (o free trial de 14 dias é suficiente).
  * O **Datadog Agent** instalado e rodando na sua máquina.
  * O **Datadog .NET Tracer**, que fornece o comando `dd-trace` e a instrumentação automática. Siga as instruções de instalação para o seu sistema operacional na [documentação oficial](https://docs.datadoghq.com/tracing/trace_collection/dd_libraries/dotnet-core/).
* Uma IDE de sua preferência (Visual Studio, VS Code, Rider)

### 🔧 Instalação e Configuração

**1. Clone o repositório:**
```bash
git clone https://github.com/BryanCarlos/API_Banho_Tosa.git
```

**2. Navegue para a pasta do projeto da API:**
```bash
# A partir da raiz do repositório clonado
cd API_Banho_Tosa/API_Banho_Tosa
```
*(**Nota:** Os comandos a seguir devem ser executados de dentro desta pasta, que contém o arquivo `.csproj`)*

**3. Inicialize os User Secrets:**
Este passo cria um "cofre" local para guardar suas chaves secretas, como a connection string, de forma segura.
```bash
dotnet user-secrets init
```

**4. Configure a Conexão com o Banco de Dados:**

* **Banco de Dados:** Adicione sua connection string ao "cofre". Substitua os valores pelos do seu banco de dados PostgreSQL local.
```bash
dotnet user-secrets set "ConnectionStrings:BanhoTosaContext" "Host=localhost;Port=5432;Database=db_pet_control;Username=seu_usuario;Password=sua_senha"
```
**Chave do Token JWT (Obrigatório):** Gere uma chave segura e a configure. Você pode usar um gerador online ou o próprio C# para criar uma chave de 64 bytes em Base64.
  ```bash
  dotnet user-secrets set "AppSettings:Token" "SUA_CHAVE_SUPER_SECRETA_GERADA_AQUI"
  ```
* **Datadog (Opcional):**: Informe a sua API Key que foi gerada a partir da criação da sua conta.
```bash
dotnet user-secrets set "Datadog:ApiKey" "sua_api_key_do_datadog"
```
*(**Nota:** A integração com o Datadog pode ser desabilitada no arquivo `appsettings.json` alterando a chave `Datadog:Enabled` para `false`.)*

**5. Aplique as Migrations do Banco de Dados:**
Este comando usará a connection string configurada para criar o banco de dados (se não existir) e todas as suas tabelas.
```bash
dotnet ef database update
```
Ao final, a estrutura completa do banco de dados estará pronta para uso.

### ▶️ Executando a Aplicação

Você pode executar a aplicação de duas formas, dependendo se você quer ou não ativar o monitoramento de performance (APM) do Datadog.

#### **Para Rodar a Aplicação (Padrão)**
Este comando inicia a API sem o rastreamento do Datadog.
```bash
dotnet run
```

#### **Para Rodar com o Datadog APM (Recomendado para Testes de Observabilidade)**
Este comando usa a ferramenta `dd-trace` para iniciar a API com o rastreamento de performance ativado. Garanta que o Datadog Agent esteja rodando na sua máquina.
```bash
dd-trace -- dotnet run --launch-profile https
```
*Após iniciar com este comando, a aplicação começará a enviar traces de APM e logs (se configurados) automaticamente para o Datadog.*

A API estará rodando nas portas configuradas no arquivo `Properties/launchSettings.json`.

A documentação interativa da API estará disponível via Swagger em sua respectiva URL (ex: `https://localhost:8080/swagger`).

### 🧪 Executando os Testes

Para executar a suíte de testes de unidade e garantir que tudo está funcionando como esperado, navegue para a pasta da solução e execute:
```bash
dotnet test
```

---

## 📄 Licença

Este projeto está sob a licença MIT. Veja o arquivo `LICENSE` para mais detalhes.
