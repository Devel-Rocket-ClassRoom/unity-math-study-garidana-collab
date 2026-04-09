
using Microsoft.Unity.VisualStudio.Editor;
using Unity.VisualScripting;
using UnityEngine;

public class OffScreenIndicator : MonoBehaviour
{
    public GameObject image;

    public Camera cam;

    private float margin = 10f;



    // Update is called once per frame
    void Update()
    {
        // 
        Vector3 targetPosition = cam.WorldToScreenPoint(transform.position);
        // targetPostion이 2d 카메라 화면 밖에 위치할 때
        if (targetPosition.z < 0 || targetPosition.x >= Screen.width || targetPosition.y >= Screen.height || targetPosition.x < 0 || targetPosition.y < 0)
        {
            image.SetActive(true);

            if (targetPosition.z < 0)
            {
                targetPosition.x *= -1;
                targetPosition.y *= -1;
            }

            image.transform.position = new Vector3 (
                Mathf.Clamp(targetPosition.x, margin, Screen.width - margin - 50),
                Mathf.Clamp(targetPosition.y, margin, Screen.height - margin - 50),
                0f
            );
        }
        else
        {
            image.SetActive(false);
        }

    }
}
