using UnityEngine;

public class GrabAndDrop : MonoBehaviour
{
    Camera MainCamera;
    private GameObject _grabbedObject;
    private Transform _grabbedObjectsOldTransform;
    private float _grabbedObjectSize;
    private float _hoverRange = 150.0f;
    // Use this for initialization

    public void Start () {
        MainCamera=Camera.main;
        Debug.Log("Basladim");
    }

    public static Vector3 AddToVector3(Vector3 vector, float x=0, float y=0, float z=0)
    {
        return new Vector3(vector.x + x, vector.y + y, vector.z + z);
    }
    public GameObject GetMouseHoverObject()
    {
        //CurrentGameObject = null;
        Vector3 position = AddToVector3(gameObject.transform.position, 0, 0, 0);
            
        RaycastHit raycastHit;
        Vector3 target = position + MainCamera.transform.forward * _hoverRange;
        //Debug.Log("position:" + position + ",target:" + target);
        Debug.DrawLine(position, target, Color.red);

        //iki secim var

        if (!Physics.Linecast(position, target, out raycastHit)) return null;

        //Ray ray = MainCamera.ScreenPointToRay(Input.mousePosition);
        //if (!Physics.Raycast(ray, out raycastHit)) return null;


        var objectThatBeGrabbed = raycastHit.collider.gameObject;
            
        objectThatBeGrabbed.GetComponent<Renderer>().material.color = Color.red;
            
        if (objectThatBeGrabbed.tag!= "objectThatBeGrabbed")
        {
            Debug.Log(" tuta bilirem ama tutmuram =" + objectThatBeGrabbed);
            return null;
        }
            
        return objectThatBeGrabbed;
    }

         
    public  void TryGrabObject(GameObject grabObject)
    {
        if (grabObject==null)
        {
            return;
        }
        _grabbedObject = grabObject;
        _grabbedObjectSize = grabObject.GetComponent<Renderer>().bounds.size.magnitude;
        _grabbedObjectsOldTransform = _grabbedObject.transform;
        Debug.Log("TryGrabObject");
    }

    public  void DropObect()
    {
        if (_grabbedObject == null) return;
        Debug.Log("Buraxdim");
        _grabbedObject.transform.position = _grabbedObjectsOldTransform.position;
        _grabbedObject.transform.rotation = _grabbedObjectsOldTransform.rotation;
        _grabbedObject.transform.localScale = _grabbedObjectsOldTransform.localScale;
        _grabbedObject = null;
        _grabbedObjectsOldTransform = null;
    }
    // Update is called once per frame
    public void Update()
    {
        if (_grabbedObject != null)
        {
            _grabbedObject.transform.Rotate(0, 0, 5);
            //GUI.Label(new Rect(50, 50, 300, 300), "HELLO WORLD");
        }
        if (!Input.GetMouseButton(0)) return;
        if (_grabbedObject == null)
        {
            TryGrabObject(GetMouseHoverObject());
        }
        else
        {
            DropObect();
        }
        if (_grabbedObject == null) return;
        Vector3 newPosition = gameObject.transform.position + MainCamera.transform.forward * _grabbedObjectSize;
        _grabbedObject.transform.position = newPosition;
    }

    public void OnGUI()
    {
        if (_grabbedObject != null)
        {
            //CurrentGameObject.transform.Rotate(0, 0, 5);
            GUI.Label(new Rect(50, 50, 300, 300), "HELLO WORLD");
        }
    }
}
