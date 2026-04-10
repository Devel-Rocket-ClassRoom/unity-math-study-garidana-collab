using UnityEngine;
using UnityEngine.iOS;

public class OffScreenIndicator : MonoBehaviour
{
    public GameObject image;

    public Camera cam;

    private float margin = 10f;



    // Update is called once per frame
    void Update()
    {
        // assign position
        Vector3 targetPosition = cam.WorldToScreenPoint(transform.position);
        // targetPostion이 2d 카메라 화면 밖에 위치할 때
        if (targetPosition.z < 0 || targetPosition.x >= Screen.width || targetPosition.y >= Screen.height || targetPosition.x < 0 || targetPosition.y < 0)
        {
            image.SetActive(true);

            if (targetPosition.z < 0f)
            {
                targetPosition.x *= - 1;
                targetPosition.y *= - 1;
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
        

        Vector3 local = cam.transform.InverseTransformPoint(transform.position); // targetPosition도 함께 사용해봐야 할듯
        Vector2 dir = new Vector2(local.x, local.y).normalized;
        Vector2 center = new Vector2 (Screen.width * 0.5f, Screen.height * 0.5f);
        
        float scale = Mathf.Min (center.x / Mathf.Abs(dir.x), center.y / Mathf.Abs(dir.y));
        Vector2 pos = center +dir * scale;
        image.transform.position = new Vector3 (pos.x, pos.y, 0f);


        // float scale = Screen.width;
        // Vector2 pos = center + dir * scale;
        // pos.x = Mathf.Clamp (pos.x, 0f, Screen.width);
        // pos.y = Mathf.Clamp (pos.y, 0f, Screen.height);
        
        // image.transform.position =  new Vector3 (pos.x, pos.y, 0f);
        
    }
}
