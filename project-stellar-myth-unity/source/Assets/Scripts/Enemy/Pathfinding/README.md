# Sistema de Pathfinding para Inimigos

## Visão Geral

Este sistema implementa um pathfinding simples mas robusto para inimigos, com evitação de obstáculos e outros inimigos, detecção de travamento e comportamentos de combate diferentes.

## Características Principais

- 🎯 **Pathfinding Simples**: Segue o jogador evitando obstáculos
- 🚧 **Evitação de Colisão**: Evita obstáculos e outros inimigos
- ⚔️ **Sistema de Combate**: Suporte para inimigos melee e ranged
- 🔄 **Detecção de Travamento**: Evita que inimigos fiquem presos
- 🎮 **Fácil de Configurar**: Interface intuitiva no Unity Inspector

## Como Usar

### 1. Setup Básico

1. **Adicione o Componente ao Prefab:**
   - Selecione o prefab do inimigo
   - Adicione o componente `EnemyPathfinder`

2. **Configure as Layers:**
   - Configure uma layer para obstáculos (ex: "Foreground")
   - Configure uma layer para inimigos (ex: "Enemy")
   - Atribua as layers corretas aos objetos no cenário

3. **Configure o Pathfinding:**
   ```
   Movement Settings:
   - Move Speed: Velocidade de movimento
   - Rotation Speed: Velocidade de rotação
   - Stopping Distance: Distância mínima do alvo

   Combat Settings:
   - Combat Type: Melee, Ranged ou Support
   - Attack Range: Alcance do ataque
   - Attack Cooldown: Tempo entre ataques

   Avoidance Settings:
   - Avoidance Radius: Raio de detecção
   - Avoidance Force: Força da evitação
   - Obstacle Layer: Layer dos obstáculos
   - Enemy Layer: Layer dos inimigos
   ```

### 2. Tipos de Combate

#### Melee
- Ataca apenas quando muito próximo ao jogador
- Ideal para inimigos corpo a corpo

```csharp
config.combatType = EnemyCombatType.Melee;
config.attackRange = 2f;
```

#### Ranged
- Mantém distância do jogador
- Ataca de longe
- Tem uma distância mínima de ataque

```csharp
config.combatType = EnemyCombatType.Ranged;
config.attackRange = 8f;
```

#### Support
- Similar ao Ranged mas com comportamento diferente
- Útil para inimigos que curam ou dão buff

```csharp
config.combatType = EnemyCombatType.Support;
config.attackRange = 5f;
```

### 3. Evitação de Colisão

O sistema automaticamente:
- Evita outros inimigos próximos
- Desvia de obstáculos na layer configurada
- Ajusta a força de evitação baseado na proximidade

### 4. Debug Visual

Para debug, ative "Show Debug Gizmos" nas configurações para ver:
- Raio de ataque (vermelho)
- Raio de evitação (amarelo)
- Direção do movimento (verde)
- Objetos detectados (linhas coloridas)
- Estado atual do inimigo

## Arquitetura SOLID

### Single Responsibility Principle (SRP)
- `EnemyPathfinder`: Coordena o movimento
- `CombatBehavior`: Gerencia o combate
- `CollisionAvoidance`: Trata evitação de colisões

### Open/Closed Principle (OCP)
- Sistema modular e extensível
- Fácil adicionar novos tipos de combate
- Comportamentos podem ser estendidos

### Liskov Substitution Principle (LSP)
- Interfaces bem definidas
- Comportamentos intercambiáveis
- Mantém contratos consistentes

### Interface Segregation Principle (ISP)
- `IPathfinder`: Movimento básico
- `ICombatBehavior`: Comportamento de combate
- `ICollisionAvoidance`: Evitação de colisão

### Dependency Inversion Principle (DIP)
- Depende de abstrações
- Configuração via interfaces
- Fácil de testar e modificar

## Estrutura de Arquivos

```
Assets/Scripts/Enemy/Pathfinding/
├── EnemyPathfinder.cs         # Componente principal
├── PathfindingConfig.cs       # Configurações
├── PathfindingEnums.cs        # Enums e tipos
├── IPathfinder.cs             # Interfaces
├── CombatBehavior.cs         # Comportamento de combate
└── CollisionAvoidance.cs     # Sistema de evitação
```

## Depuração e Solução de Problemas

### Inimigo Travado
Se o inimigo estiver travando:
1. Aumente o `avoidanceRadius`
2. Aumente o `avoidanceForce`
3. Verifique as layers de colisão
4. Ajuste o `stuckThreshold`

### Inimigos Amontoados
Se os inimigos estiverem se agrupando:
1. Aumente o `avoidanceRadius`
2. Configure corretamente a `enemyLayer`
3. Ajuste o `avoidanceForce`

### Problemas de Ataque
Se os ataques não estão funcionando:
1. Verifique o `attackRange`
2. Confirme o `combatType` correto
3. Ajuste o `attackCooldown`

## Próximos Passos

O sistema pode ser expandido com:
1. Path finding mais avançado (A*, NavMesh)
2. Mais tipos de comportamento de combate
3. Sistema de grupos/formações
4. IA mais complexa para cada tipo
5. Sistema de percepção (visão, audição)
