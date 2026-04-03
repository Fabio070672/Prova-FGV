# CRUD de Livros - Implementação Clean Architecture

## ? Implementação Completa

O CRUD completo para a entidade **Book** foi criado seguindo rigorosamente os princípios de Clean Architecture e CQRS.

## ?? Estrutura Criada

### 1. Infrastructure Layer

#### **Repository Implementation**
?? `src/FGV.Infrastructure/Repositories/BookRepository.cs`
- Herda de `Repository<Book, BookId>`
- Implementa `IBookRepository`
- Método adicional: `GetAllAsync()` com filtro de ativos e ordenação por título

#### **Entity Configuration (EF Core)**
?? `src/FGV.Infrastructure/Configurations/Books/BookConfiguration.cs`
- Configuração Fluent API
- Tabela: `books` (snake_case)
- Colunas: `id`, `title`, `author`, `edition`, `created_at`, etc.
- Índices em `title` e `author` para performance
- Conversão de `BookId` (strongly-typed)

#### **DbContext**
?? `src/FGV.Infrastructure/ApplicationDbContext.cs`
- ? `DbSet<Book> Books` adicionado

#### **Dependency Injection**
?? `src/FGV.Infrastructure/DependencyInjection.cs`
- ? `IBookRepository` registrado com `BookRepository`

---

### 2. Application Layer

#### **Response DTO**
?? `src/FGV.Application/Books/BookResponse.cs`
```csharp
public sealed record BookResponse(
    string Id,
    string Title,
    string Author,
    int Edition,
    bool Active,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
```

#### **Create (Command)**
?? `src/FGV.Application/Books/Create/`
- **CreateBookCommand.cs** - Command com Title, Author, Edition
- **CreateBookHandler.cs** - Handler que cria livro e persiste
- **CreateBookValidator.cs** - Validação com FluentValidation
  - Title: obrigatório, máx 500 caracteres
  - Author: obrigatório, máx 300 caracteres
  - Edition: > 0 e ? ano atual

#### **Update (Command)**
?? `src/FGV.Application/Books/Update/`
- **UpdateBookCommand.cs** - Command com Id + dados
- **UpdateBookHandler.cs** - Handler que atualiza livro existente
- **UpdateBookValidator.cs** - Validação completa

#### **Delete (Command)**
?? `src/FGV.Application/Books/Delete/`
- **DeleteBookCommand.cs** - Command com Id
- **DeleteBookHandler.cs** - Soft delete (Deactivate)

#### **GetById (Query)**
?? `src/FGV.Application/Books/GetById/`
- **GetBookByIdQuery.cs** - Query com Id
- **GetBookByIdHandler.cs** - Retorna BookResponse ou NotFound

#### **GetAll (Query)**
?? `src/FGV.Application/Books/GetAll/`
- **GetAllBooksQuery.cs** - Query sem parâmetros
- **GetAllBooksHandler.cs** - Retorna lista de BookResponse

#### **Dependency Injection**
?? `src/FGV.Application/DependencyInjection.cs`
- ? Todos os 5 handlers registrados

---

### 3. API Layer

#### **Request DTOs**
?? `src/FGV.Api/Controllers/Books/`
- **CreateBookRequest.cs** - DTO para criação
- **UpdateBookRequest.cs** - DTO para atualização

#### **Controller**
?? `src/FGV.Api/Controllers/Books/BooksController.cs`

**Endpoints implementados:**

| Método | Rota | Ação | Status de Retorno |
|--------|------|------|-------------------|
| POST | `/api/books` | Criar livro | 201 Created |
| GET | `/api/books/{id}` | Buscar por ID | 200 OK / 404 Not Found |
| GET | `/api/books` | Listar todos | 200 OK |
| PUT | `/api/books/{id}` | Atualizar livro | 204 No Content / 400 Bad Request |
| DELETE | `/api/books/{id}` | Deletar (soft delete) | 204 No Content / 404 Not Found |

---

## ?? Conformidade com Clean Architecture

### ? Checklist de Qualidade

- [x] Repository **interface** no Domain layer
- [x] Repository **implementation** no Infrastructure layer
- [x] Entity com factory pattern (construtor privado)
- [x] Entity com strongly-typed ID
- [x] Entity com método Update
- [x] Entity com Activate/Deactivate
- [x] Handlers usam Result pattern
- [x] Validators com FluentValidation
- [x] EF Configuration com Fluent API
- [x] Table names em snake_case
- [x] Column names em snake_case
- [x] Controller usa DI para handlers
- [x] Controller retorna status HTTP apropriados
- [x] Sem lógica de negócio nos controllers
- [x] Async/await em todas as camadas
- [x] CancellationToken passado em todas as camadas
- [x] Repository registrado em Infrastructure DI
- [x] Handlers registrados em Application DI
- [x] DbSet adicionado ao ApplicationDbContext

### ? Dependency Rules

```
API Layer (BooksController)
    ? depends on
Application Layer (Handlers, Commands, Queries)
    ? depends on
Domain Layer (Book, IBookRepository)
    ? depends on
SharedKernel (Entity, Result)
```

? Infrastructure implementa interfaces do Domain
? Application não depende de Infrastructure
? Domain não depende de nada (exceto SharedKernel)

