using UnityEngine;

public class PuzzlePiece : MonoBehaviour {
    public int row, col;
    public int pieceId; // Lưu chỉ số của mảnh (0: top-left, 1: top-right, 2: bottom-left, 3: bottom-right)

    public void SetPosition(int r, int c) {
        row = r;
        col = c;
        transform.localPosition = new Vector3(c * 1.1f, -r * 1.1f, 0); // spacing
    }
}