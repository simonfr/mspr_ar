using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARFoundationColorMapping : MonoBehaviour
{
    public ARTrackedImageManager imageManager;

    public GameObject arPrefabs;

    public int realWidth;
    public int realHeight;

    private GameObject arContents;
    private GameObject drawObj;

    private GameObject cube;
    
    void Start()
    {
        imageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        ARTrackedImage trackedImage = null;

        for (int i = 0; i < eventArgs.added.Count; i++)
        {
            trackedImage = eventArgs.added[i];

            string imgName = trackedImage.referenceImage.name;

            if (imgName == "serp")
            {
                arContents = Instantiate(arPrefabs, trackedImage.transform);
                cube = CreateCubeForARFoundationTarget(arContents.gameObject, trackedImage.size.x, trackedImage.size.y);
            }
        }

        for (int i = 0; i < eventArgs.updated.Count; i++)
        {
            trackedImage = eventArgs.updated[i];
            
            if (trackedImage.trackingState == TrackingState.Tracking)
            {
                arContents.SetActive(true);
                Debug.Log("Image trackeeeeed");
            }
            else
            {
                arContents.SetActive(true);
                Debug.Log("Image not trackeeeeed");
            }
        }

        for (int i = 0; i < eventArgs.removed.Count; i++)
        {
            arContents.SetActive(false);
        }
    }

    public void Play()
    {
        float[] srcValue = AirarManager.Instance.CalculateMarkerImageVertex(cube);

        Texture2D screenShotTex = ScreenShot.GetScreenShot(arContents);

        AirarManager.Instance.ProcessColoredMapTexture(screenShotTex, srcValue, realWidth, realHeight, (resultTex) =>
        {
            drawObj = GameObject.FindGameObjectWithTag("coloring");
            drawObj.GetComponent<Renderer>().material.mainTexture = resultTex;
        });
    }

    /// <summary>
    /// Create a full size cube on the ARFoundation marker image
    /// </summary>
    /// <param name="targetWidth">marker image width</param>
    /// <param name="targetHeight">marker image height</param>
    public GameObject CreateCubeForARFoundationTarget(GameObject parentObj, float targetWidth, float targetHeight)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.GetComponent<Renderer>().material = AirarManager.Instance.transparentMat;
        cube.transform.SetParent(parentObj.transform);
        cube.transform.localPosition = Vector3.zero;
        cube.transform.localRotation = Quaternion.Euler(Vector3.zero);
        cube.transform.localScale = new Vector3(targetWidth, 0.001f, targetHeight);

        return cube;
    }
}
