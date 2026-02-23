# RPG Engine

> Motor de RPG textual estilo Zork — CLI + Teoria de Grafos + Narração por IA

```
C# · .NET 8 · Graph Theory · AI-Driven Narrative
```

---

## Sumário

- [Visão Geral](#visão-geral)
- [Arquitetura](#arquitetura)
- [Tecnologias](#tecnologias)
- [Teoria de Grafos](#teoria-de-grafos)
- [Integração com LLM](#integração-com-llm)
- [Como Executar](#como-executar)
- [Como Jogar](#como-jogar)
- [Criar uma Campanha](#criar-uma-campanha)
- [Testes](#testes)

---

## Visão Geral

O RPG Engine é uma aplicação CLI desenvolvida em C# que combina **teoria de grafos** para modelagem de mundos com **integração de LLM** para narração dinâmica. O resultado é um jogo textual estilo Zork onde a engine garante consistência mecânica enquanto a IA provê riqueza narrativa.

O design é orientado por três princípios:

- **Data-driven:** todo o conteúdo vive em arquivos JSON. Zero lógica de campanha no código.
- **LLM como narradora, não como árbitro:** a IA descreve o mundo; a engine decide as regras.
- **Grafo como mapa:** o mundo é um grafo dirigido com arestas condicionais, permitindo topologias impossíveis em grades 2D.

---

## Arquitetura

O projeto está dividido em quatro camadas com dependências unidirecionais:

```
RpgEngine/
├── src/
│   ├── RpgEngine.Core/     ← Lógica pura, sem dependências externas
│   ├── RpgEngine.Data/     ← Carregamento e deserialização de JSON
│   ├── RpgEngine.LLM/      ← Integração com APIs de LLM
│   └── RpgEngine.CLI/      ← Entry point, I/O, renderização
├── campaigns/              ← Conteúdo das campanhas (dados puros)
└── tests/
    ├── RpgEngine.Core.Tests/
    └── RpgEngine.Integration.Tests/
```

### Fluxo de dependências

```
CLI → Core ← Data
       ↑
      LLM
```

**Core** não conhece nenhuma outra camada. Toda comunicação com o Core é feita via interfaces (`IDataLoader`, `ILLMBridge`, `IRenderer`), garantindo inversão de dependência e testabilidade total em isolamento.

---

## Tecnologias

| Camada | Tecnologia | Papel |
|--------|------------|-------|
| Runtime | C# / .NET 8 | Linguagem e plataforma base |
| Serialização | System.Text.Json | Leitura dos arquivos de dados JSON |
| LLM | OpenAI / Ollama API | Narração e interpretação de ações |
| HTTP | HttpClient nativo | Comunicação com a API da LLM |
| Testes | xUnit + FluentAssertions | Cobertura de grafo, combate e parser |
| CI | GitHub Actions | Build e testes automáticos em cada PR |
| Build | dotnet CLI | Build, test e publish cross-platform |

---

## Teoria de Grafos

O mapa do jogo é um **grafo dirigido** onde:

- **Nós** são salas (`Room`)
- **Arestas** são conexões (`Connection`) com direção e condição opcionais

Isso permite topologias impossíveis em grades 2D: portais unidirecionais, passagens que colapsam, atalhos condicionais e espaços não-euclidianos.

### Algoritmos implementados

| Algoritmo | Complexidade | Uso no Jogo |
|-----------|-------------|-------------|
| BFS | O(V + E) | Caminho mais curto → fast travel, hints de distância |
| DFS | O(V + E) | Salas alcançáveis → validação de mapa, fog of war |
| Componentes Conexos | O(V + E) | Detectar salas isoladas (bug de design de campanha) |
| BFS Condicional | O(V + E) | Salas acessíveis dado o estado atual do jogador |
| Detecção de Ciclos | O(V + E) | Verificar se existe rota de volta para um ponto |

### Sistema de Condições

Passagens podem exigir condições para serem atravessadas:

```
"condition": "has_item:chave_dourada"   → jogador possui o item
"condition": "flag:ponte_consertada"    → flag ativa no GameState
"condition": "level:5"                 → jogador é nível 5 ou maior
"condition": "stat:strength:12"        → atributo >= valor
"condition": "!flag:porta_destruida"   → negação de flag
```

---

## Integração com LLM

A LLM atua **exclusivamente como narradora**. Ela nunca decide se um movimento é válido ou se o HP mudou — isso é responsabilidade da engine.

### Divisão de responsabilidades

| Ação do Jogador | Quem Resolve | Motivo |
|----------------|-------------|--------|
| `"ir norte"` | Engine (Grafo) | Consulta lista de adjacência + avalia condição |
| `"inventário"` | Engine | Lê GameState diretamente |
| `"pegar espada"` | Engine | Verifica se item existe na sala atual |
| `"atacar goblin"` | Engine + LLM | Engine calcula dano; LLM narra o resultado |
| `"convencer guarda"` | LLM | Julgamento social; engine aplica efeitos retornados |
| `"examinar parede"` | LLM | Expansão narrativa do ambiente |

### Formato de resposta

A LLM sempre responde em JSON estruturado. A engine extrai os efeitos mecânicos e os valida antes de aplicar ao GameState:

```json
{
  "narrative": "O goblin rosna e avança...",
  "state_changes": {
    "player_hp_delta": -5,
    "add_item": null,
    "remove_item": null,
    "set_flag": "goblin_alertado",
    "move_to": null
  },
  "options_hint": ["atacar", "fugir", "negociar"]
}
```

---

## Como Executar

### Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- (Opcional) Chave de API da OpenAI ou instância local do Ollama

### Executar a partir do código-fonte

```bash
# Clonar o repositório
git clone https://github.com/seu-usuario/dotquest.git
cd dotquest

# Build
dotnet build RpgEngine.sln

# Executar com a campanha padrão
dotnet run --project src/RpgEngine.CLI -- --campaign campaigns/cavernas_do_caos
```

### Publicar executável standalone

```bash
# Windows
dotnet publish src/RpgEngine.CLI -r win-x64 --self-contained

# Linux
dotnet publish src/RpgEngine.CLI -r linux-x64 --self-contained

# macOS
dotnet publish src/RpgEngine.CLI -r osx-x64 --self-contained
```

---

## Como Jogar

O RPG Engine é um jogo textual. Você digita comandos em linguagem natural e a engine (com ajuda da IA) responde.

### Comandos básicos

| Comando | Exemplos | Descrição |
|---------|----------|-----------|
| Movimento | `ir norte`, `norte`, `n` | Move para a sala na direção indicada |
| Examinar | `examinar sala`, `olhar` | Descreve a sala atual |
| Inventário | `inventário`, `inv`, `i` | Lista seus itens |
| Pegar | `pegar espada`, `pegar chave` | Coleta item da sala |
| Largar | `largar tocha` | Remove item do inventário |
| Usar | `usar chave`, `usar poção` | Usa um item |
| Atacar | `atacar goblin`, `atacar` | Inicia combate |
| Poderes | `usar poder bola_de_fogo` | Usa poder da sua classe |
| Mapa | `mapa` | Exibe mapa ASCII das salas visitadas |
| Fast travel | `viajar para Entrada` | Teleporte via BFS (se disponível) |
| Salvar | `salvar` | Salva o progresso |
| Carregar | `carregar` | Carrega partida salva |
| Sair | `sair`, `quit` | Encerra o jogo |

### Direções

As direções aceitas são: `norte`, `sul`, `leste`, `oeste`, `cima`, `baixo` — e suas abreviações `n`, `s`, `l`, `o`, `c`, `b`.

### Dicas

- Use `examinar` em objetos, criaturas e elementos do ambiente para obter dicas da IA.
- A engine mostra automaticamente as saídas disponíveis da sala atual.
- Algumas passagens só se abrem quando você possui certos itens ou ativou certas flags.
- O combate é por turnos: a engine calcula o dano e a IA narra o resultado.

---

## Criar uma Campanha

Campanhas são pastas dentro de `campaigns/` compostas exclusivamente de arquivos JSON e texto. Não é necessário alterar o código para criar uma nova campanha.

### Estrutura de uma campanha

```
campaigns/minha_campanha/
├── campaign.json          ← Metadados e ponteiros para os arquivos
├── prompts/
│   └── master.txt         ← System prompt do mestre para a LLM
├── world/
│   ├── rooms.json         ← Salas (nós do grafo)
│   └── connections.json   ← Conexões (arestas do grafo)
└── entities/
    ├── items.json
    ├── creatures.json
    ├── npcs.json
    ├── classes.json
    ├── powers.json
    └── status_effects.json
```

### Exemplo de sala (`rooms.json`)

```json
[
  {
    "id": "entrada",
    "name": "Portão de Pedra",
    "description": "Um enorme portão de pedra marca a entrada das cavernas.",
    "items": ["tocha"],
    "npcs": [],
    "flags": {}
  }
]
```

### Exemplo de conexão (`connections.json`)

```json
[
  {
    "from": "entrada",
    "to": "corredor",
    "direction": "norte",
    "condition": null
  },
  {
    "from": "corredor",
    "to": "sala_secreta",
    "direction": "leste",
    "condition": "has_item:chave_enferrujada"
  }
]
```

---

## Testes

```bash
# Todos os testes
dotnet test RpgEngine.sln

# Apenas testes unitários do Core
dotnet test tests/RpgEngine.Core.Tests/

# Com detalhes de cobertura
dotnet test RpgEngine.sln --verbosity normal
```

### O que está coberto

- **WorldGraph** — adição de salas, conexões, navegação e listagem de saídas
- **ConditionEvaluator** — todas as condições suportadas e suas negações
- **GraphAlgorithms** — BFS, DFS, componentes conexos, BFS condicional e detecção de ciclos
- **CombatSystem** — turnos, dano e saves (com seeds de dado fixos)
- **GameLoop** — ciclo completo com LLMBridge mockado

---

## Licença

MIT
