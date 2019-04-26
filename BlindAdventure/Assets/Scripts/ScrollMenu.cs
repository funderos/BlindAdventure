using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

//Author: Stadler Viktor
//This class manages the Scroll Rects, that each page snap to the center of the screen
public class ScrollMenu : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler {

    private ScrollRect scrollRect;
    private RectTransform rectTransform;
    private RectTransform container;
    private int pageCount;
    private int currentPage;
    private bool move;
    private Vector2 targetPosition;
    private List<Vector2> pagePositions = new List<Vector2>();
    private bool touch;
    private Vector2 startPosition;
	private string typ;
	private string[] childName;
	private int language;
	private int width;

	private Touch startPositionTouch = new Touch (); //Startposition of the swipe
	private bool swiped = false; //If we swipe then true
	private float differenceX;
	private float differenceY;

    void Start() {
		language = PlayerPrefs.GetInt ("Language"); //Gets the current language from the player preferences; german = 1 or english = 0
		if (language == 1) {
			TTSManager.SetLanguage (TTSManager.GERMAN);
		} else {
			TTSManager.SetLanguage (TTSManager.ENGLISH);
		}
		scrollRect = GetComponent<ScrollRect>();
		rectTransform = GetComponent<RectTransform>();
		container = scrollRect.content;
		pageCount = container.childCount;
		pagePos();
	}

    void Update() {
        if (move) {
			container.anchoredPosition = Vector2.Lerp(container.anchoredPosition, targetPosition, 0.7f); //To navigate to a position
        }

		//Detects swipe direction 
		foreach (Touch touch in Input.touches) {
			if (touch.phase == TouchPhase.Began) {
				startPositionTouch = touch;
			} 
			else if (swiped == false && touch.phase == TouchPhase.Moved) {
				differenceX = startPositionTouch.position.x - touch.position.x;
				differenceY = startPositionTouch.position.y - touch.position.y;
				swiped = true;
			} 				
			else if (touch.phase == TouchPhase.Ended) {
				startPositionTouch = new Touch ();
				swiped = false;
			}
		}
    }

	//Generates each container child position from the container
    private void pagePos() {
        width = (int)rectTransform.rect.width;
        int containerWidth = width * pageCount;
        Vector2 newSize = new Vector2(containerWidth, 0);
        container.sizeDelta = newSize;
        Vector2 newPosition = new Vector2(containerWidth / 2, 0);
        container.anchoredPosition = newPosition;
		childName = new string[pageCount];
        //Child positions
        for (int i = 0; i < pageCount; i++) {
            RectTransform child = container.GetChild(i).GetComponent<RectTransform>();
            Vector2 childPosition = new Vector2(i * width - containerWidth / 2 + width / 2, 0f);
            child.anchoredPosition = childPosition;
            pagePositions.Add(-childPosition);

			//Name of each container child for the audio output (german and english)
			if (container.GetChild (0).name == "Level1") {
				int k = i + 1;
				childName [i] = "Level" + (k);
			} else if (container.GetChild (0).name == "Background1") {
				int k = i + 1;
				childName [i] = "Musik" + (k);
				typ = "background";
			} else if(container.GetChild (0).name == "Steeplechase") {
				if (i == 0) {
					if (language == 1)
						childName [i] = "Hindernislauf";
					else 
						childName [i] = "Steeplechase";
				} else if (i == 1) {
					if (language == 1)
						childName [i] = "Kampf";
					else 
						childName [i] = "Fight";
				} else {
					if (language == 1)
						childName [2] = "Rätsel";
					else 
						childName [2] = "Quiz";
				} 
				typ = "minigame";
			} else if (container.GetChild (0).name == "Book"){
				if (i == 0) {
					if (language == 1)
						childName [i] = "Buch";
					else
						childName [i] = "Book";
				} else if (i == 1) {
					if (language == 1)
						childName [i] = "Kerze";
					else
						childName [i] = "Candle";
				} else if (i == 2) {
					childName [i] = "Magnet";
				} else if (i == 3) {
					if (language == 1)
						childName [i] = "Uhr";
					else
						childName [i] = "Clock";
				} else if (i == 4) {
					if (language == 1)
						childName [i] = "Messer";
					else
						childName [i] = "Knife";
				} else if (i == 5) {
					if (language == 1)
						childName [i] = "Kompass";
					else
						childName [i] = "Compass";
				} else if (i == 6) {
					childName [i] = "Hammer";
				} else if (i == 7) {
					childName [i] = "Ring";
				} else if (i == 8) {
					if (language == 1)
						childName [i] = "Regenschirm";
					else
						childName [i] = "Umbrella";
				} else {
					if (language == 1)
						childName [9] = "Seil";
					else
						childName [9] = "Rope";
				}
				typ = "item";
			} else if (container.GetChild (0).name == "Dungeon"){
				if (i == 0) {
					if (language == 1)
						childName [i] = "Drache";
					else
						childName [i] = "Dungeon";
				} else if (i == 1) {
					if (language == 1)
						childName [i] = "Loewe";
					else
						childName [i] = "Lion";
				} else {
					if (language == 1)
						childName [2] = "Hexe";
					else
						childName [2] = "Witch";
				}
				typ = "opponent";
			} else {
				if (language == 1)
					childName [i] = "Knoten" + i;
				else {
					childName [i] = "Node" + i;
				}
			}
        }
    }

	//Moves to a certain container child, depends on the index
	private void moveToPage(int index) {
        index = Mathf.Clamp(index, 0, pageCount - 1);
        targetPosition = pagePositions[index];
        move = true;
        currentPage = index;
    }

	//Outputs the name of the next page
    private void nextPage() {
        moveToPage(currentPage + 1);
		Handheld.Vibrate();
		TTSManager.Speak(childName[currentPage], false);
		savePrefer ();
    }

	//Outputs the name of the previous page
    private void previousPage() {
		moveToPage (currentPage - 1);
		Handheld.Vibrate ();
		TTSManager.Speak (childName [currentPage], false);
		savePrefer ();
	}	

	//Beginn dragging on the screen
    public void OnBeginDrag(PointerEventData aEventData) {
        move = false;
        touch = false;
    }

	//End dragging on the screen
    public void OnEndDrag(PointerEventData aEventData) {
        float difference = startPosition.x - container.anchoredPosition.x;
		if (Mathf.Abs (differenceX) > Mathf.Abs (differenceY)) {
			if (Mathf.Abs (difference) > 5 && Mathf.Abs (difference) < width / 2) {
				if (difference > 5) {
					nextPage ();
				}
				if (difference < 5) {
					previousPage ();
				}
			} 
		} else if (differenceY < 0) {
			moveToPage (currentPage); 
		}
        touch = false;
    }

	//While dragging
    public void OnDrag(PointerEventData aEventData) {
        if (!touch) {
            touch = true;
            startPosition = container.anchoredPosition;
        } 
    }

	//Saves the current child position name for each scroll rect (for audio output)
	public void savePrefer(){
		if (typ == "minigame") {
			PlayerPrefs.SetString ("minigameName", childName [currentPage]);
		} else if (typ == "item") {
			PlayerPrefs.SetString ("itemName", childName [currentPage]);
		} else if (typ == "opponent") {
			PlayerPrefs.SetString ("opponentName", childName [currentPage]);
		} else {	//background
			PlayerPrefs.SetString ("backgroundName", childName [currentPage]);
		}
	}
}