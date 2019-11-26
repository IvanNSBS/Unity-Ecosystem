using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CameraController : MonoBehaviour {

    public static CameraController instance;

    [SerializeField] private Camera m_Camera;
    [SerializeField] private float m_CameraSpeed = 2.0f;
    [SerializeField] private float m_ZoomSpeed = 0.5f;
    [HideInInspector] public Vector3 dragOrigin;

    [HideInInspector] public GameObject selectedAnimal;
    private bool following = false;


    [Header("Data UI")]
    [SerializeField] TextMeshProUGUI text_wolf_pop;
    [SerializeField] TextMeshProUGUI text_rabbit_pop;
    [SerializeField] TextMeshProUGUI text_veg_pop;
    [Header("Animal UI")]
    [SerializeField] GameObject ui_canvas;
    [SerializeField] Image animalSprite;
    [SerializeField] Image slider_hunger;
    [SerializeField] Image slider_thirst;
    [SerializeField] Image slider_urge;
    [SerializeField] TextMeshProUGUI text_hunger;
    [SerializeField] TextMeshProUGUI text_thirst;
    [SerializeField] TextMeshProUGUI text_urge;
    [SerializeField] TextMeshProUGUI text_state;
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
 
        transform.Translate(-move*m_Camera.orthographicSize*0.75f, Space.World); 
    }

    private void Zoom(float zoomAmnt)
    {
        m_Camera.orthographicSize += zoomAmnt * m_ZoomSpeed;
        
    }

    private void SelectObject()
    {
        if(Input.GetMouseButtonDown(0))
        {
            selectedAnimal = null;
            Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection (ray, Mathf.Infinity);
            if(hit.collider != null){
                var agent = hit.collider.gameObject.GetComponent<Agent>();
                if(agent)
                    selectedAnimal = hit.collider.gameObject;
            }
            ui_canvas.SetActive(selectedAnimal != null);
        }
    }

    private void FollowSelected(){
        if(selectedAnimal){
            var x = selectedAnimal.transform.position.x;
            var y = selectedAnimal.transform.position.y;
            var z = m_Camera.transform.position.z;
            m_Camera.transform.position = new Vector3(x,y,z);
        }
    }

    private void UpdateUI()
    {
        if(!selectedAnimal)
            return;
        
        var life = selectedAnimal.GetComponent<LifeComponent>();
        animalSprite.sprite = selectedAnimal.GetComponent<SpriteRenderer>().sprite;
        animalSprite.material = selectedAnimal.GetComponent<SpriteRenderer>().material;
        slider_hunger.fillAmount = life.m_CurrentHunger;
        slider_thirst.fillAmount = life.m_CurrentThirst;
        slider_urge.fillAmount = life.m_CurrentReproductionUrge;
        text_hunger.text = "Hunger: " + (int)(life.m_CurrentHunger*100)+"%";
        text_thirst.text = "Thirst: " + (int)(life.m_CurrentThirst*100)+"%";
        text_urge.text = "Repr. Urge: " + (int)(life.m_CurrentReproductionUrge*100)+"%";
        text_state.text = "Current State: " + selectedAnimal.GetComponent<Agent>().m_FSM.state.ToString();
    }

    public void UpdateDataUI(int wolfpop, int rabbitpop, int foodpop)
    {
        text_wolf_pop.text = "Current Wolf Population: " + wolfpop.ToString();
        text_rabbit_pop.text = "Current Rabbit Population: " + rabbitpop.ToString();
        text_veg_pop.text = "Current Vegetal Population: " + foodpop.ToString();
    }

    private void Awake() {
        ui_canvas.SetActive(false);
        instance = this;
    }

    public void Scale1x()
    {
        Time.timeScale = 1.0f;
    }
    public void Scale2x()
    {
        Time.timeScale = 2.0f;
    }
    public void Scale3x()
    {
        Time.timeScale = 3.0f;
    }
    public void ScaleHalf()
    {
        Time.timeScale = 0.5f;
    }
    public void ScaleQuarter()
    {
        Time.timeScale = 0.25f;
    }
    private void LateUpdate() 
    {
        if(Input.GetKeyDown(KeyCode.F) && selectedAnimal){
            following = !following;
        }

        Zoom(-Input.mouseScrollDelta.y);
        if(!following)
            MoveCamera();
        else
            FollowSelected();
        SelectObject();
        UpdateUI();

        if(Input.GetKeyDown(KeyCode.Alpha1))
            Time.timeScale = 1.0f;

        if(Input.GetKeyDown(KeyCode.Alpha2))
            Time.timeScale = 2.0f;

        if(Input.GetKeyDown(KeyCode.Alpha3))
            Time.timeScale = 3.0f;

        if(Input.GetKeyDown(KeyCode.Alpha4))
            Time.timeScale = 0.5f;

        if(Input.GetKeyDown(KeyCode.Alpha5))
            Time.timeScale = 0.25f;
    }
}