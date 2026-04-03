# ?? Guia de Testes - Serviço de Ordenação

## Pré-requisitos
- Aplicação rodando em `http://localhost:5000`

## 1?? Ver Configurações Disponíveis

```powershell
curl http://localhost:5000/api/sorting-configurations
```

**Resposta esperada:**
```json
[
  {
    "name": "TitleAndAuthor",
    "description": "Ordena por Título e Autor (ambos ascendentes)",
    "rules": [
      {
        "attribute": "Title",
        "direction": "Ascending",
        "order": 1
      },
      {
        "attribute": "Author",
        "direction": "Ascending",
        "order": 2
      }
    ]
  },
  // ... outras configurações
]
```

## 2?? Listar Livros (Configuração Padrão)

```powershell
curl http://localhost:5000/api/books
```

Usa a configuração padrão definida em `appsettings.json` (TitleAndAuthor).

## 3?? Listar Livros com Configuração Específica

### Por Título e Autor (Ascendente)
```powershell
curl "http://localhost:5000/api/books?configurationName=TitleAndAuthor"
```

### Por Autor e Edição
```powershell
curl "http://localhost:5000/api/books?configurationName=AuthorAndEdition"
```

### Apenas por Edição
```powershell
curl "http://localhost:5000/api/books?configurationName=EditionOnly"
```

### Ordenação Completa (3 níveis)
```powershell
curl "http://localhost:5000/api/books?configurationName=CompleteSort"
```

## 4?? Comparar Resultados

Execute os comandos abaixo e compare a ordem dos livros:

```powershell
# Ordenar por Título primeiro
curl "http://localhost:5000/api/books?configurationName=TitleAndAuthor" | ConvertFrom-Json | Select-Object title, author, edition

# Ordenar por Autor primeiro
curl "http://localhost:5000/api/books?configurationName=AuthorAndEdition" | ConvertFrom-Json | Select-Object title, author, edition

# Ordenar por Edição (descendente)
curl "http://localhost:5000/api/books?configurationName=EditionOnly" | ConvertFrom-Json | Select-Object title, author, edition
```

## 5?? Testar Configuração Inválida

```powershell
curl "http://localhost:5000/api/books?configurationName=ConfiguracaoInexistente"
```

**Resposta esperada:** HTTP 400 com mensagem de erro

## 6?? Via Swagger UI

1. Acesse: `http://localhost:5000/swagger`
2. Expanda `GET /api/sorting-configurations`
3. Clique em "Try it out" ? "Execute"
4. Copie o nome de uma configuração
5. Expanda `GET /api/books`
6. Clique em "Try it out"
7. Cole o nome da configuração no campo `configurationName`
8. Clique em "Execute"

## ?? Modificar Configuração em Tempo Real

### Passo 1: Editar appsettings.json
```json
{
  "BookSorting": {
    "Configurations": [
      {
        "Name": "NovaConfiguracao",
        "Description": "Minha configuração personalizada",
        "Rules": [
          {
            "Attribute": "Author",
            "Direction": "Descending",
            "Order": 1
          }
        ]
      }
    ]
  }
}
```

### Passo 2: Reiniciar a aplicação
```powershell
# Parar a aplicação (Ctrl+C)
# Iniciar novamente
dotnet run --project src/FGV.Api/FGV.Api.csproj
```

### Passo 3: Testar nova configuração
```powershell
curl "http://localhost:5000/api/books?configurationName=NovaConfiguracao"
```

## ?? Exemplo de Resultado Comparativo

### Dados de Teste
```
Livro 1: "Clean Code", "Robert C. Martin", Edição 1
Livro 2: "Clean Architecture", "Robert C. Martin", Edição 1
Livro 3: "Domain-Driven Design", "Eric Evans", Edição 1
```

### TitleAndAuthor (Title ASC, Author ASC)
```
1. Clean Architecture - Robert C. Martin - Ed. 1
2. Clean Code - Robert C. Martin - Ed. 1
3. Domain-Driven Design - Eric Evans - Ed. 1
```

### AuthorAndEdition (Author ASC, Edition DESC)
```
1. Domain-Driven Design - Eric Evans - Ed. 1
2. Clean Architecture - Robert C. Martin - Ed. 1
3. Clean Code - Robert C. Martin - Ed. 1
```

## ? Checklist de Testes

- [ ] Listar configurações disponíveis funciona
- [ ] Listar livros sem parâmetro usa configuração padrão
- [ ] Cada configuração produz ordenação diferente
- [ ] Configuração inválida retorna erro 400
- [ ] Nova configuração no appsettings.json funciona após restart
- [ ] Swagger documenta o parâmetro configurationName
- [ ] Swagger exibe todas as opções de configuração

---

**Dica**: Use o Swagger UI (`/swagger`) para testar de forma interativa!
