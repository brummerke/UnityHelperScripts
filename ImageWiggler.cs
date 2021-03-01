using UnityEngine;
using UnityEngine.UI;

public class ImageWiggler : MonoBehaviour
{
    public bool wiggleAlpha;
    private Image imageRend;
    private Vector4 startColor;
    public float alphaWiggleSpeed;
    public float alphaWiggleAmp;
    public bool wigglePos;
    private RectTransform imageRect;

    public float posWiggleSpeed;
    public Vector3 posWiggleAmp;
    public Vector2 startAnchoredPos;
    public bool isAnimated;

    private float[] rndSeeds = new float[10];
    [SerializeField]
    private Animator imageAnimator;

    // Use this for initialization
    private void Start()
    {
        if (isAnimated)
            if (!imageAnimator)
                imageAnimator = this.transform.GetComponent<Animator>();
        if (!imageAnimator)
            imageAnimator = this.transform.GetComponentInParent<Animator>();

        imageRect = this.transform.GetComponent<RectTransform>();
        for (int i = 0; i < rndSeeds.Length; i++)
        {
            rndSeeds[i] = Random.Range(-1000f, 1000f);
        }

        if (wiggleAlpha)
        {
            imageRend = this.transform.GetComponent<Image>();
            startColor = imageRend.color;
        }

        startAnchoredPos = imageRect.anchoredPosition;
    }

    private void Update()
    {
        if (!isAnimated)
        {
            if (wigglePos)
                WigglePos();
        }
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        if (isAnimated)
        {
            if (imageAnimator != null && imageAnimator.enabled)
                if (wigglePos)
                    WigglePos();
        }
        if (wiggleAlpha)
            WiggleAlpha();
    }

    private void WigglePos()
    {
        // imageRect.anchoredPosition = startAnchoredPos;
        if (!isAnimated)
            imageRect.anchoredPosition = startAnchoredPos;
        imageRect.anchoredPosition += new Vector2(Wiggle(rndSeeds[3], rndSeeds[4], posWiggleAmp.x, posWiggleSpeed), Wiggle(rndSeeds[5], rndSeeds[6], posWiggleAmp.y, posWiggleSpeed));
        //octave imageRect.anchoredPosition += new Vector2(Wiggle(rndSeeds[1], rndSeeds[2], posWiggleAmp.x, posWiggleSpeed), Wiggle(rndSeeds[7], rndSeeds[8], posWiggleAmp.y * 0.5f, posWiggleSpeed * 2f));
    }

    private void WiggleAlpha()
    {
        Vector4 a = startColor;
        a.w += Wiggle(rndSeeds[0], rndSeeds[1], alphaWiggleAmp, alphaWiggleSpeed);
        imageRend.color = a;
    }

    private float Wiggle(float seed1, float seed2, float amp, float freq)
    {
        float val = 0f;

        val += ((Mathf.PerlinNoise((Time.unscaledTime + seed1) * freq, (Time.unscaledTime + seed2) * freq) - 0.5f) * 2f) * amp;
        // val += Mathf.Sin((Time.time + seed + i* (Mathf.PI)) * freq + ((float)octaves * Mathf.PI)) * (amp / (float)octaves);

        return val;
    }
}