// WebGL JavaScript Plugin para comunicação com Unity
// Este arquivo será incluído no build WebGL

var WebGLCommunicatorPlugin = {
  // Função para solicitar buffs do JavaScript
  RequestBuffsFromJS: function () {
    console.log("Unity: Solicitando buffs do navegador...");

    // Tenta obter dados de diferentes fontes possíveis
    var buffsData = null;

    // 1. Tenta obter de uma variável global window.gameBuffs
    if (window.gameBuffs) {
      console.log("Unity: Buffs encontrados em window.gameBuffs");
      buffsData = window.gameBuffs;
    }
    // 2. Tenta obter de URL parameters
    else if (window.location.search) {
      var urlParams = new URLSearchParams(window.location.search);
      var buffsParam = urlParams.get("buffs");
      if (buffsParam) {
        try {
          buffsData = JSON.parse(decodeURIComponent(buffsParam));
          console.log("Unity: Buffs encontrados em URL parameters");
        } catch (e) {
          console.error("Unity: Erro ao parsear buffs da URL:", e);
        }
      }
    }
    // 3. Tenta obter de localStorage
    else if (localStorage.getItem("gameBuffs")) {
      try {
        buffsData = JSON.parse(localStorage.getItem("gameBuffs"));
        console.log("Unity: Buffs encontrados em localStorage");
      } catch (e) {
        console.error("Unity: Erro ao parsear buffs do localStorage:", e);
      }
    }

    // Se encontrou dados, envia para Unity
    if (buffsData) {
      var jsonString = JSON.stringify(buffsData);
      console.log("Unity: Enviando buffs para Unity:", jsonString);

      // Envia dados para Unity
      SendMessage("WebGLCommunicator", "OnBuffsReceivedFromJS", jsonString);
    } else {
      console.log(
        "Unity: Nenhum dado de buffs encontrado, enviando dados padrão"
      );

      // Envia dados padrão se não encontrar nenhum
      var defaultBuffs = {
        buffs: [
          {
            id: "default_health",
            name: "Poção Básica de Vida",
            description: "Restaura uma pequena quantidade de vida",
            type: "HealOnly",
            value: 15.0,
            stellarTransactionId: "ST-DEFAULT-001",
          },
        ],
      };

      SendMessage(
        "WebGLCommunicator",
        "OnBuffsReceivedFromJS",
        JSON.stringify(defaultBuffs)
      );
    }
  },

  // Função para obter buffs (se necessário para chamadas diretas)
  GetBuffsFromJS: function () {
    // Esta função pode ser usada para obter dados de forma síncrona se necessário
    var buffsData = window.gameBuffs || {};
    var jsonString = JSON.stringify(buffsData);

    // Retorna string para Unity
    var bufferSize = lengthBytesUTF8(jsonString) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(jsonString, buffer, bufferSize);
    return buffer;
  },
};

// Registra o plugin
mergeInto(LibraryManager.library, WebGLCommunicatorPlugin);
