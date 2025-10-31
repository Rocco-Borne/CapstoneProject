using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinCondition : MonoBehaviour
{
    private int enemyKilled;
    [SerializeField] GameObject portal;
    // Update is called once per frame
    void Update()
    {
        if (enemyKilled >= 30)
        {
            portal = Instantiate(portal, new Vector3(0, 0, 0), Quaternion.identity);
        }

    }

}
