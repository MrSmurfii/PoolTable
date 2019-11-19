using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BilliardsManager : MonoBehaviour {
	[SerializeField] List<Ball> balls;
	[SerializeField]private GameObject cue;
	[SerializeField]private GameObject cueFocus;
	[SerializeField]private float powerMultiplier = 10f;
	private const float SkinWidth = 0.015f;
	private float radius;
	private Ball cueBall;
	private float maxPower = 40f;
	private float forcePower;
	private Camera camera;
	private Vector3 clickStart = Vector3.zero;
	private float gravity = 9f;
	
	private void Start() {
		radius = balls[0].transform.localScale.x * 0.5f;
		camera = Camera.main;
		cue.SetActive(false);
		foreach (Ball ball in balls) {
			ball.startingPosition = transform.position;
			if (ball.isCueBall)
				cueBall = ball;
		}
	}

	private void Update() {
		float sumOfVelocity = 0f;
		foreach (Ball ball in balls) {
			if (!ball.gameObject.activeSelf) continue;
			HandleCollision(ball);
			HandleGravity(ball);
			ball.Position += ball.velocity * Time.deltaTime;
			ball.velocity *= 0.98f;
			if (ball.velocity.magnitude < 0.1f)
				ball.velocity = Vector3.zero;
			sumOfVelocity += ball.velocity.magnitude;
		}
		if (sumOfVelocity <= 0.02f) {
			SpawnCueAndShoot();
		}
		else if (sumOfVelocity >= 0.02f && cue.activeSelf) {
			cue.SetActive(false);
		}
		
		if (Input.GetKeyDown(KeyCode.R))
			Reset();
	}

	void SpawnCueAndShoot() {
		cueFocus.transform.position = cueBall.Position;
		cue.SetActive(true);
			
		Plane plane = new Plane(Vector3.up, cueBall.Position);
		Ray ray = camera.ScreenPointToRay(Input.mousePosition);
		if (!plane.Raycast(ray, out float enter)) return;
		Vector3 mouseWorld = ray.origin + ray.direction * enter;
		Vector3 delta = mouseWorld - cueBall.Position;
		cueFocus.transform.forward = -delta.normalized;
		if (Input.GetMouseButtonDown(0)) {
			clickStart = delta;
		}

	
		
		if (!Input.GetMouseButtonUp(0)) return;
		Vector3 clickEnd = delta;
		float power = (clickEnd - clickStart).magnitude;
		power *= powerMultiplier;
		if (power >= maxPower)
			power = maxPower;
		
		cueBall.velocity = -delta.normalized * power;
	}

	void HandleCollision(Ball ball) {
		int it = 0;
		while (Physics.SphereCast(ball.Position, radius, ball.velocity.normalized, out RaycastHit hit,
			ball.velocity.magnitude * Time.deltaTime + SkinWidth)) {
			if (it++ > 100) {
				break;
			}
			Ball otherBall = hit.collider.GetComponent<Ball>();
			if (otherBall == ball) continue;
			if (otherBall == null) {
				
				Debug.DrawRay(ball.Position, ball.velocity * Time.deltaTime, Color.red, 10f);
				Debug.DrawRay(hit.point, hit.normal, Color.blue, 10f);
				ball.Position = hit.point + hit.normal * (radius + SkinWidth);
				ball.velocity = Vector3.Reflect(ball.velocity, hit.normal);
			}
				
			else if (Vector3.Dot(ball.velocity.normalized, hit.normal) < 0f) {
				Vector3 firstMinusSecondVelocity = ball.velocity - otherBall.velocity;
				Vector3 firstMinusSecondPosition = ball.Position - otherBall.Position;
				Vector3 secondMinusFirstVelocity = otherBall.velocity - ball.velocity;
				Vector3 secondMinusFirstPosition = otherBall.Position - ball.Position;
				
				Vector3 newVelocity1 = ball.velocity -
										Vector3.Dot(firstMinusSecondVelocity, 
											firstMinusSecondPosition) /
										firstMinusSecondPosition.sqrMagnitude *
										firstMinusSecondPosition;

				Vector3 newVelocity2 = otherBall.velocity -
										Vector3.Dot(secondMinusFirstVelocity,
											secondMinusFirstPosition) /
										secondMinusFirstPosition.sqrMagnitude *
										secondMinusFirstPosition;

				float maxMagnitude = ball.velocity.magnitude + otherBall.velocity.magnitude;
				float maxMagnitudeAfter = newVelocity1.magnitude + newVelocity2.magnitude;

				otherBall.velocity = (maxMagnitude * newVelocity2.magnitude / maxMagnitudeAfter * newVelocity2.normalized) ;
				ball.velocity = (maxMagnitude * newVelocity1.magnitude / maxMagnitudeAfter * newVelocity1.normalized);
					
			}
		}
	}

	private void Reset() {
		SceneManager.LoadScene("SampleScene");
	}

	void HandleGravity(Ball ball) {
		if (!Physics.SphereCast(ball.Position, radius, Vector3.down, out RaycastHit hit,
			 Time.deltaTime + SkinWidth)) {
			ball.velocity += Time.deltaTime * gravity * Vector3.down;
		}
		else {
			ball.velocity.y = 0f;	
		}
	}
	
}

	
