/* ListBox 1.1b                         */
/* Nov 11, 2015                         */
/* By Orbcreation BV                    */
/* Richard Knol                         */
/* info@orbcreation.com                 */
/* games, components and freelance work */

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public delegate void ValueChangedMethod(ListBox aListBox);

public class ListBox : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler {   // for now no IpointerClickHandler


    public int[] optionInts;
    public string[] optionStrings;
    public Sprite[] optionSprites;
    public object[] optionValues;
    public GameObject closedListBox;
    public GameObject openedListBox;
    public GameObject scrollableArea;
    public Scrollbar scrollbar;
    public GameObject optionTemplate;
    public GameObject highlightedOption;
    public GameObject hoverOption;

    /// <summary>
    /// Child input fields (if applicable)
    /// </summary>
    public InputField[] Input_Children;

    public bool alwaysOpen = false;
    public bool allowSelectionBehavior = true;
    public bool allowHoveringBehavior = true;
    public bool changeTitleToSelectedOption = true;
    public float scrollWheelSpeed = 10f;

    public int valueInt;
    public string valueString;
    public Sprite valueSprite;

    public ValueChangedMethod onValueChanged;

    private string defaultTitle = "";
    private Sprite defaultSprite = null;

    public bool isOpen = false;
    private bool isDragging = false;
    private Vector2 openedSize = new Vector2(100, 20);
    private Vector2 optionSize = new Vector2(100, 20);

    public GameObject[] options;
    private int nrOfOptions = 0;
    private int nrOfVisibleOptions = 0;
    private float optionDistance = 0f;
    private float mouseScroll = 0f;
    private float yScroll = 0f;
    private float yScrollSpeed = 0f;

    /// <summary>
    /// The scroll area height of this control
    /// </summary>
    private float scrollAreaHeight
    {
        get
        {
            return nrOfOptions * optionDistance;
        }
    }

    public int scrollIndexOffset = 0;
    private float lastPointerEventTime = 0f;
    private float pointerDownTime = 0f;
    private Vector2 pointerDownPosition = new Vector2(0, 0);
    private float minClickTime = 0.5f; // 0.5 seconds between pointerDown and pointerUp for click
    private float minClickMovement = 5f; // 5 pixels tolerance to handle pointerUp as click
    private int hoverIndex = -1;
    private Camera lastUsedCamera = null;


    /// <summary>
    /// Default index on reset
    /// </summary>
    public int DefaultIndex = -1;

    /// <summary>
    /// Maximum items that can be added to this listbox
    /// </summary>
    public int MaxItems = -1;

    /// <summary>
    /// Minimum items in this list
    /// </summary>
    public int MinItems = -1;

    /// <summary>
    /// Whether or not this listbox was clicked this frame
    /// </summary>
    public bool ClickedThisFrame = false;

    /// <summary>
    /// Index this frame
    /// </summary>
    int IndexThisFrame = 0;

    /// <summary>
    /// Index last frame
    /// </summary>
    int IndexLastFrame;

    /// <summary>
    /// Did listbox value change this frame?
    /// </summary>
    /// <returns></returns>
    public bool ValueChangedThisFrame
    {
        get
        {
            return (IndexThisFrame != IndexLastFrame);
        }
    }

    /// <summary>
    /// Get previous index
    /// </summary>
    public int PreviousIndex
    {
        get
        {
            return IndexLastFrame;
        }
    }

    /// <summary>
    /// Does this listbox have a selected item?
    /// If listbox has no items, or index is -1, this value will be false.
    /// </summary>
    public bool hasSelection
    {
        get
        {
            return IndexThisFrame > -1;
        }
    }

    /// <summary>
    /// If the mouse was clicked or not
    /// </summary>
    bool MouseClicked;

    /// <summary>
    /// Start functions
    /// </summary>
    void Start()
    {
        Transform subObject = closedListBox.transform.Find("Text");
        if (subObject != null) {
            Text txt = subObject.gameObject.GetComponent<Text>();
            if (txt != null) defaultTitle = txt.text;
        }
        subObject = closedListBox.transform.Find("Image");
        if (subObject != null) {
            Image img = subObject.gameObject.GetComponent<Image>();
            if (img != null) defaultSprite = img.sprite;
        }
        if (scrollbar != null) scrollbar.onValueChanged.AddListener(OnScrollbarChanged);
        Reset();
    }


