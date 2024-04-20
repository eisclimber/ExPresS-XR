using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.ProBuilder;
using UnityEditor.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.ProBuilder.Shapes;
using ExPresSXR.Experimentation.DataGathering;

namespace ExPresSXR.Editor.Utility
{
    public static class RoomCreationUtils
    {
        // Experimentation default material paths
        const string EXPERIMENTATION_FLOOR_MATERIAL_PATH = "Assets/ExPresS XR/Materials/World Materials/PVC Floor Blue Speckled.mat";
        const string EXPERIMENTATION_CEILING_MATERIAL_PATH = "Assets/ExPresS XR/Materials/World Materials/Woodchip Wall.mat";
        const string EXPERIMENTATION_WALL_MATERIAL_PATH = "Assets/ExPresS XR/Materials/World Materials/Woodchip Wall.mat";

        // Exhibition default material paths
        const string EXHIBITION_FLOOR_MATERIAL_PATH = "Assets/ExPresS XR/Materials/World Materials/Wood Fishbone.mat";
        const string EXHIBITION_WALL_MATERIAL_PATH = "Assets/ExPresS XR/Materials/World Materials/Woodchip Wall.mat";
        const string EXHIBITION_CEILING_MATERIAL_PATH = "Assets/ExPresS XR/Materials/World Materials/Woodchip Wall.mat";


        // Teleportation area placement offset
        const float TELEPORTATION_AREA_Y_OFFSET = 0.001f;

        // Threshold of an face to recognized as floor (dot product of two normalized vectors)
        const float DOT_FLOOR_THRESHOLD = 0.95f;

        [MenuItem("ExPresS XR/Room Creation.../Create Default Experimentation Room", false, 5)]
        public static void CreateExperimentationRoom()
        {
            CreateRoom(6f, 3f, 5f, true, WallMode.SeparateFloor, MaterialPreset.Experimentation);
        }

        [MenuItem("ExPresS XR/Room Creation.../Create Default Exhibition Room", false, 6)]
        public static void CreateExhibitionRoom()
        {
            CreateRoom(5f, 3f, 4f, true, WallMode.SeparateFloor, MaterialPreset.Exhibition);
        }

        [MenuItem("ExPresS XR/Room Creation.../Add Teleportation to Selected Rooms", false, 17)]
        public static void AddSelectedTeleportationAreas()
        {
            bool performedAction = false;
            IEnumerable<ProBuilderMesh> selection = MeshSelection.top;

            foreach (ProBuilderMesh mesh in selection)
            {
                bool needsNewTeleportArea = mesh.name != "Teleportation Area"
                                                && mesh.transform.Find("Teleportation Area") == null;
                performedAction |= UpdateTeleportationArea(mesh, needsNewTeleportArea);
            }

            if (!performedAction)
            {
                Debug.Log("No Rooms or ProBuilderMeshes were selected that needed an Teleportation Area. Select the ones you want to add a Teleportation Area to.");
            }
        }

        [MenuItem("ExPresS XR/Room Creation.../Update Teleportation of Selected Rooms", false, 18)]
        public static void UpdateSelectedTeleportationAreas()
        {
            bool performedAction = false;
            IEnumerable<ProBuilderMesh> selection = MeshSelection.top;

            foreach (ProBuilderMesh mesh in selection)
            {
                performedAction |= UpdateTeleportationArea(mesh);
            }

            if (!performedAction)
            {
                Debug.Log("No Rooms or ProBuilderMeshes were selected that needed an update of their Teleportation Area. Select the ones you want to update their Teleportation Area.");
            }
        }


        public static void CreateRoom(
            float width,
            float height,
            float depth,
            bool addTeleportationArea = true,
            WallMode mode = WallMode.SeparateFloor,
            MaterialPreset materialPreset = MaterialPreset.Experimentation)
        {
            CreateRoom(new Vector3(0, height / 2, 0), new Vector3(width, height, depth), addTeleportationArea, mode, materialPreset);
        }


        public static void CreateRoom(
            Vector3 roomPos,
            Vector3 roomSize,
            bool addTeleportationArea = true,
            WallMode mode = WallMode.SeparateFloor,
            MaterialPreset materialPreset = MaterialPreset.Experimentation)
        {
            ProBuilderMesh room = ShapeGenerator.GenerateCube(PivotLocation.Center, roomSize);

            room.name = "Room";
            room.transform.position = roomPos;

            // Reverse normals to turn the face inwards
            foreach (Face face in room.faces)
            {
                face.Reverse();
            }

            // Assign Materials to faces
            AssignRoomMaterials(room, mode, materialPreset);

            // Mesh cleanup
            PerformProBuilderMeshUpdate(room);

            // Add Collision
            room.gameObject.AddComponent<MeshCollider>();

            if (addTeleportationArea)
            {
                UpdateTeleportationArea(room, true);
            }

            // General Usability
            Selection.activeGameObject = room.gameObject;
            GameObjectUtility.EnsureUniqueNameForSibling(room.gameObject);
            Undo.RegisterCreatedObjectUndo(room.gameObject, "Created ExPresS XR Room");
        }

