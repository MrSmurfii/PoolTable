using UnityEngine;

public class Ball : MonoBehaviour {
	[HideInInspector]public Vector3 velocity;
	public bool isCueBall;
	public Vector3 Position {
		get => transform.position;
		set => transform.position = value;
	}

}
	
	
	
