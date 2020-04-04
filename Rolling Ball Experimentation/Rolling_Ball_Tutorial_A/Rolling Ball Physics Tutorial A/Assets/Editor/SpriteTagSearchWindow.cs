using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

public class SpriteTagWindow : EditorWindow
{
  float myRandMin = 0.0f;
  float myRandMax = 100.0f;
  float myRandNumber = 0f;
  int myRandIntegerFlagInt = 0;
  string mySearchString = null;
  string mySearchResultFinding = null;
  string[] pathList = null;
  string[] results = null;
  Vector2 scrollPosition;
  public string simNameText = "Results";
  string resultText = "Search results go here.";
  string myFinalizedList = "";
  string myCompleteSearchString = null;
  string mySearchPackingTag = null;
  string myReplacementPackingTag = null;
  string myFoundPackingTag = null;
  string[] myCulledPaths = null;
  int myMatches = 0;
  string resultsFoundText = null;



  [MenuItem( "Huxter/Sprite Tag Search" )]
  // Use this for initialization
  public static void ShowWindow()
  {
    EditorWindow.GetWindow( typeof( SpriteTagWindow ) );
  }

  void OnGUI()
  {
    this.minSize = new Vector2( 205, 205 );

    if ( GUI.Button( new Rect( 5, 5, 250, 16 ), "Search Sprites Containing Text ->" ) )
    {
      SearchSprites( false );
    }
    mySearchString = EditorGUI.TextField( new Rect( 260, 5, position.width - 285, 16 ), "", mySearchString );
    if ( GUI.Button( new Rect( position.width - 20, 5, 16, 16 ), "?" ) )
    {
      HelpSearchSprites();
      //EditorGUI.HelpBox( new Rect ( 20, 20,  40, 40 ), "This message.", MessageType.Info );
    }

    if ( GUI.Button( new Rect( 5, 25, 250, 16 ), "Search Sprites Matching Packing Tag ->" ) )
    {
      SearchSprites( true );
    }
    mySearchPackingTag = EditorGUI.TextField( new Rect( 260, 25, position.width - 285, 16 ), "", mySearchPackingTag );
    if ( GUI.Button( new Rect( position.width - 20, 25, 16, 16 ), "?" ) )
    {
      HelpPackingTag();
    }

    if ( GUI.Button( new Rect( 5, 45, 250, 16 ), "Replace Search Tag With New Tag ->" ) )
    {
      ReplacePackingTags();
    }
    myReplacementPackingTag = EditorGUI.TextField( new Rect( 260, 45, position.width - 285, 16 ), "", myReplacementPackingTag );
    if ( GUI.Button( new Rect( position.width - 20, 45, 16, 16 ), "?" ) )
    {
      HelpReplacePackingTag();
    }



    EditorGUI.LabelField( new Rect( 3, 65, position.width - 8, 16 ), "Search Result: " + mySearchResultFinding );

    // THE SCROLL WINDOW! 
    bool f = false;
    Rect chatWindow_Rect = new Rect( 0, 80, position.width - 5, position.height - 150 );
    GUILayout.BeginArea( chatWindow_Rect );
    scrollPosition = GUILayout.BeginScrollView( scrollPosition, f, true );
    resultText = EditorGUILayout.TextArea( resultText );
    //xmlText = EditorGUI.TextArea(chatWindow_Rect, xmlText);
    GUILayout.EndScrollView();
    GUILayout.EndArea();
    // END SCROLL WINDOW!



    //Random Number Selector Area - Huxter experimenting with tool code. 
    if ( GUI.Button( new Rect( 5, position.height - 40, position.width - 6, 16 ), "Print A Random Number" ) )
    {
      PrintRandomNumber();
    }
    EditorGUI.LabelField( new Rect( 3, position.height - 20, 65, 16 ), "Rand Min:" );
    myRandMin = EditorGUI.FloatField( new Rect( 68, position.height - 20, 60, 16 ), myRandMin );

    EditorGUI.LabelField( new Rect( 134, position.height - 20, 65, 16 ), "Rand Max:" );
    myRandMax = EditorGUI.FloatField( new Rect( 194, position.height - 20, 60, 16 ), myRandMax );

    //Toggle is not working. The button box never changes, and is only ever populated with the current value. Unless you shut down the window and open it again.
    //myRandIntegerFlag = EditorGUI.Toggle( new Rect(305, 205, position.width, 0), "Integer", myRandIntegerFlag );

    EditorGUI.LabelField( new Rect( 260, position.height - 20, position.width, 16 ), "Rand Type (0 = Int, 1 = Float):" );
    myRandIntegerFlagInt = EditorGUI.IntField( new Rect( 450, position.height - 20, 20, 20 ), myRandIntegerFlagInt );

    EditorGUI.LabelField( new Rect( 490, position.height - 20, position.width, 16 ), "Result: " + myRandNumber );
  }



