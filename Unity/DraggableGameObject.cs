using System;
using UnityEngine;


public class DraggableGameObject : MonoBehaviour {

	private Vector3 v3Offset;
	private Plane plane;
	private float scroll;
	void OnMouseDown()
	{
		plane.SetNormalAndPosition(Camera.main.transform.forward,transform.position);
		Ray ray=Camera.main.ScreenPointToRay(Input.mousePosition);
		float dist;
		plane.Raycast(ray,out dist);
		v3Offset=transform.position-ray.GetPoint(dist);
	}
	void OnMouseDrag()
	{
		Ray ray=Camera.main.ScreenPointToRay(Input.mousePosition);
		float dist;
		plane.Raycast(ray,out dist);
		float scrollInner=Input.GetAxis("Mouse ScrollWheel");
		if(Math.Abs(scrollInner)>0.001) scroll=scrollInner;
		//may be 
    dist-=10.0f*scroll;
		Vector3 v3Pos=ray.GetPoint(dist);
		transform.position=v3Pos+v3Offset;
	}
}
