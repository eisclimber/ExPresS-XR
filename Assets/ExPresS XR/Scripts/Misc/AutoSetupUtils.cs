using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace ExPresSXR.Misc
{
    /// <summary>
    /// Utility functions for automatic setups, mainly adding persistent listeners.
    /// </summary>
    public class AutoSetupUtils
    {
        /// <summary>
        /// Get or creates a trigger on an EventTrigger of an EventSystem used by Unity UI.
        /// </summary>
        /// <param name="trigger">Trigger to add the callback to.</param>
        /// <param name="type">Type of event to add the cal</param>
        public static EventTrigger.TriggerEvent GetOrCreateEventTriggerEvent(EventTrigger trigger, EventTriggerType type)
        {
            EventTrigger.Entry entry = trigger.triggers.Find(e => e.eventID == type);
            if (entry == null)
            {
                // Not entry exists => Add one
                entry = new()
                {
                    eventID = type
                };
                trigger.triggers.Add(entry);
            }
            return entry.callback;
        }

        /// <summary>
        /// Helper function for creating a persistent trigger call without parameters. 
        /// </summary>
        /// <param name="trigger">EventTrigger to modify.</param>
        /// <param name="triggerType">EventTriggerType to add the callback to.</param>
        /// <param name="callback">Void function call.</param>
        /// <param name="requireNoListeners">Will fail if there are already persistent listeners.</param>
        public static void AddVoidPersistentTriggerCall(EventTrigger trigger, EventTriggerType triggerType, UnityAction callback, bool requireNoListeners = true)
        {
            try 
            {
                EventTrigger.TriggerEvent triggerEvent = GetOrCreateEventTriggerEvent(trigger, triggerType);
                if (requireNoListeners && HasEventPersistentListeners(triggerEvent))
                {
                    Debug.LogWarning($"A persistent listener already exists for trigger event '{triggerEvent}'.", trigger);
                    return;
                }
                UnityEditor.Events.UnityEventTools.AddVoidPersistentListener(triggerEvent, callback);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to create persistent listen for trigger '{triggerType}' of EventTrigger '{trigger}' with call '{nameof(callback)}'. Error was:\n{e}");
            }
        }

        /// <summary>
        /// Helper function for creating a persistent trigger call with a bool parameter. 
        /// </summary>
        /// <param name="trigger">EventTrigger to modify.</param>
        /// <param name="triggerType">EventTriggerType to add the callback to.</param>
        /// <param name="callback">Bool function call.</param>
        /// <param name="argument">Bool passed as argument.</param>
        /// <param name="requireNoListeners">Will fail if there are already persistent listeners.</param>
        public static void AddPersistentTriggerCall(EventTrigger trigger, EventTriggerType triggerType, UnityAction<bool> callback, bool argument, bool requireNoListeners = true)
        {
            try 
            {
                EventTrigger.TriggerEvent triggerEvent = GetOrCreateEventTriggerEvent(trigger, triggerType);
                if (requireNoListeners && HasEventPersistentListeners(triggerEvent))
                {
                    Debug.LogWarning($"A persistent listener already exists for trigger event '{triggerEvent}'.", trigger);
                    return;
                }
                UnityEditor.Events.UnityEventTools.AddBoolPersistentListener(triggerEvent, callback, argument);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to create persistent listen for trigger '{triggerType}' of EventTrigger '{trigger}' with call '{nameof(callback)}' and bool argument '{argument}'. Error was:\n{e}");
            }
        }

        /// <summary>
        /// Helper function for creating a persistent trigger call with a float parameter. 
        /// </summary>
        /// <param name="trigger">EventTrigger to modify.</param>
        /// <param name="triggerType">EventTriggerType to add the callback to.</param>
        /// <param name="callback">Float function call.</param>
        /// <param name="argument">Float passed as argument.</param>
        /// <param name="requireNoListeners">Will fail if there are already persistent listeners.</param>
        public static void AddPersistentTriggerCall(EventTrigger trigger, EventTriggerType triggerType, UnityAction<float> callback, float argument, bool requireNoListeners = true)
        {
            try 
            {
                EventTrigger.TriggerEvent triggerEvent = GetOrCreateEventTriggerEvent(trigger, triggerType);
                if (requireNoListeners && HasEventPersistentListeners(triggerEvent))
                {
                    Debug.LogWarning($"A persistent listener already exists for trigger event '{triggerEvent}'.", trigger);
                    return;
                }
                UnityEditor.Events.UnityEventTools.AddFloatPersistentListener(triggerEvent, callback, argument);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to create persistent listen for trigger '{triggerType}' of EventTrigger '{trigger}' with call '{nameof(callback)}' and float argument '{argument}'. Error was:\n{e}");
            }
        }

        /// <summary>
        /// Helper function for creating a persistent trigger call with a int parameter. 
        /// </summary>
        /// <param name="trigger">EventTrigger to modify.</param>
        /// <param name="triggerType">EventTriggerType to add the callback to.</param>
        /// <param name="call">Int function call.</param>
        /// <param name="argument">Int passed as argument.</param>
        /// <param name="requireNoListeners">Will fail if there are already persistent listeners.</param>
        public static void AddIntPersistentTriggerCall(EventTrigger trigger, EventTriggerType triggerType, UnityAction<int> callback, int argument, bool requireNoListeners = true)
        {
            try 
            {
                EventTrigger.TriggerEvent triggerEvent = GetOrCreateEventTriggerEvent(trigger, triggerType);
                if (requireNoListeners && HasEventPersistentListeners(triggerEvent))
                {
                    Debug.LogWarning($"A persistent listener already exists for trigger event '{triggerEvent}'.", trigger);
                    return;
                }
                UnityEditor.Events.UnityEventTools.AddIntPersistentListener(triggerEvent, callback, argument);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to create persistent listen for trigger '{triggerType}' of EventTrigger '{trigger}' with call '{nameof(callback)}' and int argument '{argument}'. Error was:\n{e}");
            }
        }


        /// <summary>
        /// Helper function for creating a persistent trigger call with a string parameter. 
        /// </summary>
        /// <param name="trigger">EventTrigger to modify.</param>
        /// <param name="triggerType">EventTriggerType to add the callback to.</param>
        /// <param name="callback">String function call.</param>
        /// <param name="argument">String passed as argument.</param>
        /// <param name="requireNoListeners">Will fail if there are already persistent listeners.</param>
        public static void AddStringPersistentTriggerCall(EventTrigger trigger, EventTriggerType triggerType, UnityAction<string> callback, string argument, bool requireNoListeners = true)
        {
            try 
            {
                EventTrigger.TriggerEvent triggerEvent = GetOrCreateEventTriggerEvent(trigger, triggerType);
                if (requireNoListeners && HasEventPersistentListeners(triggerEvent))
                {
                    Debug.LogWarning($"A persistent listener already exists for trigger event '{triggerEvent}'.", trigger);
                    return;
                }
                UnityEditor.Events.UnityEventTools.AddStringPersistentListener(triggerEvent, callback, argument);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to create persistent listen for trigger '{triggerType}' of EventTrigger '{trigger}' with call '{nameof(callback)}' and string argument '{argument}'. Error was:\n{e}");
            }
        }

        /// <summary>
        /// Helper function for creating a persistent trigger call with an Unity.Object parameter. 
        /// </summary>
        /// <param name="trigger">EventTrigger to modify.</param>
        /// <param name="triggerType">EventTriggerType to add the callback to.</param>
        /// <param name="callback">Void function call.</param>
        /// <param name="argument">UnityEngine.Object passed as argument.</param>
        /// <param name="requireNoListeners">Will fail if there are already persistent listeners.</param>
        public static void AddObjectPersistentTriggerCall<T>(EventTrigger trigger, EventTriggerType triggerType, UnityAction<UnityEngine.Object> callback, UnityEngine.Object argument, bool requireNoListeners = true) where T : UnityEngine.Object
        {
            try 
            {
                EventTrigger.TriggerEvent triggerEvent = GetOrCreateEventTriggerEvent(trigger, triggerType);
                if (requireNoListeners && HasEventPersistentListeners(triggerEvent))
                {
                    Debug.LogWarning($"A persistent listener already exists for trigger event '{triggerEvent}'.", trigger);
                    return;
                }
                UnityEditor.Events.UnityEventTools.AddObjectPersistentListener(triggerEvent, callback, argument);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to create persistent listen for trigger '{triggerType}' of EventTrigger '{trigger}' with call '{nameof(callback)}' and UnityEngine.Object argument '{argument}'. Error was:\n{e}");
            }
        }

        /// <summary>
        /// Check whether a persistent listener to any function exists in an event. 
        /// </summary>
        /// <param name="evnt">Event to check.</param>
        /// <returns>A persistent listener exists.</returns>
        public static bool HasEventPersistentListeners(UnityEvent evnt) => evnt.GetPersistentEventCount() > 0;


        /// <summary>
        /// Check whether a persistent listener to any function exists in an event. 
        /// </summary>
        /// <param name="evnt">Event to check.</param>
        /// <returns>A persistent listener exists.</returns>
        public static bool HasEventPersistentListeners<T>(UnityEvent<T> evnt) => evnt.GetPersistentEventCount() > 0;
    }
}