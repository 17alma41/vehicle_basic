using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaculeMovement : MonoBehaviour
{
    [SerializeField] Transform objectToMove;
    [SerializeField] Transform[] points;
    int from = 0;
    int to = 1;

    [SerializeField] Color[] colors;
    Material material;

    [SerializeField] AnimationCurve ease;
    [SerializeField] float animationDuration;

    float elapsedTime;

    // Start is called before the first frame update
    void Start()
    {
        material = objectToMove.GetComponent<MeshRenderer>().material;

        StartCoroutine(AnimationLinearInterpolation());
    }


    IEnumerator AnimationLinearInterpolation()
    {

        while (true)
        {
            elapsedTime = 0;

            while (elapsedTime < animationDuration)
            {
                elapsedTime += Time.deltaTime;

                //LerpUnclamped sirve para limitar el resultado entre dos puntos específicos
                //Recorre de un punto hacia otro
                objectToMove.position = Vector3.LerpUnclamped(
                    points[from].position,
                    points[to].position,
                    ease.Evaluate(elapsedTime / animationDuration));

                yield return null;
            }

            IndexCount();

            yield return null;
        }

    }

    void IndexCount()
    {
        from = to;
        to = (to + 1) % points.Length;
    }
}
