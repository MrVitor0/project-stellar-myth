# 🎮 Sistema de Game Over - Guia de Setup

## 📋 Visão Geral

O sistema de Game Over foi integrado tanto no `UIController` quanto no `AdvancedUIController`. Quando a vida do player chegar a 0, a tela de Game Over será automaticamente exibida com um botão para reiniciar o jogo.

---

## 🔧 Setup Rápido

### **1. Configure a UI de Game Over**

Na sua scene, crie a seguinte estrutura de UI:

```
Canvas
├── HealthSlider (já existente)
├── StaminaSlider (já existente)
└── GameOverPanel ← NOVO!
    ├── Background (Image - pode usar a imagem Gameover.png)
    ├── Title (Text - "GAME OVER")
    └── RestartButton (Button)
        └── Text (Text - "Restart Game")
```

### **2. Configure o UIController/AdvancedUIController**

No Inspector do UIController ou AdvancedUIController:

#### **Game Over UI:**
- **Game Over Panel**: Arraste o GameObject `GameOverPanel`
- **Restart Button**: Arraste o componente Button do `RestartButton`

### **3. Configuração do Botão (Opcional)**

Você pode configurar o botão de duas maneiras:

#### **Método 1 - Automático (Recomendado):**
- O script já configura automaticamente o `onClick` do botão
- Não precisa fazer nada no Inspector

#### **Método 2 - Manual:**
- No Inspector do Button, em "On Click ()"
- Clique no "+" para adicionar um evento
- Arraste o UIController/AdvancedUIController para o campo
- Selecione: `UIController.RestartGame()` ou `AdvancedUIController.RestartGame()`

---

## 🎨 Personalização da UI

### **Usando a Imagem Gameover.png:**

1. **Crie um Image Component** no GameOverPanel
2. **Configure o Image:**
   - Source Image: `Gameover` (da pasta Images)
   - Image Type: Simple
   - Preserve Aspect: ✅ (recomendado)

### **Estilo Recomendado:**

```
GameOverPanel:
- Anchor: Stretch (cobre toda a tela)
- Color: (0, 0, 0, 180) - fundo semi-transparente escuro

Background Image:
- Anchor: Center
- Width: 400, Height: 300
- Source Image: Gameover

RestartButton:
- Anchor: Bottom Center of GameOverPanel
- Width: 200, Height: 60
- Colors: Normal (255,255,255), Highlighted (200,200,200)
```

---

## ⚙️ Funcionamento Automático

### **O que acontece quando o player morre:**

1. **CombatAttributes.OnDeath** é disparado
2. **UIController.OnPlayerDeath()** é chamado automaticamente
3. **Game Over Panel** é exibido
4. **Time.timeScale = 0f** (jogo pausado)
5. Player pode clicar em **"Restart"**
6. **RestartGame()** recarrega a cena atual

### **Fluxo de Eventos:**
```
Player Health = 0 
    ↓
CombatAttributes.OnDeath
    ↓  
UIController.OnPlayerDeath()
    ↓
ShowGameOverScreen()
    ↓
Time.timeScale = 0 (pausa)
    ↓
Player clica "Restart"
    ↓
RestartGame()
    ↓
SceneManager.LoadScene() (reinicia)
```

---

## 🔄 Métodos Públicos Disponíveis

### **UIController / AdvancedUIController:**

```csharp
// Mostrar tela de Game Over manualmente
uiController.ShowGameOverScreen();

// Esconder tela de Game Over
uiController.HideGameOverScreen();

// Reiniciar o jogo (conecte ao botão)
uiController.RestartGame();
```

### **Exemplo de Uso Manual:**
```csharp
// Se quiser disparar Game Over por outro motivo
UIController ui = FindObjectOfType<UIController>();
ui.ShowGameOverScreen();
```

---

## 🎮 Controles de Debug (F2)

Se o **Debug Mode** estiver ativo no GameController:

- **F2**: Causar 10 de dano (para testar Game Over)
- O sistema automaticamente mostrará Game Over quando a vida chegar a 0

---

## ✅ Checklist de Setup

### **Antes de Testar:**
- [ ] Canvas com GameOverPanel criado
- [ ] RestartButton adicionado dentro do GameOverPanel
- [ ] UIController/AdvancedUIController com referências configuradas
- [ ] Game Over Panel desabilitado por padrão (será ativado automaticamente)
- [ ] Player tem sistema de combate funcionando

### **Teste Final:**
- [ ] Player recebe dano e vida diminui
- [ ] Quando vida = 0, Game Over aparece automaticamente
- [ ] Jogo pausa (Time.timeScale = 0)
- [ ] Botão "Restart" funciona
- [ ] Cena recarrega corretamente
- [ ] Logs no Console confirmam funcionamento

---

## 🆘 Problemas Comuns

### **Game Over não aparece:**
- ✅ Verifique se GameOverPanel está referenciado
- ✅ Verifique se OnDeath está sendo chamado (logs no Console)
- ✅ Verifique se Player tem CombatAttributes configurado

### **Botão não funciona:**
- ✅ Verifique se RestartButton está referenciado
- ✅ Verifique se há erros no Console
- ✅ Teste manualmente: `uiController.RestartGame()`

### **Jogo não pausa:**
- ✅ O sistema usa `Time.timeScale = 0` automaticamente
- ✅ Para UI que ignora pausa, use `Canvas Scaler` com `Unscaled Time`

### **Game Over sempre visível:**
- ✅ GameOverPanel deve começar **desabilitado** na scene
- ✅ O script ativa automaticamente quando necessário

---

## 🚀 Recursos Avançados

### **Adicionando Som:**
```csharp
// No método OnPlayerDeath(), adicione:
AudioSource.PlayClipAtPoint(gameOverSound, Camera.main.transform.position);
```

### **Animações:**
- Adicione `Animator` no GameOverPanel
- Crie animações de fade in/out
- Chame via script: `gameOverPanel.GetComponent<Animator>().SetTrigger("Show");`

### **Estatísticas:**
```csharp
// Salve dados antes de reiniciar
PlayerPrefs.SetInt("Deaths", PlayerPrefs.GetInt("Deaths") + 1);
PlayerPrefs.SetFloat("SurvivalTime", Time.timeSinceLevelLoad);
```

---

## 📝 Notas Importantes

- **Time.timeScale** é restaurado automaticamente ao reiniciar
- **SceneManager.LoadScene()** recarrega a cena inteira
- O sistema funciona com **ambos** UIController e AdvancedUIController
- **GameController** coordena a comunicação entre os sistemas
- **Debug logs** ajudam a rastrear o funcionamento

**🎯 Agora seu jogo tem um sistema completo de Game Over integrado!**
