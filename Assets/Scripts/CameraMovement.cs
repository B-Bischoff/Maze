using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Camera _cam;
    [SerializeField] private Transform _target;
    [SerializeField] private float zoomSpeed = 6;
    [SerializeField] private float movementSpeed;

    private Vector3 _previousPosition;
    [SerializeField] private Vector3 _targetPosition;
    [SerializeField] private float _offset = -15;

    public void Awake() => _cam = GetComponent<Camera>();

    public void Start()
    {

    }

    public void Update()
    {

        //Zoom
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            if (_offset < 0)
            {
                _offset += Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
                _cam.transform.position = _targetPosition;
                _cam.transform.Translate(new Vector3(0, 0, _offset));
                _previousPosition = _cam.ScreenToViewportPoint(Input.mousePosition);
            }
            else //Clamping the offset to -1 => Can't enter "in" the terrain with the camera
                _offset = -1;
        }

        //Rotate with left click
        if (Input.GetMouseButtonDown(0) && !Input.GetMouseButton(1))//Left click & not right click
        {
            _previousPosition = _cam.ScreenToViewportPoint(Input.mousePosition);
        }
        if (Input.GetMouseButton(0) && !Input.GetMouseButton(1))
        {
            Vector3 direction = _previousPosition - _cam.ScreenToViewportPoint(Input.mousePosition);

            _cam.transform.position = _targetPosition;

            _cam.transform.Rotate(new Vector3(1, 0, 0), direction.y * 180);
            _cam.transform.Rotate(new Vector3(0, 1, 0), -direction.x * 180, relativeTo: Space.World);
            _cam.transform.Translate(new Vector3(0, 0, _offset));

            _previousPosition = _cam.ScreenToViewportPoint(Input.mousePosition);
        }

        //Translate with right click
        if (Input.GetMouseButtonDown(1) && !Input.GetMouseButton(0))//Right click & not left click
        {
            _previousPosition = _cam.ScreenToViewportPoint(Input.mousePosition);
        }
        if (Input.GetMouseButton(1) && !Input.GetMouseButton(0))
        {
            Vector3 direction = _previousPosition - _cam.ScreenToViewportPoint(Input.mousePosition);

            _cam.transform.Translate(direction * movementSpeed);

            RaycastHit hit;
            var ray = _cam.transform.forward;

            if (Physics.Raycast(_cam.transform.position, ray, out hit))
            {
                Debug.DrawRay(_cam.transform.position, _cam.transform.forward * 100f, Color.yellow);
                _targetPosition = hit.point;
            }

            _offset = -Vector3.Distance(_targetPosition, _cam.transform.position);
            _previousPosition = _cam.ScreenToViewportPoint(Input.mousePosition);
        }
    }
}
