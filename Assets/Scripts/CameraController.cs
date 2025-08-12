using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
	private float _zoom;
	private float _zoomSpeed;

	private CinemachineCamera _camera;

	// Inspector
	[SerializeField] private float _startingZoom;
	[SerializeField] private float _scrollMultiplier = 50;
	[SerializeField] private float _zoomInDivider;
	[SerializeField] private float _scrollSmoothing = 0.25f;
	[SerializeField] private float _minZoom = 10;
	[SerializeField] private float _maxZoom = 150;

	private void Start()
	{
		_camera = GetComponent<CinemachineCamera>();
		_zoom = _startingZoom;
	}

	private void Update() {
		UpdateZoom();
	}
	
	private void UpdateZoom() {
		// Get the user input
		bool canZoom = !EventSystem.current.IsPointerOverGameObject();
		float input = canZoom ? Input.GetAxis("Mouse ScrollWheel") : 0;
		float scroll = input * _camera.Lens.OrthographicSize;

		if (input > 0) scroll /= _zoomInDivider;

		// Apply the zoom
		_zoom -= scroll * _scrollMultiplier;
		_zoom = Mathf.Clamp(_zoom, _minZoom, _maxZoom);

		_camera.Lens.OrthographicSize =
			Mathf.SmoothDamp(_camera.Lens.OrthographicSize, _zoom, ref _zoomSpeed, _scrollSmoothing);
	}
}
