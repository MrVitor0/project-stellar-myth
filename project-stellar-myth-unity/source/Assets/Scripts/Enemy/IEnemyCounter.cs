namespace EnemySystem
{
    /// <summary>
    /// Interface para contar inimigos vivos
    /// Seguindo o Interface Segregation Principle (ISP)
    /// </summary>
    public interface IEnemyCounter
    {
        int AliveEnemiesCount { get; }
        void RegisterEnemy();
        void UnregisterEnemy();
        bool AreAllEnemiesDead();
    }
}
