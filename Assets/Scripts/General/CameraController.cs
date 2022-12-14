using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    bool mEnforceBounds = false;
    float mZoomAmount;
    [SerializeField]
    float mZoomSpeed;
    [SerializeField]
    float mCameraSpeed;

    [SerializeField]
    Vector2 mXBounds;
    [SerializeField]
    Vector2 mYBounds;
    [SerializeField]
    Vector2 mZBounds;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W)) 
        {
            transform.position += Vector3.forward * mCameraSpeed;
            mEnforceBounds = true;
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position -= Vector3.forward * mCameraSpeed;
            mEnforceBounds = true;
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += Vector3.right * mCameraSpeed;
            mEnforceBounds = true;
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.position -= Vector3.right * mCameraSpeed;
            mEnforceBounds = true;
        }
        mZoomAmount = Input.GetAxis("Mouse ScrollWheel");
        if (mZoomAmount != 0.0f)
        {
            transform.position += transform.forward * mZoomAmount * mZoomSpeed;
            mEnforceBounds = true;
        }

        if (mEnforceBounds)
        {
            EnforceBounds();
        }

    }

    void EnforceBounds()
    {
        mEnforceBounds = false;
        float xPos = Mathf.Clamp(transform.position.x, mXBounds.x, mXBounds.y);
        float yPos = Mathf.Clamp(transform.position.y, mYBounds.x, mYBounds.y);
        float zPos = Mathf.Clamp(transform.position.z, mZBounds.x, mZBounds.y);
        transform.position = new Vector3(xPos, yPos, zPos);
    }
}
