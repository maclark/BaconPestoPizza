using UnityEngine;
using System.Collections;

public class GLTriangler : MonoBehaviour {
	public Material mat;
	void Update() {
		if (!mat) {
			Debug.LogError("Please Assign a material on the inspector");
			return;
		}
		GL.PushMatrix();
		mat.SetPass(0);
		GL.LoadOrtho();
		GL.Begin(GL.TRIANGLES);
		GL.Color(Color.red);
		GL.Vertex3(0, 0, 0);
		GL.Vertex3(1, 1, 0);
		GL.Vertex3(0, 1, 0);
		GL.End();
		GL.PopMatrix();
	}
}