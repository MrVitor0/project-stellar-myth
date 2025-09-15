# 🎮 Sistema de UI - Guia de Setup

## 📋 Scripts Criados

Foram criados 4 scripts para controlar a UI do seu jogo:

### 1. **UIController.cs** (Básico)
- ✅ Integração direta com o sistema de combate existente
- ✅ Atualização automática via eventos
- ✅ Compatibilidade total com `CombatAttributes`

### 2. **SimpleUIController.cs** (Simples) 
- ✅ Apenas controla sliders
- ✅ Para usar separado do sistema de combate
- ✅ Valores normalizados (0 a 1)

### 3. **AdvancedUIController.cs** (Avançado)
- ✅ Todos os recursos do básico + efeitos visuais
- ✅ Animações suaves nos sliders
- ✅ Mudança de cor baseada em thresholds
- ✅ Efeito de pulso quando vida baixa
- ✅ Flash quando recebe dano
- ✅ Textos opcionais com valores/percentuais

### 4. **GameController.cs** (Controlador Principal)
- ✅ Centraliza controle de todos os sistemas
- ✅ Auto-detecta referencias
- ✅ Controles de debug
- ✅ Alternância entre UI básica e avançada

---

## 🚀 Setup Rápido (5 minutos)

### 1️⃣ **Criar a UI no Canvas**

1. **Crie um Canvas** (se não tiver):
   - GameObject → UI → Canvas

2. **Adicione os Sliders**:
   - Canvas → UI → Slider (nomeie "HealthSlider")  
   - Canvas → UI → Slider (nomeie "StaminaSlider")

3. **Configure os Sliders**:
   - **Min Value**: `0`
   - **Max Value**: `100` (será ajustado automaticamente)
   - **Value**: `100`

### 2️⃣ **Adicionar o Script no GameController**

**Opção A: Usar o GameController Completo (Recomendado)**
1. Crie um GameObject vazio chamado "GameController"
2. Adicione o script `GameController.cs`
3. Configure no Inspector:
   - **Use Advanced UI**: ✅ (para efeitos visuais)
   - **Auto Find References**: ✅
   - **Debug Mode**: ✅ (para testes)

**Opção B: Usar apenas o UIController**
1. Adicione `UIController.cs` ou `AdvancedUIController.cs` em qualquer GameObject
2. Configure as referências no Inspector

### 3️⃣ **Configurar Referências** 

No Inspector do GameController/UIController:
- **Health Slider**: Arraste o slider de vida
- **Stamina Slider**: Arraste o slider de stamina  
- **Player Controller**: Arraste o GameObject do player

**💡 DICA:** Se marcar "Auto Find References", ele encontra automaticamente!

### 4️⃣ **Testar o Sistema**

1. ▶️ **Execute o jogo**
2. 🎯 **Ataque inimigos** (mouse esquerdo) - stamina diminui
3. 🏃 **Use o dash** (Espaço) - stamina diminui significativamente 
4. 😵 **Receba dano** - vida diminui  
5. 👁️ **Observe os sliders** atualizando automaticamente!

**💡 IMPORTANTE:** Os sliders são inicializados automaticamente com os valores do sistema de combate quando o jogo inicia. Os valores máximos são definidos baseados no `CombatAttributes` do player.

**🚀 NOVO:** O dash agora consome stamina! A funcionalidade de correr foi removida - apenas movimento normal e dash.

---

## ⚙️ Configuração Avançada

### 🎨 **Efeitos Visuais (AdvancedUIController)**

```csharp
[Header("Visual Effects")]
public Color healthColor = Color.green;        // Cor vida normal
public Color lowHealthColor = Color.red;       // Cor vida baixa  
public Color staminaColor = Color.blue;        // Cor stamina normal
public Color lowStaminaColor = Color.yellow;   // Cor stamina baixa
public float lowHealthThreshold = 0.25f;       // Threshold vida baixa (25%)
public float lowStaminaThreshold = 0.2f;       // Threshold stamina baixa (20%)
```

### 🎬 **Animações**

```csharp
[Header("Animation Settings")]  
public bool animateSliders = true;             // Ativar animação suave
public float sliderAnimationSpeed = 5f;       // Velocidade da animação
public bool pulseOnLowHealth = true;          // Pulso quando vida baixa
public float pulseSpeed = 2f;                 // Velocidade do pulso
```

### 📝 **Textos Opcionais**

1. Adicione componentes **Text** nos sliders
2. Configure no Inspector:
   - **Health Text**: Referência pro texto da vida
   - **Stamina Text**: Referência pro texto da stamina
   - **Show Values**: ✅ Mostrar valores
   - **Show Percentage**: ✅ Mostrar como % (opcional)

---

## 🎮 Controles de Debug

Quando `Debug Mode` estiver ativo no GameController:

- **F1**: Alternar entre UI básica e avançada  
- **F2**: Causar 10 de dano (teste)
- **F3**: Curar 20 de vida (teste)
- **F4**: Consumir 15 de stamina (teste)
- **F5**: Forçar reinicialização da UI (se houver problemas)

---

## 🔧 APIs Úteis 

### Para usar no código:

```csharp
// Acessar o GameController
GameController.Instance.IsPlayerAlive;
GameController.Instance.PlayerAttributes.CurrentHealth;

// Acessar UI diretamente
UIController ui = FindObjectOfType<UIController>();
float healthPercent = ui.HealthPercentage;
bool lowHealth = ui.IsLowHealth;

// Atualização manual (se necessário)
ui.ManualUpdateHealth(currentHealth, maxHealth);
ui.ManualUpdateStamina(currentStamina, maxStamina);
```

---

## ✅ Checklist de Setup

### **Antes de Testar:**
- [ ] Canvas criado na cena
- [ ] 2 Sliders adicionados (vida e stamina)
- [ ] Script UIController/AdvancedUIController adicionado
- [ ] Referências configuradas (ou Auto Find marcado)
- [ ] Player tem sistema de combate ativo

### **Teste Final:**

- [ ] Sliders aparecem na tela  
- [ ] Stamina diminui ao atacar
- [ ] **Stamina diminui significativamente ao usar dash (Espaço)**
- [ ] Vida diminui ao receber dano
- [ ] **Dash só funciona se tiver stamina suficiente**
- [ ] Efeitos visuais funcionando (se usando Advanced)
- [ ] Console sem erros

---

## 🆘 Problemas Comuns

### ❌ **Sliders não atualizam:**

**Solução:** Verifique se o Player tem `PlayerCombatController` ativo

### ❌ **Referências não encontradas:**

**Solução:** Marque "Auto Find References" ou configure manualmente

### ❌ **Valores não inicializam:**

**Solução:** Pressione **F5** (Debug Mode ativo) para forçar reinicialização

### ❌ **Efeitos visuais não funcionam:**

**Solução:** Use `AdvancedUIController` e configure as referências de Image

### ❌ **Dash não funciona:**

**Solução:** Verifique se tem stamina suficiente. Dash consome stamina!

### ❌ **Console mostra "stamina insuficiente":**

**Solução:** Aguarde a stamina regenerar ou ajuste o custo do dash no PlayerController2D

---

## 🎯 **RESUMO FINAL**

✅ **UI totalmente integrada com sistema de combate**  
✅ **Atualização automática via eventos**  
✅ **Efeitos visuais opcionais**  
✅ **Controle centralizado via GameController**  
✅ **Debug tools incluídas**  
✅ **Compatibilidade com sistema existente**

**🚀 Agora é só configurar e testar!**
