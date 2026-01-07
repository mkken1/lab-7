using System;
using UnityEngine;

namespace ProjectScripts
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class Bullet : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float moveSpeed = 5f;
        [NonSerialized] public int Damage = 1;
        [SerializeField] private DirectionType directionType;  

        private Rigidbody2D rb;
        private int direction;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            direction = directionType == DirectionType.Left ? -1 : 1;

            IgnoreCollisionsWith("Barricade");
            IgnoreCollisionsWith("Player");
        }

        private void Start()
        {
            rb.linearVelocity = new Vector2(direction * moveSpeed, 0f);
        }

        void Update()
        {
            if (Mathf.Abs(transform.position.x) > 20f)
            {
                Destroy(gameObject);
            }
        }


        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Enemy"))
            {
                other.GetComponent<Enemy>().TakeDamage(Damage);
                Destroy(gameObject);
            }
        }
        private void IgnoreCollisionsWith(string tag)
        {
            GameObject[] targets = GameObject.FindGameObjectsWithTag(tag);
            foreach (var go in targets)
            {
                Collider2D otherCol = go.GetComponent<Collider2D>();
                if (otherCol != null)
                {
                    Physics2D.IgnoreCollision(
                        GetComponent<Collider2D>(),
                        otherCol,
                        true);
                }
            }
        }

    }
}
