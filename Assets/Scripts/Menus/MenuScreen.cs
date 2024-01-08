using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuScreen : MonoBehaviour
{
    // Deciding whether or not to show continue
    void Start()
    {

        if (PlayerPrefs.GetInt("Init") == 1) {
            GameObject cont = transform.Find("Continue").gameObject;
            Transform Start = transform.Find("Start");

            cont.SetActive(true);
            Start.localPosition = Vector3.up * 205;
        }

        
    }
}
