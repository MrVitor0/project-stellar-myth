# 🗡️ Sistema de Combate - Project Stellar Myth

## 📋 Visão Geral

Este sistema de combate modular permite que players e inimigos tenham atributos personalizáveis (vida, stamina, poder de ataque), detecção automática de ataques e animações integradas.

## 🎯 Funcionalidades

- ✅ **Sistema de Atributos**: Vida, stamina, poder de ataque configuráveis
- ✅ **Detecção Automática**: Sistema inteligente para detectar alvos
- ✅ **Animações Integradas**: Suporte para animações de ataque, dano e morte
- ✅ **Compatibilidade**: Funciona com sistemas existentes
- ✅ **Modular**: Fácil de estender e customizar

## 📁 Estrutura de Arquivos

```
Assets/Scripts/Combat/
├── CombatAttributes.cs         # Atributos de combate (vida, stamina, etc.)
├── CombatController.cs         # Controlador base de combate
├── PlayerCombatController.cs   # Controlador específico do player
├── EnemyCombatController.cs    # Controlador específico dos inimigos
├── CombatDetector.cs          # Sistema de detecção de ataques
├── CombatPreset.cs            # Presets configuráveis
├── CombatLayers.cs            # Utilitários para layers
└── CombatSystemSetup.cs       # Setup automático
```

## ⚙️ Setup Completo

### 1️⃣ **Configuração de Layers** (OBRIGATÓRIO)

No Unity, vá em **Edit > Project Settings > Tags and Layers** e adicione:

```
Layer 8: Player
Layer 9: Enemy
Layer 10: Neutral (opcional)
Layer 11: Projectile (opcional)
```

### 2️⃣ **Setup do Player**

#### Manual:
1. Selecione o GameObject do Player
2. Adicione o componente `PlayerCombatController`
3. Configure os atributos no Inspector:
   - **Max Health**: 100
   - **Max Stamina**: 100
   - **Attack Power**: 25
   - **Attack Speed**: 1
   - **Attack Range**: 1.5

#### Automático:
1. Adicione o componente `CombatSystemSetup`
2. Configure:
   - **Combatant Type**: Player
   - **Auto Setup On Start**: ✅
3. Execute o jogo - setup será feito automaticamente

### 3️⃣ **Setup dos Inimigos**

#### Para cada inimigo:
1. Selecione o GameObject do Inimigo
2. Adicione o componente `EnemyCombatController`
3. Configure os layer:
   - Set Layer: `Enemy`
   - Target Layers: `Player`

#### Automático:
1. Adicione `CombatSystemSetup` em cada inimigo
2. Configure:
   - **Combatant Type**: Enemy
   - **Auto Setup On Start**: ✅

### 4️⃣ **Integração com Animações**

Configure no **Animator Controller** os seguintes parâmetros:

#### Player:
- `isAttacking` (Bool)
- `Attack` (Trigger)

#### Enemy:
- `isHurt` (Trigger)
- `isHurting` (Bool)
- `isDead` (Trigger/Bool)

### 5️⃣ **Configuração de Attack Points**

O sistema cria automaticamente, mas você pode ajustar manualmente:

1. Crie um GameObject filho chamado "AttackPoint"
2. Posicione onde o ataque deve originar
3. Assign no campo `Attack Point` do CombatController

## 🎮 Controles

### Player:
- **Mouse Esquerdo**: Atacar
- **Sistema automático**: Detecção de inimigos no alcance
- **Efeito visual**: Piscada vermelha quando recebe dano
- **Sistema de vida**: Pode morrer após receber dano suficiente

### Inimigos:
- **Automático**: Integrado com o sistema de pathfinding
- **Alcance**: Ataca automaticamente quando player está próximo
- **Inteligência**: Persegue e ataca o jogador agressivamente
- **Dano ao player**: Causa dano real no jogador

## 🔧 Personalização

### Criando Presets de Combate:

1. Clique direito no Project: `Create > Combat System > Combat Preset`
2. Configure os atributos desejados
3. Assign o preset no `CombatSystemSetup` ou `CombatController`

### Exemplo de Preset - Guerreiro:
```
Max Health: 150
Max Stamina: 80
Attack Power: 35
Attack Speed: 0.8
Attack Range: 2.0
```

### Exemplo de Preset - Arqueiro:
```
Max Health: 80
Max Stamina: 120
Attack Power: 20
Attack Speed: 1.5
Attack Range: 5.0
```

## 🐛 Resolução de Problemas

### ❌ **"Combat layers não estão configurados"**
- **Solução**: Configure as layers conforme o passo 1

### ❌ **"CombatDetector precisa de um ICombatController"**
- **Solução**: Adicione `PlayerCombatController` ou `EnemyCombatController`

### ❌ **Ataques não funcionam**
- **Verificar**: Layers estão corretos?
- **Verificar**: Attack Point está posicionado?
- **Verificar**: Target Layers está configurado?

### ❌ **Animações não tocam**
- **Verificar**: Parâmetros do Animator estão criados?
- **Verificar**: Animator está assigned no GameObject?

## 📊 Debugging

### Visualização no Scene View:
- **Círculos vermelhos**: Alcance de ataque
- **Linhas amarelas**: Alvos detectados
- **Barras verdes**: Vida dos inimigos

### Console Logs:
- Ative `debugAttackDetection` para logs detalhados
- Ative `debugEnemyCombat` para logs dos inimigos

## 🔄 Integração com Sistemas Existentes

### PlayerController2D:
- ✅ **Já integrado** - Ataques usam mouse
- ✅ **Compatível** com sistema de movimento

### EnemyPathfinder:
- ✅ **Já integrado** - Usa novo sistema de combate
- ✅ **Fallback** para sistema antigo

### Enemy.cs:
- ✅ **Compatibilidade** mantida
- ✅ **Sincronização** automática entre sistemas

## 📈 Próximos Passos

### Para implementar futuramente:
- [ ] Ataques dos inimigos no player
- [ ] Sistema de defesa/bloqueio
- [ ] Efeitos visuais (partículas)
- [ ] Som de ataques
- [ ] Diferentes tipos de ataque
- [ ] Sistema de críticos
- [ ] Buffs/Debuffs

## 💡 Dicas de Performance

- Use `LayerMask` ao invés de `GameObject.FindGameObjectsWithTag()`
- Configure `Physics2D.queriesHitTriggers = false` se não usar triggers
- Limite a frequência de detecção de alvos
- Use `Object Pooling` para efeitos visuais

## 📝 Exemplo de Uso em Código

```csharp
// Obter componente de combate
var combat = GetComponent<ICombatController>();

// Verificar se pode atacar
if (combat.CanAttack)
{
    combat.Attack();
}

// Aplicar dano específico
combat.TakeDamage(50f, Vector2.right);

// Verificar vida atual
float healthPercent = combat.Attributes.CurrentHealth / combat.Attributes.MaxHealth;
```

---

## 🎯 **Setup Rápido (TL;DR)**

1. Configure **Layers**: Player (8), Enemy (9)
2. Adicione `PlayerCombatController` no Player
3. Adicione `EnemyCombatController` nos Inimigos
4. Configure **Attack Points** manualmente ou use setup automático
5. Adicione parâmetros no **Animator Controller**
6. Teste os ataques com mouse esquerdo

**🚀 Pronto para usar!**