    public void Reset()
    {
        SetOptions(optionInts, optionStrings, optionSprites, optionValues);

        RectTransform rectTrans = scrollableArea.GetComponent<RectTransform>();
        openedSize = rectTrans.rect.size;

        RectTransform optionRectTrans = optionTemplate.GetComponent<RectTransform>();
        optionSize = optionRectTrans.rect.size;

        //NOTE: DISTANCE BETWEEN OPTIONS IS OPTIONS Y MAX POSITION - HIGHLIGHTED OPTION MAX Y POSITION
        RectTransform hlOptionRectTrans = highlightedOption.GetComponent<RectTransform>();
        optionDistance = Mathf.Abs(optionRectTrans.offsetMax.y - hlOptionRectTrans.offsetMax.y);

        yScroll = 0f;
        highlightedOption.SetActive(false);
        optionTemplate.SetActive(false);
        SetSelectedIndex(DefaultIndex);
    }

    void Update()
    {
        //Set index last frame, index this frame
        IndexLastFrame = IndexThisFrame;
        IndexThisFrame = GetSelectedIndex();

        if (hoverIndex >= 0) mouseScroll += Input.mouseScrollDelta.y * scrollWheelSpeed;
        yScroll += mouseScroll * Time.deltaTime;  // apply mouse scrollspeed

        yScrollSpeed = Mathf.Clamp(yScrollSpeed, -750f, 750f);  // clamp to prevent insane big scrollspeeds

        yScroll += yScrollSpeed * Time.deltaTime;  // apply drag scrollspeed

        float oldYScroll = yScroll;
        if (yScroll < 0f) {  // bounce back when scrolled to the top
            if (yScroll > -2f * Time.deltaTime && Mathf.Abs(mouseScroll + yScrollSpeed) < 1f) yScroll = 0f;
            else yScroll += Mathf.Sqrt(yScroll * -1f) * 35f * Time.deltaTime;
            if (yScrollSpeed < 0f && (!isDragging)) yScrollSpeed *= 1f - (4f * Time.deltaTime);
            else yScrollSpeed *= 1f - (1f * Time.deltaTime);
        } else if (yScroll > Mathf.Max(0f, scrollAreaHeight - openedSize.y)) {  // bounce back when scrolled to bottom
            float dif = yScroll - Mathf.Max(0f, scrollAreaHeight - openedSize.y);
            if (dif < 2f * Time.deltaTime && Mathf.Abs(mouseScroll + yScrollSpeed) < 1f) yScroll = Mathf.Max(0f, scrollAreaHeight - openedSize.y);
            else yScroll -= Mathf.Sqrt(dif) * 35f * Time.deltaTime;
            if (yScrollSpeed > 0f && (!isDragging)) yScrollSpeed *= 1f - (4f * Time.deltaTime);
            else yScrollSpeed *= 1f - (1f * Time.deltaTime);
        }

        // Fade scroll speed out
        yScrollSpeed *= 1f - (0.5f * Time.deltaTime);
        if (Mathf.Abs(yScrollSpeed) < 0.5f) yScrollSpeed = 0f;  // prevent miniscule scroll amounts

        // Fade mouse scroll out
        mouseScroll = Mathf.Lerp(mouseScroll, 0f, 2f * Time.deltaTime);
        if (Mathf.Abs(mouseScroll) < 0.5f) mouseScroll = 0f;  // prevent miniscule scroll amounts


        if (alwaysOpen && !isOpen) OpenListBox();
        if (isOpen) {
            if (scrollbar != null) {
                if ((scrollAreaHeight - openedSize.y) > 0f) scrollbar.value = yScroll / (scrollAreaHeight - openedSize.y);
                if (scrollAreaHeight > 0f) scrollbar.size = Mathf.Clamp01(openedSize.y / scrollAreaHeight);
            }
            // Close the listbox when another object is selected
            GameObject selObj = EventSystem.current.currentSelectedGameObject;
            if (selObj != gameObject && selObj != scrollableArea && (scrollbar == null || selObj != scrollbar.gameObject)) {
                CloseListBox();
            }

            OnPointerMoved(Input.mousePosition);
            ShowOptions();
        }

        //Reset vars?
        if (MouseClicked)
        {
            //Mouse click handler
            MouseClicked = false;
            ClickedThisFrame = true;
        }
        else
        {

            //If mouse was clicked last frame, set as clicked
            if (ClickedThisFrame)
            {
                ResetVarsThisFrame();
            }
        }

    }

