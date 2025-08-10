using System;
using System.Runtime.InteropServices;
using AOT;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using Unity.VisualScripting;
using UnityEngine;
using Debug = UnityEngine.Debug;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace Scripts.Audio
{
    public class DialogueAudioScript : MonoBehaviour
    {
        EVENT_CALLBACK dialogueCallback;

        public EventReference EventName;
        public EventInstance DialogueInstance { get; private set; }


        public static DialogueAudioScript Instance;

#if UNITY_EDITOR
        void Reset()
        {
            EventName = EventReference.Find("event:/Dialog/DIALOG_PI");
        }
#endif

        void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        void Start()
        {
            // Explicitly create the delegate object and assign it to a member so it doesn't get freed
            // by the garbage collected while it's being used
            dialogueCallback = DialogueEventCallback;
        }

        public void PlayDialogue(string _key)
        {
            DialogueInstance.getPlaybackState(out var state);
            if (state == PLAYBACK_STATE.PLAYING)
            {
                DialogueInstance.stop(STOP_MODE.IMMEDIATE);
            }

            DialogueInstance = RuntimeManager.CreateInstance(EventName);

            // Pin the key string in memory and pass a pointer through the user data
            var stringHandle = GCHandle.Alloc(_key);
            DialogueInstance.setUserData(GCHandle.ToIntPtr(stringHandle));

            DialogueInstance.setCallback(dialogueCallback);
            DialogueInstance.start();
            if (DialogueInstance.isValid()) DialogueInstance.release();
        }

        [MonoPInvokeCallback(typeof(EVENT_CALLBACK))]
        static RESULT DialogueEventCallback(EVENT_CALLBACK_TYPE _type, IntPtr _instancePtr, IntPtr _parameterPtr)
        {
            var instance = new EventInstance(_instancePtr);

            // Retrieve the user data
            instance.getUserData(out var stringPtr);

            // Get the string object
            var stringHandle = GCHandle.FromIntPtr(stringPtr);
            string key = stringHandle.Target as string;

            switch (_type)
            {
                case EVENT_CALLBACK_TYPE.CREATE_PROGRAMMER_SOUND:
                {
                    const MODE soundMode = MODE.LOOP_NORMAL | MODE.CREATECOMPRESSEDSAMPLE | MODE.NONBLOCKING;
                    var parameter = (PROGRAMMER_SOUND_PROPERTIES)Marshal.PtrToStructure(_parameterPtr, typeof(PROGRAMMER_SOUND_PROPERTIES));

                    if (key != null && key.Contains("."))
                    {
                        var soundResult = RuntimeManager.CoreSystem.createSound(Application.streamingAssetsPath + "/" + key, soundMode, out var dialogueSound);
                        if (soundResult == RESULT.OK)
                        {
                            parameter.sound = dialogueSound.handle;
                            parameter.subsoundIndex = -1;
                            Marshal.StructureToPtr(parameter, _parameterPtr, false);
                        }
                    }
                    else
                    {
                        var keyResult = RuntimeManager.StudioSystem.getSoundInfo(key, out var dialogueSoundInfo);
                        if (keyResult != RESULT.OK)
                        {
                            break;
                        }

                        var soundResult = RuntimeManager.CoreSystem.createSound(dialogueSoundInfo.name_or_data, soundMode | dialogueSoundInfo.mode, ref dialogueSoundInfo.exinfo, out var dialogueSound);
                        if (soundResult == RESULT.OK)
                        {
                            
                            parameter.sound = dialogueSound.handle;
                            parameter.subsoundIndex = dialogueSoundInfo.subsoundindex;
                            Marshal.StructureToPtr(parameter, _parameterPtr, false);
                        }
                    }

                    break;
                }
                case EVENT_CALLBACK_TYPE.DESTROY_PROGRAMMER_SOUND:
                {
                    var parameter = (PROGRAMMER_SOUND_PROPERTIES)Marshal.PtrToStructure(_parameterPtr, typeof(PROGRAMMER_SOUND_PROPERTIES));
                    var sound = new Sound(parameter.sound);
                    if (sound.hasHandle()) sound.release();

                    break;
                }
                case EVENT_CALLBACK_TYPE.DESTROYED:
                {
                    // Now the event has been destroyed, unpin the string memory so it can be garbage collected
                    stringHandle.Free();

                    break;
                }
            }

            return RESULT.OK;
        }
    }
}