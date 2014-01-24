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
    }

    protected void Update()
    {
        Vector3 velocityChange = _desiredSpeed.x * transform.right - _desiredSpeed.y * transform.forward - rigidbody.velocity;
        velocityChange.y = 0;
        rigidbody.velocity += velocityChange * acceleration * Time.deltaTime;

       // print(rigidbody.velocity);


        rigidbody.velocity += Vector3.up*gravity*Time.deltaTime;
    }
}