    // Bunch of overloaded methods to set the listbox options
    public void SetOptions(Sprite[] sprites) {
        SetOptions(null, null, sprites, null);
    }
    public void SetOptions(string[] strings) {
        SetOptions(null, strings, null, strings);
    }
    public void SetOptions(int[] ints) {
        // When you only set an array of ints, we have to create an array of strings to display the values
        if (ints == null || ints.Length <= 0) return;
        string[] strings = new string[ints.Length];
        for (int i = 0; i < ints.Length; i++) strings[i] = "" + ints[i];
        SetOptions(ints, strings, null, strings);
    }
    public void SetOptions(int[] ints, Sprite[] sprites) {
        SetOptions(ints, null, sprites, null);
    }
    public void SetOptions(int[] ints, string[] strings) {
        SetOptions(ints, strings, null, strings);
    }
    public void SetOptions(string[] strings, Sprite[] sprites) {
        SetOptions(null, strings, sprites, strings);
    }

    public void SetOptions(int[] ints, string[] strings, Sprite[] sprites, object[] Values) {
        // we have to close the listbox when you set the option values 
        CloseListBox();

        // obtain the number of options
        nrOfOptions = 0;
        if (ints != null && ints.Length > 0) nrOfOptions = ints.Length;
        else if (strings != null && strings.Length > 0) nrOfOptions = strings.Length;
        else if (sprites != null && sprites.Length > 0) nrOfOptions = sprites.Length;

        // Make sure the list is filled with a minimal number of options. if not, the first item will appear somewhere in the middel instead of at the top of the opened listbox
        nrOfVisibleOptions = 5;
        if (optionDistance > 0f) nrOfVisibleOptions = Mathf.CeilToInt(openedSize.y / optionDistance) + 1;
        if (nrOfOptions < nrOfVisibleOptions - 1) {
            if (strings != null && strings.Length > 0) {
                int i = 0;
                string[] newStrings = new string[nrOfVisibleOptions - 1];
                for (i = 0; i < strings.Length; i++) newStrings[i] = strings[i];
                for (; i < nrOfVisibleOptions - 1; i++) newStrings[i] = "";
                strings = newStrings;
                nrOfOptions = nrOfVisibleOptions - 1;
            }
            if (sprites != null && sprites.Length > 0) {
                int i = 0;
                Sprite[] newSprites = new Sprite[nrOfVisibleOptions - 1];
                for (i = 0; i < sprites.Length; i++) newSprites[i] = sprites[i];
                for (; i < nrOfVisibleOptions - 1; i++) newSprites[i] = null;
                sprites = newSprites;
                nrOfOptions = nrOfVisibleOptions - 1;
            }
            if (Values != null && Values.Length > 0)
            {
                int i = 0;
                object[] newVals = new object[nrOfVisibleOptions - 1];
                for (i = 0; i < Values.Length; i++) newVals[i] = Values[i];
                for (; i < nrOfVisibleOptions - 1; i++) newVals[i] = "";
                Values = newVals;
                nrOfOptions = nrOfVisibleOptions - 1;
            }
        }

        // length of all arrays should be the same
        if (strings != null && strings.Length > 0 && strings.Length != nrOfOptions) {
            Debug.Log("Length of string array(" + strings.Length + ") not equal to nr of options(" + nrOfOptions + ")");
            return;
        }
        if (sprites != null && sprites.Length > 0 && sprites.Length != nrOfOptions) {
            Debug.Log("Length of sprite array(" + sprites.Length + ") not equal to nr of options(" + nrOfOptions + ")");
            return;
        }

        // store the new arrays as option values
        optionInts = ints;
        optionStrings = strings;
        optionSprites = sprites;
        optionValues = Values;
    }

