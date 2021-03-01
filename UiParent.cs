using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiParent : MonoBehaviour
{
    [SerializeField]
    RectTransform parent;
    RectTransform thisRect;
    Vector2 startOffsetRect;
    [SerializeField]
    bool forceMode = false;
    [SerializeField]
    bool ignoreOffset = false;
    [SerializeField]
    bool ignoreRotation = false;
    Vector3 startOffsetPos;
    [SerializeField]
    bool isAnimated = false;
    // Start is called before the first frame update
    void Start()
    {
        thisRect = this.transform.GetComponent<RectTransform>();
        startOffsetRect = thisRect.anchoredPosition - parent.anchoredPosition;
        startOffsetPos = thisRect.transform.position - parent.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAnimated)
        {
            if (!forceMode)
                thisRect.anchoredPosition = parent.anchoredPosition + (ignoreOffset ? Vector2.zero : startOffsetRect);
            else
            {
                thisRect.transform.position = parent.transform.position + (ignoreOffset ? Vector3.zero : startOffsetPos);
                if (ignoreRotation)
                    thisRect.transform.rotation = parent.transform.rotation;
            }
        }
    }
    private void LateUpdate()
    {
        if (isAnimated)
        {
            if (!forceMode)
                thisRect.anchoredPosition = parent.anchoredPosition + (ignoreOffset ? Vector2.zero : startOffsetRect);
            else
            {
                thisRect.transform.position = parent.transform.position + (ignoreOffset ? Vector3.zero : startOffsetPos);
                if (ignoreRotation)
                    thisRect.transform.rotation = parent.transform.rotation;
            }
        }
    }
}
