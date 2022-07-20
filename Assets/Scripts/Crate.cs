using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate : MonoBehaviour
{
    bool opened;
    public Material opendMaterial;
    public MeshRenderer[] meshRenderers;
    public Animation animation;
    public void openCrate()
    {
        if (!opened)
        {
            foreach (MeshRenderer MR in meshRenderers)
            {
                MR.material = opendMaterial;
            }
            animation.Play();
            opened = true;
        }
    }


}
