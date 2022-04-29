using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.ProBuilder;
using UnityEditor.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.XR.Interaction.Toolkit;

public static class AutoXRRoomCreationUtils
{
    const string DEFAULT_FLOOR_MATERIAL_PATH = "Assets/AutoXR/Materials/Room/Room Floor.mat";
    const string DEFAULT_WALL_MATERIAL_PATH = "Assets/AutoXR/Materials/Room/Room Wall.mat";
    const string DEFAULT_CEILING_MATERIAL_PATH = "Assets/AutoXR/Materials/Room/Room Wall.mat";

    const string DEFAULT_TELEPORTATION_RETICLE_PATH = "Assets/AutoXR/Prefabs/Reticles/Teleport Reticle.prefab";

    const float TELEPORTATION_AREA_Y_OFFSET = 0.001f;

    // Threshold of an face to recognized as floor (dot product of two normalized vectors)
    const float DOT_FLOOR_THRESHOLD = 0.95f;

    enum ROOM_FACE_IDXS
    {
        WALL_BACK,
        WALL_RIGHT,
        WALL_FRONT,
        WALL_LEFT,
        CEILING,
        FLOOR
    }

    [MenuItem("AutoXR/Rooms.../Open Room Creator")]
    public static void LaunchRoomCreator(MenuCommand menuCommand)
    {
        Debug.Log("Room Creator");
    }

    [MenuItem("AutoXR/Rooms.../Create a New Test Room")]
    public static void CreateTestRoom(MenuCommand menuCommand)
    {
        CreateRoom(5f, 3f, 4f);
    }

    [MenuItem("AutoXR/Rooms.../Update Teleportation of Selected Rooms")]
    public static void UpdateSelectedTeleportationAreas()
    {
        var selection = MeshSelection.top;

        foreach (ProBuilderMesh mesh in selection)
        {
            UpdateTeleportationArea(mesh);
        }
    }

    [MenuItem("AutoXR/Rooms.../Add Teleportation to Selected Rooms")]
    public static void AddSelectedTeleportationAreas()
    {
        var selection = MeshSelection.top;

        foreach (ProBuilderMesh mesh in selection)
        {
            if (mesh.name != "Teleportation Area" && mesh.transform.Find("Teleportation Area") == null)
            {
                UpdateTeleportationArea(mesh, true);
            }
            UpdateTeleportationArea(mesh);
        }
    }


    public static void CreateRoom(float width, float height, float depth, MaterialMode mode = MaterialMode.SEPARATE_FLOOR, bool addTeleportationArea = true)
    {
        ProBuilderMesh room = ShapeGenerator.GenerateCube(PivotLocation.Center, new Vector3(width, height, depth));

        room.name = "Room";
        room.transform.position = new Vector3(0, height / 2, 0);

        // Reverse normals to turn the face inwards
        foreach (Face face in room.faces)
        {
            face.Reverse();
        }

        // Assign Materials to faces
        AssignRoomMaterials(room, mode);


        // Mesh cleanup
        room.ToMesh();
        room.Refresh();
        room.Optimize();

        // Add Collision
        room.gameObject.AddComponent<MeshCollider>();

        if (addTeleportationArea)
        {
            UpdateTeleportationArea(room, true);
        }

        // General Usability
        Selection.activeGameObject = room.gameObject;
        GameObjectUtility.EnsureUniqueNameForSibling(room.gameObject);
        Undo.RegisterCreatedObjectUndo(room.gameObject, "Created AutoXR Room");
    }

    private static void AssignRoomMaterials(ProBuilderMesh room, MaterialMode mode)
    {
        if (room == null || room.GetComponent<MeshRenderer>() == null)
        {
            Debug.LogError("Cannot assign materials to a non-existing room or one without renderer!");
            return;
        }

        MeshRenderer roomRenderer = room.GetComponent<MeshRenderer>();

        Material wallMaterial = AssetDatabase.LoadAssetAtPath<Material>(DEFAULT_WALL_MATERIAL_PATH);
        Material ceilingMaterial = AssetDatabase.LoadAssetAtPath<Material>(DEFAULT_CEILING_MATERIAL_PATH);
        Material floorMaterial = AssetDatabase.LoadAssetAtPath<Material>(DEFAULT_FLOOR_MATERIAL_PATH);

        // The faces of a generated cube are enumerated as in ROOM_FACE_IDXS (when looking towards positive z and x towards right)
        switch (mode)
        {
            case MaterialMode.SEPARATE_FLOOR:
                // Make a list of materials
                roomRenderer.materials = new Material[] { wallMaterial, floorMaterial };
                room.faces[(int)ROOM_FACE_IDXS.FLOOR].submeshIndex = 1;
                break;
            case MaterialMode.SEPARATE_FLOOR_AND_CEILING:
                // Make a list of materials
                roomRenderer.materials = new Material[] { wallMaterial, ceilingMaterial, floorMaterial };
                room.faces[(int)ROOM_FACE_IDXS.CEILING].submeshIndex = 1;
                room.faces[(int)ROOM_FACE_IDXS.FLOOR].submeshIndex = 2;
                break;
            case MaterialMode.ALL_SEPARATE:
                // Make a list of materials
                roomRenderer.materials = new Material[] { wallMaterial, wallMaterial, wallMaterial, wallMaterial, ceilingMaterial, floorMaterial };
                // If each side has it's own material, use a mapping of face index to material index
                for (int i = 0; i < roomRenderer.materials.Length; i++)
                {
                    room.faces[(int)ROOM_FACE_IDXS.FLOOR].submeshIndex = 1;
                }
                break;
            default: // == MaterialMode.SINGLE
                roomRenderer.materials = new Material[] { wallMaterial };
                // No Mapping needed as all have default submeshIndex = 0
                break;
        }
    }


