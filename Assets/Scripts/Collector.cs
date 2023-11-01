using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collector : MonoBehaviour
{
    public int breadNum = 0;
    public void ReceiveBread()
    {
        breadNum++;
    }

}
