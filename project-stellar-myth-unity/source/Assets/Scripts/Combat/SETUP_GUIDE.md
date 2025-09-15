# 🚀 Guia de Setup do Sistema de Combate - Passo a Passo

## ⚡ Setup Rápido (5 minutos)

### 1. Configure as Layers (Unity Editor)
1. Vá em **Edit → Project Settings → Tags and Layers**
2. Adicione as seguintes layers:
   - Layer 8: `Player`
   - Layer 9: `Enemy`

### 2. Setup do Player
1. Selecione seu GameObject do Player
2. No Inspector, clique **Add Component**
3. Adicione: `Player Combat Controller`
4. Configure no layer do GameObject: `Player`
5. Os valores padrão já estão otimizados!

### 3. Setup dos Inimigos
1. Para CADA GameObject de inimigo:
2. No Inspector, clique **Add Component**
3. Adicione: `Enemy Combat Controller`
4. Configure no layer do GameObject: `Enemy`
5. Pronto!

### 4. Teste o Sistema
- ▶️ Execute o jogo
- 🎯 Use o **botão esquerdo do mouse** para atacar
- 👀 Observe os gizmos vermelhos mostrando o alcance

---

## 🔧 Setup Automático (Recomendado)

### Para facilitar o setup, use o componente automático:

#### No Player:
1. Adicione o componente `Combat System Setup`
2. Configure:
   - **Combatant Type**: `Player`
   - **Auto Setup On Start**: ✅ marcado
   - **Create Combat Controller**: ✅ marcado
   - **Create Attack Point**: ✅ marcado
3. Execute o jogo - tudo será configurado automaticamente!

#### Nos Inimigos:
1. Adicione o componente `Combat System Setup`
2. Configure:
   - **Combatant Type**: `Enemy`
   - **Auto Setup On Start**: ✅ marcado
3. Execute o jogo - setup automático para todos!

---

## 🎮 Controles e Funcionalidades

### Player:
- **Fire1** (Input Manager): Atacar inimigos
  - Mouse Esquerdo (padrão)
  - Left Ctrl (alternativo)
- **Teclas Alternativas**: E, Enter
- **Detecção Automática**: Sistema detecta inimigos no alcance
- **Stamina**: Consome stamina por ataque (regenera automaticamente)

### Inimigos:
- **Dano Automático**: Recebem dano quando atacados pelo player
- **Animação de Dano**: Trigger `isHurt` tocado automaticamente
- **Morte**: Animação `isDead` + destruição automática

---

## 🎯 Configuração de Input (IMPORTANTE)

### Input Manager Setup:
1. Vá em **Edit → Project Settings → Input Manager**
2. Encontre **Fire1** e configure:
   - **Positive Button**: left ctrl
   - **Alt Positive Button**: mouse 0
3. Isso permite ataques com mouse OU Ctrl

### Inputs Suportados:
- 🖱️ **Mouse Esquerdo** (Fire1)
- ⌨️ **Left Ctrl** (Fire1)
- ⌨️ **E** (hardcoded)
- ⌨️ **Enter** (hardcoded)

---

## 🎨 Configuração de Animações

### No Animator Controller do Player:
Adicione os parâmetros:
- `isAttacking` (Bool)
- `Attack` (Trigger)

### No Animator Controller dos Inimigos:
Adicione os parâmetros:
- `isHurt` (Trigger) - Para animação de dano
- `isHurting` (Bool) - Estado de recebendo dano
- `isDead` (Trigger) - Para animação de morte
- `isDead` (Bool) - Estado morto

---

## ⚙️ Personalização de Atributos

### Valores Padrão:
```
Player:
- Vida: 100
- Stamina: 100
- Poder de Ataque: 25
- Velocidade de Ataque: 1/segundo
- Alcance: 1.5 unidades

Inimigos:
- Vida: 100
- Stamina: 100
- Poder de Ataque: 25
- Velocidade de Ataque: 1/segundo
- Alcance: 1.5 unidades
```

### Para Personalizar:
1. Selecione o GameObject
2. No componente `Combat Controller`
3. Expanda **Attributes**
4. Ajuste os valores conforme necessário

---

## 🐛 Resolução de Problemas Comuns

### ❌ Player não ataca:
**Verificar:**
- Layer do player está como `Player`?
- Existe `PlayerCombatController` no GameObject?
- Inimigos estão na layer `Enemy`?

### ❌ Inimigos não recebem dano:
**Verificar:**
- Layer dos inimigos está como `Enemy`?
- Existe `EnemyCombatController` nos inimigos?
- Attack Point está posicionado corretamente?

### ❌ Animações não funcionam:
**Verificar:**
- Parâmetros foram criados no Animator?
- Componente Animator está no GameObject?
- Transições estão configuradas corretamente?

---

## 🔍 Debugging e Visualização

### No Scene View (durante o jogo):
- **Círculos Vermelhos**: Alcance de ataque
- **Linhas Verdes**: Conexões com alvos detectados
- **Barras de Vida**: Vida atual dos personagens

### Para mais detalhes:
1. Marque `Debug Attack Detection` no PlayerCombatController
2. Marque `Debug Enemy Combat` no EnemyCombatController
3. Observe o Console para logs detalhados

---

## 🎯 Integração com Sistemas Existentes

### ✅ PlayerController2D:
- Sistema já integrado
- Ataques não interferem com movimento
- Compatibilidade total

### ✅ EnemyPathfinder:
- Sistema já integrado
- Inimigos atacam automaticamente quando próximos
- Fallback para sistema antigo mantido

### ✅ Enemy.cs:
- Compatibilidade mantida
- Sincronização automática entre sistemas

---

## 📋 Checklist de Verificação

### ✅ **Antes de Testar:**
- [ ] Layers `Player` e `Enemy` configuradas
- [ ] `PlayerCombatController` no player
- [ ] `EnemyCombatController` nos inimigos
- [ ] GameObjects com layers corretas
- [ ] Attack Points posicionados (automático se usar setup)

### ✅ **Para Animações:**
- [ ] Parâmetros criados no Animator Controller
- [ ] Transições configuradas
- [ ] Componente Animator nos GameObjects

### ✅ **Teste Final:**
- [ ] Player ataca com mouse esquerdo
- [ ] Inimigos recebem dano
- [ ] Animações tocam corretamente
- [ ] Console sem erros

---

## 💡 Dicas Importantes

### 🎯 **Performance:**
- Sistema otimizado para muitos inimigos
- Use layers ao invés de tags
- Attack Points são criados automaticamente

### 🎨 **Visual:**
- Gizmos mostram alcances no editor
- Debug logs ajudam na configuração
- Sistema compatível com efeitos visuais

### 🔧 **Manutenção:**
- Código modular e extensível
- Fácil de adicionar novos tipos de ataque
- Integração simples com novos sistemas

---

## 🚀 **RESUMO - O que foi Implementado**

✅ **Sistema Completo de Combate**
✅ **Player ataca inimigos com mouse**  
✅ **Inimigos recebem dano e tocam animações**
✅ **Sistema de vida, stamina e atributos**
✅ **Detecção automática de alvos**
✅ **Integração com sistemas existentes**
✅ **Setup automático disponível**
✅ **Debug e visualização incluídos**

**🎮 Agora é só testar e se divertir!**
