using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdpersonCharController : MonoBehaviour
{
    public float speed;
    public string team;
    // Start is called before the first frame update
    void Start()
    {
        team = "Red";
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMovement();
    }

    void PlayerMovement()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        Vector3 playerMove = new Vector3(x, 0f, y) * speed * Time.deltaTime;
        transform.Translate(playerMove, Space.Self);
    }
}
