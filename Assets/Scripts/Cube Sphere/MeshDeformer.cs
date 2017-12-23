using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshDeformer : MonoBehaviour 
{
    #region Properties

    [Range(1, 50)]
    public float springForce = 20f;

    [Range(1, 50)]
    public float damping = 5f;

    private Mesh deformingMesh;

    private Vector3[] originalVertices;

    private Vector3[] displacedVertices;

    private Vector3[] vertexVelocities;

    private float uniformScale = 1f;

    #endregion

    #region Unity Callbacks

	private void Start () 
    {
        deformingMesh = GetComponent<MeshFilter>().mesh;

        originalVertices = deformingMesh.vertices;

        displacedVertices = new Vector3[originalVertices.Length];

        for (int i = 0; i < originalVertices.Length; i++)
        {
            displacedVertices[i] = originalVertices[i];
        }

        vertexVelocities = new Vector3[originalVertices.Length];
	}

    private void Update()
    {
        uniformScale = transform.localScale.x;

        for (int i = 0; i < displacedVertices.Length; i++) 
        {
            UpdateVertex(i);
        }

        deformingMesh.vertices = displacedVertices;
        deformingMesh.RecalculateNormals();
    }
	
    #endregion

    #region Methods

    public void AddDeformingForce(Vector3 point, float force)
    {
        Debug.DrawLine(Camera.main.transform.position, point);

        point = transform.InverseTransformPoint(point);

        for (int i = 0; i < displacedVertices.Length; i++)
        {
            AddForceToVertex(i, point, force);
        }
    }

    private void AddForceToVertex (int i, Vector3 point, float force) 
    {
        Vector3 pointToVertex = displacedVertices[i] - point;

        pointToVertex *= uniformScale;

        float attenuatedForce = force / (1f + pointToVertex.sqrMagnitude);

        float velocity = attenuatedForce * Time.deltaTime;

        vertexVelocities[i] += pointToVertex.normalized * velocity;
    }

    private void UpdateVertex (int i) 
    {
        Vector3 velocity = vertexVelocities[i];

        Vector3 displacement = displacedVertices[i] - originalVertices[i];

        displacement *= uniformScale;

        velocity -= displacement * springForce * Time.deltaTime;

        velocity *= 1f - damping * Time.deltaTime;

        vertexVelocities[i] = velocity;

        displacedVertices[i] += velocity * (Time.deltaTime / uniformScale);
    }

    #endregion
}
