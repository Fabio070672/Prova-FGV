# Avaliação Técnica da FGV Caso de Uso - Serviço de Ordenação

## Atores
- CSO - Cliente do Serviço de Ordenação

## Fluxo de Eventos

### Fluxo Básico
1. CSO envia um conjunto de livros a ser requisitados pelo serviço de ordenação.
2. O serviço de ordenação ordena o conjunto de livros por título e nome do autor, ambos ascendentes.
3. O serviço de ordenação retorna o conjunto ordenado de livros para o CSO.

## Pré-condições
- Um conjunto não nulo para ser ordenado.

## Pós-condições
- O conjunto de livros tenha sido ordenado com êxito.

## Casos de Uso Relacionados
- Nenhum.

## Requisitos Especiais
- O serviço de ordenação considera um ou mais atributos para classificar os livros.
- Para cada atributo, uma direção de classificação pode ser definida: ascendente ou descendente.
- Configuração deve ser feita via arquivo, sem alterações no código.
