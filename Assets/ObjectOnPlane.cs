using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
public class ObjectOnPlane : MonoBehaviour
{
    [SerializeField] private GameObject _placedPrefab;
    [SerializeField] private GameObject _selectedObjectPrefab;

    public GameObject SpawnedObject { get; private set; }
    public GameObject PlacedPrefab { get => _placedPrefab; set => _placedPrefab = value; }
    public event Action OnObjectPlacedEvent;

    private ARRaycastManager _raycastManager;
    private static readonly List<ARRaycastHit> _selectHits = new List<ARRaycastHit>();

    private void Awake()
    {
        _raycastManager = GetComponent<ARRaycastManager>();
        OnObjectPlacedEvent += DisableVisual;
    }

    private void Update()
    {
        if (!TryGetTouchPosition(out var position))
            return;

        if (!_raycastManager.Raycast(position, _selectHits, TrackableType.PlaneWithinPolygon)) 
            return;

        var hit = _selectHits.First().pose;
        if (SpawnedObject == null)
        {
            SpawnedObject = Instantiate(_placedPrefab, hit.position, hit.rotation);
        }
        else
        {
            SpawnedObject.transform.position = hit.position;
            SpawnedObject.transform.rotation = hit.rotation;
        }

        OnObjectPlacedEvent?.Invoke();
    }

    private void DisableVisual()
    {
        _selectedObjectPrefab.SetActive(false);
    }

    private static bool TryGetTouchPosition(out Vector2 touch)
    {
        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0).position;
            return true;
        }

        touch = default;
        return false;
    }

    private void OnDestroy()
    {
        OnObjectPlacedEvent -= DisableVisual;
    }
}
