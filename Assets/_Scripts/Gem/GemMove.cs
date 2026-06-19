using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemMove : BaseBehaviour
{
    private Coroutine moveCoroutine;

    public void MoveTo(Vector3 target, float duration)
    {
        if (this.moveCoroutine != null)
        {
            StopCoroutine(this.moveCoroutine);
            this.moveCoroutine = null;
        }

        if (duration <= 0f)
        {
            transform.parent.position = target;
            return;
        }

        this.moveCoroutine = StartCoroutine(this.MoveToRoutine(target, duration));
    }

    private IEnumerator MoveToRoutine(Vector3 target, float duration)
    {
        Vector3 start = transform.parent.position;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            transform.parent.position = Vector3.Lerp(start, target, t);
            yield return null;
        }

        transform.parent.position = target;
        this.moveCoroutine = null;
    }
}