        private static void AssignRoomMaterials(ProBuilderMesh room, WallMode mode, MaterialPreset materialPreset)
        {
            if (room == null || room.GetComponent<MeshRenderer>() == null)
            {
                Debug.LogError("Cannot assign materials to a non-existing room or one without renderer!");
                return;
            }

            MeshRenderer roomRenderer = room.GetComponent<MeshRenderer>();

            Material wallMaterial = AssetDatabase.LoadAssetAtPath<Material>(GetWallPathFromPreset(materialPreset));
            Material ceilingMaterial = AssetDatabase.LoadAssetAtPath<Material>(GetCeilingPathFromPreset(materialPreset));
            Material floorMaterial = AssetDatabase.LoadAssetAtPath<Material>(GetFloorPathFromPreset(materialPreset));

            // The faces of a generated cube are enumerated as in ROOM_FACE_IDXS (when looking towards positive z and x towards right)
            switch (mode)
            {
                case WallMode.SeparateFloor:
                    // Make a list of materials
                    roomRenderer.materials = new Material[] { wallMaterial, floorMaterial };
                    room.faces[(int)RoomFaceIds.Floor].submeshIndex = 1;
                    break;
                case WallMode.SeparateFloorAndCeiling:
                    // Make a list of materials
                    roomRenderer.materials = new Material[] { wallMaterial, ceilingMaterial, floorMaterial };
                    room.faces[(int)RoomFaceIds.Ceiling].submeshIndex = 1;
                    room.faces[(int)RoomFaceIds.Floor].submeshIndex = 2;
                    break;
                case WallMode.AllSeparate:
                    // Make a list of materials
                    roomRenderer.materials = new Material[] { wallMaterial, wallMaterial, wallMaterial, wallMaterial, ceilingMaterial, floorMaterial };

                    // If each side has it's own material, use a mapping of face index to material index
                    for (int i = 0; i < roomRenderer.sharedMaterials.Length; i++)
                    {
                        room.faces[i].submeshIndex = i;
                    }
                    break;
                default: // == WallMode.SINGLE
                    roomRenderer.materials = new Material[] { wallMaterial };
                    // No Mapping needed as all have default submeshIndex = 0
                    break;
            }
        }


        public static bool UpdateTeleportationArea(ProBuilderMesh parentMesh, bool addIfNotExists = false)
        {
            Transform tpTransform = parentMesh.transform.Find("Teleportation Area");
            bool needsUpdate = tpTransform != null || (addIfNotExists && tpTransform == null);

            if (needsUpdate)
            {
                // Find faces to be deleted (= no floors)
                List<int> inverse = new();
                for (int i = 0; i < parentMesh.faceCount; i++)
                {
                    if (!IsFloor(parentMesh, parentMesh.faces[i]))
                    {
                        inverse.Add(i);
                    }
                }

                // Abort if no floor was found
                if (inverse.Count >= parentMesh.faceCount)
                {
                    Debug.LogError("Could not find any suitable floors in the room. No TeleportationAreas created.");
                    return false;
                }

                // Copy new Floor
                ProBuilderMesh copy = Object.Instantiate(parentMesh.gameObject, parentMesh.transform.parent).GetComponent<ProBuilderMesh>();
                copy.MakeUnique();
                UnityEditor.ProBuilder.EditorUtility.SynchronizeWithMeshFilter(copy);

                if (copy.transform.childCount > 0)
                {
                    for (int i = copy.transform.childCount - 1; i > -1; i--)
                    {
                        Object.DestroyImmediate(copy.transform.GetChild(i).gameObject);
                    }

                    foreach (ProBuilderMesh child in parentMesh.transform.GetComponentsInChildren<ProBuilderMesh>())
                    {
                        UnityEditor.ProBuilder.EditorUtility.SynchronizeWithMeshFilter(child);
                    }
                }

                Undo.RegisterCreatedObjectUndo(copy.gameObject, "Update Floor Teleportation");

                copy.DeleteFaces(inverse);
                PerformProBuilderMeshUpdate(copy);

                MakeMeshTeleportationArea(copy.gameObject, parentMesh.transform, tpTransform);
            }
            ProBuilderEditor.Refresh();
            return needsUpdate;
        }