    /// <summary>
    /// Clears current options
    /// </summary>
    public void Clear()
    {
        optionInts = new List<int>().ToArray();
        optionStrings = new List<string>().ToArray();
        optionSprites = new List<Sprite>().ToArray();
        optionValues = new List<string>().ToArray();
    }

    /// <summary>
    /// Sets option name
    /// </summary>
    /// <param name="Index">Index to set to text</param>
    /// <param name="Text">Text to set this option to</param>
    public void SetOptionText(int Index, string Text)
    {
        if (Index < optionStrings.Length && Index > -1)
        {
            optionStrings[Index] = Text;
        }
    }

    /// <summary>
    /// Sets the n
    /// </summary>
    /// <param name="Text">The optiont to set he current option to</param>
    public void SetCurrentOptionText(string Text)
    {
        //Get selected index
        int Curr_Index = GetSelectedIndex();
        SetOptionText(Curr_Index, Text);

    }

    /// <summary>
    /// Add new option
    /// </summary>
    /// <param name="Txt"></param>
    public void AddOption(string Txt)
    {
        AddOption(Txt, null);
    }

    /// <summary>
    /// Adds an option to this listbox
    /// </summary>
    /// <param name="Txt"></param>
    public void AddOption(string Txt, object Value)
    {
        //If items < maximum options
        if (Count() < MaxItems || MaxItems == -1)
        {

            //If valid map seleted, then
            if (!string.IsNullOrEmpty(Txt))
            {
                //Get old options
                string[] OldOptions = optionStrings;
                object[] OldValues = optionValues;

                //Add to list of selected maps
                List<string> NewOptions = new List<string>();
                List<int> NewInts = new List<int>();
                List<object> NewVals = new List<object>();
                List<Sprite> NewSprites = new List<Sprite>();
                for (int i = 0; i < OldOptions.Length; i++)
                {
                    string Opt = OldOptions[i];
                    if (!string.IsNullOrEmpty(Opt))
                    {
                        NewOptions.Add(Opt);
                        NewInts.Add(i);
                        NewVals.Add(OldValues[i]);
                        NewSprites.Add(null);

                    }
                }

                //Add new item
                NewOptions.Add(Txt);
                NewInts.Add(NewInts.Count);
                NewVals.Add(Value);
                NewSprites.Add(null);

                //Update options
                SetOptions(NewInts.ToArray(), NewOptions.ToArray(), NewSprites.ToArray(), NewVals.ToArray());
            }
        }
    }

    /// <summary>
    /// Inserts a new option at index
    /// </summary>
    /// <param name="Txt">Display Text</param>
    /// <param name="Value">Value to insert</param>
    /// <param name="Spr">Sprite</param>
    /// <param name="Index">Index to insert at</param>
    public void InsertOption(string Txt, object Value, Sprite Spr, int Index)
    {
        //If items < maximum options
        if (Count() < MaxItems || MaxItems == -1)
        {
            List<string> StringTxt = optionStrings.ToList();
            List<object> ValueTxt = optionValues.ToList();
            List<Sprite> Sprites = optionSprites.ToList();
            List<int> Indices = optionInts.ToList();

            //Insert item
            StringTxt.Insert(Index, Txt);
            ValueTxt.Insert(Index, Value);
            Sprites.Insert(Index, Spr);
            Indices.Add(Indices.Count);

            //Set values
            SetOptions(Indices.ToArray(), StringTxt.ToArray(), Sprites.ToArray(), ValueTxt.ToArray());
        }


    }

    /// <summary>
    /// Removes the currently selected option
    /// </summary>
    public void RemoveSelectedOption()
    {
        //Get selected index for added maps
        int Index = GetSelectedIndex();

        //Remove this option if option to remove
        if (Index > -1)
        {
            RemoveOption(Index);
        }
    }

    /// <summary>
    /// Refreshes indices
    /// </summary>
    void RefreshInts()
    {
        //Reset option ints
        optionInts = new int[optionStrings.Length];

        for(int i = 0; i < optionStrings.Length; i++)
        {
            optionInts[i] = i;
        }
    }

