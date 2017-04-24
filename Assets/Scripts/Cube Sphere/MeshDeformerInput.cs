using UnityEngine;

[RequireComponent(typeof(MeshDeformer))]
public class MeshDeformerInput : MonoBehaviour 
{
    #region Properties

    [Range(0, 50)]
    public float force = 10f;

    [Range(0, 2.5f)]
    public float forceOffset = 0.1f;

    #endregion

    #region Unity Callbacks

	private void Update () 
    {
        if (Input.GetMouseButton(0))
            HandleInput();
	}
	
    #endregion

    #region Methods

    private void HandleInput()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(inputRay, out hit))
        {
            MeshDeformer deformer = hit.collider.GetComponent<MeshDeformer>();

            if (deformer)
            {
                Vector3 point = hit.point;
                point += hit.normal * forceOffset;
                    
                deformer.AddDeformingForce(point, force);
            }
        }
    }

    #endregion
}
