using UnityEngine;
using System.Collections;

public class mainGame : MonoBehaviour
{


	public bool _canTransitDiagonally = false;
//Indicate if we can switch diagonally

	public int _gridWidth;
	public int _gridHeight;

	private GameObject _firstObject;
	private GameObject _secondObject;
	public GameObject _indicator;
	//	private GameObject _currentIndicator;
	// default shape,to initialization
	public GameObject emptyObject;
	public GameObject[] _listOfShapes;
	public GameObject[,] _arrayOfShapes;


	// Use this for initialization
	void Start ()
	{
		// use empty shape intialization grid
		_arrayOfShapes = new GameObject[_gridWidth, _gridHeight];
		for (int i = 0; i <= _gridWidth - 1; i++) {
			for (int j = 0; j <= _gridHeight - 1; j++) {
				GameObject emptyObj = GameObject.Instantiate (emptyObject, new Vector3 (i, j, 0), transform.rotation) as GameObject;
				_arrayOfShapes [i, j] = emptyObj;
			}
		}

		DoEmptyDown (ref _arrayOfShapes);
	}


	void DoSwapMotion (Transform a, Transform b)
	{
		Vector3 temp = a.localPosition;
		a.localPosition = b.localPosition;
		b.localPosition = temp;
	}
	// Swap Two Tile, it swaps the position of two objects in the grid array
	void DoSwapTile (GameObject a, GameObject b, ref GameObject[,] cells)
	{
		GameObject cell = cells [(int)a.transform.position.x, (int)a.transform.position.y];

		cells [(int)a.transform.position.x, (int)a.transform.position.y] = 
			cells [(int)b.transform.position.x, (int)b.transform.position.y];

		cells [(int)b.transform.position.x, (int)b.transform.position.y] = cell;

		canMatch = true;
	}

	private ArrayList FindMatch (GameObject[,] cells)
	{
		ArrayList stack = new ArrayList ();
		for (var x = 0; x <= cells.GetUpperBound (0); x++) {
			for (var y = 0; y <= cells.GetUpperBound (1); y++) {
				var thiscell = cells [x, y];
				if (thiscell.name == "empty(Clone)")
					continue;
				int matchCount = 0;
				int y2 = cells.GetUpperBound (1);
				int y1;
				for (y1 = y + 1; y1 <= y2; y1++) {
					if (cells [x, y1].name == "empty(Clone)" ||
					    (thiscell.name != cells [x, y1].name))
						break;
					matchCount++;
				}
				if (matchCount >= 2) {
					y1 = Mathf.Min (cells.GetUpperBound (1), y1 - 1);
					for (var y3 = y; y3 <= y1; y3++) {
						if (!stack.Contains (cells [x, y3])) {
							stack.Add (cells [x, y3]);
						}
					}
				}
			}
		}

		for (var y = 0; y <= cells.GetUpperBound (1); y++) {
			for (var x = 0; x <= cells.GetUpperBound (0); x++) {
				var thiscell = cells [x, y];
				if (thiscell.name == "empty(Clone)")
					continue;


				int matchCount = 0;
				int x2 = cells.GetUpperBound (0);
				int x1;
				for (x1 = x + 1; x1 <= x2; x1++) {
					if (cells [x1, y].name == "empty(Clone)" ||
					    (thiscell.name != cells [x1, y].name))
						break;
					matchCount++;
				}
				if (matchCount >= 2) {
					x1 = Mathf.Min (cells.GetUpperBound (0), x1 - 1);
					for (var x3 = x; x3 <= x1; x3++) {
						if (!stack.Contains (cells [x3, y])) {
							stack.Add (cells [x3, y]);
						}
					}
				}
			}
		}
		return stack;
	}

