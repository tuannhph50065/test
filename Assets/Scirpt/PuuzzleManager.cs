using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum CellType { Empty, Piece, Block }

public class GridManager : MonoBehaviour {
    public int rows = 4, cols = 4;
    public GameObject[] piecePrefabs;
    public GameObject blockPrefab;
    public GameObject winPanel;
    public GameObject losePanel;
    public TextMeshProUGUI timeLeftText;
    public int bl;

    private GridCell[,] grid;
    private float timeLeft = 45f;
    private bool isGameOver;

    void Start() {
        if (piecePrefabs.Length != 4) {
            Debug.LogError("Cần đúng 4 prefab!");
            return;
        }
        grid = new GridCell[rows, cols];
        InitializeGame();
    }

    void InitializeGame() {
        timeLeft = 45f;
        // Làm sạch lưới
        for (int r = 0; r < rows; r++)
            for (int c = 0; c < cols; c++)
                grid[r, c] = new GridCell(CellType.Empty);

        // Tạo danh sách vị trí khả dụng
        var positions = new List<(int r, int c)>();
        for (int r = 0; r < rows; r++)
            for (int c = 0; c < cols; c++)
                positions.Add((r, c));

        // Spawn 2 block
        for (int i = 0; i < bl; i++)
            if (positions.Count > 0) {
                int idx = Random.Range(0, positions.Count);
                PlaceBlock(positions[idx].r, positions[idx].c);
                positions.RemoveAt(idx);
            }

        // Spawn 4 mảnh ghép
        for (int i = 0; i < 5; i++)
            if (positions.Count > 0) {
                int idx = Random.Range(0, positions.Count);
                PlacePiece(positions[idx].r, positions[idx].c, piecePrefabs[i], i);
                positions.RemoveAt(idx);
            }

        
        if (winPanel) winPanel.SetActive(false);
        isGameOver = false;
        timeLeft = 45f;
        UpdateTime();
    }

    void Update() {
        if (isGameOver) return;
        // Di chuyển mảnh
        if (Input.GetKeyDown(KeyCode.LeftArrow)) MoveAllPieces(0, -1);
        if (Input.GetKeyDown(KeyCode.RightArrow)) MoveAllPieces(0, 1);
        if (Input.GetKeyDown(KeyCode.UpArrow)) MoveAllPieces(-1, 0);
        if (Input.GetKeyDown(KeyCode.DownArrow)) MoveAllPieces(1, 0);
      
        // Cập nhật thời gian
        timeLeft -= Time.deltaTime;
        UpdateTime();
        if (timeLeft <= 0) {
            isGameOver = true;
            Debug.Log("Hết thời gian! Bạn thua.");
            losePanel.SetActive(true);
        }
    }

   

    void UpdateTime() {
        if (timeLeftText) {
            int minutes = (int)(timeLeft / 60);
            int seconds = (int)(timeLeft % 60);
            timeLeftText.text = $"{minutes:00}:{seconds:00}";
        }
    }

    void PlaceBlock(int r, int c) {
        grid[r, c] = new GridCell(CellType.Block);
        Instantiate(blockPrefab, GetWorldPos(r, c), Quaternion.identity, transform);
    }

    void PlacePiece(int r, int c, GameObject prefab, int pieceId) {
        GameObject obj = Instantiate(prefab, GetWorldPos(r, c), Quaternion.identity, transform);
        PuzzlePiece piece = obj.GetComponent<PuzzlePiece>();
        piece.pieceId = pieceId; // Gán pieceId
        piece.SetPosition(r, c);
        grid[r, c] = new GridCell(CellType.Piece, piece);
    }

    void MoveAllPieces(int dr, int dc) {
        var moves = new List<(PuzzlePiece piece, int newR, int newC)>();

        // Duyệt lưới theo hướng ngược
        for (int r = dr > 0 ? rows - 1 : 0; dr > 0 ? r >= 0 : r < rows; r += dr > 0 ? -1 : 1)
            for (int c = dc > 0 ? cols - 1 : 0; dc > 0 ? c >= 0 : c < cols; c += dc > 0 ? -1 : 1)
                if (grid[r, c].type == CellType.Piece) {
                    int newR = r + dr, newC = c + dc;
                    if (IsInBounds(newR, newC) && grid[newR, newC].type == CellType.Empty) {
                        moves.Add((grid[r, c].piece, newR, newC));
                        grid[newR, newC] = new GridCell(CellType.Piece, grid[r, c].piece);
                        grid[r, c] = new GridCell(CellType.Empty);
                    }
                }

        foreach (var move in moves)
            move.piece.SetPosition(move.newR, move.newC);

        CheckWinCondition();
    }

    void CheckWinCondition() {
        for (int r = 0; r < rows - 1; r++)
            for (int c = 0; c < cols - 1; c++)
                if (grid[r, c].type == CellType.Piece &&
                    grid[r, c + 1].type == CellType.Piece &&
                    grid[r + 1, c].type == CellType.Piece &&
                    grid[r + 1, c + 1].type == CellType.Piece &&
                    grid[r, c].piece.pieceId == 0 &&
                    grid[r, c + 1].piece.pieceId == 1 &&
                    grid[r + 1, c].piece.pieceId == 2 &&
                    grid[r + 1, c + 1].piece.pieceId == 3) {
                    isGameOver = true;
                    ShowWinPanel();
                    return;
                }
    }

    void ShowWinPanel() {
        if (winPanel) winPanel.SetActive(true);
        Debug.Log("Chúc mừng! Bạn đã xếp đúng hình!");
    }

    public void Replay() {
        
        foreach (Transform child in transform)
            Destroy(child.gameObject);
        timeLeft = 45f;
         isGameOver = false;
        if (winPanel) winPanel.SetActive(false);
        if (losePanel) losePanel.SetActive(false);
       UpdateTime();
        InitializeGame();
    }

    bool IsInBounds(int r, int c) => r >= 0 && r < rows && c >= 0 && c < cols;

    Vector3 GetWorldPos(int r, int c) => new Vector3(c * 1.1f, -r * 1.1f, 0);
}

public class GridCell {
    public CellType type;
    public PuzzlePiece piece;

    public GridCell(CellType t, PuzzlePiece p = null) {
        type = t;
        piece = p;
    }
}