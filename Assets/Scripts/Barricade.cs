using UnityEngine;
using System.Collections;

namespace ProjectScripts
{
    public class Barricade : MonoBehaviour
    {
        [Header("Characteristics")]
        [SerializeField] private int _currentHealth = 10;
        [SerializeField] private int _maxHealth = 10;
        private bool _isBroken = false;

        [Header("Components")]
        [SerializeField] private Collider2D _mainCollider; // Основной коллайдер для блокировки врагов
        [SerializeField] private Collider2D _repairTrigger; // Триггер для зоны починки
        [SerializeField] private SpriteRenderer _spriteRenderer;

        private Coroutine _damageCoroutine = null;
        private Coroutine _rebuildCoroutine = null;
        private PlayerController _playerInRange = null;

        private void Awake()
        {
            if (_mainCollider == null) _mainCollider = GetComponent<Collider2D>();
            if (_repairTrigger == null) _repairTrigger = GetComponent<Collider2D>();
            if (_spriteRenderer == null) _spriteRenderer = GetComponent<SpriteRenderer>();
            
            // Настраиваем триггер для починки
            _repairTrigger.isTrigger = true;
            _repairTrigger.enabled = false; // Изначально отключен
            
            SetupInitialState();
        }

        private void SetupInitialState()
        {
            _isBroken = false;
            _currentHealth = _maxHealth;
            
            // Целая баррикада: белая, блокирует врагов
            _spriteRenderer.color = Color.white;
            _mainCollider.enabled = true;
            
            // Отключаем триггер починки
            _repairTrigger.enabled = false;
        }

        public void InitializePlayerInteraction(PlayerController player)
        {
            // Игрок ВСЕГДА проходит сквозь основной коллайдер баррикады
            Physics2D.IgnoreCollision(
                player.GetComponent<Collider2D>(),
                _mainCollider,
                true
            );
        }

        public bool IsAlive() => _currentHealth > 0 && !_isBroken;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && _isBroken)
            {
                _playerInRange = other.GetComponent<PlayerController>();
                TryStartRepair();
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                PlayerController player = other.GetComponent<PlayerController>();
                
                // Проверяем именно этого игрока
                if (player == _playerInRange)
                {
                    _playerInRange = null;
                    
                    // Прерываем починку только если она была начата
                    if (_rebuildCoroutine != null)
                    {
                        StopCoroutine(_rebuildCoroutine);
                        _rebuildCoroutine = null;
                        Debug.Log("Починка прервана - игрок вышел из зоны");
                    }
                }
            }
        }

        private void TryStartRepair()
        {
            if (_playerInRange != null && _isBroken)
            {
                if (_playerInRange.HasEnoughResources(5, "Wood") && _playerInRange.HasEnoughResources(3, "Stone"))
                {
                    if (_rebuildCoroutine == null)
                    {
                        // Добавляем проверку на null перед запуском корутины
                        if (_playerInRange != null)
                        {
                            Debug.Log("Начата починка баррикады...");
                            _rebuildCoroutine = StartCoroutine(StartRebuildBarricade());
                        }
                    }
                }
                else
                {
                    Debug.LogWarning("Нехватка ресурсов для починки!");
                }
            }
        }

        private void StopRepairAttempt()
        {
            if (_rebuildCoroutine != null)
            {
                StopCoroutine(_rebuildCoroutine);
                _rebuildCoroutine = null;
                Debug.Log("Починка прервана - игрок вышел из зоны");
            }
        }

        private IEnumerator StartRebuildBarricade()
        {
            yield return new WaitForSeconds(3f);

            PlayerController currentPlayer = _playerInRange;
            
            if (_isBroken && _playerInRange != null && _playerInRange.HasEnoughResources(5, "Wood")
                        && _playerInRange.HasEnoughResources(3, "Stone"))
            {
                RebuildBarricade();
                currentPlayer.SpendResources(5, "Wood");
                currentPlayer.SpendResources(3, "Stone");
                Debug.Log("Баррикада успешно починена!");
            }
            else
            {
                Debug.LogWarning("Починка не завершена - недостаточно ресурсов или игрок ушел");
            }
            
            _rebuildCoroutine = null;
        }

        private void RebuildBarricade()
        {
            _playerInRange = null;
            
            _isBroken = false;
            _currentHealth = _maxHealth;
            
            _spriteRenderer.color = Color.white;
            
            _mainCollider.enabled = true;
            
            _repairTrigger.enabled = false;
            
            Debug.Log("Баррикада восстановлена!");
        }

        public void StartTakingDamage(int damage = 1, float interval = 3f)
        {
            if (_damageCoroutine != null)
                StopCoroutine(_damageCoroutine);

            _damageCoroutine = StartCoroutine(TakeDamageOverTime(damage, interval));
        }

        private IEnumerator TakeDamageOverTime(int damage, float interval)
        {
            while (IsAlive())
            {
                yield return new WaitForSeconds(interval);
                TakeDamage(damage);
            }

            _damageCoroutine = null;
        }

        public void StopTakingDamage()
        {
            if (_damageCoroutine != null)
            {
                StopCoroutine(_damageCoroutine);
                _damageCoroutine = null;
            }
        }

        public void TakeDamage(int damage = 1)
        {
            if (!IsAlive()) return;
            
            _currentHealth -= damage;
            Debug.Log($"Баррикада получила урон! Здоровье: {_currentHealth}/{_maxHealth}");
            
            if (_currentHealth <= 0)
            {
                BreakBarricade();
            }
        }

        public void BreakBarricade()
        {
            if (_isBroken) return;
            
            _isBroken = true;
            _currentHealth = 0;
            
            // Меняем визуальное состояние на серый
            _spriteRenderer.color = Color.gray;
            
            // Отключаем основной коллайдер (все проходят сквозь)
            _mainCollider.enabled = false;
            
            // Включаем триггер для починки
            _repairTrigger.enabled = true;
            
            Debug.Log("Баррикада разрушена – теперь её можно починить");
        }
    }
}