	bool shouldTransit = false;
	bool canMatch = false;
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Escape))
			Application.Quit ();
		shouldTransit = false;
		ArrayList matches = new ArrayList ();

		if (Input.GetButtonDown ("Fire1")) {
//			Destroy (_currentIndicator);
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			//RaycastHit2D hit = Physics2D.Raycast (Camera.main.ScreenToWorldPoint (Input.mousePosition), Vector2.zero);
			RaycastHit hit;
//			if(Physics.Raycast(ray,out hit,100)){
//				Debug.Log ("Hit gameobject" + hit.collider.gameObject);
//			}
//			if (hit.transform != null) {
//				if (hit.transform.gameObject.name == "empty(Clone)") {
			if (Physics.Raycast (ray, out hit, 100)) {
				if (hit.collider.gameObject.name == "empty(Clone)") {
					DoEmptyDown (ref _arrayOfShapes);
					return;
				}

				// if hit a gem
				bool foundGem = false;
				Vector2 gemPosition = new Vector2 (-1, -1);
				for (int x = 0; x <= _arrayOfShapes.GetUpperBound (0); x++) {
					for (int y = 0; y <= _arrayOfShapes.GetUpperBound (1); y++) {
						if (_arrayOfShapes [x, y].GetInstanceID () == hit.transform.gameObject.GetInstanceID ()) {
							foundGem = true;
							gemPosition = new Vector2 (x, y);
						}
					}
				}
				if (!foundGem)
					return;
				if (_firstObject == null) {
					_firstObject = hit.transform.gameObject;
				} else {
					_secondObject = hit.transform.gameObject;
					shouldTransit = true;
				}
//				_currentIndicator = GameObject.Instantiate(_indicator,
//					new Vector3(hit.transform.gameObject.transform.position.x,
//					hit.transform.gameObject.transform.position.y,-1), 
//					transform.rotation) as GameObject ;
			
			
				if (shouldTransit) {
					var distance = _firstObject.transform.position - _secondObject.transform.position;
					if (Mathf.Abs (distance.x) <= 1 && Mathf.Abs (distance.y) <= 1) {
						if (!_canTransitDiagonally) {
							if (distance.x != 0 && distance.y != 0) {
//								Destroy (_currentIndicator);
								_firstObject = null;
								_secondObject = null;
								return;
							}
						}
						DoSwapMotion(_firstObject.transform, _secondObject.transform);
						//
						DoSwapTile (_firstObject, _secondObject, ref _arrayOfShapes);
					} else {
						_firstObject = null;
						_secondObject = null;
					}
//					Destroy (_currentIndicator);
				}
			}
		}
		if (canMatch) {
			matches.AddRange (FindMatch (_arrayOfShapes));
			foreach (GameObject go in matches) {
				_arrayOfShapes [(int)go.transform.position.x, (int)go.transform.position.y] =
					GameObject.Instantiate (emptyObject, new Vector3 ((int)go.transform.position.x,
					(int)go.transform.position.y, 0), transform.rotation) as GameObject;

				Destroy (go);
			}
			_firstObject = null;
			_secondObject = null;
			canMatch = false;
			DoEmptyDown (ref _arrayOfShapes);
		} else if (_firstObject != null && _secondObject != null) {
//			DoSwapTile (_firstObject, _secondObject, ref _arrayOfShapes);
//			_firstObject = null;
//			_secondObject = null;

		}
	}

	private void DoEmptyDown (ref GameObject[,] cells)
	{
		for (int x = 0; x <= cells.GetUpperBound (0); x++) {
			for (int y = 0; y <= cells.GetUpperBound (1); y++) {
			
				var thisCell = cells [x, y];
				if (thisCell.name == "empty(Clone)") {
					for (int y2 = y; y2 <= cells.GetUpperBound (1); y2++) {
						if (cells [x, y2].name != "empty(Clone)") {
							cells [x, y] = cells [x, y2];
							cells [x, y2] = thisCell;
							break;
						}
					}
				}
			}
		}

		for (int x = 0; x <= cells.GetUpperBound (0); x++) {
			for (int y = 0; y <= cells.GetUpperBound (1); y++) {
				if (cells [x, y].name == "empty(Clone)") { 
					Destroy (cells [x, y]);
					cells [x, y] = GameObject.Instantiate (_listOfShapes [Random.Range (0, _listOfShapes.Length)] 
						as GameObject, new Vector3 (x, y, 0), transform.rotation) as GameObject;
				}
			}
		}

		for (int x = 0; x <= cells.GetUpperBound (0); x++) {
			for (int y = 0; y <= cells.GetUpperBound (1); y++) {
				cells [x, y].transform.position = new Vector3 (x, y, 0);
			}
		}
	}

}
