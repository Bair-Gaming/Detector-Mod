using UnityEngine;
using BepInEx;
using System.Linq;
using System.Collections;

namespace Detector.Behaviours {

internal class DetectorItem : PhysicsProp
{
    public enum DetectorStatus
    {
        OFF,
        HEALTHY,
        UNHEALTHY,
        CRITICAL // ?
    }

    private DetectorStatus status = DetectorStatus.OFF;
    private bool inRange = false;
    private Light indicator;
    [SerializeField] private float detectionRange = 20;
    private Material red, green;
    private MeshRenderer onOffIndicator;

    private AudioSource detectSound;

    private AnimationCurve beepDelayCurve;
    private float beepDelay = 0f, currentDist = 10f;
    private bool playSound = false;

    public override void Start() {
        base.Start();

        beepDelayCurve = AnimationCurve.EaseInOut(0, 10f, detectionRange, 0.15f);

        indicator = GetComponentInChildren<Light>();
        red = transform.GetChild(3).GetChild(0).GetComponent<MeshRenderer>().material;
        green = transform.GetChild(3).GetChild(1).GetComponent<MeshRenderer>().material;
        onOffIndicator = transform.GetChild(0).GetChild(1).GetComponent<MeshRenderer>();

        detectSound = transform.GetChild(4).GetChild(0).GetComponent<AudioSource>();
    }

    public override void DiscardItem() {
        playSound = false;

        base.DiscardItem();
    }

    public override void EquipItem() {
        playSound = true;

        base.EquipItem();
    }
    public override void ItemActivate(bool used, bool buttonDown = true)
    {
        base.ItemActivate(used, buttonDown);
        isBeingUsed = !isBeingUsed;
        playSound = isBeingUsed;
        Debug.Log(playSound);

        status = (isBeingUsed == false || insertedBattery.charge <= 0) ? DetectorStatus.OFF : (insertedBattery.charge > .35f ? DetectorStatus.HEALTHY : DetectorStatus.UNHEALTHY);


        if(!isBeingUsed) {
            indicator.enabled = false;
        }
        
        if(isBeingUsed && status != DetectorStatus.OFF) {
            onOffIndicator.material = green;
        }
        else if(!isBeingUsed || status == DetectorStatus.OFF) {
            onOffIndicator.material = red;
        }
    }
        public override void Update()
        {
            base.Update();
            var contacts = Physics.OverlapSphere(transform.position, detectionRange, LayerMask.GetMask("Enemies"));

            if(contacts.Length > 0) 
            {
                // string str = "";
                // contacts.ToList().ForEach((a) => str += (a.gameObject.name + ", "));
                // Debug.Log("enemy(ies)in range:" + str);
                inRange = true;
            }

            if(inRange) {
                if(indicator.enabled == false && isBeingUsed == true) {
                    indicator.enabled = true;
                    indicator.intensity = 0;
                }
                else if(playSound && isBeingUsed && contacts.Length > 0) {
                    float dist = Vector3.Distance(transform.position, contacts[0].transform.position);
                    float x = detectionRange - dist;
                    indicator.intensity = 0.1f * x*x;

                    if(beepDelay <= 0) {
                        Debug.Log("BEEPE");
                        detectSound.Play();
                        currentDist = dist;
                        beepDelay = beepDelayCurve.Evaluate(currentDist);
                    }
                }   
            }
            else {
                if(!indicator.enabled && isBeingUsed && status == DetectorStatus.UNHEALTHY) {
                    Debug.Log("twtf, " + status);
                    float rand = Random.Range(0, 100);
                    bool brk = rand > 95;
                    beepDelay -= beepDelayCurve.Evaluate(rand) * Time.deltaTime;

                    
                    if(brk) {
                        StartCoroutine(detectNothing());
                    }
                }
            }

            beepDelay -=  beepDelayCurve.Evaluate(currentDist) * Time.deltaTime;
        }

        private IEnumerator detectNothing() {
            Debug.Log("BREAK");
            float duration = Random.Range(2, 10);
            while(duration > 0) {
                indicator.intensity = Random.Range(0, 40);
                duration -= Time.deltaTime;
                yield return 0; 
            }

            yield return null;
        }


        public override void UseUpBatteries()
        {
            base.UseUpBatteries();
            indicator.enabled = false;
        }

    }
}