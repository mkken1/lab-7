using System.Collections;
using UnityEngine;


namespace ProjectScripts
{
    public class FarmSpotAction : MonoBehaviour
    {
        public ResourceType ResourceTypee = ResourceType.Wood;
        public bool IsInHarvestZone = false;
        public float HarvestInterval = 3f;
        public uint ResourceAmount;

        private bool _isUpgradable = true;

        public enum ResourceType
        {
            Wood,
            Stone
        }

        void Start()
        {

            ResourceAmount = ResourceTypee switch
            {
                ResourceType.Wood => 5,
                ResourceType.Stone => 3,
                _ => 1
            };
        }

        void Update()
        {
            if (_isUpgradable)
            {
                StartCoroutine(UpgradeCoroutine());
                Debug.Log("Фармлинги улучшены");
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                IsInHarvestZone = true;
                StartCoroutine(HarvestResources());
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                IsInHarvestZone = false;
                StopAllCoroutines();
            }
        }

        private IEnumerator HarvestResources()
        {
            while (true)
            {
                yield return new WaitForSeconds(HarvestInterval);

                if (IsInHarvestZone)
                    CollectResources(ResourceAmount);
            }
        }

        void CollectResources(uint amount)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if (player != null)
            {
                PlayerResources playerResources = player.GetComponent<PlayerResources>();

                if (playerResources != null)
                {
                    playerResources.AddResources(amount, ResourceTypee.ToString());
                }
                else
                {
                    Debug.LogWarning("PlayerResources component not found!");
                }
            }
            else
            {
                Debug.LogWarning("Player not found!");
            }
        }


        private IEnumerator UpgradeCoroutine()
        {
            _isUpgradable = false;
            HarvestInterval -= 0.05f;
            yield return new WaitForSeconds(15f);
            _isUpgradable = true;
        }

    }
}
