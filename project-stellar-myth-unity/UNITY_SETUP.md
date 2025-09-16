# Unity WebGL Setup para Project Stellar Myth

## Configuração do GameObject para Comunicação Web

Para que a comunicação entre Vue.js e Unity funcione corretamente, você precisa criar um GameObject na sua cena principal do Unity com as seguintes configurações:

### 1. Criar o GameObject WebGLCommunicator

1. **Na Unity, clique com o botão direito na Hierarchy**
2. **Selecione "Create Empty"**
3. **Renomeie o GameObject para exatamente: `WebGLCommunicator`**
4. **Anexe o script `WebGLCommunicator.cs` ao GameObject**

### 2. Configurações do Script

No Inspector do GameObject WebGLCommunicator, certifique-se de que:
- ✅ **Debug Mode está marcado** (para ver os logs no Console)
- ✅ **O GameObject está ativo na cena**

### 3. Métodos Disponíveis para JavaScript

O script WebGLCommunicator agora possui os seguintes métodos que podem ser chamados do JavaScript:

#### Parâmetros Customizados
```javascript
// Enviar texto simples
unityService.sendParameter("hello");

// Enviar objeto JSON
unityService.sendParameter({
  message: "Hello Unity!",
  value: 123,
  timestamp: Date.now()
});
```

#### Controle do Jogo
```javascript
// Iniciar o jogo
unityService.startGame();

// Conectar carteira
unityService.connectWallet({
  address: "0x123...",
  network: "ethereum"
});

// Definir dados do player
unityService.setPlayerData({
  level: 5,
  score: 1200
});

// Atualizar blessings
unityService.updateBlessings([
  {id: 1, name: "Fire Blessing", power: 10}
]);
```

### 4. Debug e Logs

Todos os parâmetros enviados do JavaScript aparecerão no **Console da Unity** quando o jogo estiver rodando, com mensagens detalhadas como:

```
WebGLCommunicator: Parâmetro recebido do JavaScript: hello
WebGLCommunicator: Processando texto: 'hello'
WebGLCommunicator: Olá do Unity!
```

### 5. Comandos de Texto Predefinidos

Os seguintes comandos de texto têm comportamentos especiais:

- `"test"` - Executa um comando de teste
- `"debug"` - Ativa o modo debug
- `"hello"` - Responde com "Olá do Unity!"

### 6. Build Settings

Certifique-se de que nas **Build Settings**:
- ✅ **Platform está definido como WebGL**
- ✅ **A cena com o WebGLCommunicator está incluída no build**
- ✅ **O WebGLCommunicator GameObject está ativo na cena**

### 7. Estrutura do Projeto

```
Assets/
├── Scripts/
│   ├── WebGLCommunicator.cs  ← Script principal de comunicação
│   └── ...outros scripts...
└── Scenes/
    └── MainScene.unity       ← Sua cena principal com WebGLCommunicator
```

### 8. Troubleshooting

Se você receber o erro "object WebGLCommunicator not found!":

1. **Verifique se o GameObject existe** na cena ativa
2. **Confirme o nome exato**: `WebGLCommunicator` (case-sensitive)
3. **Certifique-se de que o script está anexado** ao GameObject
4. **Verifique se o GameObject está ativo** na hierarchy
5. **Rebuilde o projeto** WebGL se necessário

### 9. Teste no Editor

Para testar no Unity Editor sem WebGL:
- O script simula automaticamente alguns dados
- Use os botões de contexto no Inspector: 
  - "Testar Recebimento de Parâmetro"
  - "Testar Start Game"

### 10. Exemplo de Uso Completo

```javascript
// No Vue.js/JavaScript
import unityService from './utils/UnityService.js';

// Inicializar Unity com parâmetros iniciais
const initParams = {
  sessionId: "session_123",
  playerLevel: 5,
  gameMode: "adventure"
};

unityService.initializeUnity(canvas, progressCallback, initParams)
  .then(() => {
    // Unity carregado com sucesso
    console.log("Unity loaded!");
    
    // Enviar mais parâmetros depois
    setTimeout(() => {
      unityService.sendParameter("start_music");
    }, 2000);
  });
```

---

## Próximos Passos

1. **Compile seu projeto** Unity para WebGL
2. **Coloque os arquivos** na pasta `src/public/build/Build/`
3. **Teste no navegador** usando o Vue.js frontend
4. **Monitore o console** da Unity para ver os parâmetros sendo recebidos

**✅ Agora sua aplicação Vue.js pode enviar parâmetros para Unity e eles aparecerão como Debug.Log no console!**
