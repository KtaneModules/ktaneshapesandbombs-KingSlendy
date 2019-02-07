using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using KMBombInfoHelper;
using UnityEngine;

using Random = UnityEngine.Random;
using Math = UnityEngine.Mathf;

public class ShapesBombs : MonoBehaviour {
	public KMAudio BombAudio;
	public KMBombInfo BombInfo;
	public KMBombModule BombModule;
	public KMSelectable[] ModuleButtons;
	public KMSelectable ModuleSelect, NumScreen, ResetButton, EmptyButton, SubmitButton;
	public KMRuleSeedable RuleSeedable;
	public KMColorblindMode ColorblindMode;
	public Light LightTemp;
	public GameObject ArrowScreen;
	public Texture2D[] ArrowTex = new Texture2D[0];
	public GameObject ColScreen;

	Color[] randomColors = { Color.yellow, Color.green, Color.cyan, Color.blue, Color.magenta, Color.white };
	Color buttonOFF = new Color32(50, 50, 50, 255);
	Color buttonsColor = Color.black;
	readonly Light[] buttonLight = new Light[40];
	readonly float lightIntens = 20.0f;
	MonoRandom rnd;

	List<int> pickedValues = new List<int>();
	int[] chooseArrows = new int[15];
	int nowArrow;
	Coroutine arrowCoroutine, nowCoroutine, subCoroutine;

	string[] intShape;
	string myShape = "";
	string modLetter = "";
	string shapeSolution = "";

	delegate int checkCount(string x, char y);

	checkCount getCount;
	bool countUnlit, countHalf;

	int solvedMods;
	int prevSolvedMods = -1;
	bool moduleSolved;

	static int moduleIdCounter = 1;
	int moduleId;

