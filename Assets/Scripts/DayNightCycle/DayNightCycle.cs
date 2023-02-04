using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    private static DayNightCycle instance = null;
    public static DayNightCycle Instance
    {
        get { return instance;  }
    }
    
    //
    public Light DirectionalLight;
    //0->DayDurationInMinutes + NightDurationInMinutes
    //real time without speed changing applied
    [Header("You can modify these values to set the day and night duration (only at start)")]
    [SerializeField] private float DayDurationInMinutes;
    [SerializeField] private float NightDurationInMinutes;
    
    [Header("Preview only, do not modify !")]
    //calculated values
    [SerializeField] private float CurrentTimeInSeconds;
    [SerializeField] private float RealTimeInSeconds;
    private float TimeSpeed = 0.0f;
    private float DayTimeSpeed = 0.0f;
    private float NightTimeSpeed = 0.0f;
    private float DayDurationInSeconds;
    private float NightDurationInSeconds;
    private float DayNightDurationInSeconds;

    public List<Material> mats;
    public List<Color> colors;
    public Gradient colorGradient;
    public Gradient colorLightGradient;
    public AnimationCurve lightIntensity;

    //0->1, 0->0.5 = day, 0.5->1.0 = night
    public float GetCurrentTimeInRatio()
    {
        return CurrentTimeInSeconds / DayNightDurationInSeconds;
    }

    public float GetCurrentTimeInSeconds()
    {
        return CurrentTimeInSeconds;
    }

    public bool IsDay()
    {
        return GetCurrentTimeInRatio() < 0.5f ? true : false;
    }
    
    //
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        if(DirectionalLight == null)
            Debug.LogError("Directional Light reference is null !");

        DayDurationInSeconds = DayDurationInMinutes * 60.0f;
        NightDurationInSeconds = NightDurationInMinutes * 60.0f;
        DayNightDurationInSeconds = DayDurationInSeconds + NightDurationInSeconds;

        //day speed is reference
        DayTimeSpeed = DayNightDurationInSeconds / 2.0f / DayDurationInSeconds;
        NightTimeSpeed = DayNightDurationInSeconds / 2.0f / NightDurationInSeconds;

        CurrentTimeInSeconds = 0.0f;
        RealTimeInSeconds = 0.0f;
    }

    void IncrementTimeOfTheDay()
    {
        TimeSpeed = IsDay() ? DayTimeSpeed : NightTimeSpeed;
        
        CurrentTimeInSeconds += TimeSpeed * Time.deltaTime;
        if (CurrentTimeInSeconds > DayNightDurationInSeconds)
        {
            CurrentTimeInSeconds -= DayNightDurationInSeconds;
        }

        RealTimeInSeconds += Time.deltaTime;
    }

    void UpdateDirectionalLight()
    {
        float angle = GetCurrentTimeInRatio() * 360.0f;
        
        Quaternion NewQuaternion = Quaternion.Euler(angle, 0.0f, 0.0f);
        DirectionalLight.transform.rotation = NewQuaternion;
    }
    
    private void Update()
    {
        IncrementTimeOfTheDay();
        UpdateDirectionalLight();

        RenderSettings.ambientSkyColor = colorGradient.Evaluate(GetCurrentTimeInRatio());
        RenderSettings.ambientLight = colorLightGradient.Evaluate(GetCurrentTimeInRatio());

        DirectionalLight.intensity = lightIntensity.Evaluate(GetCurrentTimeInRatio());

        if (GetCurrentTimeInRatio() > .1f && GetCurrentTimeInRatio() < .4f)
        {
            RenderSettings.skybox = mats[1];
        }
        else if (GetCurrentTimeInRatio() > .5f && GetCurrentTimeInRatio() < 1f)
        {
            RenderSettings.skybox = mats[2];
        }
        else
        {
            RenderSettings.skybox = mats[0];
        }
    }
}
