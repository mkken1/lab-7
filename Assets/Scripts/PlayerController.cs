using System.Collections;
using System.Linq;
using UnityEngine;


namespace ProjectScripts
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Transform))]

    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        public float MoveSpeed = 5f;
        public float JumpForce = 6f;
        public bool isGrounded = false;
        public Transform GroundCheck;
        public float checkRadius = 0.0139f;
        public LayerMask GroundLayer;
        private Rigidbody2D rb;

        public SpriteRenderer SpriteRenderer;
        public PlayerResources PlayerResources;

        private bool _isUpgradable = true;

        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            SpriteRenderer = GetComponent<SpriteRenderer>();
            PlayerResources = GetComponent<PlayerResources>();

            var barricadesObjects = GameObject.FindGameObjectsWithTag("Barricade");
            Barricade[] barricades = barricadesObjects.Select(b => b.GetComponent<Barricade>()).ToArray();

            foreach (var barricade in barricades)
            {
                barricade.InitializePlayerInteraction(this);
            }
        }

        void Update()
        {
            isGrounded = Physics2D.OverlapCircle(
                GroundCheck.position,
                checkRadius,
                GroundLayer
            );

            if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            {
                rb.AddForce(new Vector2(0f, JumpForce), ForceMode2D.Impulse);
                isGrounded = false;
            }

            if (_isUpgradable)
            {
                StartCoroutine(UpgradeCoroutine());
                Debug.Log("Вы стали быстрее");
            }
        }

        void FixedUpdate()
        {
            float movementHorizontal = Input.GetAxis("Horizontal");

            if (Input.GetKey(KeyCode.A))
                SpriteRenderer.flipX = true;
            if (Input.GetKey(KeyCode.D))
                SpriteRenderer.flipX = false;

            rb.linearVelocity = new Vector2(movementHorizontal * MoveSpeed, rb.linearVelocityY);
        }

        public void AddResources(string key, uint amount)
        {
            if (string.IsNullOrEmpty(key)) return;

            if (!PlayerResources.Resources.ContainsKey(key))
                PlayerResources.Resources[key] = 0;

            PlayerResources.Resources[key] += amount;
            Debug.Log($"{key} +{amount} => {PlayerResources.Resources[key]}");
        }

        public bool HasEnoughResources(uint amount, string resourceType = "Wood")
        {
            if (PlayerResources.Resources.TryGetValue(resourceType, out uint currentAmount))
            {
                return currentAmount >= amount;
            }
            return false;
        }

        public void SpendResources(uint amount, string resourceType = "Wood")
        {
            if (PlayerResources.Resources.TryGetValue(resourceType, out uint currentAmount) && currentAmount >= amount)
            {
                PlayerResources.Resources[resourceType] -= amount;
                Debug.Log($"Потрачено {amount} {resourceType}. Осталось: {PlayerResources.Resources[resourceType]}");
            }
        }


        private IEnumerator UpgradeCoroutine()
        {
            _isUpgradable = false;
            MoveSpeed += 0.1f;
            yield return new WaitForSeconds(20f);
            _isUpgradable = true;
        }
    }
}
