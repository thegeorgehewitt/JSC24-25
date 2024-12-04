using System.Collections.Generic;

using UnityEditor;
using UnityEngine;
using UnityEditor.AnimatedValues;
using UnityEditor.SceneManagement;

using FunkyCode.LightingSettings;
using FunkyCode.LightSettings;
using FunkyCode.Utilities;

using Custom.Editor.Styles;

namespace FunkyCode
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(Light2D))]
	public class Light2DEditor : Editor
	{
		private Light2D light2D;

		private SerializedProperty lightType;

		private SerializedProperty lightPresetId;
		private SerializedProperty eventPresetId;
		private SerializedProperty lightLayer;
		private SerializedProperty occlusionLayer;
		private SerializedProperty translucentLayer;
		private SerializedProperty translucentPresetId;

		private SerializedProperty color;

		private SerializedProperty size;
		private SerializedProperty spotAngleInner;
		private SerializedProperty spotAngleOuter;

		private SerializedProperty eventImpactCurveMap;

        private SerializedProperty outerAngle;
		private SerializedProperty shadowDistanceClose;
		private SerializedProperty shadowDistanceFar;

		private SerializedProperty maskTranslucencyType;
		private SerializedProperty maskTranslucencyStrength;

		private SerializedProperty lightStrength;

        private SerializedProperty textureSize;

		private SerializedProperty lightSprite;
		private SerializedProperty spriteFlipX;
		private SerializedProperty spriteFlipY;
		private SerializedProperty sprite;

		private SerializedProperty freeFormPoints;
		private SerializedProperty freeFormFalloff;
		private SerializedProperty freeFormPoint;
		private SerializedProperty freeFormFalloffStrength;

		private SerializedProperty whenInsideCollider;


		private bool translucentFoldout = false;

		private AnimBool expandLightProperties;
		private AnimBool expandShadowProperties;
		private AnimBool expandEventProperties;
		private AnimBool expandOverlayProperties;
		private AnimBool expandBumpMapProperties;

        private bool IsLightPropertiesExpanded
        {
            get { return SessionState.GetBool($"Light Properties Expanded ({GetType()}) : {serializedObject.targetObject.GetInstanceID()}", false); }
            set { SessionState.SetBool($"Light Properties Expanded ({GetType()}) : {serializedObject.targetObject.GetInstanceID()}", value); }
        }

        private bool IsShadowPropertiesExpanded
        {
            get { return SessionState.GetBool($"Shadow Properties Expanded ({GetType()}) : {serializedObject.targetObject.GetInstanceID()}", false); }
            set { SessionState.SetBool($"Shadow Properties Expanded ({GetType()}) : {serializedObject.targetObject.GetInstanceID()}", value); }
        }

        private bool IsEventPropertiesExpanded
        {
            get { return SessionState.GetBool($"Event Properties Expanded ({GetType()}) : {serializedObject.targetObject.GetInstanceID()}", false); }
            set { SessionState.SetBool($"Event Properties Expanded ({GetType()}) : {serializedObject.targetObject.GetInstanceID()}", value); }
        }

        private bool IsOverlayPropertiesExpanded
        {
            get { return SessionState.GetBool($"Overlay Properties Expanded ({GetType()}) : {serializedObject.targetObject.GetInstanceID()}", false); }
            set { SessionState.SetBool($"Overlay Properties Expanded ({GetType()}) : {serializedObject.targetObject.GetInstanceID()}", value); }
        }

        private bool IsBumpMapPropertiesExpanded
        {
            get { return SessionState.GetBool($"Bump Map Properties Expanded ({GetType()}) : {serializedObject.targetObject.GetInstanceID()}", false); }
            set { SessionState.SetBool($"Bump Map Properties Expanded ({GetType()}) : {serializedObject.targetObject.GetInstanceID()}", value); }
        }



        #region On Enable
        private void OnEnable()
		{
			light2D = target as Light2D;

			InitProperties();
			InitAnimBools();

            Undo.undoRedoPerformed += RefreshAll;
		}

		internal void OnDisable()
		{
			Undo.undoRedoPerformed -= RefreshAll;
		}
		private void InitProperties()
		{
			lightType = serializedObject.FindProperty("lightType");

			lightPresetId = serializedObject.FindProperty("lightPresetId");
			color = serializedObject.FindProperty("color");
			size = serializedObject.FindProperty("size");
			spotAngleInner = serializedObject.FindProperty("spotAngleInner");
			spotAngleOuter = serializedObject.FindProperty("spotAngleOuter");
			lightStrength = serializedObject.FindProperty("lightStrength");

            whenInsideCollider = serializedObject.FindProperty("whenInsideCollider");


            eventPresetId = serializedObject.FindProperty("eventPresetId");
            eventImpactCurveMap = serializedObject.FindProperty("eventImpactCurveMap");


            lightLayer = serializedObject.FindProperty("lightLayer");
			occlusionLayer = serializedObject.FindProperty("occlusionLayer");
			translucentLayer = serializedObject.FindProperty("translucentLayer");
			translucentPresetId = serializedObject.FindProperty("translucentPresetId");
			
			shadowDistanceClose = serializedObject.FindProperty("shadowDistanceClose");
			shadowDistanceFar = serializedObject.FindProperty("shadowDistanceFar");

			outerAngle = serializedObject.FindProperty("outerAngle");


            textureSize = serializedObject.FindProperty("textureSize");

			lightSprite = serializedObject.FindProperty("lightSprite");
			sprite = serializedObject.FindProperty("sprite");
			spriteFlipX = serializedObject.FindProperty("spriteFlipX");
			spriteFlipY = serializedObject.FindProperty("spriteFlipY");

            maskTranslucencyType = serializedObject.FindProperty("maskTranslucencyQuality");
			maskTranslucencyStrength = serializedObject.FindProperty("maskTranslucencyStrength");

			freeFormPoints = serializedObject.FindProperty("freeFormPoints.points");
			freeFormFalloff = serializedObject.FindProperty("freeFormFalloff");
			freeFormPoint = serializedObject.FindProperty("freeFormPoint");
			freeFormFalloffStrength = serializedObject.FindProperty("freeFormFalloffStrength");
        }

		private void InitAnimBools()
		{
			expandLightProperties = new(IsLightPropertiesExpanded);
			expandLightProperties.valueChanged.AddListener(Repaint);

			expandShadowProperties = new(IsShadowPropertiesExpanded);
			expandShadowProperties.valueChanged.AddListener(Repaint);

			expandEventProperties = new(IsEventPropertiesExpanded);
			expandEventProperties.valueChanged.AddListener(Repaint);

			expandOverlayProperties = new(IsOverlayPropertiesExpanded);
			expandOverlayProperties.valueChanged.AddListener(Repaint);

			expandBumpMapProperties = new(IsBumpMapPropertiesExpanded);
			expandBumpMapProperties.valueChanged.AddListener(Repaint);
        }

		void RefreshAll()
		{
			Light2D.ForceUpdateAll();
		}
        #endregion

        #region On Scene GUI
        private void OnSceneGUI()
		{
			if (light2D == null)
			{
				return;
			}

			bool changed = false;

            Undo.RecordObject(light2D, $"Modified {light2D.name} Light 2D");
            Undo.RecordObject(light2D.transform, $"Modified {light2D.name} Light 2D");

            switch (light2D.lightType)
			{
				case Light2D.LightType.FreeForm:
					changed = OnScene_FreeForm();
				break;

				case Light2D.LightType.Point:
				case Light2D.LightType.Sprite:
					changed = OnScene_Point();
				break;
			}

            if (changed)
			{
				light2D.ForceUpdate();

				if (!EditorApplication.isPlaying)
				{
					EditorUtility.SetDirty(target);
	
					EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
				}
			}
		}

		private void DrawPoints(List<Vector2> points)
		{
			for(int i = 0; i < points.Count; i++)
			{
				Vector3 point = points[i];

				point.z = light2D.transform.position.z;

				Vector3 nextPoint = points[(i + 1) % points.Count];

				point.x += light2D.transform2D.position.x;
				point.y += light2D.transform2D.position.y;

				nextPoint.x += light2D.transform2D.position.x;
				nextPoint.y += light2D.transform2D.position.y;

				Handles.DrawLine(point, nextPoint);
			}
		}

		public Camera GetSceneCamera()
		{
			UnityEditor.SceneView sceneView = UnityEditor.SceneView.lastActiveSceneView;
			
			Camera camera = null;

			if (sceneView != null)
			{
				camera = sceneView.camera;

				if (!camera.orthographic)
				{
					camera = null;
				}
			}

			return(camera);
		}

		private bool OnScene_FreeForm()
		{
			Camera camera = GetSceneCamera();

			if (camera == null)
			{
				return(false);
			}

			bool changed = true;

			float cameraSize = camera.orthographicSize;

			Handles.color = new Color(1, 0.4f, 0);	

			List<Vector2> points = light2D.freeFormPoints.points;

			bool intersect = false;

			for(int i = 0; i < points.Count; i++)
			{
				Vector3 point = points[i];

				point.z = light2D.transform.position.z;

				Vector3 nextPoint = points[(i + 1) % points.Count];

				nextPoint.z = light2D.transform.position.z;

				point.x += light2D.transform2D.position.x;
				point.y += light2D.transform2D.position.y;

				nextPoint.x += light2D.transform2D.position.x;
				nextPoint.y += light2D.transform2D.position.y;

				Handles.DrawLine(point, nextPoint);

				var fmh_201_52_638664052838452240 = Quaternion.identity; Vector3 result = Handles.FreeMoveHandle(point, 0.05f * cameraSize, Vector2.zero, Handles.CylinderHandleCap);

				if (point != result)
				{
					result.x -= light2D.transform2D.position.x;
					result.y -= light2D.transform2D.position.y;

					List<Vector2> cPoints = new List<Vector2>(points);

					cPoints[i] = result;

					if (Utilities.Math2D.PolygonIntersectItself(cPoints))
					{
						intersect = true;
					}
						else
					{
						points[i] = result;

						changed = true;
					}
				}
			}

			if (!intersect)
			{
				DrawPoints(points);
			}

			return(changed);
		}

		private bool OnScene_Point()
		{
			Camera camera = GetSceneCamera();

			if (camera == null) return false;


            bool changed = false;
            float cameraSize = camera.orthographicSize;
            float rotation = (light2D.transform.localRotation.eulerAngles.z + 90) * Mathf.Deg2Rad;
			Vector3 center = light2D.transform.position;
            float transformRotation = light2D.transform.rotation.eulerAngles.z;

			Color yellow = Color.yellow;
			Color orange = new Color(1, 0.4f, 0);

            #region Radius
            Handles.color = orange;	

            Vector3 radiusControlPos = center;
            radiusControlPos.x += Mathf.Cos(rotation) * light2D.size;
            radiusControlPos.y += Mathf.Sin(rotation) * light2D.size;

            Vector3 result = Handles.FreeMoveHandle(radiusControlPos, 0.05f * cameraSize, Vector2.zero, Handles.CylinderHandleCap);
			float moveDistance = Vector2.Distance(radiusControlPos, result);

			if (moveDistance > 0)
			{
				float newSize = Vector2.Distance(result, light2D.transform2D.position);
				light2D.size = newSize;

				changed = true;
            }
            #endregion

            #region Rotation
            float originAngle = 90f + (int)(Mathf.Atan2 (center.y - radiusControlPos.y, center.x - radiusControlPos.x) * Mathf.Rad2Deg);
			float rotateAngle = 90f + (int)(Mathf.Atan2 (center.y - result.y, center.x - result.x) * Mathf.Rad2Deg);

			rotateAngle = Math2D.NormalizeRotation(rotateAngle);
			originAngle = Math2D.NormalizeRotation(originAngle);

			if (Mathf.Abs(rotateAngle - originAngle) > 0.001f)
			{
				Quaternion QRotation = light2D.transform.rotation;
				Vector3 vRotation = QRotation.eulerAngles;
				vRotation.z = rotateAngle;

				light2D.transform.rotation = Quaternion.Euler(vRotation);

				changed = true;
			}
            #endregion

            #region Outer Handles
            Handles.color = yellow;

            Vector3 outerPointLeft = center;
			float outerValue = (light2D.spotAngleOuter) * Mathf.Deg2Rad * 0.5f;
			outerPointLeft.x += Mathf.Cos(rotation + outerValue) * light2D.size;
			outerPointLeft.y += Mathf.Sin(rotation + outerValue) * light2D.size;

            Vector3 outerPointRight = center;
            outerPointRight.x += Mathf.Cos(rotation - outerValue) * light2D.size;
            outerPointRight.y += Mathf.Sin(rotation - outerValue) * light2D.size;

            Vector3 outerHandleLeft = Handles.FreeMoveHandle(outerPointLeft, 0.05f * cameraSize, Vector2.zero, Handles.CylinderHandleCap);
            Vector3 outerHandleRight = Handles.FreeMoveHandle(outerPointRight, 0.05f * cameraSize, Vector2.zero, Handles.CylinderHandleCap);

			if (Vector2.Distance(outerPointLeft, outerHandleLeft) > 0.001f)
			{
				rotateAngle = 90f + (int)(Mathf.Atan2(center.y - outerHandleLeft.y, center.x - outerHandleLeft.x) * Mathf.Rad2Deg);
				rotateAngle -= transformRotation;
                rotateAngle = Mathf.Max(Math2D.NormalizeRotation(rotateAngle), 0);

                light2D.spotAngleOuter = Mathf.Max(rotateAngle, 0) * 2f;
                light2D.spotAngleOuter = Mathf.Clamp(Mathf.Max(light2D.spotAngleInner, light2D.spotAngleOuter), 0, 360);

				changed = true;
			}

			if (Vector2.Distance(outerPointRight, outerHandleRight) > 0.01f)
			{
				rotateAngle = -90f - (int)(Mathf.Atan2 (center.y - outerHandleRight.y, center.x - outerHandleRight.x) * Mathf.Rad2Deg);
				rotateAngle += transformRotation;
                rotateAngle = Mathf.Max(Math2D.NormalizeRotation(rotateAngle), 0);

                light2D.spotAngleOuter = rotateAngle * 2f;
                light2D.spotAngleOuter = Mathf.Clamp(Mathf.Max(light2D.spotAngleInner, light2D.spotAngleOuter), 0, 360);

				changed = true;
			}
            #endregion

            #region Inner Handles
            Handles.color = orange;

			float innerValue = (light2D.spotAngleInner) * Mathf.Deg2Rad * 0.5f;

            Vector3 innerPointLeft = center;
			innerPointLeft.x += Mathf.Cos(rotation + innerValue) * light2D.size;
			innerPointLeft.y += Mathf.Sin(rotation + innerValue) * light2D.size;

            Vector3 innerPointRight = center;
            innerPointRight.x += Mathf.Cos(rotation - innerValue) * light2D.size;
            innerPointRight.y += Mathf.Sin(rotation - innerValue) * light2D.size;

            Vector3 innerHandleLeft = Handles.FreeMoveHandle(innerPointLeft, 0.05f * cameraSize, Vector2.zero, Handles.CylinderHandleCap);
            Vector3 innerHandleRight = Handles.FreeMoveHandle(innerPointRight, 0.05f * cameraSize, Vector2.zero, Handles.CylinderHandleCap);

			if (Vector2.Distance(innerHandleLeft, innerPointLeft) > 0.001f)
			{
                rotateAngle = 90f + (int)(Mathf.Atan2(center.y - innerHandleLeft.y, center.x - innerHandleLeft.x) * Mathf.Rad2Deg);
                rotateAngle -= transformRotation;
                rotateAngle = Mathf.Max(Math2D.NormalizeRotation(rotateAngle), 0);

                light2D.spotAngleInner = rotateAngle * 2f;
                light2D.spotAngleInner = Mathf.Clamp(light2D.spotAngleInner, 0, 360);
                light2D.spotAngleOuter = Mathf.Max(light2D.spotAngleOuter, light2D.spotAngleInner);

				changed = true;
			}

            if (Vector2.Distance(innerHandleRight, innerPointRight) > 0.001f)
            {
                rotateAngle = -90f - (int)(Mathf.Atan2(center.y - innerHandleRight.y, center.x - innerHandleRight.x) * Mathf.Rad2Deg);
                rotateAngle += transformRotation;
                rotateAngle = Mathf.Max(Math2D.NormalizeRotation(rotateAngle), 0);

                light2D.spotAngleInner = rotateAngle * 2f;
                light2D.spotAngleInner = Mathf.Clamp(light2D.spotAngleInner, 0, 360);
                light2D.spotAngleOuter = Mathf.Max(light2D.spotAngleOuter, light2D.spotAngleInner);

                changed = true;
            }

            //Handles.DrawWireArc(center, Vector3.back, Vector3.right, light2D.spotAngleInner, Vector2.Distance(center, radiusControlPos));
            #endregion

            #region Light Area
            Handles.color = yellow;
            Handles.DrawDottedLine(center, outerPointLeft, 5);
            Handles.DrawDottedLine(center, outerPointRight, 5);
            Handles.DrawWireArc(center, Vector3.back, outerPointLeft - center, light2D.spotAngleOuter, Vector2.Distance(center, radiusControlPos));

            Handles.color = orange;
            Handles.DrawLine(center, innerPointLeft);
            Handles.DrawLine(center, innerPointRight);
            Handles.DrawWireArc(center, Vector3.back, innerPointLeft - center, light2D.spotAngleInner, Vector2.Distance(center, radiusControlPos));
            #endregion

            return changed;
		}
        #endregion

        #region On Inspector GUI
        public override void OnInspectorGUI()
		{
            #region General Properties
            EditorGUILayout.PropertyField(lightType, new GUIContent("Type"));
			lightPresetId.intValue = EditorGUILayout.Popup("Light Preset", lightPresetId.intValue, Lighting2D.Profile.lightPresets.GetPresetNames());

			EditorGUILayout.Space();
			
            light2D.applyRotation = (Light2D.Rotation)EditorGUILayout.EnumPopup("Rotation", light2D.applyRotation);
            EditorGUILayout.PropertyField(whenInsideCollider, new GUIContent("When Inside Collider"));

			bool customSize = Lighting2D.Profile.qualitySettings.lightTextureSize == LightingSourceTextureSize.Custom;
            EditorGUI.BeginDisabledGroup(!customSize);
            if (customSize)
            {
                textureSize.intValue = EditorGUILayout.Popup("Resolution", (int)light2D.textureSize, LightingSettings.QualitySettings.LightingSourceTextureSizeArray);
            }
            else
            {
                EditorGUILayout.Popup("Resolution", (int)Lighting2D.Profile.qualitySettings.lightTextureSize, LightingSettings.QualitySettings.LightingSourceTextureSizeArray);
            }
            EditorGUI.EndDisabledGroup();

			EditorGUILayout.Space();
            #endregion

            #region Light Properties
            IsLightPropertiesExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(IsLightPropertiesExpanded, "Light", CustomEditorStyles.foldoutHeader);
			EditorGUILayout.EndFoldoutHeaderGroup();
			expandLightProperties.target = IsLightPropertiesExpanded;

            if (EditorGUILayout.BeginFadeGroup(expandLightProperties.faded))
			{
				EditorGUILayout.Space();

                // Light color
                Color colorValue = EditorGUILayout.ColorField(new GUIContent("Color"), color.colorValue, true, true, true);
                colorValue.a = EditorGUILayout.Slider("Alpha", colorValue.a, 0, 1);
                color.colorValue = colorValue;

				EditorGUILayout.Space();

                // Light radius
                EditorGUI.BeginDisabledGroup(light2D.lightType == Light2D.LightType.FreeForm);
                size.floatValue = EditorGUILayout.Slider("Radius", size.floatValue, 0.1f, Lighting2D.ProjectSettings.MaxLightSize);
                EditorGUI.EndDisabledGroup();

                // Light shape
                if (light2D.lightLayer >= 0 || light2D.translucentLayer > 0 || light2D.occlusionLayer > 0)
                {
                    switch (light2D.lightType)
                    {
                        case Light2D.LightType.Sprite:
                            DrawLightSpotAngleSlider();

                            break;

                        case Light2D.LightType.Point:
                            DrawLightSpotAngleSlider();

                            lightStrength.floatValue = EditorGUILayout.Slider("Falloff", lightStrength.floatValue, 0, 1);

                            break;
                    }
                }

                switch (light2D.lightType)
                {
                    case Light2D.LightType.Sprite:

                        EditorGUILayout.Space();
                        EditorGUI.indentLevel++;

                        EditorGUILayout.PropertyField(lightSprite, new GUIContent("Type"));

                        if (light2D.lightSprite == Light2D.LightSprite.Custom)
                        {
                            EditorGUILayout.PropertyField(spriteFlipX, new GUIContent("Flip X"));
                            EditorGUILayout.PropertyField(spriteFlipY, new GUIContent("Flip Y"));

                            sprite.objectReferenceValue = (Sprite)EditorGUILayout.ObjectField("", sprite.objectReferenceValue, typeof(Sprite), true);
                        }
                        else
                        {
                            if (light2D.sprite != Light2D.GetDefaultSprite())
                            {
                                light2D.sprite = Light2D.GetDefaultSprite();
                            }
                        }

                        EditorGUI.indentLevel--;
                        EditorGUILayout.Space();

                        break;

                    case Light2D.LightType.FreeForm:

                        EditorGUILayout.Space();
                        EditorGUI.indentLevel++;

                        freeFormPoint.floatValue = EditorGUILayout.Slider("Point", freeFormPoint.floatValue, 0, 1);

                        EditorGUILayout.PropertyField(freeFormFalloff, new GUIContent("Falloff"));

                        freeFormFalloffStrength.floatValue = EditorGUILayout.Slider("Falloff Strength", freeFormFalloffStrength.floatValue, 0, 1);

                        freeFormFalloff.floatValue = Mathf.Max(freeFormFalloff.floatValue, 0);

                        EditorGUILayout.PropertyField(freeFormPoints, new GUIContent("Points"));

                        EditorGUI.indentLevel--;
                        EditorGUILayout.Space();

                        break;
                }

				EditorGUILayout.Space();
            }
            EditorGUILayout.EndFadeGroup();
            #endregion

            #region Event Properties
            IsEventPropertiesExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(IsEventPropertiesExpanded, "Event Handling", CustomEditorStyles.foldoutHeader);
            EditorGUILayout.EndFoldoutHeaderGroup();
            expandEventProperties.target = IsEventPropertiesExpanded;

			if (EditorGUILayout.BeginFadeGroup(expandEventProperties.faded))
			{
				EditorGUILayout.Space();

				eventPresetId.intValue = EditorGUILayout.Popup("Event Preset", eventPresetId.intValue, Lighting2D.Profile.eventPresets.GetBufferLayers());
				EditorGUILayout.PropertyField(eventImpactCurveMap);

				EditorGUILayout.Space();
            }
            EditorGUILayout.EndFadeGroup();
            #endregion

            #region Shadow Properties
            IsShadowPropertiesExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(IsShadowPropertiesExpanded, "Shadow", CustomEditorStyles.foldoutHeader);
			EditorGUILayout.EndFoldoutHeaderGroup();
			expandShadowProperties.target = IsShadowPropertiesExpanded;

            if (EditorGUILayout.BeginFadeGroup(expandShadowProperties.faded))
			{
				EditorGUILayout.Space();

				// Layers
                int value = lightLayer.intValue + 1;
                value = EditorGUILayout.Popup("Shadows (Light)", value, Lighting2D.Profile.layers.lightLayers.GetOcclusionNames());
                lightLayer.intValue = value - 1;

                occlusionLayer.intValue = EditorGUILayout.Popup("Occlusion (Light)", occlusionLayer.intValue, Lighting2D.Profile.layers.lightLayers.GetOcclusionNames());

                translucentLayer.intValue = EditorGUILayout.Popup("Translucency (Light)", translucentLayer.intValue, Lighting2D.Profile.layers.lightLayers.GetTranslucencyNames());

				// Shadow properties
                if (light2D.lightLayer >= 0 || light2D.translucentLayer > 0)
                {
                    EditorGUILayout.Space();

                    if (UsesLegacyShadows())
                    {
                        outerAngle.floatValue = EditorGUILayout.Slider("Soft Legacy Shadows", outerAngle.floatValue, 0, 60);
                    }

                    if (UsesSoftShadows())
                    {
                        light2D.coreSize = EditorGUILayout.Slider("Soft Shadows", light2D.coreSize, 0.1f, 10f);

                        light2D.falloff = EditorGUILayout.Slider("Soft Falloff", light2D.falloff, 0, 10f);
                    }

                    if (UsesSoftDefaultShadows())
                    {
                        light2D.lightRadius = EditorGUILayout.Slider("Soft Radius", light2D.lightRadius, 0.1f, 10f);
                    }

                    if (UsesDefaultShadows())
                    {
                        EditorGUILayout.PropertyField(shadowDistanceClose, new GUIContent("Soft Distance Close"));

                        EditorGUILayout.PropertyField(shadowDistanceFar, new GUIContent("Soft Distance Far"));
                    }
                }

                // Translucent
                EditorGUILayout.Space();

                if (light2D.translucentLayer > 0)
                {
					translucentFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(translucentFoldout, "Translucent Properties");
					EditorGUILayout.EndFoldoutHeaderGroup();

                    if (translucentFoldout)
					{
                        EditorGUI.indentLevel++;

                        translucentPresetId.intValue = EditorGUILayout.Popup("Light Preset", translucentPresetId.intValue, Lighting2D.Profile.lightPresets.GetPresetNames());

                        EditorGUILayout.PropertyField(maskTranslucencyType, new GUIContent("Quality"));

                        maskTranslucencyStrength.floatValue = EditorGUILayout.Slider("Strength", maskTranslucencyStrength.floatValue, 0, 1);

                        EditorGUI.indentLevel--;
                    }
                }

                EditorGUILayout.Space();
            }
            EditorGUILayout.EndFadeGroup();
            #endregion

            #region Overlay Properties
            DrawMeshMode(light2D.meshMode);
            #endregion

            #region Bump Map Properties
            if (UsesMasks())
            {
                IsBumpMapPropertiesExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(IsBumpMapPropertiesExpanded, "Bump Map", CustomEditorStyles.foldoutHeader);
				EditorGUILayout.EndFoldoutHeaderGroup();
				expandBumpMapProperties.target = IsBumpMapPropertiesExpanded;

                if (EditorGUILayout.BeginFadeGroup(expandBumpMapProperties.faded))
                {
                    EditorGUILayout.Space();
                    EditorGUI.indentLevel++;

                    light2D.bumpMap.intensity = EditorGUILayout.Slider("Intensity", light2D.bumpMap.intensity, 0, 2);
                    light2D.bumpMap.depth = EditorGUILayout.Slider("Depth", light2D.bumpMap.depth, 0.1f, 20f);

                    EditorGUI.indentLevel--;
                    EditorGUILayout.Space();
                }
                EditorGUILayout.EndFadeGroup();
            }
            #endregion

			serializedObject.ApplyModifiedProperties();

            if (GUI.changed) OnGUIChanged();
        }

		public bool UsesSoftShadows()
		{
			LayerSetting[] layerSettings = light2D.GetLightPresetLayers();

			for(int i = 0; i < layerSettings.Length; i++)
			{
				LayerSetting setting = layerSettings[i];

				if (setting.shadowEffect == LightLayerShadowEffect.SoftConvex || setting.shadowEffect == LightLayerShadowEffect.SoftConvex)
				{
					return(true);
				}
			}

			layerSettings = light2D.GetTranslucencyPresetLayers();

			for(int i = 0; i < layerSettings.Length; i++)
			{
				LayerSetting setting = layerSettings[i];

				if (setting.shadowEffect == LightLayerShadowEffect.SoftConvex || setting.shadowEffect == LightLayerShadowEffect.SoftConvex)
				{
					return(true);
				}
			}

			return(false);
		}
		
		public bool UsesSoftDefaultShadows()
		{
			LayerSetting[] layerSettings = light2D.GetLightPresetLayers();

			for(int i = 0; i < layerSettings.Length; i++)
			{
				LayerSetting setting = layerSettings[i];

				if (setting.shadowEffect == LightLayerShadowEffect.Soft)
				{
					return(true);
				}
			}

			layerSettings = light2D.GetTranslucencyPresetLayers();

			for(int i = 0; i < layerSettings.Length; i++)
			{
				LayerSetting setting = layerSettings[i];

				if (setting.shadowEffect == LightLayerShadowEffect.Soft)
				{
					return(true);
				}
			}

			return(false);
		}

		public bool UsesMasks()
		{
			LayerSetting[] layerSettings = light2D.GetLightPresetLayers();

			for(int i = 0; i < layerSettings.Length; i++)
			{
				LayerSetting setting = layerSettings[i];

				if (setting.type == LightLayerType.MaskOnly || setting.type == LightLayerType.ShadowAndMask)
				{
					return(true);
				}
			}

			layerSettings = light2D.GetTranslucencyPresetLayers();

			for(int i = 0; i < layerSettings.Length; i++)
			{
				LayerSetting setting = layerSettings[i];

				if (setting.type == LightLayerType.MaskOnly || setting.type == LightLayerType.ShadowAndMask)
				{
					return(true);
				}
			}

			return(false);
		}

		public bool UsesLegacyShadows()
		{
			LayerSetting[] layerSettings = light2D.GetLightPresetLayers();

			for(int i = 0; i < layerSettings.Length; i++)
			{
				LayerSetting setting = layerSettings[i];

				if (setting.shadowEffect == LightLayerShadowEffect.LegacyCPU || setting.shadowEffect == LightLayerShadowEffect.LegacyGPU)
				{
					return(true);
				}
			}

			layerSettings = light2D.GetTranslucencyPresetLayers();

			for(int i = 0; i < layerSettings.Length; i++)
			{
				LayerSetting setting = layerSettings[i];

				if (setting.shadowEffect == LightLayerShadowEffect.LegacyCPU || setting.shadowEffect == LightLayerShadowEffect.LegacyGPU)
				{
					return(true);
				}
			}

			return(false);
		}

		public bool UsesDefaultShadows()
		{
			LayerSetting[] layerSettings = light2D.GetLightPresetLayers();

			for(int i = 0; i < layerSettings.Length; i++)
			{
				LayerSetting setting = layerSettings[i];

				if (setting.shadowEffect == LightLayerShadowEffect.Default)
				{
					return(true);
				}
			}

			layerSettings = light2D.GetTranslucencyPresetLayers();

			for(int i = 0; i < layerSettings.Length; i++)
			{
				LayerSetting setting = layerSettings[i];

				if (setting.shadowEffect == LightLayerShadowEffect.Default)
				{
					return(true);
				}
			}

			return(false);
		}

        private void DrawMeshMode(MeshMode meshMode)
        {
            IsOverlayPropertiesExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(IsOverlayPropertiesExpanded, "Overlay", CustomEditorStyles.foldoutHeader);
			EditorGUILayout.EndFoldoutHeaderGroup();
			expandOverlayProperties.target = IsOverlayPropertiesExpanded;

			if (EditorGUILayout.BeginFadeGroup(expandOverlayProperties.faded))
			{
				EditorGUILayout.Space();
                EditorGUI.indentLevel++;

                SerializedProperty meshModeEnable = serializedObject.FindProperty("meshMode.enable");
                SerializedProperty meshModeAlpha = serializedObject.FindProperty("meshMode.alpha");
                SerializedProperty meshModeShader = serializedObject.FindProperty("meshMode.shader");

                EditorGUILayout.PropertyField(meshModeEnable, new GUIContent("Enable"));

                meshModeAlpha.floatValue = EditorGUILayout.Slider("Alpha", meshModeAlpha.floatValue, 0, 1);

                EditorGUILayout.PropertyField(meshModeShader, new GUIContent("Material"));

                if (meshModeShader.intValue == (int)MeshModeShader.Custom)
                {
                    bool value2 = GUIFoldout.Draw("Materials", meshMode.materials);

                    if (value2)
                    {
                        EditorGUI.indentLevel++;

                        int count = meshMode.materials.Length;
                        count = EditorGUILayout.IntSlider("Material Count", count, 0, 10);

                        if (count != meshMode.materials.Length)
                        {
                            System.Array.Resize(ref meshMode.materials, count);
                        }

                        for (int id = 0; id < meshMode.materials.Length; id++)
                        {
                            Material material = meshMode.materials[id];

                            material = (Material)EditorGUILayout.ObjectField("Material", material, typeof(Material), true);

                            meshMode.materials[id] = material;
                        }

                        EditorGUI.indentLevel--;
                    }
                }

                GUISortingLayer.Draw(serializedObject, meshMode.sortingLayer, "meshMode.");

                EditorGUI.indentLevel--;
				EditorGUILayout.Space();
            }
			EditorGUILayout.EndFadeGroup();
        }

        private void DrawLightSpotAngleSlider()
		{
			GUIStyle rightLabel = new GUIStyle(EditorStyles.boldLabel);
			rightLabel.alignment = TextAnchor.UpperRight;


            float controlEndsWidth = 50;
            float inner = spotAngleInner.floatValue;
			float outer = spotAngleOuter.floatValue;


            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Spot Angle");

            EditorGUILayout.BeginVertical();
            spotAngleInner.floatValue = EditorGUILayout.FloatField(spotAngleInner.floatValue, GUILayout.Width(controlEndsWidth));
			EditorGUILayout.LabelField("Inner", EditorStyles.boldLabel, GUILayout.Width(controlEndsWidth));
            EditorGUILayout.EndVertical();

            EditorGUILayout.MinMaxSlider(ref inner, ref outer, 0f, 360f);

            EditorGUILayout.BeginVertical();
            spotAngleOuter.floatValue = EditorGUILayout.FloatField(spotAngleOuter.floatValue, GUILayout.Width(controlEndsWidth));
            EditorGUILayout.LabelField("Outer", rightLabel, GUILayout.Width(controlEndsWidth));
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();


            spotAngleInner.floatValue = System.MathF.Round(inner, 1);
            spotAngleOuter.floatValue = System.MathF.Round(outer, 1);
        }

		private void OnGUIChanged()
		{
            foreach (UnityEngine.Object target in targets)
            {
                if (target == null)
                {
                    continue;
                }

                Light2D light2D = target as Light2D;

                if (light2D == null)
                {
                    continue;
                }

                light2D.ForceUpdate();

                if (!EditorApplication.isPlaying)
                {
                    EditorUtility.SetDirty(target);
                }
            }

            if (!EditorApplication.isPlaying)
            {
                EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
            }
        }
        #endregion
    }
}
