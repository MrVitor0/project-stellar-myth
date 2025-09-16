// src/lib.rs
#![no_std]

// Importações do Soroban SDK
use soroban_sdk::{
    contract, contractimpl, contracttype, symbol_short, Address, Env, String, Vec,
};

// --- Definição do Tipo de Dados Customizado ---
// O atributo #[contracttype] torna nossa struct 'Option' utilizável
// no armazenamento do contrato e como parâmetro de função.
#[contracttype]
#[derive(Clone)] // Clone é necessário para trabalhar com a struct em várias partes do código

{"buff": "+25 Max Health + Full Heal",
 "cost": 0,
  "description": "est",
  "icon": "health_upgrade_icon",
  "option_name": "est",
  "option_type": "HealthUpgrade",
  "owner": "GC3FY6U52OKWJHKDEICT7KWUGBLD4XAMPO4U7PQF7MSO2B7BNUC7LRK6",
  "rarity": "rare",
  "stellar_transaction_id": "ST-HEALTH-150",
  "title": "est",
  "value": 25
}
pub struct Option {
    pub option_name: String,
    pub description: String,
    pub stellar_transaction_id: String, // Usando String, pois pode conter caracteres não numéricos
    pub title: String,
    pub buff: String,
    pub icon: String,
    pub option_type: String,
    pub value: u64, // Usando u64 para valores numéricos sem sinal
    pub rarity: String,
    pub cost: u64,   // Usando u64 para custo
    pub owner: Address, // Adicionamos quem criou o registro
}

// --- Chave de Armazenamento ---
// Usaremos esta chave para acessar nossa lista de opções no ledger.
const OPTIONS: soroban_sdk::Symbol = symbol_short!("OPTIONS");

#[contract]
pub struct StellarMythInGameBonusContract;

#[contractimpl]
impl StellarMythInGameBonusContract {
    /// Adiciona uma nova 'Option' à lista de registros do contrato.
    ///
    /// Parâmetros:
    ///   - user: O endereço da conta que está criando o registro. A transação deve ser assinada por ele.
    ///   - option: O objeto completo 'Option' a ser salvo.
    pub fn create_option(env: Env, user: Address, mut option: Option) {
        // Garante que a transação foi assinada pelo 'user'
        user.require_auth();

        // Atribui o dono ao registro antes de salvar
        option.owner = user.clone();

        // Acessa o armazenamento persistente do contrato
        let mut storage = env.storage().instance();

        // Obtém a lista (Vec) de opções atual. Se não existir, cria uma nova e vazia.
        let mut options_vec: Vec<Option> = storage
            .get(&OPTIONS)
            .unwrap_or_else(|| Vec::new(&env));

        // Adiciona a nova opção ao final da lista
        options_vec.push_back(option);

        // Salva a lista atualizada de volta no armazenamento
        storage.set(&OPTIONS, &options_vec);

        // (Opcional) Define um tempo de vida para os dados, para evitar que o aluguel da rede seja cobrado infinitamente
        // Estende a validade dos dados por mais 100.000 ledgers (~5.7 dias)
        storage.extend_ttl(100_000, 100_000);

        // Emite um evento para notificar indexadores off-chain
        env.events()
            .publish((OPTIONS, symbol_short!("created")), (user));
    }

    /// Retorna as N últimas opções registradas no contrato.
    ///
    /// Parâmetros:
    ///   - n: O número de registros recentes a serem retornados.
    /// Retorna:
    ///   - Um Vec<Option> contendo os registros.
    pub fn get_last_n_options(env: Env, n: u32) -> Vec<Option> {
        let storage = env.storage().instance();

        // Obtém a lista completa de opções
        let options_vec: Vec<Option> = storage
            .get(&OPTIONS)
            .unwrap_or_else(|| Vec::new(&env));

        // Cria um novo Vec para armazenar o resultado
        let mut result_vec = Vec::new(&env);

        // Calcula o índice inicial. Cuidado para não subtrair abaixo de zero.
        let start_index = if options_vec.len() > n {
            options_vec.len() - n
        } else {
            0
        };

        // Itera da 'start_index' até o final da lista, adicionando ao resultado
        for i in start_index..options_vec.len() {
            if let Some(option) = options_vec.get(i) {
                result_vec.push_back(option);
            }
        }
        
        // Retorna o subconjunto de opções mais recentes
        result_vec
    }
}