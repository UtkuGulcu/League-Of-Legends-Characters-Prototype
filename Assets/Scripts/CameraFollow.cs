using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    #region public variables

    [SerializeField] float scrollSpeed = 8f;

    #endregion

    #region public references



    #endregion

    #region private variables

    bool isLocked;
    Vector3 cameraOffset;

    #endregion

    #region private references

    MouseInfo mouseInfo;

    #endregion

    private void Start()
    {
        cameraOffset = Camera.main.transform.position;
        isLocked = true;
    }

    private void Awake()
    {
        mouseInfo = new MouseInfo();
    }

    void Update()
    {
        ReadInput();
    }

    void LateUpdate()
    {
        FollowCamera();
    }

    void ReadInput()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            isLocked = !isLocked;
        }

        if (Input.GetKey(KeyCode.Space) && !isLocked)
        {
            Camera.main.transform.position = transform.position + cameraOffset;
        }
    }

    void FollowCamera()
    {
        if (isLocked)
        {
            Camera.main.transform.position = transform.position + cameraOffset;
        }
        else
        {
            if (Input.mousePosition.y >= Screen.height * 0.95f)
            {
                Camera.main.transform.Translate(Vector3.forward * Time.deltaTime * scrollSpeed, Space.World);
            }
            if (Input.mousePosition.y <= 54)
            {
                Camera.main.transform.Translate(Vector3.back * Time.deltaTime * scrollSpeed, Space.World);
            }
            if (Input.mousePosition.x >= Screen.width * 0.95f)
            {
                Camera.main.transform.Translate(Vector3.right * Time.deltaTime * scrollSpeed, Space.World);
            }
            if (Input.mousePosition.x <= 96)
            {
                Camera.main.transform.Translate(Vector3.left * Time.deltaTime * scrollSpeed, Space.World);
            }
        }
    }

    public MouseInfo GetMouseInfo()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 999f))
        {
            mouseInfo.mousePosition = hit.point;
            mouseInfo.isOnEnemy = hit.transform.CompareTag("Enemy");
            mouseInfo.hitObject = hit.transform.gameObject;
        }

        return mouseInfo;
    }
}
