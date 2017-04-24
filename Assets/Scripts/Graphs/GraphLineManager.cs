using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(ParticleSystem))]
public class GraphLineManager : MonoBehaviour 
{
    #region Properties

    public ParticleSystemSortMode particleSysRendSortMode;

    public GraphLine[] graphs;

    private List<ParticleSystem.Particle>[] pointsTab;

    private ParticleSystem.Particle[] particles;

    private ParticleSystem particleSys;

    private ParticleSystemRenderer particleSysRend;

    #endregion

    #region Unity Callbacks

    private void Update()
    {
        foreach (GraphLine g in graphs)
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

    private List<ParticleSystem.Particle> CreatePoints(GraphLine g)
    {
        List<ParticleSystem.Particle> points = new List<ParticleSystem.Particle>();

        if (g.isOn)
        {

            float increment = 1f / (g.resolution - 1);

            for (int x = 0; x < g.resolution; x++)
            {
                ParticleSystem.Particle particle = new ParticleSystem.Particle();

                Vector3 step = new Vector3(x * increment, 0f, 0f);

                float ZValue = g.function.GetFuncValue(step, g.isAnimated, false, false);

                particle.position = new Vector3(step.x, 0f, ZValue); 

                if (!g.isAnimated)
                    particle.startColor = Color.Lerp(g.startColor, g.endColor, step.x);                    
                else 
                    particle.startColor = Color.Lerp(g.startColor, g.endColor, Mathf.Sin(Mathf.PI * 2f * step.x + Time.timeSinceLevelLoad));
                
                particle.startSize = Mathf.Lerp(g.startSize, g.endSize, step.x); 

                points.Add(particle);
            }
        }

        return points;
    }

    #endregion
}
