using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(ParticleSystem))]
public class Graph3DManager : MonoBehaviour 
{
    #region Properties

    public ParticleSystemSortMode particleSysRendSortMode;

    public bool absolute;

    [Range(0.01f, 1.0f)]
    public float threshold = 0.5f;

    public Graph3D[] graphs;

    private List<ParticleSystem.Particle>[] pointsTab;

    private ParticleSystem.Particle[] particles;

    private ParticleSystem particleSys;

    private ParticleSystemRenderer particleSysRend;

    #endregion

    #region Unity Callbacks

    private void Update()
    {
        foreach (Graph3D g in graphs)
        {
            if (g.isOn && g.isAnimated)
            {
                UpdateGraphs();
                break;
            }
        }
    }

    private void OnValidate()
    {
        if (Application.isPlaying)
        {
            if (particleSys == null)
                particleSys = GetComponent<ParticleSystem>();

            if (particleSysRend == null)
                particleSysRend = (ParticleSystemRenderer) particleSys.GetComponent<Renderer>();

            if (!particleSysRend.sortMode.Equals(particleSysRendSortMode))
                particleSysRend.sortMode = particleSysRendSortMode;

            UpdateGraphs();
        }
    }

    #endregion

    #region Methods

    private void UpdateGraphs()
    {
        pointsTab = new List<ParticleSystem.Particle>[graphs.Length];

        int nbTotalPoints = 0;

        for (int i = 0; i < pointsTab.Length; i++)
        {
            pointsTab[i] = CreatePoints(graphs[i]);

            nbTotalPoints += pointsTab[i].Count;
        }

        UpdateParticles(nbTotalPoints);
    }

    private void UpdateParticles(int size)
    {
        particles = new ParticleSystem.Particle[size];

        int cmpt = 0;

        for (int i = 0; i < pointsTab.Length; i++)
        {
            for (int j = 0; j < pointsTab[i].Count; j++)
            {
                particles[cmpt] = pointsTab[i][j];
                cmpt++;
            }
        }

        particleSys.SetParticles(particles, particles.Length);
    }

    private List<ParticleSystem.Particle> CreatePoints(Graph3D g)
    {
        List<ParticleSystem.Particle> points = new List<ParticleSystem.Particle>();

        if (g.isOn)
        {
            float increment = 1f / (g.resolution - 1);

            for (int x = 0; x < g.resolution; x++)
            {
                for (int y = 0; y < g.resolution; y++)
                {
                    for (int z = 0; z < g.resolution; z++)
                    {
                        ParticleSystem.Particle particle = new ParticleSystem.Particle();

                        Vector3 step = new Vector3(x, y, z) * increment;

                        particle.position = new Vector3(step.x, step.y, -step.z);

                        float funcValue = g.function.GetFuncValue(particle.position, g.isAnimated, false, true);

                        if (absolute)
                            funcValue = funcValue >= threshold ? 1f : 0f;
                        
                        particle.startColor = new Color(step.x, step.y, -step.z, funcValue);

                        particle.startSize = Mathf.Lerp(g.startSizeX, g.endSizeX, (step.x + step.y) / 2f); 

                        points.Add(particle);
                    }
                }
            }
        }

        return points;
    }

    #endregion
}
