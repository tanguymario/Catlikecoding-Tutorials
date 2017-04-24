using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum FractalColorMode
{
    Default,
    Random
}

public class fractalManager : MonoBehaviour 
{
    #region Properties

    public GameObject goFractal;
    public GameObject toggleOption;

    public Mesh[] meshes;

    public ToggleGroup toggleGrpMesh;
    public Slider sliderDepth;
    public Slider sliderChildScale;
    public Slider sliderSpawnProbability;
    public Slider sliderRotationSpeed;
    public Toggle toggleRandomRotation;
    public Slider sliderMaxTwist;
    public ToggleGroup toggleGrpColor;

    private cameraRotate camRotate;

    private readonly int depthDefaultValue = 3;
    private readonly float childScaleDefaultValue = 0.5f;
    private readonly float spawnProbabilityDefaultValue = 1f;
    private readonly bool randomRotationDefaultValue = false;
    private readonly float rotationSpeedDefaultValue = 0f;
    private readonly int maxTwistDefaultValue = 0;

    #endregion

    #region Unity Callbacks

    private void Start()
    {
        camRotate = FindObjectOfType<cameraRotate>();

        if (toggleGrpMesh != null && sliderDepth != null && sliderChildScale != null && sliderSpawnProbability != null && sliderRotationSpeed != null && toggleRandomRotation != null && sliderMaxTwist != null && toggleGrpColor != null)
        {
            disableCamRotate();

            generateToggles();
        }
        else
        {
            if (meshes.Length > 0)
                generateFractal(new Mesh[]{ meshes[0] }, depthDefaultValue, childScaleDefaultValue, spawnProbabilityDefaultValue, randomRotationDefaultValue, rotationSpeedDefaultValue, maxTwistDefaultValue, FractalColorMode.Default);
        }
    }

    #endregion

    #region Methods

    public void getInfoAndGenerate()
    {
        int meshIndex = toggleGrpMesh.GetActive().transform.GetSiblingIndex();
        if (meshIndex < meshes.Length)
            meshes = new Mesh[]{ meshes[meshIndex] };

        int sliderDepthValue = (int)sliderDepth.value;
        float sliderChildScaleValue = sliderChildScale.value;
        float sliderSpawnProbabilityValue = sliderSpawnProbability.value;
        bool toggleRandomRotationValue = toggleRandomRotation.isOn;
        float sliderRotationSpeedValue = sliderRotationSpeed.value;
        int sliderMaxTwistValue = (int)sliderMaxTwist.value;

        FractalColorMode fracColorMode = (FractalColorMode)toggleGrpColor.GetActive().transform.GetSiblingIndex();

        generateFractal(meshes, sliderDepthValue, sliderChildScaleValue, sliderSpawnProbabilityValue, toggleRandomRotationValue, sliderRotationSpeedValue, sliderMaxTwistValue, fracColorMode); 
    }

    private void generateFractal(Mesh[] meshes, int depthValue, float childScale, float spawnProbability, bool randomRotation, float maxRotationSpeed, int maxTwist, FractalColorMode colorMode)
    {
        Transform t;
        if (goFractal != null)
        {
            t = Instantiate<GameObject>(goFractal).transform;

            Fractal f = t.GetComponent<Fractal>();
            f.meshes = meshes;
            f.maxDepth = depthValue;
            f.childScale = childScale;
            f.spawnProbability = spawnProbability;
            f.randomRotationSpeed = randomRotation;
            f.maxRotationSpeed = maxRotationSpeed;
            f.maxTwist = maxTwist;
            f.colorMode = colorMode;

            enableCamRotate(t);
        }

        Destroy(gameObject);
    }

    private void generateToggles()
    {
        if (toggleOption == null)
            return;

        Transform t;
        for (int i = 0; i < meshes.Length; i++)
        {
            string meshName = meshes[i].name;

            t = Instantiate<GameObject>(toggleOption).transform;
            t.name = meshName;
            t.SetParent(toggleGrpMesh.transform);

            Toggle toggleTemp = t.GetComponentInChildren<Toggle>();
            toggleTemp.group = toggleGrpMesh;
            if (i.Equals(0))
                toggleTemp.isOn = true;
            else
                toggleTemp.isOn = false;

            t.GetComponentInChildren<Text>().text = meshName;
        }

        t = Instantiate<GameObject>(toggleOption).transform;
        t.name = "Random";
        t.SetParent(toggleGrpMesh.transform);

        t.GetComponentInChildren<Toggle>().group = toggleGrpMesh;
        t.GetComponentInChildren<Text>().text = "Random";

        foreach (FractalColorMode fracColorMode in System.Enum.GetValues(typeof(FractalColorMode)))
        {
            t = Instantiate<GameObject>(toggleOption).transform;
            t.name = fracColorMode.ToString();
            t.SetParent(toggleGrpColor.transform);

            Toggle toggleTemp = t.GetComponentInChildren<Toggle>();
            toggleTemp.group = toggleGrpColor;
            if (toggleGrpColor.GetActive() == null)
                toggleTemp.isOn = true;
            else
                toggleTemp.isOn = false;
            
            t.GetComponentInChildren<Toggle>().group = toggleGrpColor;
            t.GetComponentInChildren<Text>().text = fracColorMode.ToString();
        }
    }

    private void enableCamRotate(Transform t)
    {
        if (camRotate != null && t != null)
        {
            camRotate.enabled = true;

            if (camRotate.slider != null)
                camRotate.slider.interactable = true;

            camRotate.affectTarget(t);
        }
    }

    private void disableCamRotate()
    {
        if (camRotate != null)
        {
            if (camRotate.slider != null)
                camRotate.slider.interactable = false;

            camRotate.enabled = false;
        }
    }

    #endregion
}