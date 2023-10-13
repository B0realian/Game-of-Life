using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;
using TMPro;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Cellularium : MonoBehaviour
{
	public GameObject cellPrefab;
	public GameObject background;
	
	public TextMeshProUGUI pauseText;
	public TextMeshProUGUI infoText;
    public TextMeshProUGUI controlsText;
    
	Cell[,] cells;
	float cellSize = 0.05f;
	
	int numberOfColumns, numberOfRows;
	static int spawnChancePercentage = 15;
	int maxSpawn = 50;
	int minSpawn = 1;
	static int frameRate = 4;
	int minFrameRate = 1;
	int maxFrameRate = 25;
	int gameUpdates = 1;
	int cellsAlive;
	float oldestCell;
	bool paused = true;
	bool textVisible = true;


    void Start()
	{
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = frameRate;

		numberOfColumns = (int)Mathf.Floor((Camera.main.orthographicSize * Camera.main.aspect * 2) / cellSize);
		numberOfRows = (int)Mathf.Floor(Camera.main.orthographicSize * 2 / cellSize);

		cells = new Cell[numberOfColumns, numberOfRows];

		for (int y = 0; y < numberOfRows; y++)
		{
			for (int x = 0; x < numberOfColumns; x++)
			{
				Vector2 newPos = new Vector2(x * cellSize - Camera.main.orthographicSize *
					Camera.main.aspect + cellSize / 2, y * cellSize - Camera.main.orthographicSize + cellSize / 2);

				var newCell = Instantiate(cellPrefab, newPos, Quaternion.identity);
				newCell.transform.localScale = Vector2.one * cellSize;
				cells[x, y] = newCell.GetComponent<Cell>();
				cells[x, y].gensDead = 20;									//Making dead cells start as completely black.

				if (Random.Range(0, 100) < spawnChancePercentage)
				{
					cells[x, y].alive = true;
				}

				cells[x, y].UpdateStatus();
			}
		}
		//Debug.Log($"Grid size: {numberOfRows} * {numberOfColumns}");
		//Debug.Log($"Number of cells: {cells.Length}");
        Pause();
    }

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			paused = !paused;
			Pause();
		}
        if (Input.GetKeyDown(KeyCode.KeypadPlus) && maxFrameRate > frameRate)
        {
            frameRate += 3;
            Application.targetFrameRate = frameRate;
			TextToggle();
        }
		if (Input.GetKeyDown(KeyCode.KeypadMinus) && minFrameRate < frameRate)
		{
			frameRate -= 3;
			Application.targetFrameRate = frameRate;
			TextToggle();
		}
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        if (Input.GetKeyUp(KeyCode.Escape)) Application.Quit();

		if (paused)
		{
			if (Input.GetKeyDown(KeyCode.Backspace))
			{
				textVisible = !textVisible;
				TextToggle();
			}
			if (Input.GetKeyDown(KeyCode.UpArrow) && maxSpawn > spawnChancePercentage)
			{
				spawnChancePercentage++;
				TextToggle();
			}
			if (Input.GetKeyDown(KeyCode.DownArrow) && minSpawn < spawnChancePercentage)
			{
				spawnChancePercentage--;
				TextToggle();
			}
		}
		else
		{
			FindLife();

			for (int y = 0; y < numberOfRows; y++)
			{
				for (int x = 0; x < numberOfColumns; x++)
				{
					cells[x, y].alive = cells[x, y].willLive;
					cells[x, y].UpdateStatus();
				}
			}
			gameUpdates++;
			textVisible = true;
		}
	}

    void FindLife()
    {
        int [] neighbours = new int[4];
        bool[] livingNeighbour = new bool[8];
        for (int y = 0; y < numberOfRows; y++)
		{
			for (int x = 0; x < numberOfColumns; x++)
			{
				if (x > 0) neighbours[0] = x - 1;                                               //Neighbour left of cell
                else neighbours[0] = numberOfColumns - 1;                                       //Wrapping to right edge of screen
                if (x < numberOfColumns - 1) neighbours[1] = x + 1;                             //Neighbour right of cell
                else neighbours[1] = 0;                                                         //Wrapping to the left edge of screen
                if (y < numberOfRows -1) neighbours[2] = y + 1;                                 //Neighbour above cell
                else neighbours[2] = 0;                                                         //Wrapping to the bottom edge of screen
                if (y > 0) neighbours[3] = y - 1;                                               //Neighbour below cell
                else neighbours[3] = numberOfRows - 1;                                          //Wrapping to the top edge of screen
                
                if (cells[neighbours[0], neighbours[2]].alive) livingNeighbour[0] = true;       //Check neighbours for life, top left
                else livingNeighbour[0] = false;
                if (cells[x, neighbours[2]].alive) livingNeighbour[1] = true;                   //Top middle
                else livingNeighbour[1] = false;
                if (cells[neighbours[1], neighbours[2]].alive) livingNeighbour[2] = true;       //Top right
                else livingNeighbour[2] = false;
                if (cells[neighbours[0], y].alive) livingNeighbour[3] = true;                   //Middle left
                else livingNeighbour[3] = false;
                if (cells[neighbours[1], y].alive) livingNeighbour[4] = true;                   //Middle right
                else livingNeighbour[4] = false;
                if (cells[neighbours[0], neighbours[3]].alive) livingNeighbour[5] = true;       //Bottom left
                else livingNeighbour[5] = false;
                if (cells[x, neighbours[3]].alive) livingNeighbour[6] = true;                   //Bottom middle
                else livingNeighbour[6] = false;
                if (cells[neighbours[1], neighbours[3]].alive) livingNeighbour[7] = true;       //Bottom right
                else livingNeighbour[7] = false;

                int totalLivingNeighbours = livingNeighbour.Count(c => c);
                if (cells[x, y].alive && totalLivingNeighbours == 2 || cells[x, y].alive && totalLivingNeighbours == 3) cells[x, y].willLive = true;
                else if (!cells[x, y].alive && totalLivingNeighbours == 3) cells[x, y].willLive = true;
                else cells[x, y].willLive = false;
			}
		}
    }

    void Pause()
    {
		oldestCell = 0;
		for (int y = 0; y < numberOfRows; y++)
		{
			for (int x = 0; x < numberOfColumns; x++)
			{
				if (cells[x, y].generation > oldestCell) oldestCell = cells[x, y].generation;
			}
		}

		cellsAlive = 0;
		foreach (var cell in cells) 
		{
			if (cell.alive) cellsAlive++;
		}

		TextToggle();
    }

    void TextToggle()
    {
        if (paused)
		{
            pauseText.enabled = textVisible;
            controlsText.enabled = textVisible;
            infoText.enabled = textVisible;
            background.gameObject.SetActive(textVisible);
            controlsText.text = $"Press Space to resume/pause\n\nToggle framerate: +/- on keypad: {frameRate}\n\nToggle spawn %: arrow up/down: " +
                $"{spawnChancePercentage}\nRequires restart to take effect\n\nHide/unhide text: Backspace\n\nRestart game: Return\n\nQuit with Esc";
            infoText.text = $"Cells are born green but soon turn blue. When 100 generations old, they turn red, then yellow at 1000. Dead cells fade to black.\n\n" +
                $"Generations in game: {gameUpdates}\nOldest cell: {oldestCell}\nNumber of living cells: {cellsAlive}";
        }
		else
		{
            pauseText.enabled = false;
            controlsText.enabled = false;
            infoText.enabled = false;
            background.gameObject.SetActive(false);
        }
    }
}
