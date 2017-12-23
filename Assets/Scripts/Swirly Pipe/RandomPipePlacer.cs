using UnityEngine;

public class RandomPipePlacer : PipeItemGenerator 
{
    #region Properties

    public PipeItem[] itemPrefabs;

    #endregion

    #region Methods

    public override void GenerateItems(Pipe p)
    {
        float angleStep = p.CurveAngle / p.CurveSegmentCount;

        for (int i = 0; i < p.CurveSegmentCount; i++)
        {
            PipeItem item = Instantiate<PipeItem>(itemPrefabs[Random.Range(0, itemPrefabs.Length)]);

            float pipeRotation = (Random.Range(0, p.pipeSegmentCount) + 0.5f) * 360f / p.pipeSegmentCount;

            item.Position(p, i * angleStep, pipeRotation);
        }
    }

    #endregion
}
