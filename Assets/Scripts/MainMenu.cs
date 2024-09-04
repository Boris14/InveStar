using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] float initialInputDelay = 1.5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        initialInputDelay -= Time.deltaTime;

        if (initialInputDelay <= 0 && Input.anyKey)
        {
            SceneManager.LoadScene("SampleScene");
        }
    }
}
