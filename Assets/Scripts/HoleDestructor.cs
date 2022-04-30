using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleDestructor : MonoBehaviour
{
    private void OnBecameInvisible() => Destroy(gameObject);
}
