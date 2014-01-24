using UnityEngine;
using System.Collections;

public class Moveable : MonoBehaviour
{
    public float speed;
    public float acceleration;
    public float deceleration;

    public float gravity = -10;
    public float jumpSpeed = 10;

    private Vector2 _desiredSpeed;
    private float _rotation;
    private float _timeSinceGrounded;

    public void SetDesiredInput(Vector2 input)
    {
        _desiredSpeed = input * speed;
    }
    public void LookRelative(float angle)
    {
        _rotation += angle;
        transform.rotation = Quaternion.Euler(0, _rotation, 0);
    }

    public void Jump()
    {
        StartCoroutine(JumpCoroutine());
    }

    private IEnumerator JumpCoroutine()
    {
        for (float t = 0; t < 0.2f && _timeSinceGrounded > 0.1f; t += Time.deltaTime)
            yield return null;

        if (_timeSinceGrounded > 0.1f)
            yield break;
        rigidbody.velocity += (jumpSpeed - rigidbody.velocity.y) * Vector3.up;
    }

    protected void Update()
    {
        Vector3 velocityChange = _desiredSpeed.x * transform.right - _desiredSpeed.y * transform.forward - rigidbody.velocity;
        velocityChange.y = 0;
        rigidbody.velocity += velocityChange * acceleration * Time.deltaTime;


        rigidbody.velocity += Vector3.up*gravity*Time.deltaTime;

        _timeSinceGrounded += Time.deltaTime;
    }

    protected void OnCollisionStay(Collision col)
    {
        bool above = false;
        
        foreach (var point in col.contacts)
        {
            if (point.normal.y > 0.5f && point.point.y < collider.bounds.center.y)
            {
                above = true;
            }
        }
        if (above && rigidbody.velocity.y <= 0.01f)
            _timeSinceGrounded = 0;
    }
}
