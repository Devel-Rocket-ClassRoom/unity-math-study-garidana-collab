using NUnit.Framework;
using UnityEngine;

public class DragObject : MonoBehaviour
{
    public float timeReturn = 2f;
    public bool isReturning;

    public Vector3 originalPos;
    private Vector3 startPosition;
    private Terrain terrain;
    private float timer;

    private void Start()
    {
        terrain = Terrain.activeTerrain;
    }

    private void ResetDrag()
    {
        isReturning = false;
        timer = 0f;
        originalPos = Vector3.zero;
        startPosition = Vector3.zero;
    }

    public void DragStart()
    {
        // isReturning = false;
        // timer = 0f;
        ResetDrag();
        originalPos = transform.position;
    }

    public void DragEnd()
    {
        ResetDrag();
    }

    public void Return()
    {
        timer = 0f;
        isReturning = true;
        startPosition = transform.position;
    }

    private void Update()
    {
        if (isReturning)
        {
            timer += Time.deltaTime / timeReturn; 
            Vector3 newPos = Vector3.Lerp(startPosition, originalPos, timer);
            newPos.y = terrain.SampleHeight(newPos);
            transform.position = newPos;

            if (timer > 1f)
            {
                isReturning = false;
                transform.position = originalPos;
                timer = 0f;
            }
        }
    }    
}
