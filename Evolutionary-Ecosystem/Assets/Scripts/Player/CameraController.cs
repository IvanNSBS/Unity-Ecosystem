using UnityEngine;

public class CameraController : MonoBehaviour {
    [SerializeField] private Camera m_Camera;
    [SerializeField] private float m_CameraSpeed = 2.0f;
    [SerializeField] private float m_ZoomSpeed = 0.5f;
    [HideInInspector] public Vector3 dragOrigin;

    private void MoveCamera()
    {
        if (Input.GetMouseButtonDown(1))
        {
            dragOrigin = Input.mousePosition;
            return;
        }
 
        if (!Input.GetMouseButton(1)) return;
 
        Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
        Vector3 move = new Vector3(pos.x * m_CameraSpeed, pos.y * m_CameraSpeed, 0) * Time.deltaTime;
 
        transform.Translate(move, Space.World); 
    }

    private void Zoom(float zoomAmnt)
    {
        m_Camera.orthographicSize += zoomAmnt * m_ZoomSpeed;
        
    }

    private void SelectObject()
    {
        if(Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection (ray, Mathf.Infinity);
            if(hit.collider != null)
                Debug.Log(hit.collider.gameObject);
        }
    }

    private void LateUpdate() 
    {
        Zoom(-Input.mouseScrollDelta.y);
        MoveCamera();
        SelectObject();
    }
}