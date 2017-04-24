using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(ParticleSystem))]
public class GraphGridManager : MonoBehaviour 
{
    #region Properties

    public ParticleSystemSortMode particleSysRendSortMode;

    public GraphGrid[] graphs;

    private List<ParticleSystem.Particle>[] pointsTab;

    private ParticleSystem.Particle[] particles;

    private ParticleSystem particleSys;

    private ParticleSystemRenderer particleSysRend;

    #endregion

    #region Unity Callbacks

    private void Update()
    {
        foreach (GraphGrid g in graphs)
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

    private List<ParticleSystem.Particle> CreatePoints(GraphGrid g)
    {
        List<ParticleSystem.Particle> points = new List<ParticleSystem.Particle>();

        if (g.isOn)
        {
            float increment = 1f / (g.resolution - 1);

            for (int x = 0; x < g.resolution; x++)
            {
                for (int y = 0; y < g.resolution; y++)
                {
                    ParticleSystem.Particle particle = new ParticleSystem.Particle();

                    Vector3 step = new Vector3(x * increment, y * increment, 0f);

                    float ZValue = g.function.GetFuncValue(step, g.isAnimated, g.useYAxisInFormulas, false);

                    particle.position = new Vector3(step.x, step.y, ZValue);

                    if (g.isAnimated)
                    {
                        if (g.useFullColors)
                        {
                            particle.startColor = new Color(step.x, particle.position.z, step.y);
                        }
                        else
                        {
                            particle.startColor = Color.Lerp(g.startColorX, g.endColorX, Mathf.Sin(Mathf.PI * 2f * ((step.x - step.y) / 2f) + Time.timeSinceLevelLoad));
                        }
                    }
                    else
                    {
                        if (g.useFullColors)
                        {
                            particle.startColor = new Color(step.x, particle.position.z, step.y);
                        }
                        else
                        {
                            Color a = Color.Lerp(g.startColorX, g.endColorX, step.x);
                            Color b = Color.Lerp(g.startColorX, g.startColorY, step.y);
                            Color c = Color.Lerp(g.endColorY, g.startColorY, step.x);
                            Color d = Color.Lerp(g.endColorY, g.endColorX, step.y);

                            Color e = Color.Lerp(a, b, ((step.x + step.y) / 2f) / 2f);
                            Color f = Color.Lerp(c, d, ((step.x + step.y) / 2f) / 2f);

                            particle.startColor = Color.Lerp(e, f, (step.x + step.y) / 2f);
                        }
                    }

                    particle.startSize = Mathf.Lerp(g.startSizeX, g.endSizeX, (step.x + step.y) / 2f); 

                    points.Add(particle);
                }
            }
        }

        return points;
    }

    #endregion
}
