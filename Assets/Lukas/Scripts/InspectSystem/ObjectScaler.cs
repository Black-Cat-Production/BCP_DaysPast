using UnityEngine;

public static class ObjectScaler
{
    /// <summary>
    /// Scales the holder so that the object fits the screen vertically based on its bounds.
    /// Uses a multiplier to visually scale objects that are very small or inconsistently sized.
    /// </summary>
    /// <param name="_uiCamera">The camera rendering the UI and 3D object.</param>
    /// <param name="_holder">The dummy root GameObject to position and scale.</param>
    /// <param name="_obj">The 3D model to frame inside the holder.</param>
    /// <param name="_screenHeightFraction">Fraction of vertical screen space the object should fill (e.g. 0.5 = 50%).</param>
    /// <param name="_visualScaleMultiplier">Extra multiplier for visual scaling (e.g. 1 = exact fit, 2 = 2x larger).</param>
    public static void FitObjectInView(Camera _uiCamera, Transform _holder, GameObject _obj, float _screenHeightFraction = 0.6f, float _visualScaleMultiplier = 1.0f)
    {
        // Parent, zero, and reset the object under the holder
        _obj.transform.SetParent(_holder, false);
        _obj.transform.localPosition = Vector3.zero;
        var savedRotation = _obj.transform.rotation;
        _obj.transform.localRotation = Quaternion.identity;
        _obj.transform.localScale = Vector3.one;

        // Calculate total bounds of the object in world space
        var bounds = CalculateBounds(_obj, _holder);
        float objectHeight = bounds.size.y;

        if (objectHeight <= 0.0001f)
        {
            Debug.LogWarning("Object height is too small or zero — cannot scale.");
            return;
        }

        // Compute camera frustum height at object's distance
        float distance = Vector3.Distance(_uiCamera.transform.position, _holder.position);
        float verticalFOV = _uiCamera.fieldOfView;
        float fovRad = verticalFOV * Mathf.Deg2Rad;

        float screenFrustumHeight = 2f * distance * Mathf.Tan(fovRad / 2f);
        float desiredHeight = screenFrustumHeight * _screenHeightFraction;

        // Calculate scale factor and apply it with a visual fudge multiplier
        float scaleFactor = (desiredHeight / objectHeight) * _visualScaleMultiplier;
        _holder.localScale = Vector3.one * scaleFactor;
        _obj.transform.rotation = savedRotation;
    }

    static Bounds CalculateBounds(GameObject _obj, Transform _relativeTo)
    {
        var renderers = _obj.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
            return new Bounds(_obj.transform.position, Vector3.zero);

        var bounds = renderers[0].bounds;
        foreach (Renderer r in renderers)
        {
            bounds.Encapsulate(r.bounds);
        }

        // Convert center to local space of holder
        var localCenter = _relativeTo.InverseTransformPoint(bounds.center);
        return new Bounds(localCenter, bounds.size);
    }
}