using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour {

    public Transform parentTransform;   //the transform of the player or object you want the camera to focus on
    public Transform originalTransform;
    public float rotSpeed;              //how fast the camera rotates around the focussed object
    public float magAdjustment;       //controls how close a collider has to be to the back of the camera to justify turning of backwards zoom
    public int tolerance;               
    public float zoomSpeed;
    public float yAngleTol;             //the angle in degrees that the focus object to camera vector can be from the flat plane running throw the focus object's origin
    public int minZoomDist;
    public int maxZoomDist;

    private Vector3 targToCamNoY;               //vector from the camera to the object of focus with y=0
    private float yAngle;                       
    private float yCompOrg;
    private bool tooHigh;
    private bool tooLow;
    private Vector3 playerToCamera;
    private bool allowZoomOut;
    private float mouseOrgX;                   //screen x coordinate when middle mouse button is first pressed
    private float mouseOrgY;                   //screen y coordinate when middle mouse button is first pressed
    

    // Initialization
    void Start() {
        mouseOrgX = 0;
        mouseOrgY = 0;
        tooHigh = false;
        tooLow = false;
    }

    // Called once per frame
    void Update() {

        // if pressed middle mouse button
        if (Input.GetMouseButtonDown(2)) {
            mouseOrgX = Input.mousePosition.x;
            mouseOrgY = Input.mousePosition.y;
        }

        // if hold pressing middle mouse button
        if (Input.GetMouseButton(2)) {

            //left/right input/movement
            if (Input.mousePosition.x >= (mouseOrgX + tolerance)) {
                //move right
                transform.Translate(Vector3.right * Time.deltaTime * rotSpeed, Space.Self);
            } else if (Input.mousePosition.x <= (mouseOrgX - tolerance)) {
                //move left
                transform.Translate(Vector3.right * Time.deltaTime * -rotSpeed, Space.Self);
            }

            //check to see if too high or low
            targToCamNoY = parentTransform.position - transform.position;
            yCompOrg = targToCamNoY.y;
            targToCamNoY.y = 0;

            yAngle = Vector3.Angle(targToCamNoY, parentTransform.position - transform.position);
            if (yAngle > yAngleTol && yCompOrg < 0) {
                //too far up
                tooHigh = true;
            } else if (yAngle > yAngleTol && yCompOrg > 0) {
                //too far down
                tooLow = true;
            }

            //only listen to curtain input based on prior analysis
            if (tooHigh) {
                //if too high only listen to downward input
                if ((Input.mousePosition.y <= (mouseOrgY - tolerance))) {
                    // down
                    transform.Translate(Vector3.up * Time.deltaTime * -rotSpeed, Space.Self);
                }
            } else if (tooLow) {
                // only listen to upward input
                if ((Input.mousePosition.y >= (mouseOrgY + tolerance))) {
                    // up
                    transform.Translate(Vector3.up * Time.deltaTime * rotSpeed, Space.Self);
                }
            } else {
                //camera is not too high or too low so listen to both up and down input
                if ((Input.mousePosition.y >= (mouseOrgY + tolerance))) {
                    //up
                    transform.Translate(Vector3.up * Time.deltaTime * rotSpeed, Space.Self);
                } else if ((Input.mousePosition.y <= (mouseOrgY - tolerance))) {
                    //down
                    transform.Translate(Vector3.up * Time.deltaTime * -rotSpeed, Space.Self);
                }
            }

            //check to see if camera is no longer too high or too low
            targToCamNoY = parentTransform.position - transform.position;
            yCompOrg = targToCamNoY.y;
            targToCamNoY.y = 0;
            yAngle = Vector3.Angle(targToCamNoY, parentTransform.position - transform.position);
            if (yAngle <= yAngleTol - 4) {
                //4 is arbitrary
                tooHigh = false;
                tooLow = false;
            }
        }

        playerToCamera = transform.position - parentTransform.position;
        if (Input.mouseScrollDelta.y > 0 && playerToCamera.magnitude >= minZoomDist) {
            //zoom in
            transform.Translate(Vector3.forward * Time.deltaTime * zoomSpeed, Space.Self);
        } else if (Input.mouseScrollDelta.y < 0 && allowZoomOut && playerToCamera.magnitude <= maxZoomDist) {
            //zoom out
            transform.Translate(Vector3.forward * Time.deltaTime * -zoomSpeed, Space.Self);
        }
        transform.rotation = Quaternion.LookRotation(parentTransform.position - transform.position, Vector3.up);

    }

    void FixedUpdate() {
        RaycastHit hitInfo;
        playerToCamera = transform.position - parentTransform.position;
        Vector3 desiredPosition = originalTransform.position; // Make sure to have a variable to store the camera's original position

        // Check for any obstructions between the parentTransform and the camera
        if (Physics.Raycast(parentTransform.position, playerToCamera.normalized, out hitInfo, playerToCamera.magnitude)) {
            // If there is an obstruction, immediately move the camera in front of the obstruction
            transform.position = hitInfo.point;
            allowZoomOut = false; // Do not allow zooming out when obstructed
        } else {
            // If there is no obstruction, allow zooming out
            allowZoomOut = true;
            // Gradually move the camera back to the original position
            transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.fixedDeltaTime * zoomSpeed);
        }
    }
}