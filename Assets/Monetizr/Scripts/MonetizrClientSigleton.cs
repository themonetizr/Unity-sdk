using UnityEngine;

namespace Monetizr
{
    public class MonetizrClient
    {
        public static MonetizrMonoBehaviour Instance { get; private set; }
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeMonetizr()
        {
            if(!Instance)
            {
                // First check if we can find settings
                var settings = Resources.Load<MonetizrSettings>("MonetizrSettings");
                if (settings == null)
                {
                    Debug.LogError("MONETIZR: Monetizr isn't set up! Go to Window -> Monetizr Settings for setup.");
                    return;
                }
                GameObject n = new GameObject();
                n.name = "_MonetizrInstance";
                var newMonetizrBehaviour = n.AddComponent<MonetizrMonoBehaviour>();
                Instance = newMonetizrBehaviour;
                newMonetizrBehaviour.Init(settings);
                Object.DontDestroyOnLoad(n);
                Debug.Log("MONETIZR: Created Monetizr instance successfully!");
            }
        }
    }
}
