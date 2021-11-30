using UnityEngine;
using UnityEngine.SceneManagement;

public class GateToNextLevel : MonoBehaviour
{
    [SerializeField] string sceneName;
    

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            SceneManager.LoadScene(sceneName);    // Input can either be a serial number or a name; here we use name.
        }
    }
}
