using UnityEngine;

namespace EnemySystem
{
    /// <summary>
    /// Exemplo de inimigo específico que extende a classe Enemy base
    /// Seguindo o Open/Closed Principle (OCP)
    /// </summary>
    public class BasicEnemy : Enemy
    {
        [Header("Basic Enemy Settings")]
        [SerializeField] private float moveSpeed = 2f;
        [SerializeField] private Color enemyColor = Color.red;
        
        private Renderer enemyRenderer;
        private Rigidbody enemyRigidbody;

        #region Unity Lifecycle

        private void Start()
        {
            InitializeComponents();
            SetupVisuals();
        }

        private void Update()
        {
            // Aqui você pode adicionar comportamento básico do inimigo
            // Por exemplo, movimento simples, rotação, etc.
            if (!IsDead)
            {
                BasicMovement();
            }
        }

        #endregion

        #region Initialization

        private void InitializeComponents()
        {
            // Obter ou adicionar componentes necessários
            enemyRenderer = GetComponent<Renderer>();
            if (enemyRenderer == null && GetComponent<MeshRenderer>() != null)
            {
                enemyRenderer = GetComponent<MeshRenderer>();
            }

            enemyRigidbody = GetComponent<Rigidbody>();
            if (enemyRigidbody == null)
            {
                enemyRigidbody = gameObject.AddComponent<Rigidbody>();
                enemyRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            }
        }

        private void SetupVisuals()
        {
            // Configurar a aparência visual do inimigo
            if (enemyRenderer != null && enemyRenderer.material != null)
            {
                enemyRenderer.material.color = enemyColor;
            }
        }

        #endregion

        #region Movement

        private void BasicMovement()
        {
            // Movimento simples - pode ser sobrescrito em subclasses
            // Por exemplo: patrulhamento, seguir o jogador, etc.
            
            // Movimento aleatório simples para exemplo
            if (Random.Range(0f, 1f) < 0.01f) // 1% chance por frame de mudar direção
            {
                Vector3 randomDirection = new Vector3(
                    Random.Range(-1f, 1f), 
                    0, 
                    Random.Range(-1f, 1f)
                ).normalized;

                if (enemyRigidbody != null)
                {
                    enemyRigidbody.linearVelocity = randomDirection * moveSpeed;
                }
            }
        }

        #endregion

        #region Overrides

        public override void OnDamageTaken(float damage)
        {
            base.OnDamageTaken(damage);
            
            // Efeito visual ao receber dano
            if (enemyRenderer != null)
            {
                StartCoroutine(FlashColor(Color.white, 0.1f));
            }
        }

        protected override void OnDeath()
        {
            base.OnDeath();
            
            // Parar movimento ao morrer
            if (enemyRigidbody != null)
            {
                enemyRigidbody.linearVelocity = Vector2.zero;
                enemyRigidbody.isKinematic = true;
            }

            // Efeito visual de morte
            if (enemyRenderer != null)
            {
                StartCoroutine(FadeOut(1f));
            }
        }

        #endregion

        #region Visual Effects

        private System.Collections.IEnumerator FlashColor(Color flashColor, float duration)
        {
            if (enemyRenderer?.material != null)
            {
                Color originalColor = enemyRenderer.material.color;
                enemyRenderer.material.color = flashColor;
                yield return new WaitForSeconds(duration);
                enemyRenderer.material.color = originalColor;
            }
        }

        private System.Collections.IEnumerator FadeOut(float duration)
        {
            if (enemyRenderer?.material != null)
            {
                Color startColor = enemyRenderer.material.color;
                float elapsedTime = 0f;

                while (elapsedTime < duration)
                {
                    elapsedTime += Time.deltaTime;
                    float alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);
                    
                    Color currentColor = startColor;
                    currentColor.a = alpha;
                    enemyRenderer.material.color = currentColor;
                    
                    yield return null;
                }
            }
        }

        #endregion

        #region Editor Support

        private void OnValidate()
        {
            // Atualizar cor no editor quando mudado no inspector
            if (Application.isPlaying && enemyRenderer?.material != null)
            {
                enemyRenderer.material.color = enemyColor;
            }
        }

        #endregion
    }
}
