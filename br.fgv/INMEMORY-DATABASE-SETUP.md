# Configuração de Banco de Dados InMemory

## ? Mudanças Implementadas

A aplicação FGV foi configurada para usar **Entity Framework Core InMemory Database** ao invés de PostgreSQL.

---

## ?? Alterações Realizadas

### 1. **Pacote NuGet Adicionado**

**Projeto**: `FGV.Infrastructure`

```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" Version="10.0.5" />
```

---

### 2. **Configuração do DbContext**

**Arquivo**: `src/FGV.Infrastructure/DependencyInjection.cs`

**Antes** (PostgreSQL):
```csharp
services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
```

**Depois** (InMemory):
```csharp
services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("FGV_BookSorting_DB"));
```

---

### 3. **DatabaseSeeder Criado**

**Arquivo**: `src/FGV.Infrastructure/Data/DatabaseSeeder.cs`

Popula automaticamente o banco InMemory com os **4 livros de teste** da documentação FGV:

| ID | Título | Autor | Edição |
|----|--------|-------|--------|
| 1 | Java How to Program | Deitel & Deitel | 2007 |
| 2 | Patterns of Enterprise Application Architecture | Martin Fowler | 2002 |
| 3 | Head First Design Patterns | Elisabeth Freeman | 2004 |
| 4 | Internet & World Wide Web: How to Program | Deitel & Deitel | 2007 |

---

### 4. **Seed Automático no Startup**

**Arquivo**: `src/FGV.Api/Program.cs`

```csharp
// Seed InMemory Database with test data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    DatabaseSeeder.SeedTestData(context);
}
```

---

## ? Vantagens do InMemory Database

### ? **1. Sem Dependências Externas**
- ? Não precisa instalar PostgreSQL
- ? Não precisa configurar connection strings
- ? Não precisa criar migrations
- ? Roda imediatamente após `dotnet run`

### ?? **2. Desenvolvimento Rápido**
- Dados resetados a cada reinicialização
- Testes isolados e reproduzíveis
- Seed automático com dados de teste

### ?? **3. Ideal para Testes**
- Performance extremamente rápida
- Não persiste dados entre execuções
- Perfeito para ambiente de desenvolvimento

### ?? **4. Portabilidade**
- Funciona em qualquer máquina
- Não requer configuração de ambiente
- Ideal para demos e apresentações

---

## ?? Como Executar

```bash
cd src/FGV.Api
dotnet run
```

**Saída esperada:**
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7091
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
```

---

## ?? Acessar a API

### **Swagger UI**
```
https://localhost:7091/swagger
```

### **Testar GetAll Books** (já virá com 4 livros)
```http
GET https://localhost:7091/api/books
```

**Resposta esperada:**
```json
[
  {
    "id": "...",
    "title": "Java How to Program",
    "author": "Deitel & Deitel",
    "edition": 2007,
    "active": true,
    "createdAt": "2024-04-02T...",
    "updatedAt": null
  },
  {
    "id": "...",
    "title": "Patterns of Enterprise Application Architecture",
    "author": "Martin Fowler",
    "edition": 2002,
    "active": true,
    "createdAt": "2024-04-02T...",
    "updatedAt": null
  },
  {
    "id": "...",
    "title": "Head First Design Patterns",
    "author": "Elisabeth Freeman",
    "edition": 2004,
    "active": true,
    "createdAt": "2024-04-02T...",
    "updatedAt": null
  },
  {
    "id": "...",
    "title": "Internet & World Wide Web: How to Program",
    "author": "Deitel & Deitel",
    "edition": 2007,
    "active": true,
    "createdAt": "2024-04-02T...",
    "updatedAt": null
  }
]
```

---

## ?? Comportamento do InMemory DB

### **A cada reinicialização da aplicação:**

1. ? Banco InMemory é criado em memória
2. ? `DatabaseSeeder` verifica se há dados
3. ? Se não houver dados, insere os 4 livros de teste
4. ? Aplicação fica pronta para uso

### **Durante a execução:**

- ? Você pode criar, atualizar, deletar livros normalmente
- ? Todos os endpoints CRUD funcionam perfeitamente
- ? Dados persistem enquanto a aplicação estiver rodando

### **Ao parar a aplicação:**

- ? Todos os dados são perdidos
- ? Próxima execução recria o banco com os 4 livros iniciais

---

## ?? Como Voltar para PostgreSQL (se necessário)

### **1. Reverter DependencyInjection.cs**

```csharp
services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
```

### **2. Criar Migration**

```bash
cd src/FGV.Infrastructure
dotnet ef migrations add InitialCreate --startup-project ../FGV.Api
dotnet ef database update --startup-project ../FGV.Api
```

### **3. Remover Seed (opcional)**

Comentar ou remover o código de seed no `Program.cs`:

```csharp
// using (var scope = app.Services.CreateScope())
// {
//     var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
//     DatabaseSeeder.SeedTestData(context);
// }
```

---

## ?? Comparação: InMemory vs PostgreSQL

| Característica | InMemory | PostgreSQL |
|----------------|----------|------------|
| **Instalação** | Não requer | Requer instalação |
| **Configuração** | Zero | Connection string |
| **Performance** | Extremamente rápida | Rápida |
| **Persistência** | Não persiste | Persiste |
| **Migrations** | Não precisa | Requer migrations |
| **Uso ideal** | Dev/Testes | Produção |
| **Portabilidade** | 100% portável | Requer servidor |

---

## ? Status Atual

```
? InMemory Database configurado
? Seed automático implementado
? 4 livros de teste carregados automaticamente
? Build successful
? Pronto para execução
```

---

## ?? Benefícios para Avaliação FGV

1. ? **Execução Imediata**: Não requer setup de banco de dados
2. ? **Dados de Teste**: 4 livros já carregados conforme documentação
3. ? **Demonstração Fácil**: Basta rodar e testar
4. ? **Foco no Código**: Avaliador pode focar na arquitetura, não em configuração
5. ? **Reproduzibilidade**: Sempre começa com o mesmo estado inicial

---

## ?? Notas Importantes

### ?? **Limitações do InMemory Database**

1. **Não suporta todas as features do PostgreSQL**:
   - Não há constraints de FK reais
   - Não há transações distribuídas
   - Não há stored procedures

2. **Dados voláteis**:
   - Perdidos ao parar a aplicação
   - Não há backup/restore

3. **Não recomendado para produção**:
   - Use apenas em desenvolvimento/testes
   - Para produção, use PostgreSQL, SQL Server, etc.

### ? **Quando usar InMemory**

- ? Desenvolvimento local
- ? Testes unitários e de integração
- ? Demos e apresentações
- ? Avaliações técnicas (como FGV)
- ? Protótipos rápidos

---

## ?? Próximos Passos

Agora você pode:

1. ? Executar a aplicação sem configurar banco de dados
2. ? Testar todos os endpoints CRUD
3. ? Implementar o serviço de ordenação
4. ? Criar testes unitários/integração
5. ? Demonstrar a aplicação facilmente

**A aplicação está pronta para uso imediato!** ??