    /// <summary>
    /// Remove an option at index X
    /// </summary>
    /// <param name="Index"></param>
    public void RemoveOption(int Index)
    {
        //If valid index, then
        if (Index > -1)
        {
            if (Count() > MinItems || MinItems == -1)
            {
                //New Strings
                List<string> NewStrings = new List<string>();
                for (int i = 0; i < optionStrings.Length; i++)
                {
                    string text = optionStrings[i];
                    if (i != Index) NewStrings.Add(text);
                }

                //New Values
                List<object> NewVals = new List<object>();
                for (int i = 0; i < optionValues.Length; i++)
                {
                    object obj = optionValues[i];
                    if (i != Index) NewVals.Add(obj);
                }

                //Fill integers
                List<int> NewInts = new List<int>();
                for (int i = 0; i < NewVals.Count; i++)
                {
                    NewInts.Add(i);
                }

                //Add Options
                SetOptions(NewInts.ToArray(), NewStrings.ToArray(), null, NewVals.ToArray());
            }
        }
    }

    /// <summary>
    /// Get number of text
    /// </summary>
    /// <returns></returns>
    public int Count()
    {
        int NumItems = 0;
        for(int i = 0; i < optionStrings.Length; i++)
        {
            string item = optionStrings[i];
            if(!string.IsNullOrEmpty(item))
            {
                NumItems++;
            }
        }
        return NumItems;
    }

    /// <summary>
    /// Gets currently selected text
    /// </summary>
    /// <returns></returns>
    public string GetSelectedText()
    {
        int Index = GetSelectedIndex();
        if(Index > -1)
        {
            return optionStrings[Index];
        }
        else
        {
            return "";
        }
    }

    /// <summary>
    /// Gets currently selected value
    /// </summary>
    /// <returns></returns>
    public object GetSelectedValue()
    {
        int Index = GetSelectedIndex();
        if (Index > -1)
        {
            return optionValues[Index];
        }
        else
        {
            return "";
        }
    }

    /// <summary>
    /// Return generic value of value. Be careful with this - may cause errors if dropdown box has invalid selections (like a blank value). Use only for options that can't have an empty value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T GetSelectedValueOfGenericType<T>()
    {
        int Index = GetSelectedIndex();
        if (Index < -1) Index = 0;
        return (T)optionValues[Index];
    }

    // Find the index of the currently selected option 
    // returns -1 if no option is selected
    public int GetSelectedIndex() {
		if(valueInt > 0) {
			for(int i=0;i<optionInts.Length;i++) {
				if(optionInts[i] == valueInt) return i;
			}
		}
		if(valueString != null && valueString.Length > 0) {
			for(int i=0;i<optionStrings.Length;i++) {
				if(optionStrings[i] == valueString) return i;
			}
		}
		if(valueSprite != null) {
			for(int i=0;optionSprites != null && i<optionSprites.Length;i++) {
				if(optionSprites[i] == valueSprite) return i;
			}
		}
		return -1;
	}

    /// <summary>
    /// Gets a value at a given index. If no value set, returns the display value
    /// </summary>
    public object GetValueAtIndex(int Index)
    {
        if(Index < optionValues.Length)
        {
            return optionValues[Index];
        }
        else
        {
            return optionStrings[Index];
        }
    }

    /// <summary>
    /// Get previously selected value
    /// </summary>
    /// <returns></returns>
    public object GetPreviousValue()
    {
        int Index = IndexLastFrame;
        if (Index > -1)
        {
            return optionValues[Index];
        }
        else
        {
            if(Count() > 0)
            {
                return optionValues[0];
            }
            else
            {
                return null;
            }
        }
    }

