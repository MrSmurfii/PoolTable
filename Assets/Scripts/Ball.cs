using UnityEngine;

public class Ball : MonoBehaviour {
	public Vector3 velocity;
	public bool isCueBall;
	[HideInInspector]public Vector3 startingPosition;
	
	public Vector3 Position {
		get => transform.position;
		set => transform.position = value;
	}

}
	
	
	