        private static void MakeMeshTeleportationArea(GameObject areaObject, Transform roomTransform, Transform prevTeleportationArea)
        {
            if (areaObject != null && areaObject.GetComponent<ProBuilderMesh>() && roomTransform != null)
            {
                // Disable mesh renderer
                areaObject.GetComponent<MeshRenderer>().enabled = false;

                // Setup Teleportation Area (Collisions and layers)
                TeleportationArea area = areaObject.AddComponent<TeleportationArea>();
                area.interactionLayers = 1 << InteractionLayerMask.NameToLayer("Teleport");
                area.colliders.Add(areaObject.GetComponent<MeshCollider>());

                // Copy Old Teleportation Area Config
                if (prevTeleportationArea != null && prevTeleportationArea.GetComponent<TeleportationArea>() != null)
                {
                    TeleportationArea prevArea = prevTeleportationArea.GetComponent<TeleportationArea>();
                    // Copy component
                    area.customReticle = prevArea.customReticle;

                    // Copy "Match Directional Input" for teleport anchor control
                    area.matchDirectionalInput = prevArea.matchDirectionalInput;
                }
                else
                {
                    // "Match Directional Input" should be set to true per default
                    area.matchDirectionalInput = true;
                }


                // Delete any previous Teleportation Area if exists under the parent
                if (prevTeleportationArea != null)
                {
                    Object.DestroyImmediate(prevTeleportationArea.gameObject);
                }

                // Parent it to the room and move up (prevents errors with teleportation detection)
                areaObject.transform.parent = roomTransform;
                areaObject.name = "Teleportation Area";
                areaObject.transform.position += new Vector3(0, TELEPORTATION_AREA_Y_OFFSET, 0);
            }
            else
            {
                Debug.LogError("Invalid areaObject or roomTransform");
            }
        }


        private static void PerformProBuilderMeshUpdate(ProBuilderMesh mesh)
        {
            mesh.ToMesh();
            mesh.Refresh();
            mesh.Optimize();
        }


        private static Vector3 GetFaceNormal(ProBuilderMesh mesh, Face face)
        {
            if (face == null || !MeshContainsFace(mesh, face))
            {
                return new Vector3();
            }

            Vertex[] vertices = mesh.GetVertices(face.distinctIndexes);

            if (vertices.Length > 3)
            {
                Vector3 v1 = vertices[0].position;
                Vector3 v2 = vertices[1].position;
                Vector3 v3 = vertices[2].position;

                return Vector3.Cross(v2 - v1, v3 - v1).normalized;

            }
            return Vector3.zero;
        }

        private static bool MeshContainsFace(ProBuilderMesh mesh, Face face)
        {
            if (face == null)
            {
                return false;
            }
            foreach (Face face1 in mesh.faces)
            {
                if (face1 == face)
                {
                    return true;
                }
            }
            return false;
        }

        private static bool IsFloor(ProBuilderMesh mesh, Face face)
        {
            var faceNormal = GetFaceNormal(mesh, face);
            if (faceNormal != Vector3.zero)
            {
                return Vector3.Dot(faceNormal, Vector3.up) >= DOT_FLOOR_THRESHOLD;
            }
            return false;
        }


        private static string GetWallPathFromPreset(MaterialPreset materialPreset)
        {
            return materialPreset switch
            {
                MaterialPreset.Exhibition => EXHIBITION_WALL_MATERIAL_PATH,
                _ => EXPERIMENTATION_WALL_MATERIAL_PATH,
            };
        }

        private static string GetFloorPathFromPreset(MaterialPreset materialPreset)
        {
            return materialPreset switch
            {
                MaterialPreset.Exhibition => EXHIBITION_FLOOR_MATERIAL_PATH,
                _ => EXPERIMENTATION_FLOOR_MATERIAL_PATH,
            };
        }

        private static string GetCeilingPathFromPreset(MaterialPreset materialPreset)
        {
            return materialPreset switch
            {
                MaterialPreset.Exhibition => EXHIBITION_CEILING_MATERIAL_PATH,
                _ => EXPERIMENTATION_CEILING_MATERIAL_PATH,
            };
        }
    }

    public enum WallMode
    {
        Single,
        SeparateFloor,
        SeparateFloorAndCeiling,
        AllSeparate
        // Assembly Name: ExPresSXR.Editor.Utility.WallMode,Assembly-CSharp-Editor
    }

    public enum MaterialPreset
    {
        Experimentation,
        Exhibition

        // Assembly Name: ExPresSXR.Editor.Utility.MaterialPreset,Assembly-CSharp-Editor
    }


    public enum RoomFaceIds
    {
        // When looking at the room towards positive z
        WallBack,
        WallRight,
        WallFront,
        WallLeft,
        Ceiling,
        Floor
    }
}
