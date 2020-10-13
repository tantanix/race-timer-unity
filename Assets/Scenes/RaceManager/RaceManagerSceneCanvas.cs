using UnityEngine;

public class RaceManagerSceneCanvas : MonoBehaviour
{
    void Start()
    {
        var position = transform.position;
        position.z = 1;
        transform.position = position;
    }
}