  void SearchSprites( bool findTags )
  {

    // myCompleteSearchString is the search string the user inputs, with t: Texture appended, so we know we are searching ONLY textures.
    myCompleteSearchString = "";
    mySearchResultFinding = "0 Results Found";
    pathList = null;
    Array.Resize( ref pathList, 0 );


    if ( mySearchString == "" )
    {
      Debug.Log( "The search field is empty." );
      return;
    }
    else
    {
      myCompleteSearchString = mySearchString + " t:Texture";
      Debug.Log( "Your Completed Search String was: " + myCompleteSearchString );

      results = AssetDatabase.FindAssets( myCompleteSearchString ); // results array is filled with all found assets using the search term now in myCompleteSearchString
      resultsFoundText = " Results Found."; // Setting up the results text by making it default to x "Results Found."
      if ( results.Length == 1 )
        resultsFoundText = " Result Found."; // And as a stickler, I want a result of 1 to say "1 Result Found." without the plural "s".
      mySearchResultFinding = results.Length + resultsFoundText; // The result finding text is the number of results, with that string added.
      Debug.Log( "I found " + results.Length + " Results Before Determining If Any Are Non-Sprites." );

      if ( results.Length == 0 ) // If there are no results
      {
        Debug.LogError( "Can't find any items named " + mySearchString );
        mySearchResultFinding = "No Results Found."; // Make the reporting text say so.
        resultText = "No Results Found."; // And make the text that's in the scrollable window ALSO say so.
        return;
      }

      //REMOVE ALL THIS LATER
      Debug.Log( "\n\n\nLength of Search array: " + results.Length );
      int resultsLast = results.Length - 1; // resultsLast is an index into the array, pointed at the final item in it, so I can actually test whether or not I have the array right

      Debug.Log( "The first item in the array is: " + AssetDatabase.GUIDToAssetPath( results[ 0 ] ) );
      Debug.Log( "The last item in the array is at position " + resultsLast + " which is " + AssetDatabase.GUIDToAssetPath( results[ resultsLast ] ) );
      Debug.Log( "\n" );
      // END INITIAL FIND DEBUG REPORTING

      // This section will attempt to clear out a new array "pathList" and then copy the results -> path into pathList. To do this, I resize the new array to 0 first.
      //Array.Resize( ref pathList, 0 );





      // Then we go through results, get the GUID there, and for now, just add to the length of pathList array, and tell us what we ended up with.
      // **** HERE! I think if after I find the path, I put it through AssetImporter to test for null TextureType, I skip that i and go to the next one
      // **** IF NOT, then I should do a second loop after this, removing the offending Render Textures
      for ( int i = 0; i < results.Length; i++ )
      {
        Array.Resize( ref pathList, pathList.Length + 1 ); // Extend the array size by 1 each time
        pathList.SetValue( AssetDatabase.GUIDToAssetPath( results[ i ] ), i ); // Fill that array space with the result path in the same index in the results array
        Debug.Log( "Index = " + i + ", Path = " + pathList[ i ] );
      }
      Debug.Log( "\n" );






      // Call function to cull the render textures out
      cullRenderTextures();






      // Let's see the culled down list
      convertPathsToString();





      // Trying to use Jessery's advice to find out if an item is a render texture
      // So my strategy is to go through the search results and do a FIRST culling searching on whether or not it's a render texture, BEFORE I then cull it out a second time.
      //TextureImporter textureImporter = AssetImporter.GetAtPath( pathList[ 0 ] ) as TextureImporter;
      //if ( textureImporter == null )
      //  Debug.Log( "\n>>>>>>>> FIRST ITEM IS LIKELY A RENDER TEXTURE!" );
      //else
      //  Debug.Log( "\n<<<<<<<<< FIRST ITEM IS LIKELY A SPRITE TEXTURE!" );





      // Go through the list of results and print out the packing tag of each texture - THIS WILL GO AWAY LATER BECAUSE IT'S JUST DEBUG REPORTING
      // ***********************************************************************
      // THIS SECTION HAS HUGE PROBLEM! I GET THIS ERROR WHEN "banner" IS THE SEARCH TEXT! Some texture with "banner" in it is failing this, perhaps because it's not a Sprite?
      // ***********************************************************************
      // This section just logs the packing tags of the entire list of paths, unculled. This will go away.
      int numMatches = 0;
      //
      for ( int i = 0; i < pathList.Length; i++ )
      {
        Debug.Log( "\n\n ************** HERE IS A PROBLEM! pathList[" + i + "] is causing issues." );
        TextureImporter myTexture = ( TextureImporter )TextureImporter.GetAtPath( pathList[ i ] );
        //TextureImporter myTexture = AssetImporter.GetAtPath( pathList[ i ] ) as TextureImporter;
        myFoundPackingTag = myTexture.spritePackingTag;
        if ( myFoundPackingTag == mySearchPackingTag )
        {
          Debug.Log( " A MATCH!!! " );
          numMatches++;
        }
        if ( myFoundPackingTag == "" )
          myFoundPackingTag = " --- NON EXISTENT --- ";
        Debug.Log( "Packing Tag of item " + i + ", " + pathList[ i ] + " is " + myFoundPackingTag );
      }
      Debug.Log( "\nNumber of matches - " + numMatches );





      // Find out if any are not sprites.
      // HOW is not quite sure. I can try to find out if it's TextureImporterType.Sprite is false...
















      // Optionally, cull down the list based on spriteTag match

      if ( findTags )
      {
        myCulledPaths = null;
        myMatches = 0;
        // Then we go through path results, compare to the Sprite Tag you specified, and add only those that match
        for ( int i = 0; i < pathList.Length; i++ )
        {
          TextureImporter myTexture = ( TextureImporter )TextureImporter.GetAtPath( pathList[ i ] );
          myFoundPackingTag = myTexture.spritePackingTag;

          if ( myFoundPackingTag == mySearchPackingTag )
          {
            Array.Resize( ref myCulledPaths, myMatches + 1 ); // Extend the array size by 1 each time
            myCulledPaths.SetValue( pathList[ i ], myMatches ); // Fill that array space with the result path in the same index in the results array
            Debug.Log( "MATCH!!! Culled Paths Index = " + myMatches + ", and Culled Path = " + myCulledPaths[ myMatches ] );
            myMatches++;
          }
          else
            Debug.Log( "No match at " + i + " so skipping." );
        }
        Debug.Log( " *** Ending up with a culled list of " + myMatches + " items." );


        resultsFoundText = " Results Found.";
        if ( myMatches == 0 )
        {
          mySearchResultFinding = "No matches found.";
          resultText = "No Results Found.";
          return;
        }
        if ( myMatches == 1 )
          resultsFoundText = " Result Found."; // And as a stickler, I want a result of 1 to say "1 Result Found." without the plural "s".

        mySearchResultFinding = myMatches + resultsFoundText; // The result finding text is the number of results, with that string added.
        Debug.Log( "\n---- CULLED RESULTS: " + myMatches + " matches found." );

        // Move the culled list to the real list for further treatment.
        pathList = myCulledPaths;

      }









      // MAKE THE RESULTS INTO A SINGLE LONG STRING
      // Now I'm going to append all those individual string paths into one very long string separated by "\n" so I get one long string that can display in lines.
      Debug.Log( "\nStarting to append the culled paths to a long string." );

      // Move the culled list to the real list for further treatment.
      convertPathsToString();


      // Select the top item, just as a test to ensure selecting something in the array works. This will go away later.
      Debug.Log( "Selecting the top object in Results: " + pathList[ 0 ] );
      Selection.activeObject = AssetDatabase.LoadMainAssetAtPath( pathList[ 0 ] );



      // Make yet another string array in which we will put a refined result of any texture in the search that has a SpriteTag in it - at all...
      // Then change that to refine the search by a user-specified sprite tag, with proper errors if it finds none, and display the list of those with that specific tag in the window
      // Then add a new field that asks for a replacement text, with a button to go through that list and replace the SpriteTag with the user text. This will APPLY the changes - which begs the question. Does it auto checkout? Or do I have to do that?
      // Then add a button to empty the sprite tag, since we sometimes want to remove that sprite tag
      // Stretch goal: Instead of a scrollable list, make a scrollable list of BUTTONS with the sprite name only (delimit the path) and make selecting it SELECT that sprite in the Project view
      // Even more stretch: Make the damn project window select ALL of the sprites in my finalized list.
      // Now refine the search to everything if no directory is selected, but only from that dir down, if one IS selected

    }
  }