    public void SetValue(int aValue) 
	{
		valueInt = aValue;
		int idx = 0;
		for(int i=0;i<optionInts.Length;i++) {
			if(optionInts[i] == valueInt) idx = i;
		}
		SetSelectedIndex(idx);
	}
	public void SetValue(string aValue) 
	{
		valueString = aValue;
		int idx = 0;
		for(int i=0;i<optionStrings.Length;i++) {
			if(optionStrings[i] == valueString) idx = i;
		}
		SetSelectedIndex(idx);
	}
	public void SetValue(Sprite aSprite) 
	{
		valueSprite = aSprite;
		int idx = 0;
		for(int i=0;optionSprites != null && i<optionSprites.Length;i++) {
			if(optionSprites[i] == valueSprite) idx = i;
		}
		SetSelectedIndex(idx);
	}
	// Set valueInt, valueString and valueSprite to match the index of a newly selected option
	// Will also trigger the onValueChanged delegate
	public void SetSelectedIndex(int idx) 
	{
		int oldIdx = GetSelectedIndex();

        //If is valid index, then
        if (idx < Count())
        {
            valueInt = idx;
            valueString = null;
            valueSprite = null;

            if (optionStrings != null && idx >= 0 && idx < optionStrings.Length)
            {
                valueString = optionStrings[idx];
            }
            if (optionSprites != null && idx >= 0 && idx < optionSprites.Length)
            {
                valueSprite = optionSprites[idx];
            }

            string titleToShow = defaultTitle;
            Sprite spriteToShow = defaultSprite;
            if (changeTitleToSelectedOption)
            {
                if (valueString != null) titleToShow = valueString;
                if (valueSprite != null) spriteToShow = valueSprite;
            }
            Transform subObject = closedListBox.transform.Find("Text");
            if (subObject != null)
            {
                Text txt = subObject.gameObject.GetComponent<Text>();
                if (txt != null)
                {
                    if (titleToShow != null)
                    {
                        txt.text = titleToShow;
                        txt.enabled = true;
                    }
                    else txt.enabled = false;
                }
            }
            subObject = closedListBox.transform.Find("Image");
            if (subObject != null)
            {
                Image img = subObject.gameObject.GetComponent<Image>();
                if (img != null)
                {
                    if (spriteToShow != null)
                    {
                        img.sprite = spriteToShow;
                        img.enabled = true;
                    }
                    else img.enabled = false;
                }
            }
            if (idx != oldIdx && onValueChanged != null)
            {
                onValueChanged(this);
            }
        }
	}

	public void OpenListBox() {
		SetOpen(true);
	}
	public void CloseListBox() {
		if(!alwaysOpen) SetOpen(false);
	}

	// This title will be shown if no option is selected or when changeTitleToSelectedOption == false
	public void SetTitle(string aTitle) {
		defaultTitle = aTitle;
	}
	// This sprite will be shown if no option is selected or when changeTitleToSelectedOption == false
	public void SetSprite(Sprite aSprite) {
		defaultSprite = aSprite;
	}

	// Catching events
	public void OnPointerDown(PointerEventData data) {
		lastPointerEventTime = Time.time;
		// We handle clicking manually, because OnPointerClick is too trigger happy 
		// at the time of writing this code
		pointerDownTime = Time.time;
		pointerDownPosition = data.position;
		lastUsedCamera = data.enterEventCamera;
	}
	public void OnPointerUp(PointerEventData data) {
		float deltaTime = Time.time - pointerDownTime;
		if(deltaTime < minClickTime && Vector2.Distance(pointerDownPosition, data.position) < minClickMovement) {
			// send it manually because the new EventSystem doesnt seem to work properly
			// at the time of writing this code
			// OnPointerClick is too trigger happy for my taste
			OnPointerClick(data);  
		}
	}
	// We invoke this manually, because IOnPointerClick is too trigger happy 
	// at the time of writing this code
	public void OnPointerClick(PointerEventData data) {

        //Set the listbox as clicked
        MouseClicked = true;

		if(isOpen && allowSelectionBehavior) {
			for(int i=0;i<nrOfVisibleOptions;i++) {
				RectTransform optionTransform = options[i].GetComponent<RectTransform>();
				if(RectTransformUtility.RectangleContainsScreenPoint(optionTransform, data.position, data.enterEventCamera)) {
					SetSelectedIndex(i + scrollIndexOffset);
				}
			}
			ShowOptions();
			Invoke("CloseListBox", 0.3f);
		} else OpenListBox();
	}

