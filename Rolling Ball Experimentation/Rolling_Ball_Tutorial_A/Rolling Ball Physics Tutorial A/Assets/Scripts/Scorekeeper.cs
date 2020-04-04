using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Scorekeeper : MonoBehaviour {

	public static int Score = 0;

	public static int Total = 0;

	public static GameObject[] squares; // To store all squares

	[SerializeField]
	private TextMesh ScoreText = null;

	[SerializeField]
	private TextMesh SeparatorText = null;

	[SerializeField]
	private TextMesh TotalText = null;

	// Use this for initialization
	void Start () {
		Score = 0;
		squares = GameObject.FindGameObjectsWithTag ("Square");

		Total = squares.Length;
		TotalText.text = Total.ToString();

		SeparatorText.text = "/";

	}
	
	// Update is called once per frame
	public void Update () {

		if (Score == Total) 
		{
			TotalText.text = "GAME!";
			Score = 0;
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}

		ScoreText.text = Score.ToString ();
				
	}


}