    public static void UpdateTeleportationArea(ProBuilderMesh parentMesh, bool addIfNotExists = false)
    {
        Transform tpTransform = parentMesh.transform.Find("Teleportation Area");
        bool needsUpdate = addIfNotExists;

        if (addIfNotExists || tpTransform != null)
        {
            // Find faces to be deleted (= no floors)
            List<int> inverse = new List<int>();
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
                Debug.LogError("Could not find any suitable floors in the room. Teleportation Area Creation aborted");
                return;
            }

            // Copy new Floor
            ProBuilderMesh copy = Object.Instantiate(parentMesh.gameObject, parentMesh.transform.parent).GetComponent<ProBuilderMesh>();
            UnityEditor.ProBuilder.EditorUtility.SynchronizeWithMeshFilter(copy);

            if (copy.transform.childCount > 0)
            {
                for (int i = copy.transform.childCount - 1; i > -1; i--)
                {
                    Object.DestroyImmediate(copy.transform.GetChild(i).gameObject);
                }

                // Should be done (as in DuplicateFaces but somehow throws an error:/)
                // foreach (var child in parentMesh.transform.GetComponentsInChildren<ProBuilderMesh>())
                // {
                //     UnityEditor.ProBuilder.EditorUtility.SynchronizeWithMeshFilter(child);
                // }
            }
            Undo.RegisterCreatedObjectUndo(copy.gameObject, "Update Floor Teleportation");

            foreach(var v in inverse) {Debug.Log(v);}

            copy.DeleteFaces(inverse);
            copy.ToMesh();
            copy.Refresh();
            copy.Optimize();
            copy.ClearSelection();

            // Debug.Log(copy.gameObject + " x " + mesh.gameObject.transform + " x " + teleportationArea);
            MakeMeshTeleportationArea(copy.gameObject, parentMesh.transform, tpTransform);
        }
    }

    private static void MakeMeshTeleportationArea(GameObject areaObject, Transform roomTransform, Transform prevTeleportationArea)
    {
        if (areaObject != null && areaObject.GetComponent<ProBuilderMesh>() && roomTransform != null)
        {
            // Disable mesh renderer
            areaObject.GetComponent<Renderer>().sharedMaterials = new Material[0];
            areaObject.GetComponent<MeshRenderer>().enabled = false;

            // Add collider
            areaObject.gameObject.AddComponent<MeshCollider>();

            // Add Teleportation Area
            areaObject.AddComponent<TeleportationArea>();
            // Setup Teleportation Area (Collisions and layers)
            TeleportationArea area = areaObject.GetComponent<TeleportationArea>();
            area.interactionLayers = (1 << InteractionLayerMask.NameToLayer("Teleportation"));
            area.colliders.Add(areaObject.GetComponent<MeshCollider>());

            // Add/Copy Reticle Teleportation Area
            if (prevTeleportationArea != null && prevTeleportationArea.GetComponent<TeleportationArea>() != null)
            {
                // Copy component
                area.customReticle = prevTeleportationArea.GetComponent<TeleportationArea>().customReticle;
            }
            else
            {
                // Add Reticle
                GameObject reticle = AssetDatabase.LoadAssetAtPath<GameObject>(DEFAULT_TELEPORTATION_RETICLE_PATH);
                if (reticle != null)
                {
                    area.customReticle = reticle;
                }
            }

            // Delete any previous Teleportation Area if exists under the parent
            if (prevTeleportationArea != null)
            {
                GameObject.DestroyImmediate(prevTeleportationArea.gameObject);
            }

            // Parent it to the room and move up (prevents errors with teleportation detection)
            areaObject.transform.parent = roomTransform;
            areaObject.gameObject.name = "Teleportation Area";
            areaObject.transform.position += new Vector3(0, TELEPORTATION_AREA_Y_OFFSET, 0);
        }
        else
        {
            Debug.LogError("Invalid areaObject or roomTransform");
        }
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
}

public enum MaterialMode
{
    SINGLE,
    SEPARATE_FLOOR,
    SEPARATE_FLOOR_AND_CEILING,
    ALL_SEPARATE
}