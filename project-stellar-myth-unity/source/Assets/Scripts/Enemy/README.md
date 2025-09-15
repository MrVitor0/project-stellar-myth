# Sistema de Spawn de Inimigos

## Visão Geral

Este sistema implementa um spawner de inimigos robusto e flexível para Unity, seguindo os princípios SOLID para garantir código limpo, extensível e fácil de manter.

## Princípios SOLID Aplicados

### 1. **Single Responsibility Principle (SRP)**
- `EnemySpawner`: Gerencia apenas o spawn de inimigos
- `Wave`: Responsável apenas por dados da wave
- `EnemyCounter`: Conta apenas inimigos vivos
- `SpawnPoint`: Gerencia apenas informações de pontos de spawn

### 2. **Open/Closed Principle (OCP)**
- `Enemy`: Classe base que pode ser estendida sem modificação
- `BasicEnemy`: Exemplo de extensão da classe Enemy
- Sistema permite novos tipos de inimigos sem alterar código existente

### 3. **Liskov Substitution Principle (LSP)**
- Qualquer implementação de `ISpawnPoint` pode ser usada
- Qualquer implementação de `IEnemyCounter` pode ser usada
- Subclasses de `Enemy` funcionam perfeitamente no lugar da classe base

### 4. **Interface Segregation Principle (ISP)**
- `ISpawnPoint`: Interface específica para pontos de spawn
- `IEnemyCounter`: Interface específica para contagem de inimigos
- Interfaces pequenas e focadas

### 5. **Dependency Inversion Principle (DIP)**
- `EnemySpawner` depende de abstrações (`ISpawnPoint`, `IEnemyCounter`)
- Não depende de implementações concretas

## Componentes do Sistema

### Classes Principais

#### `EnemySpawner`
Componente principal que gerencia todo o sistema de spawn.

**Funcionalidades:**
- Spawn automático de waves de inimigos
- Seleção aleatória de pontos de spawn
- Controle de waves sequenciais
- Sistema de eventos para integração
- Suporte a loop de waves

#### `Wave` e `WaveConfiguration`
Estruturas de dados para configurar waves de inimigos.

**Configurações por Wave:**
- Nome da wave
- Lista de inimigos e quantidades
- Delay entre spawns
- Delay inicial da wave

#### `Enemy`
Classe base para todos os inimigos.

**Funcionalidades:**
- Sistema de vida
- Notificação automática de morte ao spawner
- Métodos virtuais para customização
- Visualização de vida no editor

#### `SpawnPoint`
Componente que marca pontos de spawn no mundo.

**Funcionalidades:**
- Posição do spawn
- Status de disponibilidade
- Visualização no editor com Gizmos

### Utilitários

#### `EnemySpawnerTester`
Ferramenta para testar o sistema durante desenvolvimento.

**Controles:**
- `Space`: Iniciar waves
- `Escape`: Parar waves
- `N`: Pular para próxima wave
- `R`: Reiniciar sistema
- `K`: Matar todos inimigos
- `X`: Matar inimigo aleatório

## Como Usar

### 1. Configuração Básica

1. **Criar Wave Configuration:**
   - Clique direito no Project → Create → Enemy System → Wave Configuration
   - Configure as waves desejadas com inimigos e quantidades

2. **Setup do Spawner:**
   - Adicione o componente `EnemySpawner` a um GameObject
   - Arraste a Wave Configuration criada para o campo correspondente
   - Configure os Transform points onde inimigos devem spawnar

3. **Setup dos Spawn Points:**
   - Crie GameObjects vazios nas posições desejadas
   - Adicione o componente `SpawnPoint` (ou será adicionado automaticamente)
   - Arraste esses GameObjects para o array de Spawn Points no EnemySpawner

### 2. Criação de Inimigos

#### Método 1: Usar BasicEnemy
```csharp
// Adicione o componente BasicEnemy ao prefab do inimigo
// Configure vida, velocidade e cor no Inspector
```

#### Método 2: Criar Inimigo Customizado
```csharp
using EnemySystem;

public class MyCustomEnemy : Enemy
{
    protected override void OnDeath()
    {
        base.OnDeath();
        // Lógica customizada de morte
        // Ex: dropar itens, tocar som, etc.
    }
    
    protected override void OnDamageTaken(float damage)
    {
        base.OnDamageTaken(damage);
        // Lógica customizada ao receber dano
        // Ex: efeitos visuais, som, etc.
    }
}
```

