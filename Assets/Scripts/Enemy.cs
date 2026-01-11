using System;
using System.Linq;
using UnityEngine;

namespace ProjectScripts
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class Enemy : MonoBehaviour
    {
        [Header("Settings")]
        public float moveSpeed = 3f;
        public DirectionType spawnSide;
        private GameObject _player;
        private Transform _playerTransform;
        private Rigidbody2D _rb;
        private SpriteRenderer _sr;
        private bool _isDamaging = false;
        [SerializeField] private float _attackInterval = 2f;
        public int MaxHp = 2;
        public int CurrentHp;

        void Awake()
        {
            GameObject[] enemiesObjects = GameObject.FindGameObjectsWithTag("Enemy");
            Enemy[] enemies = enemiesObjects.Select(e => e.GetComponent<Enemy>()).ToArray();
            foreach (var e in enemies)
            {
                Collider2D playerCol = GetComponent<Collider2D>();
                Collider2D enemyCol = e.GetComponent<Collider2D>();
                Physics2D.IgnoreCollision(playerCol, enemyCol, true);
            }

            CurrentHp = MaxHp;
            _rb = GetComponent<Rigidbody2D>();
            _rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            _sr = GetComponent<SpriteRenderer>();

            _player = GameObject.FindGameObjectWithTag("Player");
            if (_player != null)
                _playerTransform = _player.transform;
            else
                Debug.LogError("[Enemy] Player not found in scene!");
        }

        void Start() => SetDirection(spawnSide == DirectionType.Left ? 1 : -1);

        void Update()
        {
            if (_playerTransform == null) return;

            if (!IsAlive())
                Destroy(gameObject);

            if (!_isDamaging)
            {
                Vector2 dir = (_playerTransform.position - transform.position).normalized;
                _rb.linearVelocityX = (dir * moveSpeed).x;
                _sr.flipX = dir.x < 0;
            }
        }
        private void OnCollisionStay2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Barricade"))
            {
                Barricade barricade = collision.gameObject.GetComponent<Barricade>();
                if (barricade != null && barricade.IsAlive())
                {
                    if (!_isDamaging)
                    {
                        _isDamaging = true;
                        barricade.StartTakingDamage(1, _attackInterval);
                    }
                }
            }
            else if (collision.gameObject.CompareTag("Player"))
            {
                GameManager.Instance.GameOver();
            }
            else if (collision.gameObject.CompareTag("Enemy"))
            {
                var enemy = collision.gameObject.GetComponent<Enemy>();
                IgnoreEnemy(enemy);
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Barricade"))
            {
                Barricade barricade = collision.gameObject.GetComponent<Barricade>();
                if (barricade != null)
                {
                    barricade.StopTakingDamage();
                }

                _isDamaging = false;
            }
        }



        private bool IsAlive() => CurrentHp > 0;

        private void SetDirection(int dir) => _sr.flipX = (dir == -1);

        public void TakeDamage(int damage)
        {
            CurrentHp -= damage;
            if (CurrentHp <= 0)
                Destroy(gameObject);
        }

        public void IgnoreEnemy(Enemy enemy)
        {
            Physics2D.IgnoreCollision(
                GetComponent<Collider2D>(),
                enemy.GetComponent<Collider2D>(),
                true);
        }

    }
}