	void Start() {
		moduleId = moduleIdCounter++;
		string[] choseColor = { "Yellow", "Green", "Cyan", "Blue", "Purple", "White" };
		int chooseRndColor = Random.Range(0, randomColors.Length);
		buttonsColor = randomColors[chooseRndColor];
		ColScreen.transform.GetChild(0).GetComponent<TextMesh>().text = choseColor[chooseRndColor];
		char[] moduleLetters = { 'A', 'B', 'D', 'E', 'G', 'I', 'K', 'L', 'N', 'O', 'P', 'S', 'T', 'X', 'Y' };
		string[] intLetter = {
			"XOOOXOXXXOOXXXOOOOOOOXXXOOXXXOOXXXOOXXXO",
			"OOOOXOXXXOOXXXOOOOOXOXXXOOXXXOOXXXOOOOOX",
			"OOOOXOXXXOOXXXOOXXXOOXXXOOXXXOOXXXOOOOOX",
			"OOOOOOXXXXOXXXXOOOOXOXXXXOXXXXOXXXXOOOOO",
			"XOOOXOXXXOOXXXXOXXXXOXOOOOXXXOOXXXOXOOOX",
			"OOOOOXXOXXXXOXXXXOXXXXOXXXXOXXXXOXXOOOOO",
			"OXXXOOXXOXOXOXXOOXXXOXOXXOXXOXOXXOXOXXXO",
			"OXXXXOXXXXOXXXXOXXXXOXXXXOXXXXOXXXXOOOOO",
			"OXXXOOOXXOOXOXOOXOXOOXXOOOXXXOOXXXOOXXXO",
			"XOOOXOXXXOOXXXOOXXXOOXXXOOXXXOOXXXOXOOOX",
			"OOOOXOXXXOOXXXOOOOOXOXXXXOXXXXOXXXXOXXXX",
			"XOOOXOXXXOOXXXXXOOOXXXXXOXXXXOOXXXOXOOOX",
			"OOOOOXXOXXXXOXXXXOXXXXOXXXXOXXXXOXXXXOXX",
			"OXXXOXOXOXXOXOXXXOXXXXOXXXOXOXXOXOXOXXXO",
			"OXXXOXOXOXXOXOXXXOXXXXOXXXXOXXXXOXXXXOXX"
		};

		string[] chooseShapes = {
			"XXXXXXXXXXXXXXXOXXXXOXXXOOXOXOOXOXOOOOOO",
			"XOOOXOXXXOXXXXOXXXOXXXOXXXXOXXXXXXXXXOXX",
			"XXOXXXOXOXOXXXOXOOOXOXXXOXOXOXXXOXXOOXOO",
			"XOOOXXXXXOXXXXOXOOOXXXXXOXXXXOXXXXOXOOOX",
			"OXOXOXOXOXOXOXOXOXOXOXOXOXOXOXOXOXOXOXOX",
			"XXOXXXXOXXXXOXXXOOOXOXOXOOOOOOXOOOXXXOXX",
			"XOOOXOXXXOOXXXOXOXOXOOOOOXOXOXOOOOOXOXOX",
			"XXXXXXXOOOXXOOOXXOOOXXXOXOXOXXXOOXXXOOXX",
			"XXOXOOXXOXXXXXXXOOOXXXXXXXOXOOXXXXXOOOOO",
			"OOXOOOXXXOOXOXOOOXOOOOXOOOXOXOOXXXOOOXOO",
			"XXXXXXXOXXXXOXXXXOXXXOOOXXXOXXOXOXOOOOOO",
			"OOOOOOXXXOOXXXOOXOXOOXOXOOXXXOOXXXOOOOOO",
			"OOOOOXXXOXXXOXXXXXOXOOOOOOXXXXOXXXXOXOOO",
			"XXXXOXXXOXXXOXXOOXOOOXOXOOXXXOOXXXOOOOOO",
			"XOXOXOXOOXXXXXXOXOXOOXOXOXXXXXOXOOXXOXOX",
			"XOXOXOXXXOOXXXOXOOOXOXXXOOOXOOOXXXOXOOOX",
			"XOXOXXOXOXXXOXXXOXOXOXOXOOXXXOXOXOXXXOXX",
			"OXOXOOXOXOOXOXOOXOXOOXOXOOXOXOOXOXOOXOXO",
			"OOOOOXXXXXOOOOOXXXXXOOOOOXXXXXOOOOOXXXXX",
			"XXOXXXXOXXXXOXXXXOXXXXOXXXXOXXXXXXXXXOXX",
			"XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX",
			"XXOXXXXXXXXXOXXXXOXXXXOXXXXOXXXXOXXXXOXX",
			"OXOXOOOOOOOXOXOOOOOOOXOXOOOOOOOXOXOOOOOO",
			"OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO",
			"XOXOXOXOXOXOXOXXXXXXOXOXOOOOOOXOXOXXXOXX",
			"OOOOOOXXXOOXXXOOXXXOOXXXOOXXXOOXXXOOOOOO",
			"XXOXXXXXXXXXOXXXXOXXXOXXXOXXXXOXXXOXOOOX",
			"XOXOXXXOXXXOXOXXXXXXXXXXXXOXOXXXOXXXOXOX",
			"OOOOOXXOXXXXOXXOOOOOXXOXXOOOOOOXXXOOOOOO",
			"OOOOXOXXXOOXXXOOOOOXOXXXXOXXXXOXXXXOXXXX",
			"OXXXOOOXOOOXOXOOXXXOOXXXOOXXXOOXXXOOXXXO",
			"XXXXXXOXOXXOXOXXXXXXXXXXXXOOOXOXXXOXXXXX",
			"XXOXXXOXOXOXXXOXXOXXXXOXXOXXXOXOXOXXXOXX",
			"XXOXXXOXOXOXXXOOOOOOOOOOOOXXXOXOXOXXXOXX",
			"OOOOXOXXXOOXXXOOOOOXOXXXOOXXXOOXXXOOOOOX",
			"XXOXXXOOXXOXOXXXXOXXXXOXXXXOXXXXOXXOOOOO",
			"OXXXOOXXXOOXXXOOXXXOOXXXOOXXXOOXXXOXOOOX",
			"XXXXXXXXXXXOXOXXOXOXXXXXXOOOOOXOOOXXXXXX",
			"OXXXOOXXOXOXOXXOOXXXOXOXXOXXOXOXXOXOXXXO",
			"XXOXXXXOXXXXOXXXXOXXXXOXXOXOXOXOOOXXXOXX",
			"OOOOXOXXXOOXXXOOOOOXOOXXXOXOXXOXXOXOXXXO",
			"OOOOOXXOXXXXOXXXXOXXXXOXXXXOXXXXOXXXXOXX",
			"XXXXXXOXOXXOXOXXXXXXXXXXXOXXXOXOOOXXXXXX",
			"XOOOXOXXXOOXXXOOXXXOOXXXOOXOXOXOOOXXXXOO",
			"XOOOXOXXXOOXXXXXOOOXXXXXOXXXXOOXXXOXOOOX",
			"XXXOXXXOXXXOXXXXXOXXXXXOXXXOXXXOXXXXXOXX",
			"XOOOXOXOXOOOOOOOXOXOOXOXOOOXOOOXOXOXOOOX",
			"XOOOXOXXXOOXXXOOOOOOOXXXOOXXXOOXXXOOXXXO",
			"OOOOOXXXXOXXXOXXXXOXXXOXXXXOXXXOXXXXOXXX",
			"OXXXOOXXXOOXXXOOOOOOXXXXOXXXXOXXXXOXXXXO",
			"XXXXXXXXXXXXXXXXXXXOXXXOOXXOOOXOOOOOOOOO",
			"OOOOOOXXXXOXXXXOOOOXOXXXXOXXXXOXXXXOXXXX",
			"XOOOXOXXXOOXXXXOXXXXOXOOOOXXXOOXXXOXOOOX",
			"OOOOOXXXXXXOOOXOXXXOXOOOXXOOOXOXXXOXOOOX",
			"XOOOXOXXXXOXXXXOXXXXOOOOXOXXXOOXXXOXOOOX",
			"OOXOOOXOXOXOXOXOXXXOOXXXOXOXOXOXOXOOOXOO",
			"XOOOXOXXXOOOOOOXXXXXOXXXOXOOOXOXXXOXOOOX",
			"OOOOOXXXXOXXXOXXXOXXXXOXXXOXXXOXXXXOOOOO",
			"OXXXOXOXOXXOXOXXXOXXXXOXXXXOXXXXOXXXXOXX",
			"XOOOXOXXOOOXOXOOXOXOOXOXOOXOXOOOXXOXOOOX",
			"XOOOXOXXXOOXXXOOXOXOOXOXOOXXXOOXXXOXOOOX",
			"XXOXXXOXOXXOXOXOXXXOOXXXOXOXOXXOXOXXXOXX",
			"XOOOXOXXXOOXXXOXOOOOXXXXOXXXXOXXXXOXXXXO",
			"OOOOOXOXOXXOXOXXOXOXXOXOXXOXOXXOXOXOOOOO",
			"OOOOOXOXOXOOOOOXOXOXOOOOOXOXOXOOOOOXOXOX",
			"XOOOXOXXXOOXXXXOXXXXOXXXXOXXXXOXXXOXOOOX",
			"OXOXOOXOXOXOXOXXXXXXXXXXXXOXOXOXOXOOXOXO",
			"XXXXOXXXOXXXOXXOOOXXXXOXXXXOXXXXOXXXXOXX",
			"XXXXXOOOOOOXXXOOOXOOOXXXOOOOOOOXXXOOOOOO",
			"OOOOOOXXXXOXXXXOOOOXOXXXXOXXXXOXXXXOOOOO",
			"OXOXOOXOXOXOXOXXOXOXOXOXOOXOXOXOXOXXOXOX",
			"OOXOOOXXXOOXXXOOXXXOOXXXOOXXXOOXXXOOOXOO",
			"XXXXXXXOXXXXOXXXOXOXXOXOXOXXXOOXXXOOOOOO",
			"OXXXOOXXXOOXXXOOOOOOOXXXOOXXXOOXXXOOXXXO",
			"OOOOOOOXOOOXOXOOXXXOOXXXOOXOXOOOXOOOOOOO",
			"OOOOOOXXXXOXXXXOOOOXXXXXOXXXXOXXXXOOOOOX",
			"XXOXXXOOOXOXOXOXXOXXXXOXXOXOXOXOOOXXXOXX",
			"XXOXXOOOOOOXOXOOXOXOOOOOOXXOXXXXOXXXXOXX",
			"XXXXXXXOXXXOXOXXXOXXXXOXXXOXOXXXOXXXXXXX",
			"OXXXOOOXXOOXOXOOXOXOOXXOOOXXXOOXXXOOXXXO",
			"OXXXOOXXXOOXXXOOXXXOOXXXOOXXXOXOXOXXXOXX",
			"OOOOXOXXXOOXXXOOXXXOOXXXOOXXXOOXXXOOOOOX",
			"XOXOXOXOXOOXOXOOXOXOOXOXOOXOXOOXOXOXOXOX",
			"XOOOXXOXOXXOOOXXOXOXXOXOXXOXOXXOOOXXXOXX",
			"OXXXOXOXOXXOXOXXXOXXXXOXXXOXOXXOXOXOXXXO",
			"XXOXXXOOOXOOOOOXXOXXXXOXXXOOOXOOOOOOXXXO",
			"OXXXXOXXXXOXXXXOXXXXOXXXXOXXXXOXXXXOOOOO",
			"XOOOXOOXOOOXOXOOOXOOOXOXOOOXOOOXOXOOOOOO",
			"XOOOXOXXXOXXXXOXXXOXXXOXXXOXXXOXXXXOOOOO",
			"XOOOXOXXXOOXXXOXOOOXOXXXOOXXXOOXXXOXOOOX",
			"OOOOOOXOXOOOXOOOXOXOOXOXOOOXOOOXOXOOOOOO",
			"XOOOXOXXXOOXXXOOXXXOOXXXOOXXXOOXXXOXOOOX",
			"OXXXOOXXXOOXXXOOXXXOOXOXOOXOXOOXOXOXOXOX",
			"OXOXOXOXOXXOXOXXXOXXXXOXXXOXOXXOXOXOXOXO",
			"OOOOOXXOXXXXOXXXXOXXXXOXXXXOXXXXOXXOOOOO",
			"XXXXXXXOXXXOOOXOXXXOOOOOOOXXXOOXXXOOOOOO",
			"XXOXXXOXOXXOXOXXXOXXOXXXOXOXOXXXOXXXXOXX",
			"XXXOXXXXOXOOOOOXXOOXXOXOXOXXOXXXXOXXXXOX",
			"OOOOXXXXOXXXXOXXXXOXXXXOXXXXOXOXXOXXOOXX",
			"XXOXXXOOOXOXOXOXXOXXXXOXXXXOXXXXOXXXXOXX"
		};
			
		var selectLetter = Random.Range(0, intLetter.Length);
		var onSqrCount = intLetter[selectLetter].Count(x => x == 'O');
		var offSqrCount = intLetter[selectLetter].Count(x => x == 'X');
		rnd = RuleSeedable.GetRNG();
		Debug.LogFormat(@"[Shapes Bombs #{0}] Using rule seed: {1}", moduleId, rnd.Seed);
		char[] intFullLetter = { 'A', 'B', 'D', 'E', 'G', 'I', 'K', 'L', 'N', 'O', 'P', 'S', 'T', 'X', 'Y' };
		int[] chooseSequence = {
			Array.FindIndex(intFullLetter, x => x.Equals(moduleLetters[selectLetter])),
			BombInfo.GetModuleNames().Count,
			onSqrCount,
			offSqrCount,
			0
		};

		int[] sequenceTables = new int[5];
		string[] letterTables = Enumerable.Repeat("", 5).ToArray();
		intShape = chooseShapes.Take(20).ToArray();

		if (rnd.Seed == 1) {
			chooseSequence.CopyTo(sequenceTables, 0);
			letterTables = new[] {
				"KEPTALINGDOBYXS",
				"KINGSLEDYOPXTAB",
				"ONSLYPKIDTXBAEG",
				"ABDEGIKLNOPSTXY",
				"YXTSPONLKIGEDBA"
			};
		} else if (rnd.Seed == 333) {
			for (int i = 0; i < sequenceTables.Length; i++) {
				sequenceTables[i] = 3;
			}

			for (int i = 0; i < letterTables.Length; i++) {
				for (int j = 0; j < 15; j++) {
					letterTables[i] += "K";
				}
			}

			for (int i = 0; i < intShape.Length; i++) {
				intShape[i] = chooseShapes[3];
			}
		} else {
			for (int i = 0; i < intFullLetter.Length; i++) {
				intFullLetter[i] = moduleLetters[ChooseUnique(15)];
			}

			chooseSequence[0] = Array.FindIndex(intFullLetter, x => x.Equals(moduleLetters[selectLetter]));
			chooseSequence[4] = rnd.Next(15);
			pickedValues.Clear();

			for (int i = 0; i < sequenceTables.Length; i++) {
				sequenceTables[i] = chooseSequence[ChooseUnique(5)];
			}

			for (int i = 0; i < letterTables.Length; i++) {
				pickedValues.Clear();

				for (int j = 0; j < 15; j++) {
					letterTables[i] += moduleLetters[ChooseUnique(15)].ToString();
				}
			}

			pickedValues.Clear();

			for (int i = 0; i < intShape.Length; i++) {
				intShape[i] = chooseShapes[ChooseUnique(100)];
			}
		}
			
		myShape = intLetter[selectLetter];
		modLetter = myShape;
		int[] arrowDirVal = { 0, 1, -5, -1, 5, -4, 6, -6, 4 };
		var logArrows = "";

		for (int i = 0; i < chooseArrows.Length; i++) {
			chooseArrows[i] = Random.Range(0, arrowDirVal.Length);
			logArrows += new[] { "Center", "Right", "Up", "Left", "Down", "Right-Up", "Right-Down", "Up-Left", "Left-Down" }[chooseArrows[i]] + " (" + i + ") ";
		}

		ColScreen.SetActive(ColorblindMode.ColorblindModeActive);
		Debug.LogFormat(@"[Shapes Bombs #{0}] Initial letter is: {1}", moduleId, moduleLetters[selectLetter]);
		Debug.LogFormat(@"[Shapes Bombs #{0}] Color for the squares is: {1}", moduleId, choseColor[chooseRndColor]);
		Debug.LogFormat(@"[Shapes Bombs #{0}] Arrow sequence is: {1}", moduleId, logArrows);

		if (!buttonsColor.Equals(Color.white)) {
			var selectTable = letterTables[chooseRndColor];
			var intStartSequence = sequenceTables[chooseRndColor];
			var tableStartSequence = intStartSequence % 15;
			var nowLetter = Array.FindIndex(selectTable.ToCharArray(), x => x.Equals(moduleLetters[selectLetter]));

			for (int i = 0; i < chooseArrows.Length; i++) {
				if (i >= tableStartSequence) {
					nowLetter = GetWrapedTable(nowLetter, arrowDirVal[chooseArrows[i]], selectTable.Length, 3);
				}
			}

			var endSequence = Array.FindIndex(intFullLetter, x => x.Equals(selectTable[nowLetter]));
			Debug.LogFormat(@"[Shapes Bombs #{0}] Initial number of table is: {1}", moduleId, intStartSequence);
			Debug.LogFormat(@"[Shapes Bombs #{0}] Starting table arrow sequence at: {1}", moduleId, tableStartSequence);
			Debug.LogFormat(@"[Shapes Bombs #{0}] Letter table is:", moduleId);
			InputLogShape(selectTable);
			Debug.LogFormat(@"[Shapes Bombs #{0}] Ended table at letter: {1}", moduleId, selectTable[nowLetter]);
			var nowSquare = Math.Clamp(intStartSequence, 0, ModuleButtons.Length - 1);
			Debug.LogFormat(@"[Shapes Bombs #{0}] Initial square location is: {1}", moduleId, nowSquare + 1);
			Debug.LogFormat(@"[Shapes Bombs #{0}] Module starting arrow sequence at: {1}", moduleId, endSequence);

			for (int i = endSequence; i < chooseArrows.Length; i++) {
				nowSquare = GetWrapedTable(nowSquare, arrowDirVal[chooseArrows[i]] * ((modLetter[nowSquare].Equals('O')) ? 1 : -1), ModuleButtons.Length, 5);
				var addChar = (modLetter[nowSquare].Equals('O')) ? "X" : "O";
				modLetter = modLetter.Remove(nowSquare, 1);
				modLetter = modLetter.Insert(nowSquare, addChar);
			}
		}

		arrowCoroutine = StartCoroutine(SetArrowScreen());
		getCount = ((x, y) => x.Count(z => z == y));
		countUnlit = (int.Parse(BombInfo.GetSerialNumber()[5].ToString())) % 2 == 1;
		countHalf = ((Array.FindIndex(intFullLetter, x => x.Equals(moduleLetters[selectLetter]))) % 2 == 0);
		UpdateSolution();
		var nowHalf = new String(((countHalf)) ? modLetter.Take(20).ToArray() : modLetter.Skip(20).ToArray());
		var nowCount = getCount(nowHalf, (countUnlit) ? 'X' : 'O');
		Debug.LogFormat(@"[Shapes Bombs #{0}] Counting {1} squares", moduleId, (countUnlit) ? "unlit" : "lit");
		Debug.LogFormat(@"[Shapes Bombs #{0}] The final {1} square count in the {2}-half is: {3}", moduleId, (countUnlit) ? "unlit" : "lit", (countHalf) ? "upper" : "bottom", nowCount);
		Debug.LogFormat(@"[Shapes Bombs #{0}] Even solved modules final shape is:", moduleId);
		InputLogShape(intShape[Math.Clamp(nowCount, 5, 14) - 5]);
		Debug.LogFormat(@"[Shapes Bombs #{0}] Odd solved modules final shape is:", moduleId);
		InputLogShape(intShape[(Math.Clamp(nowCount, 5, 14) + 10) - 5]);
		var lightScalar = transform.lossyScale.x;

		for (int i = 0; i < ModuleButtons.Length; i++) {
			ModuleButtons[i].transform.GetComponent<Renderer>().material.color = buttonOFF;
			var nowLight = buttonLight[i] = ((i == 0) ? LightTemp : Instantiate(LightTemp));
			nowLight.name = "ButtonLight" + i;
			nowLight.transform.parent = ModuleButtons[i].transform;
			nowLight.transform.localPosition = new Vector3(0.0f, 0.8f, 0.0f);
			nowLight.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
			nowLight.range = 0.01f * lightScalar;
			nowLight.intensity = 0.0f;

			if (intLetter[selectLetter][i].Equals('O')) {
				AssignButtonColor(i, false);
			}

			int j = i;

			ModuleButtons[i].OnInteract += delegate() {
				OnButtonPress(j, true);

				return false;
			};
		}

		NumScreen.OnInteract += delegate() {
			BombAudio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);

			if (arrowCoroutine != null) {
				StopCoroutine(arrowCoroutine);
				arrowCoroutine = StartCoroutine(SetArrowScreen());
			}

			return false;
		};

