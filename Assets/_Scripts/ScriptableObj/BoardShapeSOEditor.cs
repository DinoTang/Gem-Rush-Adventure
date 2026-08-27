using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BoardShapeSO))]
public class BoardShapeSOEditor : Editor
{
    private int width;
    private int height;

    private void OnEnable()
    {
        BoardShapeSO board = (BoardShapeSO)target;

        width = board.Width;
        height = board.Height;
    }

    public override void OnInspectorGUI()
    {
        BoardShapeSO board = (BoardShapeSO)target;

        // =====================================================
        // SIZE
        // =====================================================

        width = EditorGUILayout.IntField("Width", width);
        height = EditorGUILayout.IntField("Height", height);

        width = Mathf.Max(1, width);
        height = Mathf.Max(1, height);

        if (GUILayout.Button("Apply Size"))
        {
            Undo.RecordObject(board, "Resize Board Shape");

            board.SetSize(width, height);

            EditorUtility.SetDirty(board);
        }

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField(
            "Board Layout",
            EditorStyles.boldLabel
        );

        // =====================================================
        // GRID
        // =====================================================

        bool[] cells = board.GetValidCells();

        if (cells == null ||
            cells.Length != board.Width * board.Height)
        {
            EditorGUILayout.HelpBox(
                "Board layout chưa được khởi tạo. Hãy nhấn Apply Size.",
                MessageType.Warning
            );

            return;
        }

        for (int y = board.Height - 1; y >= 0; y--)
        {
            EditorGUILayout.BeginHorizontal();

            for (int x = 0; x < board.Width; x++)
            {
                int index = y * board.Width + x;

                GUIStyle style = new GUIStyle(GUI.skin.button)
                {
                    fontSize = 18,
                    alignment = TextAnchor.MiddleCenter
                };

                string symbol = cells[index] ? "■" : "□";

                if (GUILayout.Button(
                    symbol,
                    style,
                    GUILayout.Width(30),
                    GUILayout.Height(30)))
                {
                    Undo.RecordObject(
                        board,
                        "Change Board Cell"
                    );

                    cells[index] = !cells[index];

                    EditorUtility.SetDirty(board);
                }
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}