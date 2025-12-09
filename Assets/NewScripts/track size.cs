using UnityEngine;

public class TrackBounds : MonoBehaviour
{
    public GameObject track;  // Assign your track object in the Unity editor

    void Start()
    {
        if (track != null)
        {
            // Get the Renderer component attached to your track
            Renderer trackRenderer = track.GetComponent<Renderer>();

            if (trackRenderer != null)
            {
                Bounds trackBounds = trackRenderer.bounds;
                Vector3 minBounds = trackBounds.min;
                Vector3 maxBounds = trackBounds.max;

                Debug.Log("Track Min Bounds: " + minBounds);
                Debug.Log("Track Max Bounds: " + maxBounds);
            }
            else
            {
                Debug.LogError("Track does not have a Renderer component.");
            }
        }
    }
}
