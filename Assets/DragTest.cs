using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragTest : MonoBehaviour
{
    public void BeginDrag()
    {
        print("BeginDrag" + gameObject.name);
    }
    public void OnDrag()
    {
        print("OnDrag" + gameObject.name);
    }
    public void EndDrag()
    {
        print("EndDrag" + gameObject.name);
    }

    public void OnDrop()
    {
        print("OnDrop" + gameObject.name);
    }
}
