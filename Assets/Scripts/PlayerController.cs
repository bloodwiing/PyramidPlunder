using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;
    private Rigidbody2D rb;

    public bool isFrozen;

    private bool prevState;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isFrozen) return;
        var script = collision.gameObject.GetComponent<PassageScript>();
        if (!script) return;
        script.Teleport(transform);
        FindObjectOfType<AudioManager>().Play("Passage");
    }

    public void UpdateFreeze(bool newState)
    {
        FindObjectOfType<AudioManager>().Stop("Walk");
        if (!newState && prevState)
            FindObjectOfType<AudioManager>().Play("Walk");
        isFrozen = newState;
    }

    void FixedUpdate()
    {
        if (isFrozen) return;
        var control = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

        bool curState = Vector2.Distance(control, Vector2.zero) > 0;
        if (curState && !prevState)
            FindObjectOfType<AudioManager>().Play("Walk");
        if (!curState && prevState)
            FindObjectOfType<AudioManager>().Stop("Walk");
        prevState = curState;

        rb.MovePosition(transform.position * Vector2.one + control * moveSpeed * Time.deltaTime);
    }
}
