using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StatusEffectSprites : MonoBehaviour
{
    public static StatusEffectSprites Instance;

    [SerializeField]
    List<Material> materials = new List<Material>();

    [SerializeField]
    List<string> SpriteNames = new();

    [SerializeField]
    Material defaultMaterial;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }


    public Material GetSprite(string spriteName)
    {
        try
        {
            return materials[SpriteNames.IndexOf(spriteName)];
        }
        catch
        {
            return defaultMaterial;
        }
    }




}
