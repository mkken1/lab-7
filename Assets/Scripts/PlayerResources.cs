using System.Collections.Generic;
using UnityEngine;


namespace ProjectScripts
{
    public class PlayerResources : MonoBehaviour
    {
        public Dictionary<string, uint> Resources = new();

        void Awake()
        {
            Resources["Wood"] = 0;
            Resources["Stone"] = 0;
        }

        public void AddResources(uint amount, string resourceType = "Default")
        {
            if (!Resources.ContainsKey(resourceType))
            {
                Resources[resourceType] = 0;
            }

            Resources[resourceType] += amount;
            Debug.Log($"Added {amount} {resourceType} resources");
        }

        public uint GetResourceAmount(string resourceType = "Default")
        {
            if (Resources.TryGetValue(resourceType, out uint amount))
            {
                return amount;
            }
            return 0;
        }

        public void DisplayAllResources()
        {
            foreach (var resource in Resources)
            {
                Debug.Log($"{resource.Key}: {resource.Value}");
            }
        }

        public bool WasteEnoughResources()
        {
            if (Resources["Wood"] >= 4 && Resources["Stone"] >= 2)
            {
                Resources["Wood"] -= 4;
                Resources["Stone"] -= 2;
                Debug.Log("Ресурсы потрачены на починку");
                return true;
            }
            Debug.LogWarning("Нехватка ресурсов");
            return false;
        }
    }
}
