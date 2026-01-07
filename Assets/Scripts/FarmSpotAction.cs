using System.Collections;
using UnityEngine;


namespace ProjectScripts
{
    public class FarmSpotAction : MonoBehaviour
    {
        public ResourceType resourceType = ResourceType.Wood;
        public bool isInHarvestZone = false;
        public float harvestInterval = 3f;
        public uint resourceAmount;

        public enum ResourceType
        {
            Wood,
            Stone
        }

        void Start()
        {

            resourceAmount = resourceType switch
            {
                ResourceType.Wood => 5,
                ResourceType.Stone => 3,
                _ => 1
            };
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                isInHarvestZone = true;
                StartCoroutine(HarvestResources());
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                isInHarvestZone = false;
                StopAllCoroutines();
            }
        }

        private IEnumerator HarvestResources()
        {
            while (true)
            {
                yield return new WaitForSeconds(harvestInterval);

                if (isInHarvestZone)
                    CollectResources(resourceAmount);
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
                    playerResources.AddResources(amount, resourceType.ToString());
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
    }
}
