# Serviço de Ordenação de Livros - Configuração

## ?? Visão Geral

O Serviço de Ordenação permite configurar múltiplos critérios de ordenação para livros através do arquivo `appsettings.json`, sem necessidade de alterar código.

## ?? Configuração

### Estrutura do appsettings.json

```json
{
  "BookSorting": {
    "DefaultConfiguration": "TitleAndAuthor",
    "Configurations": [
      {
        "Name": "TitleAndAuthor",
        "Description": "Ordena por Título e Autor (ambos ascendentes)",
        "Rules": [
          {
            "Attribute": "Title",
            "Direction": "Ascending",
            "Order": 1
          },
          {
            "Attribute": "Author",
            "Direction": "Ascending",
            "Order": 2
          }
        ]
      }
    ]
  }
}
```

### Parâmetros

#### BookSorting (Raiz)
- **DefaultConfiguration**: Nome da configuração padrão a ser usada quando nenhuma for especificada

#### Configurations (Array)
Lista de configurações de ordenação disponíveis.

Cada configuração contém:
- **Name**: Identificador único da configuração (usado na API)
- **Description**: Descrição opcional da configuração
- **Rules**: Array de regras de ordenação (aplicadas em ordem)

#### Rules (Array)
Cada regra define um critério de ordenação:
- **Attribute**: Atributo do livro para ordenar
  - Valores: `Title`, `Author`, `Edition`
- **Direction**: Direção da ordenação
  - Valores: `Ascending` (ascendente), `Descending` (descendente)
- **Order**: Ordem de aplicação da regra (1, 2, 3...)
  - Menor número = aplicado primeiro
  - Permite ordenação multi-nível (ex: por título, depois por autor)

## ?? Exemplos de Configuração

### Exemplo 1: Ordenar por Título e Autor (Ambos Ascendentes)
```json
{
  "Name": "TitleAndAuthor",
  "Description": "Ordena por Título e Autor (ambos ascendentes)",
  "Rules": [
    {
      "Attribute": "Title",
      "Direction": "Ascending",
      "Order": 1
    },
    {
      "Attribute": "Author",
      "Direction": "Ascending",
      "Order": 2
    }
  ]
}
```

### Exemplo 2: Ordenar por Autor (Asc) e Edição (Desc)
```json
{
  "Name": "AuthorAndEdition",
  "Description": "Ordena por Autor (ascendente) e Edição (descendente)",
  "Rules": [
    {
      "Attribute": "Author",
      "Direction": "Ascending",
      "Order": 1
    },
    {
      "Attribute": "Edition",
      "Direction": "Descending",
      "Order": 2
    }
  ]
}
```

### Exemplo 3: Ordenar apenas por Edição (Desc)
```json
{
  "Name": "EditionOnly",
  "Description": "Ordena somente por Edição (descendente)",
  "Rules": [
    {
      "Attribute": "Edition",
      "Direction": "Descending",
      "Order": 1
    }
  ]
}
```

### Exemplo 4: Ordenação Completa (3 níveis)
```json
{
  "Name": "CompleteSort",
  "Description": "Ordena por Título, Autor e Edição",
  "Rules": [
    {
      "Attribute": "Title",
      "Direction": "Ascending",
      "Order": 1
    },
    {
      "Attribute": "Author",
      "Direction": "Ascending",
      "Order": 2
    },
    {
      "Attribute": "Edition",
      "Direction": "Descending",
      "Order": 3
    }
  ]
}
```

## ?? Como Usar

### Via API

#### Listar livros com configuração padrão:
```
GET /api/books
```

#### Listar livros com configuração específica:
```
GET /api/books?configurationName=AuthorAndEdition
```

### Configurações Disponíveis
As seguintes configurações estão disponíveis por padrão:
- `TitleAndAuthor` - Ordena por título e autor (ambos ascendentes)
- `AuthorAndEdition` - Ordena por autor (asc) e edição (desc)
- `EditionOnly` - Ordena apenas por edição (descendente)
- `CompleteSort` - Ordena por título, autor e edição

## ?? Como Adicionar Nova Configuração

1. Abra o arquivo `appsettings.json`
2. Adicione uma nova entrada no array `Configurations`:

