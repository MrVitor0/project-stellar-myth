using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace EnemySystem
{
    /// <summary>
    /// Classe principal que gerencia o sistema de spawn de inimigos
    /// Seguindo os princípios SOLID:
    /// - Single Responsibility: Gerencia apenas o spawn de inimigos
    /// - Open/Closed: Pode ser extendida sem modificação
    /// - Liskov Substitution: Funciona com qualquer implementação de ISpawnPoint e IEnemyCounter
    /// - Interface Segregation: Usa interfaces específicas
    /// - Dependency Inversion: Depende de abstrações, não implementações
    /// </summary>
    public class EnemySpawner : MonoBehaviour
    {
        [Header("Spawn Configuration")]
        [SerializeField] private WaveConfiguration waveConfiguration;
        [SerializeField] private Transform[] spawnPoints;
        [SerializeField] private bool startAutomatically = true;
        [SerializeField] private bool debugMode = false;
        [SerializeField] private bool pauseForShop = false; // Nova opção para pausar para loja

        [Header("Events")]
        public UnityEvent OnWaveStarted;
        public UnityEvent OnWaveCompleted;
        public UnityEvent OnAllWavesCompleted;
        public UnityEvent<int> OnEnemySpawned;
        public UnityEvent<int> OnEnemyKilled;

        // Dependências injetadas (Dependency Inversion Principle)
        private IEnemyCounter enemyCounter;
        private List<ISpawnPoint> availableSpawnPoints;
        
        // Estado interno
        private int currentWaveIndex = 0;
        private bool isSpawning = false;
        private bool allWavesCompleted = false;
        private Coroutine currentWaveCoroutine;

        #region Unity Lifecycle

        private void Awake()
        {
            InitializeDependencies();
            ValidateConfiguration();
        }

        private void Start()
        {
            if (startAutomatically && waveConfiguration != null)
            {
                StartWaveSystem();
            }
        }

        private void OnDestroy()
        {
            if (currentWaveCoroutine != null)
            {
                StopCoroutine(currentWaveCoroutine);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Inicia o sistema de waves
        /// </summary>
        public void StartWaveSystem()
        {
            if (waveConfiguration == null || allWavesCompleted)
            {
                LogError("Cannot start wave system: No configuration or already completed");
                return;
            }

            if (!isSpawning)
            {
                currentWaveIndex = 0;
                StartCurrentWave();
            }
        }

        /// <summary>
        /// Para o sistema de waves
        /// </summary>
        public void StopWaveSystem()
        {
            if (currentWaveCoroutine != null)
            {
                StopCoroutine(currentWaveCoroutine);
                currentWaveCoroutine = null;
            }
            isSpawning = false;
        }

        /// <summary>
        /// Pula para a próxima wave
        /// </summary>
        public void SkipToNextWave()
        {
            if (isSpawning)
            {
                StopWaveSystem();
                AdvanceToNextWave();
            }
        }

        /// <summary>
        /// Continua para a próxima wave (usado pelo sistema de loja)
        /// </summary>
        public void ContinueToNextWave()
        {
            if (!isSpawning && !allWavesCompleted)
            {
                AdvanceToNextWave();
            }
        }

        /// <summary>
        /// Define se o spawner deve pausar para loja entre waves
        /// </summary>
        public void SetPauseForShop(bool pause)
        {
            pauseForShop = pause;
            Log($"Pause for shop set to: {pause}");
        }

        /// <summary>
        /// Reinicia o sistema de waves
        /// </summary>
        public void RestartWaveSystem()
        {
            StopWaveSystem();
            currentWaveIndex = 0;
            allWavesCompleted = false;
            enemyCounter = new EnemyCounter(); // Reset counter
            StartWaveSystem();
        }

        /// <summary>
        /// Método para ser chamado quando um inimigo morre
        /// </summary>
        public void OnEnemyDeath()
        {
            enemyCounter.UnregisterEnemy();
            OnEnemyKilled?.Invoke(enemyCounter.AliveEnemiesCount);
            
            Log($"Enemy killed. Remaining: {enemyCounter.AliveEnemiesCount}");

            // Verifica se a wave foi completada
            if (enemyCounter.AreAllEnemiesDead() && !isSpawning)
            {
                CompleteCurrentWave();
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Inicializa as dependências do sistema
        /// </summary>
        private void InitializeDependencies()
        {
            // Inicializa o contador de inimigos
            enemyCounter = new EnemyCounter();

            // Converte os Transform points em ISpawnPoint
            availableSpawnPoints = new List<ISpawnPoint>();
            foreach (var spawnTransform in spawnPoints)
            {
                if (spawnTransform != null)
                {
                    // Tenta obter componente SpawnPoint existente ou cria um novo
                    var spawnPoint = spawnTransform.GetComponent<SpawnPoint>();
                    if (spawnPoint == null)
                    {
                        spawnPoint = spawnTransform.gameObject.AddComponent<SpawnPoint>();
                    }
                    availableSpawnPoints.Add(spawnPoint);
                }
            }
        }

        /// <summary>
        /// Valida a configuração do sistema
        /// </summary>
        private void ValidateConfiguration()
        {
            if (waveConfiguration == null)
            {
                LogError("Wave Configuration is not assigned!");
                return;
            }

            if (availableSpawnPoints == null || availableSpawnPoints.Count == 0)
            {
                LogError("No spawn points configured!");
                return;
            }

            if (waveConfiguration.TotalWaves == 0)
            {
                LogError("No waves configured in Wave Configuration!");
                return;
            }
        }

        /// <summary>
        /// Inicia a wave atual
        /// </summary>
        private void StartCurrentWave()
        {
            if (currentWaveIndex >= waveConfiguration.TotalWaves)
            {
                if (waveConfiguration.LoopWaves)
                {
                    currentWaveIndex = 0;
                }
                else
                {
                    CompleteAllWaves();
                    return;
                }
            }

            var currentWave = waveConfiguration.GetWave(currentWaveIndex);
            if (currentWave != null)
            {
                Log($"Starting {currentWave.WaveName} (Wave {currentWaveIndex + 1}/{waveConfiguration.TotalWaves})");
                OnWaveStarted?.Invoke();
                currentWaveCoroutine = StartCoroutine(SpawnWaveCoroutine(currentWave));
            }
        }

        /// <summary>
        /// Coroutine que spawna uma wave completa
        /// </summary>
        private IEnumerator SpawnWaveCoroutine(Wave wave)
        {
            isSpawning = true;

            // Delay inicial da wave
            yield return new WaitForSeconds(wave.WaveStartDelay);

            // Cria lista de todos os inimigos para spawnar de acordo com a configuração
            List<GameObject> enemiesToSpawn = CreateEnemySpawnList(wave);

            // Embaralha a lista para spawn aleatório
            enemiesToSpawn = enemiesToSpawn.OrderBy(x => Random.Range(0f, 1f)).ToList();

            // Spawna os inimigos
            foreach (var enemyPrefab in enemiesToSpawn)
            {
                if (enemyPrefab != null)
                {
                    SpawnEnemy(enemyPrefab);
                    yield return new WaitForSeconds(wave.DelayBetweenSpawns);
                }
            }

            isSpawning = false;
            Log($"Finished spawning {wave.WaveName}. Total enemies: {enemyCounter.AliveEnemiesCount}");

            // Se não há inimigos vivos, completa a wave imediatamente
            if (enemyCounter.AreAllEnemiesDead())
            {
                CompleteCurrentWave();
            }
        }

        /// <summary>
        /// Cria a lista de inimigos para spawnar baseada na configuração da wave
        /// </summary>
        private List<GameObject> CreateEnemySpawnList(Wave wave)
        {
            var enemiesToSpawn = new List<GameObject>();

            foreach (var enemyData in wave.EnemySpawnData)
            {
                for (int i = 0; i < enemyData.Count; i++)
                {
                    enemiesToSpawn.Add(enemyData.EnemyPrefab);
                }
            }

            return enemiesToSpawn;
        }

        /// <summary>
        /// Spawna um inimigo em um ponto aleatório
        /// </summary>
        private void SpawnEnemy(GameObject enemyPrefab)
        {
            var spawnPoint = GetRandomAvailableSpawnPoint();
            if (spawnPoint != null)
            {
                var enemy = Instantiate(enemyPrefab, spawnPoint.Position, Quaternion.identity);
                
                // Configura o inimigo para notificar quando morrer
                var enemyComponent = enemy.GetComponent<Enemy>();
                if (enemyComponent == null)
                {
                    enemyComponent = enemy.AddComponent<Enemy>();
                }
                enemyComponent.Initialize(this);

                enemyCounter.RegisterEnemy();
                OnEnemySpawned?.Invoke(enemyCounter.AliveEnemiesCount);
                
                Log($"Spawned {enemyPrefab.name} at {spawnPoint.Position}. Total enemies: {enemyCounter.AliveEnemiesCount}");
            }
            else
            {
                LogError("No available spawn points!");
            }
        }

        /// <summary>
        /// Obtém um ponto de spawn aleatório disponível
        /// </summary>
        private ISpawnPoint GetRandomAvailableSpawnPoint()
        {
            var availablePoints = availableSpawnPoints.Where(sp => sp.IsAvailable).ToList();
            
            if (availablePoints.Count == 0)
            {
                // Se não há pontos disponíveis, usa qualquer ponto
                availablePoints = availableSpawnPoints.ToList();
            }

            if (availablePoints.Count > 0)
            {
                return availablePoints[Random.Range(0, availablePoints.Count)];
            }

            return null;
        }

        /// <summary>
        /// Completa a wave atual e avança para a próxima
        /// </summary>
        private void CompleteCurrentWave()
        {
            var completedWave = waveConfiguration.GetWave(currentWaveIndex);
            Log($"Wave completed: {completedWave?.WaveName}");
            
            OnWaveCompleted?.Invoke();
            
            // Se pauseForShop está ativo, não avança automaticamente
            // O WaveShopController vai controlar quando avançar
            if (!pauseForShop)
            {
                AdvanceToNextWave();
            }
        }

        /// <summary>
        /// Avança para a próxima wave
        /// </summary>
        private void AdvanceToNextWave()
        {
            currentWaveIndex++;
            
            if (currentWaveIndex >= waveConfiguration.TotalWaves)
            {
                if (waveConfiguration.LoopWaves)
                {
                    currentWaveIndex = 0;
                    StartCoroutine(StartNextWaveAfterDelay());
                }
                else
                {
                    CompleteAllWaves();
                }
            }
            else
            {
                StartCoroutine(StartNextWaveAfterDelay());
            }
        }

        /// <summary>
        /// Inicia a próxima wave após um delay
        /// </summary>
        private IEnumerator StartNextWaveAfterDelay()
        {
            yield return new WaitForSeconds(waveConfiguration.TimeBetweenWaves);
            StartCurrentWave();
        }

        /// <summary>
        /// Completa todas as waves
        /// </summary>
        private void CompleteAllWaves()
        {
            allWavesCompleted = true;
            Log("All waves completed!");
            OnAllWavesCompleted?.Invoke();
        }

        /// <summary>
        /// Log com verificação de debug mode
        /// </summary>
        private void Log(string message)
        {
        
        }

        /// <summary>
        /// Log de erro
        /// </summary>
        private void LogError(string message)
        {
        }

        #endregion

        #region Editor Support

        private void OnDrawGizmos()
        {
            if (spawnPoints != null)
            {
                foreach (var spawnPoint in spawnPoints)
                {
                    if (spawnPoint != null)
                    {
                        Gizmos.color = Color.blue;
                        Gizmos.DrawWireCube(spawnPoint.position, Vector3.one * 0.5f);
                        Gizmos.DrawIcon(spawnPoint.position, "sv_icon_dot3_pix16_gizmo", true);
                    }
                }
            }
        }

        #endregion
    }
}