	// This is invoked by catching Input.mousePosition in the Update function
	public void OnPointerMoved(Vector3 newPosition) {
		hoverIndex = -1;
		if(isOpen && allowHoveringBehavior) {
			for(int i=0;i<nrOfVisibleOptions;i++) {
				RectTransform optionTransform = options[i].GetComponent<RectTransform>();
				if(RectTransformUtility.RectangleContainsScreenPoint(optionTransform, newPosition, lastUsedCamera)) {
					hoverIndex = i + scrollIndexOffset;
				}
			}
		}
	}


	public void OnBeginDrag(PointerEventData data) {
		isDragging = true;
	}

	public void OnEndDrag(PointerEventData data) {
		isDragging = false;
	}

	public void OnDrag(PointerEventData data) {
		float deltaTime = Time.time - lastPointerEventTime;
		if(deltaTime>0f) yScrollSpeed = (yScrollSpeed + (data.delta.y / deltaTime)) * 0.5f;
		lastPointerEventTime = Time.time;
	}

	public void OnScrollbarChanged(float newValue) {
		yScrollSpeed = 0f;
		yScroll = (scrollAreaHeight - openedSize.y) * newValue;
	}

	// Set up/clear an array of visible options, and switch view between closedListBox and openedListBox
	private void SetOpen(bool yn) {
		if(isOpen == yn) return;
		closedListBox.SetActive(!yn);
		openedListBox.SetActive(yn);
		if(yn) {

            //TODO: When only adding items, is never being called
	        //scrollAreaHeight = nrOfOptions * optionDistance;
			nrOfVisibleOptions = 5;
			if(optionDistance > 0f) nrOfVisibleOptions = Mathf.CeilToInt(openedSize.y / optionDistance) + 1;

			float y = 0f;
			options = new GameObject[nrOfVisibleOptions];
			for(int i=0;i<nrOfVisibleOptions;i++) {
				options[i] = (GameObject)GameObject.Instantiate(optionTemplate);
				RectTransform optionTransform = options[i].GetComponent<RectTransform>();
				optionTransform.SetParent(scrollableArea.transform);
				optionTransform.localScale = new Vector3(1f ,1f, 1f);
				optionTransform.pivot = new Vector2(0f ,1f);
		        optionTransform.anchorMin = new Vector2(0f, 1f);
		        optionTransform.anchorMax = new Vector2(0f, 1f);
		    	FixLocalPositionAndRotation(optionTransform);
		        y -= optionDistance;
				options[i].SetActive(true);
		    }

			RectTransform highlightedOptionTransform = highlightedOption.GetComponent<RectTransform>();
		    highlightedOptionTransform.SetParent(scrollableArea.transform);
			highlightedOptionTransform.localScale = new Vector3(1f ,1f, 1f);
			highlightedOptionTransform.pivot = new Vector2(0f ,1f);
		    highlightedOptionTransform.anchorMin = new Vector2(0f, 1f);
		    highlightedOptionTransform.anchorMax = new Vector2(0f, 1f);
	      	FixLocalPositionAndRotation(highlightedOptionTransform);

	      	if(hoverOption != null) {
				RectTransform hoverOptionTransform = hoverOption.GetComponent<RectTransform>();
			    hoverOptionTransform.SetParent(scrollableArea.transform);
				hoverOptionTransform.localScale = new Vector3(1f ,1f, 1f);
				hoverOptionTransform.pivot = new Vector2(0f ,1f);
			    hoverOptionTransform.anchorMin = new Vector2(0f, 1f);
			    hoverOptionTransform.anchorMax = new Vector2(0f, 1f);
		      	FixLocalPositionAndRotation(hoverOptionTransform);
		    }

			isOpen = yn;
			ShowOptions();

		} else {
			isOpen = yn;
			ClearAllOptions();
		}
	}

