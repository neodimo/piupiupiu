using UnityEngine;
using System.Collections;

public class VectorGridForce2 : MonoBehaviour 
{
	public VectorGrid m_VectorGrid;
	public float m_ForceScale;
	public bool m_Directional;
	public Vector3 m_ForceDirection;
	public float m_Radius;
	public Color m_Color = Color.white;
	public bool m_HasColor;
    float m_Red = 0.0f;
    float m_Green = 0.0f;
    float m_Blue = 255.0f;
    float m_ColorInterp;
    Color m_StartColor = Color.red;
    Color m_TargetColor = Color.blue;

    // Update is called once per frame
    void Update () 
	{
        Color color = new Color(m_Red / 255.0f, m_Green / 255.0f, m_Blue / 255.0f, 1.0f);

        UpdateRandomColor();

        if (m_VectorGrid)
		{
			if(m_Directional)
			{
				m_VectorGrid.AddGridForce(this.transform.position, m_ForceDirection * m_ForceScale, m_Radius, color, true);
			}
			else
			{
				m_VectorGrid.AddGridForce(this.transform.position, m_ForceScale, m_Radius, color, true);
			}
		}
	}

    void UpdateRandomColor()
    {
        m_ColorInterp += Time.deltaTime;

        if (m_ColorInterp > 1.0f)
        {
            m_ColorInterp -= 1.0f;
            m_StartColor = m_TargetColor;
            m_TargetColor = new Color(UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f));
        }

        Color interpolatedColor = m_StartColor + ((m_TargetColor - m_StartColor) * m_ColorInterp);
        m_Red = interpolatedColor.r * 255.0f;
        m_Green = interpolatedColor.g * 255.0f;
        m_Blue = interpolatedColor.b * 255.0f;
    }
}