```json
{
  "BookSorting": {
    "Configurations": [
      {
        "Name": "MinhaNovaConfiguracao",
        "Description": "Descrição da minha configuração",
        "Rules": [
          {
            "Attribute": "Author",
            "Direction": "Descending",
            "Order": 1
          },
          {
            "Attribute": "Title",
            "Direction": "Ascending",
            "Order": 2
          }
        ]
      }
    ]
  }
}
```

3. Salve o arquivo
4. Reinicie a aplicação
5. Use: `GET /api/books?configurationName=MinhaNovaConfiguracao`

## ?? Arquitetura

A implementação segue Clean Architecture:

```
???????????????????????????????????????????????
?           API Layer (Controllers)            ?
?  - BooksController                           ?
?  - Aceita configurationName como parâmetro  ?
???????????????????????????????????????????????
                    ?
???????????????????????????????????????????????
?      Application Layer (CQRS Handlers)      ?
?  - GetAllBooksHandler                        ?
?  - Injeta IBookSortingService                ?
???????????????????????????????????????????????
                    ?
???????????????????????????????????????????????
?         Domain Layer (Interfaces)            ?
?  - IBookSortingService                       ?
?  - ISortingConfigurationRepository           ?
?  - SortingConfiguration (Entity)             ?
?  - SortingRule (Entity)                      ?
???????????????????????????????????????????????
                    ? implementa
???????????????????????????????????????????????
?    Infrastructure Layer (Implementation)     ?
?  - BookSortingService                        ?
?  - SortingConfigurationRepository            ?
?  - Lê configurações do appsettings.json      ?
???????????????????????????????????????????????
```

## ?? Benefícios

? **Sem alteração de código**: Basta editar o `appsettings.json`
? **Múltiplas configurações**: Suporta diferentes estratégias de ordenação
? **Ordenação multi-nível**: Combina múltiplos atributos (1º por título, 2º por autor, etc.)
? **Flexível**: Adicione, remova ou modifique configurações facilmente
? **Clean Architecture**: Separação de responsabilidades entre camadas
? **Type-safe**: Usa enums para atributos e direções

## ?? Observações

- A configuração é carregada na inicialização da aplicação
- Mudanças no `appsettings.json` requerem reinicialização da aplicação
- Todas as regras devem ter um valor válido para `Attribute` e `Direction`
- O campo `Order` determina a prioridade da ordenação (menor = maior prioridade)
- Configurações inválidas no JSON causarão erro na inicialização

## ?? Exemplo de Uso Completo

### 1. Configuração no appsettings.json
```json
{
  "BookSorting": {
    "DefaultConfiguration": "TitleAndAuthor",
    "Configurations": [
      {
        "Name": "TitleAndAuthor",
        "Rules": [
          { "Attribute": "Title", "Direction": "Ascending", "Order": 1 },
          { "Attribute": "Author", "Direction": "Ascending", "Order": 2 }
        ]
      }
    ]
  }
}
```

### 2. Chamada à API
```bash
# Usar configuração padrão (TitleAndAuthor)
curl -X GET "http://localhost:5000/api/books"

# Usar configuração específica
curl -X GET "http://localhost:5000/api/books?configurationName=AuthorAndEdition"
```

### 3. Resposta
```json
[
  {
    "id": "guid-1",
    "title": "Clean Code",
    "author": "Robert C. Martin",
    "edition": 1,
    "active": true,
    "createdAt": "2024-01-01T00:00:00Z"
  },
  {
    "id": "guid-2",
    "title": "Clean Architecture",
    "author": "Robert C. Martin",
    "edition": 1,
    "active": true,
    "createdAt": "2024-01-02T00:00:00Z"
  }
]
```

## ?? Testando Diferentes Configurações

Você pode testar diferentes ordenações alterando o parâmetro `configurationName`:

```bash
# Ordenar por título e autor
GET /api/books?configurationName=TitleAndAuthor

# Ordenar por autor e edição
GET /api/books?configurationName=AuthorAndEdition

# Ordenar apenas por edição
GET /api/books?configurationName=EditionOnly

# Ordenação completa (3 níveis)
GET /api/books?configurationName=CompleteSort
```

---

**Última atualização**: 2024
**Projeto**: FGV - Book Sorting Service
