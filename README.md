# Desafio Target Sistemas — Desenvolvedor de Sistemas Júnior

Este é um programa de console em C# feito para o desafio técnico da Target Sistemas, com três exercícios: comissão de vendas, movimentação de estoque e juros por atraso.

O enunciado original pode estar na pasta, no arquivo `desafio_dev.docx`, mas ele fica de fora do repositório por causa do `.gitignore`, então as regras e os dados usados pelo programa estão neste README e na pasta `Dados`.

## Tecnologias

- C#
- .NET 10
- `System.Text.Json`, já incluído no .NET, para ler os JSON
- xUnit para os testes

Os valores de dinheiro usam `decimal`.

## Os três exercícios

### 1. Comissões de vendas

A opção 1 lê `vendas.json`, que traz 36 vendas de João Silva, Maria Souza, Carlos Oliveira e Ana Lima.

A comissão é calculada em cada venda e só depois somada por vendedor:

- abaixo de R$ 100,00: 0%
- de R$ 100,00 até menos de R$ 500,00: 1%
- a partir de R$ 500,00: 5%

R$ 100,00 fica na faixa de 1% e R$ 500,00 fica na faixa de 5%, e cada comissão é arredondada para dois centavos antes da soma e, no meio centavo, o valor se afasta do zero, então R$ 12,005 vira R$ 12,01.

O exemplo abaixo veio de uma execução real da opção 1, o programa lista todas as vendas, e aqui aparecem os totais com algumas linhas de cada vendedor:

```text
João Silva
Vendas: 10
Total vendido: R$ 10.754,70
Comissão total: R$ 495,69
  Valor R$ 1.200,50 | 5% | Comissão R$ 60,03
  Valor R$ 250,30 | 1% | Comissão R$ 2,50
  Valor R$ 480,75 | 1% | Comissão R$ 4,81

Maria Souza
Vendas: 9
Total vendido: R$ 9.874,30
Comissão total: R$ 465,96
  Valor R$ 90,75 | 0% | Comissão R$ 0,00
  Valor R$ 400,50 | 1% | Comissão R$ 4,01

Carlos Oliveira
Vendas: 8
Total vendido: R$ 7.928,35
Comissão total: R$ 379,38
  Valor R$ 500,00 | 5% | Comissão R$ 25,00

Ana Lima
Vendas: 9
Total vendido: R$ 8.763,95
Comissão total: R$ 404,99
  Valor R$ 75,30 | 0% | Comissão R$ 0,00

TOTAL
Vendas: 36
Total vendido: R$ 37.321,30
Comissão total: R$ 1.746,02
```

Uma venda sem vendedor, com valor zero ou com valor negativo, é avisada e fica de fora dos totais, embora o JSON do desafio não tenha esses casos.

### 2. Movimentação de estoque

A opção 2 usa os cinco produtos de `estoque.json`:

| Código | Produto | Estoque inicial |
|---:|---|---:|
| 101 | Caneta Azul | 150 |
| 102 | Caderno Universitário | 75 |
| 103 | Borracha Branca | 200 |
| 104 | Lápis Preto HB | 320 |
| 105 | Marcador de Texto Amarelo | 90 |

O submenu consulta o estoque, registra entrada, registra saída e mostra o histórico e cada movimentação aceita recebe um número a partir de 1 e guarda o tipo, a descrição, a quantidade, o estoque anterior e o estoque final.

Exemplo executado no console:

```text
Código do produto: 102
Quantidade: 5
Descrição da movimentação: compra

Movimentação 1 registrada.
Produto: 102 - Caderno Universitário
Tipo: Entrada
Descrição: compra
Quantidade: 5
Estoque anterior: 75
Estoque final: 80
```

```text
Código do produto: 101
Quantidade: 10
Descrição da movimentação: venda

Movimentação 2 registrada.
Produto: 101 - Caneta Azul
Tipo: Saída
Descrição: venda
Quantidade: 10
Estoque anterior: 150
Estoque final: 140
```

Uma saída maior que o saldo é recusada, sem mudar o saldo, sem entrar no histórico e sem gastar o próximo número. O Produto inexistente, quantidade zero, quantidade negativa ou texto que não seja um número inteiro também são recusados, e a descrição em branco vira "Entrada" ou "Saída".

O estoque fica armazenado na memória enquanto o programa estiver aberto, permitindo navegar entre os exercícios sem perder os saldos e o histórico das movimentações, porém, ao fechar e abrir novamente o sistema, os valores retornam às quantidades originais do JSON, que permanece inalterado.

### 3. Juros por atraso

A opção 3 pede o valor e a data de vencimento no formato `dd/MM/yyyy`, usando como data de hoje a data local do computador, sem considerar o horário.

