# ⚔ RPG ENGINE
### Plano Completo de Projeto
*CLI + Sistema de Grafos + LLM Integration*
`C# · .NET 8 · Graph Theory · AI-Driven Narrative`

---

## Sumário

1. [Visão Geral do Projeto](#1-visão-geral-do-projeto)
2. [Arquitetura do Projeto](#2-arquitetura-do-projeto)
3. [Sistema de Grafos](#3-sistema-de-grafos)
4. [Integração com LLM](#4-integração-com-llm)
5. [Planejamento de Sprints e Commits](#5-planejamento-de-sprints-e-commits)
6. [Convenções dos Arquivos de Dados](#6-convenções-dos-arquivos-de-dados)

---

## 1. Visão Geral do Projeto

O RPG Engine é uma aplicação CLI desenvolvida em C# que combina teoria de grafos para modelagem de mundos, processamento determinístico de regras e integração com LLM para geração narrativa dinâmica. O resultado é um jogo estilo Zork onde a engine garante consistência mecânica enquanto a IA provê riqueza narrativa.

### Pilares Técnicos

- Game Loop determinístico com separação clara de responsabilidades
- Mapa como grafo dirigido com arestas condicionais
- Data-driven design: todo conteúdo em JSON, zero lógica de campanha no código
- LLM como narradora, nunca como árbitro de regras mecânicas
- Arquitetura em camadas com inversão de dependência

### Stack Tecnológica

| Camada | Tecnologia | Responsabilidade |
|--------|------------|-----------------|
| Runtime | C# / .NET 8 | Linguagem e plataforma base |
| Serialização | System.Text.Json | Leitura dos arquivos de dados JSON |
| LLM | OpenAI / Ollama API | Narração e interpretação de ações complexas |
| HTTP Client | HttpClient nativo | Comunicação com a API da LLM |
| Testes | xUnit + FluentAssertions | Cobertura de graph, combat e parser |
| Build | dotnet CLI | Build, test e publish cross-platform |

---

## 2. Arquitetura do Projeto

O projeto segue uma arquitetura em quatro camadas, onde cada projeto C# tem responsabilidades bem definidas e dependências unidirecionais. A regra fundamental é: camadas superiores dependem de camadas inferiores, nunca o contrário.

### 2.1 Estrutura de Pastas

```
RpgEngine/
├── RpgEngine.sln
├── README.md
├── .gitignore
│
├── src/
│   ├── RpgEngine.Core/               ← Lógica pura, sem dependências externas
│   │   ├── Models/
│   │   │   ├── Room.cs
│   │   │   ├── Connection.cs
│   │   │   ├── Item.cs
│   │   │   ├── Creature.cs
│   │   │   ├── PlayerClass.cs
│   │   │   ├── Power.cs
│   │   │   ├── StatusEffect.cs
│   │   │   └── GameState.cs
│   │   ├── Graph/
│   │   │   ├── WorldGraph.cs
│   │   │   ├── GraphAlgorithms.cs
│   │   │   └── ConditionEvaluator.cs
│   │   ├── Engine/
│   │   │   ├── GameLoop.cs
│   │   │   ├── CommandProcessor.cs
│   │   │   ├── CommandParser.cs
│   │   │   └── ActionResult.cs
│   │   ├── Systems/
│   │   │   ├── CombatSystem.cs
│   │   │   ├── InventorySystem.cs
│   │   │   ├── StatusSystem.cs
│   │   │   └── LevelSystem.cs
│   │   └── Interfaces/
│   │       ├── IRenderer.cs
│   │       ├── ILLMBridge.cs
│   │       └── IDataLoader.cs
│   │
│   ├── RpgEngine.Data/               ← Carregamento e deserialização de dados
│   │   ├── Loaders/
│   │   │   ├── GameDataLoader.cs
│   │   │   └── CampaignLoader.cs
│   │   ├── DTOs/
│   │   │   └── (espelha Models, para deserialização)
│   │   └── GameData.cs
│   │
│   ├── RpgEngine.LLM/                ← Integração com APIs de LLM
│   │   ├── LLMBridge.cs
│   │   ├── ContextBuilder.cs
│   │   ├── PromptTemplates.cs
│   │   └── LLMResponse.cs
│   │
│   └── RpgEngine.CLI/                ← Entry point, I/O, renderização
│       ├── Program.cs
│       ├── ConsoleRenderer.cs
│       ├── ConsoleTheme.cs
│       └── SaveManager.cs
│
├── campaigns/                        ← Conteúdo das campanhas (dados puros)
│   └── cavernas_do_caos/
│       ├── campaign.json
│       ├── prompts/
│       │   └── master.txt
│       ├── world/
│       │   ├── rooms.json
│       │   └── connections.json
│       └── entities/
│           ├── items.json
│           ├── creatures.json
│           ├── npcs.json
│           ├── classes.json
│           ├── powers.json
│           └── status_effects.json
│
└── tests/
    ├── RpgEngine.Core.Tests/
    │   ├── GraphTests.cs
    │   ├── CombatTests.cs
    │   └── CommandParserTests.cs
    └── RpgEngine.Integration.Tests/
        └── GameLoopTests.cs
```

### 2.2 Responsabilidades por Camada

**RpgEngine.Core** — O coração do sistema. Contém toda a lógica de negócio sem nenhuma dependência externa. Aqui vivem os modelos de domínio, o grafo do mundo, o game loop, o processador de comandos e todos os subsistemas (combate, inventário, level up). É a única camada que pode ser testada completamente em isolamento.

**RpgEngine.Data** — Responsável exclusivamente por carregar os arquivos JSON e transformá-los em objetos de domínio. Depende do Core para conhecer os tipos, mas o Core não sabe nada sobre como os dados foram carregados — isso é inversão de dependência via a interface `IDataLoader`.

**RpgEngine.LLM** — Encapsula toda a comunicação com a API da LLM. Constrói o contexto serializado do estado do jogo, monta o prompt, faz a chamada HTTP e transforma a resposta JSON estruturada em um `ActionResult` que a engine consegue processar. O Core não sabe que existe uma LLM — ele só conhece a interface `ILLMBridge`.

**RpgEngine.CLI** — O ponto de entrada da aplicação. Responsável pela renderização no terminal, leitura de input, gerenciamento de saves e composição de todas as dependências via injeção. É a camada mais fina: orquestra sem implementar lógica.

---

## 3. Sistema de Grafos

O mapa do jogo é um grafo dirigido onde nós são salas e arestas são conexões com direção e condição opcionais. Isso permite topologias impossíveis em grades 2D: portais unidirecionais, passagens que colapsam, atalhos condicionais, espaços não-euclidianos.

### 3.1 Estruturas de Dados

```csharp
// Nó do grafo
public record Room(
    string Id, string Name, string Description,
    List<string> Items, List<string> Npcs,
    Dictionary<string, string> Flags
);

// Aresta do grafo (dirigida, com predicado opcional)
public record Connection(
    string From, string To,
    string Direction,
    string? Condition   // ex: "has_item:chave" ou "flag:ponte_ok"
);

// O grafo em si (lista de adjacência)
public class WorldGraph {
    private Dictionary<string, Room> _rooms;
    private Dictionary<string, List<Connection>> _adjacency;

    public Room? Navigate(string from, string dir, GameState state)
        => GetExits(from)
           .FirstOrDefault(c => c.Direction == dir
                             && Evaluator.Check(c.Condition, state))
           ?.To is string toId ? _rooms[toId] : null;
}
```

### 3.2 Algoritmos Implementados

| Algoritmo | Complexidade | Uso no Jogo |
|-----------|-------------|-------------|
| BFS | O(V + E) | Caminho mais curto → fast travel, hints de distância |
| DFS | O(V + E) | Salas alcançáveis → validação de mapa, fog of war |
| Componentes Conexos | O(V + E) | Detectar salas isoladas (bug de design) |
| BFS Condicional | O(V + E) | Salas acessíveis dado o estado atual do jogador |
| Detecção de Ciclos | O(V + E) | Verificar se existe rota de volta para um ponto |

---

## 4. Integração com LLM

A LLM atua exclusivamente como narradora. Ela nunca decide se um movimento é válido, se um item existe ou se o HP mudou — isso é responsabilidade da engine.

### 4.1 Fluxo de Decisão

| Ação do Jogador | Quem Resolve | Motivo |
|----------------|-------------|--------|
| `"ir norte"` | Engine (Grafo) | Consulta lista de adjacência + avalia condição |
| `"inventário"` | Engine | Lê GameState diretamente |
| `"pegar espada"` | Engine | Verifica se item existe na sala atual |
| `"atacar goblin"` | Engine + LLM | Engine calcula dano; LLM narra o resultado |
| `"convencer guarda"` | LLM | Julgamento social; engine aplica efeitos retornados |
| `"examinar parede"` | LLM | Expansão narrativa do ambiente |
| `"usar chave"` | Engine valida + LLM narra | Engine checa item; LLM descreve a abertura |

### 4.2 Formato de Resposta da LLM

A LLM sempre responde em JSON estruturado, definido no system prompt. A engine extrai os efeitos mecânicos e os aplica ao GameState após validação:

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

## 5. Planejamento de Sprints e Commits

O projeto está dividido em 5 sprints de 2 semanas cada, totalizando aproximadamente 10 semanas. Cada sprint entrega um incremento jogável ou um subsistema completo e testado.

---

### 🔵 Sprint 1 — Fundação e Estrutura (Semanas 1–2)

> **Objetivo:** solution criada, projetos configurados, modelos de domínio definidos, CI rodando.

| # | Mensagem do Commit | Entregável |
|---|-------------------|------------|
| C01 | `chore: init solution com 4 projetos e gitignore` | RpgEngine.sln funcional |
| C02 | `feat(core): adiciona modelos Room, Connection, Item, Creature` | Domínio base |
| C03 | `feat(core): adiciona modelos PlayerClass, Power, StatusEffect, GameState` | Domínio completo |
| C04 | `feat(core): implementa WorldGraph com lista de adjacência` | Grafo funcional |
| C05 | `feat(core): implementa Navigate, GetExits e ConditionEvaluator` | Movimento no grafo |
| C06 | `feat(data): implementa GameDataLoader com leitura de JSON` | Dados carregando |
| C07 | `test: adiciona testes unitários para WorldGraph e ConditionEvaluator` | Cobertura do grafo |
| C08 | `feat(core): implementa GraphAlgorithms — BFS e DFS` | Algoritmos prontos |
| C09 | `test: adiciona testes para BFS/DFS com mapas de fixture` | Algoritmos validados |
| C10 | `chore: configura GitHub Actions com dotnet test na PR` | CI funcionando |

---

### 🟢 Sprint 2 — Game Loop e CLI Básica (Semanas 3–4)

> **Objetivo:** jogo rodando no terminal sem LLM. O jogador consegue navegar pelo mapa, pegar itens e ver o inventário.

| # | Mensagem do Commit | Entregável |
|---|-------------------|------------|
| C11 | `feat(core): implementa CommandParser com tokenização de input` | Parser de texto |
| C12 | `feat(core): implementa CommandProcessor com handlers locais` | Comandos básicos |
| C13 | `feat(core): implementa GameLoop com ciclo read→process→render` | Loop funcional |
| C14 | `feat(cli): implementa ConsoleRenderer com output colorido` | Visual do terminal |
| C15 | `feat(core): implementa InventorySystem — pegar, largar, listar` | Inventário completo |
| C16 | `feat(cli): adiciona ConsoleTheme configurável por campanha` | Temas visuais |
| C17 | `feat(data): cria campanha de exemplo com 5 salas e itens` | Conteúdo de teste |
| C18 | `feat(cli): implementa SaveManager com serialização de GameState` | Save/Load |
| C19 | `test: testes de integração para GameLoop com mapa de fixture` | Loop testado |
| C20 | `docs: adiciona README com instruções de execução e estrutura` | Documentação base |

---

### 🟡 Sprint 3 — Sistemas de Regras (Semanas 5–6)

> **Objetivo:** combate, level up, status effects e poderes funcionando mecanicamente, sem LLM.

| # | Mensagem do Commit | Entregável |
|---|-------------------|------------|
| C21 | `feat(core): implementa DiceRoller com notação XdY+Z` | Dados funcionando |
| C22 | `feat(core): implementa CombatSystem — turnos, ataque, defesa` | Combate base |
| C23 | `feat(core): adiciona saving throws e resistências no CombatSystem` | Regras avançadas |
| C24 | `feat(core): implementa StatusSystem — aplicar, tick e remover efeitos` | Status effects |
| C25 | `feat(core): implementa LevelSystem com level_up_powers por threshold` | Progressão |
| C26 | `feat(core): implementa PowerSystem — usar poder, verificar custo e cooldown` | Poderes |
| C27 | `feat(data): expande campanha com criaturas, classes e poderes reais` | Conteúdo rico |
| C28 | `test: testes para CombatSystem com seeds de dado fixos` | Combate testado |
| C29 | `test: testes para StatusSystem e LevelSystem` | Progressão testada |
| C30 | `refactor: extrai interfaces IRenderer e IDataLoader no Core` | DI preparado |

---

### 🔴 Sprint 4 — Integração LLM (Semanas 7–8)

> **Objetivo:** LLM integrada como narradora. Ações complexas passam pela IA, que retorna JSON estruturado processado pela engine.

| # | Mensagem do Commit | Entregável |
|---|-------------------|------------|
| C31 | `feat(llm): implementa LLMBridge com HttpClient para OpenAI/Ollama` | Cliente HTTP |
| C32 | `feat(llm): implementa ContextBuilder — serializa GameState para prompt` | Contexto montado |
| C33 | `feat(llm): implementa PromptTemplates com system prompt de mestre` | Prompt base |
| C34 | `feat(llm): implementa parser de LLMResponse e extração de efeitos` | Resposta processada |
| C35 | `feat(core): integra ILLMBridge no CommandProcessor via fallback` | LLM no loop |
| C36 | `feat(core): implementa validador de state_changes retornados pela LLM` | Segurança mecânica |
| C37 | `feat(llm): adiciona retry com backoff exponencial para falhas de API` | Resiliência |
| C38 | `feat(data): cria master.txt — system prompt completo da campanha base` | Prompt de campanha |
| C39 | `test: testes com LLMBridge mockado para validar fluxo completo` | Integração testada |
| C40 | `feat(cli): adiciona indicador visual de loading durante chamada LLM` | UX melhorada |

---

### 🟣 Sprint 5 — Polimento e Entrega (Semanas 9–10)

> **Objetivo:** campanha jogável completa do início ao fim, documentação técnica, build publicável.

| # | Mensagem do Commit | Entregável |
|---|-------------------|------------|
| C41 | `feat(data): completa campanha com 20+ salas, boss final e quest principal` | Campanha completa |
| C42 | `feat(cli): implementa tela de criação de personagem com seleção de classe` | Onboarding |
| C43 | `feat(core): implementa sistema de fast travel via BFS condicional` | QoL feature |
| C44 | `feat(cli): adiciona comando "mapa" com visualização ASCII do grafo` | Visualização |
| C45 | `feat(core): implementa sistema de NPC com diálogos e flags de relação` | NPCs vivos |
| C46 | `perf: adiciona cache de contexto LLM para evitar rebuilds redundantes` | Performance |
| C47 | `test: testes E2E — joga campanha inteira com LLM mockada` | QA completo |
| C48 | `docs: documenta arquitetura, formato JSON e como criar campanhas` | Docs técnicas |
| C49 | `chore: configura dotnet publish para win/linux/osx self-contained` | Build distribuível |
| C50 | `release: v1.0.0 — tag, changelog e binários no GitHub Releases` | Release oficial |

---

## 6. Convenções dos Arquivos de Dados

Todos os arquivos de conteúdo ficam em `campaigns/{nome}/`. A engine não tem nenhum conhecimento sobre campanhas específicas — ela só conhece os schemas. Isso permite criar novas campanhas sem tocar no código.

### 6.1 Schemas dos Arquivos Principais

| Arquivo | Schema Raiz | Campos Obrigatórios |
|---------|------------|---------------------|
| `rooms.json` | `Room[]` | id, name, description |
| `connections.json` | `Connection[]` | from, to, direction |
| `items.json` | `Item[]` | id, name, description, type |
| `creatures.json` | `Creature[]` | id, name, stats, combat, loot_table |
| `classes.json` | `PlayerClass[]` | id, name, base_stats, starting_items |
| `powers.json` | `Power[]` | id, name, type, effects |
| `status_effects.json` | `StatusEffect[]` | id, name, duration, effects_per_tick |
| `campaign.json` | `Campaign` | id, title, starting_room, data_files |

### 6.2 Sistema de Condições

Condições aparecem em conexões (para bloquear passagens) e em poderes (pré-requisitos). São strings com sintaxe simples interpretadas pelo `ConditionEvaluator`:

```
"condition": "has_item:chave_dourada"      // jogador possui o item
"condition": "flag:ponte_consertada"        // flag ativa no GameState
"condition": "level:5"                     // jogador é nível 5 ou maior
"condition": "stat:strength:12"            // atributo >= valor
"condition": "!flag:porta_destruida"       // negação de flag
```

---