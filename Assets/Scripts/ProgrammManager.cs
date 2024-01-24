using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ProgrammManager : MonoBehaviour
{
    private ARRaycastManager ARRaycastManagerScript;
    List<ARRaycastHit> hits = new List<ARRaycastHit>();

    [Header("Put your planeMarker here")]
    [SerializeField] private GameObject PlaneMarkerPrefab;
    public GameObject ObjectToSpawn;
    [Header("Put ScrollView here")]
    public GameObject ScrollView;
    public GameObject SelectedObject;
    [SerializeField] GameObject MaketShell;
    [SerializeField] private GameObject EndText;

    public GameObject traject;
    List<GameObject> lst;


    [SerializeField] private Camera ARCamera;

    private Vector2 TouchPosition;

    private Quaternion YRotation;

    public bool ChooseObject = false;
    public bool Rotation;
    public bool Recharging;

    public int Strikes;

    void Start()
    {
        lst = new List<GameObject>();

        traject.SetActive(false);

        ARRaycastManagerScript = FindObjectOfType<ARRaycastManager>();

        EndText.SetActive(false);

        PlaneMarkerPrefab.SetActive(false);
        ScrollView.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

        //Если объект выбран, то показываем маркер
        if (ChooseObject)
        {
            ShowMarkerAndSetObject();
        }

        MoveObjectAndRotation();

        if (Strikes > 2)
        {
            EndText.SetActive(true);
        }


        if (Recharging)
        {
            MaketShell.SetActive(false);
        }
        else
        {
            MaketShell.SetActive(true);
        }

    }

    void ShowMarkerAndSetObject()
    {
        List<ARRaycastHit> hits = new List<ARRaycastHit>();

        //пускаем лучи из центра экрана для распознавания плоскости
        ARRaycastManagerScript.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), hits, TrackableType.Planes);

        // show marker
        if (hits.Count > 0)
        {
            PlaneMarkerPrefab.transform.position = hits[0].pose.position;
            PlaneMarkerPrefab.SetActive(true);
        }
        // set object
        if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
        {
            var elem = Instantiate(ObjectToSpawn, hits[0].pose.position, ObjectToSpawn.transform.rotation);
            lst.Add(elem);
            MaketShell = GameObject.Find("MaketShell");
            ChooseObject = false;
            PlaneMarkerPrefab.SetActive(false);
        }
    }

   public void Remove_Obj()
   {
        foreach (var x in lst)
            Destroy(x);
        traject.SetActive(false);
       
        ChooseObject = false;
        PlaneMarkerPrefab.SetActive(false);
    }

    void MoveObjectAndRotation()
    {
        if(Input.touchCount > 0)
        {
            //отслеживаем позицию касания 
            Touch touch = Input.GetTouch(0);
            TouchPosition = touch.position;
            
            // чтобы не выделять все объекты при косании(began срабатывает только в момент нажатия)
            if (touch.phase == TouchPhase.Began)
            {
                Ray ray = ARCamera.ScreenPointToRay(touch.position);
                // Контейнер для хранения инфор об объектах, которые пересекают луч
                RaycastHit hitObject; 

                //если мы пересекли объекты
                if (Physics.Raycast(ray, out hitObject))
                {
                    if (hitObject.collider.CompareTag("UnSelected"))
                    {
                        hitObject.collider.gameObject.tag = "Selected";
                    }
                }
            }

            SelectedObject = GameObject.FindWithTag("Selected");

            //срабатывает при движении пальцем по экрану
            if (touch.phase == TouchPhase.Moved && Input.touchCount == 1 )
            {
               // Вращение с одним косанием
                if (Rotation)
                {
                    YRotation = Quaternion.Euler(0f, -touch.deltaPosition.x * 0.1f, 0f);
                    SelectedObject.transform.rotation = YRotation * SelectedObject.transform.rotation;
                }
               // Перемеещение объекта, если rotation = false
                else
                {
                    //фиксируем точки, где пересекается плоскость
                    ARRaycastManagerScript.Raycast(TouchPosition, hits, TrackableType.Planes);
                    //двигаем выбранный объект(его позицию меняем на позицию, где луч пересекся с плоскостью)
                    SelectedObject.transform.position = hits[0].pose.position;
                }
            }
            // Rotate object with 2 fingers
            if (Input.touchCount == 2)
            {
                Touch touch1 = Input.touches[0];
                Touch touch2 = Input.touches[1];

                if (touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved)
                {
                    float DistanceBetweenTouches = Vector2.Distance(touch1.position, touch2.position);
                    float prevDistanceBetweenTouches = Vector2.Distance(touch1.position - touch1.deltaPosition, touch2.position - touch2.deltaPosition);
                    float Delta = DistanceBetweenTouches - prevDistanceBetweenTouches;

                    if (Mathf.Abs(Delta) > 0)
                    {
                        Delta *= 0.1f;
                    }
                    else
                    {
                        DistanceBetweenTouches = Delta = 0;
                    }
                    YRotation = Quaternion.Euler(0f, -touch1.deltaPosition.x * Delta, 0f);
                    SelectedObject.transform.rotation = YRotation * SelectedObject.transform.rotation;
                }

            }
            // При отпускании пальца(снятие выбора)
            if (touch.phase == TouchPhase.Ended)
            {
                if (SelectedObject.CompareTag("Selected"))
                {
                    SelectedObject.tag = "UnSelected";
                }
            }
        }
    }
}
