using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;
//Schin
public class RealtimeCameraAdjustments : MonoBehaviour
{

    private PostProcessingBehaviour ppBehaviour;
    private float distance;
    private Plane central_plane;
    private void OnEnable()
    {
        //get ppBehaviour of current camera of scene.
        // this assumes there's only one camera using PP in current scene.
        ppBehaviour = GetComponent<PostProcessingBehaviour>();
        central_plane = new Plane(new Vector3(0,1,0),new Vector3(0,0,0));
    }

    void FixedUpdate()
    {
        ChangeDepthOfFieldAtRuntime();

    }
    void ChangeDepthOfFieldAtRuntime()
    {
        //get camera position and angle between its pointing rotation and the (minus) law of plane
        Vector3 cameraPos = transform.position;
        Quaternion minusLawOfPlane = Quaternion.LookRotation(Vector3.down, Vector3.up);
        float cameraAngle = Quaternion.Angle(transform.rotation, minusLawOfPlane);

        //calculate camera distance to 0-level plain, and calculate its hypotenuse length
        float dis = Mathf.Abs(central_plane.GetDistanceToPoint(cameraPos)/ Mathf.Sin(cameraAngle));

        //copy current "depth of field" settings from the profile into a temporary variable
        DepthOfFieldModel.Settings deaptoffieldSettings = ppBehaviour.profile.depthOfField.settings;
        deaptoffieldSettings.focusDistance = dis;
        //set the "depth of field" settings in the actual profile to the temp settings with the changed value
        ppBehaviour.profile.depthOfField.settings = deaptoffieldSettings;
    }

}
