using UnityEngine;

public class MoveableVertical : MonoBehaviour
{
	[SerializeField] public bool active = false;

	[SerializeField] private GameObject target;

	Vector3 origin;
	Vector3 distance;
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	private void Awake()
	{
		origin = transform.position;
	}
	void Start()
	{
		distance = transform.position - target.transform.position;
	}

	// Update is called once per frame
	void Update()
	{
		if (active)
		{
			Move();
		}
		if (!active && transform.position != origin)
		{
			MoveBack();
		}
	}

	public void Move()
	{
		Vector3 targetPosition = target.transform.position - distance;
		Vector3 lerpTarget = Vector3.Lerp(origin, targetPosition, 1);
		transform.position = Vector3.Lerp(transform.position, lerpTarget, 2f * Time.deltaTime);
	}

	public void MoveBack()
	{
		transform.position = Vector2.Lerp(transform.position, origin, Time.deltaTime);
	}
}