	// Compute positions and contents of the visible options.
	// The listbox uses only a handfull of UI option elements to cover the visible area.
	// The options value array can be much longer.
	// This functions makes sure only the currently visible option values are placed inside the UI elements
	// and displayed in the right place. This will make it seem like a very long list, which it is not.
	private void ShowOptions() {
		if(!isOpen) return;
		int selectedIndex = GetSelectedIndex();  // get the index of the selected option
		scrollIndexOffset = 0;
		float y = yScroll;  // set y to match the current scroll position

		hoverOption.SetActive(false);   // deactivate the hover option (will be activated later if needed)
		highlightedOption.SetActive(false);   // deactivate the highlighted option (will be activated later if needed)

		// compute the index of the first visible option and the matching y coordinate;
		scrollIndexOffset = Mathf.FloorToInt(Mathf.Max(y / optionDistance, 0f));
		y -= scrollIndexOffset * optionDistance;

		// loop through the visible options
		for(int i=0;i<nrOfVisibleOptions;i++) {
			GameObject option = options[i];

			// if this option matches the currently selected index or the hoverIndex,
			// we disable the normal option and use the highlightedOption instead
			if(i + scrollIndexOffset == hoverIndex && hoverOption != null) {  
				option.SetActive(false);
				option = hoverOption;
			}
			if(i + scrollIndexOffset == selectedIndex) {  
				option.SetActive(false);
				option = highlightedOption;
			}

			option.SetActive(true);  // make active in case it was previously disabled

			// Set the position and size of the option
			RectTransform optionTransform = option.GetComponent<RectTransform>();
	        optionTransform.offsetMin = new Vector2(0f, y - optionSize.y);
	        optionTransform.offsetMax = new Vector2(openedSize.x, y);
		    FixLocalPositionAndRotation(optionTransform);

	        // Set the contents
	        Transform subObject = option.transform.Find("Text");
	        if(subObject != null) {
	        	Text txt = subObject.gameObject.GetComponent<Text>();
	        	if(txt != null) {
		        	if(optionStrings.Length > i+scrollIndexOffset) {
			        	txt.enabled = true;
			        	txt.text = optionStrings[i+scrollIndexOffset];
			        } else {
			        	txt.enabled = false;
			        }
			    }
		    }
			subObject = option.transform.Find("Image");
	        if(subObject != null) {
	        	Image img = subObject.gameObject.GetComponent<Image>();
	        	if(img != null) {
		        	if(optionSprites != null && optionSprites.Length > i+scrollIndexOffset) {
			        	img.enabled = true;
		        		img.sprite = optionSprites[i+scrollIndexOffset];
		        	} else {
		        		img.enabled = false;
		        	}
		        }
			}

			// move y to next option
	        y -= optionDistance;
		}
	}

	private void ClearAllOptions() {
		if(options == null) return;  // nothing to clear

		// destroy the option gameobjects
		for(int i=options.Length-1;i>=0;i--) {
			if(options[i] != null) GameObject.Destroy(options[i]);
		}
		options = null;
	}

	private void FixLocalPositionAndRotation(RectTransform aTransform) {
	    Vector3    pos = aTransform.localPosition;
	    pos.z = 0f;
	    aTransform.localPosition = pos;
	    aTransform.localRotation = Quaternion.identity;
	}

    /// <summary>
    /// Performs variable reset afer the frame is finished
    /// </summary>
    void ResetVarsThisFrame()
    {
        //Reset item clicked variable
        ClickedThisFrame = false;
    }

    /// <summary>
    /// Move selected listbox item up
    /// </summary>
    public void MoveSelectedItem(int Amt)
    {
        //Get selected index
        int Selected_Index = GetSelectedIndex();

        if (Selected_Index > -1)
        {
            if (Selected_Index < optionStrings.Length)
            {
                //Get display text
                string ItemTxt = optionStrings[Selected_Index];

                //Get lisbox value
                object ItemVal = null;
                if (optionValues.Length < Selected_Index) ItemVal = optionValues[Selected_Index];

                //Insert new item
                AddOption(ItemTxt, ItemVal);

                //Remove selected
                RemoveOption(Selected_Index);

                //Refresh indices
                RefreshInts();
            }
        }

    }

    /// <summary>
    /// Get the actual option component at this index. Ignores option offset.
    /// </summary>
    /// <param name="Index"></param>
    /// <returns></returns>
    public GameObject GetOption(int Index)
    {
        return options[Index];
    }

}