		KMSelectable[] sideButtons = { ResetButton, EmptyButton, SubmitButton };

		for (int i = 0; i < sideButtons.Length; i++) {
			int j = i;

			sideButtons[i].transform.GetComponent<Renderer>().material.color = new Color32(239, 228, 176, 255);
			sideButtons[i].OnInteract += delegate() {
				BombAudio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
				ModuleSelect.AddInteractionPunch(0.75f);

				switch (j) {
					case 0:
					case 1:
						if (nowCoroutine == null && subCoroutine == null) {
							nowCoroutine = StartCoroutine(((j == 0) ? ResetLetter(intLetter[selectLetter]) : EmptyLights()));
						}
						break;

					default:
						if (!moduleSolved && nowCoroutine == null && subCoroutine == null) {
							if (myShape.Equals(shapeSolution)) {
								StopAllCoroutines();
								Debug.LogFormat(@"[Shapes Bombs #{0}] Module solved!", moduleId);
								BombAudio.PlaySoundAtTransform("CorrectShape", transform);
								moduleSolved = true;
								BombModule.HandlePass();
							} else {
								var solvedModsEven = (solvedMods % 2 == 0);
								Debug.LogFormat(@"[Shapes Bombs #{0}] Current shape doesn't equal to the {1} final shape.", moduleId, solvedModsEven ? "even" : "odd");
								Debug.LogFormat(@"[Shapes Bombs #{0}] Current shape:", moduleId);
								InputLogShape(myShape);
								BombModule.HandleStrike();
							}

							subCoroutine = StartCoroutine(CheckSubmit());
						}
						break;
				}

				return false;
			};
		}
	}

	void Update() {
		if (!moduleSolved) {
			UpdateSolution();
		}
	}

	int ChooseUnique(int maxVal) {
		var nowPicked = 0;

		do {
			nowPicked = rnd.Next(maxVal);
		} while (pickedValues.Contains(nowPicked));

		pickedValues.Add(nowPicked);

		return nowPicked;
	}

	int GetWrapedTable(int setSquare, int tempDir, int tableSize, int tableRows) {
		var tempSquare = (Math.Abs(tempDir) == 4) ? -1 * (int)Math.Sign(tempDir) : (Math.Abs(tempDir) % 5) * (int)Math.Sign(tempDir);

		if ((setSquare % tableRows) + tempSquare == -1 || (setSquare % tableRows) + tempSquare == tableRows) {
			tempSquare = (tableRows - 1) * ((((setSquare % tableRows) + tempSquare) == tableRows) ? -1 : 1);
		}

		setSquare += tempSquare;

		if (Math.Abs(tempDir) != 1 && Math.Abs(tempDir) != 0) {
			tempSquare = tableRows * (int)Math.Sign(tempDir);
			setSquare += tempSquare;
		}

		if (setSquare < 0) {
			setSquare = tableSize - Math.Abs(setSquare);
		}

		setSquare %= tableSize;

		return setSquare;
	}

	void OnButtonPress(int buttonPressed, bool assignChar) {
		if (nowCoroutine != null || subCoroutine != null) {
			return;
		}

		BombAudio.PlaySoundAtTransform("PressButton" + (Random.Range(1, 4).ToString()), transform);
		AssignButtonColor(buttonPressed, assignChar);
	}

	void AssignButtonColor(int passedButton, bool passedAssign) {
		var tempColor = ModuleButtons[passedButton].transform.GetComponent<Renderer>().material.color;
		ModuleButtons[passedButton].transform.GetComponent<Renderer>().material.color = (tempColor.Equals(buttonOFF)) ? buttonsColor : buttonOFF;
		buttonLight[passedButton].intensity = (tempColor.Equals(buttonOFF)) ? lightIntens : 0.0f;

		if (passedAssign) {
			var addChar = (myShape[passedButton].Equals('O')) ? "X" : "O";
			myShape = myShape.Remove(passedButton, 1);
			myShape = myShape.Insert(passedButton, addChar);
		}
	}

	IEnumerator SetArrowScreen() {
		while (!moduleSolved) {
			SetArrows();

			yield return new WaitForSeconds(3.0f);
		}
	}

	IEnumerator ResetLetter(string resetLetter) {
		if (moduleSolved || myShape.Equals(resetLetter)) {
			yield break;
		}

		var tempShape = myShape;
		myShape = resetLetter;

		for (int k = 0; k < myShape.Length; k++) {
			if (myShape[k].Equals('O')) {
				if (tempShape[k].Equals('X')) {
					AssignButtonColor(k, false);
					BombAudio.PlaySoundAtTransform("PressButton4", transform);

					yield return new WaitForSeconds(0.08f);
				}
			} else {
				ModuleButtons[k].transform.GetComponent<Renderer>().material.color = buttonOFF;
				buttonLight[k].intensity = 0.0f;

				if (tempShape[k].Equals('O')) {
					BombAudio.PlaySoundAtTransform("PressButton5", transform);

					yield return new WaitForSeconds(0.08f);
				}
			}
		}

		nowCoroutine = null;
	}

	IEnumerator EmptyLights() {
		for (int k = 0; k < ModuleButtons.Length; k++) {
			if (myShape[k].Equals('O')) {
				ModuleButtons[k].transform.GetComponent<Renderer>().material.color = buttonOFF;
				buttonLight[k].intensity = 0.0f;
				myShape = myShape.Remove(k, 1);
				myShape = myShape.Insert(k, "X");
				BombAudio.PlaySoundAtTransform("PressButton5", transform);

				yield return new WaitForSeconds(0.08f);
			}
		}

		nowCoroutine = null;
	}

	IEnumerator CheckSubmit() {
		var tempShape = myShape;
		var flickCount = 0;

		while (flickCount++ < 4) {
			for (int i = 0; i < ModuleButtons.Length; i++) {
				var buttonRend = ModuleButtons[i].transform.GetComponent<Renderer>();
				buttonRend.material.color = buttonOFF;
				buttonLight[i].intensity = 0.0f;

				if (flickCount == 4) {
					if (tempShape[i].Equals('O')) {
						AssignButtonColor(i, false);
					}
				} else {
					if (Random.Range(0, 2) == 1) {
						buttonRend.material.color = (moduleSolved) ? Color.green : Color.red;
						buttonLight[i].intensity = lightIntens;
					}
				}
			}

			yield return new WaitForSeconds(0.4f);
		}

		subCoroutine = null;
	}

	void SetArrows() {
		ArrowScreen.transform.GetComponent<Renderer>().material.SetTexture("_MainTex", ArrowTex[chooseArrows[nowArrow]]);
		NumScreen.transform.GetChild(0).GetComponent<TextMesh>().text = nowArrow.ToString();
		nowArrow++;
		nowArrow %= 15;
	}

	void UpdateSolution() {
		solvedMods = BombInfo.GetSolvedModuleNames().Count;

		if (solvedMods != prevSolvedMods) {
			var nowHalf = new String((countHalf) ? modLetter.Take(20).ToArray() : modLetter.Skip(20).ToArray());
			shapeSolution = intShape[(Math.Clamp(nowHalf.Count(x => x == ((countUnlit) ? 'X' : 'O')), 5, 14) + ((solvedMods % 2 == 1) ? 10 : 0)) - 5];
		}

		prevSolvedMods = solvedMods;
	}

	void InputLogShape(string passedShape) {
		for (int i = 0; i < ((passedShape.Length == 40) ? 8 : 5); i++) {
			var inputShape = "";
			var rowNum = ((passedShape.Length == 40) ? 5 : 3);

			for (int j = 0; j < rowNum; j++) {
				var addInput = (passedShape[j + (i * rowNum)].Equals('O')) ? "0" : "X";

				if (rowNum == 3) {
					addInput = passedShape[j + (i * rowNum)].ToString();
				}

				inputShape += addInput;
			}

			Debug.LogFormat(@"[Shapes Bombs #{0}] {1}", moduleId, "   " + inputShape);
		}
	}

	#pragma warning disable 414
	private readonly string TwitchHelpMessage = @"!{0} press A1 B39 C123... (column [A to E] and row [1 to 8] to press [you can input multiple rows in the same column]) | !{0} reset/res/r (resets initial letter) | !{0} empty/emp/e (empties lit squares) | !{0} submit/sub/s (submits current shape) | !{0} colorblind/cb (enables colorblind mode)";
	#pragma warning restore 414

	KMSelectable[] ProcessTwitchCommand(string command) {
		command = command.ToLowerInvariant().Trim();

		if (Regex.IsMatch(command, @"^press +[a-e1-8^, |&]+$")) {
			command = command.Substring(6).Trim();

			var presses = command.Split(new[] { ',', ' ', '|', '&' }, System.StringSplitOptions.RemoveEmptyEntries);
			var pressList = new List<KMSelectable>();

			for (int i = 0; i < presses.Length; i++) {
				if (Regex.IsMatch(presses[i], @"^[a-e][1-8]{1,8}$")) {
					var setCol = presses[i][0] - 'a';

					for (int j = 0; j < presses[i].Length - 1; j++) {
						var setPress = setCol + (5 * (presses[i][j + 1] - '1'));
						pressList.Add(ModuleButtons[setPress]);
					}
				}
			}

			return (pressList.Count > 0) ? pressList.ToArray() : null;
		}

		if (Regex.IsMatch(command, @"^\s*(colorblind|cb)\s*$")) {
			ColScreen.SetActive(true);

			return new KMSelectable[0];
		}

		return (Regex.IsMatch(command, @"^\s*(reset|res|r)\s*$")) ? new[] { ResetButton } :
			(Regex.IsMatch(command, @"^\s*(empty|emp|e)\s*$")) ? new[] { EmptyButton } :
			(Regex.IsMatch(command, @"^\s*(submit|sub|s)\s*$")) ? new[] { SubmitButton } : null;
	}
}