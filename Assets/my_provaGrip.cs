using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

using UnityEngine.XR.Hands;
using UnityEngine.XR.Management;
//camera offset 1.1176

using Unity.Robotics.ROSTCPConnector.ROSGeometry;

public class my_provaGrip : MonoBehaviour
{
    private XRHand hand; // dichiarazione della variabile hand

    private float timeElapsed;
    
    
    public float printConsoleFrequency = 4.0f;
    private float timeElapsedConsole;

    //per trasformare
    public Transform origin;
    
    float roll,pitch,yaw;

    bool handClosed = false;
    void Start()  
    {
    XRHandSubsystem m_Subsystem = 
        XRGeneralSettings.Instance?
            .Manager?
            .activeLoader?
            .GetLoadedSubsystem<XRHandSubsystem>();

    if (m_Subsystem != null)
        m_Subsystem.updatedHands += OnHandUpdate;
    }

    public Pose ToWorldPose(XRHandJoint joint, Transform origin)
    {
      Pose xrOriginPose = new Pose(origin.position, origin.rotation);
      if(joint.TryGetPose(out Pose jointPose))
      {
            return jointPose.GetTransformedBy(xrOriginPose);
      }
      else
       return Pose.identity;

    }


    void OnHandUpdate(XRHandSubsystem subsystem,
                  XRHandSubsystem.UpdateSuccessFlags updateSuccessFlags,
                  XRHandSubsystem.UpdateType updateType) // ultima riga hands aggiunta dopo
    {

        //solo per debug stampa
        timeElapsedConsole += Time.deltaTime;
      
                var thumbJoint = subsystem.leftHand.GetJoint(XRHandJointIDUtility.FromIndex(5));
                var indexJoint = subsystem.leftHand.GetJoint(XRHandJointIDUtility.FromIndex(10));
                var midJoint = subsystem.leftHand.GetJoint(XRHandJointIDUtility.FromIndex(15));
                float threshold = 0.05f;
                
                //Con questo stampa tutto : Pos//rot//LinVEl//AngVEl ecc
                if(timeElapsedConsole > printConsoleFrequency) 
                {

                    if (thumbJoint.TryGetPose(out Pose thumbPose)) //per estrarre pose del pollice
                    {
                        Debug.Log(" -- POS POLLICE : x =  " + thumbPose.position.x + " y = " + thumbPose.position.y + " z = " + thumbPose.position.z);
                       
                    }

                    if (indexJoint.TryGetPose(out Pose indexPose)) //per estrarre pose dell'indice
                    {
                        Debug.Log(" ## POS INDICE : x =  " + indexPose.position.x + " y = " + indexPose.position.y + " z = " + indexPose.position.z);
                       
                    }

                    if (midJoint.TryGetPose(out Pose midPose)) //per estrarre pose del medio
                    {
                        Debug.Log(" ## POS INDICE : x =  " + midPose.position.x + " y = " + midPose.position.y + " z = " + midPose.position.z);
                       
                    }

                    float TI_distance = Mathf.Abs(Vector3.Distance(indexPose.position, thumbPose.position)); //prendo distanza in valore assoluto
                    float TM_distance = Mathf.Abs(Vector3.Distance(midPose.position, thumbPose.position));


                    Debug.Log(" DISTANZA POLLICE INDICE =   " + TI_distance );
                    Debug.Log(" DISTANZA POLLICE MEDIO =   " + TM_distance );

                    if(TI_distance < threshold && TM_distance < threshold  )
                    {
                        Debug.Log(" !!! MANO CHIUSA !!!   PollINDICE =  " + TI_distance + " PollMed =" + TM_distance);
                       
                    }
                    timeElapsedConsole = 0;
                }
                      
               
     
    switch (updateType)
      {
        case XRHandSubsystem.UpdateType.Dynamic:
            
            break;
        case XRHandSubsystem.UpdateType.BeforeRender: 
            // Update visual objects that use hand data
            //Debug.Log("BBBBBBBBBBB");
            break;
      }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