  void ReplacePackingTags()
  {
    SearchSprites( true );
    if ( mySearchPackingTag != myReplacementPackingTag )
    {
      mySearchResultFinding = "Searching for Sprites with Packing Tag '" + mySearchPackingTag + "' and Replacing With New Packing Tag '" + myReplacementPackingTag + "'.";
      for ( int i = 0; i < pathList.Length; i++ )
      {
        TextureImporter ti = ( TextureImporter )TextureImporter.GetAtPath( pathList[ i ] );
        ti.spritePackingTag = myReplacementPackingTag;
        AssetDatabase.ImportAsset( pathList[ i ], ImportAssetOptions.ForceUpdate );
      }
      Debug.Log( "\n" );
      Debug.Log( "!!!!!!!!!!!!!! Beginning to replace all results that match '" + mySearchPackingTag + "' with new Packing Tag: '" + myReplacementPackingTag + "'.\n" );
      mySearchPackingTag = myReplacementPackingTag;
      SearchSprites( true );
    }
    else
    {
      mySearchResultFinding = "You are trying to replace with the same Packing Tag! NO REPLACEMENT ATTEMPTED.";
    }
  }



  void convertPathsToString()
  {

    resultText = "";
    myFinalizedList = "";

    for ( int i = 0; i < pathList.Length; i++ )
    {
      myFinalizedList += pathList[ i ];
      myFinalizedList += "   >   ";
      TextureImporter myTexture = ( TextureImporter )TextureImporter.GetAtPath( pathList[ i ] );
      myFinalizedList += myTexture.spritePackingTag;
      myFinalizedList += "\n";

    }
    // Print it out - BUT NOTE THAT THE DEBUG.LOG DOES NOT SHOW MORE THAN THE FIRST LINE AND I DO NOT KNOW WHY! However, the actual string is fine, and displays in the window ok
    Debug.Log( "And this is the length of the new string: " + myFinalizedList.Length );

    // Make sure that if this string is too long, rather than put it in the window, print an error.
    if ( myFinalizedList.Count() > 51000 )
    {
      //resultText = "List too long to fit in window. Narrow your search.";
      Debug.Log( "List too long to fit in window. Narrow your search." );
      myFinalizedList = myFinalizedList.Substring( 0, 51000 );
      resultText = "Results too long to print here, so last line may be truncated, and some results won't print at all:\n\n" + myFinalizedList;
      resultText = resultText + "...";
    }
    else
    {
      resultText = myFinalizedList; // Otherwise put the combined string into resultText which is what we display in the scroll window
    }
  }



