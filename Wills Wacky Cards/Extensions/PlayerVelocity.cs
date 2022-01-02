using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnboundLib;

namespace WWC.Extensions
{
    public static class PlayerVelocityExtension
    {
        public static void AddForce(this PlayerVelocity playerVelocity, Vector2 force, ForceMode2D forceMode)
        {
			if (forceMode == ForceMode2D.Force)
			{
				force *= 0.02f;
			}
			else
			{
				force *= 1f;
			}

			playerVelocity.SetFieldValue("velocity", (Vector2)playerVelocity.GetFieldValue("velocity") + (force / (float) playerVelocity.GetFieldValue("mass")));
		}
    }
}