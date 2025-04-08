using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    public void DestroyMe()
    {
        Destroy(this.gameObject, 0.2f);
    }
}
