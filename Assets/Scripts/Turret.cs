using System.Collections;
using UnityEngine;

namespace ProjectScripts
{
    public class Turret : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private Transform bulletStartPos;

        [Header("Settings")]
        [SerializeField] private float fireInterval = 3f; 

        private void Start()
        {
            StartCoroutine(FireRoutine());
        }

        private IEnumerator FireRoutine()
        {
            while (true)
            {
                Instantiate(bulletPrefab, bulletStartPos.position, Quaternion.identity);

                yield return new WaitForSeconds(fireInterval);
            }
        }
    }
}