---

## ?? Pacotes NuGet Adicionados

- **FGV.Application**: FluentValidation (12.1.1)
- **FGV.Infrastructure**: EntityFrameworkCore, Npgsql, EF Design (já instalados)

---

## ?? Como Usar

### 1. Criar Migration

```bash
cd src/FGV.Infrastructure
dotnet ef migrations add AddBookEntity --startup-project ../FGV.Api
```

### 2. Atualizar Database

```bash
dotnet ef database update --startup-project ../FGV.Api
```

### 3. Executar API

```bash
cd src/FGV.Api
dotnet run
```

### 4. Testar Endpoints (Swagger)

Acesse: `https://localhost:5001/swagger`

---

## ?? Exemplos de Uso dos Endpoints

### **Criar Livro**
```http
POST /api/books
Content-Type: application/json

{
  "title": "Java How to Program",
  "author": "Deitel & Deitel",
  "edition": 2007
}
```

**Resposta**: `201 Created` com ID do livro

### **Buscar por ID**
```http
GET /api/books/{id}
```

**Resposta**: 
```json
{
  "id": "123e4567-e89b-12d3-a456-426614174000",
  "title": "Java How to Program",
  "author": "Deitel & Deitel",
  "edition": 2007,
  "active": true,
  "createdAt": "2024-04-02T20:30:00Z",
  "updatedAt": null
}
```

### **Listar Todos**
```http
GET /api/books
```

**Resposta**: Array de BookResponse

### **Atualizar Livro**
```http
PUT /api/books/{id}
Content-Type: application/json

{
  "title": "Java How to Program - Updated",
  "author": "Deitel & Deitel",
  "edition": 2008
}
```

**Resposta**: `204 No Content`

### **Deletar Livro (Soft Delete)**
```http
DELETE /api/books/{id}
```

**Resposta**: `204 No Content`

---

## ?? Validações Implementadas

### CreateBookCommand / UpdateBookCommand

1. **Title**:
   - ? Não pode ser vazio
   - ? Máximo 500 caracteres

2. **Author**:
   - ? Não pode ser vazio
   - ? Máximo 300 caracteres

3. **Edition**:
   - ? Deve ser maior que 0
   - ? Não pode ser no futuro (ano atual é o máximo)

---

## ??? Schema do Banco de Dados

### Tabela: `books`

| Coluna | Tipo | Nullable | Descrição |
|--------|------|----------|-----------|
| `id` | varchar(36) | NOT NULL | Primary Key |
| `title` | varchar(500) | NOT NULL | Título do livro |
| `author` | varchar(300) | NOT NULL | Autor do livro |
| `edition` | int | NOT NULL | Ano de edição |
| `created_at` | timestamp | NOT NULL | Data de criação |
| `created_by` | uuid | NULL | Usuário criador |
| `updated_at` | timestamp | NULL | Data de atualização |
| `updated_by` | uuid | NULL | Usuário atualizador |
| `active` | boolean | NOT NULL | Status ativo/inativo |

**Índices**:
- `ix_books_title` - Index em title
- `ix_books_author` - Index em author

---

## ?? Padrões Aplicados

### 1. **CQRS (Command Query Responsibility Segregation)**
- Commands: Create, Update, Delete
- Queries: GetById, GetAll
- Separação clara entre leitura e escrita

### 2. **Repository Pattern**
- Interface no Domain
- Implementação no Infrastructure
- Abstração de acesso a dados

### 3. **Result Pattern**
- Retorno tipado com sucesso/falha
- Tratamento de erros sem exceções
- Error records padronizados

### 4. **Factory Pattern**
- Método `Create()` estático
- Construtor privado
- Encapsulamento de lógica de criação

### 5. **Dependency Injection**
- Handlers injetados nos controllers
- Repositories injetados nos handlers
- Inversão de controle completa

### 6. **Validation Pattern**
- FluentValidation
- Validação declarativa
- Separação de concerns

---

## ? Build Status

```
? Build successful
? Sem erros de compilação
? Todas as camadas integradas corretamente
```

---

## ?? Próximos Passos Sugeridos

1. ? **CRUD de Books completo** ? CONCLUÍDO!
2. ?? Implementar serviço de ordenação
3. ?? Criar SortingConfiguration CRUD
4. ?? Implementar BookSortingService
5. ?? Testes unitários
6. ?? Testes de integração

---

## ?? Documentação Relacionada

- **Clean Architecture Agent**: `.github/copilot/copilot-agents/clean-architecture-agent.md`
- **Domain Entities**: `DOMAIN-ENTITIES.md`
- **Getting Started**: `GETTING-STARTED.md`
- **Quick Reference**: `QUICK-REFERENCE.md`

---

## ?? Resumo

O CRUD completo de Books foi implementado seguindo 100% os princípios de Clean Architecture:

? **5 Endpoints RESTful**
? **3 Commands** (Create, Update, Delete)
? **2 Queries** (GetById, GetAll)
? **5 Handlers** registrados com DI
? **Validators** com FluentValidation
? **Repository** com EF Core
? **Result Pattern** para controle de erros
? **Strongly-typed IDs**
? **Soft Delete** implementado

**Tudo pronto para uso e extensão!** ??
