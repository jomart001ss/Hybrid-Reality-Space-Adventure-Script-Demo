using UnityEngine;
using System.Collections;

public class ScreenFaderCamera : MonoBehaviour
{
    public bool checkScreenFader = false;

	void OnPostRender()
	{
        if (checkScreenFader && ScreenFader.instance.isFading)
		{
			ScreenFader.instance.fadeMaterial.SetPass(0);
			GL.PushMatrix();
			GL.LoadOrtho();
			GL.Color(ScreenFader.instance.fadeMaterial.color);
			GL.Begin(GL.QUADS);
			GL.Vertex3(0f, 0f, -12f);
			GL.Vertex3(0f, 1f, -12f);
			GL.Vertex3(1f, 1f, -12f);
			GL.Vertex3(1f, 0f, -12f);
			GL.End();
			GL.PopMatrix();
		}
	}
}
