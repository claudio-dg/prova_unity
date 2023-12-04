using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// x mani
using UnityEngine.XR;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Management;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
//camera offset 1.1176

// per pubblicare ros
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.UnityRoboticsDemo;



public class pub_LeftHandGrip : MonoBehaviour
{
    private XRHand hand; // dichiarazione della variabile hand

    //per connessione ROS
    ROSConnection ros;
    public string topicName = "left_hand_grip_topic";
    public float publishMessageFrequency = 0.5f;

    private float timeElapsed;
    
    bool handClosed = false;
    bool previousCondition = false;

    void Start()  
    {
    XRHandSubsystem m_Subsystem = 
        XRGeneralSettings.Instance?
            .Manager?
            .activeLoader?
            .GetLoadedSubsystem<XRHandSubsystem>();

    if (m_Subsystem != null)
        m_Subsystem.updatedHands += OnHandUpdate;
         // DECOMMENTA PER INIZIALIZZARE CONNECTION ROS
           ///*
        // start the ROS connection
        ros = ROSConnection.GetOrCreateInstance();
        // inizializza publisher su topic right_arm_frame_topic che accetta msg di tipo  ...
        ros.RegisterPublisher<MyHandClosedMsg>(topicName);
          //*/
    }


    void OnHandUpdate(XRHandSubsystem subsystem,
                  XRHandSubsystem.UpdateSuccessFlags updateSuccessFlags,
                  XRHandSubsystem.UpdateType updateType) // ultima riga hands aggiunta dopo
    {

        //solo per debug stampa
        timeElapsed += Time.deltaTime;
      
                var thumbJoint = subsystem.leftHand.GetJoint(XRHandJointIDUtility.FromIndex(5));
                var indexJoint = subsystem.leftHand.GetJoint(XRHandJointIDUtility.FromIndex(10));
                var midJoint = subsystem.leftHand.GetJoint(XRHandJointIDUtility.FromIndex(15));

                var palmJoint = subsystem.leftHand.GetJoint(XRHandJointIDUtility.FromIndex(1));
                float threshold = 0.05f;
                float sec_threshold = 0.065f;
                
                //Con questo stampa tutto : Pos//rot//LinVEl//AngVEl ecc
                if(timeElapsed > publishMessageFrequency) 
                {

                    if (thumbJoint.TryGetPose(out Pose thumbPose)) //per estrarre pose del pollice
                    {
                        //Debug.Log(" -- POS POLLICE : x =  " + thumbPose.position.x + " y = " + thumbPose.position.y + " z = " + thumbPose.position.z);
                       
                    }

                    if (indexJoint.TryGetPose(out Pose indexPose)) //per estrarre pose dell'indice
                    {
                        //Debug.Log(" ## POS INDICE : x =  " + indexPose.position.x + " y = " + indexPose.position.y + " z = " + indexPose.position.z);
                       
                    }

                    if (midJoint.TryGetPose(out Pose midPose)) //per estrarre pose del medio
                    {
                        //Debug.Log(" ## POS INDICE : x =  " + midPose.position.x + " y = " + midPose.position.y + " z = " + midPose.position.z);
                       
                    }
                    if (palmJoint.TryGetPose(out Pose PalmPose)) //per estrarre pose del palmo
                    {
                        //Debug.Log(" ## POS palmo : x =  " + PalmPose.position.x + " y = " + PalmPose.position.y + " z = " + PalmPose.position.z);
                       
                    }

                    // first check conditions (distanza fra pollice&indice e fra Pollice&Medio)
                    float TI_distance = Mathf.Abs(Vector3.Distance(indexPose.position, thumbPose.position)); //prendo distanza in valore assoluto
                    float TM_distance = Mathf.Abs(Vector3.Distance(midPose.position, thumbPose.position));

                    // second check conditions (distanza fra Palmo&indice e fra Palmo&medio)
                    float PI_distance = Mathf.Abs(Vector3.Distance(indexPose.position, PalmPose.position)); //prendo distanza in valore assoluto
                    float PM_distance = Mathf.Abs(Vector3.Distance(midPose.position, PalmPose.position));

                    //Debug.Log(" DISTANZA POLLICE INDICE =   " + TI_distance );
                    //Debug.Log(" DISTANZA POLLICE MEDIO =   " + TM_distance );

                    //Debug.Log(" DISTANZA Palmo INDICE =   " + PI_distance );
                    //Debug.Log(" DISTANZA Palmo MEDIO =   " + PM_distance );

                    if(TI_distance < threshold && TM_distance < threshold  )
                    {
                        Debug.Log("  MANO SX CHIUSA prima condizione...  PollINDICE =  " + TI_distance + " PollMed =" + TM_distance);
                        handClosed = true;
                    }
                    else if(PI_distance < sec_threshold && PM_distance < sec_threshold ) //aggiungo seconda condizione
                    {
                        
                        Debug.Log("  MANO SX CHIUSA  PER CONDIZIONE NUOVA!!!   PalmoINDICE =  " + PI_distance + " palmoMed =" + PM_distance);
                        handClosed = true;

                    }
                    else //mano e' aperta
                    {
                        //Debug.Log(" mano aperta.. =  " + TI_distance + " PollMed =" + TM_distance);
                        handClosed = false;

                    }

                    if(handClosed != previousCondition) //pubblica solo se condizione mano e' cambiata da open/close o close/open
                    {

                     MyHandClosedMsg left_hand_closed = new MyHandClosedMsg(handClosed); 
                     if(handClosed){Debug.Log(" condizione cambiata => ora e' CHIUSA ");}
                     else{Debug.Log(" condizione cambiata => ora e' APERTA ");}
                     
                     // Finally send the message to server_endpoint.py running in ROS
                     ros.Publish(topicName, left_hand_closed);
                     //update prev condition with current one
                     previousCondition = handClosed;
                    }
                    else
                    {
                        Debug.Log(" condizione mano immutata = " );
                        
                    }
                    timeElapsed = 0;
                }
                      
               
     
    switch (updateType)
      {
        case XRHandSubsystem.UpdateType.Dynamic:
            
            break;
        case XRHandSubsystem.UpdateType.BeforeRender: 
            
            break;
      }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

