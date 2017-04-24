using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class OctahedronSphereTester : MonoBehaviour 
{
    #region Properties

    [Range(0, 6)]
    public int subdivisions = 0;

    [Range(0.1f, 10f)]
    public float radius = 1f;

    public Material mat;

    #endregion

    #region Unity Callbacks

	private void Awake () 
    {
        setMesh();
	}

    private void OnValidate()
    {
        setMesh();
    }
	
    #endregion

    #region Methods

    private void setMesh()
    {
        GetComponent<MeshFilter>().mesh = OctahedronSphereCreator.Create(subdivisions, radius);

        GetComponent<MeshRenderer>().material = mat;
    }

    #endregion
}