### 3. Configuração de Wave

1. **No Wave Configuration:**
   - Defina o número de waves
   - Para cada wave, configure:
     - Nome da wave
     - Lista de Enemy Spawn Data (prefab + quantidade)
     - Delays entre spawns
     - Delay inicial da wave

2. **Exemplo de configuração:**
   - Wave 1: 5 inimigos básicos
   - Wave 2: 3 inimigos básicos + 2 inimigos fortes
   - Wave 3: 10 inimigos mistos

### 4. Eventos do Sistema

O `EnemySpawner` fornece eventos Unity para integração:

```csharp
// Conectar eventos no Inspector ou via código
spawner.OnWaveStarted.AddListener(() => Debug.Log("Wave iniciada!"));
spawner.OnWaveCompleted.AddListener(() => Debug.Log("Wave completa!"));
spawner.OnAllWavesCompleted.AddListener(() => Debug.Log("Todas waves completas!"));
spawner.OnEnemySpawned.AddListener(count => Debug.Log($"Inimigo spawnado! Total: {count}"));
spawner.OnEnemyKilled.AddListener(count => Debug.Log($"Inimigo morto! Restam: {count}"));
```

## Estrutura de Arquivos

```
Assets/Scripts/Enemy/
├── EnemySpawner.cs          # Componente principal
├── Wave.cs                  # Estruturas de dados das waves
├── WaveConfiguration.cs     # ScriptableObject de configuração
├── ISpawnPoint.cs          # Interface para pontos de spawn
├── SpawnPoint.cs           # Implementação de pontos de spawn
├── IEnemyCounter.cs        # Interface para contador de inimigos
├── EnemyCounter.cs         # Implementação do contador
├── Enemy.cs                # Classe base para inimigos
├── BasicEnemy.cs           # Exemplo de inimigo concreto
└── EnemySpawnerTester.cs   # Utilitário para testes
```

## Personalização e Extensão

### Novos Tipos de Inimigos
```csharp
public class FastEnemy : Enemy
{
    [SerializeField] private float speed = 5f;
    
    protected override void OnDeath()
    {
        base.OnDeath();
        // Lógica específica do inimigo rápido
    }
}
```

### Novos Tipos de Spawn Points
```csharp
public class RandomSpawnPoint : MonoBehaviour, ISpawnPoint
{
    public Vector3 Position => transform.position + Random.insideUnitSphere * radius;
    public bool IsAvailable => true;
    public void SetAvailability(bool available) { }
}
```

### Sistema de Contagem Customizado
```csharp
public class WeightedEnemyCounter : IEnemyCounter
{
    // Implementação que considera peso dos inimigos
    // Inimigos mais fortes contam mais para completar a wave
}
```

## Debugging e Testes

### EnemySpawnerTester
- Adicione o componente `EnemySpawnerTester` para controles de teste
- Use a GUI in-game ou teclas de atalho
- Configure auto-kill para testes automatizados

### Visualização no Editor
- Spawn Points aparecem como cubos azuis no Scene View
- Inimigos mostram barra de vida quando selecionados
- Gizmos indicam status dos componentes

### Debug Mode
- Ative "Debug Mode" no EnemySpawner para logs detalhados
- Monitore o progresso das waves no Console

## Considerações de Performance

- Sistema otimizado para muitos inimigos simultâneos
- Spawn Points são cached no início
- Eventos usam UnityEvents para performance
- Contador de inimigos é leve e eficiente

## Próximos Passos

Com o sistema básico funcionando, você pode adicionar:

1. **Pathfinding**: Integrar com Navigation Mesh ou A*
2. **IA de Inimigos**: Comportamentos mais complexos
3. **Sistema de Drops**: Itens dropados ao morrer
4. **Efeitos Visuais**: Partículas, animações, sons
5. **Balanceamento**: Sistema de dificuldade progressiva
6. **Boss Battles**: Waves especiais com chefes
7. **Spawns Condicionais**: Baseados em eventos do jogo

## Suporte

Este sistema foi projetado para ser fácil de usar e estender. Se encontrar problemas ou precisar de funcionalidades específicas, o código está bem documentado e segue padrões de qualidade para facilitar modificações.
