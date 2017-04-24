using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Fractal : MonoBehaviour 
{
    #region Properties

    public Mesh[] meshes;
    public Material material;

    public int maxDepth;

    public float childScale;

    public float spawnProbability;

    public bool randomRotationSpeed = false;
    public float maxRotationSpeed;

    private Vector3 rotationSpeed;

    public int maxTwist;

    public FractalColorMode colorMode;

    private int depth;

    private Material[] materials;

    private readonly Vector3[] childDirections =
        {
            Vector3.up,
            Vector3.down,
            Vector3.right,
            Vector3.left,
            Vector3.forward,
            Vector3.back
        };

    private readonly Dictionary<Vector3, Quaternion> childOrientations = new Dictionary<Vector3, Quaternion>()
    {
        {Vector3.up, Quaternion.identity},
        {Vector3.down, Quaternion.Euler(0f, -90f, 0f)},
        {Vector3.right, Quaternion.Euler(0f, 0f, -90f)},
        {Vector3.left, Quaternion.Euler(0f, 0f, 90f)},
        {Vector3.forward, Quaternion.Euler(90f, 0f, 0f)},
        {Vector3.back, Quaternion.Euler(-90f, 0f, 0f)}
    };

    #endregion

    #region Unity Callbacks

    private void Initialize(Fractal parent, int childIndex)
    {
        meshes = parent.meshes; 
        materials = parent.materials;

        maxDepth = parent.maxDepth;
        depth = parent.depth + 1;

        transform.SetParent(parent.transform);

        childScale = parent.childScale;
        transform.localScale = Vector3.one * childScale;
        transform.localPosition = childDirections[childIndex] * (0.5f + 0.5f * childScale);

        transform.localRotation = childOrientations[childDirections[childIndex]];

        spawnProbability = parent.spawnProbability;

        randomRotationSpeed = parent.randomRotationSpeed;
        maxRotationSpeed = parent.maxRotationSpeed;

        maxTwist = parent.maxTwist;
    }

    private void Start () 
    {
        if (materials == null)
            InitializeMaterials();

        rotationSpeed = new Vector3();
        if (randomRotationSpeed)
        {
            if (Random.value < 0.33)
                rotationSpeed.x = Random.Range(-maxRotationSpeed, maxRotationSpeed);

            if (Random.value < 0.33)
                rotationSpeed.y = Random.Range(-maxRotationSpeed, maxRotationSpeed);

            if (Random.value < 0.33)
                rotationSpeed.z = Random.Range(-maxRotationSpeed, maxRotationSpeed);
        }
        else
        {
            if (Random.value < 0.33)
                rotationSpeed.x = maxRotationSpeed;

            if (Random.value < 0.33)
                rotationSpeed.y = maxRotationSpeed;

            if (Random.value < 0.33)
                rotationSpeed.z = maxRotationSpeed;
        }

        transform.Rotate(Random.Range(-maxTwist, maxTwist), 0f, 0f);

        gameObject.AddComponent<MeshFilter>().mesh = meshes[Random.Range(0, meshes.Length - 1)];
        gameObject.AddComponent<MeshRenderer>().material = materials[depth];

        if (depth < maxDepth)
            StartCoroutine(createChildren());
    }

    private void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }

    #endregion

    #region Coroutines

    private void InitializeMaterials()
    {
        materials = new Material[maxDepth + 1];
        for (int i = 0; i <= maxDepth; i++) 
        {
            float t = i / (maxDepth - 1f);
            t *= t;

            materials[i] = new Material(material);

            switch (colorMode)
            {
                case FractalColorMode.Default:
                    materials[i].color = Color.Lerp(Color.red, new Color(1f, 0.5f, 0.016f, 1.0f), t);
                    break;
                case FractalColorMode.Random:
                    materials[i].color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
                    break;
                default:
                    materials[i].color = Color.Lerp(Color.red, new Color(1f, 0.5f, 0.016f, 1.0f), t);
                    break;
            }
        }

        if (colorMode.Equals(FractalColorMode.Default))
            materials[maxDepth].color = Color.yellow;
    }

    private IEnumerator createChildren()
    {
        for (int i = 0; i < childDirections.Length; i++) 
        {
            if (Random.value < spawnProbability)
            {
                yield return new WaitForSeconds(Random.Range((float)depth / maxDepth, 2.0f * (float)depth / maxDepth));

                new GameObject("Fractal Child").AddComponent<Fractal>().

                Initialize(this, i);
            }
        }

        yield return null;
    }

    #endregion
}
