using UnityEngine;
using UnityEngine.SceneManagement;

public class WinningSound : MonoBehaviour
{
    [SerializeField] AudioClip winningSound;
    [SerializeField] AudioClip loseSound;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        print(SceneManager.GetActiveScene().name);
        
        if(SceneManager.GetActiveScene().name == "Lose")
                    AudioSource.PlayClipAtPoint(loseSound,transform.position);
        if(SceneManager.GetActiveScene().name == "Win")
                    AudioSource.PlayClipAtPoint(winningSound,transform.position);
        
        
        
    }
 
}
