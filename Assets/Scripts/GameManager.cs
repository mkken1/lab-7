using UnityEngine;

namespace ProjectScripts
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        public void GameOver()
        {
            Debug.Log("GAME OVER!");
        }
    }

}
