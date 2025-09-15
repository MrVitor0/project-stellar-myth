namespace EnemySystem
{
    /// <summary>
    /// Implementação concreta do contador de inimigos
    /// Seguindo o Single Responsibility Principle (SRP)
    /// </summary>
    public class EnemyCounter : IEnemyCounter
    {
        private int aliveEnemiesCount = 0;

        public int AliveEnemiesCount => aliveEnemiesCount;

        public void RegisterEnemy()
        {
            aliveEnemiesCount++;
        }

        public void UnregisterEnemy()
        {
            if (aliveEnemiesCount > 0)
            {
                aliveEnemiesCount--;
            }
        }

        public bool AreAllEnemiesDead()
        {
            return aliveEnemiesCount <= 0;
        }

        public void Reset()
        {
            aliveEnemiesCount = 0;
        }
    }
}