  // NOW CULL THE INITIAL SEARCH LIST BY REMOVING RENDER TEXTURES (textures with no importer)
  void cullRenderTextures()
  {

   // Cull down the list, removing any Render Textures

    myCulledPaths = null;
    int pathIndex = 0;
    // Cycle through the paths and determine if any are Render Textures, and DO NOT ADD THOSE
    for ( int i = 0; i<pathList.Length; i++ )
    {
      TextureImporter myImporter = AssetImporter.GetAtPath( pathList[ i ] ) as TextureImporter;
      if ( myImporter == null )
        Debug.Log( " ************************* RENDER TEXTURE - DO NOT ADD!" );
      else
      {
        Array.Resize( ref myCulledPaths, pathIndex + 1 ); // Extend the array size by 1 each time
        myCulledPaths.SetValue( pathList[ i ], pathIndex ); // Fill that array space with the result path in the same index in the results array
        Debug.Log( "MATCH!!! Culled Paths Index = " + pathIndex + ", and Culled Path = " + myCulledPaths[ pathIndex ] );
        pathIndex++;
      }
    }
    if ( pathIndex == 0 )
    {
      resultText = "No Results Found.";
      mySearchResultFinding = "No Results Found.";
      Debug.Log( "******************All results were Render Textures!!!" );
      return;
    }
    else
      Debug.Log( "****** And the final result is " + pathIndex + " paths after Render Textures were culled out." );
    pathList = myCulledPaths;


    resultsPluralization();

  }



