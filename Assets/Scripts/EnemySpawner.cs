using UnityEngine;
using System.Collections;

namespace ProjectScripts
{
    public enum DirectionType
    {
        Left,
        Right
    }

    public class EnemySpawner : MonoBehaviour
    {
        public DirectionType spawnSide;
        public GameObject enemyPrefab;
        public float spawnInterval = 5f;
        public int MaxEnemyHp = 1;

        private bool _isUpgradable = true;

        void Start()
        {
            Invoke("SpawnEnemy", spawnInterval);
        }

        void Update()
        {
            if (_isUpgradable)
            {
                StartCoroutine(UpgradeEnemy());
                Debug.Log("Басурманы стали сильнее");
            }
        }

        private IEnumerator UpgradeEnemy()
        {
            MaxEnemyHp++;
            _isUpgradable = false;
            yield return new WaitForSeconds(20);
            _isUpgradable = true;
        }

        private IEnumerator SpawnRoutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(spawnInterval);
                SpawnEnemy();
            }
        }

        private void SpawnEnemy()
        {
            GameObject enemyObj = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
            Enemy enemy = enemyObj.GetComponent<Enemy>();
            enemy.MaxHp = MaxEnemyHp;
            enemy.CurrentHp = enemy.MaxHp;

            if (enemy != null)
            {
                enemy.spawnSide = spawnSide;
            }
        }
    }
}
