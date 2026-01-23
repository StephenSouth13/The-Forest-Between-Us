using UnityEngine;

public class playerMove : MonoBehaviour
{
    [SerializeField] private float speed = 5f;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
    }

    public void MovePlayer()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(x, y, 0) * speed * Time.deltaTime;
        transform.Translate(move.normalized);
    }
}
