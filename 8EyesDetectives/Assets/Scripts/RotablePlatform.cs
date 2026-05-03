using UnityEngine;

public class RotablePlatform : MonoBehaviour
{
    float speed = 2f;

    Vector3 angle = new Vector3(0, 0, 45f);

    public bool rotar;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (rotar)
        {
            Rotate();
        }
    }

    public void Rotate()
    {
		if (transform.eulerAngles.z < 45f)
		{
			transform.Rotate(angle * speed * Time.deltaTime);
		}
	}
}
