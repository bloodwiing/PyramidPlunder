using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var script = collision.gameObject.GetComponent<PassageScript>();
        if (!script) return;
        script.Teleport(transform);
    }

    void FixedUpdate()
    {
        var control = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        rb.MovePosition(transform.position * Vector2.one + control * moveSpeed * Time.deltaTime);
    }
}
