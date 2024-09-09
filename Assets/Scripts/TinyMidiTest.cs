using UnityEngine;

public class TinyMidiTest : MonoBehaviour
{

    private void Start()
    {
        TinyMidi.Input.Setup();

        TinyMidi.Input.AddEventListener(midiEvent =>
        {
            switch (midiEvent.status)
            {
                case TinyMidi.MidiEventStatus.NoteOn:
                case TinyMidi.MidiEventStatus.NoteOff:
                    Debug.Log(
                        $"Note Down: {midiEvent.controllerNumber}" +
                        $" Velocity: {midiEvent.value}" +
                        $" Device Index: {midiEvent.deviceIndex}");

                    break;
                case TinyMidi.MidiEventStatus.ControlChange:
                    Debug.Log(
                        $"Control Changed: {midiEvent.controllerNumber}" +
                        $" Value: {midiEvent.value}" +
                        $" Device Index: {midiEvent.deviceIndex}");

                    break;
            }
        });

        TinyMidi.Input.Start();
    }

    private void OnDestroy()
    {
        TinyMidi.Input.Stop();
    }

}
