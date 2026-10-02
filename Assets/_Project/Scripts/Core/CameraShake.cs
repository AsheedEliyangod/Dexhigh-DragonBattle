using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private Vector3 originalLocalPosition;
    private Coroutine shakeCoroutine;

    private void Awake()
    {
        originalLocalPosition = transform.localPosition;
    }

    public void Shake(float duration, float strength)
    {
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
        }

        shakeCoroutine = StartCoroutine(
            ShakeRoutine(duration, strength)
        );
    }

    private IEnumerator ShakeRoutine(
        float duration,
        float strength)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float fade =
                1f - (elapsed / duration);

            Vector3 offset =
                Random.insideUnitSphere *
                strength *
                fade;

            // Keep shake mostly horizontal/vertical,
            // not forward/backward.
            offset.z *= 0.35f;

            transform.localPosition =
                originalLocalPosition + offset;

            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.localPosition =
            originalLocalPosition;

        shakeCoroutine = null;
    }

    private void OnDisable()
    {
        transform.localPosition =
            originalLocalPosition;
    }
}