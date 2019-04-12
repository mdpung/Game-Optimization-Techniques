using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;

public class GenerateBlocks : MonoBehaviour
{
    public Terrain terrain;
    public Camera camera;
    public GameObject medLion;
    public GameObject highLion;
    float width;
    float height;
    List<GameObject> gameObjects;
    int lodSize = 3;
    int numberOfObjects;
    UnityEvent toggleLOD = new UnityEvent();


    // Start is called before the first frame update
    void Start()
    {
        width = terrain.terrainData.size.x;
        height = terrain.terrainData.size.z;
        numberOfObjects = (int)(width * height);
        gameObjects = new List<GameObject>(numberOfObjects);
        LODGroup lodGroup;
        camera.transform.position = new Vector3(width / 2, 5f, 0f);

        for (int i = 0; i < width; i += 2)
        {
            for (int j = 0; j < height; j+= 2)
            {
                GameObject gameObject = new GameObject();
                gameObject.GetComponent<Transform>();
                gameObject.transform.position = new Vector3(i, 0.5f, j);
                lodGroup = gameObject.AddComponent<LODGroup>();
                lodGroup.SetLODs(makeLODs(gameObject));
                gameObjects.Add(gameObject);
            }
        }


        toggleLOD.AddListener(ToggleLOD);
    }

    private void ToggleLOD()
    {
        foreach (GameObject obj in gameObjects)
        {
            LODGroup group = obj.GetComponent<LODGroup>();

            GameObject childMedLion = obj.transform.Find("LionMedC").gameObject;
            childMedLion.SetActive(!childMedLion.activeSelf);

            GameObject childCube = obj.transform.Find("Cube").gameObject;
            childCube.SetActive(!childCube.activeSelf);

            group.enabled = !group.enabled;

        }
    }


    private LOD[] makeLODs(GameObject parent)
    {
        LOD[] lods = new LOD[lodSize];
        for (int i = 0; i < lodSize; i++)
        {
            GameObject go;
            Renderer[] renderers = new Renderer[1];
            switch (i)
            {
                case 0:
                    // (highLion, parent.GetComponent<Transform>()
                    go =  PrefabUtility.InstantiatePrefab(highLion) as GameObject;
                    renderers[0] = go.GetComponentInChildren<Renderer>();
                    go.AddComponent<MeshRenderer>();
                    renderers[0].materials[0].color = Color.blue;
                    renderers[0].materials[1].color = Color.blue;
                    break;

                case 1:
                    go = PrefabUtility.InstantiatePrefab(medLion) as GameObject;
                    // go.AddComponent<MeshRenderer>();
                    renderers[0] = go.GetComponent<Renderer>();
                    renderers[0].material.color = Color.red;
                    break;

                default:
                    go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    renderers[0] = go.GetComponent<Renderer>();
                    renderers[0].material.color = Color.green;
                    break;
            }
            go.transform.parent = parent.transform;
            go.GetComponent<Transform>().transform.position = parent.GetComponent<Transform>().transform.position;
        
            lods[i] = new LOD(0.8f - (0.25f * i) - (0.05f * Mathf.Pow(i, 2)), renderers);
        }

        return lods;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.L) && toggleLOD != null)
        {
            //Begin the action
            toggleLOD.Invoke();
        }
    }
}
