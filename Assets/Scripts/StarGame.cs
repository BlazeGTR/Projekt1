using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class StarGame : MonoBehaviour
{

    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            SceneManager.LoadScene("Map1", LoadSceneMode.Single);
        }
        if (Input.GetMouseButtonDown(0))
        {
            SceneManager.LoadScene("Multi_Map1", LoadSceneMode.Single);
        }
    }


}
