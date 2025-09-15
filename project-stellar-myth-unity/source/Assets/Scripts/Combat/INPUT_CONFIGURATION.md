# 🎮 Configuração do Input Manager para Sistema de Combate

## 📋 Visão Geral

O sistema de combate foi atualizado para usar o **Input Manager** do Unity, proporcionando maior flexibilidade e compatibilidade com diferentes tipos de controle.

## ⚙️ Configuração no Unity

### 1. **Input Manager Setup**

No Unity, vá em **Edit → Project Settings → Input Manager** e configure:

#### **Fire1 (Input Principal de Ataque):**
- **Name**: Fire1
- **Positive Button**: left ctrl
- **Alt Positive Button**: mouse 0
- **Gravity**: 1000
- **Dead**: 0.001
- **Sensitivity**: 1000
- **Snap**: ✅ (marcado)
- **Invert**: ❌ (desmarcado)
- **Type**: Key or Mouse Button
- **Axis**: X axis
- **Joy Num**: Get Motion from all Joysticks

### 2. **Inputs Suportados**

O sistema agora suporta múltiplos tipos de input:

#### **Primários (Fire1):**
- 🖱️ **Mouse Esquerdo** (padrão)
- ⌨️ **Left Ctrl** (alternativo)

#### **Secundários (hardcoded):**
- ⌨️ **E** (tecla de ação)
- ⌨️ **Enter/Return** (confirmação)

#### **Joystick/Gamepad (futuro):**
- 🎮 **Button A** (Xbox)
- 🎮 **X Button** (PlayStation)

## 🔧 Customização

### **PlayerCombatController**

No componente, você pode configurar:

```csharp
[Header("Input Settings")]
[SerializeField] private string attackInputName = "Fire1"; // Nome do input
[SerializeField] private KeyCode[] alternativeAttackKeys = { KeyCode.E, KeyCode.Return };
```

### **Como Adicionar Novos Inputs:**

1. **Via Inspector**: Adicione teclas no array `Alternative Attack Keys`
2. **Via Código**: Modifique o array `alternativeAttackKeys`
3. **Input Manager**: Crie novos inputs personalizados

## 🎯 Exemplos de Configuração

### **Para Joystick:**
```csharp
// No PlayerCombatController, configure:
attackInputName = "Fire1";           // Botão A do controle
alternativeAttackKeys = { KeyCode.JoystickButton0, KeyCode.JoystickButton1 };
```

### **Para Teclas Personalizadas:**
```csharp
// Configuração para WASD + espaço:
attackInputName = "Fire1";           // Mantém Fire1
alternativeAttackKeys = { KeyCode.Space, KeyCode.LeftShift, KeyCode.Z };
```

## 🐛 Troubleshooting

### ❌ **"Ataque não funciona com mouse"**
**Solução:**
1. Verifique se `Fire1` está configurado no Input Manager
2. Confirme se `mouse 0` está em `Alt Positive Button`

### ❌ **"Tecla E não funciona"**
**Solução:**
1. Verifique se `KeyCode.E` está no array `alternativeAttackKeys`
2. Confirme se não há conflitos com outros sistemas

### ❌ **"Joystick não funciona"**
**Solução:**
1. Configure `Fire1` para `joystick button 0`
2. Adicione `KeyCode.JoystickButton0` nas alternativas

## 🔄 Compatibilidade

### **Sistemas Afetados:**
- ✅ **PlayerCombatController**: Atualizado
- ✅ **PlayerController2D**: Atualizado  
- ✅ **Sistema de Pathfinding**: Não afetado
- ✅ **Sistema de Inimigos**: Não afetado

### **Backward Compatibility:**
- ✅ Mouse esquerdo ainda funciona
- ✅ Teclas E e Enter mantidas
- ✅ Sistemas antigos não quebram

## 💡 Vantagens do Input Manager

### **Flexibilidade:**
- 🎮 Suporte a múltiplos dispositivos
- ⌨️ Personalização fácil pelo jogador
- 🖱️ Compatibilidade com mouse/teclado/gamepad

### **Performance:**
- 🚀 Otimizado pelo Unity
- 📱 Funciona em todas as plataformas
- 🔧 Sistema nativo, sem dependências

## 📝 Código de Referência

### **Input Check Atual:**
```csharp
private bool GetAttackInput()
{
    // Input principal (Fire1)
    if (Input.GetButtonDown(attackInputName))
        return true;
    
    // Teclas alternativas
    foreach (KeyCode key in alternativeAttackKeys)
    {
        if (Input.GetKeyDown(key))
            return true;
    }
    
    return false;
}
```

### **Como Usar em Outros Scripts:**
```csharp
// Para verificar ataque em qualquer script:
if (Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.E))
{
    // Executar ataque
}
```

---

## ✅ **Resumo da Correção**

O bug foi corrigido! Agora o sistema:

- ✅ **Usa Input Manager** ao invés de `GetMouseButtonDown(0)`
- ✅ **Suporta múltiplas teclas** (mouse, E, Enter, Ctrl)
- ✅ **É configurável** pelo Inspector
- ✅ **Mantém compatibilidade** com sistemas existentes
- ✅ **Funciona em todas as plataformas**

**🎮 O ataque agora funciona com qualquer input configurado!**
