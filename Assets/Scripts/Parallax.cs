using UnityEngine;

public class Parallax : MonoBehaviour
{
    public bool loop;
    private Camera cam;
    private Vector2 startpos;
    private Vector2 size;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        startpos = transform.position;
        size = GetComponent<SpriteRenderer>().bounds.size;
    }

    // Update is called once per frame
    void Update()
    {
        float parallaxFactor = transform.position.z / 100;
        Vector2 camCenter = cam.transform.position * (1 - parallaxFactor);
        float camSize = cam.orthographicSize;
        Vector2 distance = cam.transform.position * parallaxFactor;
 
        Vector3 newPosition = startpos + distance;
        newPosition.z = transform.position.z;

        transform.position = newPosition;//PixelPerfectClamp(newPosition, 32);
 
        if (loop)
        {
            if (camCenter.x - camSize > startpos.x + (size.x / 2))      startpos.x += size.x;
            else if (camCenter.x - camSize < startpos.x - (size.x / 2)) startpos.x -= size.x;
            if (camCenter.y - camSize > startpos.y + (size.y / 2))      startpos.y += size.y;
            else if (camCenter.y - camSize < startpos.y - (size.y / 2)) startpos.y -= size.y;
        }
    }
}
