using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomImage : Image
{
    protected override void Awake()
    {
        base.Awake();
        alphaHitTestMinimumThreshold = 0.1f;
    }
}
