using UnityEngine;

public class MoveableWeightPlatform : MonoBehaviour
{
    public bool weighted = false;
    public bool unweighted = false;
    public float weight;

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
        if (weighted)
        {
            Move();
        }
        if (unweighted)
        {
            MoveBack();
        }
    }

    public void Move()
    {
		Vector3 targetPosition = target.transform.position - distance;
		Vector3 lerpTarget = Vector3.Lerp(origin, targetPosition, weight);
		transform.position = Vector3.Lerp(transform.position, lerpTarget, 2f * Time.deltaTime);
    }

    public void MoveBack()
    {
        transform.position = Vector2.Lerp(transform.position, origin, Time.deltaTime);
    }
}
