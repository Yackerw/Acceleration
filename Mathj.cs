using System;
using Microsoft.Xna.Framework;

namespace Mathj
{

	static class Matht
	{
		public const float Deg2Rad = 0.017453292519943f;
		public const float Rad2Deg = 57.29577951308232f;
		/// <summary>
		/// Returns shortest rotation between two degrees
		/// </summary>
		/// <param name="dir1"></param>
		/// <param name="dir2"></param>
		/// <returns></returns>
		public static float AngleBetween(float dir1, float dir2)
		{

			float a = dir2 - dir1;

			a += 180.0f;

			if (a < 0)
			{
				a += 360.0f;
			}

			if (a > 360)
			{
				a -= 360.0f;
			}

			a -= 180.0f;

			return a;
		}

		public static float Lerp(float x, float y, float t)
		{
			return (x * (1f - t)) + (y * t);
		}

		static public float SmootherStep(float edge0, float edge1, float x)
		{
			// Evaluate polynomial
			return Lerp(edge0, edge1, x * x * x * (x * (x * 6f - 15f) + 10f));
		}

		public static float DotProduct(Vector2 a, Vector2 b)
		{
			return (a.X * b.X) + (a.Y * b.Y);
		}

		public static float Magnitude(Vector2 v)
		{
			return (float)Math.Sqrt((v.X * v.X) + (v.Y * v.Y));
		}

		public static bool HasSeparateAxis(Vector2[] points1, Vector2[]points2)
		{
			for (int i = 0; i < points1.Length; ++i)
			{
				// ?? idk how this works i've just accepted it does
				Vector2 normal;
				normal.X = points1[(i + 1) % points1.Length].Y - points1[i].Y;
				normal.Y = points1[i].X - points1[(i + 1) % points1.Length].Y;
				normal.Normalize();
				for (int j = 0; j < points2.Length; ++j)
				{
					Vector2 target = points2[j] - points1[i];
					target.Normalize();
					if (DotProduct(normal, target) <= 0)
					{
						break;
					}
					if (j == points2.Length - 1)
					{
						return true;
					}
				}
			}
			return false;
		}

		public static bool RectangleOnRectangleOverlap(Rectangle rect1, Rectangle rect2, float rotation1, float rotation2)
		{
			Vector2[] points1 = new Vector2[4];
			Vector2[] points2 = new Vector2[4];
			points1[0] = new Vector2(rect1.Left, rect1.Top);
			points1[1] = new Vector2(rect1.Right, rect1.Top);
			points1[2] = new Vector2(rect1.Right, rect1.Bottom);
			points1[3] = new Vector2(rect1.Left, rect1.Bottom);
			points2[0] = new Vector2(rect2.Left, rect2.Top);
			points2[1] = new Vector2(rect2.Right, rect2.Top);
			points2[2] = new Vector2(rect2.Right, rect2.Bottom);
			points2[3] = new Vector2(rect2.Left, rect2.Bottom);
			float cos1 = (float)Math.Cos(rotation1);
			float cos2 = (float)Math.Cos(rotation2);
			float sin1 = (float)Math.Sin(rotation1);
			float sin2 = (float)Math.Sin(rotation2);
			for (int i = 0; i < 4; ++i)
			{
				// rotate them now
				points1[i] = new Vector2((cos1 * points1[i].X) - (sin1 * points1[i].Y), (cos1 * points1[i].Y) + (sin1 * points1[i].X));
				points2[i] = new Vector2((cos2 * points2[i].X) - (sin2 * points2[i].Y), (cos2 * points2[i].Y) + (sin2 * points2[i].X));
			}

			return !(HasSeparateAxis(points1, points2) || HasSeparateAxis(points2, points1));
		}
	}
}