using UnityEngine;

public class SpiralPipePlacer : PipeItemGenerator 
{
    #region Properties

    public PipeItem[] itemPrefabs;

    #endregion

    #region Methods

    public override void GenerateItems(Pipe p)
    {
        float start = (Random.Range(0, p.pipeSegmentCount) + 0.5f);
        float direction = Random.value < 0.5f ? 1f : -1f;

        float angleStep = p.CurveAngle / p.CurveSegmentCount;

        for (int i = 0; i < p.CurveSegmentCount; i++)
        {
            PipeItem item = Instantiate<PipeItem>(itemPrefabs[Random.Range(0, itemPrefabs.Length)]);

            float pipeRotation = (start + i * direction) * 360f / p.pipeSegmentCount;

            item.Position(p, i * angleStep, pipeRotation);
        }
    }

    #endregion
}
