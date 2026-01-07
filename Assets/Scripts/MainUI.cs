using System;
using UnityEngine;
using TMPro;

namespace ProjectScripts
{
    public class MainUI : MonoBehaviour
    {

        [Header("Enemies On Scene")]
        public TextMeshProUGUI EnemiesOnScene;

        [Header("Resources")]
        public TextMeshProUGUI Woods;
        public TextMeshProUGUI Stones;

        [Header("Barricades")]
        public TextMeshProUGUI LeftBarricades;
        public TextMeshProUGUI RightBarricades;
        [SerializeField] private GameObject[] leftBarricades;
        [SerializeField] private GameObject[] rightBarricades;
        private readonly char BarricadeIsOk = '1';
        private readonly char BarricadeIsNotOk = '0';

        [Header("Complexity")]
        public TextMeshProUGUI Complexity;

        [Header("Time")]
        public TextMeshProUGUI TimePassed;

        private DateTime _startTime;
        private PlayerResources _playerResources;

        void Awake()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            _playerResources = player.GetComponent<PlayerResources>();
            _startTime = DateTime.Now;
        }

        void FixedUpdate() 
        {
            int enemiesCount = FindEnemiesCount();
            string leftBarricadesStatuses = FindLeftBarricadesStatuses();
            string rightBarricadesStatuses = FindRightBarricadesStatuses();
            string passedTime = GetPassedTime();
            int woodCount = FindResourceWood();
            int stoneCount = FindResourceStone();
            int complexity = FindComplexity();

            ConcatEnemiesCount(enemiesCount);
            ConcatBarricadesStatuses(leftBarricadesStatuses, rightBarricadesStatuses);
            ConcatPassedTime(passedTime);
            ConcatResources(woodCount, stoneCount);
            ConcatComplexity(complexity);
        }

        private int FindEnemiesCount() 
        {
            return GameObject.FindGameObjectsWithTag("Enemy").Length;
        }

        private void ConcatEnemiesCount(int enemiesCount) =>
            EnemiesOnScene.text = $"Врагов вживых: \n{enemiesCount}";
 


        private string FindLeftBarricadesStatuses()
        {
            string leftStatus = string.Empty;
            foreach(var b in leftBarricades)
            {
                var barricade = b.GetComponent<Barricade>();
                leftStatus += barricade.IsAlive() ? $" {BarricadeIsOk}" : $" {BarricadeIsNotOk}";
            }
            return leftStatus;
        }

        private string FindRightBarricadesStatuses()
        {
            string rightStatus = string.Empty;
            foreach(var b in rightBarricades)
            {
                var barricade = b.GetComponent<Barricade>();
                rightStatus += barricade.IsAlive() ? $"{BarricadeIsOk} " : $"{BarricadeIsNotOk} ";
            }
            return rightStatus;
        }

        private void ConcatBarricadesStatuses(string leftStatus, string rightStatus)
        {
            LeftBarricades.text = $"Состояние левых баррикад\n{leftStatus}";
            RightBarricades.text = $"Состояние правых баррикад\n{rightStatus}";
        }


        private string GetPassedTime() =>
            string.Format("{0:mm\\:ss}", DateTime.Now - _startTime);
       
        private void ConcatPassedTime(string timePassed) =>
            TimePassed.text = $"{timePassed} - Времени прошло";


        private int FindResourceWood() =>
            (int) _playerResources.Resources["Wood"];            

        private int FindResourceStone() =>
            (int) _playerResources.Resources["Stone"];

        private void ConcatResources(int woodCount, int stoneCount)
        {
            Woods.text = $"Кол-во дерева: {woodCount}";
            Stones.text = $"Кол-во камня: {stoneCount}";
        }

       
       private int FindComplexity()
        {
            var enemySpawner = GameObject.FindFirstObjectByType<EnemySpawner>();
            return enemySpawner.MaxEnemyHp - 1;
        }

        private void ConcatComplexity(int complexity) =>
            Complexity.text = $"{complexity} - Сложность";
       

    }
    
}