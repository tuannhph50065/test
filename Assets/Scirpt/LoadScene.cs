using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
   public string sceneName;
   public void LoadHome()
   {
      SceneManager.LoadScene("Home");
   }

   public void levelScene()
   {
      SceneManager.LoadScene("level");
   }

   public void LoadLevel()
   {
      SceneManager.LoadScene(sceneName);
   }
}
