using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class S_CameraManager : MonoBehaviour
{
    private CinemachineImpulseSource impulseSource;
    private List<CinemachineVirtualCamera> cameras = new List<CinemachineVirtualCamera>();
    private CinemachineVirtualCamera currentCam;
    private CinemachineBrain cameraBrain;

    private float defaultBlendDuration;
    private CinemachineBlendDefinition.Style defaultBlendStyle;

    public static S_CameraManager Instance { get; private set;}

    private GameObject canvas;
    private Image fadeImage;

    public AnimationCurve fadeOutCurve;
    public AnimationCurve fadeInCurve;
    private FadeTypes currentFade;

    private float startTime;
    private float endTime;

    enum FadeTypes
    {
        NotFading,
        FadingOut,
        FadingIn,
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        impulseSource = GetComponent<CinemachineImpulseSource>();
        cameraBrain = GetComponent<CinemachineBrain>();

        if (cameraBrain != null)
        {
            defaultBlendDuration = cameraBrain.m_DefaultBlend.m_Time;
            defaultBlendStyle = cameraBrain.m_DefaultBlend.m_Style;
        }

        canvas = GetComponentInChildren<Canvas>().gameObject;
        fadeImage = GetComponentInChildren<Image>();

        canvas.SetActive(false);
        currentFade = FadeTypes.NotFading;
    }

    private void Update()
    {
        if (currentFade == FadeTypes.FadingOut)
        {
            float p = Mathf.Clamp01((Time.time - startTime) / (endTime - startTime));
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, fadeOutCurve.Evaluate(p));

            if (p >= 1f)
            {
                EndFade();
            }
        }

        else if (currentFade == FadeTypes.FadingIn)
        {
            float p = Mathf.Clamp01((Time.time - startTime) / (endTime - startTime));
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, fadeInCurve.Evaluate((1f - p)));

            if (p >= 1f)
            {
                EndFade();
            }
        }
    }

    public void ShakeCamera(float force, float duration, CinemachineImpulseDefinition.ImpulseShapes shape, Vector3 direction)
    {
        if (impulseSource != null)
        {

            impulseSource.m_ImpulseDefinition.m_ImpulseDuration = duration;
            impulseSource.m_ImpulseDefinition.m_ImpulseShape = shape;
            impulseSource.m_DefaultVelocity = direction;

            impulseSource.GenerateImpulseWithForce(force);
        }       
    }

    public void FadeOut(float duration)
    {
        if (currentFade == FadeTypes.NotFading)
        {           
            currentFade = FadeTypes.FadingOut;

            startTime = Time.time;
            endTime = startTime + duration;

            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 0f);
            canvas.SetActive(true);
        }
    }

    public void FadeIn(float duration)
    {
        if (currentFade == FadeTypes.NotFading)
        {
            currentFade = FadeTypes.FadingIn;

            startTime = Time.time;
            endTime = startTime + duration;

            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 1f);
            canvas.SetActive(true);
        }
    }

    private void EndFade()
    {
        if (currentFade == FadeTypes.FadingOut)
        {
            currentFade = FadeTypes.NotFading;
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 1f);
        }

        else if (currentFade == FadeTypes.FadingIn)
        {
            currentFade = FadeTypes.NotFading;
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 0f);
            canvas.SetActive(false);
        }
    }

    public void RegisterCamera(CinemachineVirtualCamera newCam)
    {
        if (newCam != null)
        {
            if (!cameras.Contains(newCam))
            {
                cameras.Add(newCam);

                if (currentCam == null)
                {
                    currentCam = newCam;
                }

                else
                {
                    if (newCam.Priority > currentCam.Priority)
                    {
                        currentCam = newCam;
                    }
                }
                
                Debug.Log("Added new cam.");
            }
        }
    }

    public void SwitchToCamera(CinemachineVirtualCamera newCam)
    {
        if (newCam == currentCam)
        {
            Debug.Log("Cannot switch to camera because it's already active.");
            return;
        }

        ResetBlend();

        newCam.Priority = 100;
        
        foreach (CinemachineVirtualCamera c in cameras.ToArray())
        {
            if (c == newCam)
            {
                continue;
            }
            
            if (c == currentCam)
            {
                c.Priority = 1;
            }

            else
            {
                c.Priority = 0;
            }        
        }

        newCam.Priority = 2;
        currentCam = newCam;
    }

    public void SwitchToCamera(CinemachineVirtualCamera newCam, float duration, CinemachineBlendDefinition.Style style)
    {
        if (newCam == currentCam)
        {
            Debug.Log("Cannot switch to camera because it's already active.");
            return;
        }

        if (cameraBrain != null)
        {
            cameraBrain.m_DefaultBlend.m_Time = duration;
            cameraBrain.m_DefaultBlend.m_Style = style;
        }

        newCam.Priority = 100;

        foreach (CinemachineVirtualCamera c in cameras.ToArray())
        {
            if (c == newCam)
            {
                continue;
            }

            if (c == currentCam)
            {
                c.Priority = 1;
            }

            else
            {
                c.Priority = 0;
            }
        }

        newCam.Priority = 2;
        currentCam = newCam;
    }

    public void SwitchToPreviosCamera()
    {
        CinemachineVirtualCamera foundCam = null;

        foreach (CinemachineVirtualCamera c in cameras.ToArray())
        {
            if (c.Priority == 1 && c != currentCam)
            {
                foundCam = c;
                break;
            }
        }

        if (foundCam != null)
        {
            SwitchToCamera(foundCam);
        }

    }

    public void SwitchToPreviosCamera(float duration, CinemachineBlendDefinition.Style style)
    {
        CinemachineVirtualCamera foundCam = null;

        foreach (CinemachineVirtualCamera c in cameras.ToArray())
        {
            if (c.Priority == 1 && c != currentCam)
            {
                foundCam = c;
                break;
            }
        }

        if (foundCam != null)
        {
            SwitchToCamera(foundCam, duration, style);
        }

    }

    public void ResetCameraPosition()
    {
        if (currentCam != null)
        {
            if (currentCam.Follow != null)
            {
                Debug.Log("Had Follow");
                Transform previousFollow = currentCam.Follow;
                currentCam.Follow = null;

                currentCam.ForceCameraPosition(new Vector3(previousFollow.transform.position.x, previousFollow.transform.position.y, currentCam.transform.position.z), currentCam.transform.rotation);
                currentCam.Follow = previousFollow;
            }
        }
    }

    private void ResetBlend()
    {
        if (cameraBrain != null)
        {
            cameraBrain.m_DefaultBlend.m_Time = defaultBlendDuration;
            cameraBrain.m_DefaultBlend.m_Style = defaultBlendStyle;
        }
    }

}