O acréscimo é juros simples de 2,5% ao dia sobre o valor original, sem juros sobre juros e sem outra multa além dessa taxa e no enunciado, as palavras "juros" e "multa" se referem a essa mesma cobrança.

- dias de atraso = data atual menos o vencimento
- vencimento hoje ou no futuro: zero dias
- juros = valor original × 0,025 × dias de atraso
- os juros são arredondados para dois centavos, com o mesmo critério das comissões
- valor final = valor original + juros

Execução feita em 21/11/2026, com vencimento em 18/11/2026:

```text
Valor original: 200,00
Data de vencimento (dd/MM/yyyy): 18/11/2026

Valor original: R$ 200,00
Data de vencimento: 18/11/2026
Data atual: 21/11/2026
Dias de atraso: 3
Juros: R$ 15,00
Valor final: R$ 215,00
```

Em outra data a quantidade de dias muda, mas a conta continua a mesma: R$ 1.000,00 com um dia de atraso gera R$ 25,00 e termina em R$ 1.025,00, enquanto dez dias geram R$ 250,00 e um valor final de R$ 1.250,00, o vencimento hoje ou no futuro mantém o valor original.

Valor zero, valor negativo, texto que não é número, data vazia ou data impossível, como 31/02/2026, são recusados e o programa não faz o cálculo.

## Estrutura do projeto

```text
Teste_TargetSistemas02/
├── README.md
├── .gitignore
├── global.json
├── TargetSistemas.sln
├── src/TargetSistemas.Console/
│   ├── Program.cs
│   ├── AplicacaoConsole.cs
│   ├── Dados/
│   │   ├── vendas.json
│   │   └── estoque.json
│   ├── Comissoes/
│   ├── Estoque/
│   └── Juros/
└── tests/TargetSistemas.Testes/
```

`CalculadoraComissao`, `ControleEstoque` e `CalculadoraJuros` guardam as regras, `AplicacaoConsole` mostra o menu e os resultados, e os JSON são copiados para a pasta de saída na compilação.

## Requisitos

- Visual Studio Code
- SDK do .NET 10
- Extensão C# da Microsoft no VS Code, para colorir o código e navegar entre os arquivos

O `global.json` pede o SDK 10.0.401 e aceita um patch mais novo da linha 10.0, e o comando `dotnet --version`, no terminal, mostra a versão instalada.

## Executar no Visual Studio Code

1. Instale o [Visual Studio Code](https://code.visualstudio.com/).
2. Instale o [SDK do .NET 10](https://dotnet.microsoft.com/download).
3. No VS Code, instale a extensão **C#**, publicada pela Microsoft, se ela ainda não estiver instalada.
4. Abra a pasta do projeto em Arquivo > Abrir Pasta.

Para compilar antes de executar, use `dotnet build` no terminal integrado.

### Executando o sistema pelo Visual Studio Code

Com o projeto aberto no Visual Studio Code, abra o terminal pelo menu **Terminal > Novo Terminal** e execute o comando abaixo:

```powershell
dotnet run --project src/TargetSistemas.Console
```

Após executar o comando, o sistema será iniciado no próprio terminal, apresentando o menu com as opções de comissões de vendas, movimentação de estoque e cálculo de juros, daí em diante é só escolher uma das opções e testar o funcionamento do programa.

Para encerrar, basta selecionar a opção `0`.

## Testes automatizados

No mesmo terminal integrado do VS Code, na pasta do projeto:

```text
dotnet test
```

Os testes cobrem as faixas de comissão, o JSON oficial, as movimentações aceitas e recusadas, os juros com uma data fixa e o próprio console, com o teclado simulado.

## Premissas

- A comissão é calculada em cada venda, já arredondada, e só então somada, sem aplicar o percentual sobre o total do vendedor.
- O meio centavo sobe, pelo critério `MidpointRounding.AwayFromZero`.
- O nome do vendedor é comparado como foi escrito, apenas sem espaços nas pontas.
- A saída de estoque não pode deixar o saldo negativo.
- O estoque e o histórico ficam na memória enquanto o programa está aberto, sem banco de dados e sem gravar o JSON.
- Os juros são simples, de 2,5% ao dia sobre o valor original, de modo que dez dias sobre R$ 1.000,00 dão R$ 250,00, sem capitalizar.
- A data de hoje ignora o horário e usa o calendário do computador.

## Limitações

- O estoque volta aos valores do JSON quando o programa é aberto de novo.
- A data de vencimento precisa existir no formato `dd/MM/yyyy`, e dia ou mês com um algarismo, como `1/11/2026`, também é aceito.
- Os dias de atraso dependem da data do computador na hora da execução.
