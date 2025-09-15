using UnityEngine;

namespace EnemySystem
{
    /// <summary>
    /// Implementação concreta de um ponto de spawn
    /// Seguindo o Dependency Inversion Principle (DIP)
    /// </summary>
    public class SpawnPoint : MonoBehaviour, ISpawnPoint
    {
        [SerializeField] private bool isAvailable = true;
        
        public Vector3 Position => transform.position;
        public bool IsAvailable => isAvailable;

        public void SetAvailability(bool available)
        {
            isAvailable = available;
        }

        private void OnDrawGizmos()
        {
            // Visualização do ponto de spawn no editor
            Gizmos.color = IsAvailable ? Color.green : Color.red;
            Gizmos.DrawWireSphere(transform.position, 0.5f);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, 1f);
        }
    }
}
