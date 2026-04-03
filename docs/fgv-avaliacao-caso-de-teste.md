# Avaliação Técnica

## Caso de Teste - Serviço de Ordenação

Este documento apresenta o caso de teste a serem passados pela implementação da interface OrdenacaoLivros.

### Regras de Ordenação

| Regras de Ordenação | Saída Esperada | Anotações |
|----------------------|----------------|-----------|
| Título ascendente | Livros 3, 4, 1, 2 | |
| Autor ascendente Título descendente | Livros 1, 4, 3, 2 | |
| Edição descendente Autor descendente Título ascendente | Livros 4, 1, 3, 2 | |
| Nulo | - | Deve lançar uma OrdenacaoException |
| (conjunto vazio) | (conjunto vazio) | |

### Conjunto de Livros

| Livro | Título | Autor | Edição |
|-------|--------|-------|--------|
| Livro 1 | Java How to Program | Deitel & Deitel | 2007 |
| Livro 2 | Patterns of Enterprise Application Architecture | Martin Fowler | 2002 |
| Livro 3 | Head First Design Patterns | Elisabeth Freeman | 2004 |
| Livro 4 | Internet & World Wide Web: How to Program | Deitel & Deitel | 2007 |
