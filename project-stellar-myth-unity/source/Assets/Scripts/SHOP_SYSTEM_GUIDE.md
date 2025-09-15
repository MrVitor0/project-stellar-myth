# Sistema de Loja Entre Waves - Guia de Configuração

Este documento explica como configurar o sistema de loja que aparece entre waves no seu projeto Unity.

## Componentes Criados

### 1. ShopManager
Gerencia a UI da loja e as opções disponíveis para o jogador.

### 2. WaveShopController
Integra o sistema de waves com a loja, controlando quando mostrar a loja.

### 3. Modificações no CombatAttributes
Adicionados métodos para aumentar atributos máximos:
- `IncreaseMaxHealth(float amount)`
- `IncreaseMaxStamina(float amount)`
- `RestoreStamina(float amount)`

### 4. Modificações no EnemySpawner
Adicionado método `ContinueToNextWave()` para integração com a loja.

## Configuração no Unity

### Passo 1: Configurar a UI da Loja

1. **Criar o painel da loja:**
   - Crie um GameObject `ShopPanel` como filho do Canvas
   - Adicione um componente `Image` com background semi-transparente
   - Configure `Raycast Target` como true para bloquear interações

2. **Criar os botões de opção:**
   - Crie 3 GameObjects `OptionButton1`, `OptionButton2`, `OptionButton3` como filhos do ShopPanel
   - Cada botão deve ter:
     - Componente `Button`
     - Componente `Image` (para o ícone da opção)
     - Um filho `Text` (para o nome e descrição)

3. **Adicionar o ShopManager:**
   - Crie um GameObject vazio `ShopManager`
   - Adicione o componente `ShopManager`
   - Configure as referências:
     - `Shop Panel`: Arraste o ShopPanel
     - `Option Buttons`: Arraste os 3 botões
     - `Option Texts`: Arraste os componentes Text dos botões
     - `Option Images`: Arraste os componentes Image dos botões

### Passo 2: Configurar as Opções da Loja

No `ShopManager`, configure o array `Available Options`:

```
Element 0:
- Option Name: "Vida Extra"
- Description: "+20 de vida máxima e cura completa"
- Option Type: Health Upgrade
- Value: 20

Element 1:
- Option Name: "Stamina Extra"
- Description: "+20 de stamina máxima e restaura completa"
- Option Type: Stamina Upgrade
- Value: 20

Element 2:
- Option Name: "Cura Rápida"
- Description: "Restaura 50 de vida"
- Option Type: Heal Only
- Value: 50

Element 3:
- Option Name: "Energia"
- Description: "Restaura 30 de stamina"
- Option Type: Stamina Restore
- Value: 30
```

### Passo 3: Configurar o WaveShopController

1. **Criar o controller:**
   - Crie um GameObject vazio `WaveShopController`
   - Adicione o componente `WaveShopController`

2. **Configurar as referências:**
   - `Enemy Spawner`: Arraste seu EnemySpawner
   - `Shop Manager`: Arraste o ShopManager criado
   - `Game Controller`: Arraste seu GameController (ou deixe null para auto-encontrar)

3. **Configurações:**
   - `Show Shop Between Waves`: Deixe marcado
   - `Debug Mode`: Marque para ver logs de debug

### Passo 4: Configurar os Botões da Loja

Para cada botão da loja, configure o evento `OnClick`:
- `OptionButton1`: Chame `ShopManager.SelectOption(0)`
- `OptionButton2`: Chame `ShopManager.SelectOption(1)`
- `OptionButton3`: Chame `ShopManager.SelectOption(2)`

## Como Funciona

1. **Fim da Wave**: Quando uma wave termina, o `EnemySpawner` dispara o evento `OnWaveCompleted`

2. **Pausa do Sistema**: O `WaveShopController` pausa o sistema de spawning

3. **Abertura da Loja**: A loja é aberta com 3 opções aleatórias das disponíveis

4. **Seleção**: O jogador clica em uma opção, que é aplicada aos atributos do player

5. **Continuação**: A loja fecha e o sistema de waves continua normalmente

## Métodos Públicos Importantes

### ShopManager
- `OpenShop()`: Abre a loja manualmente
- `SelectOption(int index)`: Seleciona uma opção (usado pelos botões)
- `CloseShop()`: Fecha a loja manualmente

### WaveShopController
- `SetShopBetweenWaves(bool enabled)`: Habilita/desabilita a loja entre waves
- `ForceOpenShop()`: Força a abertura da loja (para debug)

## Eventos Disponíveis

### ShopManager
- `OnShopOpened`: Disparado quando a loja abre
- `OnShopClosed`: Disparado quando a loja fecha
- `OnOptionSelected(int index)`: Disparado quando uma opção é selecionada

## Troubleshooting

### A loja não abre
- Verifique se o `WaveShopController` está configurado corretamente
- Verifique se o `EnemySpawner` está disparando `OnWaveCompleted`
- Verifique se `Show Shop Between Waves` está marcado

### As opções não funcionam
- Verifique se o `GameController.Instance` está configurado
- Verifique se o player tem componente `CombatAttributes`
- Verifique se os valores das opções estão configurados corretamente

### O jogo não continua após a loja
- Verifique se os botões estão chamando `SelectOption` corretamente
- Verifique se o `WaveShopController` está recebendo o evento `OnShopClosed`

## Debug

Para debugar o sistema:
1. Marque `Debug Mode` no `ShopManager` e `WaveShopController`
2. Verifique o Console do Unity para mensagens de debug
3. Use `WaveShopController.ForceOpenShop()` para testar a loja manualmente
