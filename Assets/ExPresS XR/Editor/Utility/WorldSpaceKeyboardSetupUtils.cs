
using System;
using System.Text.RegularExpressions;
using ExPresSXR.Misc;
using UnityEditor;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace ExPresSXR.UI
{
    public class WorldSpaceKeyboardSetupUtils : MonoBehaviour
    {
        /// <summary>
        /// Automatically tries to set up buttons, connecting them to the buttons click event (i.e. button up).
        /// Not recommended when using with poke as moving the finger of the button while pressed will invalidate the click.
        /// 
        /// Buttons should have the format `[Button|Key]_<letter>`, where <letter> is either a single character or one of [Space, Backspace, Confirm, Enter, Minus, Plus].
        /// Normal matches will add the <letter> (case-sensitive) to the text, the special strings implement the respective character or function.
        /// Errors will be printed (including a link to said button).
        /// </summary>
        [MenuItem("ExPresS XR/Auto Setup.../World Space Keyboard/Buttons on Click", false, 6)]
        private static void SetupButtonsAsClick(MenuCommand _) => SetupButtonsEventListeners(false);

        /// <summary>
        /// Automatically tries to set up buttons, connecting them to the buttons down event (i.e. button down).
        /// 
        /// Buttons should have the format `Button_<key>`, where key is either a single character or one of [Space, Backspace, Clear, Confirm, Enter, Minus, Plus].
        /// Normal matches will add the <ke<> (case-sensitive) to the text, the special strings implement the respective character or function.
        /// Errors will be printed (including a link to said button).
        /// </summary>
        [MenuItem("ExPresS XR/Auto Setup.../World Space Keyboard/Buttons on Down", false, 6)]
        private static void SetupButtonsAsDown(MenuCommand _) => SetupButtonsEventListeners(true);

        private static void SetupButtonsEventListeners(bool connectDown)
        {
            GameObject selectedGo = Selection.activeGameObject;
            if (selectedGo == null || !selectedGo.TryGetComponent(out WorldSpaceKeyboard keyboard))
            {
                Debug.LogError("Can not set up a WorldSpaceKeyboard since the Selection did not contain one. It must be a Component of the currently selected GameObject.");
                return;
            }

            Undo.SetCurrentGroupName("Bind Keyboard Buttons");
            int undoGroup = Undo.GetCurrentGroup();
            
            foreach (Button btn in keyboard.GetComponentsInChildren<Button>())
            {
                string letter = ParseLetterFromName(btn.name);
                if (letter == "")
                {
                    Debug.LogWarning($"Skipping setup for button {btn}, could not be parsed.", btn);
                    continue;
                }
                // Debug.Log($"Found button '{btn.name}' in keyboard '{keyboard.name}', trying to add callback for letter '{letter}'.");
                if (connectDown)
                {
                    CreateButtonDownEventListeners(keyboard, btn, letter);
                }
                else
                {
                    CreateButtonClickedEventListeners(keyboard, btn, letter);
                }
            }
            Debug.Log($"Completed setting up keyboard '{keyboard}' buttons with the button {(connectDown ? "down" : "click")}-event.");

            Undo.CollapseUndoOperations(undoGroup);
        }

        
        private static void CreateButtonClickedEventListeners(WorldSpaceKeyboard keyboard, Button btn, string letter)
        {
            if (AutoSetupUtils.HasEventPersistentListeners(btn.onClick))
            {
                Debug.LogWarning($"Button '{btn}' already has event persistent listeners. Skipping assuming it was already set up to avoid duplications.", btn);
                return;
            }

            Undo.RecordObject(btn, "Bind Keyboard Buttons");
            
            try
            {
                string lowerLetter = letter.ToLower();
                if (lowerLetter == "backspace")
                {
                    UnityEventTools.AddPersistentListener(btn.onClick, keyboard.RemoveLastFromText);
                }
                else if (lowerLetter == "clear")
                {
                    UnityEventTools.AddPersistentListener(btn.onClick, keyboard.ClearText);
                }
                else if (lowerLetter == "confirm" || lowerLetter == "enter")
                {
                    UnityEventTools.AddPersistentListener(btn.onClick, keyboard.ConfirmText);
                }
                else if (lowerLetter == "shift" || lowerLetter == "caps")
                {
                    // Connect to the caps button toggle instead if it is used to implement toggle mode
                    if (btn.TryGetComponent(out ButtonToggler toggler))
                    {
                        if (AutoSetupUtils.HasEventPersistentListeners(toggler.OnToggleChanged))
                        {
                            UnityEventTools.AddPersistentListener(toggler.OnToggleChanged, keyboard.ChangeCapsActive);
                        }
                        else
                        {
                            Debug.LogWarning($"Found persistent call for event 'OnToggleChanged' of ButtonToggler '{toggler}', "
                                + "skip connecting call to 'ChangeCapsActive' to prevent duplication.", toggler);
                        }
                    }
                    else
                    {
                        if (AutoSetupUtils.HasEventPersistentListeners(btn.onClick))
                        {
                            UnityEventTools.AddPersistentListener(btn.onClick, keyboard.ToggleCapsActive);
                        }
                        else
                        {
                            Debug.LogWarning($"Found persistent call for event 'onClick' of ButtonToggler '{btn}', "
                                + "skip connecting call to 'ToggleCapsActive' to prevent duplication.", toggler);
                        }
                    }
                }
                else if (lowerLetter.Length == 1)
                {
                    // Use case sensitive value of the letter
                    UnityEventTools.AddStringPersistentListener(btn.onClick, keyboard.AppendToText, letter);
                }
                else
                {
                    Debug.LogError($"Failed to decide a function button '{btn}' based on the letter '{letter}'.");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed add a persistent event listener for button '{btn}'. Failed with exception:\n{e}");
            }
            
            EditorUtility.SetDirty(btn);
            PrefabUtility.RecordPrefabInstancePropertyModifications(btn);
        }


        private static void CreateButtonDownEventListeners(WorldSpaceKeyboard keyboard, Button btn, string letter, EventTriggerType triggerType = EventTriggerType.PointerDown)
        {
            if (!btn.TryGetComponent(out EventTrigger trigger))
            {
                Undo.RecordObject(btn, "Add Event Trigger");
                // Debug.Log($"Button {btn} is supposed to be triggered on down and is missing the required 'EventTrigger'-Component. Adding it.");
                trigger = btn.gameObject.AddComponent<EventTrigger>();
            }

            Undo.RecordObject(btn, "Bind Keyboard Buttons");

            
            try
            {
                string lowerLetter = letter.ToLower();
                if (lowerLetter == "backspace")
                {
                    AutoSetupUtils.AddVoidPersistentTriggerCall(trigger, triggerType, keyboard.RemoveLastFromText);
                }
                else if (lowerLetter == "linebreak")
                {
                    AutoSetupUtils.AddVoidPersistentTriggerCall(trigger, triggerType, keyboard.AppendLineBreak);
                }
                else if (lowerLetter == "clear")
                {
                    AutoSetupUtils.AddVoidPersistentTriggerCall(trigger, triggerType, keyboard.ClearText);
                }
                else if (lowerLetter == "confirm" || lowerLetter == "enter")
                {
                    AutoSetupUtils.AddVoidPersistentTriggerCall(trigger, triggerType, keyboard.ConfirmText);
                }
                else if (lowerLetter == "shift" || lowerLetter == "caps")
                {
                    // Connect to the caps button toggle instead if it is used to implement toggle mode
                    if (btn.TryGetComponent(out ButtonToggler toggler))
                    {
                        // Use a normal event here as it is not affected like this
                        if (!AutoSetupUtils.HasEventPersistentListeners(toggler.OnToggleChanged))
                        {
                            UnityEventTools.AddPersistentListener(toggler.OnToggleChanged, keyboard.ChangeCapsActive);
                        }
                        else
                        {
                            Debug.LogWarning($"Found persistent call for event 'OnToggleChanged' of ButtonToggler '{toggler}', "
                                + "skip connecting call to 'ChangeCapsActive' to prevent duplication.", toggler);
                        }
                        // Also connect the toggler and button via button down
                        toggler.ConnectToClick = false;
                        AutoSetupUtils.AddVoidPersistentTriggerCall(trigger, triggerType, toggler.ToggleButton);
                    }
                    else
                    {
                        AutoSetupUtils.AddVoidPersistentTriggerCall(trigger, triggerType, keyboard.ToggleCapsActive);
                    }
                }
                else if (lowerLetter.Length == 1)
                {
                    // Use case sensitive value of the letter
                    AutoSetupUtils.AddStringPersistentTriggerCall(trigger, triggerType, keyboard.AppendToText, letter);
                }
                else
                {
                    Debug.LogError($"Failed to decide a function button '{btn}' based on the letter '{letter}'.");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed add a persistent event listener for button '{btn}'. Failed with exception:\n{e}");
            }
            
            EditorUtility.SetDirty(btn);
            PrefabUtility.RecordPrefabInstancePropertyModifications(btn);
        }

        
        private static string ParseLetterFromName(string buttonName)
        {
            // Matches either the "Button_" or "Key_" prefix (case insensitive).
            // Captures the characters after that as named group "letter".
            string pattern = @"^(?:button|key)_(\w+)$";
            Match match = Regex.Match(buttonName, pattern, RegexOptions.IgnoreCase);
            string rawMatch = match.Success ? match.Groups[1].Value : "";
            string lowerMatch = rawMatch.ToLower(); // Cast to lower fr easier comparison
            
            // We then check for special cases (space, comma, colon/dot, ...) for a simpler logic connecting the callbacks.
            return lowerMatch switch
            {
                "space" => " ",
                "newline" or "linebreak" => "linebreak",
                "comma" => ",",
                "dot" or "point" => ".",
                "plus" => "+",
                "minus" or "dash" => "-",
                "question" or "questionmark" => "!",
                "exclamation" or "exclamationmark" or "bang" => "!",
                "hash" or "hashtag" or "bang" => "#",
                "slash" or "divide" => "/",
                "multiply" or "star" or "asterisk" => "*",
                "colon" => ":",
                "euro" => "€",
                "dollar" => "$",
                "ue" => "Ü",
                "oe" => "Ö",
                "ae" => "Ä",
                "sz" or "sharps" or "schafs" or "eszett"=> "ß",
                _ => rawMatch
            };
        }
    }
}