  void resultsPluralization()
  {
    resultsFoundText = " Results Found.";
    if ( pathList.Length == 1 )
      resultsFoundText = " Result Found.";
    mySearchResultFinding = pathList.Length + resultsFoundText;
  }



  void HelpSearchSprites()
  {
    resultText = "\n\n      Search Sprites Containing Text ->\n\n  Search ALL textures in the Project whose names contain your Search String, and that have Importers.\n  This does not necessarily mean Sprites.\n  But it does disregard Render Textures which do not have Importers, and were crashing the tool.\n  These results will be ALL TEXTURES with your string in the name.\n\n";
    Debug.Log( "\n HELP TEXT HERE - Search All Textures That Have Importers. This does not necessarily mean sprites. But it does disregard Render Textures." );
  }

  void HelpPackingTag()
  {
    resultText = "\n\n      Search Sprites Matching Packing Tag ->\n\n  Search ALL textures in the Project whose names contain your Search String\n  AND have a packing tag that matches Your Search Packing Tag.\n  This should guarantee Sprites becuase only Sprites have Packing Tags.\n  It disregards Render Textures too, which do not have packing tags.\n\n";
    Debug.Log( "\n HELP TEXT HERE - Search above results and return ONLY those whose packing tags match your input tag." );
  }


  void HelpReplacePackingTag()
  {
    resultText = "\n\n      Replace Search Tag With New Tag ->\n\n  This button searches through the Project for all Sprites whose names contain your Search String\n  AND match your Packing Tag, and will check those files out, replacing the Packing Tag with your New Packing Tag.\n\n";
    Debug.Log( "\n HELP TEXT HERE - Replace the Packing Tag given with New Packing Tag, ONLY in selection where the tag matches." );
  }


  void PrintRandomNumber()
  {
    
    myRandNumber = ((( myRandMax - myRandMin ) * UnityEngine.Random.value ) + myRandMin );
    if ( myRandIntegerFlagInt == 0 )
    {
      myRandNumber = ( int )myRandNumber;
      Debug.Log( "Random Number" + myRandNumber );
    }
    else
      Debug.Log( "Random Number " + myRandNumber );
  }

}


