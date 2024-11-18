using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public struct Gesture
{
    public string name;
    public Color DetectionColor;
    public List<Vector3> fingerData;
    public UnityEvent onRecognized;
}

public class GestureDetector : MonoBehaviour // Duplicate the script for each hand ( change assignment ) 
{
    public bool DebugMode = false;
    public float threshhold = 0.05f;
    public OVRSkeleton skeleton;
    public bool CaplampActive = false;
    public List<Gesture> gestures;
    public TeleportPlayer playerRef;
    public Color clrGreen;
    private Color clrDefault;
    private List<OVRBone> fingerBones;
    private Gesture previousGesture;

    public bool canMove = false;

    // Start is called before the first frame update
    void Start()
    {
        fingerBones = new List<OVRBone>(skeleton.Bones);
        previousGesture = new Gesture();
        clrDefault = skeleton.gameObject.GetComponent<Renderer>().material.color;
    }

    // Update is called once per frame
    void Update()
    {
        skeleton.gameObject.GetComponent<Renderer>().material.color = clrDefault;

        if (DebugMode && Input.GetKeyDown(KeyCode.Space))
        {
            Save();
        }

        Gesture currentGesture = Recognise();
        bool hasRecognised = !currentGesture.Equals(new Gesture());
        if(hasRecognised && !currentGesture.Equals(previousGesture))
        {
            //Debug.Log("New Gesture Found : " + currentGesture.name);
            previousGesture = currentGesture;
            currentGesture.onRecognized.Invoke();
        }
        if (!canMove) return;
        
        switch(currentGesture.name)
        {
            case "Finger Point":
                skeleton.gameObject.GetComponent<Renderer>().material.color = currentGesture.DetectionColor;
                playerRef.MoveForward();
                break;
            case "ThumbPinky":
                skeleton.gameObject.GetComponent<Renderer>().material.color = currentGesture.DetectionColor;
                playerRef.MoveBackwards();
                break;
            case "Fist":
                skeleton.gameObject.GetComponent<Renderer>().material.color = currentGesture.DetectionColor;
                //Seems to trigger onStart until hands pickup
                break;
            case "Pinky":
                skeleton.gameObject.GetComponent<Renderer>().material.color =currentGesture.DetectionColor;
                playerRef.RotatePlayerLeft();
                break;
            case "Thumb Up":
                skeleton.gameObject.GetComponent<Renderer>().material.color = currentGesture.DetectionColor;
                playerRef.RotatePlayerRight();
                break;
            default:
                //skeleton.gameObject.GetComponent<Renderer>().material.color = clrDefault;
                //playerRef.isRotating = false;
                break;
        }
    }
    public void CanMove(bool _canMove)
    {
        canMove = _canMove;
    }

    public void ToggleGameObject(GameObject g)
    {
        if(CaplampActive)
        {
            g.SetActive(false);
            CaplampActive = false;
        }
        else if(!CaplampActive)
        {
            g.SetActive(true);
            CaplampActive = true;
        }
    }

    void Save() // In play mode, make hand gesture and press space. Copy component and rename it to save it
    {
        Gesture g = new Gesture();
        g.name = "New Gesture";
        List<Vector3> data = new List<Vector3>();
        Debug.Log("Bone Count : " + skeleton.Bones.Count);
        foreach(var bone in skeleton.Bones)
        {
            data.Add(skeleton.transform.InverseTransformPoint(bone.Transform.position));
        }

        g.fingerData = data;
        gestures.Add(g);
        Debug.Log("New Gesture Saved");
    }

    Gesture Recognise() // Check against gestures saved to find a match
    {
        fingerBones = new List<OVRBone>(skeleton.Bones);

        Gesture currentGesture = new Gesture();
        float currentMin = Mathf.Infinity;

        foreach(var gesture in gestures)
        {
            float sumDistance = 0;
            bool isDiscarded = false;
            for (int i = 0; i < fingerBones.Count; i++)
            {
                Vector3 currentData = skeleton.transform.InverseTransformPoint(fingerBones[i].Transform.position);
                float distance = Vector3.Distance(currentData, gesture.fingerData[i]);
                if(distance > threshhold)
                {
                    isDiscarded = true;
                    break;
                }
                sumDistance += distance;
            }
            if(!isDiscarded && sumDistance < currentMin)
            {
                currentMin = sumDistance;
                currentGesture = gesture;
            }
        }
        return currentGesture;
    }
}
